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

    public class itBlesoRailChamber : InventoryItem
    {
        /// <summary>
        /// Bullet Speed Module
        /// </summary>
        public itBlesoRailChamber(float bulletSpeed)
        {
            GameEffect newEffect = new GameEffect();
            newEffect.bulletSpeedModifier = bulletSpeed;
            this.gameEffect = newEffect;

            int displayStat = (int)Math.Round(newEffect.bulletSpeedModifier * 100.0f);
            image = sprite.inventory.RailChamber;
            name = iResource.BlesoRailChamber;
            description = string.Format(iResource.BlesoRailChamberDescription,
                displayStat);
        }
    }
}