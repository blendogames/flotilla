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
    public class EventPopup : GamePopup
    {
        public Rectangle image = Rectangle.Empty;
        public string description;

        public string eventName = "";

        public EventPopup(SysMenuManager owner)
            : base(owner)
        {
            screenPos = new Vector2(
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - 512,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - 256);

            transitionOnTime = 500;
            transitionOffTime = 300;

            canBeExited = false;
        }

        public override void InitializeItems()
        {
            Vector2 drawPos;

            if (screenPos != Vector2.Zero)
                drawPos = screenPos;
            else
                //center it ons creen.
                drawPos = Helpers.GetScreenCenter();

            Vector2 itemPos = drawPos;
            itemPos.Y += 512;
            itemPos.Y -= GetItemHeight() * menuItems.Count;

            foreach (MenuItem item in menuItems)
            {
                item.position = itemPos;
                itemPos.Y += GetItemHeight();
            }

            if (description != null)
            {
                description = Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                    description, 515, Vector2.Zero, Color.White);
            }
        }

        private void SelectAndRun(int index, InputManager inputManager)
        {
            if (menuItems.Count <= 0)
                return;

            //sanity check the index value.
            if (index > menuItems.Count - 1)
                return;

            if (index < 0)
                return;

            try
            {
                selectedItem = menuItems[index];
                base.ActivateItem(inputManager);
            }
            catch
            {
            }
        }

        private void PressSpace(InputManager inputManager)
        {
            if (menuItems.Count <= 0)
                return;

            if (menuItems.Count > 1)
                return;

            SelectAndRun(0, inputManager);
        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            if (Transition >= 1)
            {
                if (inputManager.kbSpace)
                {
                    PressSpace(inputManager);
                }

                if (inputManager.kb1Pressed)
                {
                    SelectAndRun(0, inputManager);
                }
                else if (inputManager.kb2Pressed)
                {
                    SelectAndRun(1, inputManager);
                }
                else if (inputManager.kb3Pressed)
                {
                    SelectAndRun(2, inputManager);
                }
                else if (inputManager.kb4Pressed)
                {
                    SelectAndRun(3, inputManager);
                }



                bool mouseHover = false;
                foreach (MenuItem item in menuItems)
                {
                    Rectangle itemRect = new Rectangle(
                        (int)item.position.X,
                        (int)item.position.Y,
                        512,//windowWidth
                        (int)GetItemHeight());
                    itemRect.X -= 15;
                    itemRect.Width += 15;
                    itemRect.Y -= 3;

                    if (itemRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                    {
                        mouseHover = true;

                        if (inputManager.mouseHasMoved)
                            selectedItem = item;
                    }
                }

                if (mouseHover && inputManager.mouseLeftClick)
                {
                    ActivateItem(inputManager);
                }
                else if (!mouseHover && inputManager.mouseHasMoved)
                {
                    selectedItem = null;
                }
            }
#endif

#if DEBUG
            if (inputManager.kbIPressed)
            {
                Deactivate();
            }
#endif

            base.Update(gameTime, inputManager);
            //base.UpdateMouseItems(gameTime, inputManager);
        }

        public override void Draw(GameTime gameTime)
        {

            //VIGNETTE.
            Color vigColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, Transition);
            Rectangle wholeScreen = new Rectangle(
                0, 0,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                wholeScreen, sprite.vignette, vigColor);



            Vector2 drawPos;

            if (screenPos != Vector2.Zero)
                drawPos = screenPos;
            else
                //center it ons creen.
                drawPos = Helpers.GetScreenCenter();


            Color boxColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color backColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, Transition);

            float xOffset = Helpers.PopLerp(Transition, -500, 80, 0);
            drawPos.X += xOffset;
            
            Rectangle itemRect = new Rectangle(
                (int)drawPos.X,
                (int)drawPos.Y,
                512,
                512);

            itemRect.Y -= 4;
            itemRect.Height += 4;
            itemRect.X -= 4;
            itemRect.Width += 4;


            //draw the border rectangle.
            Rectangle backRectangle = itemRect;
            backRectangle.Inflate(4, 4);
            DrawRectangle(backRectangle, backColor);

            //draw the white rectangle.
            DrawRectangle(itemRect, boxColor);

            
            //draw the little bar beneath the image.
            Rectangle imageBar = itemRect;
            imageBar.Height = image.Height + 3;
            DrawRectangle(imageBar, backColor);

            //draw the image.
            Rectangle imageRect = itemRect;
            imageRect.Height = image.Height;
            imageRect.X -= 6;
            imageRect.Y += 2;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.eventSheet,
                imageRect, image, boxColor);


            Rectangle mainBG = new Rectangle(
                (int)itemRect.X-6,
                (int)itemRect.Y + image.Height+5,
                imageRect.Width,
                512 - image.Height - (int)(menuItems.Count * GetItemHeight()));            
            mainBG.Height -= 10;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.eventSheet, mainBG,
                sprite.eventSprites.mainBG, boxColor);


            if (description != null)
            {
                Vector2 descriptionPos = new Vector2(imageBar.X, imageBar.Y);
                descriptionPos.Y += image.Height + 16;
                descriptionPos.X += 6;
                itemRect.Width -= 8;

                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, description,
                    descriptionPos, backColor);


                /*
                Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                    description, itemRect.Width, descriptionPos, backColor);
                 */
            }



            //draw the option items.
            Vector2 itemPos = drawPos;
            itemPos.Y += 512;
            itemPos.Y -= GetItemHeight() * menuItems.Count;

            //itemBox background.
            Rectangle itemBG = new Rectangle(
                (int)itemPos.X-10,
                (int)itemPos.Y-6,
                512+4,
                (int)(GetItemHeight() * menuItems.Count) + 8);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.eventSheet, itemBG,
                sprite.eventSprites.itemsBG, boxColor);

            DrawItems(gameTime, xOffset);

            //draw the bar above the options.
            itemPos.Y -= 11;
            itemPos.X -= 5;
            Rectangle itemBar = new Rectangle(
                (int)itemPos.X,
                (int)itemPos.Y,
                520,
                3);
            DrawRectangle(itemBar, backColor);

