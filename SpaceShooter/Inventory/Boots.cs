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
    public class itBoots : InventoryItem
    {
        public itBoots()
        {
            GameEffect newEffect = new GameEffect();
            newEffect.fireRateModifier = 2.5f;
            this.gameEffect = newEffect;

            
            image = sprite.inventory.Boot;
            name = iResource.Boot;
            description = iResource.BootDescription;
        }
    }
}