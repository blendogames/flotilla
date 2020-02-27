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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ParticleManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //NOVA is used for ship explosions.
        NovaFire novaFire;
        NovaSmoke novaSmoke;
        NovaSparks novaSparks;
        NovaGlow novaGlow;

        SmiteSmoke smiteSmoke;

        RockTrailSmoke rockTrailSmoke;
        RockTrailSparks rockTrailSparks;
        MissileTrailSmoke missileTrailSmoke;
        MissileTrailSparks missileTrailSparks;

        TorpedoTrailFire torpedoTrailFire;
        TorpedoTrailSmoke torpedoTrailSmoke;

        //MissileDeflect is used when a missile pings off enemy armor.
        MissileDeflectSparks missileDeflectSparks;
        MissileDeflectFire missileDeflectFire;

        //MissileHit is used when a missile penetrates armor.
        MissileHitFire missileHitFire;
        MissileHitSmoke missileHitSmoke;
        MissileHitSparks missileHitSparks;
        MissileHitDebris missileHitDebris;

        PlanetExplosionFire planetExplosionFire;
        PlanetExplosionSmoke planetExplosionSmoke;

        RockExplosionSparks rockExplosionSparks;
        RockExplosionSmoke rockExplosionSmoke;
        RockExplosionFire rockExplosionFire;

        //Muzzle is used when a turret fires a round.
        MuzzleFire muzzleFire;
        MuzzleSmoke muzzleSmoke;

        MissileRingSmoke missileRingSmoke;

        HulkTrailSmoke hulkTrailSmoke;
        HulkTrailDebris hulkTrailDebris;
        HulkTrailFire hulkTrailFire;

        DeflectTrailSmoke deflectTrailSmoke;
        DeflectTrailSparks deflectTrailSparks;

        List<ParticleSystem> particleSystems;

        public void ClearAll()
        {
            for (int i = 0; i < particleSystems.Count; ++i)
                particleSystems[i].Initialize();
        }

        Camera camera;

        public Camera Camera
        {
            get
            {
                return camera;
            }

            set
            {
                camera = value;
                for (int i = 0; i < particleSystems.Count; ++i)
                    particleSystems[i].Camera = camera;
            }
        }

        public ParticleManager(Game game)
            : base(game)
        {
            particleSystems = new List<ParticleSystem>();

            rockTrailSmoke = new RockTrailSmoke(game);
            rockTrailSparks = new RockTrailSparks(game);
            planetExplosionFire = new PlanetExplosionFire(game);
            planetExplosionSmoke = new PlanetExplosionSmoke(game);

            rockExplosionSparks = new RockExplosionSparks(game);
            rockExplosionSmoke = new RockExplosionSmoke(game);
            rockExplosionFire = new RockExplosionFire(game);

            novaFire = new NovaFire(game);
            novaGlow = new NovaGlow(game);
            novaSmoke = new NovaSmoke(game);
            novaSparks = new NovaSparks(game);
            missileHitFire = new MissileHitFire(game);
            missileHitSmoke = new MissileHitSmoke(game);
            missileHitSparks = new MissileHitSparks(game);
            missileHitDebris = new MissileHitDebris(game);

            smiteSmoke = new SmiteSmoke(game);

            missileDeflectSparks = new MissileDeflectSparks(game);
            missileDeflectFire = new MissileDeflectFire(game);

            missileTrailSmoke = new MissileTrailSmoke(game);
            missileTrailSparks= new MissileTrailSparks(game);

            muzzleFire = new MuzzleFire(game);
            muzzleSmoke = new MuzzleSmoke(game);

            missileRingSmoke = new MissileRingSmoke(game);

            hulkTrailSmoke = new HulkTrailSmoke(game);
            hulkTrailDebris = new HulkTrailDebris(game);
            hulkTrailFire = new HulkTrailFire(game);

            deflectTrailSmoke = new DeflectTrailSmoke(game);
            deflectTrailSparks = new DeflectTrailSparks(game);

            torpedoTrailFire = new TorpedoTrailFire(game);
            torpedoTrailSmoke = new TorpedoTrailSmoke(game);

            particleSystems.Add(rockTrailSmoke);
            particleSystems.Add(rockTrailSparks);

            particleSystems.Add(planetExplosionFire);
            particleSystems.Add(planetExplosionSmoke);

            particleSystems.Add(rockExplosionSparks);
            particleSystems.Add(rockExplosionSmoke);
            particleSystems.Add(rockExplosionFire);

            particleSystems.Add(novaFire);
            particleSystems.Add(novaSmoke);
            particleSystems.Add(novaSparks);
            particleSystems.Add(novaGlow);

            particleSystems.Add(smiteSmoke);

            particleSystems.Add(missileTrailSmoke);
            particleSystems.Add(missileTrailSparks);
            particleSystems.Add(missileHitFire);
            particleSystems.Add(missileHitSmoke);
            particleSystems.Add(missileHitSparks);
            particleSystems.Add(missileHitDebris);

            particleSystems.Add(missileDeflectSparks);
            particleSystems.Add(missileDeflectFire);

            particleSystems.Add(muzzleFire);
            particleSystems.Add(muzzleSmoke);

            particleSystems.Add(missileRingSmoke);

            particleSystems.Add(hulkTrailSmoke);
            particleSystems.Add(hulkTrailDebris);
            particleSystems.Add(hulkTrailFire);

            particleSystems.Add(deflectTrailSmoke);
            particleSystems.Add(deflectTrailSparks);

            particleSystems.Add(torpedoTrailFire);
            particleSystems.Add(torpedoTrailSmoke);
        }

        public override void Initialize()
        {
            for (int i = 0; i < particleSystems.Count; ++i)
                particleSystems[i].Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < particleSystems.Count; ++i)
                particleSystems[i].Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < particleSystems.Count; ++i)
                particleSystems[i].Draw(gameTime);

            base.Draw(gameTime);
        }



        public void CreateSmite(Vector3 position, Vector3 velocity)
        {
            velocity.Normalize();
            CreateSmokeRing(position, velocity, smiteSmoke, 128);
        }



        //WHEN A SPACESHIP EXPLODES.
        public void CreateNova(Vector3 position, Vector3 velocity)
        {
            velocity.Normalize();
            CreateSmokeRing(position, velocity, novaSmoke, 128);

            novaGlow.AddParticle(position, Vector3.Zero);

            for (int i = 0; i < 8; i++)
                novaFire.AddParticle(position, Vector3.Zero);

            for (int i = 0; i < 16; i++)
                novaSparks.AddParticle(position, Vector3.Zero);
        }

        public ParticleEmitter CreateRockEmitter(Vector3 position)
        {
            ParticleEmitter emitter = new ParticleEmitter(96.0f, position, missileTrailSmoke , missileTrailSparks);
            return emitter;
        }

        public ParticleEmitter CreateTorpedoEmitter(Vector3 position)
        {
            ParticleEmitter emitter = new ParticleEmitter(128.0f, position, torpedoTrailFire, torpedoTrailSmoke);
            return emitter;
        }

        public ParticleEmitter CreateHulkEmitter(Vector3 position)
        {
            ParticleEmitter emitter = new ParticleEmitter(16.0f, position, hulkTrailSmoke, hulkTrailDebris, hulkTrailFire);
            return emitter;
        }

        public ParticleEmitter CreateDeflectEmitter(Vector3 position)
        {
            ParticleEmitter emitter = new ParticleEmitter(6.0f, position, deflectTrailSmoke, deflectTrailSparks);
            return emitter;
        }

        public ParticleEmitter CreateDamageSmokeEmitter(Vector3 position)
        {
            ParticleEmitter emitter = new ParticleEmitter(16.0f, position, hulkTrailSmoke);
            return emitter;
        }



        public void CreateRockExplosion(Vector3 position, Vector3 velocity)
        {
            velocity.Normalize();

            for (int i = 0; i < 256; i++)
                rockExplosionFire.AddParticle(position, velocity);

            for (int i = 0; i < 128; i++)
                rockExplosionSparks.AddParticle(position, Vector3.Zero);

            for (int i = 0; i < 64; i++)
                rockExplosionSmoke.AddParticle(position, Vector3.Zero);
        }

        public void CreateMissileHit(Vector3 position, Vector3 velocity)
        {
            velocity.Normalize();

            
            for (int i = 0; i < 8; i++)
                missileHitFire.AddParticle(position, velocity);

            for (int i = 0; i < 8; i++)
                missileHitSmoke.AddParticle(position, velocity);

            for (int i = 0; i < 24; i++)
                missileHitSparks.AddParticle(position, velocity);

            for (int i = 0; i < 16; i++)
                missileHitDebris.AddParticle(position, velocity);


            //ring of smoke effect.
            CreateSmokeRing(position, velocity, missileRingSmoke, CIRCLENUMPOINTS);
        }

        private void CreateSmokeRing(Vector3 position,  Vector3 boltVelocity, ParticleSystem particleType, int particleAmount)
        {
            Matrix hitNormal = Matrix.CreateLookAt(position, boltVelocity, Vector3.Up);
            hitNormal = Matrix.Invert(hitNormal);

            float angle = MathHelper.TwoPi / particleAmount;

            for (int i = 0; i < particleAmount; i++)
            {
                Matrix moveDir = RotateMatrix(hitNormal, boltVelocity, (i * angle), false);
                particleType.AddParticle(position, moveDir.Up);
            }
        }

        private const int CIRCLENUMPOINTS = 24;

        public Matrix RotateMatrix(Matrix refMatrix, Vector3 axis, float angle, bool local)
        {
            Matrix rot = Matrix.CreateFromAxisAngle(axis, angle);

            if (local)
            {
                refMatrix = rot * refMatrix;
            }
            else
            {
                refMatrix *= rot;
            }

            return refMatrix;
        }


        public void CreateMuzzleFlash(Vector3 position, Vector3 velocity)
        {
            velocity.Normalize();

            muzzleFire.AddParticle(position, velocity);

            muzzleSmoke.AddParticle(position, velocity);
        }

        public void CreateMissileDeflect(Vector3 position, Vector3 velocity)
        {
            velocity.Normalize();

            for (int i = 0; i < 16; i++)
                missileDeflectSparks.AddParticle(position, velocity);

            missileDeflectFire.AddParticle(position, velocity);
        }

        public void CreatePlanetExplosion(Vector3 position, Vector3 velocity)
        {

            velocity.Normalize();
            
            for (int i = 0; i < 16; i++)
                planetExplosionFire.AddParticle(position, Vector3.Zero);

            velocity *= 3.0f;

            for (int i = 0; i < 8; i++)
                planetExplosionSmoke.AddParticle(position, velocity);
        }
    }
}