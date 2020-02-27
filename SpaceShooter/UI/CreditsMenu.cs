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
    public class CreditsMenu : SysMenu
    {
        string[] creditLines;

        int LINESIZE;

        public CreditsMenu(bool showUnlockMessage)
        {
            string lastLine = showUnlockMessage ? 
                string.Format(Resource.MenuCreditsUnlock, FrameworkCore.adventureNumber)
                :
                "";

            creditLines = new string[]
            {                
                Resource.MenuCreditsBlendoSite,
                "",
                "",
                Resource.MenuCreditsCreatedBy,
                Resource.MenuCreditsBrendon,
                "",
                "",
                Resource.MenuCreditsAudioBy,
                Resource.MenuCreditsSoundSnap,
                "",
                "",
                Resource.MenuCreditsPortedBy,
                Resource.MenuCreditsEthan,
                "",
                "",
                Resource.MenuCreditsSpecialThanks,
                Resource.CreditsNameDaniel,
                Resource.CreditsNameDrew,
                Resource.CreditsNameNeil,
                Resource.CreditsNameRobert,
                Resource.CreditsNameSherman,
                Resource.CreditsNameTom,
                Resource.CreditsNameVenny,
                "",
                "",
                lastLine,
                "",
                "",
                Resource.copyright,
            };



            MenuItem item = new MenuItem(Resource.MenuDone);
            item.Selected += OnDone;
            base.AddItem(item);

            //font size
            LINESIZE = (int)(FrameworkCore.SerifBig.MeasureString("Sample").Y);

            PosY = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2;
        }

        float fontSize = 0.9f;
        float PosY = 0;

        private void OnDone(object sender, EventArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }

        int delayTimer = 100;
        int scrollTimer = 0;
        bool scrollDown = true;

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (Transition >= 1)
            {
#if WINDOWS
                if (inputManager.mouseLeftClick)
                    Deactivate();

                if (inputManager.mouseWheelDown)
                {
                    scrollTimer = 200;
                    scrollDown = true;
                }
                else if (inputManager.mouseWheelUp)
                {
                    scrollTimer = 200;
                    scrollDown = false;
                }

                if (scrollTimer > 0)
                    scrollTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                
#endif

                if (Math.Abs(inputManager.stickLeft.Y) > 0.2f || scrollTimer > 0)
                {
                    delayTimer = 400;

                    if (inputManager.stickLeft.Y < 0
#if WINDOWS
                        || scrollDown
#endif
                        )
                    {
                        PosY = MathHelper.Clamp(PosY - 1f * (float)gameTime.ElapsedGameTime.TotalMilliseconds,
                            (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - (creditLines.Length * LINESIZE),
                            FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);
                    }
                    else if (inputManager.stickLeft.Y > 0
#if WINDOWS
                        || !scrollDown
#endif
                        )
                    {
                        PosY = MathHelper.Clamp(PosY + 1f * (float)gameTime.ElapsedGameTime.TotalMilliseconds,
                            (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - (creditLines.Length * LINESIZE),
                            FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);
                    }
                }
                else if (delayTimer <= 0)
                {
                    PosY = MathHelper.Clamp(PosY - 0.03f * (float)gameTime.ElapsedGameTime.TotalMilliseconds,
                        (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - (creditLines.Length * LINESIZE),
                        FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);
                }

                delayTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            base.Update(gameTime, inputManager);
        }


        public override void Draw(GameTime gameTime)
        {
            Vector2 startPos = new Vector2(
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2,
                PosY);


            Color fontColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, Transition);
            float displayFontSize = Helpers.PopLerp(Transition, 0, 1.2f, fontSize);
            int titleVec = (int)FrameworkCore.Gothic.MeasureString("Sample").Y;

            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Gothic,
                    Resource.MenuTitle, startPos + new Vector2(0, -titleVec/2), fontColor, bgColor, displayFontSize, 0);

            for (int i = 0; i < creditLines.Length; i++)
            {
                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.SerifBig,
                    creditLines[i], startPos, fontColor, bgColor, displayFontSize, 0);

                startPos.Y += LINESIZE;
            }

        }

    }
}
