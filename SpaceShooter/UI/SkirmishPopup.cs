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
    public class SkirmishPopup : GamePopup
    {
        public SkirmishPopup(SysMenuManager owner)
            : base(owner)
        {
            transitionOnTime = 80;
            transitionOffTime = 180;
            
            menuFont = FrameworkCore.Serif;
            SetOwner(owner);

            InitializeItems();
        }

        public override void InitializeItems()
        {
            Vector2 drawPos;

            if (screenPos != Vector2.Zero)
                drawPos = screenPos;                
            else
            {
                screenPos = Helpers.GetScreenCenter();
                screenPos.X -= this.width / 2;                

                drawPos = screenPos;
            }
            

            Vector2 itemPos = drawPos;

            foreach (MenuItem item in menuItems)
            {
                item.position = itemPos;
                itemPos.Y += GetItemHeight();
            }
        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {

#if WINDOWS
            if (Transition >= 1)
            {
                foreach (MenuItem item in menuItems)
                {
                    int itemIndex = menuItems.IndexOf(item);

                    Rectangle itemBox = new Rectangle(
                        (int)screenPos.X,
                        (int)(screenPos.Y - GetItemHeight() / 2 + (itemIndex * GetItemHeight())),
                        (int)this.width,
                        (int)GetItemHeight());

                    if (itemBox.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                    {
                        if (inputManager.mouseHasMoved)
                            selectedItem = item;

                        if (inputManager.mouseLeftClick)
                            ActivateItem(inputManager);
                    }

                }
            }
#endif

            base.Update(gameTime, inputManager);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (MenuItem item in menuItems)
            {
                if (item.hitBox == null)
                    continue;

                Helpers.DrawDebugRectangle(item.hitBox, Color.Green);
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}
