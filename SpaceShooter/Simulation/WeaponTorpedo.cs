
#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class WeaponTorpedo: Weapon
    {
        public WeaponTorpedo()
        {
            boltRange = 11f;            
            refireTime = 7000;
            muzzleOffset = Vector2.Zero;
            muzzleVariation = Vector2.Zero;
            
            prjData = ProjectileTypes.PrjTorpedo;

            /*
            minDeviation = -0.05f;
            maxDeviation = 0.05f;
            minDeviationTime = 300;
            maxDeviationTime = 800;
            */


            base.Initialize();
        }
    }
}