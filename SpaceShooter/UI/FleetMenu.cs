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
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
#endregion

namespace SpaceShooter
{
    public class FleetMenu : SysMenu
    {
        Vector2 LINESIZE;
        int GAPSIZE = 32;

        int selectedIndex = -1;

        int selectedInventoryIndex = 0;

        public FleetMenu()
        {
            darkenScreen = true;
            
            

            if (FrameworkCore.players[0].campaignShips.Count > 0)
            {
                selectedIndex = 0;
                UpdateTargetPositions();

                //force menu items to instantly snap to initial positions.
                ResetMenuPositions();
            }

            LINESIZE = FrameworkCore.Serif.MeasureString("Sample");

            
        }

        int mouseScrollTimer = 0;

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            UpdateMenuPositions(gameTime);
            UpdateMenuTransitions(gameTime);

            
#if WINDOWS
            handleMouse(FrameworkCore.players[0].inputmanager);

            mouseScrollTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            mouseScrollTimer = Math.Max(mouseScrollTimer, 0);
#endif
            
            //move stick left/right.
            if (FrameworkCore.players[0].campaignShips.Count > 0 && Transition >= 1)
            {

                int additionalButtons = 1;

                if (FrameworkCore.players.Count > 1)
                    additionalButtons = 2;

                bool mouseAtLeft = FrameworkCore.players[0].inputmanager.mousePos.X < 
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Width * 0.1f;

                bool mouseAtRight = FrameworkCore.players[0].inputmanager.mousePos.X > 
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Width * 0.9f;

#if XBOX
                mouseAtLeft = false;
                mouseAtRight = false;
#endif


                if (inputManager.sysMenuLeft || inputManager.mouseWheelUp ||
                    (mouseScrollTimer <= 0 && mouseAtLeft))
                {
                    mouseScrollTimer = 500;

                    if (selectedIndex <= 0)
                        selectedIndex = FrameworkCore.players[0].campaignShips.Count - 1;
                    else
                        selectedIndex = (int)MathHelper.Clamp(selectedIndex - 1, 0, FrameworkCore.players[0].campaignShips.Count - 1);

                    UpdateTargetPositions();

                    if (FrameworkCore.players[0].campaignShips.Count > 0)
                        FrameworkCore.PlayCue(sounds.click.whoosh);
                    //selectedInventoryIndex = 0;
                }
                if (inputManager.sysMenuRight || inputManager.mouseWheelDown ||
                    (mouseScrollTimer <= 0 && mouseAtRight))
                {
                    mouseScrollTimer = 500;

                    if (selectedIndex >= FrameworkCore.players[0].campaignShips.Count - 1)
                        selectedIndex = 0;
                    else
                        selectedIndex = (int)MathHelper.Clamp(selectedIndex + 1, 0, FrameworkCore.players[0].campaignShips.Count - 1);

                    UpdateTargetPositions();

                    if (FrameworkCore.players[0].campaignShips.Count > 0)
                        FrameworkCore.PlayCue(sounds.click.whoosh);
                    //selectedInventoryIndex = 0;
                }
                if (inputManager.sysMenuDown)
                {
                    if (FrameworkCore.players[0].campaignShips[selectedIndex] != null)
                    {
                        if (selectedInventoryIndex >= FrameworkCore.players[0].campaignShips[selectedIndex].upgradeArray.Length + additionalButtons - 1)
                            selectedInventoryIndex = 0;
                        else
                            selectedInventoryIndex = (int)MathHelper.Clamp(selectedInventoryIndex + 1, 0, FrameworkCore.players[0].campaignShips[selectedIndex].upgradeArray.Length + additionalButtons - 1);

                        FrameworkCore.PlayCue(sounds.click.select);
                    }
                }
                if (inputManager.sysMenuUp)
                {
                    if (FrameworkCore.players[0].campaignShips[selectedIndex] != null)
                    {
                        if (selectedInventoryIndex <= 0)
                            selectedInventoryIndex = FrameworkCore.players[0].campaignShips[selectedIndex].upgradeArray.Length + additionalButtons/*extra buttons*/ - 1;
                        else
                            selectedInventoryIndex = (int)MathHelper.Clamp(selectedInventoryIndex - 1, 0, FrameworkCore.players[0].campaignShips[selectedIndex].upgradeArray.Length + additionalButtons - 1);

                        FrameworkCore.PlayCue(sounds.click.select);
                    }
                }

                if (inputManager.buttonAPressed)
                {
                    if (FrameworkCore.players[0].campaignShips[selectedIndex] != null)
                    {
                        ClickButton();
                    }
                }

            }

