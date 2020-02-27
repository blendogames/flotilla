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


using System.Globalization;


#endregion

namespace SpaceShooter
{
    public class HighScoreMenu : SysMenu
    {
        int LINESIZE;        

        Vector2[] confirmButtons;
        int hoverConfirmButton = -1;


        string[] menuHighscoreName;
        int[] menuHighscoreScore;

#if (WINDOWS && STEAM)
        bool[] isSteamFriend;
#endif

        bool showOnlineScores = true;
        public bool ShowOnlineScores
        {
            get { return showOnlineScores; }
            set { showOnlineScores = value; }
        }



        float offset = 0;
        public float Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        float lastMousePosY = 0;

        float scrollTimer = 0;
        bool scrollDown = false;

        public HighScoreMenu()
        {
            int margin = 170;
            confirmButtons = new Vector2[4]
            {
                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - margin,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),

                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - margin,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 190),

                new Vector2(margin,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 190),

                new Vector2(margin,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),

            };


            LINESIZE = (int)FrameworkCore.Serif.MeasureString("Sample").Y + 4;

            darkenScreen = true;

            transitionOnTime = 300;
            transitionOffTime = 300;

            MenuItem item = new MenuItem(Resource.MenuDone);
            item.Selected += OnDone;
            base.AddItem(item);

