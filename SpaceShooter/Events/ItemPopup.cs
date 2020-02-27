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
    public class ItemPopup : GamePopup
    {
        public InventoryItem inventoryItem;

        public ItemPopup(SysMenuManager owner)
            : base(owner)
        {
            screenPos = new Vector2(
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - 256,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - 128);

            transitionOnTime = 500;
            transitionOffTime = 300;

            darkenScreen = true;

            MenuItem item = new MenuItem("");
            item.Selected += OnClose;
            base.AddItem(item);

            canBeExited = false;
        }

        private void OnClose(object sender, InputArgs e)
        {
            //Helpers.CloseThisMenu(sender);
            OpenEquipMenu();
        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            //player wants to equip this item immediately.
            if ((inputManager.kbSpace || inputManager.OpenMenu) && Transition >= 1)
            {
                OpenEquipMenu();
            }

#if WINDOWS
            if (Transition >= 1)
            {
                if (inputManager.mouseLeftClick)
                {
                    OpenEquipMenu();
                }

                /*
                if (FrameworkCore.worldMap.FleetButtonRect.Contains((int)inputManager.mousePos.X,
                    (int)inputManager.mousePos.Y))
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                    TimeSpan.FromMilliseconds(200).TotalMilliseconds);

                    fleetButtonTransition = MathHelper.Clamp(fleetButtonTransition + delta, 0, 1);

                    if (inputManager.mouseLeftClick)
                        OpenEquipMenu();
                }
                else
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                    TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                    fleetButtonTransition = MathHelper.Clamp(fleetButtonTransition - delta, 0, 1);

                    if (inputManager.mouseLeftClick)
                        Deactivate();
                }*/
            }
                
#endif

            base.Update(gameTime, inputManager);
        }

        private void OpenEquipMenu()
        {
            Deactivate();
            Owner.AddMenu(new FleetMenu());
        }


        float fleetButtonTransition = 0;

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            Vector2 drawPos;

            if (screenPos != Vector2.Zero)
                drawPos = screenPos;
            else
                //center it ons creen.
                drawPos = Helpers.GetScreenCenter();

            Color boxColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color backColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, Transition);

            drawPos.Y += Helpers.PopLerp(Transition, 500, -40, 0);
            
            Rectangle itemRect = new Rectangle(
                (int)drawPos.X,
                (int)drawPos.Y,
                512,
                256);

            //draw the border rectangle.
            Rectangle backRectangle = itemRect;
            backRectangle.Inflate(4, 4);
            DrawRawRectangle(backRectangle, backColor);

            //draw the white rectangle.
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemRect, sprite.inventoryBox, boxColor);


            Vector2 fanfarePos = new Vector2(
                screenPos.X + 256,
                screenPos.Y - 80);
            fanfarePos.Y += Helpers.PopLerp(Transition, -100, 30, 0);
            float fanfareAngle = MathHelper.Lerp(-0.6f, -0.1f, Transition);
            fanfarePos.Y += Helpers.Pulse(gameTime, 8, 3);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, Resource.GameNewCargo, fanfarePos, boxColor,
                fanfareAngle, Helpers.stringCenter(FrameworkCore.Gothic, Resource.GameNewCargo), 1, SpriteEffects.None, 0);

            
            if (inventoryItem == null)
                return;

            //draw the item icon.
            Vector2 iconPos = new Vector2(
                screenPos.X + 256,
                screenPos.Y + 16);

            iconPos.Y += Helpers.PopLerp(Transition, -300, 20, 0);
            float iconAngle = MathHelper.Lerp(0.8f, 0.1f, Transition);

            iconAngle += Helpers.Pulse(gameTime, 0.05f, 5);

            Color squareColor = Color.Lerp(Helpers.transColor(Helpers.ITEMCOLOR), Helpers.ITEMCOLOR, Transition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, iconPos, sprite.roundSquare, squareColor,
                iconAngle, Helpers.SpriteCenter(sprite.roundSquare), 1.3f, SpriteEffects.None, 0);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, iconPos, inventoryItem.image, boxColor,
                iconAngle, Helpers.SpriteCenter(inventoryItem.image), 1, SpriteEffects.None, 0);


            //draw the item name.            
            Vector2 textPos = new Vector2(itemRect.X + 256, itemRect.Y + 100);
            Vector2 titleSize = Vector2.Zero;
            if (inventoryItem.name != null)
            {
                 titleSize = Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, inventoryItem.name,
                    textPos, backColor, 1.3f, 0);
            }

            textPos.Y += titleSize.Y;
            textPos.Y += 16;

            //draw the description.
            if (inventoryItem.description != null)
            {
                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, inventoryItem.description,
                    textPos, backColor, 1);
                
            }

#if WINDOWS
            //FrameworkCore.worldMap.DrawFleetButton(fleetButtonTransition);

            Helpers.DrawClickMessage(gameTime, Transition, Resource.MenuClickToEquip);
#else
            //float x = Helpers.DrawLegend(Resource.MenuOK, sprite.buttons.a, Transition);
            //Helpers.DrawLegendAt(Resource.MenuFleetEquip, sprite.buttons.rb, Transition, x-24);

            Helpers.DrawLegend(Resource.MenuFleetEquip, sprite.buttons.a, Transition);
#endif
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}
