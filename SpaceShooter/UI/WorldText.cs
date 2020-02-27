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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    class WorldTextItem
    {
        public string text;
        public Color textColor = Color.White;
        public Vector3 position;
        public SpriteFont font = FrameworkCore.Serif;
        public float size = 0.7f;
        public float transition = 0;
        public int lifeTime;
        public float moveSpeed = 2;
        public Vector3 moveDir = Vector3.Up;
        public float outTransition = 1;
        public Color backColor = Color.Red;

        public WorldTextItem()
        {
        }

        public WorldTextItem(string text, Color textColor, Vector3 position, int lifeTime, Vector3 moveDir)
        {
            this.text = text;
            this.textColor = textColor;
            this.position = position;
            this.lifeTime = lifeTime;
            this.moveDir = moveDir;
        }
    }

    public class WorldTextManager : DrawableGameComponent
    {
        //SpriteBatch worldBatch;
        List<WorldTextItem> textItems = new List<WorldTextItem>();

        public WorldTextManager(Game game)
            : base(game)
        {
        }


        public void ClearAll()
        {
            textItems.Clear();
        }


        public void RearArmorHit(Vector3 position, Vector3 moveDir)
        {
            GenericArmorHit(position, moveDir, Resource.MsgRearHit);
        }

        public void BottomArmorHit(Vector3 position, Vector3 moveDir)
        {
            GenericArmorHit(position, moveDir, Resource.MsgBottomHit);
        }

        public void ArmorHit(Vector3 position, Vector3 moveDir)
        {
            GenericArmorHit(position, moveDir, Resource.MsgHit);
        }


        public void SmiteMessage(Vector3 position, Vector3 moveDir, String text)
        {
            moveDir.Normalize();

            WorldTextItem item = new WorldTextItem();
            item.text = text;
            item.textColor = Color.Black;
            item.position = position;
            item.lifeTime = 10000;
            item.moveDir = moveDir;
            item.moveSpeed = 2.0f;
            item.size = 1.1f;

            item.backColor = Color.Gold;
            

            textItems.Add(item);
        }




        public void GenericArmorHit(Vector3 position, Vector3 moveDir, String text)
        {
            moveDir.Normalize();

            WorldTextItem item = new WorldTextItem();
            item.text = text;
            item.textColor = Color.White;
            item.position = position;
            item.lifeTime = 3000;
            item.moveDir = moveDir;
            item.moveSpeed = MathHelper.Lerp(0.5f, 2.0f, (float)FrameworkCore.r.NextDouble());

            textItems.Add(item);
        }

        public void AddItem(string text, Color textColor, Vector3 position, int lifeTime, Vector3 moveDir)
        {
            WorldTextItem item = new WorldTextItem(text, textColor, position, lifeTime, moveDir);
            textItems.Add(item);
        }
        
        public override void Update(GameTime gameTime)
        {
            if (textItems.Count <= 0)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = textItems.Count - 1; i >= 0; i--)
            {
                WorldTextItem textItem = textItems[i];
                textItem.position += (textItem.moveSpeed * textItem.moveDir) * dt;

                textItem.lifeTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (textItem.lifeTime <= 0)
                {
                    //outTransition
                    float delta = Helpers.GetDelta(gameTime, 400);
                    textItem.outTransition = MathHelper.Clamp(textItem.outTransition - delta, 0, 1);                    

                    if (textItem.outTransition <= 0)
                    {
                        textItems.Remove(textItem);
                    }
                }
            }
        }


        public void Draw(GameTime gameTime, Camera camera)
        {
            if (FrameworkCore.HideHud)
            {
                return;
            }

            if (textItems.Count <= 0)
                return;

            //worldBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;


            for (int i = 0; i < textItems.Count; i++)
            {
                WorldTextItem textItem = textItems[i];

                float textSize = textItem.size;

                if (textItem.outTransition < 1)
                {
                    textSize = MathHelper.Lerp(textSize / 2, textSize, textItem.outTransition);
                }


                /*Vector3 screenPos = FrameworkCore.Graphics.GraphicsDevice.Viewport.Project(
                                    textItem.position, camera.Projection, camera.View,
                                    Matrix.Identity);*/

                Vector2 screenPos = Helpers.GetScreenPos(camera, textItem.position);

                Vector2 textCenter = textItem.font.MeasureString(textItem.text);
                Vector2 textOrigin = new Vector2(textCenter.X / 2, textCenter.Y / 2);

                Rectangle textRect = new Rectangle(
                    (int)((screenPos.X - textOrigin.X * textSize)),
                    (int)((screenPos.Y - textOrigin.Y * textSize)),
                    (int)(textCenter.X * textSize),
                    (int)(textCenter.Y * textSize));
                textRect.Height += 4;
                textRect.X -= 4;
                textRect.Width += 8;


                Color boxColor = Color.Lerp(Helpers.transColor(textItem.backColor), textItem.backColor, textItem.outTransition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, textRect, sprite.roundBox, boxColor, 0,
                    Vector2.Zero, SpriteEffects.None, 0);

                Color textColor = Color.Lerp(Helpers.transColor(textItem.textColor), textItem.textColor, textItem.outTransition);
                FrameworkCore.SpriteBatch.DrawString(textItem.font, textItem.text, new Vector2(screenPos.X, screenPos.Y),
                    textColor, 0, textOrigin, textSize, SpriteEffects.None, 0);
            }

            //worldBatch.End();
        }
    }
}
