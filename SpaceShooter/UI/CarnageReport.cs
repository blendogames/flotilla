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
    public class CarnageReport : SysMenu
    {
        List<Commander> commanders1 = new List<Commander>();
        List<Commander> commanders2 = new List<Commander>();

        bool playerVictory;

        Vector2[] bottomButtonPositions;
        int bottomButtonHover = -1;

        public CarnageReport(bool playerWon)
        {
            this.playerVictory = playerWon;

            canBeExited = false;

            CreateCommanderList(Faction.Blue, commanders1, false);
            CreateCommanderList(Faction.Blue, commanders2, true);

            bottomButtonPositions = new Vector2[2];
            bottomButtonPositions[0] = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 120 - 125 - 256,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 100);
            bottomButtonPositions[1] = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 120 - 125,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 100);


            //BC 6-12-2011 Summer Achievement
#if STEAM
            if (this.playerVictory && FrameworkCore.isCampaign && !FrameworkCore.worldMap.IsRunningTutorial)
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(FrameworkCore.playbackSystem.WorldTimer);
                int totalSeconds = (int)timeSpan.TotalSeconds;

                if (totalSeconds <= 94)
                {
                    FrameworkCore.SetAchievement("ach_summer");
                }
            }
#endif
        }

        /// <summary>
        /// Create a list of commanders associated with a faction
        /// </summary>
        /// <param name="faction"></param>
        /// <param name="commanderList"></param>
        /// <param name="FilterReverse">Create a list of commanders NOT associated with the faction.</param>
        private void CreateCommanderList(FactionInfo faction, List<Commander> commanderList, bool FilterReverse)
        {
            foreach (PlayerCommander player in FrameworkCore.players)
            {
                if (player.factionName != faction && !FilterReverse)
                    continue;
                else if (player.factionName == faction && FilterReverse)
                    continue;

                commanderList.Add(player);                
            }

            foreach (Commander enemy in FrameworkCore.level.Enemies)
            {
                if (enemy.factionName != faction && !FilterReverse)
                    continue;
                else if (enemy.factionName == faction && FilterReverse)
                    continue;

                commanderList.Add(enemy);
            }
        }


        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            foreach (PlayerCommander player in FrameworkCore.players)
            {
                if (player.playbackMenuTransition > 0)
                    continue;

                if (player.inputmanager.buttonBPressed || player.inputmanager.kbBackspaceJustPressed)
                {
                    player.ActivatePlaybackMode(null, null);
                }
            }

            if (!FrameworkCore.players[0].IsPlaybackMode() && FrameworkCore.players[0].playbackMenuTransition <= 0 &&
                !PlayerIsInPlayback)
            {
                if (inputManager.buttonAPressed || inputManager.kbEnter)
                {
                    Deactivate();
                }
            }

#if WINDOWS
            HandleMouse(inputManager);
