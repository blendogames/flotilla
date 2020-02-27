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
    class MuzzleSmoke : ParticleSystem
    {
        public MuzzleSmoke(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.smoke;

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.DurationRandomness = 0.5f;

            settings.EmitterVelocitySensitivity = 3f;

            settings.MinHorizontalVelocity = -1f;
            settings.MaxHorizontalVelocity = 1f;

            settings.MinVerticalVelocity = -1f;
            settings.MaxVerticalVelocity = 1f;

            settings.MinColor = new Color(255, 255, 255, 16);
            settings.MaxColor = new Color(255, 255, 255, 32);

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1f;
            settings.MaxStartSize = 2f;

            settings.MinEndSize = 3f;
            settings.MaxEndSize = 5f;

            settings.MinPositionOffset = 0;
            settings.MaxPositionOffset = 0;
        }
    }
}