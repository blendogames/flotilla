
#if WINDOWS

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
    public class BugReport : SysMenu
    {
        const int DESCRIPTIONLINES = 11;

        int LINESIZE;
        Vector2[] textBoxes;
        Vector2[] confirmButtons;

        int windowWidth = 600;
        int windowHeight = 600;

        public BugReport()
        {
            LINESIZE = (int)FrameworkCore.Serif.MeasureString("Sample").Y;

            darkenScreen = true;

            transitionOnTime = 300;
            transitionOffTime = 300;


            textBoxes = new Vector2[2]
            {
                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 - windowWidth/2,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2 - 180),

                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 - windowWidth/2,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2 - 100),
            };

            confirmButtons = new Vector2[2]
            {
                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 - windowWidth/2 + 100,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),

                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 + windowWidth/2 - 100,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120),
            };
        }

        private void OnDone(object sender, EventArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (isSending)
                return;

            blinkTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (blinkTimer <= 0)
                blinkTimer = 500;

            backspaceTimer = Math.Max(
                backspaceTimer - (int)gameTime.ElapsedGameTime.TotalMilliseconds,
                0);


            UpdateText(gameTime, inputManager);

            UpdateInput(inputManager);

            UpdateMouseInput(gameTime, inputManager);

            base.Update(gameTime, inputManager);
        }


        private void UpdateMouseInput(GameTime gameTime, InputManager inputManager)
        {
            if (isSending)
                return;

            if (Transition < 1)
                return;

            Rectangle subjectRect = new Rectangle(
                (int)textBoxes[0].X,
                (int)textBoxes[0].Y,
                windowWidth,
                LINESIZE);

            subjectRect.Inflate(8, 8);

            Rectangle descriptionRect = new Rectangle(
                (int)textBoxes[1].X,
                (int)textBoxes[1].Y,
                windowWidth,
                LINESIZE * DESCRIPTIONLINES);

            descriptionRect.Inflate(8, 8);

            if (subjectRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
            {
                hoverTextBox = 0;

                if (inputManager.mouseLeftClick)
                    selectedTextBox = 0;
            }
            else if (descriptionRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
            {
                hoverTextBox = 1;

                if (inputManager.mouseLeftClick)
                    selectedTextBox = 1;
            }
            else
                hoverTextBox = -1;

            bool confirmHover = false;  
            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Rectangle confirmRect = new Rectangle(
                    (int)(confirmButtons[i].X - 100),
                    (int)(confirmButtons[i].Y - 30),
                    200,
                    60);

                if (confirmRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    hoverConfirmButton = i;
                    confirmHover = true;

                    if (inputManager.mouseLeftClick)
                    {
                        if (i == 0)
                        {
                            Deactivate();
                        }
                        else if (i == 1)
                        {
                            CommitReport();
                        }
                    }
                }
            }

            if (!confirmHover)
                hoverConfirmButton = -1;
        }


        public override void Deactivate()
        {
            FrameworkCore.PlayCue(sounds.click.activate);

            subjectText = "";
            descriptionText = "";

            base.Deactivate();
        }


        int hoverConfirmButton = -1;
        int hoverTextBox = -1;


        string subjectText= "";
        string descriptionText = "";

        int blinkTimer = 0;
        int backspaceTimer = 0;

        private void UpdateText(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1 || isSending)
                return;

            if (inputManager.kbBackspaceHold)
            {
                if (selectedTextBox == 0)
                {
                    string output = string.Empty;
                    if (HandleBackspace(inputManager.kbBackspaceJustPressed, subjectText, out output))
                    {
                        subjectText = output;
                    }
                }
                else
                {
                    string output = string.Empty;
                    if (HandleBackspace(inputManager.kbBackspaceJustPressed, descriptionText, out output))
                    {
                        descriptionText = output;
                    }
                }
                
            }
            else
            {
                //key is not held. reset the timer.
                backspaceTimer = 0;

                if (selectedTextBox == 0)
                {
                    string output = string.Empty;
                    if (HandleKeyInput(inputManager, subjectText, out output))
                    {
                        subjectText = output;
                    }
                }
                else
                {
                    string output = string.Empty;
                    if (HandleKeyInput(inputManager, descriptionText, out output))
                    {
                        descriptionText = output;
                    }
                }
                
            }
        }

        private bool HandleBackspace(bool justHeld, string selectedText, out string textOutput)
        {
            textOutput = selectedText;

            if (backspaceTimer > 0)
                return false;

            if (selectedText.Length <= 0)
                return false;

            selectedText = selectedText.Remove(selectedText.Length - 1, 1);

            if (justHeld)
                backspaceTimer = 300;
            else
                backspaceTimer = 20;


            textOutput = selectedText;
            return true;
        }

        int selectedTextBox = 0;

        private bool HandleKeyInput(InputManager inputManager, string selectedText, out string textOutput)
        {
            textOutput = selectedText;

            if (selectedTextBox == 0)
            {
                //subject line.
                if (FrameworkCore.Serif.MeasureString(selectedText).X + 16 >= windowWidth)
                    return false;
            }

            List<Keys> keyToAdd = inputManager.getPressedKeys;

            if (keyToAdd == null)
                return false;

            foreach (Keys key in keyToAdd)
            {
                bool shift = inputManager.kbShiftHeld;

                bool allowCarriageReturn = false;

                if (selectedTextBox >= 1)
                    allowCarriageReturn = true;

                string letterToAdd = Helpers.ConvertKeyToChar(key, shift, allowCarriageReturn);

                if (letterToAdd == string.Empty)
                    return false;

                selectedText = string.Concat(selectedText,
                    letterToAdd);
            }

            textOutput = selectedText;

            return true;
        }



        bool isSending = false;

        private void UpdateInput(InputManager inputManager)
        {
            if (Transition < 1)
                return;

            if (inputManager.kbTabPressed)
            {
                if (selectedTextBox == 0)
                    selectedTextBox = 1;
                else
                    selectedTextBox = 0;
            }

            if (inputManager.kbEnter && selectedTextBox == 0)
                selectedTextBox = 1;
        }

        private void CommitReport()
        {
            if (isSending)
                return;

            if (descriptionText.Length <= 0)
            {
                //error message.
                SysPopup signPrompt = new SysPopup(Owner, Resource.MenuBugReportEmptyDescription);
                signPrompt.transitionOnTime = 200;
                signPrompt.transitionOffTime = 200;
                signPrompt.darkenScreen = true;
                signPrompt.hideChildren = false;
                signPrompt.canBeExited = false;
                signPrompt.sideIconRect = sprite.windowIcon.error;

                MenuItem item = new MenuItem(Resource.MenuOK);
                item.Selected += ClosePopup;
                signPrompt.AddItem(item);

                Owner.AddMenu(signPrompt);
                return;
            }

            FrameworkCore.PlayCue(sounds.click.activate);

            isSending = true;

            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                message.From = new System.Net.Mail.MailAddress("test@test.com");
                message.To.Add(new System.Net.Mail.MailAddress("bugreport@blendogames.com"));

                message.Subject = "[FLOTILLA] " + subjectText;



                string registered = string.Empty;
                if (FrameworkCore.isTrialMode())
                    registered = "Demo";
                else
                    registered = "Registered";

                string finalString = registered + " version " + FrameworkCore.VERSION + "\n\n" + descriptionText;

                message.Body = finalString;


                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Host = "smtp.gmail.com"; //smtp server                    
                client.Port = 587; //Port for TLS/STARTTLS
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("bugreport@blendogames.com", "");


                client.SendCompleted += new System.Net.Mail.SendCompletedEventHandler(client_SendCompleted);
                client.SendAsync(message, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            isSending = false;

            SysPopup signPrompt = new SysPopup(Owner, Resource.MenuBugReportThanks);
            signPrompt.transitionOnTime = 200;
            signPrompt.transitionOffTime = 200;
            signPrompt.darkenScreen = true;
            signPrompt.hideChildren = false;
            signPrompt.canBeExited = false;
            signPrompt.sideIconRect = sprite.windowIcon.info;

            MenuItem item = new MenuItem(Resource.MenuOK);
            item.Selected += CloseBugPopup;
            signPrompt.AddItem(item);

            Owner.AddMenu(signPrompt);
        }

        private void ClosePopup(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }

        private void CloseBugPopup(object sender, InputArgs e)
        {
            subjectText = "";
            descriptionText = "";

            Helpers.CloseThisMenu(sender);

            Deactivate();
        }

        

        public override void Draw(GameTime gameTime)
        {
            
            base.DrawDarkenScreen();

            Vector2 titlePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 - windowWidth/2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - windowHeight/2);

            Vector2 modifier = new Vector2(Helpers.PopLerp(Transition, -300, 50, 0), 0);
            

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color numberColor = Color.Lerp(Helpers.transColor(Color.Gray), Color.Gray, Transition);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, Resource.MenuBugReport,
                titlePos + modifier, titleColor);

            int titleVec = (int)FrameworkCore.Gothic.MeasureString("S").Y;



            Vector2 subjectPos = textBoxes[0];
            subjectPos.Y -= LINESIZE;
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, "TITLE:",
                subjectPos + modifier, titleColor);

            Rectangle subjectRect = new Rectangle(
                (int)(textBoxes[0].X + modifier.X - 2),
                (int)textBoxes[0].Y,
                windowWidth + 4,
                LINESIZE + 4);



            Color subjectColor = (hoverTextBox == 0 ? new Color(255, 255, 255, 192) : new Color(255, 255, 255, 160));
            subjectColor = Color.Lerp(Helpers.transColor(subjectColor), subjectColor, Transition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, subjectRect,
                sprite.blank, subjectColor);


            //draw the subject text.
            string cursor = "";

            if (selectedTextBox == 0 && blinkTimer > 250)
                cursor = "__";

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, subjectText +cursor,
                textBoxes[0], Color.Black);
            










            Vector2 descPos = textBoxes[1];
            descPos.Y -= LINESIZE;
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, "DESCRIPTION:",
                descPos + modifier, titleColor);


            Rectangle descriptionRect = new Rectangle(
                (int)(textBoxes[1].X + modifier.X - 2),
                (int)textBoxes[1].Y,
                windowWidth + 4,
                LINESIZE * DESCRIPTIONLINES);

            //BOX
            Color descColor = (hoverTextBox == 1 ? new Color(255, 255, 255, 192) : new Color(255, 255, 255, 160));
            descColor = Color.Lerp(Helpers.transColor(descColor), descColor, Transition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, descriptionRect,
                sprite.blank, descColor);



            //draw the description text.
            string descWrapText = descriptionText;

            if (selectedTextBox >= 1 && blinkTimer > 250)
                descWrapText += "__";

            descWrapText = Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                descWrapText, windowWidth, Vector2.Zero, Color.Black);


            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, descWrapText,
                textBoxes[1], Color.Black);



            if (isSending && Transition >= 1)
            {
                Helpers.DarkenScreen(128);
                Vector2 clockPos = Helpers.GetScreenCenter();

                float circleAngle = (float)gameTime.TotalGameTime.TotalSeconds * 0.5f;
                float circleSize = 1.7f + Helpers.Pulse(gameTime, 0.1f, 4);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, clockPos, sprite.bigCrosshair, 
                    new Color(255,255,255,128),
                    circleAngle, Helpers.SpriteCenter(sprite.bigCrosshair), circleSize, SpriteEffects.None, 0);



                
                clockPos.Y += Helpers.Pulse(gameTime, 8, 8);

                float clockAngle = Helpers.Pulse(gameTime, 0.2f, 5);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, clockPos, sprite.clock, Color.White,
                    clockAngle, Helpers.SpriteCenter(sprite.clock), 1, SpriteEffects.None, 0);
            }

            DrawConfirmButtons(gameTime);
        }

        private void DrawConfirmButtons(GameTime gameTime)
        {
            if (isSending)
                return;

            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Vector2 buttonPos = confirmButtons[i];
                buttonPos.Y += (int)Helpers.PopLerp(Transition, 200, -30, 0);

                Rectangle descriptionRect = new Rectangle(
                    (int)(buttonPos.X - 100),
                    (int)(buttonPos.Y - 30),
                    200,
                    60);

                //draw the box.                                
                Color buttonColor = Color.Lerp(Color.Black, Faction.Blue.teamColor, 0.5f);
                Color textColor = Color.White;

                if (hoverConfirmButton == i)
                {
                    buttonColor = Faction.Blue.teamColor;
                    textColor = Color.Orange;
                }

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, descriptionRect,
                    sprite.vistaBox, buttonColor);

                //draw the text.
                string buttText = string.Empty;
                if (i == 0)
                {
                    buttText = Resource.MenuCancel;
                }
                else
                    buttText = Resource.MenuSubmit;

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, buttText,
                    buttonPos, textColor, 1);
            }

            
        }

     }
}


#endif