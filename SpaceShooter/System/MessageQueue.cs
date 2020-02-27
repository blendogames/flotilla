#region Using
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class Message
    {
        public Rectangle image;
        public string text;
        public Color color;

        public Vector2 position;
        public Vector2 targetPosition;

        public float transition;
        public MessageState state;
        public int timer;
    }

    public enum MessageState
    {
        TransitionOn,
        Active,
        TransitionOff,
    }

    public class MessageQueue
    {
        List<Message> messages;

        int GAPSIZE;
        int LINESIZE;

        public MessageQueue()
        {
            messages = new List<Message>();
        }

        public void Initialize()
        {
            GAPSIZE = sprite.icons.skull.Height + 4;
            LINESIZE = (int)FrameworkCore.Serif.MeasureString("Sample").Y;            
        }

        /// <summary>
        /// Death message.
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="txtString"></param>
        public void AddMessage(string txt, Color txtString)
        {
            Message slot = new Message();

            slot.state = MessageState.TransitionOn;
            slot.transition = 0;

            slot.text = txt;
            slot.color = txtString;
            slot.image = sprite.icons.skull;
            slot.timer = 4000;

            messages.Add(slot);

            UpdateTargetPositions();

            slot.position = slot.targetPosition  + new Vector2(0,GAPSIZE);
        }

        public void AddVeterancyMessage(string txt, Color txtString)
        {
            Message slot = new Message();

            slot.state = MessageState.TransitionOn;
            slot.transition = 0;

            slot.text = txt;
            slot.color = txtString;
            slot.image = sprite.icons.veterancy;
            slot.timer = 4000;

            messages.Add(slot);

            UpdateTargetPositions();

            slot.position = slot.targetPosition + new Vector2(0, GAPSIZE);
        }

        

        private void UpdateTargetPositions()
        {
            for (int x = 0; x < messages.Count; x++)
            {
                messages[x].targetPosition.X = 100;
                messages[x].targetPosition.Y = 100 /*FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2*/
                    + GAPSIZE * messages.IndexOf(messages[x]);
            }
        }

        public void ClearAll()
        {
            this.messages.Clear();
        }

        public void Update(GameTime gameTime)
        {
            if (messages.Count <= 0)
                return;

            List<int> toDelete = new List<int>(messages.Count);

            for (int x = 0; x < messages.Count; x++)
            {
                if (messages[x].state == MessageState.TransitionOn)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(200).TotalMilliseconds);
                    messages[x].transition = MathHelper.Clamp(messages[x].transition + delta, 0, 1);

                    if (messages[x].transition >= 1)
                        messages[x].state = MessageState.Active;
                }
                else if (messages[x].state == MessageState.Active)
                {
                    messages[x].timer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (messages[x].timer <= 0)
                        messages[x].state = MessageState.TransitionOff;
                }
                else
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(200).TotalMilliseconds);
                    messages[x].transition = MathHelper.Clamp(messages[x].transition - delta, 0, 1);

                    if (messages[x].transition <= 0)
                        toDelete.Add(x);
                }

                messages[x].position = Vector2.Lerp(messages[x].position,
                    messages[x].targetPosition, 8.0f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            //delete old messages. make sure to iterate backwards to prevent getting a null reference
            if (toDelete.Count > 0)
            {
                for (int x = toDelete.Count - 1; x >= 0; x--)
                {
                    messages.RemoveAt(toDelete[x]);
                }

                UpdateTargetPositions();
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (FrameworkCore.HideHud)
            {
                return;
            }

            for (int x = 0; x < messages.Count; x++)
            {
                Color textColor = Color.Lerp(OldXNAColor.TransparentWhite,
                    Color.White, messages[x].transition);

                Color iconColor = Color.Lerp(OldXNAColor.TransparentWhite,
                    Color.White, messages[x].transition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, messages[x].position, messages[x].image,
                    iconColor, 0, new Vector2(0, messages[x].image.Height / 2), 1, SpriteEffects.None, 0);

                Vector2 textPos = messages[x].position;
                textPos.X += messages[x].image.Width + 12/*gap between icon and text*/;
                textPos.Y -= 2;

                Vector2 textVec = FrameworkCore.Serif.MeasureString(messages[x].text);
                Rectangle textRect = new Rectangle(
                    (int)textPos.X,
                    (int)(textPos.Y - LINESIZE / 2),
                    (int)textVec.X,
                    LINESIZE);
                textRect.Height += 3;
                textRect.Inflate(4, 0);
                Color rectColor = Color.Lerp(Color.Black, messages[x].color, 0.85f);
                rectColor = Color.Lerp(Helpers.transColor(rectColor),
                    rectColor, messages[x].transition);

                Rectangle backRect = textRect;
                backRect.Inflate(1, 1);

                Color backColor = Color.Lerp(Color.Black, messages[x].color, 0.2f);
                backColor = Color.Lerp(Helpers.transColor(backColor),
                    backColor, messages[x].transition);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, backRect, sprite.blank, backColor);    
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, textRect, sprite.blank, rectColor);    

                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, messages[x].text,
                    textPos, textColor, 0, new Vector2(0,LINESIZE/2), 1, SpriteEffects.None, 0);
            }
        }
    }
}