#if WINDOWS
            Vector2 mousePos = Vector2.Zero;

            foreach (PlayerCommander player in FrameworkCore.players)
            {
                if (player.playerindex == FrameworkCore.ControllingPlayer)
                {
                    mousePos = player.inputmanager.mousePos;
                }
            }

            Helpers.DrawMouseCursor(FrameworkCore.SpriteBatch,
                mousePos);
#endif


#if DEBUG
            foreach (MenuItem item in menuItems)
            {
                if (item.hitBox == null)
                    return;

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, item.hitBox,
                    sprite.blank, new Color(255, 0, 0, 128));
            }
#endif

            if (!FrameworkCore.players[0].mouseEnabled)
            {
                float x = Helpers.DrawLegend(Resource.MenuOK, sprite.buttons.a, Transition);

                if (menuItems.Count > 1)
                    Helpers.DrawLegendAt(Resource.MenuSelectChoice, sprite.buttons.upDown, Transition, x - 32);
            }

            DrawEventName(gameTime);
        }

        private void DrawEventName(GameTime gameTime)
        {
            if (eventName == null)
                return;

            if (eventName.Length <= 0)
                return;

            Vector2 textPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                screenPos.Y);
            textPos.X -= 16;

            textPos.Y += Helpers.Pulse(gameTime, 6, 4);

            float textSize = Helpers.PopLerp(Transition, 4, 0.3f, 0.6f);

            Color whiteColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color blackColor = OldXNAColor.TransparentBlack;
            if (Transition >= 1)
                blackColor = Color.Black;

            Helpers.DrawOutline(FrameworkCore.Gothic, eventName, textPos,
                whiteColor, blackColor, -0.05f, Vector2.Zero, textSize);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}