            if (Transition >= 1 && (inputManager.OpenMenu || inputManager.kbBackspaceJustPressed || inputManager.kbSpace))
            {
                Deactivate();
            }


            base.Update(gameTime, inputManager);
        }

        private void ClickButton()
        {
            if (selectedInventoryIndex < 0)
                return;

            FrameworkCore.PlayCue(sounds.click.activate);

            if (selectedInventoryIndex <= 1)
            {
                //click on a inventorySlot.
                bool slotOccupied = (FrameworkCore.players[0].campaignShips[selectedIndex].upgradeArray[selectedInventoryIndex] != null);
                InventoryPopup popup = new InventoryPopup(Owner, slotOccupied);
                Owner.AddMenu(popup);

                ResetMenuPositions();
            }
            else
            {
                //click on an additionalButton.
                if (FrameworkCore.players.Count > 1 && selectedInventoryIndex == 2)
                {
                    //coop                                
                    FrameworkCore.players[0].campaignShips[selectedIndex].childShip =
                        !FrameworkCore.players[0].campaignShips[selectedIndex].childShip;
                }
                else
                    Deactivate();
            }
        }




        Vector2[] boxCenters = new Vector2[4]
        {new Vector2(-512,-512),
        new Vector2(-512,-512),
        new Vector2(-512,-512),
        new Vector2(-512,-512)};

        private void handleMouse(InputManager inputManager)
        {
            if (Transition < 1)
                return;

            if (inputManager.kbEnter)
                ClickButton();

            bool isHovering = false;
            for (int i = 0; i < boxCenters.Length; i++)
            {
                Point boxSize = new Point(80, 80);

                if (i >= 2)
                    boxSize = new Point(512, 60);

                Rectangle itemRect = new Rectangle(
                    (int)boxCenters[i].X - boxSize.X/2,
                    (int)boxCenters[i].Y - boxSize.Y/2,
                    boxSize.X,
                    boxSize.Y);

                if (itemRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    isHovering = true;

                    if (inputManager.mouseHasMoved)
                        selectedInventoryIndex = i;

                    if (inputManager.mouseLeftClick)
                        ClickButton();
                }
            }

            if (!isHovering)
            {
                if (inputManager.mouseHasMoved)
                    selectedInventoryIndex = -1;
            }
        }

        

        private void UpdateTargetPositions()
        {
            if (FrameworkCore.players[0].campaignShips.Count <= 0)
                return;

            Vector3 centerPos = FrameworkCore.players[0].lockCamera.CameraPosition;
            centerPos.Z -= 48;
            centerPos.Y += 7;

            Vector3 startPos = centerPos - new Vector3(selectedIndex * GAPSIZE, 0, 0);

            int curIndex = 0;
            foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
            {
                ship.targetMenuPos = startPos + new Vector3(curIndex * GAPSIZE, 0, 0);

                if (ship.shipData.displayDistanceModifier != 0)
                {
                    ship.targetMenuPos.Z -= ship.shipData.displayDistanceModifier;
                }

                curIndex++;
            }
        }

