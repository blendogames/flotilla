#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
#endregion

namespace SpaceShooter
{
    public class MainMenu : SysMenu
    {
        bool trialMenu = false;

        public MainMenu()
        {
            canBeExited = false;

            MenuItem item = new MenuItem(string.Format(Resource.MenuCampaign, FrameworkCore.adventureNumber));
            item.Selected += OnSelectCampaign;
            base.AddItem(item);

            item = new MenuItem(Resource.MenuHighScoresTitle);
            item.Selected += OnSelectHighScores;
            base.AddItem(item);

            item = new MenuItem(Resource.MenuSkirmish);
            item.Selected += OnSelectSkirmish;
            base.AddItem(item);

            item = new MenuItem(Resource.HelpAndOptions);
            item.Selected += OnSelectOptions;
            base.AddItem(item);

            if (FrameworkCore.isTrialMode())
            {
                trialMenu = true;

                item = new MenuItem(Resource.MenuUnlockFullGame);
                item.Selected += OnBuyGame;
                base.AddItem(item);
            }

#if WINDOWS && !ONLIVE
            item = new MenuItem(Resource.MenuBugReport);
            item.Selected += OnBugReport;
            base.AddItem(item);
#endif

            item = new MenuItem(Resource.MenuQuit);
            item.Selected += OnSelectExit;
            base.AddItem(item);

            UpdateItemPositions();
        }

        private void OnSelectExit(object sender, EventArgs e)
        {
            //quit.
            if (FrameworkCore.isTrialMode())
                Owner.AddMenu(new SellScreen());
            else
                FrameworkCore.Game.Exit();
        }


        private void OnBuyGame(object sender, EventArgs e)
        {
            FrameworkCore.BuyGame();
        }

#if WINDOWS
        private void OnBugReport(object sender, EventArgs e)
        {
            Owner.AddMenu(new BugReport());
        }
#endif

        private void OnSelectHighScores(object sender, EventArgs e)
        {
            Owner.AddMenu(new HighScoreMenu());
        }

        private void OnSelectCampaign(object sender, EventArgs e)
        {
            Owner.AddMenu(new CampaignMenu());
        }

        private void OnSelectSkirmish(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new SkirmishMenu());
        }

        private void OnSelectOptions(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new HelpLobby());
        }



        int LINESIZE = 0;

        private void UpdateItemPositions()
        {
            Vector2 startPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width * 0.1f,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.5f);

            startPos.Y -= 64;

            Vector2 textVec = menuFont.MeasureString("Sample");
            LINESIZE = (int)textVec.Y;
            int gapSize = (int)textVec.Y + 8;
            
            for (int i = 0; i < menuItems.Count; i++)
            {
                menuItems[i].position = startPos + new Vector2(0, i * gapSize);
            }
        }




        /// <summary>
        /// Remove the "unlock full game" option if it's still on the screen.
        /// </summary>
        private void TrialCheck()
        {
            if (!trialMenu)
                return;

            if (FrameworkCore.isTrialMode())
                return;

            for (int x = menuItems.Count - 1; x >= 0; x--)
            {
                if (menuItems[x].text == Resource.MenuUnlockFullGame)
                {
                    menuItems.RemoveAt(x);
                }
            }

            //default the cursor to the first item.
            selectedItem = menuItems[0];

            trialMenu = false;
        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            //iterate through list. if the "buy game" option is on the screen && !trialmode, then remove the option.
            TrialCheck();
#if WINDOWS
            if (Transition >= 1)
            {
                bool mouseHovering;
                MenuItem mouseSelectedItem;
                Helpers.UpdateTiltedMouseMenu(menuItems, inputManager.mousePos, -0.15f,
                    true,
                    new Point(0, 0),
                    FrameworkCore.SerifBig,
                    false,
                    inputManager,
                    selectedItem,
                    out mouseHovering, out mouseSelectedItem);

                if (inputManager.mouseHasMoved)
                {
                    selectedItem = mouseSelectedItem;
                }

                if (mouseHovering && inputManager.mouseLeftClick && selectedItem != null)
                {
                    base.ActivateItem(inputManager);
                }
            }
#endif
            base.Update(gameTime, inputManager);
        }



        

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {
            base.Activate();
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = menuItems[0].position;
            //pos.X += Helpers.PopLerp(Transition, -100,30,0);
            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");

            pos.Y -= titleVec.Y + LINESIZE + 8;

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), Transition);

            float modifier = Helpers.PopLerp(Transition, -200, 50, 0);
            pos.X += modifier;

            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuTitle, pos, titleColor, darkColor,
                -0.15f, Vector2.Zero, 1);

            DrawSignedIn(pos, titleColor, darkColor);

            drawitems(gameTime, modifier);

            DrawCopyright();

            FrameworkCore.DrawTrialMode(gameTime);


