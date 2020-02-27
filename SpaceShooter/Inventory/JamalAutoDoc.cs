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
    public class itJamalAutoDoc : InventoryItem
    {
        /// <summary>
        /// Health Regen Module.
        /// </summary>
        public itJamalAutoDoc(float healSpeed)
        {
            GameEffect newEffect = new GameEffect();
            newEffect.repairRate = healSpeed;
            this.gameEffect = newEffect;

            float displayStat = newEffect.repairRate * 0.01f;
            image = sprite.inventory.AutoDoc;
            name = iResource.JamalAutoDoc;
            description = string.Format( iResource.JamalAutoDocDescription,
                displayStat);
        }
    }
}