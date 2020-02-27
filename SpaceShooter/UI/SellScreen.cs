


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
using Microsoft.Xna.Framework.Media;

#endregion

namespace SpaceShooter
{
    public class SellScreen : SysMenu
    {
        int LINESIZE;

        Vector2[] confirmButtons;
        int hoverConfirmButton = -1;

        int windowWidth = 900;
        int windowHeight = 500;


        string[] sellLines;
        Vector2[] linePositions;
        float[] lineTransitions;

        public SellScreen()
        {
            LINESIZE = (int)FrameworkCore.Serif.MeasureString("Sample").Y;

            darkenScreen = true;
            canBeExited = true;

            transitionOnTime = 300;
            transitionOffTime = 300;

            
            confirmButtons = new Vector2[2]
            {
                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 - 160,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),

                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 + 160,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),
            };

#if XBOX
            hoverConfirmButton = 0;
#endif

            sellLines = new string[]
            {
                Resource.MenuSellExplore,
                Resource.MenuSellFight,
                Resource.MenuSellShips,
                Resource.MenuSellUpgrades,
                Resource.MenuSellScores,
                Resource.MenuSellAnimals,
            };

            int linesPerSide = sellLines.Length / 2;

            Rectangle vidRect = GetVideoRect();
            vidRect.Inflate(64, 0);
            linePositions = new Vector2[sellLines.Length];

            int gapSize = vidRect.Height / linesPerSide;

            for (int i = 0; i < linePositions.Length; i++)
            {
                if (i < linesPerSide)
                    linePositions[i] = new Vector2(vidRect.X, vidRect.Y + i * gapSize);
                else
                    linePositions[i] = new Vector2(vidRect.X + vidRect.Width, vidRect.Y + (i - linesPerSide) * gapSize);

                linePositions[i].Y += 40;
            }

            lineTransitions = new float[sellLines.Length];
        }


        float vidTransition = 0;

        int activeLine = 0;
        int nextLineTimer = 0;

        int initialDelay = 600;

        private void UpdateLineTransitions(GameTime gameTime)
        {
            if (Transition < 1)
                return;

            if (initialDelay > 0)
            {
                initialDelay -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                return;
            }

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                TimeSpan.FromMilliseconds(400).TotalMilliseconds);

            for (int i = 0; i < lineTransitions.Length; i++)
            {
                if (i <= activeLine)
                {
                    lineTransitions[i] = MathHelper.Clamp(lineTransitions[i] + delta, 0, 1);
                }
            }

            if (activeLine <= lineTransitions.Length)
            {
                nextLineTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (nextLineTimer >= 300)
                {
                    nextLineTimer = 0;
                    activeLine++;
                }
            }
        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            UpdateLineTransitions(gameTime);


            if (!videoAttempted && Transition >= 1)
            {
                videoAttempted = true;
                playVideo();
            }

            if (videoIsGood)
            {
                if (player != null && !player.IsDisposed && Transition >= 1)
                {

                    if (player.State != MediaState.Playing)
                    {
                        vidTransition = 0;
                        try
                        {
                            player.Play(sellVideo);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(1500).TotalMilliseconds);

                        vidTransition = MathHelper.Clamp(vidTransition + delta, 0, 1);
                    }
                }
            }
            
            



#if WINDOWS
            UpdateMouseInput(gameTime, inputManager);
#else
            UpdateXboxControl(gameTime, inputManager);
