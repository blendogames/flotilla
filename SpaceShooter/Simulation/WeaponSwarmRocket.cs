
#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class WeaponSwarmRocket : Weapon
    {
        public WeaponSwarmRocket()
        {
            boltRange = 25f;
            refireTime = 150; //150
            muzzleOffset = new Vector2(0, 0);
            muzzleVariation = new Vector2(0.2f, 0.2f);
            

            //boltModel = "missile";
            //boltSpeed = 7f;
            //boltSpeedVariance = 0.5f;
            //boltMinDamage = 20;
            //boltMaxDamage = 30;            
            prjData = ProjectileTypes.PrjMissile;


            
            minDeviation = -0.3f;  //-0.5
            maxDeviation = 0.3f; //0.5
            minDeviationTime = 50;
            maxDeviationTime = 150;
            
            

            burstAmount = 5;
            burstReloadTime = 7000; //amount of time between volleys (milliseconds)

            //emitter = FrameworkCore.Particles.CreateRockEmitter(Vector3.Zero);

            base.Initialize();
        }
    }
}