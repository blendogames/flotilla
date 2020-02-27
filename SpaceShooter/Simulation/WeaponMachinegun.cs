
#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class WeaponMachineGun : Weapon
    {
        public WeaponMachineGun()
        {
            boltRange = 12f;
            refireTime = 50;
            muzzleOffset = new Vector2(0, 0);
            muzzleVariation = new Vector2(0.1f, 0.1f);


            //boltModel = "bullet";
            //boltSpeed = 8f;
            //boltSpeedVariance = 3;
            //boltMinDamage = 1;
            //boltMaxDamage = 10;
            prjData = ProjectileTypes.PrjBullet;


            burstAmount = 20;
            burstReloadTime = 4000;

            //emitter = FrameworkCore.Particles.CreateBulletTrailEmitter(Vector3.Zero);

            base.Initialize();
        }
    }
}