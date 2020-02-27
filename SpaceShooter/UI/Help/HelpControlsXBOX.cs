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
    public class HelpControlsXBOX : SysMenu
    {
        int WINDOWWIDTH = 900;
        int WINDOWHEIGHT = 550;

        Vector2[] confirmButtons;
        int hoverConfirmButton = -1;

        public HelpControlsXBOX()
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
#if WINDOWS
            UpdateMouseInput(gameTime, inputManager);
#else
            if (Transition >= 1)
            {
                if (inputManager.buttonAPressed || inputManager.buttonBPressed)
                {
                    Deactivate();
                }
            }
#endif


            base.Update(gameTime, inputManager);

#if WINDOWS
            base.UpdateMouseItems(gameTime, inputManager);
#endif
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
                            Owner.AddMenu(new HelpControlsPC());
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

#if WINDOWS
            DrawConfirmButtons(gameTime);
#else
            Helpers.DrawLegend(Resource.MenuOK, sprite.buttons.a, Transition);
#endif

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


            DrawControllerFront(gameTime);

            DrawControllerTop(gameTime);            
        }

        private void DrawControllerTop(GameTime gameTime)
        {
            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);

            //draw the xbox controller top.
            Vector2 controlTop = Helpers.GetScreenCenter();
            controlTop.X += Helpers.PopLerp(Transition, 100, -50, 0);
            controlTop.X += 340;
            controlTop.Y += 30;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, controlTop, sprite.buttons.xboxControllerTop,
                titleColor, 0, Helpers.SpriteCenter(sprite.buttons.xboxControllerTop), 1, SpriteEffects.None, 0);

            //left trigger
            DrawArrow(gameTime, controlTop + new Vector2(-108, -77), 1.27f, 1, SpriteEffects.None);
            DrawTextBlockCenter(controlTop + new Vector2(-108, -175),
                new string[]
                {
                    Resource.HelpLTrigger1,
                    Resource.HelpCamera
                });

            //right trigger
            DrawArrow(gameTime, controlTop + new Vector2(108, -77), 1.87f, 1, SpriteEffects.None);
            DrawTextBlockCenter(controlTop + new Vector2(108, -175),
                new string[]
                {
                    Resource.HelpRTrigger1,
                    Resource.HelpCamera
                });

            //Left bumper
            DrawArrow(gameTime, controlTop + new Vector2(-120, -24), -1.57f, 1.85f, SpriteEffects.None);
            DrawTextBlockCenter(controlTop + new Vector2(-120, 100),
                new string[]
                {
                    Resource.HelpLBumper1,
                    Resource.HelpBumper2,
                    Resource.HelpBumper3
                });

            //Right bumper
            DrawArrow(gameTime, controlTop + new Vector2(120, -24), -1.57f, 1.85f, SpriteEffects.FlipVertically);
            DrawTextBlockCenter(controlTop + new Vector2(120, 100),
                new string[]
                {
                    Resource.HelpRBumper1,
                    Resource.HelpBumper2,
                    Resource.HelpBumper3
                });
        }

        private void DrawControllerFront(GameTime gameTime)
        {
            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);

            //draw the xbox controller front.
            Vector2 controlFront = Helpers.GetScreenCenter();
            controlFront.X += Helpers.PopLerp(Transition, -100, 50, 0);
            controlFront.X -= 340;
            controlFront.Y += 30;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, controlFront, sprite.buttons.xboxController,
                titleColor, 0, Helpers.SpriteCenter(sprite.buttons.xboxController), 1, SpriteEffects.None, 0);


            //left stick.
            DrawArrow(gameTime, controlFront + new Vector2(-98, -65), 1.4f, 1, SpriteEffects.None);
            DrawTextBlockCenter(controlFront + new Vector2(-55, -185),
                new string[]
                {
                    Resource.HelpLeftStick1,
                    Resource.HelpLeftStick2,
                    Resource.HelpLeftStick3,
                });

            //right stick.
            DrawArrow(gameTime, controlFront + new Vector2(35, 20), -1.17f, 1.35f, SpriteEffects.None);
            DrawTextBlockCenter(controlFront + new Vector2(0, 125),
                new string[]
                {
                    Resource.HelpRightButton1,
                    Resource.HelpWheel2,
                });

            //A Button
            Vector2 buttonPos = controlFront + new Vector2(200, -90);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, sprite.buttons.a,
                titleColor, 0, Helpers.SpriteCenter(sprite.buttons.a), 1, SpriteEffects.None, 0);
            DrawTextBlock(buttonPos + new Vector2(30, -13), new string[] { Resource.HelpLeftButton1 });

            //B button.
            buttonPos.Y += 48;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, sprite.buttons.b,
                titleColor, 0, Helpers.SpriteCenter(sprite.buttons.b), 1, SpriteEffects.None, 0);
            DrawTextBlock(buttonPos + new Vector2(30, -13), new string[] { Resource.MenuCancel });


            //xbutton
            buttonPos.Y += 48;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, sprite.buttons.x,
                titleColor, 0, Helpers.SpriteCenter(sprite.buttons.x), 1, SpriteEffects.None, 0);
            DrawTextBlock(buttonPos + new Vector2(30, -13), new string[] { Resource.HelpConfirmSelectNext2});
        }

        private void DrawArrow(GameTime gameTime, Vector2 pos, float angle, float size, SpriteEffects fx)
        {
            Color titleColor = Color.Goldenrod;
            titleColor = Color.Lerp(Helpers.transColor(titleColor), titleColor, Transition);
            angle += Helpers.Pulse(gameTime, 0.02f, 3);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos,
                sprite.helpArrow, titleColor, angle,
                new Vector2(60, 18), size, fx, 0);
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
