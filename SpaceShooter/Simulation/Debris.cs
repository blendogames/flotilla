#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class DebrisItem
    {
        public Vector3 position;     //worldPosition
        public Quaternion rotation;  //rotation
        public ModelType debrisModel;     //mesh.
        public Color debrisColor = Color.White; //diffuse color.
        public Vector3 moveDir;       //what direction debris moves.
        public int lifeTime = 2000;   //how much millisecond this debris exists.
        public float moveSpeed = 2;  //how fast debris moves.

        public Vector3 angles = Vector3.Zero;   //yaw pitch roll
        public Vector3 angularVelocity = Vector3.Zero;  //rotation speeds.

        public ParticleEmitter emitter;

        public bool isActive = false;
    }

    public class DebrisManager
    {
        const int MAXDEBRISITEMS = 256;
        DebrisItem[] debrisItems;

        public DebrisManager()
        {
            debrisItems = new DebrisItem[MAXDEBRISITEMS];
            for (int i = 0; i < debrisItems.Length; i++)
            {
                debrisItems[i] = new DebrisItem();
            }
        }

        private bool GetFreeIndex(out int index)
        {
            index = -1;
            for (int i = 0; i < debrisItems.Length; i++)
            {
                if (!debrisItems[i].isActive)
                {
                    debrisItems[i].emitter = null;

                    index = i;
                    return true;
                }
            }

            return false;
        }

        //when a rocket bounces off an armored ship.
        public void AddRocketDeflect(ModelType model, Vector3 position, Vector3 moveDir)
        {
            int index = -1;
            if (!GetFreeIndex(out index))
                return;            

            DebrisItem item = debrisItems[index];
            item.debrisModel = model;
            moveDir.Normalize();
            item.moveDir = moveDir;
            item.position = position;
            item.lifeTime = 5000;
            item.debrisColor = new Color(192, 192, 192);
            item.moveSpeed = MathHelper.Lerp(4, 6, (float)FrameworkCore.r.NextDouble());
            item.angularVelocity = new Vector3(0, MathHelper.Lerp(-3,3,(float)FrameworkCore.r.NextDouble()), 0);
            item.emitter = FrameworkCore.Particles.CreateDeflectEmitter(item.position);

            item.isActive = true;
        }

        public void ClearAll()
        {
            for (int i = 0; i < debrisItems.Length; i++)
            {
                debrisItems[i].emitter = null;
                debrisItems[i].isActive = false;
            }
        }

        private ModelType getRandomDebris()
        {
            if (FrameworkCore.r.NextDouble() > 0.5f)
                return ModelType.debrisDebris01;
            else
                return ModelType.debrisDebris02;
            
        }

        //hulk trail.
        public void AddHulkTrail(Vector3 position, Vector3 moveDir)
        {
            int amount = FrameworkCore.r.Next(2, 6);

            for (int i = 0; i < amount; i++)
            {
                int index = -1;
                if (!GetFreeIndex(out index))
                    return;

                DebrisItem item = debrisItems[index];
                item.debrisModel = getRandomDebris();
                moveDir.Normalize();
                item.moveDir = moveDir;
                item.position = position;

                item.position.X += Helpers.randFloat(-1.5f, 1.5f);
                item.position.Y += Helpers.randFloat(-1.5f, 1.5f);

                item.lifeTime = FrameworkCore.r.Next(1500, 3000);

                item.moveSpeed = Helpers.randFloat(0.5f, 1.5f);
                item.angularVelocity = new Vector3(Helpers.randFloat(-2, 2), Helpers.randFloat(-4, 4), 0);

                item.isActive = true;
            }
        }

        //missile hit debris.
        public void AddMissleHitDebris(Vector3 position, Vector3 moveDir)
        {
            int amount = FrameworkCore.r.Next(3, 6);

            for (int i = 0; i < amount; i++)
            {
                int index = -1;
                if (!GetFreeIndex(out index))
                    return;

                DebrisItem item = debrisItems[index];
                item.debrisModel = getRandomDebris();
                moveDir.Normalize();
                item.moveDir = moveDir;
                item.position = position;
                item.lifeTime = FrameworkCore.r.Next(1000, 3000);

                item.moveSpeed = Helpers.randFloat(4, 8);
                item.angularVelocity = new Vector3(Helpers.randFloat(-1, 1), Helpers.randFloat(-3, 3), 0);

                item.isActive = true;
            }
        }

        /// <summary>
        /// debris that shoots out when hulk is destroyed. shoots out in random directions.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="moveDir"></param>
        public void AddHulkDebris(Vector3 position, bool isAsteroid)
        {
            int amount = FrameworkCore.r.Next(16, 24);

            for (int i = 0; i < amount; i++)
            {
                int index = -1;
                if (!GetFreeIndex(out index))
                    return;

                DebrisItem item = debrisItems[index];

                if (!isAsteroid)
                    item.debrisModel = getRandomDebris();
                else
                    item.debrisModel = ModelType.asteroidchunk;


                //give position a random offset.
                Vector3 itemPosition = position +
                    new Vector3(
                    Helpers.randFloat(-5, 5),
                    Helpers.randFloat(-5, 5),
                    Helpers.randFloat(-5, 5));

                item.position = itemPosition;
                item.lifeTime = FrameworkCore.r.Next(500, 2000);


                Vector3 moveDir = item.position - position;
                moveDir.Normalize();


                item.moveDir = moveDir;

                item.moveSpeed = Helpers.randFloat(12, 48);
                item.angularVelocity = new Vector3(Helpers.randFloat(-3, 3), Helpers.randFloat(-6, 6), 0);

                item.isActive = true;
            }
        }






        //destruction debris
        public void AddDebris(ModelType model, Vector3 position, Vector3 moveDir)
        {
            int index = -1;
            if (!GetFreeIndex(out index))
                return;

            DebrisItem item = debrisItems[index];
            item.debrisModel = model;
            moveDir.Normalize();
            item.moveDir = moveDir;
            item.position = position;
            item.lifeTime = FrameworkCore.r.Next(5000,10000);

            item.moveSpeed = Helpers.randFloat(8,16);
            item.angularVelocity = new Vector3(Helpers.randFloat(-1, 1), Helpers.randFloat(-3, 3), 0);

            item.isActive = true;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;            

            for (int i = 0; i < debrisItems.Length; i++)
            {
                if (!debrisItems[i].isActive)
                    continue;

                DebrisItem item = debrisItems[i];
                item.position += (item.moveSpeed * item.moveDir) * dt;

                if (Math.Abs(item.angularVelocity.X) > 0)
                    item.angles.X += item.angularVelocity.X * dt;

                if (Math.Abs(item.angularVelocity.Y) > 0)
                    item.angles.Y += item.angularVelocity.Y * dt;

                if (Math.Abs(item.angularVelocity.Z) > 0) 
                    item.angles.Z += item.angularVelocity.Z * dt;

                item.rotation = Quaternion.CreateFromYawPitchRoll(item.angles.X, item.angles.Y, item.angles.Z);


                item.lifeTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (item.emitter != null)
                    item.emitter.Update(gameTime, item.position);

                
                if (item.lifeTime <= 0)
                {
                    item.isActive = false;
                }
            }



            
        }


        public virtual void Draw(GameTime gameTime, Camera camera)
        {
            for (int i = 0; i < debrisItems.Length; i++)
            {
                if (!debrisItems[i].isActive)
                    continue;

                Matrix worldMatrix = Matrix.CreateFromQuaternion(debrisItems[i].rotation);
                worldMatrix.Translation = debrisItems[i].position;

                FrameworkCore.meshRenderer.Draw(debrisItems[i].debrisModel, worldMatrix, camera, debrisItems[i].debrisColor);
            }
        }
    }
}