            /*
#if WINDOWS
            GotoGlobalScores(Helpers.HighScoreType.Normal);
            
#else
            GotoLocalScores();
#endif
*/
            //BC2019 Only do local scores.
            GotoLocalScores();
        }

        public bool GotoGlobalScores(Helpers.HighScoreType type)
        {
            bool success = Helpers.GetGlobalHighScores(type);

            if (success)
            {
                showOnlineScores = true;

                menuHighscoreName = new string[Helpers.GlobalHighscoreNames.Length];
                menuHighscoreScore = new int[Helpers.GlobalHighscoreNames.Length];

                menuHighscoreName = Helpers.GlobalHighscoreNames;
                menuHighscoreScore = Helpers.GlobalHighscoreScores;

#if (WINDOWS && STEAM)
                FrameworkCore.RefreshSteamFriendArray();

                isSteamFriend = new bool[menuHighscoreName.Length];

                try
                {
                    for (int i = 0; i < menuHighscoreName.Length; i++)
                    {

                        for (int x = 0; x < FrameworkCore.SteamFriendNames.Length; x++)
                        {
                            string steamFriendName = Helpers.StripOutAmpersands(FrameworkCore.SteamFriendNames[x]);
                                
                            if (System.String.Compare(menuHighscoreName[i], steamFriendName, true, CultureInfo.InvariantCulture) == 0)
                            {
                                isSteamFriend[i] = true;
                                break;
                            }
                            else
                                isSteamFriend[i] = false;
                        }
                    }
                }
                catch
                {
                }
#endif
            }
            else
            {
                menuHighscoreName = new string[0];
                menuHighscoreScore = new int[0];

                
            }

            return success;
        }

        public void GotoLocalScores()
        {
            curTable = 2;
            steamFilterFriends = false;
            showOnlineScores = false;

            menuHighscoreName = new string[FrameworkCore.highScores.count];
            menuHighscoreScore = new int[FrameworkCore.highScores.count];

            for (int i = 0; i < FrameworkCore.highScores.count; i++)
            {
                menuHighscoreName[i] = FrameworkCore.highScores.commanderName[i];
                menuHighscoreScore[i] = FrameworkCore.highScores.scores[i];
            }
        }


        private void OnDone(object sender, EventArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        public override void Update(GameTime gameTime, InputManager inputManager)
        {


#if WINDOWS
            if (Transition >= 1)
            {
                if (inputManager.kbSpace)
                    Deactivate();

                UpdateMouseInput(gameTime, inputManager);

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

                    float scrollSpeed = 2.0f;
                    if (inputManager.sysMenuUpHeld)
                        ScrollList(gameTime, offset + (float)gameTime.ElapsedGameTime.TotalMilliseconds * scrollSpeed);
                    else if (inputManager.sysMenuDownHeld)
                        ScrollList(gameTime, offset - (float)gameTime.ElapsedGameTime.TotalMilliseconds * scrollSpeed);

                    if (inputManager.kbHome)
                    {
                        ScrollList(gameTime, offset + (float)gameTime.ElapsedGameTime.TotalMilliseconds * 99999);
                    }
                    else if (inputManager.kbEnd)
                    {
                        ScrollList(gameTime, offset - (float)gameTime.ElapsedGameTime.TotalMilliseconds * 99999);
                    }

                    if (!inputManager.mouseLeftHeld)
                    {
                        if (inputManager.mousePos.Y < 90)
                        {
                            ScrollList(gameTime, offset + (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.5f);
                        }
                        else if (inputManager.mousePos.Y > FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 90)
                        {
                            ScrollList(gameTime, offset - (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.5f);
                        }
                    }
                }
                else if (scrollTimer > 0)
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
#endif

            base.Update(gameTime, inputManager);
        }

        public int curTable = 0;
        public string onlineButtonText = Resource.MenuHighscoresHardcore;

        public virtual void ToggleOnline()
        {
            offset = 0;

            if (curTable == 0)
            {
                //go to hardcore scores.
                showOnlineScores = true;
                if (!GotoGlobalScores(Helpers.HighScoreType.Hardcore))
                    OnlineError();

                curTable = 1;
                onlineButtonText = Resource.MenuHighscoresLocal;
            }
            else if (curTable == 1)
            {
                //go to local table.
                showOnlineScores = false;
                GotoLocalScores();
                curTable = 2;
                onlineButtonText = Resource.MenuHighscoresOnline;
            }
            else
            {
                //go to normal online scores.
                showOnlineScores = true;
                if (!GotoGlobalScores(Helpers.HighScoreType.Normal))
                    OnlineError();

                curTable = 0;
                onlineButtonText = Resource.MenuHighscoresHardcore;
            }
        }

        private bool ValidButtonCheck(int index)
        {
            
#if !(WINDOWS && STEAM)
            if (index >= 3)
                return false;
#endif

#if (WINDOWS && STEAM)
            if (index == 2 && steamFilterFriends)
                return false;
#endif

            if (!showOnlineScores && index >= 2)
            {
                return false;
            }

            return true;
        }

        private void UpdateMouseInput(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;

            bool confirmHover = false;
            for (int i = 0; i < confirmButtons.Length; i++)
            {
                if (!ValidButtonCheck(i))
                    continue;



                Rectangle confirmRect = new Rectangle(
                    (int)(confirmButtons[i].X - 130),
                    (int)(confirmButtons[i].Y - 30),
                    260,
                    60);

                if (confirmRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {

                    if (i == 1) //BC2019 Remove the online scoreboard.
                        continue;

                    hoverConfirmButton = i;
                    confirmHover = true;

                    if (inputManager.mouseLeftClick)
                    {
                        if (i == 0)
                        {
                            FrameworkCore.PlayCue(sounds.click.activate);
                            Deactivate();
                        }
                        else if (i == 1)
                        {                            
                            FrameworkCore.PlayCue(sounds.click.activate);
                            ToggleOnline();
                        }
                        else if (i == 2)
                        {
                            CenterPlayer(gameTime);
                        }
#if (WINDOWS && STEAM)
                        else if (i == 3)
                        {
                            FrameworkCore.PlayCue(sounds.click.activate);
                            //filter friends.
                            FilterFriendToggle();
                        }
#endif
                    }
                }
            }

            if (!confirmHover)
                hoverConfirmButton = -1;
        }

        bool steamFilterFriends = false;

        private void FilterFriendToggle()
        {
            offset = 0;

            steamFilterFriends = !steamFilterFriends;
        }

        private void CenterPlayer(GameTime gameTime)
        {
            int playerIndex = -1;

            for (int k = 0; k < menuHighscoreName.Length; k++)
            {
                string playerName = Helpers.StripOutAmpersands(FrameworkCore.players[0].commanderName);

                if (System.String.Compare(menuHighscoreName[k], playerName, true, CultureInfo.InvariantCulture) == 0)
                {
                    //found player name.
                    playerIndex = k;
                    break;
                }
            }

            if (playerIndex < 0)
            {
                //couldn't find player.
                FrameworkCore.PlayCue(sounds.click.error);
                return;
            }

            FrameworkCore.PlayCue(sounds.click.activate);
            offset = ((LINESIZE * playerIndex) * -1f) + HEADERSIZE;
        }

        public int HEADERSIZE = 200;

        private void ScrollList(GameTime gameTime, float value)
        {
            float minDist = (menuHighscoreName.Length * LINESIZE) + 
                (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.5f);
            float screenHeight = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - (160);



            if (minDist > screenHeight)
                minDist -= screenHeight;
            else
                minDist = 0;


            offset = MathHelper.Clamp(
                value,
                -minDist, 0);
        }

        int WINDOWWIDTH = 600;

        public override void Draw(GameTime gameTime)
        {
            
            base.DrawDarkenScreen();



            Vector2 titlePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - WINDOWWIDTH / 2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2 - 300);
            titlePos.Y += Helpers.PopLerp(Transition, -400, 20, 0);

            titlePos.Y += offset;



            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);


            string tableName = string.Empty;

            if (curTable == 0)
                tableName = Resource.MenuHighScoresTitleOnline;
            else if (curTable == 1)
                tableName = Resource.MenuHighscoresHardcoreHall;
            else
                tableName = Resource.MenuHighscoresLocal;


#if XBOX
            tableName = Resource.MenuHighscoresLocal;
#endif


            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic,
                tableName,
                titlePos, titleColor);


            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");

            Vector2 entryPos = titlePos;
            entryPos.Y += titleVec.Y;

            DrawEntries(gameTime, entryPos, -1);

            DrawConfirmButtons(gameTime);


#if XBOX
            Helpers.DrawLegend(Resource.MenuDone, sprite.buttons.a, Transition);
#endif
        }

        public void DrawEntries( GameTime gameTime, Vector2 entryPos, int addedIndex )
        {
            Color numberColor = Color.Lerp(Helpers.transColor(Color.Gray), Color.Gray, Transition);
            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);

            int nameMargin = (int)FrameworkCore.Serif.MeasureString("000").X;

            bool drawDownArrow = false;
            if (menuHighscoreName.Length > 0)
            {
                if (menuHighscoreName[0].Length <= 0 || FrameworkCore.isTrialMode())
                {
                    string txt = Resource.MenuHighScoresEmpty;

                    if (FrameworkCore.isTrialMode())
                        txt = Resource.MenuHighScoresTrial;

                    DrawErrorMessage(gameTime, txt);
                }
                else
                {
                    Color darkColor = Color.Black;
                    darkColor = Color.Lerp(Helpers.transColor(darkColor), darkColor, Transition);

                    for (int i = 0; i < menuHighscoreName.Length; i++)
                    {
                        if (menuHighscoreName[i].Length <= 0)
                            continue;



                        string playerName = Helpers.StripOutAmpersands(FrameworkCore.players[0].commanderName);
                        bool isCurrentPlayer = ((System.String.Compare(menuHighscoreName[i], playerName, true, CultureInfo.InvariantCulture) == 0) && showOnlineScores) ||
                            (!showOnlineScores && i == addedIndex);

                        if (isCurrentPlayer)
                        {
                            titleColor = Color.Lerp(Helpers.transColor(Color.Orange), Color.Orange, Transition);

                            DrawBouncyArrow(gameTime, entryPos, titleColor);
                        }
#if (WINDOWS && STEAM)
                        else if (showOnlineScores && isSteamFriend[i])
                        {
                            titleColor = Color.Lerp(Helpers.transColor(Color.Lime), Color.Lime, Transition);

                            DrawBouncyArrow(gameTime, entryPos, titleColor);
                        }
#endif
                        else
                            titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);




#if (WINDOWS && STEAM)
                        if (steamFilterFriends)
                        {
                            if (!isSteamFriend[i] && !isCurrentPlayer)
                            {
                                continue;
                            }
                        }
#endif



                        FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif,
                            (i + 1).ToString(), entryPos + new Vector2(-20, 0), numberColor);

                        Helpers.DrawOutline(FrameworkCore.Serif, menuHighscoreName[i],
                            entryPos + new Vector2(nameMargin, 0), titleColor, darkColor);


                        string scoreString = menuHighscoreScore[i].ToString();
                        Vector2 scoreVec = FrameworkCore.Serif.MeasureString(scoreString);
                        Vector2 scorePos = entryPos;
                        scorePos.X += WINDOWWIDTH;
                        scorePos.X -= scoreVec.X;
                        Helpers.DrawOutline(FrameworkCore.Serif, scoreString,
                            scorePos, titleColor, darkColor);


                        if (entryPos.Y > FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 50)
                            drawDownArrow = true;

                        entryPos.Y += LINESIZE;
                    }
                }
            }
            else
            {
                string txt = Resource.MenuHighScoreOnlineEmpty;

                if (FrameworkCore.isTrialMode())
                    txt = Resource.MenuHighScoresTrial;

                DrawErrorMessage(gameTime, txt);
            }

            if (drawDownArrow)
            {
                Vector2 arrowPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 96);

                arrowPos.Y += Helpers.Pulse(gameTime, 7, 5);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, arrowPos,
                        sprite.arrow, new Color(255, 255, 255, 64),
                        -1.57f,
                        Helpers.SpriteCenter(sprite.arrow), 1.5f, SpriteEffects.None, 0);
            }
        }

        public void OnlineError()
        {
            if (FrameworkCore.isTrialMode())
                return;

            SysPopup signPrompt = new SysPopup(FrameworkCore.sysMenuManager,
                    Resource.MenuHighScoreSendError);
            signPrompt.transitionOnTime = 200;
            signPrompt.transitionOffTime = 200;
            signPrompt.darkenScreen = true;
            signPrompt.hideChildren = false;
            signPrompt.sideIconRect = sprite.windowIcon.error;

            MenuItem item = new MenuItem(Resource.MenuOK);
            item.Selected += CloseMenu;
            signPrompt.AddItem(item);

            FrameworkCore.sysMenuManager.AddMenu(signPrompt);

            FrameworkCore.PlayCue(sounds.click.error);
        }

        private static void CloseMenu(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }

        private void DrawErrorMessage(GameTime gameTime, string txt)
        {
            Color errorColor = Color.Lerp(Helpers.transColor(Color.Orange), Color.Orange, Transition);
            Vector2 errorPos = Helpers.GetScreenCenter();
            errorPos.Y += Helpers.Pulse(gameTime, 6, 4);
            errorPos.X += Helpers.PopLerp(Transition, 200, -30, 0);

            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                txt, errorPos, errorColor, 1.4f);
        }

        private void DrawBouncyArrow(GameTime gameTime, Vector2 entryPos, Color arrowColor)
        {
            float arrowTransition = MathHelper.Lerp(0,1, 0.5f + Helpers.Pulse(gameTime, 0.49f, 5));
            float arrowSize = MathHelper.Lerp(0.9f, 0.7f, arrowTransition);

            Vector2 playerArrowPos = entryPos;
            playerArrowPos.X -= 35;
            playerArrowPos.Y += 13;

            playerArrowPos.X += MathHelper.Lerp(-3,3,arrowTransition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, playerArrowPos,
                sprite.arrow, arrowColor,
                3.14f,
                Helpers.SpriteCenter(sprite.arrow), arrowSize, SpriteEffects.None, 0);
        }

        public void DrawConfirmButtons(GameTime gameTime)
        {
#if XBOX
            return;
#endif

            for (int i = 0; i < confirmButtons.Length; i++)
            {
                if (i == 1) //BC2019 Remove the online scoreboard.
                    continue;

                if (!ValidButtonCheck(i))
                    continue;

#if !(WINDOWS && STEAM)
                if (i >= 3)
                    return;
#endif

                Vector2 buttonPos = confirmButtons[i];
                buttonPos.Y += (int)Helpers.PopLerp(Transition, 200, -30, 0);

                Rectangle descriptionRect = new Rectangle(
                    (int)(buttonPos.X - 130),
                    (int)(buttonPos.Y - 30),
                    260,
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
                    buttText = Resource.MenuDone;
                }
                else if (i==1)
                {
                    buttText = onlineButtonText;
                }
                else if (i == 2)
                {
                    buttText = Resource.MenuHighScoresFindMe;                    
                }
#if (WINDOWS && STEAM)
                else if (i==3)
                {
                    buttText = Resource.MenuHighScoresFilter;
                }
#endif


                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, buttText,
                    buttonPos, textColor, 1);
            }
        }        


     }
}
