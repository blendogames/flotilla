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
    class PlaybackMenu : Menu
    {
        float playbackTime = 0;
        float playSpeed = 0;

        int numberDisplayWidth = 0;

        public PlaybackMenu(Game game, PlayerCommander owner)
            : base(game, owner)
        {
            buttonPositions = new Vector2[3];
            
        }

        public void Initialize()
        {
            Vector2 numberVec = FrameworkCore.Serif.MeasureString("00:00");
            numberDisplayWidth = (int)numberVec.X;

            
        }

        #region Update
        float lbTimer = 0;
        float rbTimer = 0;


        Vector2 donePosition;
        bool doneHover;

        Vector2[] buttonPositions;
        int buttonHover;
        int buttonSelected;
        Rectangle barRectangle;

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            HandleMouse(inputManager);
#endif

            if (Math.Abs(playSpeed) > 0 || rbTimer >= 1 || lbTimer >= 1)
            {
                float adjustedSpeed = playSpeed;
                if (rbTimer >= 1)
                {
                    playSpeed = REGULARSPEED;
                    adjustedSpeed = FASTSPEED;
                }
                else if (lbTimer >= 1)
                {
                    playSpeed = -REGULARSPEED;
                    adjustedSpeed = -FASTSPEED;
                }

                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromSeconds(Math.Abs(adjustedSpeed)).TotalSeconds);

                if (playSpeed > 0)
                    playbackTime = Math.Min(playbackTime + delta, FrameworkCore.playbackSystem.WorldTimer);
                else
                    playbackTime = Math.Max(playbackTime - delta, 0);

                if (playbackTime >= FrameworkCore.playbackSystem.WorldTimer && playSpeed > 0)
                    playSpeed = 0;
                else if (playbackTime <= 0 & playSpeed < 0)
                    playSpeed = 0;
            }

            if (inputManager.menuNextHeld && inputManager.menuPrevHeld)
            {
                playSpeed = 0;
            }
            else
            {
                if (inputManager.playbackBackwardPress)
                {
                    DecreaseSpeed();
                }
                else if (inputManager.playbackForwardPress || inputManager.kbSpace)
                {
                    IncreaseSpeed();
                }

                if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed ||
                    inputManager.kbSpace)
                {
                    Deactivate();
                }
            }

            if (inputManager.playbackBackwardHold)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(180).TotalMilliseconds);

                lbTimer = MathHelper.Clamp(lbTimer + delta, 0, 1);
            }
            else if (lbTimer > 0)
                lbTimer = 0;

            if (inputManager.playbackForwardHold)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(180).TotalMilliseconds);

                rbTimer = MathHelper.Clamp(rbTimer + delta, 0, 1);
            }
            else if (rbTimer > 0)
                rbTimer = 0;
            
            UpdateShips(gameTime);
        }
