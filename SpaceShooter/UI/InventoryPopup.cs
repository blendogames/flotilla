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
    public class InventoryMenuItem : MenuItem
    {
        public InventoryItem inventoryItem;

        public InventoryMenuItem(string text)
            : base(text)
        {            
        }
    }

    public class InventoryPopup : BasePopup
    {
        

        public Vector2 screenPos = Vector2.Zero;
        public int width = 512;


        bool noItemsFound = false;

        public InventoryPopup(SysMenuManager owner, bool includeClearSlot)
            : base(owner)
        {
            transitionOnTime = 200;
            transitionOffTime = 200;

            menuFont = FrameworkCore.Serif;
            SetOwner(owner);

            darkenScreen = true;
            hideChildren = false;

            InventoryMenuItem menuItem = null;

            if (includeClearSlot)
            {
                menuItem = new InventoryMenuItem(Resource.MenuSkirmishSlotClear);
                menuItem.Selected += OnSelectClear;
                menuItems.Add(menuItem);
            }

            //populate the menu items.
            foreach (InventoryItem item in FrameworkCore.players[0].inventoryItems)
            {
                if (!isValidItem(item))
                    continue;

                menuItem = new InventoryMenuItem(item.name);
                menuItem.Selected += OnSelectItem;
                menuItem.inventoryItem = item;
                menuItems.Add(menuItem);
            }

            if (FrameworkCore.players[0].inventoryItems.Count > 0)
                selectedIndex = 0;


            if (menuItems.Count <= 0)
            {
                noItemsFound = true;

                menuItem = new InventoryMenuItem("");
                menuItem.Selected += OnSelectDeactivate;
                menuItems.Add(menuItem);
            }
            else
            {
                menuItem = new InventoryMenuItem(Resource.MenuCancel);
                menuItem.Selected += OnSelectDeactivate;
                menuItems.Add(menuItem);
            }

            UpdateTargetPositions();

            foreach (InventoryMenuItem item in menuItems)
            {
                item.position = item.targetPosition;
            }
        }


        private void OnSelectDeactivate(object sender, EventArgs e)
        {
            Deactivate();
        }


        Vector2 LINESIZE = FrameworkCore.Serif.MeasureString("Sample");

        int GAPSIZE = (int)(sprite.roundSquare.Width * 1.5f);
        int selectedIndex = -1;

        private void UpdateTargetPositions()
        {
            if (FrameworkCore.players[0].inventoryItems.Count <= 0)
                return;

            Vector2 centerPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width * 0.13f,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2);

            Vector2 startPos = centerPos - new Vector2(0, selectedIndex * GAPSIZE);

            int curIndex = 0;
            foreach (InventoryMenuItem item in menuItems)
            {
                item.targetPosition = startPos + new Vector2(0, curIndex * GAPSIZE);
                curIndex++;
            }
        }



        private void UpdateMenuPositions(GameTime gameTime)
        {
            if (FrameworkCore.players[0].inventoryItems.Count <= 0)
                return;

            foreach (InventoryMenuItem item in menuItems)
            {
                item.position = Vector2.Lerp(item.position, item.targetPosition,
                    14 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public override void SelectUp()
        {
            selectedIndex = menuItems.IndexOf(selectedItem);
            UpdateTargetPositions();
        }

        public override void SelectDown()
        {
            selectedIndex = menuItems.IndexOf(selectedItem);
            UpdateTargetPositions();
        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            UpdateMenuPositions(gameTime);

#if WINDOWS
            if (Transition >= 1)
            {
                if (noItemsFound)
                {
                    if (inputManager.mouseLeftClick || inputManager.kbSpace)
                        Deactivate();

                    return;
                }

                foreach (MenuItem item in menuItems)
                {
                    bool mouseHovering = false;
                    Helpers.UpdateTiltedMouseMenu(menuItems, inputManager.mousePos,
                        -0.35f, true,
                        new Point(450, 30),
                        null,
                        false,
                        inputManager,
                        selectedItem,
                        out mouseHovering,
                        out selectedItem);

                    if (mouseHovering && inputManager.mouseLeftClick)
                    {
                        ActivateItem(inputManager);
                    }
                }
            }
#endif 

            base.Update(gameTime, inputManager);
        }

        private void OnSelectClear(object sender, EventArgs e)
        {
            Deactivate();

            if (sender.GetType() != typeof(InventoryMenuItem))
                return;

            for (int i = 0; i < Owner.menus.Count; i++)
            {
                if (Owner.menus[i].GetType() == typeof(FleetMenu))
                {
                    ((FleetMenu)Owner.menus[i]).ClearItem();
                    break;
                }
            }
        }

        public override void Activate()
        {
            if (noItemsFound)
                FrameworkCore.PlayCue(sounds.click.error);

            base.Activate();
        }

        private bool isValidItem(InventoryItem item)
        {
            foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
            {
                for (int i = 0; i < ship.upgradeArray.Length; i++)
                {
                    if (ship.upgradeArray[i] == item)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void OnSelectItem(object sender, EventArgs e)
        {
            Deactivate();

            if (sender.GetType() != typeof(InventoryMenuItem))
                return;

            if (((InventoryMenuItem)sender).inventoryItem == null)
                return;

            FrameworkCore.PlayCue(sounds.Fanfare.drill);

            for (int i = 0; i < Owner.menus.Count; i++)
            {
                if (Owner.menus[i].GetType() == typeof(FleetMenu))
                {
                    ((FleetMenu)Owner.menus[i]).ApplyItem(((InventoryMenuItem)sender).inventoryItem);
                    break;
                }
            }
        }

        
        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            if (noItemsFound)
            {
                Color redColor = Color.Lerp(new Color(255, 0, 0, 0), new Color(255, 60, 60), Transition);
                Color whiteColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
                Vector2 textVec = FrameworkCore.Gothic.MeasureString(Resource.MenuUpgradesNoneAvailable);
                float textSize = Helpers.PopLerp(Transition, 0.5f, 0.7f, 0.6f);
                Vector2 textPos = new Vector2(
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - (textVec.X*textSize),
                    (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - 100);

                textPos.X += Helpers.PopLerp(Transition, 200, -50, 0);

                textPos.Y += MathHelper.Lerp(0,-20,0.5f + Helpers.Pulse(gameTime, 0.49f, 2));

                
                
                
                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, Resource.MenuUpgradesNoneAvailable,
                    textPos, redColor, 0, Vector2.Zero, textSize,
                    SpriteEffects.None, 0);

                textPos.Y += (textVec.Y*textSize) + 8;

                string desc = Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                    Resource.MenuUpgradesNoneAvailableDescription,
                    (int)(textVec.X*textSize), textPos,
                    whiteColor);
                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, desc, textPos, whiteColor);

#if XBOX
                Helpers.DrawLegend(Resource.MenuOK, sprite.buttons.a, Transition);
#else
                Helpers.DrawClickMessage(gameTime, Transition, Resource.MenuClickToContinue);
#endif

                return;
            }

            foreach (InventoryMenuItem item in menuItems)
            {
                float itemAngle = -0.35f;
                
                Vector2 itemPos = item.position;
                itemPos.X += Helpers.PopLerp(Transition, -250, 50, 0);

                //draw the glow.
                float glowSize = MathHelper.Lerp(0, 2, item.selectTransition);
                Color glowColor = Color.Lerp(new Color(255, 160, 0, 255), new Color(255, 160, 0, 128),
                    0.5f + Helpers.Pulse(gameTime, 0.49f, 5));
                float glowAngle = (float)gameTime.TotalGameTime.TotalSeconds * 0.5f;
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.sparkle, glowColor,
                    glowAngle, Helpers.SpriteCenter(sprite.sparkle), glowSize, SpriteEffects.None, 0);

                glowAngle = (float)gameTime.TotalGameTime.TotalSeconds  * -0.2f;
                glowSize = MathHelper.Lerp(0, 1.5f, item.selectTransition);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.sparkle, glowColor,
                    glowAngle, Helpers.SpriteCenter(sprite.sparkle), glowSize, SpriteEffects.None, 0);



                //draw the vista box.
                Vector2 vistaSize = Vector2.Lerp(new Vector2(0.7f, 1.3f),
                    new Vector2(0.9f, 1.3f), item.selectTransition);

                if (item.selectTransition >= 1)
                {
                    vistaSize = Vector2.Lerp(new Vector2(vistaSize.X - 0.01f, vistaSize.Y - 0.1f),vistaSize,
                        0.5f + Helpers.Pulse(gameTime, 0.49f, 10));
                }

                Color vistaColor = Color.Lerp(new Color(64, 64, 64), new Color(160, 120, 50), item.selectTransition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.vistaBox,
                    vistaColor,
                    itemAngle, new Vector2(sprite.vistaBox.Height / 2, sprite.vistaBox.Height / 2),
                    vistaSize, SpriteEffects.None, 0);

                //draw the square.
                Color squareColor = Color.White;

                if (item.inventoryItem != null)
                    squareColor = Helpers.ITEMCOLOR;

                Color darkenColor = Color.Lerp(Color.Black, squareColor, 0.5f);
                squareColor = Color.Lerp(
                    darkenColor,
                    squareColor,
                    item.selectTransition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.roundSquare, squareColor,
                    itemAngle, Helpers.SpriteCenter(sprite.roundSquare), 1, SpriteEffects.None, 0);

                if (item.inventoryItem == null)
                {
                    
                    if (menuItems.IndexOf(item) == 0)
                    {
                        //draw the "no" sign
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.noSign, Color.Black,
                            itemAngle, Helpers.SpriteCenter(sprite.cancel), 1, SpriteEffects.None, 0);
                    }
                    else
                    {
                        //draw the x icon.
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.cancel, Color.Black,
                            itemAngle, Helpers.SpriteCenter(sprite.cancel), 1, SpriteEffects.None, 0);
                    }
                }
                else if (item.inventoryItem.image != null)
                {
                    Color iconColor = Color.Lerp(Color.Gray, Color.White, item.selectTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, item.inventoryItem.image,
                        iconColor, itemAngle, Helpers.SpriteCenter(item.inventoryItem.image), 0.9f,
                        SpriteEffects.None, 0);
                }


                //selection cursor.
                if (item == selectedItem)
                {
                    float selectSize = 1.1f + Helpers.Pulse(gameTime, 0.08f, 10);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.roundSquareSelector, Color.Orange,
                        itemAngle, Helpers.SpriteCenter(sprite.roundSquareSelector), selectSize, SpriteEffects.None, 0);
                }

                //draw item text.
                float textSize = MathHelper.Lerp(0.9f, 1.15f, item.selectTransition);
                Vector2 textPos = itemPos;
                textPos.X += (float)(Math.Cos(itemAngle)  * ((sprite.roundSquare.Width/2) + 8));
                textPos.Y += (float)(Math.Sin(itemAngle) * ((sprite.roundSquare.Width / 2) + 8));

                Helpers.DrawOutline(FrameworkCore.Serif, item.text, textPos, Color.White, new Color(0,0,0,64),
                    itemAngle, new Vector2(0, LINESIZE.Y), textSize);

                if (item.inventoryItem != null)
                {
                    Helpers.DrawOutline(FrameworkCore.Serif, item.inventoryItem.description, textPos,
                        Color.Gray, new Color(0, 0, 0, 64),
                        itemAngle, Vector2.Zero, textSize);
                }
            }
        }


        
    }
}
