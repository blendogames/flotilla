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
    public class HelpOrders : SysMenu
    {
        int WINDOWWIDTH = 1080;
        int WINDOWHEIGHT = 550;

        Vector2[] confirmButtons;
        int hoverConfirmButton = -1;

        public HelpOrders()
        {
            darkenScreen = true;

            confirmButtons = new Vector2[3]
            {
                new Vector2(150,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),


                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 270,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),

                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 150,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),
            };
        }

        private void OnDone(object sender, EventArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            UpdateMouseInput(gameTime, inputManager);
            UpdateGamepad(gameTime, inputManager);

            base.Update(gameTime, inputManager);
            base.UpdateMouseItems(gameTime, inputManager);
        }

        private void Done()
        {
            FrameworkCore.PlayCue(sounds.click.activate);
            Deactivate();
        }

        private void PrevPage()
        {
            FrameworkCore.PlayCue(sounds.click.activate);
            Owner.AddMenu(new HelpArmor());
            Deactivate();
        }

        private void NextPage()
        {
            FrameworkCore.PlayCue(sounds.click.activate);
            Owner.AddMenu(new HelpTurns());
            Deactivate();
        }

        private void UpdateGamepad(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;

            if (inputManager.sysMenuLeft)
            {
                PrevPage();
            }
            else if (inputManager.sysMenuRight)
            {
                NextPage();
            }
            else if (inputManager.buttonAPressed)
            {
                Done();
            }
        }


        private void UpdateMouseInput(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;

            bool confirmHover = false;
            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Rectangle confirmRect = new Rectangle(
                    (int)(confirmButtons[i].X - 50),
                    (int)(confirmButtons[i].Y - 30),
                    100,
                    60);

                if (confirmRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    hoverConfirmButton = i;
                    confirmHover = true;

                    if (inputManager.mouseLeftClick)
                    {
                        if (i == 0)
                        {
                            Done();
                        }
                        else if (i == 1)
                        {
                            PrevPage();
                        }
                        else if (i==2)
                        {
                            NextPage();
                        }
                    }
                }
            }

            if (!confirmHover)
                hoverConfirmButton = -1;
        }

        private void DrawLegend()
        {
            float x = Helpers.DrawLegend(Resource.MenuDone, sprite.buttons.a, Transition);
            Helpers.DrawLegendAt(Resource.HelpPrevNext, sprite.buttons.leftRight, Transition, x-32);
        }

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

#if WINDOWS
            DrawConfirmButtons(gameTime);
#else
            DrawLegend();
#endif

            Vector2 transitionMod = new Vector2(Helpers.PopLerp(Transition, -100, 40, 0),0);

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);
            
            Vector2 titleVec = FrameworkCore.Gothic.MeasureString(Resource.MenuPaused);
            Vector2 titlePos = Helpers.GetScreenCenter();
            titlePos.X -= WINDOWWIDTH / 2;
            titlePos.Y -= WINDOWHEIGHT / 2;
            titlePos += transitionMod;



            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.HelpOrdersTitle, titlePos, titleColor, darkColor,
                0, Vector2.Zero, 1);

            // draw content.
            titlePos.Y += titleVec.Y;
            DrawTextBlock(titlePos, new string[] { descTxt });



            Vector2 offset = new Vector2(WINDOWWIDTH / 3 + 10, 0);

            int turnVec = (int)FrameworkCore.Serif.MeasureString(descTxt).Y;
            titlePos.Y += turnVec + 60;


            Color phaseColor = Color.Goldenrod;
            phaseColor = Color.Lerp(Helpers.transColor(phaseColor), phaseColor, Transition);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.SerifBig, Resource.HelpOrdersAttackTitle, titlePos,
                phaseColor, 0, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.SerifBig, Resource.HelpOrdersFlankTitle,
                titlePos + offset,
                phaseColor, 0, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.SerifBig, Resource.HelpOrdersFocusTitle,
                titlePos + (offset*2),
                phaseColor, 0, Vector2.Zero, 0.9f, SpriteEffects.None, 0);





            int phaseVec = (int)(FrameworkCore.SerifBig.MeasureString("S").Y * 0.9f);
            titlePos.Y += phaseVec;


            DrawTextBlock(titlePos, new string[] { descAttack });

            DrawTextBlock(titlePos + offset, new string[] { descFlank });

            DrawTextBlock(titlePos + (offset*2), new string[] { descFocus});
        }



        string descTxt = string.Empty;

        string descAttack = string.Empty;
        string descFlank = string.Empty;
        string descFocus = string.Empty;

        public override void Activate()
        {
            descTxt = Helpers.StringWrap(FrameworkCore.Serif, Resource.HelpOrdersDesc, WINDOWWIDTH);
            descAttack = Helpers.StringWrap(FrameworkCore.Serif, Resource.HelpOrdersAttack, WINDOWWIDTH / 3 - 10);
            descFlank = Helpers.StringWrap(FrameworkCore.Serif, Resource.HelpOrdersFlank, WINDOWWIDTH/3 - 10);
            descFocus = Helpers.StringWrap(FrameworkCore.Serif, Resource.HelpOrdersFocus, WINDOWWIDTH/3 - 10);

            base.Activate();
        }






        private void DrawTextBlock(Vector2 pos, string[] txt)
        {
            int fontHeight = (int)FrameworkCore.Serif.MeasureString("S").Y;

            Color txtColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), Transition);

            for (int i = 0; i < txt.Length; i++)
            {
                Helpers.DrawOutline(FrameworkCore.Serif, txt[i], pos,
                    txtColor, darkColor);
                pos.Y += fontHeight;
            }
        }

        private void DrawTextBlockCenter(Vector2 pos, string[] txt)
        {
            int fontHeight = (int)FrameworkCore.Serif.MeasureString("S").Y;

            Color txtColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), Transition);

            for (int i = 0; i < txt.Length; i++)
            {
                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif, txt[i], pos,
                    txtColor, darkColor, 1,0 );
                pos.Y += fontHeight;
            }
        }



        private void DrawConfirmButtons(GameTime gameTime)
        {

            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Vector2 buttonPos = confirmButtons[i];
                //buttonPos.Y += (int)Helpers.PopLerp(Transition, 200, -30, 0);

                Rectangle descriptionRect = new Rectangle(
                    (int)(buttonPos.X - 50),
                    (int)(buttonPos.Y - 30),
                    100,
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
                    glowColor = Color.Lerp(Helpers.transColor(glowColor), glowColor, Transition);

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, glowRect,
                                        sprite.vistaBox, glowColor);

                }

                buttonColor = Color.Lerp(Helpers.transColor(buttonColor), buttonColor, Transition);
                textColor = Color.Lerp(Helpers.transColor(textColor), textColor, Transition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, descriptionRect,
                    sprite.vistaBox, buttonColor);



                //draw the text.
                string buttText = string.Empty;
                if (i == 0)
                {
                    buttText = Resource.MenuDone;
                }
                else if (i==1)
                    buttText = Resource.HelpPrev;
                else
                    buttText = Resource.HelpNext;

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, buttText,
                    buttonPos, textColor, 1);
            }
        }
    }
}
