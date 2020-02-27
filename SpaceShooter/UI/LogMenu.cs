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
    public class LogMenu : SysMenu
    {
        int WINDOWWIDTH = 1000;

        Vector2 titleVec;

        bool isGameOver = false;

        public bool IsGameOver
        {
            get { return isGameOver; }
        }

        string[] logStrings;

        Vector2[] doneButtonPos;
        int doneButtonHover;

        public LogMenu(bool gameOver, bool scrollToBottom)
        {
            this.isGameOver = gameOver;

            darkenScreen = true;

            if (gameOver)
                canBeExited = false;
            else
                canBeExited = true;

            titleVec = FrameworkCore.Gothic.MeasureString("Sample");

            /*
            MenuItem item = new MenuItem("EXIT");
            item.Selected += OnSelectExit;
            base.AddItem(item);
            */

            if (FrameworkCore.worldMap.evManager.Logs.Count > 0)
            {
                logStrings = new string[FrameworkCore.worldMap.evManager.Logs.Count];
                int textWidth = (int)(WINDOWWIDTH - (512/*imagewidth*/ * IMAGESIZE));

                for (int i = 0; i < FrameworkCore.worldMap.evManager.Logs.Count; i++)
                {
                    logStrings[i] = Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                        FrameworkCore.worldMap.evManager.Logs[i].description, textWidth, Vector2.Zero,
                        Color.White);
                }
            }

            doneButtonPos = new Vector2[]
            {
                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 140,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 155),

                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 140,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 100)
            };

            if (scrollToBottom)
            {
                ScrollList(null, -9999);
            }
        }        

        private void OnSelectExit(object sender, EventArgs e)
        {
            Deactivate();
        }

#if DEBUG
        public void DebugGameOver()
        {
            GameOver(true);
        }
