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
    class WorldButton : DrawableGameComponent
    {
        Texture2D haloDiffuse;

        SpriteBatch halo;        

        Vector3 rotation;
        Vector3 position;

        public Vector3 Position
        {
            get { return position; }
        }

        Vector2 spriteCenter;

        string text;
        Color textColor;



        worldMenuTypes menuType;

        public worldMenuTypes MenuType
        {
            get { return menuType; }
        }

        
        public float selectTransition;

        public WorldButton(Game game, string txt, Color textColor, worldMenuTypes menuType)
            : base(game)
        {
            this.text = txt;
            this.textColor = textColor;
            this.menuType = menuType;
        }



        public void SetPosition(Vector3 pos)
        {
            this.position = pos;
        }

        public void LoadContent(Game game)
        {
            haloDiffuse = game.Content.Load<Texture2D>(@"textures\glow");

            spriteCenter = new Vector2(haloDiffuse.Width / 2, haloDiffuse.Height / 2);

            halo = new SpriteBatch(game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            rotation.Y = MathHelper.ToRadians(0);
            float currentTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            rotation.X += MathHelper.ToRadians(0.30f) * currentTime;
            selectTransition = 0;
        }

        public bool IsVisible(Camera camera)
        {
            Matrix viewProj = camera.View * camera.Projection;
            Vector4 projResult = Vector4.Transform(position, viewProj);

            float halfScreenY = ((float)GraphicsDevice.Viewport.Height / 2.0f);
            float halfScreenX = ((float)GraphicsDevice.Viewport.Width / 2.0f);

            Vector2 screenPos = new Vector2(((projResult.X / projResult.W) * halfScreenX) + halfScreenX, halfScreenY - ((projResult.Y / projResult.W) * halfScreenY));

            // check if halo is anywhere on screen
            if (((screenPos.X + textRect.Width) < 0.0f) ||
               ((screenPos.X) > (float)GraphicsDevice.Viewport.Width) ||
               ((screenPos.Y + textRect.Height) < 0.0f) ||
               ((screenPos.Y) > (float)GraphicsDevice.Viewport.Height))
                return false;

            return true;
        }

        public bool IsVisible(Vector2 screenPos)
        {
            // check if halo is anywhere on screen
            if (((screenPos.X + spriteCenter.X) < 0.0f) ||
               ((screenPos.X - spriteCenter.X) > (float)GraphicsDevice.Viewport.Width) ||
               ((screenPos.Y + spriteCenter.Y) < 0.0f) ||
               ((screenPos.Y - spriteCenter.Y) > (float)GraphicsDevice.Viewport.Height))
                return false;

            return true;
        }

        Rectangle textRect;

        public Rectangle TextRect
        {
            get { return textRect; }
        }

        public void Draw(GameTime gameTime, Camera camera, float transition)
        {
            // Figure out where on screen to draw halo Effect sprite
            Matrix viewProj = camera.View * camera.Projection;
            Vector4 projResult = Vector4.Transform(position, viewProj);

            float halfScreenY = ((float)GraphicsDevice.Viewport.Height / 2.0f);
            float halfScreenX = ((float)GraphicsDevice.Viewport.Width / 2.0f);

            Vector2 screenPos = new Vector2(((projResult.X / projResult.W) * halfScreenX) + halfScreenX, halfScreenY - ((projResult.Y / projResult.W) * halfScreenY));

            // First check of projResult.W is to determine 
            // if camera is facing the sun or turned away from the sun
            // projResult.W is negative if camera is facing away
            if ((projResult.W > 0.0f) && IsVisible(screenPos))
            {
                Matrix worldMatrix = Matrix.CreateFromYawPitchRoll(rotation.X, 0, 0);
                worldMatrix = worldMatrix * Matrix.CreateFromYawPitchRoll(0, 0, rotation.Y);

                worldMatrix.Translation = position;
                
                halo.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                Vector2 finalPos = Vector2.Lerp(screenPos + new Vector2(-40, 0), screenPos, transition);

                float backgroundAlpha = MathHelper.Lerp(0, 224, transition);
                Color backgroundColor = new Color(0, 0, 0, (byte)backgroundAlpha);

                Vector2 textSize= FrameworkCore.Serif.MeasureString(text);
                Rectangle tempTextRect = new Rectangle((int)finalPos.X, (int)finalPos.Y, (int)textSize.X, (int)textSize.Y);

                float inflateSizeX = MathHelper.Lerp(16, 32, selectTransition);
                float inflateSizeY = MathHelper.Lerp(8, 24, selectTransition);

                tempTextRect.Inflate((int)inflateSizeX, (int)inflateSizeY);
                
                textRect = tempTextRect;

                halo.Draw(FrameworkCore.hudSheet, tempTextRect,
                    sprite.blank, backgroundColor);


                if (selectTransition > 0)
                {
                    Color selectBoxColor = new Color(textColor.R, textColor.G, textColor.B, 192);
                    backgroundColor = Color.Lerp(backgroundColor, selectBoxColor, selectTransition);

                    tempTextRect.Inflate(-3, -3);
                    tempTextRect.Width = (int)MathHelper.Lerp(0, tempTextRect.Width, selectTransition);

                    halo.Draw(FrameworkCore.hudSheet, tempTextRect,
                        sprite.blank, backgroundColor);
                }


                Color transparentColor = new Color(textColor.R, textColor.G, textColor.B, 0);
                Color finalColor = Color.Lerp(transparentColor, textColor, transition);

                if (selectTransition > 0)
                {
                    finalColor = Color.Lerp(finalColor, Color.Black, selectTransition);
                }

                halo.DrawString(FrameworkCore.Serif, text, finalPos, finalColor);

                if (selectTransition > 0)
                {
                    Rectangle spriteRect = sprite.buttons.a;
                    Vector2 imagePos = new Vector2(textRect.X, textRect.Y);
                    imagePos.Y += textRect.Height / 2;
                    imagePos.X -= spriteRect.Width / 2;
                    imagePos.X -= 8;
                    float buttonSize = MathHelper.Lerp(0, 1, selectTransition);
                    halo.Draw(FrameworkCore.hudSheet, imagePos, spriteRect, Color.White,
                        0, Helpers.SpriteCenter(spriteRect), buttonSize, SpriteEffects.None, 0);
                }

                halo.End();
            }
        }
    }
}