#endregion

        private void HandleMouse(InputManager inputManager)
        {
            Rectangle doneRect = new Rectangle(
                (int)donePosition.X - 64,
                (int)donePosition.Y - 24,
                128,
                48);
            if (doneRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
            {
                //player  clicked the done button.
                if (inputManager.mouseLeftClick)
                    Deactivate();

                doneHover = true;
            }
            else
                doneHover = false;



            if (barRectangle.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y) &&
                inputManager.mouseLeftHeld && FrameworkCore.playbackSystem.WorldTimer > 0)
            {
                playSpeed = 0;

                float trans = (inputManager.mousePos.X - barRectangle.X) /
                    (barRectangle.Width);



                playbackTime = MathHelper.Lerp(0, FrameworkCore.playbackSystem.WorldTimer, trans);

                return;
            }

            bool isHovering = false;
            for (int i = 0; i < buttonPositions.Length; i++)
            {
                Rectangle buttonRect = new Rectangle(
                    (int)buttonPositions[i].X - 32,
                    (int)buttonPositions[i].Y - 32,
                    64, 64);

                if (buttonRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    isHovering = true;
                    buttonHover = i;


                    if (inputManager.mouseLeftClick)
                    {
                        //player is clicking pause/play button.
                        if (i == 1)
                        {
                            if (playSpeed == 0)
                                IncreaseSpeed();
                            else
                                playSpeed = 0;
                        }
                    }

                    //F.F./R.W. buttons.
                    if (inputManager.mouseLeftHeld)
                    {
                        if (i == 0)
                        {
                            playSpeed = -FASTSPEED;
                        }
                        else if (i == 2)
                        {
                            playSpeed = FASTSPEED;
                        }
                    }
                    else
                    {
                        if (playSpeed >= FASTSPEED)
                            playSpeed = REGULARSPEED;
                        else if (playSpeed <= -FASTSPEED)
                            playSpeed = -REGULARSPEED;
                    }
                }
            }

            if (!isHovering)
                buttonHover = -1;

            if (playSpeed == REGULARSPEED)
                buttonSelected = 1;
            else if (playSpeed >= FASTSPEED && playbackTime < FrameworkCore.playbackSystem.WorldTimer)
                buttonSelected = 2;
            else if (playSpeed <= -FASTSPEED && playbackTime > 0)
                buttonSelected = 0;
            else
                buttonSelected = -1;
        }

        #region ShipUpdate
        private void UpdateShips(GameTime gameTime)
        {
            if (!HasFocus)
                return;

            for (int i = 0; i < FrameworkCore.playbackSystem.ItemIndex; i++)
            {
                PlaybackItem ship = FrameworkCore.playbackSystem.PlaybackItems[i];

                if (ship.isStatic)
                {
                    if (ship.moveEvents[0] != null)
                    {
                        ship.position = ship.moveEvents[0].position;
                        ship.rotation = ship.moveEvents[0].rotation;
                    }

                    continue;
                }


                for (int k = 0; k < ship.moveArrayIndex; k++)
                {
                    PlaybackMoveEvent moveEvent = ship.moveEvents[k];

                    if (moveEvent.timeStamp == playbackTime)
                    {
                        
                        ship.position = moveEvent.position;
                        ship.rotation = moveEvent.rotation;
                        continue;
                    }

                    if (k < ship.moveArrayIndex - 1)  //NOT the last node.
                    {
                        if (moveEvent.timeStamp < playbackTime && ship.moveEvents[k+1].timeStamp > playbackTime)
                        {
                            //this is the interval we are interested in.

                            float adjustedPlaybackTime = playbackTime - moveEvent.timeStamp;

                            float intervalTransition = adjustedPlaybackTime /
                                (ship.moveEvents[k + 1].timeStamp - moveEvent.timeStamp);

                            ship.position = Vector3.Lerp(
                                moveEvent.position,
                                ship.moveEvents[k + 1].position,
                                intervalTransition);

                            ship.rotation = Quaternion.Lerp(
                                moveEvent.rotation,
                                ship.moveEvents[k + 1].rotation,
                                intervalTransition);

                            continue;
                        }
                    }
                }
            }
        }
        #endregion

        public override void Deactivate()
        {
            FrameworkCore.PlayCue(sounds.click.whoosh);

            base.Deactivate();
        }

        public override void Activate()
        {
            FrameworkCore.PlayCue(sounds.click.whoosh);

            if (FrameworkCore.playbackSystem.RoundNumber > 1)
                playbackTime = MathHelper.Clamp(FrameworkCore.playbackSystem.WorldTimer - FrameworkCore.playbackSystem.MaxRoundTime, 0, FrameworkCore.playbackSystem.WorldTimer);
            else
                playbackTime = 0;
            playSpeed = 0;
            base.Activate();
        }

        #region playbackSpeedControl
        float REGULARSPEED = 1;
        float FASTSPEED = 0.1f;
        
        private void DecreaseSpeed()
        {
            if (playbackTime <= 0)
            {
                playSpeed = 0;
                return;
            }


            if (playSpeed > 0) //if we are playing, then stop.
                playSpeed = 0;
            else if (playSpeed == 0)      //if we are paused, then play reverse.
                playSpeed = -REGULARSPEED;
            else                 //if we are playing reverse, then pause.
                playSpeed = 0;
        }

        private void IncreaseSpeed()
        {
            if (playbackTime >= FrameworkCore.playbackSystem.WorldTimer)
            {
                playSpeed = 0;
                return;
            }

            if (playSpeed < 0)   //if we are playing reverse, then pause.
                playSpeed = 0;
            else if (playSpeed == 0)  //if we are paused, then play.
                playSpeed = REGULARSPEED;
            else                //if we are playing, then stop.
                playSpeed = 0;
        }
        #endregion

        #region bloomControl
        private void UpdateBloom()
        {
            //don't do the saturate effect in splitscreen.
            if (FrameworkCore.players.Count > 1)
                return;

            if (Transition <= 0)
            {
                //playback is OFF
                if (FrameworkCore.options.bloom)
                    FrameworkCore.Bloomcomponent.SetBaseSaturation(1f);
            }
            else
            {
                //playback is ON
                if (FrameworkCore.options.bloom)
                    FrameworkCore.Bloomcomponent.SetBaseSaturation(0.6f);
            }
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {
            UpdateBloom();

            if (Transition <= 0)
            {
                return;
            }

            //DrawShips(gameTime);

            Vector2 screenViewport = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            int marginSize = 100;//left/right margin.

            //draw title.
            
            Vector2 startPos = new Vector2(marginSize, 0);
            startPos.Y = Helpers.PopLerp(Transition, -10, 100,70);

            string titleString = Resource.MenuPlayback;
            Vector2 titleVec = FrameworkCore.Serif.MeasureString(titleString);            
            Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuPlayback, startPos, Color.White, new Color(0, 0, 0, 128));

            //draw number.
            Color numberColor = new Color(160, 160, 160, 255);
            Vector2 numberPos = startPos + new Vector2(titleVec.X, 0);            
            numberPos.X += 16;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(playbackTime);
            DateTime dt = new DateTime(timeSpan.Ticks);
            String timeDisplay = dt.ToString("mm:ss");            
            Helpers.DrawOutline(FrameworkCore.Serif, timeDisplay, numberPos, numberColor, new Color(0, 0, 0, 128));

            //draw bar.
            int gapSize = 16;
            int barHeight = 12;
            Vector2 barPos = numberPos + new Vector2(numberDisplayWidth, 0);
            barPos.X += gapSize;
            int barWidth = (int)(screenViewport.X - marginSize - barPos.X);
            float barAmount = playbackTime / FrameworkCore.playbackSystem.WorldTimer;
            barPos.X += barWidth /2;
            barPos.Y += barHeight;

            //tick marks.
            Vector2 tickPos = numberPos + new Vector2(numberDisplayWidth, 0);
            DrawTickMark(tickPos);
            tickPos.X += barWidth;
            tickPos.X -= 2;
            DrawTickMark(tickPos);

            float roundNumber = FrameworkCore.playbackSystem.RoundNumber;
            if (roundNumber > 1)
            {
                tickPos = numberPos + new Vector2(numberDisplayWidth, 0);

                for (int i = 1; i < roundNumber; i++)
                {
                    tickPos.X += (barWidth / roundNumber);                    
                    DrawTickMark(tickPos + new Vector2(-3,0));
                }
            }
            Helpers.DrawBar(barPos, barAmount, barWidth, barHeight, 1,
                new Color(250,210,0), new Color(160,160,160,64));

            Rectangle z = new Rectangle(
                (int)(barPos.X - barWidth/2),
                (int)(barPos.Y - barHeight/2),
                barWidth,
                barHeight);
            z.Inflate(0, 256); //BC 4-5-2019 Was 64, now 256. Scrub bar now allows mouse to move far away from the scrub bar during timeline scrubbing.
            barRectangle = z;
            

            

            //LB RB
#if WINDOWS
            Vector2 barEndPos = numberPos + new Vector2(numberDisplayWidth, 0);
            barEndPos.X += barAmount * barWidth;
            barEndPos += new Vector2(17,17);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, barEndPos, sprite.arrow,
                    Color.White, 1.57f, new Vector2(5,15), 0.5f, SpriteEffects.None, 0);