#endif

            


            base.Update(gameTime, inputManager);
        }

        bool videoAttempted = false;

        bool videoIsGood = false;
        

        private void playVideo()
        {
            bool error = false;

            //this function unfortunately freaks out if player has bad codecs
            //so, wrap it in a trycatch loop.
            try
            {
                sellVideo = FrameworkCore.content.Load<Video>(@"textures\endvideo");

                player = new VideoPlayer();
                //player.IsLooped = true;
                player.Volume = 0.2f;
                player.Play(sellVideo);
            }
            catch
            {
                //uh oh video didn't play
                error = true;


            }

            if (!error)
                videoIsGood = true;
        }
 

        private void UpdateXboxControl(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;


            if (inputManager.sysMenuLeft)
            {
                hoverConfirmButton = 0;
            }
            else if (inputManager.sysMenuRight)
            {
                hoverConfirmButton = 1;
            }

            if (inputManager.buttonAPressed)
            {
                if (hoverConfirmButton == 0)
                {
                    //get full game.
                    BuyGame();
                    Deactivate();
                }
                else
                {
                    QuitGame();
                }
            }
        }

        private void BuyGame()
        {
            FrameworkCore.BuyGame();
        }

        private void QuitGame()
        {
            FrameworkCore.Game.Exit();
        }

        private void UpdateMouseInput(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;

            bool confirmHover = false;  
            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Rectangle confirmRect = new Rectangle(
                    (int)(confirmButtons[i].X - 150),
                    (int)(confirmButtons[i].Y - 30),
                    300,
                    60);

                if (confirmRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    hoverConfirmButton = i;
                    confirmHover = true;

                    if (inputManager.mouseLeftClick)
                    {
                        if (i == 0)
                        {
                            BuyGame();
                        }
                        else if (i == 1)
                        {
                            QuitGame();
                        }
                    }
                }
            }

            if (!confirmHover)
                hoverConfirmButton = -1;
        }


        public override void Deactivate()
        {
            player.Dispose();

            base.Deactivate();
        }

        
        VideoPlayer player;
        Texture2D videoTexture;
        Video sellVideo;

        public override void Activate()
        {
            

            base.Activate();

        }


        private Rectangle GetVideoRect()
        {
            int videoWidth = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 380;
            int videoHeight = (int)(videoWidth * 0.3555f);

            Rectangle screen = new Rectangle(
                (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - (videoWidth / 2),
                (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - (videoHeight / 2),
                videoWidth,
                videoHeight);

            screen.Y += 30;

            return screen;
        }

        private void DrawVideo()
        {
            if (!videoIsGood)
                return;

            if (player == null)
                return;

            if (player.IsDisposed)
                return;

            if (player.State == MediaState.Stopped)
                return;
            
            videoTexture = player.GetTexture();

            // Drawing to the rectangle will stretch the 
            // video to fill the screen
            

            // Draw the video, if we have a texture to draw.
            if (videoTexture != null)
            {
                Color videoColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, vidTransition);                
                FrameworkCore.SpriteBatch.Draw(videoTexture, GetVideoRect(), videoColor);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float titleSize = 0.8f;
            base.DrawDarkenScreen();


            Rectangle vidRect = GetVideoRect();

            DrawVideo();
            

            Vector2 titlePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2,
                vidRect.Y - 110);

            Vector2 modifier = new Vector2(0, Helpers.PopLerp(Transition, -100, 50, 0));
            

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color numberColor = Color.Lerp(Helpers.transColor(Color.Gray), Color.Gray, Transition);

            string titleString = string.Format(Resource.MenuSellWellDone,
                FrameworkCore.players[0].commanderName);

            int titleVec = (int)(FrameworkCore.Gothic.MeasureString("S").Y * titleSize);
            
            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Gothic, titleString,
                titlePos + modifier, titleColor, titleSize);
            

            titlePos.Y += titleVec - 5;


            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.SerifBig, Resource.MenuSellBuyToday,
                titlePos + modifier, titleColor, 0.85f);


            

            DrawSellLines(gameTime);

            DrawConfirmButtons(gameTime);
        }

        private void DrawSellLines(GameTime gameTime)
        {
            int linesPerSide = sellLines.Length / 2;

            for (int i = 0; i < sellLines.Length; i++)
            {
                Color txtColor = Color.Goldenrod;
                txtColor = Color.Lerp(Helpers.transColor(txtColor), txtColor, lineTransitions[i]);
                Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), lineTransitions[i]);

                

                Vector2 origin = Vector2.Zero;
                float angle = -0.12f;
                Vector2 pos = linePositions[i];

                if (i >= linesPerSide)
                {
                    //Right side.
                    origin.X = FrameworkCore.Serif.MeasureString(sellLines[i]).X;
                    angle *= -1f;
                    pos.X += Helpers.PopLerp(lineTransitions[i], 200, -40, 0);
                    pos.X += Helpers.PopLerp(Transition, 200, -40, 0);
                }
                else
                {
                    //Left side.
                    pos.X += Helpers.PopLerp(lineTransitions[i], -200, 40, 0);
                    pos.X += Helpers.PopLerp(Transition, -200, 40, 0);
                }

                Color blackColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, lineTransitions[i]);

                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, sellLines[i], pos + new Vector2(3,3),
                    blackColor,
                    angle, origin, 1.4f, SpriteEffects.None, 0);

                Helpers.DrawOutline(FrameworkCore.Serif, sellLines[i], pos, txtColor,
                    darkColor, angle, origin, 1.4f);
            }
        }

        private void DrawConfirmButtons(GameTime gameTime)
        {
            if (Transition < 1)
                return;

            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Vector2 buttonPos = confirmButtons[i];
                buttonPos.Y += (int)Helpers.PopLerp(Transition, 200, -30, 0);

                Rectangle descriptionRect = new Rectangle(
                    (int)(buttonPos.X - 150),
                    (int)(buttonPos.Y - 30),
                    300,
                    60);

                //draw the box.                                
                Color buttonColor = Color.Lerp(Color.Black, Faction.Blue.teamColor, 0.5f);
                Color textColor = Color.White;

                if (hoverConfirmButton == i)
                {
                    buttonColor = Faction.Blue.teamColor;
                    textColor = Color.Orange;
                    
                    Rectangle glowRect = descriptionRect;
                    glowRect.Inflate(6, 6);

                    Color glowColor = Color.Lerp(Color.Goldenrod, Color.White, 0.5f + Helpers.Pulse(gameTime, 0.49f, 12));

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, glowRect,
                                        sprite.vistaBox, glowColor);
                    
                }

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, descriptionRect,
                    sprite.vistaBox, buttonColor);



                //draw the text.
                string buttText = string.Empty;
                if (i == 0)
                {
                    buttText = Resource.MenuUnlockFullGame;
                }
                else
                    buttText = Resource.MenuQuit;

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, buttText,
                    buttonPos, textColor, 1);
            }
        }
     }
}


