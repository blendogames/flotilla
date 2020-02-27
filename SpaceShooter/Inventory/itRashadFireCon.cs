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
    public class itRashadFireCon : InventoryItem
    {
        /// <summary>
        /// Fire Rate Module.
        /// </summary>
        public itRashadFireCon(float fireSpeed)
        {
            GameEffect newEffect = new GameEffect();
            newEffect.fireRateModifier = fireSpeed;
            this.gameEffect = newEffect;

            int displayStat = (int)Math.Round(newEffect.fireRateModifier * 100.0f);
            image = sprite.inventory.FireCon;
            name = iResource.RashadFireCon;
            description = string.Format( iResource.RashadFireConDescription,
                displayStat);
        }
    }
}