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
    class ShipMenu : Menu
    {
        public ShipMenu(Game game, PlayerCommander owner)
            : base(game, owner)
        {
        }

        private void UpdateItemPositions()
        {
            Vector2 screenViewport = owner.viewportSize;

            float gapSize = (sprite.tab.Width) + 24;
            Vector2 startPos = Vector2.Zero;
            startPos.Y = 100;            
            //startPos.X = screenViewport.X / 2;
            startPos.X = screenViewport.X - 100;
            startPos.X -= gapSize * (float)menuItems.Count;
            //startPos.X += gapSize / 2;

            foreach (MenuItem item in menuItems)
            {
                item.basePosition = startPos;
                item.basePosition.X += gapSize * menuItems.IndexOf(item);
            }
        }

        private void UpdateTargetPositions()
        {
            Vector2 screenViewport = owner.viewportSize;

            float gapSize = (sprite.tab.Width) + 24;
            Vector2 startPos = Vector2.Zero;
            startPos.Y = 100;
            //startPos.X = screenViewport.X / 2;
            startPos.X = screenViewport.X - 100; //the margin on the right side of the screen.
            startPos.X -= gapSize * (float)menuItems.Count;
            //startPos.X += gapSize / 2;

            foreach (MenuItem item in menuItems)
            {
                item.targetPosition = startPos;
                item.targetPosition.X += gapSize * menuItems.IndexOf(item);

                if (selectedItem == null)
                    continue;

                if (selectedItem.gameEffect == null)
                    continue;

                if (selectedItem.gameEffect.description == null)
                    continue;

                if (menuItems.IndexOf(selectedItem) > menuItems.IndexOf(item))
                {
                    Vector2 descVec = FrameworkCore.Serif.MeasureString(item.gameEffect.description);
                    item.targetPosition.X -= descVec.Y;
                }
            }
        }

        private void UpdateItemTransitions(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (MenuItem item in menuItems)
            {
                item.position = Vector2.Lerp(item.position, item.targetPosition, 5f * dt);
            }
        }


        

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            UpdateTargetPositions();
            UpdateItemTransitions(gameTime);

            if (inputManager.camResetClick)
                owner.WarpCameraToShip(gameTime, owner.selectedShip);


#if WINDOWS
            if (Transition >= 1 && (inputManager.kbSpace || inputManager.kbBackspaceJustPressed))
                Deactivate();

            if (Helpers.UpdateTiltedMouseMenu(menuItems, owner.CursorPos, 0.85f,
                false,
                new Point(14, 70),
                FrameworkCore.Serif,
                true,
                inputManager,
                selectedItem,
                out mouseIsHovering, out selectedItem))
            {
                if (selectedItem != lastSelectedItem)
                {
                    lastSelectedItem = selectedItem;

                    if (selectedItem != null)
                    {
                        FrameworkCore.PlayCue(sounds.click.select);                        
                    }
                }
            }


            if (inputManager.kb1Pressed)
            {
                SelectAndActivate(0, inputManager);
            }
            else if (inputManager.kb2Pressed)
            {
                SelectAndActivate(1, inputManager);
            }
            else if (inputManager.kb3Pressed)
            {
                SelectAndActivate(2, inputManager);
            }
            else if (inputManager.kb4Pressed)
            {
                SelectAndActivate(3, inputManager);
            }
#endif 

            

            base.Update(gameTime, inputManager);
        }

        private void SelectAndActivate(int index, InputManager inputManager)
        {
            //this can get messed up if we try to access a menuitem out of index range.
            try
            {
                selectedItem = menuItems[index];
            }
            catch
            {
                return;
            }

            ActivateItem(inputManager);
        }

        MenuItem lastSelectedItem = null;


        public override void Activate()
        {
            foreach (MenuItem item in menuItems)
            {
                item.position = Vector2.Zero;
            }

            UpdateItemPositions();            

            base.Activate();

            if (owner.selectedShip != null)
            {
                if (owner.selectedShip.OrderEffect != null)
                {
                    //default to "done" option.
                    selectedItem = menuItems[menuItems.Count - 1];
                }
            }
        }

        public override void Deactivate()
        {
            owner.selectedShip = null;
            owner.hoverShip = null;

            

            base.Deactivate();
        }


        public void Draw(GameTime gameTime)
        {
            if (Transition <= 0)
                return;
            

            Vector2 screenViewport = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            float smoothTransition = MathHelper.SmoothStep(0, 1, Transition);

            Vector2 originPosition = new Vector2(screenViewport.X / 2, screenViewport.Y / 2);
            Vector2 finalPosition = menuItems[menuItems.Count - 1].position + new Vector2(sprite.tab.Width,0);            
            Vector2 rbPosition = Vector2.Lerp(originPosition, finalPosition, smoothTransition);


            DrawInfoBox(gameTime, rbPosition);

            if (!owner.mouseEnabled)
                DrawShoulders(gameTime, rbPosition);

            DrawTabs(gameTime, rbPosition);

            
        }

        private void DrawUpgrades(SpaceShip ship, Vector2 pos)
        {
            if (ship.fleetShipInfo == null)
                return;

            if (ship.fleetShipInfo.upgradeArray == null)
                return;

            Vector2 textVec = FrameworkCore.Serif.MeasureString("SAMPLE");

            pos.Y -= 16; //buffer from the infobox rectangle.
            pos.Y -= textVec.Y * 2;

            Vector2 titlePos = pos + new Vector2(0,-textVec.Y - 4);

            bool drawTitle = false;

            for (int i = 0; i < ship.fleetShipInfo.upgradeArray.Length; i++)
            {
                if (ship.fleetShipInfo.upgradeArray[i] == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect == null)
                    continue;

                Helpers.DrawOutline(FrameworkCore.Serif, ship.fleetShipInfo.upgradeArray[i].description,
                    pos, Color.White, Color.Black, 0, Vector2.Zero, Transition);

                pos.Y += textVec.Y;
                drawTitle = true;
            }

            if (drawTitle && owner != null && owner.TeamColor != null)
            {
                Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuUpgrades,
                    titlePos, owner.TeamColor, new Color(0,0,0,128), 0, Vector2.Zero, Transition);
            }
        }

        private void DrawInfoBox(GameTime gameTime, Vector2 originPos)
        {
            if (owner.lastSelectedShip == null)
                return;

            int NumberOfLines = 5;

            float textSize = MathHelper.Lerp(0,1, Transition);

            SpaceShip ship = owner.lastSelectedShip;

            if (ship == null)
                return;


            Vector2 textVec = FrameworkCore.Serif.MeasureString("SAMPLE");
            float gapSize = textVec.Y + 5;


            Vector2 textPos = originPos;
            //textPos.X = MathHelper.Lerp(originPos.X, 70, Transition);
            textPos.X = Helpers.PopLerp(Transition, originPos.X, 20, 130);
            textPos.Y = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height
                - 96 /* safescreen */ - (NumberOfLines+1.5f) /* number of lines+1 */ * gapSize);


            DrawUpgrades(ship, textPos);


            int longestString = 0;

            //----------------------------------------------------------------------------
            //Rectangle bg.
            Vector2 boxPos = textPos;

            //----------------------------------------------------------------------------

            //captain name.
            Vector2 captainNamePos = textPos;
            Vector2 captainNameVec = FrameworkCore.Serif.MeasureString(ship.CaptainName);
            if (captainNameVec.X > longestString)
                longestString = (int)captainNameVec.X;
            

            //----------------------------------------------------------------------------

            //ship class.
            textPos.Y += gapSize;
            Vector2 classNamePos = textPos;
            Vector2 classNameVec = FrameworkCore.Serif.MeasureString(ship.shipName);
            if (classNameVec.X > longestString)
                longestString = (int)classNameVec.X;
            

            

            //----------------------------------------------------------------------------

            //health string.            
            textPos.Y += gapSize;
            string healthString = "" + Math.Ceiling(ship.Health) + "/" + ship.MaxDamage;
            Vector2 healthPos = textPos;


            //bar.
            int barGap = 8;
            Vector2 healthVec = FrameworkCore.Serif.MeasureString(healthString);
            float barAmount = ship.Health / ship.MaxDamage;
            int barWidth = (int)(ship.MaxDamage / 5f);            
            barWidth = (int)MathHelper.Lerp(0, barWidth, Transition);
            int barHeight = 12;
            float barOffset = barWidth / 2f;
            barOffset += healthVec.X;
            barOffset += barGap;
            Color barColor = Color.Lerp(Helpers.transColor(ship.owner.TeamColor), ship.owner.TeamColor, Transition);

            Vector2 barPos = textPos + new Vector2(barOffset, 12);

            int barTotalLength = (int)(barWidth + healthVec.X + barGap);

            if (barTotalLength > longestString)
                longestString = barTotalLength;

            //kills
            //----------------------------------------------------------------------------
            textPos.Y += gapSize;
            Vector2 killPos = textPos;

            //missions
            //----------------------------------------------------------------------------
            textPos.Y += gapSize;
            Vector2 missionPos = textPos;


            //----------------------------------------------------------------------------

            //background box.
            Color boxColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 192), Transition);
            Rectangle boxRect = new Rectangle((int)boxPos.X, (int)boxPos.Y, longestString,
                (int)(NumberOfLines * gapSize));
            boxRect.Inflate(16, 8);
            boxRect.Width = (int)MathHelper.Lerp(1, boxRect.Width, Transition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxRect, sprite.giantRectangle, boxColor);



            //----------------------------------------------------------------------------
            // -- DRAW --
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, ship.CaptainName,
                captainNamePos, Color.White, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, ship.shipName,
                classNamePos, new Color(140, 140, 140),
                0, Vector2.Zero, textSize, SpriteEffects.None, 0);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, healthString,
                healthPos, Color.White, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);

            Helpers.DrawBar(barPos, barAmount, barWidth, barHeight, 1,
                            barColor, new Color(255, 255, 255, 64));

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuKills + " " + ship.fleetShipInfo.stats.kills,
                killPos, Color.White,
                0, Vector2.Zero, textSize, SpriteEffects.None, 0);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuVeterancy,
                missionPos, Color.White,
                0, Vector2.Zero, textSize, SpriteEffects.None, 0);

            if (ship.fleetShipInfo == null)            
                return;

            if (ship.fleetShipInfo.veterancy <= 0)
                return;

            Vector2 stringLength = FrameworkCore.Serif.MeasureString(Resource.MenuVeterancy);

            for (int i = 0; i < ship.fleetShipInfo.veterancy; i++)
            {
                Vector2 starPos = missionPos;
                starPos.Y += stringLength.Y / 2f;
                starPos.X += stringLength.X;
                starPos.X += sprite.star.Width / 1.5f;
                starPos.X += (i * (sprite.star.Width - 5));
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, starPos,
                    sprite.star, Color.White, 0,
                    Helpers.SpriteCenter(sprite.star), 1, SpriteEffects.None, 0);
            }
        }

        private void DrawShoulders(GameTime gameTime, Vector2 originPos)
        {
            //draw RB
            float buttonAngle = MathHelper.Lerp(1.5f, 0, Transition);
            Color shoulderColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Rectangle buttonRect = sprite.buttons.rb;
            Vector2 rbPos = originPos;
            rbPos = Vector2.Lerp(rbPos, rbPos + new Vector2(0, 7), rbTransition);
            //rbPos.X += 32;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rbPos, buttonRect, shoulderColor, buttonAngle,
                Helpers.SpriteCenter(buttonRect), 1, SpriteEffects.None, 0);

            //draw LB
            if (Transition > 0)
            {
                buttonRect = sprite.buttons.lb;
                Vector2 finalPosition = menuItems[0].position;
                finalPosition.X -= buttonRect.Width;
                float smoothTransition = MathHelper.SmoothStep(0, 1, Transition);
                Vector2 lbPosition = Vector2.Lerp(originPos, finalPosition, smoothTransition);
                //lbPosition.X -= 32;
                lbPosition.Y = originPos.Y;
                lbPosition.Y = MathHelper.Lerp(lbPosition.Y, lbPosition.Y + 7, lbTransition);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, lbPosition, buttonRect, shoulderColor, buttonAngle,
                    Helpers.SpriteCenter(buttonRect), 1, SpriteEffects.None, 0);
            }
        }

        private void DrawTabs(GameTime gameTime, Vector2 originPos)
        {
            float dt = (float)gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(dt * 6);

            int index = 1;
            foreach (MenuItem item in menuItems)
            {
                float smoothTransition = MathHelper.SmoothStep(0, 1, Transition);
                Vector2 itemPos = Vector2.Lerp(originPos, item.position, smoothTransition);
                Color tabColor = Color.Lerp(Helpers.transColor(owner.TeamColor), owner.TeamColor, Transition);
                float tabSize = MathHelper.Lerp(0, 0.9f, Transition);
                itemPos.Y = originPos.Y;
                

                float smoothItemTransition = MathHelper.SmoothStep(0, 1, item.selectTransition);
                //itemPos.Y += MathHelper.Lerp(0, -16, smoothItemTransition);
                //itemPos.Y += Helpers.PopLerp(item.selectTransition, 0, -32, -16);

                tabColor = Color.Lerp(tabColor, Color.White, item.selectTransition);
                tabSize = Helpers.PopLerp(item.selectTransition, tabSize, tabSize + 0.35f, tabSize + 0.2f);

                if (item.selectTransition > 0)
                {
                    tabSize = MathHelper.Lerp(tabSize - 0.1f, tabSize, pulsate);
                }

                Rectangle tabRect = sprite.icons.circle;
                float tabAngle = 0;

                bool ShipHasThisOrder = false;
                if (owner.lastSelectedShip.OrderEffect != null && item.gameEffect != null &&
                        owner.lastSelectedShip.OrderEffect.ToString() == item.gameEffect.ToString())
                {
                    ShipHasThisOrder = true;
                }

                if (item.selectTransition > 0)
                {
                    tabRect = sprite.icons.circleInvert;

                    if (ShipHasThisOrder)
                    {
                        tabRect = sprite.icons.circleInvertDotted;
                        tabAngle = (float)gameTime.TotalGameTime.TotalSeconds;
                    }
                }
                else
                {
                    if (ShipHasThisOrder)
                    {
                        tabRect = sprite.icons.circleDotted;
                        tabAngle = (float)gameTime.TotalGameTime.TotalSeconds;
                    }
                }


                if (item.activateTransition < 1)
                {
                    float glowSize = MathHelper.Lerp(2, 4, item.activateTransition);
                    Color glowColor = Color.Lerp(Color.White, OldXNAColor.TransparentWhite, item.activateTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.glow, glowColor, 0,
                        Helpers.SpriteCenter(sprite.glow), glowSize, SpriteEffects.None, 0);
                }


                //the tab background.
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, tabRect, tabColor, tabAngle,
                    Helpers.SpriteCenter(tabRect), tabSize, SpriteEffects.None, 0);
                

                //icon
                float iconSize = Helpers.PopLerp(item.selectTransition, 0.85f, 1.5f, 1.1f);
                float iconRotation = MathHelper.Lerp(-0.3f, 0, smoothItemTransition);
                Color iconColor = Color.Lerp(Color.White, Color.Black, item.selectTransition);
                if (item.selectTransition > 0)
                {
                    iconSize = MathHelper.Lerp(iconSize - 0.1f, iconSize + 0.1f, pulsate);
                }
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, item.iconRect, iconColor, iconRotation,
                    Helpers.SpriteCenter(item.iconRect), iconSize, SpriteEffects.None, 0);


                //the text attached to the tab.
                Vector2 textPos = itemPos;
                textPos.Y += sprite.tab.Height * 0.5f;
                textPos.Y -= 14;
                textPos.X += 44;



                Color fontColor = Color.Lerp(Helpers.transColor(owner.TeamColor), owner.TeamColor, Transition);
                fontColor = Color.Lerp(fontColor, Color.Black, item.selectTransition);

                float fontAngle = MathHelper.Lerp(2.4f, 0.85f, smoothTransition);
                float fontSize = MathHelper.Lerp(0, 1, Transition);
                //fontSize += MathHelper.Lerp(0, 0.15f, smoothItemTransition);

                if (item.selectTransition > 0)
                {
                    textPos.X += (float)(Math.Cos(fontAngle) * pulsate *4);
                    textPos.Y += (float)(Math.Sin(fontAngle) * pulsate *4);
                }


                //the rectangle background on the text.

                string tabText = item.text;

