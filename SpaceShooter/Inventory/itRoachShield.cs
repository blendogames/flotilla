#region Using
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
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
    /// A carryable item.
    /// </summary>
    public class itRoachShield : InventoryItem
    {
        /// <summary>
        /// Extra armor bonus.
        /// </summary>
        public itRoachShield(float amount)
        {
            GameEffect newEffect = new GameEffect();
            newEffect.armorModifierRear = amount;
            this.gameEffect = newEffect;


            int displayStat = (int)Math.Round(newEffect.armorModifierRear * 100.0f);
            image = sprite.inventory.RoachShield;
            name = iResource.RoachShield;
            description = string.Format(iResource.RoachShieldDescription,
                displayStat);
        }
    }
}