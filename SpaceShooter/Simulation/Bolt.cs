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
using Microsoft.Xna.Framework.Audio;
#endregion

namespace SpaceShooter
{
    public class BoltManager : DrawableGameComponent
    {
        const int MAXBULLETS = 1024;

        Bolt[] bolts;
        List<int> activeBolts;



        public List<int> ActiveBolts
        {
            get { return activeBolts; }
        }

        public Bolt[] Bolts
        {
            get { return bolts; }
        }

        public void ClearAll()
        {
            for (int i = 0; i < MAXBULLETS; i++)
            {
                bolts[i].isActive = false;
            }

            activeBolts.Clear();
        }

        //ParticleManager particles;

        public BoltManager(Game game, ParticleManager particles)
            : base(game)
        {
            bolts = new Bolt[MAXBULLETS];
            activeBolts = new List<int>(MAXBULLETS);

            //pre allocate all our bolts.
            for (int i = 0; i < MAXBULLETS; i++)
            {
                bolts[i] = new Bolt();
            }
        }

        public void FireBolt(ProjectileData prjData, Vector3 velocity, float duration, SpaceShip ship, Vector2 offset, float minDev, float maxDev, int minDevTime, int maxDevTime, Vector3 originPos, Vector3 targetPos)
        {
            for (int i = 0; i < MAXBULLETS; i++)
            {
                if (!bolts[i].isActive)
                {
                    //brute force search for inactive bolt.
                    InitializeBolt(i, prjData, velocity, duration, ship, offset, minDev, maxDev, minDevTime, maxDevTime, originPos, targetPos);
                    return;
                }
            }
        }

        private void InitializeBolt(int index, ProjectileData prjData, Vector3 velocity, float duration, SpaceShip ship, Vector2 offset, float minDev, float maxDev, int minDevTime, int maxDevTime, Vector3 originPos, Vector3 targetPos)
        {
            bolts[index].Initialize(prjData, velocity, duration, ship, offset, minDev, maxDev, minDevTime, maxDevTime, originPos, targetPos);
            bolts[index].LoadContent(prjData.modelName);
            bolts[index].isActive = true;

            if (ship.owner != null && ship.owner.ShipColor != null)
                FrameworkCore.playbackSystem.AddItem(bolts[index], objectType.bolt, ship.owner.ShipColor);

            activeBolts.Add(index);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < activeBolts.Count; i++)
            {
                if (!bolts[activeBolts[i]].Update(gameTime))
                {
                    FrameworkCore.playbackSystem.KillItem(bolts[activeBolts[i]]);

                    bolts[activeBolts[i]].isActive = false;
                    activeBolts.Remove(activeBolts[i]);
                }
            }
        }

        public void Draw(GameTime gameTime, Camera camera)
        {
            for (int i = 0; i < activeBolts.Count; i++)
            {
                bolts[activeBolts[i]].Draw(camera);
            }
        }
    }













    public class Bolt : Entity
    {
        Vector3 velocity;

        public bool isActive = false;


        float timeRemaining;

        //IAudioEmitter audioEmitter = null;
        //Cue idleCue = null;


        ProjectileData prjData;
        public ProjectileData PrjData
        {
            get { return prjData; }
        }

        SpaceShip owner;

        BoundingSphere bSphere;




        public SpaceShip Owner
        {
            get { return owner; }
        }




        public Vector3 Velocity
        {
            get { return velocity; }
        }

        public BoundingSphere BSphere
        {
            get
            {
                bSphere.Center = Position;
                return bSphere;
            }
        }

        public Bolt()
        {
            //audioEmitter = new IAudioEmitter();
        }

        float minDeviation = 0;
        float maxDeviation = 0;
        int minDeviationTime = 5;
        int maxDeviationTime = 200;
        int deviationTimer = 0;
        ParticleEmitter emitter = null;

        public void Initialize(ProjectileData prjdata, Vector3 velocity, float duration, SpaceShip ship, Vector2 offset, float minDev, float maxDev, int minDevTime, int maxDevTime, Vector3 originPos, Vector3 targetPos)
        {
            this.prjData = prjdata;
            this.velocity = velocity;
            timeRemaining = duration;
            //position = ship.Position;
            Position = originPos;

            Matrix shipOrientation = Matrix.CreateFromQuaternion(ship.Rotation);
            Position += shipOrientation.Right * offset.X;
            Position += shipOrientation.Up * -offset.Y;





            //rotation = ship.Rotation;
            Matrix lookAt = Matrix.CreateLookAt(originPos, targetPos, Vector3.Up);
            Rotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));


            owner = ship;
            minDeviation = minDev;
            maxDeviation = maxDev;
            minDeviationTime = minDevTime;
            maxDeviationTime = maxDevTime;


            if (prjdata.trailType != TrailType.None)
            {
                if (prjdata.trailType == TrailType.RocketTrail)
                    emitter = FrameworkCore.Particles.CreateRockEmitter(this.Position);
                else if (prjdata.trailType == TrailType.TorpedoTrail)
                    emitter = FrameworkCore.Particles.CreateTorpedoEmitter(this.Position);
            }
            else
                emitter = null;


            
            /*
            if (idleCue == null)
            {
                idleCue = FrameworkCore.audiomanager.Play3DCue(sounds.Rocket.rocket3, this.audioEmitter);
            }
            */

            
        }




        public void LoadContent(ModelType model)
        {
            modelMesh = model;

            bSphere = new BoundingSphere(Vector3.Zero, 0.5f);
        }

        public bool Update(GameTime gameTime)
        {
            Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (FrameworkCore.debugMode)
            {
                Position += (velocity * 6) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (minDeviation != 0 || maxDeviation != 0)
            {
                UpdateDeviation(gameTime);
            }

            if (emitter != null)
            {
                emitter.Update(gameTime, Position);
            }

            timeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeRemaining <= 0.0f)
            {
                StopSounds();
                return false;
            }

            //audioEmitter.Position = this.Position;

            return true;
        }

        private void UpdateDeviation(GameTime gameTime)
        {
            if (deviationTimer <= 0)
            {
                deviationTimer = FrameworkCore.r.Next(minDeviationTime, maxDeviationTime);

                float deviationVertical = MathHelper.Lerp(minDeviation, maxDeviation, (float)FrameworkCore.r.NextDouble());
                float deviationHorizontal = MathHelper.Lerp(minDeviation, maxDeviation, (float)FrameworkCore.r.NextDouble());

                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Matrix bulletMatrix = Matrix.CreateFromQuaternion(Rotation);
                Vector3 projectileVelocity = bulletMatrix.Forward;

                float bulletSpeed = velocity.Length();

                projectileVelocity += bulletMatrix.Up * deviationVertical;
                projectileVelocity += bulletMatrix.Right * deviationHorizontal;

                velocity += projectileVelocity;
                velocity.Normalize();

                velocity = velocity * bulletSpeed;

            }
            else
                deviationTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        private void StopSounds()
        {/*
            if (idleCue == null)
                return;

            if (idleCue.IsPlaying)
            {
                idleCue.Stop(AudioStopOptions.AsAuthored);
                idleCue = null;
            }*/
        }

        public void Hit(Vector3 pos)
        {
            StopSounds();
            timeRemaining = 0.0f;
            Position = pos;


        }


        public void Draw(Camera camera)
        {
            Matrix worldMatrix = Matrix.CreateFromQuaternion(Rotation);
            worldMatrix.Translation = Position;

            FrameworkCore.meshRenderer.Draw(modelMesh, worldMatrix, camera);
        }
    }
}