#if WINDOWS
                if (owner != null && owner.mouseEnabled)
                {
                    tabText = "" + index + ". " + item.text;
                }
#endif

                Vector2 fontVec = FrameworkCore.Serif.MeasureString(tabText);
                Rectangle fontRect = new Rectangle((int)textPos.X, (int)textPos.Y,
                    (int)fontVec.X, (int)fontVec.Y);
                fontRect.Inflate(8, 0);
                fontRect.Y -= 6;
                fontRect.Width += 8;
                Color rectColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, Transition);
                rectColor = Color.Lerp(rectColor, Color.White, item.selectTransition);
                if (Transition > 0)
                {
                    fontRect.Width = (int)MathHelper.Lerp(0, fontRect.Width, Transition);
                }

                

                //checkmark.
                if (ShipHasThisOrder)
                {
                    Vector2 checkPos = textPos;
                    float checkSize = Helpers.PopLerp(item.activateTransition, 0.5f, 1.5f, 1.0f);
                    float checkDist = Helpers.PopLerp(item.activateTransition, fontVec.X - 24, fontVec.X + 16, fontVec.X);


                    checkPos.X += (float)(Math.Cos(fontAngle) * checkDist);
                    checkPos.Y += (float)(Math.Sin(fontAngle) * checkDist);

                    checkPos += new Vector2(7, 23);

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, checkPos, sprite.checkmark,
                        Color.Goldenrod, fontAngle, Helpers.SpriteCenter(sprite.checkmark), checkSize,
                        SpriteEffects.None, 0);
                }

                //the Rectangle.
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, fontRect, sprite.tabRectangle, rectColor,
                    fontAngle, Vector2.Zero, SpriteEffects.None, 0);

                //the Text.
                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, tabText, textPos, fontColor, fontAngle,
                    Vector2.Zero, fontSize, SpriteEffects.None, 0);


                //DescriptionText
                DrawDescription(item, textPos, fontAngle, fontVec, fontSize);

                //Color descriptionColor = Color.Lerp(owner.TeamColor, Color.Black, 0.5f);
                //Helpers.DrawDescription(gameTime, item, Transition, descriptionColor);

                float legendTransition = Math.Min(item.selectTransition, Transition);
                if (item.gameEffect != null && item.gameEffect.name != null)
                {
                    string legendText = "";
                    if (ShipHasThisOrder)
                        legendText = Resource.OrderDeactivate;
                    else
                        legendText = Resource.OrderActivate + " " + item.gameEffect.name;

                    if (!owner.mouseEnabled)
                    {
                        Helpers.DrawLegend(legendText, sprite.buttons.a, legendTransition);                        
                    }
                }
                else
                {
                    //DONE TEXT.
                    if (!owner.mouseEnabled)
                    {
                        Helpers.DrawLegend(item.text, sprite.buttons.a, legendTransition);
                    }
                }

