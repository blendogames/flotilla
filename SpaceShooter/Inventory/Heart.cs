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
    public class itHeart : InventoryItem
    {
        /// <summary>
        /// Health Regen Module.
        /// </summary>
        public itHeart()
        {
            GameEffect newEffect = new GameEffect();
            newEffect.BeamArmor = 2.0f;
            newEffect.armorModifierBottom = 2.0f;
            newEffect.armorModifierRear = 2.0f;
            this.gameEffect = newEffect;

            
            image = sprite.inventory.Heart;
            name = iResource.Heart;
            description = iResource.HeartDescription;
        }
    }
}