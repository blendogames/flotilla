#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class AudioMenu : SysMenu
    {
        public AudioMenu()
        {
            MenuItem item = new MenuItem("Audio");
            base.AddItem(item);

            item = new MenuItem("Volume");
            base.AddItem(item);
        }
        

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            base.Update(gameTime, inputManager);
        }

        

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {
            base.Activate();
        }

        public override void Draw(GameTime gameTime)
        {

            Vector2 pos = new Vector2(400, 200);
            base.DrawItems(gameTime, pos);
        }
    }
}