#if DEBUG
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, item.hitBox,
                    sprite.blank, new Color(255,0,0,128));

                
                Helpers.DrawOutline(menuItems.IndexOf(item).ToString(), item.hitCursor);
#endif
                index++;
            }
        }

        private void DrawDescription(MenuItem item, Vector2 textPos, float fontAngle, Vector2 fontVec, float fontSize)
        {
            if (item.selectTransition > 0 && item.gameEffect != null)
            {
                Vector2 descriptionPos = textPos;
                descriptionPos.X += (float)(Math.Cos(fontAngle + 1.57f) * (fontVec.Y + 5));
                descriptionPos.Y += (float)(Math.Sin(fontAngle + 1.57f) * (fontVec.Y + 5));
                Color descriptionColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, item.selectTransition);

                descriptionPos.X -= 6;
                descriptionPos.Y -= 6;

                float descSize = MathHelper.Lerp(0, 1, item.selectTransition);
                descSize = MathHelper.Lerp(0, descSize, Transition);

                Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0,0,0,128), item.selectTransition);
                //FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, item.gameEffect.description, descriptionPos, descriptionColor,
                    //fontAngle, Vector2.Zero, descSize, SpriteEffects.None, 0);
                Helpers.DrawOutline(FrameworkCore.Serif, item.gameEffect.description, descriptionPos, descriptionColor,
                    bgColor, fontAngle, Vector2.Zero, descSize);
            }
        }

        


    }
}
