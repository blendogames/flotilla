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
    public class GamePopup : BasePopup
    {
        public Vector2 screenPos = Vector2.Zero;
        public int width = 512;

        public GamePopup(SysMenuManager owner) : base(owner)
        {
            transitionOnTime = 80;
            transitionOffTime = 180;

            menuFont = FrameworkCore.Serif;
            SetOwner(owner);
        }

        
        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            Vector2 screenCenter = Vector2.Zero;
            Vector2 itemPos = Vector2.Zero;

            //if player does not provide a screen position, then center the popup window.
            if (screenPos != Vector2.Zero)
            {
                screenCenter = screenPos;
                itemPos = new Vector2(screenCenter.X + 14,
                    screenCenter.Y - 16);
            }
            else
            {
                //center it ons creen.
                screenCenter = Helpers.GetScreenCenter();
                itemPos = new Vector2(screenCenter.X - (this.width/2) + 8,
                    screenCenter.Y - ((menuItems.Count * GetItemHeight()) / 2f));
            }

            
            itemPos = Helpers.PopLerp(Transition,
                itemPos + new Vector2(-30, -20),
                itemPos + new Vector2(10, 10),
                itemPos);

            Color boxColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color backColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, Transition);


            Rectangle itemRect = new Rectangle(
                (int)itemPos.X,
                (int)itemPos.Y,
                this.width,
                (int)(menuItems.Count * GetItemHeight()));
            itemRect.Y -= 4;
            itemRect.Height += 4;
            itemRect.X -= 4;
            itemRect.Width += 4;

            Rectangle backRectangle = itemRect;
            backRectangle.Inflate(4, 4);
            DrawRectangle(backRectangle, backColor);

            DrawRectangle(itemRect, boxColor);


            DrawItems(gameTime, itemPos);
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
                    Color barColor = new Color(255, 128, 0);
                    Color selectColor = Color.Lerp(Helpers.transColor(barColor), barColor, item.selectTransition);
                    selectColor = Color.Lerp(Helpers.transColor(selectColor), selectColor, Transition);
                    Rectangle selectRect = new Rectangle((int)pos.X, (int)pos.Y, this.width, (int)textVec.Y);
                    selectRect.Y += 2;
                    selectRect.X -= 6;
                    selectRect.Width = (int)MathHelper.Lerp(this.width / 3, this.width - 4, item.selectTransition);

                    //the orange bar.
                    if (item.selectTransition >= 1)
                    {
                        Color outlineColor = Color.Lerp(selectColor, Color.Black, 0.5f);
                        outlineColor.A = 160;
                        Rectangle outlineBox = selectRect;
                        outlineBox.Inflate(1, 1);
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, outlineBox, sprite.blank, outlineColor);
                    }

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, selectRect, sprite.blank, selectColor);

                    //draw the bouncing A button.
                    Vector2 buttPos = pos + new Vector2(-7, textVec.Y / 2);
                    buttPos.X += Helpers.Pulse(gameTime, 6, 4);
                    float buttSize = MathHelper.Lerp(0, 1, item.selectTransition);

                    Rectangle iconRect = sprite.buttons.a;
                    Color iconColor = Color.White;
                    SpriteEffects iconEffect = SpriteEffects.None;
#if WINDOWS
                    iconRect = sprite.arrow;
                    iconColor = new Color(255, 180, 0);
                    iconEffect = SpriteEffects.FlipHorizontally;
#endif

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttPos, iconRect, iconColor,
                        0, Helpers.SpriteCenter(iconRect), buttSize, iconEffect, 0);
                }



                itemColor = Color.Lerp(OldXNAColor.TransparentBlack, itemColor, Transition);

                Vector2 textPos = pos;
                textPos.X += Helpers.PopLerp(item.selectTransition, 0, 24, 16);

                FrameworkCore.SpriteBatch.DrawString(menuFont, item.text, textPos,
                    itemColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                pos.Y += GetItemHeight();
            }
        }



        public override void DrawItems(GameTime gameTime, float xOffset)
        {
            Vector2 textVec = menuFont.MeasureString("Sample");            

            foreach (MenuItem item in menuItems)
            {
                Vector2 pos = item.position;
                Color itemColor = Color.Black;

                pos.X += xOffset;

                if (selectedItem != null && selectedItem == item)
                {
                    itemColor = Color.Lerp(itemColor, Color.White, item.selectTransition);
                }

                if (item.selectTransition > 0)
                {
                    Color barColor = new Color(255, 128, 0);
                    Color selectColor = Color.Lerp(Helpers.transColor(barColor), barColor, item.selectTransition);
                    selectColor = Color.Lerp(Helpers.transColor(selectColor), selectColor, Transition);
                    Rectangle selectRect = new Rectangle((int)pos.X, (int)pos.Y, this.width, (int)textVec.Y);
                    selectRect.Y += 2;
                    selectRect.X -= 6;
                    selectRect.Width = (int)MathHelper.Lerp(this.width / 3, this.width - 4, item.selectTransition);

                    //the orange bar.
                    if (item.selectTransition >= 1)
                    {
                        Color outlineColor = Color.Lerp(selectColor, Color.Black, 0.5f);
                        outlineColor.A = 160;
                        Rectangle outlineBox = selectRect;
                        outlineBox.Inflate(1, 1);
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, outlineBox, sprite.blank, outlineColor);
                    }

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, selectRect, sprite.blank, selectColor);

                    //draw the bouncing A button.
                    Vector2 buttPos = pos + new Vector2(-7, textVec.Y / 2);
                    buttPos.X += Helpers.Pulse(gameTime, 6, 4);
                    float buttSize = MathHelper.Lerp(0, 1, item.selectTransition);

                    Rectangle iconRect = sprite.buttons.a;
                    Color iconColor = Color.White;
                    SpriteEffects iconEffect = SpriteEffects.None;
#if WINDOWS
                    iconRect = sprite.arrow;
                    iconColor = new Color(255, 180, 0);
                    iconEffect = SpriteEffects.FlipHorizontally;
#endif



                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttPos, iconRect, iconColor,
                        0, Helpers.SpriteCenter(iconRect), buttSize, iconEffect, 0);
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