#else
            Vector2 barEndPos = numberPos + new Vector2(numberDisplayWidth, 0);
            barEndPos.X += barAmount * barWidth;
            barEndPos.X += 16;
            barEndPos.Y += 32;

            Rectangle buttonRect = sprite.buttons.lb;
            if (playbackTime > 0)
            {
                float buttonSize = 0.9f;
                if (playSpeed < 0)
                    buttonSize = 0.75f;

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, barEndPos + new Vector2(-24, 0), buttonRect,
                    Color.White, 0, Helpers.SpriteCenter(buttonRect), buttonSize, SpriteEffects.None, 0);
            }

            if (playbackTime < FrameworkCore.playbackSystem.WorldTimer)
            {
                float buttonSize = 0.9f;
                if (playSpeed > 0)
                    buttonSize = 0.75f;

                buttonRect = sprite.buttons.rb;
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, barEndPos + new Vector2(24, 0), buttonRect,
                    Color.White, 0, Helpers.SpriteCenter(buttonRect), buttonSize, SpriteEffects.None, 0);
            }
#endif



#if WINDOWS
            DrawMouseButtons(gameTime);
#endif


            //draw the legend button.
            if (!owner.mouseEnabled)
                Helpers.DrawLegend(Resource.MenuDone, sprite.buttons.b, Transition);
        }

        private void DrawMouseButtons(GameTime gameTime)
        {
            //we do this every frame because of splitscreen issues. 
            Vector2 initialButtonPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - (64),
                            FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 150);
            for (int i = 0; i < buttonPositions.Length; i++)
            {
                buttonPositions[i] = initialButtonPos;
                initialButtonPos.X += 64;
            }

            donePosition.Y = buttonPositions[0].Y;
            donePosition.X = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 130;







            Color doneColor = new Color(96, 96, 96);
            Color doneTextColor = Color.White;
            if (doneHover)
            {
                doneColor = owner.TeamColor;
                doneTextColor = Color.Orange;
            }

            Rectangle doneRect = new Rectangle(
                (int)donePosition.X - 96,
                (int)donePosition.Y - 24,
                192,
                48);
            int modifier = (int)Helpers.PopLerp(Transition, 200, -40, 0);
            doneRect.X += modifier;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, doneRect, sprite.vistaBox,
                doneColor);

            string doneString = Resource.MenuDone;

