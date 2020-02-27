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
    public class IngameScoreboard
    {
        int LINESIZE;
        float Transition=0;
        int gapSize;

        public IngameScoreboard()
        {            
        }

        List<int> shipArray;

        public void Initialize()
        {
            LINESIZE = (int)FrameworkCore.Serif.MeasureString("Sample").Y;

            gapSize = LINESIZE + 8;

            shipArray = new List<int>();

            for (int x = 0; x < FrameworkCore.level.Ships.Count; x++)
            {
                //only check spaceships.
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[x]))
                    continue;

                shipArray.Add(x);
            }
        }

        public void Update(GameTime gameTime, bool Activate)
        {
            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(200).TotalMilliseconds);            

            if (Activate)
                Transition = MathHelper.Clamp(Transition + delta, 0, 1);
            else
                Transition = MathHelper.Clamp(Transition - delta, 0, 1);
        }

        

        public void Draw(GameTime gameTime)
        {
            if (Transition <= 0)
                return;

            int windowWidth = 450;

            Vector2 pos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - (windowWidth/2),
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - ((shipArray.Count/2) * gapSize));
            for (int x = 0; x < shipArray.Count; x++)
            {
                if (FrameworkCore.level.Ships[shipArray[x]].IsDestroyed)
                {
                    float skullSize = Helpers.PopLerp(Transition, 0, 1.1f, 0.9f);
                    Vector2 skullPos = pos;
                    skullPos.X -= sprite.icons.skull.Width / 2;
                    skullPos.X -= 2;
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, skullPos, sprite.icons.skull, Color.White,
                        0, Helpers.SpriteCenter(sprite.icons.skull), skullSize, SpriteEffects.None, 0);
                }


                Color backColor = Helpers.transColor(Color.Black, Transition);
                Color shipColor = ((SpaceShip)FrameworkCore.level.Ships[shipArray[x]]).owner.ShipColor;
                shipColor = Helpers.transColor(shipColor, Transition);
                Color healthColor = Color.Lerp(Color.Black, shipColor, 0.3f);
                healthColor = Helpers.transColor(healthColor, Transition);

                float healthPercent = ((SpaceShip)FrameworkCore.level.Ships[shipArray[x]]).Health /
                    ((SpaceShip)FrameworkCore.level.Ships[shipArray[x]]).MaxDamage;                

                Rectangle rect = new Rectangle(
                    (int)pos.X,
                    (int)(pos.Y - LINESIZE / 2),
                    windowWidth,
                    LINESIZE);

                
                Rectangle backRect = rect;
                backRect.Inflate(1, 1);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, backRect, sprite.blank, backColor);

                rect.Width = (int)(rect.Width * healthPercent);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rect, sprite.blank, healthColor);

                Helpers.DrawOutline(FrameworkCore.Serif,
                    ((SpaceShip)FrameworkCore.level.Ships[shipArray[x]]).CaptainName,
                    pos + new Vector2(4,0),
                    shipColor,
                    backColor, 0, new Vector2(0, LINESIZE / 2), 1);

                int healthString = Math.Max(0, (int)((SpaceShip)FrameworkCore.level.Ships[shipArray[x]]).Health);

                float healthStringSize = 0.85f;
                Vector2 healthStringVec = FrameworkCore.Serif.MeasureString(
                    healthString.ToString());
                
                Color healthStringColor = Helpers.transColor(Color.White, Transition);
                Helpers.DrawOutline(FrameworkCore.Serif,
                    healthString.ToString(),
                    pos + new Vector2(windowWidth -4, 0),
                    healthStringColor,
                    backColor, 0, new Vector2(healthStringVec.X, healthStringVec.Y / 2), healthStringSize);


                pos.Y += gapSize;
            }            
        }
    }
}
