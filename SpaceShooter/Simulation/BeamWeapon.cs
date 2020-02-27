
#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class BeamWeapon : Weapon
    {
        //default values.



        public override bool Fire(SpaceShip ship, Vector3 targetPos, Vector3 originPos)
        {


        }

        public override void Update(GameTime gameTime, SpaceShip ship)
        {
            Console.WriteLine("firetime: " + curBeamFireTime + "  reload: " + curBeamReloadTime);

            
        }
    }
}