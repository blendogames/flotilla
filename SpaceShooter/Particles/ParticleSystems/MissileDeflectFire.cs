#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    /// <summary>
    /// Custom particle system for leaving smoke trails behind the rocket projectiles.
    /// </summary>
    class MissileDeflectFire : ParticleSystem
    {
        public MissileDeflectFire(Game game)
            : base(game)
        { }

        //This is the One-Frame "flash" spark that flicks on.
        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.spark;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(0.15);

            settings.DurationRandomness = 0;

            settings.EmitterVelocitySensitivity = 0;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(80, 160, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinRotateSpeed = 0;
            settings.MaxRotateSpeed = 0;

            settings.MinStartSize = 5f;
            settings.MaxStartSize = 5f;

            settings.MinEndSize = 4.0f;
            settings.MaxEndSize = 4.0f;

            settings.MinPositionOffset = 0;
            settings.MaxPositionOffset = 0;
        }
    }
}