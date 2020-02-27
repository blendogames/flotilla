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
    public class SysPopup : BasePopup
    {
        string descriptionText = "";
        public string windowname = "";

        
        

        /// <summary>
        /// A menu formated to look like a popup window.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="description">What text to display in the main window.</param>
        public SysPopup(SysMenuManager owner, string description) : base(owner)
        {
            menuFont = FrameworkCore.Serif;
            this.descriptionText = description;
            SetOwner(owner);


            this.descriptionText = Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                description, 504, Vector2.Zero, Color.Black);
        }


        public override void Update(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            //handle mouse support.
            if (Transition >= 1)
            {
                bool isHovering = false;

                Vector2 itemPos = itemStartPos;
                foreach (MenuItem item in menuItems)
                {
                    Rectangle itemRect = new Rectangle(
                        (int)itemPos.X,
                        (int)itemPos.Y,
                        512,
                        (int)GetItemHeight());

                    if (itemRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                    {
                        isHovering = true;

                        if (inputManager.mouseHasMoved)
                        {
                            selectedItem = item;
                            break;
                        }

                        if (inputManager.mouseLeftClick)
                        {
                            ActivateItem(inputManager);
                        }
                    }

                    itemPos.Y += GetItemHeight();
                }

                if (!isHovering && inputManager.mouseHasMoved)
                    selectedItem = null;
            }
#endif

            base.Update(gameTime, inputManager);
        }

        //note: next menu system has to fully integrate mouse support from the very beginning!!!
        Vector2 itemStartPos = Vector2.Zero;

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            Vector2 screenCenter = Helpers.GetScreenCenter();

            
            screenCenter.Y = MathHelper.Lerp(screenCenter.Y * 2 + 300, screenCenter.Y, Transition);

            Color greyColor = new Color(224, 224, 224);


            Rectangle backRect = new Rectangle(
                (int)screenCenter.X - 320,
                (int)screenCenter.Y - 150,
                640,
                300);
            backRect.Inflate(6, 6);
            base.DrawRectangle(backRect, Color.Black);


            Rectangle sideRect = new Rectangle(
                (int)screenCenter.X - 320,
                (int)screenCenter.Y - 150,
                120,
                300);
            base.DrawRectangle(sideRect, greyColor);





            Vector2 itemPos = new Vector2(screenCenter.X - 192,
                (screenCenter.Y + 150) - (menuItems.Count * base.GetItemHeight()));

            itemStartPos = itemPos;

            Rectangle itemRect = new Rectangle(
                (int)itemPos.X,
                (int)itemPos.Y,
                512,
                (int)(menuItems.Count * base.GetItemHeight()));
            itemRect.Y -= 4;
            itemRect.Height += 4;
            itemRect.X -= 4;
            itemRect.Width += 4;
            DrawRectangle(itemRect, Color.White);

            DrawItems(gameTime, itemPos);



            Rectangle topRect = new Rectangle(
                (int)itemRect.X,
                (int)screenCenter.Y - 150,
                itemRect.Width,
                300 - itemRect.Height - 4);
            DrawRectangle(topRect, Color.White);

            topRect.Inflate(-16, -16);

            //Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                //descriptionText, topRect.Width, new Vector2(topRect.X, topRect.Y), Color.Black);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, descriptionText,
                new Vector2(topRect.X, topRect.Y), Color.Black);


            if (sideIconRect != null)
            {
                Vector2 sideIconPos = new Vector2(
                    sideRect.X + sideRect.Width / 2f,
                    sideRect.Y + sideRect.Width / 2f);

                sideIconPos.X -= 6;
                sideIconPos.Y += 2;

                sideIconPos.Y += Helpers.Pulse(gameTime, 5, 4);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, sideIconPos, sideIconRect, Color.White,
                    0, Helpers.SpriteCenter(sideIconRect), 1, SpriteEffects.None, 0);
            }
        }


        public override void DrawItems(GameTime gameTime, Vector2 pos)
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            foreach (MenuItem item in menuItems)
            {
                Color itemColor = Color.Black;


                if (selectedItem != null && selectedItem == item)
                {
                    itemColor = Color.Lerp(itemColor, Color.White, item.selectTransition);

                 
                }

                if (item.selectTransition > 0)
                {
                    Color selectColor = Color.Lerp(new Color(255, 128, 0, 0), new Color(255, 128, 0, 255), item.selectTransition);
                    Rectangle selectRect = new Rectangle((int)pos.X, (int)pos.Y, 512, (int)textVec.Y);
                    selectRect.Y += 2;
                    selectRect.X -= 6;
                    selectRect.Width = (int)MathHelper.Lerp(128, 508, item.selectTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, selectRect, sprite.blank, selectColor);
                }



                itemColor = Color.Lerp(OldXNAColor.TransparentBlack, itemColor, Transition);

                Vector2 textPos = pos;
                textPos.X += Helpers.PopLerp(item.selectTransition, 0, 24, 16);

                FrameworkCore.SpriteBatch.DrawString(menuFont, item.text, textPos,
                    itemColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                pos.Y += GetItemHeight();
            }
        }
    }
}
