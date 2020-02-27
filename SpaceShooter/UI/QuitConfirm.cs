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
    public class QuitConfirm : SysMenu
    {
        public QuitConfirm()
        {
            darkenScreen = true;
            int xoffset = 200;

            int fontHeight = (int)menuFont.MeasureString("S").Y;

            Vector2 pos = new Vector2(400, 200);


            MenuItem item = new MenuItem(Resource.MenuYes);
            item.Selected += OnYes;
            item.position = pos;
            base.AddItem(item);

            pos.Y += fontHeight;

            item = new MenuItem(Resource.MenuNo);
            item.Selected += OnNo;
            item.position = pos;
            base.AddItem(item);


            base.RepositionItems();
        }


        private void OnNo(object sender, EventArgs e)
        {
            FrameworkCore.PlayCue(sounds.click.activate);
            Deactivate();
        }

        private void OnYes(object sender, EventArgs e)
        {
            FrameworkCore.PlayCue(sounds.click.activate);
            Deactivate();

            FrameworkCore.ExitToMainMenu(null);
        }


       
        public override void Update(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            if (Transition >= 1)
            {
                if (inputManager.kbNPressed)
                {
                    OnNo(this, null);
                }
                else if (inputManager.kbYPressed)
                {
                    
                    OnYes(this, null);
                }
            }
#endif

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
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuQuitQuestion, titlePos, titleColor, darkColor,
                0, Vector2.Zero, 1);

            base.DrawItems(gameTime, transitionMod);

        }
    }
}
