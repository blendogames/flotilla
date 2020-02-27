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
#endregion

namespace SpaceShooter
{
    public class PauseMenu : SysMenu
    {
        bool trialMenu = false;

        public PauseMenu()
        {
            darkenScreen = true;

            MenuItem item = new MenuItem(Resource.MenuResumeGame);
            item.Selected += OnSelectReturn;
            base.AddItem(item);


            item = new MenuItem(Resource.HelpAndOptions);
            item.Selected += OnSelectOptions;
            base.AddItem(item);

            if (FrameworkCore.isTrialMode())
            {
                item = new MenuItem(Resource.MenuUnlockFullGame);
                item.Selected += OnBuyGame;
                base.AddItem(item);

                trialMenu = true;
            }

#if WINDOWS && !ONLIVE
            item = new MenuItem(Resource.MenuBugReport);
            item.Selected += OnBugReport;
            base.AddItem(item);
#endif
            if (FrameworkCore.isCampaign && DestructAvailable())
            {
                item = new MenuItem(Resource.MenuSelfDestruct);
                item.Selected += OnSelectSelfDestruct;
                base.AddItem(item);
            }

            item = new MenuItem(Resource.MenuQuit);
            item.Selected += OnSelectMainMenu;
            base.AddItem(item);

            SetupItemPositions();
        }



        private bool DestructAvailable()
        {
            if (FrameworkCore.level.gamemode == GameMode.CarnageReport)
                return false;

            bool playershipAvailable = false;
            bool enemyshipAvailable = false;

            for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
            {
                //only check ships.
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[k]))
                    continue;

                //sanity check.
                if (((SpaceShip)FrameworkCore.level.Ships[k]).owner == null)
                    continue;

                if (((SpaceShip)FrameworkCore.level.Ships[k]).IsDestroyed)
                    continue;

                if (((SpaceShip)FrameworkCore.level.Ships[k]).Health <= 0)
                    continue;

                if (((SpaceShip)FrameworkCore.level.Ships[k]).owner.GetType() == typeof(PlayerCommander))
                    playershipAvailable = true;
                else
                    enemyshipAvailable = true;
            }

            return (playershipAvailable && enemyshipAvailable);
        }



#if WINDOWS
        private void OnBugReport(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new BugReport());
        }
#endif

        private void SetupItemPositions()
        {
            Vector2 pos = new Vector2(Math.Max(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - 500, 100),
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - 50);

            int itemVec = (int)menuFont.MeasureString("Sample").Y + 8;

            foreach (MenuItem item in menuItems)
            {
                item.position = pos;
                pos.Y += itemVec;
            }
        }

        private void OnBuyGame(object sender, EventArgs e)
        {
            FrameworkCore.BuyGame();
        }

        private void OnSelectReturn(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }

        private void OnSelectOptions(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new HelpLobby());
        }

        private void OnSelectSelfDestruct(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new SelfDestructConfirm());
        }

        private void OnSelectMainMenu(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new QuitConfirm());
        }







        private Vector2 ring1Pos;
        private Vector2 ring1DesiredPos;

        private Vector2 ring2Pos;
        private Vector2 ring2DesiredPos;

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            TrialCheck();

            ring1Pos = Vector2.Lerp(ring1Pos, ring1DesiredPos, 0.2f);
            ring1DesiredPos = new Vector2(100, -100);
            ring1DesiredPos.Y += menuItems.IndexOf(selectedItem) * 100;
            ring1DesiredPos.X += menuItems.IndexOf(selectedItem) * 30;

            ring2Pos = Vector2.Lerp(ring2Pos, ring2DesiredPos, 0.2f);
            ring2DesiredPos = new Vector2(300, 300);
            ring2DesiredPos.Y += menuItems.IndexOf(selectedItem) * 40;
            ring2DesiredPos.X += menuItems.IndexOf(selectedItem) * 200;



            if (Transition >= 1)
            {
                foreach (PlayerCommander commander in FrameworkCore.players)
                {
                    if (commander.inputmanager.buttonStartPressed)
                    {
                        Deactivate();
                    }
                }

                if (inputManager.kbEscPressed)
                {
                    Deactivate();
                }
            }

            base.Update(gameTime, inputManager);
            base.UpdateMouseItems(gameTime, inputManager);
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
            FrameworkCore.PlayCue(sounds.click.whoosh);

            base.Activate();
        }

        private void DrawBigCircle(GameTime gameTime, Vector2 pos)
        {
            float size = 3;
            float rotation = (float)gameTime.TotalGameTime.TotalSeconds * 0.2f;

            Color ringColor = Color.Lerp(OldXNAColor.TransparentWhite,
                new Color(255, 255, 255, 32), Transition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.bigCircle, ringColor,
                rotation, Helpers.SpriteCenter(sprite.bigCircle), size, SpriteEffects.None, 0);
        }

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            Vector2 ringMod = Vector2.Lerp(new Vector2(100, 0), Vector2.Zero, Transition);
            DrawBigCircle(gameTime, ring1Pos + ringMod);
            DrawBigCircle(gameTime, ring2Pos + ringMod);



            float transitionMod = Helpers.PopLerp(Transition, -100, 40, 0);

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);

            
            Vector2 titleVec = FrameworkCore.Gothic.MeasureString(Resource.MenuPaused);
            Vector2 titlePos = menuItems[0].position + new Vector2(0, -titleVec.Y);
            titlePos.X += transitionMod;
            titlePos.Y -= 32;
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuPaused, titlePos, titleColor, darkColor,
                0, Vector2.Zero, 1);

            base.DrawItems(gameTime, transitionMod);

            FrameworkCore.DrawTrialMode(gameTime);
        }
    }
}
