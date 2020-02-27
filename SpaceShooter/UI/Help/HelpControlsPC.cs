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
    public class HelpControlsPC : SysMenu
    {
        int WINDOWWIDTH = 900;
        int WINDOWHEIGHT = 550;

        Vector2[] confirmButtons;
        int hoverConfirmButton = -1;

        public HelpControlsPC()
        {
            darkenScreen = true;

            confirmButtons = new Vector2[2]
            {
                new Vector2(280,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),

                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 280,
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

            base.Update(gameTime, inputManager);
            base.UpdateMouseItems(gameTime, inputManager);
        }


        private void UpdateMouseInput(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;

            bool confirmHover = false;
            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Rectangle confirmRect = new Rectangle(
                    (int)(confirmButtons[i].X - 110),
                    (int)(confirmButtons[i].Y - 30),
                    220,
                    60);

                if (confirmRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
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
                            Owner.AddMenu(new HelpControlsXBOX());
                            Deactivate();
                        }
                    }
                }
            }

            if (!confirmHover)
                hoverConfirmButton = -1;
        }

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            DrawConfirmButtons(gameTime);

            Vector2 transitionMod = new Vector2(Helpers.PopLerp(Transition, -100, 40, 0),0);

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);
            
            Vector2 titleVec = FrameworkCore.Gothic.MeasureString(Resource.MenuPaused);
            Vector2 titlePos = Helpers.GetScreenCenter();
            titlePos.X -= WINDOWWIDTH / 2;
            titlePos.Y -= WINDOWHEIGHT / 2;
            titlePos += transitionMod;
            
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.HelpViewControls, titlePos, titleColor, darkColor,
                0, Vector2.Zero, 1);


            Vector2 keyboardPos = Helpers.GetScreenCenter();
            keyboardPos.X -= WINDOWWIDTH / 2;
            keyboardPos.X += 87;
            DrawElement(keyboardPos + transitionMod,
                sprite.buttons.kbWasd, new string[]{ Resource.TutorialMoveCamera});


            Vector2 shiftPos = Helpers.GetScreenCenter();
            shiftPos.X -= WINDOWWIDTH / 2;
            shiftPos.X += 87;
            shiftPos.Y += 130;
            shiftPos.X += Helpers.PopLerp(Transition, -100, 40, 0);
            shiftPos.Y += Helpers.PopLerp(Transition, 100, -40, 0);
            DrawElement(shiftPos,
                sprite.buttons.kbShift, new string[] { Resource.HelpLeftStick3 });




            Vector2 spacePos = Helpers.GetScreenCenter();
            spacePos.Y += Helpers.PopLerp(Transition, 200, -50, 0);
            spacePos.X -= 100;
            DrawElement(spacePos,
                sprite.buttons.spacebar, new string[] { Resource.HelpConfirmSelectNext, Resource.HelpConfirmSelectNext2 });


            Vector2 mousePos = Helpers.GetScreenCenter();
            mousePos.X += Helpers.PopLerp(Transition, 200, -50, 0);
            mousePos.X += 270;
            mousePos.Y += 56;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, mousePos, sprite.buttons.bigMouse, titleColor, 0,
                Helpers.SpriteCenter(sprite.buttons.bigMouse), 1, SpriteEffects.None, 0);


            Color arrowColor = Color.Goldenrod;
            arrowColor = Color.Lerp(Helpers.transColor(arrowColor), arrowColor, Transition);

            //LEFT BUTTON ARROW
            Vector2 leftArrowPos = mousePos + new Vector2(-22, -45);
            float leftArrowAngle = Helpers.Pulse(gameTime, 0.03f, 3);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, leftArrowPos,
                sprite.helpArrow, arrowColor, leftArrowAngle,
                new Vector2(60,18), 1, SpriteEffects.None, 0);

            //RIGHT BUTTON ARROW
            Vector2 rightArrowPos = mousePos + new Vector2(22, -45);
            float rightArrowAngle = Helpers.Pulse(gameTime, 0.03f, 4);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rightArrowPos,
                sprite.helpArrow, arrowColor, rightArrowAngle,
                new Vector2(4, 18), 1, SpriteEffects.FlipHorizontally, 0);

            //MOUSEWHEEL
            Vector2 mousewheelPos = mousePos + new Vector2(0, -57);
            float wheelArrowAngle = 1.57f + Helpers.Pulse(gameTime, 0.03f, 2);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, mousewheelPos,
                sprite.helpArrow, arrowColor, wheelArrowAngle,
                new Vector2(60, 18), 1, SpriteEffects.None, 0);


            //right mouse button
            DrawTextBlock(mousePos + new Vector2(70, -40),
                new string[]
                {
                    Resource.HelpRightButton1,
                    Resource.HelpRightButton2,
                    "",
                    Resource.HelpRightButton3,
                    Resource.HelpRightButton2,
                });

            DrawTextBlock(mousePos + new Vector2(-190, -40),
                 new string[]
                {
                    Resource.HelpLeftButton1,
                });

            DrawTextBlockCenter(mousePos + new Vector2(0, -160),
                new string[]
                {
                    Resource.HelpWheel1,
                    Resource.HelpWheel2,
                });
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


        private void DrawElement(Vector2 pos, Rectangle img, string[] txt)
        {
            Color txtColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), Transition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, img, txtColor, 0,
                Helpers.SpriteCenter(img), 1, SpriteEffects.None, 0);

            pos.Y += img.Height / 2;

            

            int fontHeight = (int)FrameworkCore.Serif.MeasureString("S").Y;
            pos.Y += fontHeight/2;

            for (int i = 0; i < txt.Length; i++)
            {
                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif, txt[i], pos,
                    txtColor, darkColor, 1, 0);
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
                    (int)(buttonPos.X - 110),
                    (int)(buttonPos.Y - 30),
                    220,
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
                else
                    buttText = Resource.HelpNextPage;

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, buttText,
                    buttonPos, textColor, 1);
            }
        }
    }
}
