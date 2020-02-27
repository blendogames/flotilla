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
    public class itKeshiaEngine : InventoryItem
    {
        /// <summary>
        /// Move Speed Module
        /// </summary>
        public itKeshiaEngine(float engineSpeed)
        {
            GameEffect newEffect = new GameEffect();
            newEffect.speedModifier = engineSpeed;
            this.gameEffect = newEffect;

            int displayStat = (int)Math.Round(newEffect.speedModifier * 100.0f);
            image = sprite.inventory.Engine;
            name = iResource.KeshiaEngine;
            description = string.Format( iResource.KeshiaEngineDescription, displayStat);
        }
    }
}