        private void ResetMenuPositions()
        {
            foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
            {
                ship.menuPos = ship.targetMenuPos;

                if (FrameworkCore.players[0].campaignShips.IndexOf(ship) == selectedIndex)
                    ship.menuHoverTransition = 1;
                else
                    ship.menuHoverTransition = 0;
            }
        }

        private void UpdateMenuTransitions(GameTime gameTime)
        {
            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(300).TotalMilliseconds);
            
            foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
            {
                if (FrameworkCore.players[0].campaignShips.IndexOf(ship) == selectedIndex)
                    ship.menuHoverTransition = MathHelper.Clamp(ship.menuHoverTransition + delta, 0, 1);
                else
                    ship.menuHoverTransition = MathHelper.Clamp(ship.menuHoverTransition - delta, 0, 1);                
            }
        }

        private void UpdateMenuPositions(GameTime gameTime)
        {
            if (FrameworkCore.players[0].campaignShips.Count <= 0)
                return;

            foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
            {
                Vector3 targetPos = ship.targetMenuPos;

                ship.menuPos = Vector3.Lerp(ship.menuPos, targetPos,
                    8 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            FrameworkCore.PlayCue(sounds.click.whoosh);

            base.Deactivate();
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {


#if WINDOWS
            FrameworkCore.players[0].inputmanager.ForceMouseCenter();
#endif
            FrameworkCore.PlayCue(sounds.click.whoosh);

            base.Activate();
        }

        public void ClearItem()
        {
            FrameworkCore.players[0].campaignShips[selectedIndex].upgradeArray[selectedInventoryIndex] = null;
        }

        public void ApplyItem(InventoryItem item)
        {
            FrameworkCore.players[0].campaignShips[selectedIndex].upgradeArray[selectedInventoryIndex] = item;
        }

        int bottomPos;

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();            

            int menuWidth = 1000;
            

            Color whiteColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);

            Vector2 menuPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - menuWidth / 2,
                90);

            menuPos.X += Helpers.PopLerp(Transition, 500, -80, 0);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, Resource.MenuFleet, menuPos, whiteColor,
                0,Vector2.Zero, 0.8f, SpriteEffects.None, 0);






            try
            {
                foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
                {
                    Matrix worldMatrix = Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds * 0.5f);
                    worldMatrix.Translation = ship.menuPos;

                    Vector2 textPos = Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, worldMatrix.Translation);


                    Color glowColor = Color.Lerp(new Color(0, 0, 0, 255), new Color(255, 255, 255, 64), ship.menuHoverTransition);
                    glowColor = Color.Lerp(Helpers.transColor(glowColor), glowColor, Transition);
                    float glowSize = MathHelper.Lerp(8, 4, ship.menuHoverTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, textPos, sprite.glow, glowColor,
                        0, Helpers.SpriteCenter(sprite.glow), glowSize, SpriteEffects.None, 0);


                    //Render the ship mesh.
                    if (TopOfStack() || FrameworkCore.players[0].campaignShips.IndexOf(ship) == selectedIndex)
                    {
                        Color teamColor = ship.childShip ? FrameworkCore.players[0].factionName.altColor : FrameworkCore.players[0].factionName.teamColor;
                        FrameworkCore.PlayerMeshRenderer.Draw(ship.shipData.modelname, worldMatrix, FrameworkCore.players[0].lockCamera,
                            teamColor, MathHelper.Lerp(0, 1, Transition));

                        Helpers.DrawTurrets(FrameworkCore.PlayerMeshRenderer, ship.shipData, worldMatrix, teamColor, Transition);
                    }




                    //draw the ship class name.
                    Vector2 topTextPos = textPos;
                    topTextPos.Y -= Helpers.SizeInPixels(FrameworkCore.players[0].lockCamera,
                        worldMatrix.Translation, ship.shipData.shipHeight / 2) + 32;
                    topTextPos.Y += Helpers.PopLerp(Transition, 100, -30, 0);
                    Color classColor = Color.Lerp(Helpers.transColor(Color.Gray), Color.Gray, Transition);
                    Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                            ship.shipData.name, topTextPos, classColor, 1);

                    //draw the ship name.
                    if (ship.captainName != null)
                    {
                        Color captainColor = Color.Lerp(Color.Gray, Color.White, ship.menuHoverTransition);
                        captainColor = Color.Lerp(Helpers.transColor(captainColor), captainColor, Transition);
                        Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                            ship.captainName, topTextPos + new Vector2(0, -LINESIZE.Y), captainColor, 1);
                    }

                    if (ship.veterancy > 0)
                    {
                        Color starColor = Color.Lerp(Color.Gray, Color.White, ship.menuHoverTransition);
                        Vector2 starPos = topTextPos + new Vector2(0, LINESIZE.Y * -2);
                        Helpers.DrawVeterancy(starPos, ship.veterancy, starColor);
                    }


                    //draw the upgrades boxes.
                    Vector2 bottomTextPos = textPos;
                    bottomTextPos.Y += Helpers.SizeInPixels(FrameworkCore.players[0].lockCamera,
                        worldMatrix.Translation, ship.shipData.shipHeight / 2) + sprite.roundSquare.Height / 2 + 48;
                    int bottom = DrawUpgrades(gameTime, ship, bottomTextPos);

                    if (FrameworkCore.players[0].campaignShips.IndexOf(ship) == selectedIndex)
                    {
                        if (Transition >= 1)
                            bottomPos = (int)MathHelper.Lerp(bottomPos, bottom, 0.05f);
                        else
                            bottomPos = bottom;
                    }
                }
            }
            catch
            {
            }


            int cargoAmount = 0;
            try
            {
                foreach (InventoryItem item in FrameworkCore.players[0].inventoryItems)
                {
                    bool isUsed = false;
                    foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
                    {
                        for (int i = 0; i < ship.upgradeArray.Length; i++)
                        {
                            if (ship.upgradeArray[i] == item)
                            {
                                isUsed = true;
                                break;
                            }
                        }
                    }

                    if (!isUsed)
                        cargoAmount++;
                }
            }
            catch
            {
            }

            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");
            titleVec.Y *= 0.8f;
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic,
                Resource.MenuFleetShips + " " + FrameworkCore.players[0].campaignShips.Count + "    " +
                Resource.MenuFleetCargo + " " + cargoAmount,
                menuPos + new Vector2(0, titleVec.Y), whiteColor,
                0, Vector2.Zero, 0.45f, SpriteEffects.None, 0);



            //draw the bottom buttons. ("give ship to player 2", "done")

            //  ..... hard coding this stuff.

            bool giveButtonSelected = false;
            bool doneButtonSelected = false;

            if (FrameworkCore.players.Count > 1)
            {
                //coop play.
                if (selectedInventoryIndex == 2)
                    giveButtonSelected = true;
                else if (selectedInventoryIndex == 3)
                    doneButtonSelected = true;
            }
            else
            {
                if (selectedInventoryIndex == 2)
                    doneButtonSelected = true;
            }

            Color desiredColor = FrameworkCore.players[0].campaignShips[selectedIndex].childShip ?
                FrameworkCore.players[1].factionName.altColor :
                FrameworkCore.players[0].factionName.teamColor;

            buttonColor = Color.Lerp(buttonColor, desiredColor, 0.08f);

            Vector2 switchButtonPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,

            bottomPos+28 +sprite.vistaBox.Height/2+ sprite.vistaBox.Height*2);

            switchButtonPos.Y += Helpers.PopLerp(Transition, -300, 70, 0);

            string doneString = Resource.MenuDone;