#endif

            base.Update(gameTime, inputManager);
        }

        private void HandleMouse(InputManager inputManager)
        {
            if (Transition < 1)
                return;

            bool mouseHover = false;
            for (int i = 0; i < bottomButtonPositions.Length; i++)
            {
                Rectangle bottomButtonRect = new Rectangle(
                    (int)bottomButtonPositions[i].X - 125,
                    (int)bottomButtonPositions[i].Y - 24,
                    250,
                    48);

                if (bottomButtonRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    mouseHover = true;
                    bottomButtonHover = i;

                    if (inputManager.mouseLeftClick)
                    {
                        if (i == 0)
                        {
                            //view playback.
                            //Only player 1 has mouse support, so just start the playback for Player 1.
                            FrameworkCore.players[0].ActivatePlaybackMode(null, null);
                        }
                        else
                        {
                            //done.
                            Deactivate();
                        }
                    }
                }
            }

            if (inputManager.kbSpace)
                Deactivate();

            if (!mouseHover)
                bottomButtonHover = -1;
        }
        

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();

            //where player is sent after carnage screen is exited.
            if (FrameworkCore.isCampaign)
                FrameworkCore.worldMap.CombatCompleted();                
            else
                FrameworkCore.ExitToMainMenu(new SkirmishMenu());                
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {
            FrameworkCore.PlayCue(sounds.Music.nocturnes);

            base.Activate();
        }

        private bool PlayerIsInPlayback
        {
            get
            {
                bool playbackActive = false;
                foreach (PlayerCommander player in FrameworkCore.players)
                {
                    if (player.IsPlaybackMode())
                        playbackActive = true;
                }

                return playbackActive;
            }
        }

        int lineLength = 800;
        int killLength = 100;

        public override void Draw(GameTime gameTime)
        {
            if (PlayerIsInPlayback)
                return;

            Vector2 pos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 - 400,
                110);
            pos.X += Helpers.PopLerp(Transition, -300,50,0);


            Helpers.DarkenScreen(128);

            float titleAngle = -0.03f - Helpers.Pulse(gameTime, 0.01f, 1);

            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");
            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuCarnage, pos, titleColor, darkColor,
                titleAngle, Vector2.Zero, 0.9f);

            //draw the match time.
            float timeSize = 1.2f;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(FrameworkCore.playbackSystem.WorldTimer);
            DateTime dt = new DateTime(timeSpan.Ticks);
            String timeString = dt.ToString("mm:ss");
            Vector2 timeVec = FrameworkCore.Serif.MeasureString(timeString);
            timeVec.X *= timeSize;
            timeVec.Y *= timeSize;
            Vector2 timePos = pos + new Vector2(lineLength, 0);            
            timePos.X -= timeVec.X;
            timePos.Y += timeVec.Y / 2;
            timePos.Y += Helpers.Pulse(gameTime, 4, 6);
            Helpers.DrawOutline(FrameworkCore.Serif, timeString, timePos, titleColor, darkColor,
                0, Vector2.Zero, 1.1f);

            Vector2 timeIconPos = timePos;
            timeIconPos.X -= 16;
            timeIconPos.Y += timeVec.Y / 2;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, timeIconPos, sprite.tinyHourglass,
                Color.White, 0, Helpers.SpriteCenter(sprite.tinyHourglass), 1, SpriteEffects.None, 0);


            //gap betweeen title and names.
            pos.Y += titleVec.Y + 8;

            Vector2 killVec = FrameworkCore.Serif.MeasureString("Sample");
            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                Resource.MenuCarnageKills, pos + new Vector2(lineLength - ((killLength - 8) / 2), killVec.Y),
                Color.White, 1);


            //draw the faction name.
            if (commanders1.Count > 0)
            {
                if (commanders1[0] != null)
                {
                    Vector2 factionVec = FrameworkCore.Gothic.MeasureString(commanders1[0].factionName.Name);
                    factionVec.X *= 0.5f;
                    factionVec.Y *= 0.5f;
                    Helpers.DrawOutline(FrameworkCore.Gothic, commanders1[0].factionName.Name,
                        pos, titleColor, darkColor,
                        0, Vector2.Zero, 0.5f);


                    if (playerVictory)
                    {
                        Vector2 iconPos = pos + new Vector2(factionVec.X + 32, 21);




                        DrawVictoryText(gameTime, iconPos);
                    }
                    else
                    {
                        Vector2 iconPos = pos + new Vector2(factionVec.X + 32, 21);




                        DrawDefeatText(gameTime, iconPos);
                    }

                    pos.Y += factionVec.Y + gapSize() / 2;
                }
            }


            foreach (Commander commander in commanders1)
            {
                DrawCommanderLine(commander, pos);
                pos.Y += gapSize();
            }

            if (commanders2.Count > 0)
            {
                if (commanders2[0] != null)
                {
                    Vector2 factionVec = FrameworkCore.Gothic.MeasureString(commanders2[0].factionName.Name);
                    factionVec.X *= 0.5f;
                    factionVec.Y *= 0.5f;
                    Helpers.DrawOutline(FrameworkCore.Gothic, commanders2[0].factionName.Name,
                        pos, titleColor, darkColor,
                        0, Vector2.Zero, 0.5f);

                    if (playerVictory)
                    {
                        DrawDefeatText(gameTime, pos + new Vector2(factionVec.X + 32, 21));
                    }
                    else
                    {
                        DrawVictoryText(gameTime, pos + new Vector2(factionVec.X + 32, 21));
                    }

                    pos.Y += factionVec.Y + gapSize() / 2;
                }

                foreach (Commander commander in commanders2)
                {
                    DrawCommanderLine(commander, pos);
                    pos.Y += gapSize();
                }
            }

#if WINDOWS
            for (int i = 0; i < bottomButtonPositions.Length; i++)
            {
                Vector2 boxPos = bottomButtonPositions[i];
                boxPos.Y += Helpers.PopLerp(Transition, 200, -30, 0);

                Color boxColor = new Color(96,96,96);
                Color textColor = Color.White;

                if (bottomButtonHover == i)
                {
                    boxColor = Faction.Blue.teamColor;
                    textColor = Color.Orange;
                }

                Rectangle bottomButtonRect = new Rectangle(
                    (int)boxPos.X - 125,
                    (int)boxPos.Y - 24,
                    250,
                    48);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, bottomButtonRect,
                    sprite.vistaBox, boxColor);

                string boxText = Resource.MenuCarnageHideScores;

                if (i > 0)
                    boxText = Resource.MenuDone;

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                    boxText, boxPos, textColor, 1);
            }
#else
            float z = Helpers.DrawLegend(Resource.MenuDone, sprite.buttons.a, Transition);
            Helpers.DrawLegendAt(Resource.MenuCarnageHideScores, sprite.buttons.b, Transition, z-32);
