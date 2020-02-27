
#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class BeamGun : Weapon
    {
        public BeamGun()
        {
            firetype = fireType.Beam;

            beamFireTime = 1500;
            refireTime = 2000;

            beamMinDamage = 40;
            beamMaxDamage = 70;

            beamDamageTime = 300;
            beamRange = 64;

            base.Initialize();
        }
    }
}