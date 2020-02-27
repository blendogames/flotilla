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
    class HulkTrailSmoke : ParticleSystem
    {
        public HulkTrailSmoke(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.smoke;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(7.0f);
            settings.DurationRandomness = 2.0f;

            settings.EmitterVelocitySensitivity = 0.1f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(16, 16, 16, 64);
            settings.MaxColor = new Color(64, 64, 64, 128);

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 3f;
            settings.MaxStartSize = 3f;

            settings.MinEndSize = 4.0f;
            settings.MaxEndSize = 8.0f;

            settings.MinPositionOffset = -1f;
            settings.MaxPositionOffset = 1f;
        }
    }
}