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

using Microsoft.Xna.Framework.Storage;
using System.IO;
#if XBOX

#endif
#endregion

namespace SpaceShooter
{
    public class NameMenu : SysMenu
    {
        /// <summary>
        /// Player enters a commander name. Windows only.
        /// </summary>
        public NameMenu()
        {
            transitionOnTime = 300;
            transitionOffTime = 300;

            canBeExited = false;

            LINEHEIGHT = (int)FrameworkCore.SerifBig.MeasureString("Sample").Y;
            GOTHICHEIGHT = (int)FrameworkCore.Gothic.MeasureString("Sample").Y;


            
        }

        int LINEHEIGHT = 0;
        int GOTHICHEIGHT = 0;

        int blinkTimer = 0;
        int backspaceTimer = 0;

        

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (Transition >= 1)
            {
                if (inputManager.kbEnter)
                {
                    MenuDone();
                }


                if (inputManager.kbBackspaceHold)
                {
                    HandleBackspace(inputManager.kbBackspaceJustPressed);
                }
                else
                {
                    //key is not held. reset the timer.
                    backspaceTimer = 0;

                    HandleKeyInput(inputManager);
                }

                backspaceTimer = Math.Max(
                    backspaceTimer - (int)gameTime.ElapsedGameTime.TotalMilliseconds,
                    0);



                if (inputManager.mouseLeftClick)
                    MenuDone();
            }

            blinkTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;


            if (blinkTimer <= 0)
                blinkTimer = 600;

            base.Update(gameTime, inputManager);
        }

        private void MenuDone()
        {
            bool invalidName = false;

            string playerName = FrameworkCore.players[0].commanderName;


            if (!Helpers.IsValidPlayerName(playerName))
                invalidName = true;

            if (invalidName)
            {
                FrameworkCore.players[0].commanderName = Helpers.GenerateName("Gamertag");
                return;
            }

            FrameworkCore.players[0].commanderName = Helpers.StripOutAmpersands(FrameworkCore.players[0].commanderName);
                

            SaveInfo save = FrameworkCore.storagemanager.GetDefaultSaveData();            
            FrameworkCore.storagemanager.SaveData(save);

            Deactivate();
            Owner.AddMenu(new MainMenu());
        }

        private void HandleBackspace(bool justHeld)
        {
            if (backspaceTimer > 0)
                return;

            if (FrameworkCore.players[0].commanderName.Length <= 0)
                return;

            int nameLength = FrameworkCore.players[0].commanderName.Length;
            FrameworkCore.players[0].commanderName = FrameworkCore.players[0].commanderName.Remove(nameLength-1, 1);

            if (justHeld)
                backspaceTimer = 300;
            else
                backspaceTimer = 20;
        }

        private void HandleKeyInput(InputManager inputManager)
        {
            if (FrameworkCore.players[0].commanderName.Length >= 15)
                return;

            List<Keys> keyToAdd = inputManager.getPressedKeys;

            if (keyToAdd == null)
                return;

            foreach (Keys key in keyToAdd)
            {
                bool shift = inputManager.kbShiftHeld;
                string letterToAdd = Helpers.ConvertKeyToChar(key, shift, false);

                if (letterToAdd == string.Empty)
                    return;

                FrameworkCore.players[0].commanderName = string.Concat(FrameworkCore.players[0].commanderName,
                    letterToAdd);
            }            
        }

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {
            base.Activate();
        }

        public override void Draw(GameTime gameTime)
        {
            Color textColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0,0,0,64), Transition);


            Vector2 promptPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2 - LINEHEIGHT/2);
            promptPos.Y -= 64+8;
            promptPos.Y += Helpers.PopLerp(Transition, -100, 30, 0);
            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.SerifBig,
                Resource.MenuNamePrompt, promptPos, textColor, bgColor, 1, 0);



            Vector2 namePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 + GOTHICHEIGHT/2);
            namePos.Y += Helpers.PopLerp(Transition, 100, -30, 0);
            namePos.Y -= 64;
            Color boxColor = Color.Lerp(Helpers.transColor(Faction.Blue.teamColor),
                Faction.Blue.teamColor, Transition);
            Rectangle nameBox = new Rectangle(
                (int)namePos.X - 384,
                (int)namePos.Y - GOTHICHEIGHT / 2,
                768,
                GOTHICHEIGHT);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, nameBox, sprite.vistaBox,
                boxColor);

            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Gothic,
                FrameworkCore.players[0].commanderName, namePos, textColor, bgColor, 1, 0);



            if (!FrameworkCore.isTrialMode())
            {
                Color goldColor = Color.Lerp(Helpers.transColor(Color.Gold), Color.Gold, Transition);

                Vector2 thankyouPos = new Vector2(promptPos.X,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.23f);
                thankyouPos.Y += Helpers.Pulse(gameTime, 5, 4);
                thankyouPos.Y += Helpers.PopLerp(Transition, -200, 40, 0);
                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.SerifBig,
                    Resource.ThankYou, thankyouPos, goldColor, bgColor, 1.2f, 0);
            }



            if (blinkTimer > 300)
            {
                int nameVec = (int)FrameworkCore.Gothic.MeasureString(FrameworkCore.players[0].commanderName).X;
                Vector2 underScorePos = namePos;
                underScorePos.X += nameVec / 2;
                underScorePos.Y -= GOTHICHEIGHT / 2;
                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, "_", underScorePos, textColor);
            }


            Helpers.DrawClickMessage(gameTime, Transition);
        }

    }
}