#endif

        private void GameOver(bool startNextAdventure)
        {
            base.Deactivate();

            if (startNextAdventure)
                FrameworkCore.ExitToMainMenu(new CampaignMenu());
            else
                FrameworkCore.ExitToMainMenu(new CreditsMenu(true));
        }

        public override void Deactivate()
        {
            FrameworkCore.PlayCue(sounds.click.whoosh);
            base.Deactivate();
        }

        string timeString = string.Empty;

        public override void Activate()
        {
            FrameworkCore.PlayCue(sounds.click.whoosh);

            if (isGameOver)
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(FrameworkCore.campaignTimer);
                DateTime dt = new DateTime(timeSpan.Ticks);
                timeString = dt.ToString("mm:ss");
            }

            base.Activate();
        }

        float offset=0;
        float lastMousePosY=0;

        float scrollTimer = 0;
        bool scrollDown = false;

        

        public override void Update(GameTime gameTime, InputManager inputManager)
        {

            if (Transition >= 1)
            {
#if XBOX
                if (Math.Abs(inputManager.stickLeft.Y) > 0.2f || inputManager.dpadDown ||
                    inputManager.dpadUp)
                {
                    float scrollSpeed = 1.0f;

                    if (inputManager.turboHeld)
                        scrollSpeed = 4.0f;

                    if (inputManager.stickLeft.Y > 0 || inputManager.dpadUp)
                        ScrollList(gameTime, offset + (float)gameTime.ElapsedGameTime.TotalMilliseconds * scrollSpeed);
                    else if (inputManager.stickLeft.Y < 0 || inputManager.dpadDown)
                        ScrollList(gameTime, offset - (float)gameTime.ElapsedGameTime.TotalMilliseconds * scrollSpeed);
                }

                if (inputManager.openLog || inputManager.buttonAPressed)
                {
                    if (isGameOver)
                    {
                        GameOver(true);
                    }
                    else
                        Deactivate();
                }
                else
                if (isGameOver && inputManager.buttonBPressed)
                {
                    GameOver(false);
                }
#else


                if (scrollTimer <= 0)
                {
                    if (inputManager.mouseRightHeld || inputManager.mouseLeftHeld)
                    {
                        if (inputManager.mouseRightStartHold || inputManager.mouseLeftStartHold)
                        {
                            lastMousePosY = inputManager.mousePos.Y - offset;
                        }

                        if (inputManager.mouseHasMoved)
                        {
                            ScrollList(gameTime, inputManager.mousePos.Y - lastMousePosY);
                        }
                    }


                    bool isHovering = false;
                    for (int i = 0; i < doneButtonPos.Length; i++)
                    {
                        Rectangle doneRect = new Rectangle(
                            (int)doneButtonPos[i].X - 150,
                            (int)doneButtonPos[i].Y - 24,
                            300, 48);

                        if (i == 0 && !isGameOver)
                            continue;

                        if (doneRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                        {
                            isHovering = true;
                            doneButtonHover = i;

                            if (inputManager.mouseLeftClick)
                            {
                                if (isGameOver)
                                {
                                    GameOver((i == 0));
                                }
                                else
                                    Deactivate();
                            }
                        }
                    }

                    if (!isHovering)
                    {
                        doneButtonHover = -1;

                        if (!inputManager.mouseRightHeld && !inputManager.mouseLeftHeld)
                        {
                            int panThreshold = (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.15f);
                            float scrollSpeed = 0.4f;

                            if (inputManager.mousePos.Y < panThreshold)
                                ScrollList(gameTime, offset + (float)gameTime.ElapsedGameTime.TotalMilliseconds * scrollSpeed);
                            else if (inputManager.mousePos.Y > FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - panThreshold)
                                ScrollList(gameTime, offset - (float)gameTime.ElapsedGameTime.TotalMilliseconds * scrollSpeed);
                        }
                    }
                    

                    if (inputManager.mouseWheelDown)
                    {
                        scrollDown = true;
                        scrollTimer = 1;
                    }
                    else if (inputManager.mouseWheelUp)
                    {
                        scrollDown = false;
                        scrollTimer = 1;
                    }
                }
                else
                {
                    if (scrollTimer > 0)
                    {
                        float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                            TimeSpan.FromMilliseconds(100).TotalMilliseconds);
                        scrollTimer = MathHelper.Clamp(scrollTimer - delta, 0, 1);

                        if (scrollDown)
                        {
                            ScrollList(gameTime, offset - (float)gameTime.ElapsedGameTime.TotalMilliseconds * 2);
                        }
                        else
                        {
                            ScrollList(gameTime, offset + (float)gameTime.ElapsedGameTime.TotalMilliseconds * 2);
                        }
                    }
                }

                if (inputManager.kbHome)
                {
                    ScrollList(null, 9999);
                }
                else if (inputManager.kbEnd)
                {
                    ScrollList(null, -9999);
                }
                else if (inputManager.sysMenuDownHeld)
                {
                    ScrollList(null, offset - (float)gameTime.ElapsedGameTime.TotalMilliseconds * 2.0f);
                }
                else if (inputManager.sysMenuUpHeld)
                {
                    ScrollList(null, offset + (float)gameTime.ElapsedGameTime.TotalMilliseconds * 2.0f);
                }



                if (inputManager.openLog || inputManager.kbBackspaceJustPressed || inputManager.kbSpace)
                {
                    if (IsGameOver)
                        return;
                    else
                        Deactivate();
                }
#endif
            }

            base.Update(gameTime, inputManager);
        }

        private void ScrollList(GameTime gameTime, float value)
        {
            int logCount = 1;

            try
            {
                logCount = FrameworkCore.worldMap.evManager.Logs.Count;
            }
            catch
            {
            }

            float minDist = (logCount + 4) * ((128 * IMAGESIZE) + 8);
            float screenHeight = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - (100 + titleVec.Y);            

            if (minDist > screenHeight)
                minDist -= screenHeight;
            else
                minDist = 0;            

            offset = MathHelper.Clamp(
                value,
                -minDist, 0);
        }

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();


            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Vector2 titlePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - (WINDOWWIDTH/2),
                100 + offset);

            titlePos.X += Helpers.PopLerp(Transition, -200, 30, 0);


            if (isGameOver)
            {
                int stringLength = (int)FrameworkCore.Serif.MeasureString(timeString).X;
                Vector2 timePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 110 - stringLength,
                    titlePos.Y - 18);

                timePos.Y += Helpers.Pulse(gameTime, 4, 5);

                Color darkColor = new Color(0,0,0,128);
                darkColor = Color.Lerp(Helpers.transColor(darkColor), darkColor, Transition);
                Helpers.DrawOutline(FrameworkCore.Serif, timeString, timePos, titleColor, darkColor,
                    0, Vector2.Zero, 1);

                timePos.X -= 29;
                timePos.Y -= 3;
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, timePos, sprite.tinyHourglass,
                    titleColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }

            string titleTxt = "";

            if (FrameworkCore.players[0].commanderName != null)
            {
                if (isGameOver)
                {
                    titleTxt = string.Format(Resource.MenuLog,
                        FrameworkCore.players[0].commanderName, (FrameworkCore.adventureNumber - 1));
                }
                else
                {
                    titleTxt = string.Format(Resource.MenuLog,
                        FrameworkCore.players[0].commanderName, FrameworkCore.adventureNumber);
                }
            }

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, titleTxt,
                titlePos, titleColor, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

            
            
            titlePos.Y += titleVec.Y;

            DrawLogs(titlePos, titleColor);

