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
    public class GameOverMenu : HighScoreMenu
    {
        public GameOverMenu() : base()
        {
            this.HEADERSIZE = 0;
        }

        int finalScore = 0;
        bool newGlobalHighScore = false;
        int addedIndex = -1;

        public override void Activate()
        {
            //save the data.
            FrameworkCore.adventureNumber++;

            SaveInfo save = FrameworkCore.storagemanager.GetDefaultSaveData();
            FrameworkCore.storagemanager.SaveData(save);

            finalScore = Helpers.GenerateFinalScore();

            //add high score to the high score table.
            string playerName = "";

            if (FrameworkCore.players.Count > 1)
            {
                playerName = FrameworkCore.players[0].commanderName + " " + Resource.MenuHighScoresPlus +
                    " " + FrameworkCore.players[1].commanderName;
            }
            else
            {
                playerName = FrameworkCore.players[0].commanderName;
            }


            //disable high score recording in trial mode.
            if (FrameworkCore.isTrialMode())
                addedIndex = -1;
            else
                addedIndex = Helpers.AddHighScore(playerName, finalScore, out newGlobalHighScore);


#if WINDOWS
            Helpers.HighScoreType type = Helpers.HighScoreType.Normal;

            if (FrameworkCore.isHardcoreMode)
                type = Helpers.HighScoreType.Hardcore;
            else
                type = Helpers.HighScoreType.Normal;


            //base.GotoGlobalScores(type);

#else
            //base.GotoLocalScores();
#endif

            //BC2019 Go to local scoreboard only.
            base.GotoLocalScores();


            onlineButtonText = Resource.MenuHighscoresLocal;
            

            base.Activate();
        }

        public override void ToggleOnline()
        {
            Offset = 0;

            if (curTable == 0)
            {
                //go to local table.
                ShowOnlineScores = false;
                GotoLocalScores();
                curTable = 1;



                string buttonText = string.Empty;
                if (FrameworkCore.isHardcoreMode)
                {
                    buttonText = Resource.MenuHighscoresHardcore;
                }
                else
                {
                    buttonText = Resource.MenuHighscoresOnline;
                }

                onlineButtonText = buttonText;
            }
            else
            {
                
                Helpers.HighScoreType type = Helpers.HighScoreType.Normal;
                if (FrameworkCore.isHardcoreMode)
                {
                    type = Helpers.HighScoreType.Hardcore;
                }
                else
                {
                    type = Helpers.HighScoreType.Normal;
                }
                




                //go to normal online scores.
                ShowOnlineScores = true;
                if (!GotoGlobalScores(type))
                    base.OnlineError();

                curTable = 0;
                onlineButtonText = Resource.MenuHighscoresLocal;                
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            Owner.AddMenu(new LogMenu(true, false));
        }


        public override void Draw(GameTime gameTime)
        {

            int windowWidth = 600;
            base.DrawDarkenScreen();

            

            Vector2 titlePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 - windowWidth/2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2 - 300);
            titlePos.Y += Helpers.PopLerp(Transition, -400, 30, 0);

            titlePos.Y += base.Offset;



            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color numberColor = Color.Lerp(Helpers.transColor(Color.Gray), Color.Gray, Transition);


            Vector2 centerTitlePos = titlePos;
            centerTitlePos.Y += 64;
            centerTitlePos.X = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2;
            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Gothic,
                string.Format(Resource.MenuGameOverTitle,Math.Max(FrameworkCore.adventureNumber-1, 1)),
                centerTitlePos+ new Vector2(0, Helpers.Pulse(gameTime, 3, 3)), titleColor, 0.9f);

            centerTitlePos.Y += 50;




            string finalScoreString = string.Format(Resource.MenuGameOverScore, Helpers.GetPlayerName(), finalScore);
            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.SerifBig,
                finalScoreString,
                centerTitlePos+ new Vector2(0, Helpers.Pulse(gameTime, 5, 3)), titleColor, 0.85f);



            if (newGlobalHighScore || (!base.ShowOnlineScores && addedIndex >= 0))
            {
                Color newScoreColor = Color.Lerp(Helpers.transColor(Color.Orange), Color.Orange, Transition);
                Vector2 newScorePos = centerTitlePos;
                newScorePos.X += 310;
                newScorePos.Y += 70;
                float newScoreSize = 0.45f + Helpers.Pulse(gameTime, 0.01f, 6);
                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Gothic,
                    Resource.MenuHighScoresNewScore,
                    newScorePos, newScoreColor, newScoreSize, -0.12f);
            }



            centerTitlePos.Y += 90;


            string title = string.Empty;

            if (base.ShowOnlineScores)
            {
                if (FrameworkCore.isHardcoreMode)
                    title = Resource.MenuHighscoresHardcoreHall;
                else
                    title = Resource.MenuHighScoresTitleOnline;
            }
            else
            {
                title = Resource.MenuHighScoresTitleLocal;
            }


            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Gothic,
                title,
                centerTitlePos, titleColor, 0.7f);



            
            



            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");

            Vector2 entryPos = titlePos;
            entryPos.Y += 230;

            base.DrawEntries(gameTime, entryPos, addedIndex);

            base.DrawConfirmButtons(gameTime);

#if XBOX
            Helpers.DrawLegend(Resource.MenuDone, sprite.buttons.a, Transition);
#endif
        }






    }
}
