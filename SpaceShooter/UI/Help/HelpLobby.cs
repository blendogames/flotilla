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
    public class HelpLobby : SysMenu
    {
        public HelpLobby()
        {
            darkenScreen = true;
            int xoffset = 200;

            int fontHeight = (int)menuFont.MeasureString("S").Y;

            Vector2 pos = new Vector2(400, 200);


            MenuItem item = new MenuItem(Resource.MenuOptions);
            item.Selected += OnSelectOptions;
            item.position = pos;
            base.AddItem(item);

            pos.Y += fontHeight;

            /*
            item = new MenuItem(Resource.HelpHowToPlay);
            item.Selected += OnSelectOptions;
            item.position = pos;
            base.AddItem(item);

            pos.Y += fontHeight;
            */

            item = new MenuItem(Resource.HelpHowToPlay);
            item.Selected += OnHowToPlay;
            item.position = pos;
            base.AddItem(item);

            pos.Y += fontHeight;

            item = new MenuItem(Resource.HelpViewControls);
            item.Selected += OnControls;
            item.position = pos;
            base.AddItem(item);

            pos.Y += fontHeight;

            item = new MenuItem(Resource.MenuCredits);
            item.Selected += OnCredits;
            item.position = pos;
            base.AddItem(item);

            pos.Y += fontHeight;

            item = new MenuItem(Resource.MenuDone);
            item.Selected += OnDone;
            item.position = pos;
            base.AddItem(item);




            base.RepositionItems();
        }


        private void OnSelectOptions(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new OptionsMenu());
        }


        private void OnHowToPlay(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new HelpTurns());
        }


        private void OnControls(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

#if WINDOWS
            Owner.AddMenu(new HelpControlsPC());
#else
            Owner.AddMenu(new HelpControlsXBOX());
#endif
        }

        private void OnCredits(object sender, EventArgs e)
        {
            if (Owner == null)
                return;

            Owner.AddMenu(new CreditsMenu(false));
        }

        private void OnDone(object sender, EventArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            base.Update(gameTime, inputManager);
            base.UpdateMouseItems(gameTime, inputManager);
        }


        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            float transitionMod = Helpers.PopLerp(Transition, -100, 40, 0);

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);
            
            Vector2 titleVec = FrameworkCore.Gothic.MeasureString(Resource.MenuPaused);
            Vector2 titlePos = menuItems[0].position + new Vector2(0, -titleVec.Y);
            titlePos.X += transitionMod;
            titlePos.Y -= 32;
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.HelpAndOptions, titlePos, titleColor, darkColor,
                0, Vector2.Zero, 1);

            base.DrawItems(gameTime, transitionMod);

        }
    }
}