#if WINDOWS
            if (owner != null && owner.mouseEnabled)
            {
                doneString += " " + Helpers.GetShortcutAltCancel();
            }
#endif

            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, doneString,
                donePosition + new Vector2(modifier, 0), doneTextColor, 1);

            for (int i = 0; i < buttonPositions.Length; i++)
            {
                Color squareColor = new Color(96, 96, 96);
                Color iconColor = new Color(192, 192, 192);

                if (buttonSelected == i)
                {
                    squareColor = Color.Orange;
                    iconColor = Color.White;
                }

                Vector2 buttonPos = buttonPositions[i];
                buttonPos.Y += Helpers.PopLerp(Transition, 200, -30, 0);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, sprite.roundSquare,
                    squareColor, 0, Helpers.SpriteCenter(sprite.roundSquare), 1, SpriteEffects.None, 0);

                SpriteEffects fx = SpriteEffects.None;
                Rectangle iconRect = sprite.icons.rewind;

                if (i == 1)
                    iconRect = sprite.icons.play;                
                else if (i == 2)
                    fx = SpriteEffects.FlipHorizontally;

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, iconRect,
                        iconColor, 0, Helpers.SpriteCenter(iconRect), 1.1f, fx, 0);


                if (buttonHover == i)
                {
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, sprite.roundSquareSelector,
                        Color.Orange, 0, Helpers.SpriteCenter(sprite.roundSquareSelector), 1.1f, SpriteEffects.None, 0);
                }
            }
        }

        private void DrawTickMark(Vector2 pos)
        {
            int tickSize = 5;
            pos.X += 16;
            pos.Y += tickSize / 2;
            Rectangle tickMark = new Rectangle((int)pos.X, (int)pos.Y, 2, tickSize);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, tickMark, sprite.blank, Color.White);
        }
