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
    public class asteroidInfo
    {
        public ModelType model;
        public CollisionSphere[] collisionSpheres;

        public asteroidInfo(ModelType modelname, CollisionSphere[] spheres)
        {
            this.model = modelname;
            this.collisionSpheres = spheres;
        }
    }

    public class asteroidTypes
    {
        public static asteroidInfo asteroid1 = new asteroidInfo(
            ModelType.debrisAsteroid01,
            new CollisionSphere[5] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(-1.1f, 1, -0.3f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(-1.1f, -1.2f, -0.3f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(3.4f, -0.8f, 0.5f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3f), new Vector3(-6.2f, 0.5f, -0.4f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3f), new Vector3(-5.5f, -1.1f, -1.9f))
            });

        public static asteroidInfo asteroid2 = new asteroidInfo(
            ModelType.debrisAsteroid02,
            new CollisionSphere[1] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 11f), Vector3.Zero)
            });

        public static asteroidInfo junk1 = new asteroidInfo(
            ModelType.junk1,
            new CollisionSphere[] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,25,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,20,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,15,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,10,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,5,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,0,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,-5,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,-10,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,-15,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,-20,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 5f), new Vector3(0,-25,0)),
            });

        public static asteroidInfo junk2 = new asteroidInfo(
            ModelType.junk2,
            new CollisionSphere[] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-8,-8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-5.3f,-8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-2.6f,-8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(0,-8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(2.6f,-8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(5.3f,-8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(8,-8,0)),


                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-8,     -4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-5.3f,  -4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-2.6f,  -4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(0,      -4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(2.6f,   -4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(5.3f,   -4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(8,      -4,0)),



                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-8,     0,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-5.3f,  0,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-2.6f,  0,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(0,      0,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(2.6f,   0,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(5.3f,   0,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(8,      0,0)),



                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-8,     4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-5.3f,  4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-2.6f,  4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(0,      4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(2.6f,   4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(5.3f,   4,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(8,      4,0)),


                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-8,8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-5.3f,8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-2.6f,8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(0,8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(2.6f,8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(5.3f,8,0)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(8,8,0)),                
            });
    }

    



    public class Hulk : Collideable
    {
        public Vector3 moveDir;
        public int lifeTime;
        public Vector3 angularVelocity;
        public float speed;
        public Vector3 angles;

        public ParticleEmitter emitter;
        public SpaceShip shipOwner; //ship we originated from. Hulks from the same ship do not collide into each other.

        public int debrisTimer;

        public bool permanent = false; //does this item never expire

        public override void Hit(Bolt bolt)
        {
            //this hulk was hit by a bullet.
            FrameworkCore.Particles.CreateMissileHit(bolt.Position, bolt.Velocity);

            //play a sound from... the center of the asteroid? ugh
            FrameworkCore.audiomanager.Play3DCue(sounds.Explosion.asteroid, this.audioEmitter);            
        }

        public override void Hit(Collideable ship, Vector3 intersectPoint)
        {
            //If I am an asteroid, then don't react.
            if (this.modelMesh == ModelType.debrisAsteroid01 || this.modelMesh == ModelType.debrisAsteroid02)
                return;

            //hulk chunks do not collide with other hulk chunks.
            if ((this.modelMesh == ModelType.debrisHulk1 || this.modelMesh == ModelType.debrisHulk2)
                &&
                (ship.modelMesh == ModelType.debrisHulk1 || ship.modelMesh == ModelType.debrisHulk2))
                return;

            
            //if a ship runs into me, don't react.
            if (Helpers.IsSpaceship(ship))
                return;

            //destroy the hulk.
            FrameworkCore.hulkManager.KillHulk(this);
        }

        public override void beamHit(Collideable killer, Vector3 hitPos, int minDmg, int maxDmg, Vector3 muzzleVec)
        {


            //beams evaporate all rubble.
            FrameworkCore.hulkManager.KillHulk(this);
        }
    }

    public class HulkManager
    {
        List<Collideable> hulks;
        public List<Collideable> Hulks
        {
            get { return hulks; }
        }

        
        public HulkManager()
        {
        }

        public void Initialize(List<Collideable> shipList)
        {
            hulks = shipList;
        }

        public void LoadContent()
        {
            
        }

        public void ClearAll()
        {
            hulks.Clear();
        }

        public void KillHulk(Collideable hulk)
        {
            Vector3 randVec = new Vector3(
                Helpers.randFloat(-3,3),
                Helpers.randFloat(-3,3),
                Helpers.randFloat(-3,3));

            if (hulk.modelMesh == ModelType.debrisAsteroid01 ||hulk.modelMesh == ModelType.debrisAsteroid02)
                FrameworkCore.debrisManager.AddHulkDebris(hulk.Position, true);
            else
                FrameworkCore.debrisManager.AddHulkDebris(hulk.Position, false);


            FrameworkCore.Particles.CreateNova(hulk.Position, randVec);

            //Helpers.SetRumbleExplosion();

            FrameworkCore.audiomanager.Play3DCue(sounds.Explosion.tiny, hulk.audioEmitter);


            hulks.Remove(hulk);
            FrameworkCore.playbackSystem.KillItem(hulk);
        }

        public void AddAsteroid(Vector3 position, asteroidInfo asteroidType)
        {
            Hulk item = new Hulk();
            item.modelMesh = asteroidType.model;
            item.Position = position;

            item.Rotation = Quaternion.CreateFromYawPitchRoll(
                Helpers.randFloat(-3, 3),
                Helpers.randFloat(-3, 3),
                Helpers.randFloat(-3, 3));

            item.permanent = true;
            item.BSphere = new BoundingSphere(item.Position, FrameworkCore.ModelArray[(int)item.modelMesh].Meshes[0].BoundingSphere.Radius);

            item.CollisionSpheres = new CollisionSphere[asteroidType.collisionSpheres.Length];

            for (int i = 0; i < asteroidType.collisionSpheres.Length; i++)
            {
                item.CollisionSpheres[i] = new CollisionSphere(asteroidType.collisionSpheres[i].sphere,
                    asteroidType.collisionSpheres[i].offset);
            }
            
            item.UpdateCollisionSpheres();

            
            hulks.Add(item);
        }

        /// <summary>
        /// When a ship gets blown up, spawn chunks.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ShipOwner"></param>
        /// <param name="position"></param>
        /// <param name="MoveDir"></param>
        public void AddHulk(ModelType model, SpaceShip ShipOwner, Vector3 position, Vector3 MoveDir)
        {
            MoveDir.Normalize();

            Hulk item = new Hulk();
            item.modelMesh = model;
            item.Position = position;
            item.moveDir = MoveDir;

            item.shipOwner = ShipOwner;

            Matrix lookAt = Matrix.CreateLookAt(item.Position, item.Position + MoveDir, Vector3.Up);
            item.Rotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));


            float lifeTimeModifier = Helpers.randFloat(1.25f, 1.75f);
            item.lifeTime = (int)(Helpers.MAXROUNDTIME * lifeTimeModifier);
            item.speed = MathHelper.Lerp(8, 12, (float)FrameworkCore.r.NextDouble()); ;
            item.BSphere = new BoundingSphere(item.Position, 2f);

            float x = 0;
            float y = 0;

            if ((float)FrameworkCore.r.NextDouble() > 0.5f)
            {
                x = MathHelper.Lerp(-0.0008f, 0.0008f, (float)FrameworkCore.r.NextDouble());
                y = MathHelper.Lerp(-0.0004f, 0.004f, (float)FrameworkCore.r.NextDouble());
            }
            else
            {
                x = MathHelper.Lerp(-0.0004f, 0.0004f, (float)FrameworkCore.r.NextDouble());
                y = MathHelper.Lerp(-0.0008f, 0.0008f, (float)FrameworkCore.r.NextDouble());
            }            

            item.angularVelocity = new Vector3(x, y, 0);
            item.CollisionSpheres = new CollisionSphere[1] 
            {
                new CollisionSphere(new BoundingSphere(item.Position, 2f), Vector3.Zero)
            };

            item.emitter = FrameworkCore.Particles.CreateHulkEmitter(item.Position);

            hulks.Add(item);

            
            
            FrameworkCore.playbackSystem.AddItem(item, objectType.hulk, ShipOwner.owner.ShipColor);
        }

        public void GenerateShipHulk(SpaceShip ship, ShipData shipData, ModelType mesh1, ModelType mesh2)
        {
            if (shipData.numChunks <= 0)
                return;

            //chunks are evenly distributed to the front and rear of the ship. The
            // front chunks are propelled forward, and rear chunks are propelled backward.

            for (int i = 0; i < shipData.numChunks; i++)
            {
                //first, find  a random spot to spawn the hulk within the ship bounding perimeter.
                Vector2 spawnPos = Vector2.Zero; //X = WIDTH, Y = LENGTH.
                spawnPos.X = MathHelper.Lerp(shipData.shipWidth / -2, shipData.shipWidth / 2,
                    (float)FrameworkCore.r.NextDouble());
                spawnPos.Y = MathHelper.Lerp(0.1f, shipData.shipLength / 2,
                    (float)FrameworkCore.r.NextDouble());

                if (i % 2 == 0)
                    spawnPos.Y *= -1f;

                Matrix orientationMatrix = Matrix.CreateFromQuaternion(ship.Rotation);
                Vector3 spawnPosition = ship.Position +
                    orientationMatrix.Forward * spawnPos.Y +
                    orientationMatrix.Right * spawnPos.X;


                //now, generate the direction the hulk will move.
                Vector3 moveDir = Vector3.Zero;
                if (spawnPos.Y > 0)
                {
                    //chunk spawned in front of ship. propel it forward.
                    moveDir = orientationMatrix.Forward;
                }
                else
                {
                    moveDir = orientationMatrix.Backward;
                }


                //add some random variance to the move direction.
                moveDir += orientationMatrix.Right * MathHelper.Lerp(-0.5f, 0.5f, (float)FrameworkCore.r.NextDouble());
                moveDir += orientationMatrix.Up * MathHelper.Lerp(-0.5f, 0.5f, (float)FrameworkCore.r.NextDouble());

                ModelType mesh = ModelType.debrisHulk1;

                if (i % 2 == 0)
                    mesh = mesh1;
                else
                    mesh = mesh2;

                //spawn the hulk.
                AddHulk(mesh, ship, spawnPosition, moveDir);

                for (int k = 0; k < 16; k++)
                {
                    Vector3 debrisMoveDir = moveDir;

                    debrisMoveDir += orientationMatrix.Right * MathHelper.Lerp(-0.5f, 0.5f, (float)FrameworkCore.r.NextDouble());
                    debrisMoveDir += orientationMatrix.Up * MathHelper.Lerp(-0.5f, 0.5f, (float)FrameworkCore.r.NextDouble());



                    FrameworkCore.debrisManager.AddDebris(getRandomDebris(),
                        spawnPosition, debrisMoveDir);
                }
            }
        }

        private ModelType getRandomDebris()
        {
            int k = FrameworkCore.r.Next(2);
            if (k > 0)
                return ModelType.debrisDebris01;
            else
                return ModelType.debrisDebris02;
        }

        private void GenerateAsteroidCluster(Vector3 position)
        {
            int initalRockAmount = FrameworkCore.r.Next(4,12);
            int fieldSize = FrameworkCore.r.Next(24,56);

            //shotgun the level with tons of asteroids.
            for (int i = 0; i < initalRockAmount; i++)
            {
                Vector3 rockPos = new Vector3(
                    FrameworkCore.r.Next(-fieldSize, fieldSize),
                    FrameworkCore.r.Next(-fieldSize, fieldSize),
                    FrameworkCore.r.Next(-fieldSize, fieldSize));

                rockPos = position + rockPos;

                asteroidInfo mesh = null;

                int randMesh = FrameworkCore.r.Next(2);

                if (randMesh == 0)
                    mesh = asteroidTypes.asteroid1;
                else
                    mesh = asteroidTypes.asteroid2;

                FrameworkCore.hulkManager.AddAsteroid(rockPos, mesh);
            }
        }



        /// <summary>
        /// public function that generates asteroid field.
        /// </summary>
        public void GenerateAsteroidField(int min, int max)
        {
            //asteroids.
            int clusterAmount = FrameworkCore.r.Next(min, max);
            for (int i = 0; i < clusterAmount; i++)
            {
                int clusterRange = 256;
                Vector3 clusterPos = new Vector3(
                        FrameworkCore.r.Next(-clusterRange, clusterRange),
                        FrameworkCore.r.Next(-clusterRange/2, clusterRange/2),
                        FrameworkCore.r.Next(-clusterRange, clusterRange));
                GenerateAsteroidCluster(clusterPos);
            }


            //junk chunks.
            int junkAmount = FrameworkCore.r.Next(4, 12);
            for (int i = 0; i < junkAmount; i++)
            {
                int clusterRange = 256;
                Vector3 clusterPos = new Vector3(
                        FrameworkCore.r.Next(-clusterRange, clusterRange),
                        FrameworkCore.r.Next(-clusterRange / 2, clusterRange / 2),
                        FrameworkCore.r.Next(-clusterRange, clusterRange));

                asteroidInfo mesh = null;

                int randMesh = FrameworkCore.r.Next(2);

                if (randMesh == 0)
                    mesh = asteroidTypes.junk1;
                else
                    mesh = asteroidTypes.junk2;

                FrameworkCore.hulkManager.AddAsteroid(clusterPos, mesh);
            }

            



            //Remove asteroids that  too close to ships.
            List<Collideable> toDelete = new List<Collideable>();
            foreach (Collideable hulk in FrameworkCore.hulkManager.Hulks)
            {
                if (!Helpers.IsHulk(hulk))
                    continue;

                foreach (Collideable ship in FrameworkCore.level.Ships)
                {
                    if (!Helpers.IsSpaceship(ship))
                        continue;

                    if (Vector3.Distance(hulk.Position, ship.Position) < 40)
                    {
                        toDelete.Add(hulk);
                    }
                }
            }

            for (int x = toDelete.Count - 1; x >= 0; x--)
            {
                FrameworkCore.hulkManager.Hulks.Remove(toDelete[x]);
            }

            
            //remove asteroids that are too close to one another.
            for (int x = FrameworkCore.hulkManager.Hulks.Count - 1; x >= 0; x--)
            {
                if (!Helpers.IsHulk(FrameworkCore.hulkManager.Hulks[x]))
                    continue;

                for (int i = FrameworkCore.hulkManager.Hulks.Count - 1; i >= 0; i--)
                {
                    if (!Helpers.IsHulk(FrameworkCore.hulkManager.Hulks[i]))
                        continue;

                    if (x == i)
                        continue;

                    if (Vector3.Distance(FrameworkCore.hulkManager.Hulks[x].Position, FrameworkCore.hulkManager.Hulks[i].Position) < 25)
                    {
                        FrameworkCore.hulkManager.Hulks.RemoveAt(i);
                        break;
                    }
                }
            }

            for (int x = FrameworkCore.hulkManager.Hulks.Count - 1; x >= 0; x--)
            {
                if (!Helpers.IsHulk(FrameworkCore.hulkManager.Hulks[x]))
                    continue;

                FrameworkCore.hulkManager.hulks[x].isStatic = true;
                FrameworkCore.playbackSystem.AddItem(FrameworkCore.hulkManager.Hulks[x],
                    objectType.hulk, Color.White);
            }

            
        }
        

        public void Update(GameTime gameTime)
        {
            //check if we have any hulks in our list.
            if (hulks.Count <= 0)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;            

            for (int i = 0; i < hulks.Count; i++)
            {
                if (!Helpers.IsHulk(hulks[i]))
                    continue;
                

                Hulk item = (Hulk)hulks[i];

                //update position.
                item.Position += (item.speed * item.moveDir) * dt;


                if (item.angularVelocity != Vector3.Zero)
                {
                    float time = item.lifeTime;
                    item.Rotation = Quaternion.CreateFromYawPitchRoll(
                        item.angularVelocity.X * time,
                        item.angularVelocity.Y * time,
                        item.angularVelocity.Z * time);
                }


                if (item.speed > 0)
                {
                    if (item.debrisTimer <= 0)
                    {
                        item.debrisTimer = FrameworkCore.r.Next(200, 500);
                        FrameworkCore.debrisManager.AddHulkTrail(item.Position, item.moveDir);
                    }
                    else
                        item.debrisTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                }



                item.UpdateCollisionSpheres();

                if (item.emitter != null)
                    item.emitter.Update(gameTime, item.Position);

                //update lifetime. Remove from list if it expires.
                if (!item.permanent)
                {
                    item.lifeTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (item.lifeTime <= 0)
                    {
                        KillHulk(item);
                        //hulks.Remove(item);
                        //FrameworkCore.playbackSystem.KillItem(item);
                    }
                }
            }
        }

        



        public virtual void Draw(GameTime gameTime, Camera camera, PlayerCommander curPlayer)
        {
            if (hulks.Count <= 0)
                return;

            for (int i = 0; i < hulks.Count; i++)
            {
                if (!Helpers.IsHulk(hulks[i]))
                    continue;

                Hulk item = (Hulk)hulks[i];


                if (!FrameworkCore.HideHud)
                {
                    if (!FrameworkCore.level.isDemo)
                    {
                        Vector3 gridPos = item.Position;
                        gridPos.Y = curPlayer.GridAltitude;
                        Color lineColor = new Color(96, 96, 96, 160);

                        //The rod line attached to asteroids/rubble/etc.
                        FrameworkCore.lineRenderer.Draw(item.Position, gridPos, lineColor);

                        Helpers.DrawDiamond(gridPos, .5f, lineColor); //BC 3-28-2019 New addition to 2019 build.
                        //Helpers.DrawDiamond(gridPos, item.BSphere.Radius, lineColor);
                        //FrameworkCore.pointRenderer.Draw(gridPos, 6, new Color(0, 0, 0, 128));
                        //FrameworkCore.pointRenderer.Draw(gridPos, 4, lineColor);
                    }
                }
                //FrameworkCore.discRenderer.Draw(1.5f, gridPos, lineColor);



                Matrix worldMatrix = Matrix.CreateFromQuaternion(item.Rotation);
                worldMatrix.Translation = item.Position;

                FrameworkCore.meshRenderer.Draw(item.modelMesh, worldMatrix, camera);

                if (FrameworkCore.debugMode)
                {
                    for (int k = 0; k < item.CollisionSpheres.Length; k++)
                    {
                        BoundingSphere sphere = item.CollisionSpheres[k].sphere;
                        FrameworkCore.sphereRenderer.Draw(sphere, Matrix.Identity, Color.LimeGreen);
                    }

                    
                }
            }
        }
    }
}