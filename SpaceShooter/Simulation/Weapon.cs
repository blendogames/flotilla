
#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class Weapon
    {
        public enum fireType
        {
            Projectile,
            Beam,
        }

        public fireType firetype = fireType.Projectile;

        public int lastFireTime = 100;

        public Vector2 muzzleOffset = Vector2.Zero;
        public Vector2 muzzleVariation = Vector2.Zero;
        public int refireTime = 200;  
        public float boltRange = 3.0f;

        public ProjectileData prjData;



        public int burstAmount = 0;
        private int curBurstAmount = 0;
        public int burstReloadTime = 0;
        private int curBurstReloadTime = 0;

        

        public int CurBurstReloadTime
        {
            get { return curBurstReloadTime; }
        }

        public float minDeviation = 0;
        public float maxDeviation = 0;
        public int minDeviationTime = 0;
        public int maxDeviationTime = 0;


        public int beamFireTime = 4000;        
        protected int curBeamFireTime = 0;

        /// <summary>
        /// timer for how often the beam can damage a ship.
        /// </summary>
        public int beamDamageTime = 0;
        public int curBeamDamageTime = 0;

        public int beamMinDamage = 20;
        public int beamMaxDamage = 40;

        public int beamRange = 64;

        

        


        public virtual bool beamIsFiring()
        {
            return (curBeamFireTime > 0);
        }


        public virtual void Initialize()
        {
        }

        public virtual bool Fire(SpaceShip ship, Vector3 targetPos, Vector3 originPos)
        {
            if (firetype == fireType.Beam)
                return BeamFire(ship, targetPos, originPos);
            else
                return ProjFire(ship, targetPos, originPos);
        }

        private bool ProjFire(SpaceShip ship, Vector3 targetPos, Vector3 originPos)
        {
            if (lastFireTime <= 0)
            {
                if (burstAmount > 0)
                {
                    curBurstAmount++;

                    if (curBurstReloadTime > 0)
                        return false;

                    if (curBurstAmount > burstAmount)
                    {
                        float fireRateModifier = 1f;
                        if (ship.OrderEffect != null)
                        {
                            if (ship.OrderEffect.fireRateModifier > 0)
                                fireRateModifier = ship.OrderEffect.fireRateModifier;
                        }


                        curBurstReloadTime = (int)(burstReloadTime / fireRateModifier);

                        //refiremod controls time between fire bursts.                   
                        curBurstReloadTime = Helpers.ApplyFireRateModifier(ship, curBurstReloadTime);

                        return false;
                    }
                }



                CreateBolt(ship, targetPos, originPos);
                return true;
            }

            return false;
        }

        private bool BeamFire(SpaceShip ship, Vector3 targetPos, Vector3 originPos)
        {
            if (Vector3.Distance(originPos, targetPos) > beamRange/*beam range*/ + 32/*slop room*/)
            {
                //target is too far away!
                return false;
            }


            if (curBeamFireTime <= 0 && lastFireTime <= 0)
            {
                curBeamFireTime = beamFireTime;

                return true;
            }

            return false;
        }








        public virtual void CreateBolt(SpaceShip ship, Vector3 targetPos, Vector3 originPos)
        {
            Vector3 bulletVector = targetPos - originPos;
            bulletVector.Normalize();

            float finalBoltSpeed = prjData.speed;
            
            if (prjData.speedVariance > 0)
            {
                finalBoltSpeed += MathHelper.Lerp(-prjData.speedVariance, prjData.speedVariance,
                    (float)FrameworkCore.r.NextDouble());
            }

            finalBoltSpeed = Helpers.ApplyBulletSpeedModifier(ship, finalBoltSpeed);

            Vector3 projectileVelocity = bulletVector * finalBoltSpeed;

            Vector2 tempMuzzle = muzzleOffset;
            if (muzzleVariation != Vector2.Zero)
            {
                tempMuzzle.X += MathHelper.Lerp(-muzzleVariation.X, muzzleVariation.X, (float)FrameworkCore.r.NextDouble());
                tempMuzzle.Y += MathHelper.Lerp(-muzzleVariation.Y, muzzleVariation.Y, (float)FrameworkCore.r.NextDouble());
            }


            FrameworkCore.Bolts.FireBolt(prjData, projectileVelocity, boltRange, ship,
                tempMuzzle, minDeviation, maxDeviation, minDeviationTime, maxDeviationTime, 
                originPos, targetPos);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (firetype == fireType.Beam)
                UpdateBeam(gameTime);
            else
                UpdateProjWeap(gameTime);
        }

        public virtual void UpdateProjWeap(GameTime gameTime)
        {
            if (burstAmount > 0 && curBurstReloadTime > 0)
            {
                curBurstReloadTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (curBurstReloadTime <= 0)
                {
                    curBurstAmount = 0;
                }
            }

            lastFireTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public virtual void UpdateBeam(GameTime gameTime)
        {
            //if weapon is firing, tick down its fire time.
            if (curBeamFireTime > 0)
                curBeamFireTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;


            //if weapon is reloading, tick down the reload timer.
            if (lastFireTime > 0 && curBeamFireTime <= 0)
            {
                lastFireTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

        }
    }
}