#if WINDOWS
            doneString += " " + Helpers.GetShortcutAltCancel();
#endif

            try
            {
                DrawAdditionalButton(gameTime, switchButtonPos, buttonColor, doneString, doneButtonSelected);

                if (FrameworkCore.players.Count > 1)
                {
                    boxCenters[3] = switchButtonPos;
                }
                else
                {
                    boxCenters[2] = switchButtonPos;
                    boxCenters[3] = new Vector2(-512, -512); //hide it away.
                }

                if (FrameworkCore.players.Count > 1)
                {
                    //the "give ship" button.
                    switchButtonPos.Y -= sprite.vistaBox.Height + 8;

                    string playerName = "";

                    if (!FrameworkCore.players[0].campaignShips[selectedIndex].childShip)
                    {
                        playerName = FrameworkCore.players[1].commanderName;
                    }
                    else
                    {
                        playerName = FrameworkCore.players[0].commanderName;
                    }

                    string giveText = string.Format(Resource.MenuFleetGiveTo, playerName);
                    DrawAdditionalButton(gameTime, switchButtonPos, buttonColor, giveText, giveButtonSelected);

                    boxCenters[2] = switchButtonPos;
                }
            }
            catch
            {
            }

#if XBOX
            DrawLegends();
#endif



            DrawNextShipArrows(gameTime);
        }

        private void DrawNextShipArrows(GameTime gameTime)
        {
            if (FrameworkCore.players[0].campaignShips.Count <= 1)
                return;

            Color arrowColor = Color.Goldenrod;
            arrowColor = Color.Lerp(Helpers.transColor(arrowColor), arrowColor, Transition);

            float arrowTransition = 0.5f + Helpers.Pulse(gameTime, 0.49f, 4);

            float arrowSize = MathHelper.Lerp(1.1f, 0.9f, arrowTransition);

            Vector2 leftPos = new Vector2(100, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.4f);
            leftPos.X += MathHelper.Lerp(0, 24, arrowTransition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, leftPos, sprite.arrow, arrowColor,
                0, new Vector2(5, 16), arrowSize, SpriteEffects.None, 0);


            Vector2 rightPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100,
                leftPos.Y);
            rightPos.X -= MathHelper.Lerp(0, 24, arrowTransition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rightPos, sprite.arrow, arrowColor,
                3.14f, new Vector2(5, 16), arrowSize, SpriteEffects.None, 0);
        }

        private void DrawLegends()
        {
            string aText = Resource.MenuFleetEquip;

            if (selectedInventoryIndex > 1)
                aText = Resource.MenuOK;

            Helpers.DrawLegend(aText, sprite.buttons.a, Transition);


            float z = Helpers.DrawLegendLeft(Resource.MenuFleetItemSelect, sprite.buttons.upDown, Transition);

            if (FrameworkCore.players[0].campaignShips.Count > 0)
            {
                Helpers.DrawLegendLeftAt(Resource.MenuFleetShipSelect, sprite.buttons.leftRight, Transition, z + 32);
            }
        }

        Color buttonColor=OldXNAColor.TransparentWhite;

        private void DrawAdditionalButton(GameTime gameTime, Vector2 pos, Color color, string text, bool isSelected)
        {
            Color boxColor = isSelected ?
                color :
                Color.Lerp(Color.Black, color, 0.5f);

            boxColor = Color.Lerp(Helpers.transColor(boxColor), boxColor, Transition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.vistaBox, boxColor,
                0, Helpers.SpriteCenter(sprite.vistaBox), 1, SpriteEffects.None, 0);

            if (isSelected && Transition >= 1)
            {
                float selectorSize = 0.9f + Helpers.Pulse(gameTime, 0.05f, 8);
                Vector2 selectPos = pos;
                selectPos.X -= sprite.vistaBox.Width / 2 - sprite.roundSquareSelectorHalf1.Width + 8;
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, selectPos, sprite.roundSquareSelectorHalf1,
                    Color.Orange,
                    0, new Vector2(sprite.roundSquareSelectorHalf1.Width, sprite.roundSquareSelectorHalf1.Height / 2),
                    selectorSize, SpriteEffects.None, 0);

                selectPos = pos;
                selectPos.X += sprite.vistaBox.Width / 2 - sprite.roundSquareSelectorHalf2.Width + 8;
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, selectPos, sprite.roundSquareSelectorHalf2,
                    Color.Orange,
                    0, new Vector2(0, sprite.roundSquareSelectorHalf2.Height / 2),
                    selectorSize, SpriteEffects.None, 0);
            }

            Color textColor = isSelected ? Color.Orange : Color.White;

            Vector2 textPos = pos;
            textPos.Y -= LINESIZE.Y / 2;

            textColor = Color.Lerp(Helpers.transColor(textColor), textColor, Transition);

            float textSize = isSelected ? 1.05f + Helpers.Pulse(gameTime, 0.05f, 8) : 1;

            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, text,
                pos, textColor, textSize);
        }
        

        private int DrawUpgrades(GameTime gameTime, FleetShip ship, Vector2 pos)
        {
            int lowestY = 0;

            pos.Y += Helpers.PopLerp(Transition, -100, 30, 0);

            Color whiteColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);

            Vector2 boxPos = pos;
            for (int i = 0; i < ship.upgradeArray.Length; i++)
            {
#if WINDOWS
                if (FrameworkCore.players[0].campaignShips.IndexOf(ship) == selectedIndex)
                    boxCenters[i] = boxPos;
#endif
                Color boxColor = new Color(64, 64, 64);

                if (ship.upgradeArray[i] != null)
                    boxColor = Helpers.ITEMCOLOR;

                boxColor = Color.Lerp(Helpers.transColor(boxColor), boxColor, Transition);

                //THE SQUARE.
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxPos, sprite.roundSquare, boxColor,
                    0, Helpers.SpriteCenter(sprite.roundSquare), 1, SpriteEffects.None, 0);

                if (selectedInventoryIndex == i)
                {
                    Color selectColor = Color.Lerp(Helpers.transColor(Color.Orange), Color.Orange, Transition);
                    selectColor = Color.Lerp(Helpers.transColor(selectColor), selectColor, ship.menuHoverTransition);
                    float selectSize = (TopOfStack() ? (1.1f + Helpers.Pulse(gameTime, 0.05f, 8)) : (1.1f));

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxPos, sprite.roundSquareSelector, selectColor,
                        0, Helpers.SpriteCenter(sprite.roundSquareSelector), selectSize, SpriteEffects.None, 0);
                }

                Vector2 boxTextPos = boxPos;
                boxTextPos.X += sprite.roundSquare.Width/2 + 16;
                boxTextPos.Y -= LINESIZE.Y;

                if (ship.upgradeArray[i] != null)
                {
                    if (ship.upgradeArray[i].image != null)
                    {
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxPos, ship.upgradeArray[i].image,
                            whiteColor, 0, Helpers.SpriteCenter(ship.upgradeArray[i].image), 0.9f,
                            SpriteEffects.None, 0);
                    }

                    if (ship.upgradeArray[i].name != null)
                    {
                        FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, ship.upgradeArray[i].name, boxTextPos,
                            whiteColor);
                    }

                    boxTextPos.Y += LINESIZE.Y;
                    if (ship.upgradeArray[i].description != null)
                    {
                        Color descriptionColor = Color.Lerp(Helpers.transColor(Color.Gray), Color.Gray, Transition);
                        FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, ship.upgradeArray[i].description, boxTextPos,
                            descriptionColor);
                    }
                }
                else
                {
                    Color slotColor = Color.Lerp(new Color(80,80,80,0), new Color(80,80,80), Transition);
                    boxTextPos.Y += LINESIZE.Y/2;
                    FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuSkirmishEmptySlot, boxTextPos,
                        slotColor);
                }

                lowestY = (int)boxPos.Y;
                boxPos.Y += sprite.roundSquare.Height + 16;                
            }

            return lowestY;
        }

        


     }
}