#if DEBUG
            foreach (MenuItem item in menuItems)
            {
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, item.hitBox,
                        sprite.blank, new Color(255, 0, 0, 128));

                if (menuItems.IndexOf(item) == 0)
                    Helpers.DrawOutline("x", item.hitCursor);
            }
#endif
        }

        private void DrawSignedIn(Vector2 signPos, Color color1, Color color2)
        {
            
#if LiveEnabled
            SignedInGamer gamer = SignedInGamer.SignedInGamers[FrameworkCore.ControllingPlayer];
            string signString = (gamer != null) ? Resource.MenuSignedIn + " " + gamer.Gamertag :
                Resource.MenuSignedNotSignedIn;
#else
            string signString = Resource.MenuMainWelcome + " " + FrameworkCore.players[0].commanderName;
#endif



            Color greyColor = Color.Lerp(new Color(128, 128, 128, 0), new Color(160, 160, 160), Transition);
            

            Vector2 signVec = FrameworkCore.Serif.MeasureString(signString);
            signPos.Y += 76;
            Helpers.DrawOutline(FrameworkCore.Serif, signString, signPos, greyColor, color2,
                -0.15f, Vector2.Zero, 1);
                        
        }


        private void DrawCopyright()
        {
            Vector2 copyrightPos = Vector2.Zero;
            copyrightPos.X = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;
            copyrightPos.Y = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height;

            float logoSize = 1.3f;
            Vector2 logoPos = new Vector2(copyrightPos.X - 120 - sprite.tinyLogo.Width * logoSize,
                copyrightPos.Y - 70 - sprite.tinyLogo.Height * logoSize);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, logoPos, sprite.tinyLogo, Color.White,0,
                Vector2.Zero, logoSize, SpriteEffects.None, 0);

            string copyrightString = Resource.copyright + " " + Resource.MenuVersionPrefix + FrameworkCore.VERSION;
            Vector2 copyrightVec = FrameworkCore.Serif.MeasureString(copyrightString);

            copyrightPos.X = copyrightPos.X * 0.1f;
            copyrightPos.Y = Helpers.PopLerp(Transition, copyrightPos.Y,copyrightPos.Y - copyrightVec.Y - 90,
                copyrightPos.Y - copyrightVec.Y - 70);//MathHelper.Lerp(copyrightPos.Y, copyrightPos.Y - copyrightVec.Y - 60, Transition);


            Helpers.DrawOutline(FrameworkCore.Serif, copyrightString, copyrightPos,
                new Color(128, 128, 128), new Color(0, 0, 0, 128));
        }

        public void drawitems(GameTime gameTime, float modifier)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                float angle = -0.15f;
                Color itemColor = Color.White;
                float itemSize = Helpers.PopLerp(menuItems[i].selectTransition, 0.85f, 1.1f, 1.0f);

                if (selectedItem != null && selectedItem == menuItems[i])
                {
                    itemColor = Color.Lerp(itemColor, new Color(255, 128, 0), menuItems[i].selectTransition);
                }

                itemColor = Color.Lerp(OldXNAColor.TransparentWhite, itemColor, Transition);
                Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);



                Helpers.DrawOutline(menuFont, menuItems[i].text, menuItems[i].position + new Vector2(modifier,0), itemColor, darkColor,
                    angle, new Vector2(0, (LINESIZE * itemSize) /2), itemSize);
            }
        }
    }
}
