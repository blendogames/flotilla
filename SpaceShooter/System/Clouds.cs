#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using
using System;
using System.Collections.Generic;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#if XBOX
using Microsoft.Xna.Framework.Net;
#endif
using Microsoft.Xna.Framework.Storage;
#endregion

namespace SpaceShooter
{
    public class Cloud
    {
        public Vector3 position = Vector3.Zero;
        public Vector3 moveDir = Vector3.Zero;

        public Color color=Color.Red;
        public float speed=8;
        public float size = 1;
        public float angle = 0;
        public float turnSpeed = 1;

        public float lifeTransition=0;

        public int maxLifetime = 2000;
    }

    public class CloudManager
    {
        Cloud[] clouds;

        const int NUMBEROFCLOUDS = 64;

        public CloudManager()
        {
            clouds = new Cloud[NUMBEROFCLOUDS];

            for (int i = 0; i < NUMBEROFCLOUDS; i++)
            {
                clouds[i] = new Cloud();
                InitializeCloud(clouds[i]);
            }       
        }

        private void InitializeCloud(Cloud cloud)
        {
            cloud.lifeTransition = 0;

            Location randomPlanet = FrameworkCore.worldMap.Locations[FrameworkCore.r.Next(FrameworkCore.worldMap.Locations.Count)];

            cloud.position = randomPlanet.position;


            cloud.position.Y += Helpers.randFloat(-16, 16);
            cloud.position.X += Helpers.randFloat(-16, 16);



            cloud.maxLifetime = FrameworkCore.r.Next(2000, 6000);

            cloud.moveDir = new Vector3(1 + Helpers.randFloat(-0.4f,0.4f),
                0.5f + Helpers.randFloat(-0.2f, 0.2f),
                0);

            cloud.turnSpeed = Helpers.randFloat(-0.5f, 0.5f);

            cloud.moveDir.Normalize();

            cloud.speed = Helpers.randFloat(1, 3);
            cloud.size = Helpers.randFloat(0.5f, 1.5f);

            if (!randomPlanet.isVisible)
            {
                //unexplored, not visible
                cloud.color = new Color(0, 0, 0, 96);
            }
            else if (!randomPlanet.isExplored)
            {
                //explorable planet.
                cloud.color = new Color(255, 128, 0, (byte)FrameworkCore.r.Next(24,48));
            }
            else
            {
                //explored.
                cloud.color = new Color(255, 128, 0, 16);
            }

        }

        public void Update(GameTime gameTime)
        {

            for (int i = 0; i < NUMBEROFCLOUDS; i++)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(clouds[i].maxLifetime).TotalMilliseconds);
                clouds[i].lifeTransition = MathHelper.Clamp(clouds[i].lifeTransition + delta, 0, 1);

                if (clouds[i].lifeTransition < 1)
                {
                    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    //move the cloud.
                    clouds[i].position += clouds[i].moveDir * (clouds[i].speed * dt);
                    clouds[i].angle += clouds[i].turnSpeed * dt;
                }
                else
                {
                    //reset and reinitialize this cloud.
                    InitializeCloud(clouds[i]);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < NUMBEROFCLOUDS; i++)
            {
                if (clouds[i].lifeTransition <= 0)
                    continue;

                Vector2 drawPos = Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera,
                    clouds[i].position);


                Color cloudColor = clouds[i].color;


                if (clouds[i].lifeTransition < 0.2f)
                {
                    float adjustedTime = clouds[i].lifeTransition * 5f;
                    cloudColor = Color.Lerp(Helpers.transColor(cloudColor), cloudColor, adjustedTime);
                }
                else if (clouds[i].lifeTransition > 0.8f)
                {
                    float adjustedTime = (clouds[i].lifeTransition- 0.8f) * 5f;
                    cloudColor = Color.Lerp(cloudColor, Helpers.transColor(cloudColor), adjustedTime);
                }


                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, drawPos, sprite.cloud, cloudColor,
                    clouds[i].angle, Helpers.SpriteCenter(sprite.cloud), clouds[i].size, SpriteEffects.None, 0);
            }
        }
    }    
}