#endif
        }

        private void DrawVictoryText(GameTime gameTime, Vector2 pos)
        {
            //draw the glow.
            float glowSize = 1 + Helpers.Pulse(gameTime, 0.1f, 8);
            Color glowColor = Color.Lerp(new Color(255, 160, 0, 160), new Color(255, 160, 0, 96),
                0.5f + Helpers.Pulse(gameTime, 0.49f, 5));
            float glowAngle = (float)gameTime.TotalGameTime.TotalSeconds * 0.5f;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.sparkle, glowColor,
                glowAngle, Helpers.SpriteCenter(sprite.sparkle), glowSize, SpriteEffects.None, 0);

            glowAngle = (float)gameTime.TotalGameTime.TotalSeconds * -0.2f;
            glowSize = 0.8f - Helpers.Pulse(gameTime, 0.05f, 8);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.sparkle, glowColor,
                glowAngle, Helpers.SpriteCenter(sprite.sparkle), glowSize, SpriteEffects.None, 0);




            float size = 1 + Helpers.Pulse(gameTime, 0.04f, 8);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.icons.trophy, Color.White,
                0, Helpers.SpriteCenter(sprite.icons.trophy), size, SpriteEffects.None, 0);

            /*
            float size = 0.55f + Helpers.Pulse(gameTime, 0.01f, 2);
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuCarnageVictory,
                pos, Color.Goldenrod, new Color(0, 0, 0, 128),
                -0.08f, Vector2.Zero, size);*/
        }

        private void DrawDefeatText(GameTime gameTime, Vector2 pos)
        {
            float glowSize = 2.5f + Helpers.Pulse(gameTime, 0.3f, 5);
            Color glowColor = Color.Lerp(new Color(0, 0, 0, 255), new Color(0, 0, 0, 160),
                0.5f + Helpers.Pulse(gameTime, 0.49f, 8));
            float glowAngle = (float)gameTime.TotalGameTime.TotalSeconds * 0.5f;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.glow, glowColor,
                glowAngle, Helpers.SpriteCenter(sprite.glow), glowSize, SpriteEffects.None, 0);




            float size = 1 + Helpers.Pulse(gameTime, 0.03f, 5);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.icons.skull, Color.White,
                0, Helpers.SpriteCenter(sprite.icons.skull), size, SpriteEffects.None, 0);
            /*
            float size = 0.45f + Helpers.Pulse(gameTime, 0.01f, 1.5f);
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuCarnageDefeat,
                pos, new Color(255,40,40), new Color(0, 0, 0, 128),
                -0.08f, Vector2.Zero, size);*/
        }

        private int LineSize()
        {
            return (int)FrameworkCore.SerifBig.MeasureString("Sample").Y;
        }

        private int gapSize()
        {
            return ((int)FrameworkCore.SerifBig.MeasureString("Sample").Y + 16);
        }

        private void DrawCommanderLine(Commander commander, Vector2 position)
        {
            //draw the vistaBox.
            Rectangle nameRect = new Rectangle(
                (int)position.X,
                (int)(position.Y - LineSize() / 2),
                lineLength - killLength,
                LineSize());
            nameRect.Inflate(0, 4);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, nameRect, sprite.vistaBox,
                commander.TeamColor);

            //draw the icon.
            Rectangle iconRect = sprite.computer;
            float iconAngle = 0;

            if (commander.GetType() == typeof(PlayerCommander))
            {
#if WINDOWS
                if (commander == FrameworkCore.players[0])
                {
                    iconRect = sprite.mouseIcon;
                    iconAngle = 0;
                }
                else
                {
                    iconRect = sprite.xboxRing;
                    iconAngle = Helpers.GetRingAngle(((PlayerCommander)commander).playerindex);
                }
#else
                iconRect = sprite.xboxRing;
                iconAngle = Helpers.GetRingAngle(((PlayerCommander)commander).playerindex);
#endif                
            }

            Vector2 iconPos = position;
            iconPos.X += 8;
            iconPos.X += iconRect.Width / 2;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, iconPos, iconRect, Color.White,
                iconAngle, Helpers.SpriteCenter(iconRect), 1, SpriteEffects.None, 0);


            //draw the player name.
            iconPos.X += iconRect.Width / 2;
            iconPos.X += 8;
            Helpers.DrawOutline(FrameworkCore.SerifBig, commander.commanderName,
                iconPos + new Vector2(0, -3), Color.White, new Color(0, 0, 0, 64), 0,
                new Vector2(0, LineSize() / 2), 1);


            //draw box for kills.
            Rectangle killRect = new Rectangle(
                (int)position.X + lineLength - killLength + 8,
                (int)(position.Y - LineSize() / 2),
                killLength - 8,
                LineSize());
            killRect.Inflate(0, 4);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, killRect, sprite.vistaBox,
                commander.TeamColor);
            

            //kill string
            string killString = commander.roundKills.ToString();
            Vector2 killVec = FrameworkCore.SerifBig.MeasureString(killString);
            Vector2 killPos = position;
            killPos.X += lineLength - 8;


            Helpers.DrawOutline(FrameworkCore.SerifBig, killString,
                                        killPos + new Vector2(0, -3), Color.White, new Color(0, 0, 0, 64), 0,
                                        new Vector2(killVec.X, LineSize() / 2), 1);
        }





        private void DrawRectangle(Rectangle rect, Color rectColor)
        {
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rect, sprite.blank, rectColor);
        }

    }
}
