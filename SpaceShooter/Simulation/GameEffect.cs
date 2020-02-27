#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
//using System;
//using System.Collections.Generic;
//using System.Text;


using Microsoft.Xna.Framework;
#endregion

namespace SpaceShooter
{
    public class GameEffect
    {
        /// <summary>
        /// adjusts move speed. 2.0 = 2.0x
        /// </summary>
        public float speedModifier = 1.0f;
        
        /// <summary>
        /// adjusts rotation speed. 2.0 = 2.0x
        /// </summary>
        public float rotationModifier = 1.0f;
        
        /// <summary>
        /// adjusts fire rate. 2.0 = 2.0x
        /// </summary>
        public float fireRateModifier = 1.0f;

        /// <summary>
        /// determines if ship can fire weapon.
        /// </summary>
        public bool canFire = true;        
        
        /// <summary>
        /// how fast ship repairs itself.  100 is a good default.
        /// </summary>
        public float repairRate = 0.0f;

        /// <summary>
        /// adjusts bullet speed. 2.0 = 2.0x
        /// </summary>
        public float bulletSpeedModifier = 0.0f;


        /// <summary>
        /// Armor modifier. 0.2 = 20% extra armor, 1.0 = block all shots.
        /// </summary>
        public float armorModifierBottom = 0.0f;


        public float armorModifierRear = 0.0f;


        /// <summary>
        /// Invincible vs beams. 1.0 = BLOCKS EVERYTHING, 0.0 = block nothing.
        /// </summary>
        public float BeamArmor = 0;











        public bool canMove = true;
        public bool canRotate = true;

        public Rectangle iconRect = Rectangle.Empty;  //this gameEffect's icon.

        public string name;
        public string description;
    }

    public class GE
    {
        public class FlankSpeed : GameEffect
        {
            public FlankSpeed()
            {
                iconRect = sprite.icons.flankSpeed;
                name = Resource.OrderFlank;
                speedModifier = 2f;
                canFire = false;
                description = Resource.OrderFlankDescription;
            }
        }

        public class FocusFire : GameEffect
        {
            public FocusFire()
            {
                iconRect = sprite.icons.focusFire;
                name = Resource.OrderFocusFire;
                fireRateModifier = 3.0f;
                speedModifier = 0.2f;
                rotationModifier = 0.7f;
                description = Resource.OrderFocusFireDescription;
            }
        }

        public class FieldRepairs : GameEffect
        {
            public FieldRepairs()
            {
                iconRect = sprite.icons.wrench;
                name = Resource.OrderRepair;
                canMove = false;
                canRotate = false;
                repairRate = 150;
                canFire = false;
                description = Resource.OrderRepairDescription;
            }
        }

        //default move order.
        public class DefaultMove : GameEffect
        {
            public DefaultMove()
            {
                iconRect = sprite.icons.move;
                name = Resource.OrderMove;
                description = Resource.OrderMoveDescription;
            }
        }
    }

}