#endregion

        private bool DrawStar(PlaybackItem item, Color explosionColor)
        {
            float sizeScale = playbackTime - item.moveEvents[0].timeStamp;
            sizeScale *= 0.001f;

            if (sizeScale > 1 || sizeScale < 0)
                return false;

            explosionColor = Color.Lerp(explosionColor, Helpers.transColor(explosionColor), sizeScale);

            float explosionSize = MathHelper.Lerp(4, 1, sizeScale);
            FrameworkCore.lineRenderer.Draw(
                item.position + Matrix.Identity.Up * explosionSize,
                item.position + Matrix.Identity.Up * -explosionSize,
                explosionColor);

            FrameworkCore.lineRenderer.Draw(
                item.position + Matrix.Identity.Left * explosionSize,
                item.position + Matrix.Identity.Left * -explosionSize,
                explosionColor);

            FrameworkCore.lineRenderer.Draw(
                item.position + Matrix.Identity.Forward * explosionSize,
                item.position + Matrix.Identity.Forward * -explosionSize,
                explosionColor);

            explosionSize = MathHelper.Lerp(2, 0.5f, sizeScale);

            FrameworkCore.lineRenderer.Draw(
                item.position + (Matrix.Identity.Forward * explosionSize) + (Matrix.Identity.Up * explosionSize) + (Matrix.Identity.Left * explosionSize),
                item.position + (Matrix.Identity.Forward * -explosionSize) + (Matrix.Identity.Up * -explosionSize) + (Matrix.Identity.Left * -explosionSize),
                explosionColor);

            FrameworkCore.lineRenderer.Draw(
                item.position + (Matrix.Identity.Forward * -explosionSize) + (Matrix.Identity.Up * explosionSize) + (Matrix.Identity.Left * explosionSize),
                item.position + (Matrix.Identity.Forward * explosionSize) + (Matrix.Identity.Up * -explosionSize) + (Matrix.Identity.Left * -explosionSize),
                explosionColor);


            FrameworkCore.lineRenderer.Draw(
                item.position + (Matrix.Identity.Forward * explosionSize) + (Matrix.Identity.Up * explosionSize) + (Matrix.Identity.Left * -explosionSize),
                item.position + (Matrix.Identity.Forward * -explosionSize) + (Matrix.Identity.Up * -explosionSize) + (Matrix.Identity.Left * explosionSize),
                explosionColor);

            FrameworkCore.lineRenderer.Draw(
                item.position + (Matrix.Identity.Forward * -explosionSize) + (Matrix.Identity.Up * explosionSize) + (Matrix.Identity.Left * -explosionSize),
                item.position + (Matrix.Identity.Forward * explosionSize) + (Matrix.Identity.Up * -explosionSize) + (Matrix.Identity.Left * explosionSize),
                explosionColor);

            return true;
        }

        #region DrawShips
        public void DrawShips(GameTime gameTime)
        {
            if (!HasFocus)
                return;

            for (int k = 0; k < FrameworkCore.playbackSystem.ItemIndex; k++)
            {
                PlaybackItem item = FrameworkCore.playbackSystem.PlaybackItems[k];

                if (!item.ShouldDraw(playbackTime))
                    continue;

                Matrix worldMatrix = Matrix.CreateFromQuaternion(item.rotation);
                worldMatrix.Translation = item.position;

                if (item.ObjectType == objectType.ship)
                    FrameworkCore.meshRenderer.Draw(item.modelType, worldMatrix, owner.lockCamera, item.color);


                if (item.ObjectType == objectType.deflection)
                {
                    DrawStar(item, Color.Gray);
                    continue;

                }


                if (item.ObjectType == objectType.explosion)
                {
                    DrawStar(item, Color.Orange);
                    continue;
                }

                if (item.ObjectType == objectType.beamHit)
                {
                    FrameworkCore.lineRenderer.Draw(
                        item.spawnPos,
                        item.position,
                        item.color);

                    DrawStar(item, Color.Orange);
                    continue;
                }


                if (item.ObjectType == objectType.hulk)
                {
                    FrameworkCore.meshRenderer.Draw(item.modelType, worldMatrix, owner.lockCamera);

                    //only draw diamond for the ship hulks
                    if (item.modelType == ModelType.debrisHulk1 || item.modelType == ModelType.debrisHulk2)
                    {
                        int diamondSize = 2;
                        FrameworkCore.lineRenderer.Draw(
                            item.position + worldMatrix.Left * diamondSize,
                            item.position + worldMatrix.Forward * diamondSize,
                            item.color);

                        FrameworkCore.lineRenderer.Draw(
                            item.position + worldMatrix.Left * -diamondSize,
                            item.position + worldMatrix.Forward * diamondSize,
                            item.color);

                        FrameworkCore.lineRenderer.Draw(
                            item.position + worldMatrix.Left * diamondSize,
                            item.position + worldMatrix.Forward * -diamondSize,
                            item.color);

                        FrameworkCore.lineRenderer.Draw(
                            item.position + worldMatrix.Left * -diamondSize,
                            item.position + worldMatrix.Forward * -diamondSize,
                            item.color);
                    }

                    continue;
                }

                
                if (item.ObjectType == objectType.bolt)
                {
                    FrameworkCore.meshRenderer.Draw(item.modelType, worldMatrix, owner.lockCamera, Color.White);

                    Color orangeColor = new Color(255, 128, 0);

                    Vector3 boltDir = item.spawnPos - item.position;
                    boltDir.Normalize();

                    FrameworkCore.pointRenderer.Draw(item.position, 4, item.color);                    

                    FrameworkCore.lineRenderer.Draw(
                        item.position + boltDir * 3,
                        item.position,
                        item.color);

                    continue;
                }


                if (item.ObjectType == objectType.ship)
                {
                    //Rods.
                    Color rodColor = new Color(128, 128, 128);
                    Vector3 planePos = item.position;
                    planePos.Y = owner.GridAltitude;
                    FrameworkCore.lineRenderer.Draw(item.position, planePos, rodColor);

                    FrameworkCore.pointRenderer.Draw(planePos, 4, rodColor);
                }

                //BoundingSphere s = new BoundingSphere(ship.entityScratch.Position, 3);
                //FrameworkCore.sphereRenderer.Draw(s, Matrix.Identity, Color.LimeGreen);
            }
        }
        #endregion
    }
}
