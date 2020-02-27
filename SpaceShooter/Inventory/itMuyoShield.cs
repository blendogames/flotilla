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
    public class itMuyoShield : InventoryItem
    {
        /// <summary>
        /// Extra armor bonus.
        /// </summary>
        public itMuyoShield(float amount)
        {
            GameEffect newEffect = new GameEffect();
            newEffect.armorModifierBottom = amount;
            this.gameEffect = newEffect;

            int displayStat = (int)Math.Round(newEffect.armorModifierBottom * 100.0f);
            image = sprite.inventory.MuyosShield;
            name = iResource.BotosShield;
            description = string.Format(iResource.BotosShieldDescription,
                displayStat);
        }
    }
}