#if WINDOWS
            //Helpers.DrawClickMessage(gameTime, Transition);

            for (int i = 0; i < doneButtonPos.Length; i++)
            {
                if (!isGameOver && i == 0)
                    continue;

                Vector2 buttPos = doneButtonPos[i];
                buttPos.X += Helpers.PopLerp(Transition, 200, -40, 0);

                Rectangle doneRect = new Rectangle(
                            (int)buttPos.X - 150,
                            (int)buttPos.Y - 24,
                            300, 48);

                Color buttonColor = Color.Lerp(Color.Black, FrameworkCore.players[0].TeamColor, 0.5f);
                Color txtColor = Color.White;

                if (doneButtonHover == i)
                {
                    buttonColor = FrameworkCore.players[0].TeamColor;
                    txtColor = Color.Orange;
                }

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, doneRect, sprite.vistaBox,
                    buttonColor);

                string text = Resource.MenuDone;

#if WINDOWS
                text += " " + Helpers.GetShortcutAltCancel();
#endif

                if (isGameOver)
                {
                    if (i == 0)
                        text = string.Format(Resource.MenuCampaignStart, FrameworkCore.adventureNumber);
                    else
                        text = Resource.MenuMainMenu;
                }

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, text,
                    buttPos, txtColor, 1);
            }

#else
            if (isGameOver)
            {
                float x = Helpers.DrawLegend(
                    string.Format(Resource.MenuCampaignStart, FrameworkCore.adventureNumber),
                    sprite.buttons.a, Transition);

                Helpers.DrawLegendAt(Resource.MenuMainMenu,
                    sprite.buttons.b, Transition, x - 32);
            }
            else
            {
                float x = Helpers.DrawLegend(Resource.MenuDone, sprite.buttons.a, Transition);
                Helpers.DrawLegendAt(Resource.MenuLogScroll, sprite.buttons.upDown, Transition, x - 32);
            }
#endif
        }

        float IMAGESIZE = 0.6f;

        private void DrawLogs(Vector2 startPos, Color textColor)
        {
            if (FrameworkCore.worldMap.evManager.Logs.Count <= 0)
            {
                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuLogEmpty,
                    startPos, textColor);
                return;
            }

            int fadeThreshold = 350;

            //foreach (LogEvent ev in FrameworkCore.worldMap.evManager.Logs)
            for (int i = 0; i < FrameworkCore.worldMap.evManager.Logs.Count; i++)
            {
                LogEvent ev = FrameworkCore.worldMap.evManager.Logs[i];

                Color itemColor = textColor;

                if (startPos.Y > FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - fadeThreshold)
                {
                    float alpha = startPos.Y - (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - fadeThreshold);
                    alpha /= fadeThreshold;
                    
                    itemColor.A = (byte)MathHelper.Lerp(255, 16, alpha);
                    itemColor.A = (byte)MathHelper.Lerp(0, itemColor.A, Transition);
                }

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.eventSheet, startPos, ev.image, itemColor,
                    0, Vector2.Zero, IMAGESIZE, SpriteEffects.None, 0);

       


                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, logStrings[i],
                    startPos + new Vector2(8 + (ev.image.Width * IMAGESIZE), -6),
                    itemColor);


                
                startPos.Y += (128/*ev.image.Height*/ * IMAGESIZE) + 8;
            }
        }
    }
}
