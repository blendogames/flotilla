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
    public class BasePopup : SysMenu
    {
        
        public Rectangle sideIconRect;

        /// <summary>
        /// A menu formated to look like a popup window.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="description">What text to display in the main window.</param>
        public BasePopup(SysMenuManager owner)
        {
            menuFont = FrameworkCore.Serif;
            SetOwner(owner);
        }

        protected void DrawRectangle(Rectangle rect, Color rectColor)
        {
            rect.Y += 2;
            rect.X -= 6;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rect, sprite.blank, rectColor);
        }

        protected void DrawRawRectangle(Rectangle rect, Color rectColor)
        {
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rect, sprite.blank, rectColor);
        }       

        

        public override void Draw(GameTime gameTime)
        {
        }

        protected float GetItemHeight()
        {
            Vector2 textVec = menuFont.MeasureString("Sample");
            return textVec.Y + 8;
        }

        public override void DrawItems(GameTime gameTime, Vector2 pos)
        {
        }
    }
}
