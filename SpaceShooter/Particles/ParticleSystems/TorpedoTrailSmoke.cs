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
    class TorpedoTrailSmoke : ParticleSystem
    {
        public TorpedoTrailSmoke(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.smoke;

            settings.MaxParticles = 40000;

            settings.Duration = TimeSpan.FromSeconds(2f);
            settings.DurationRandomness = 0.5f;

            settings.EmitterVelocitySensitivity = 0.05f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(64, 64, 64, 16);
            settings.MaxColor = new Color(100, 150, 100, 16);

            settings.MinRotateSpeed = -5;
            settings.MaxRotateSpeed = 5;

            settings.MinStartSize = 0.3f;
            settings.MaxStartSize = 0.5f;

            settings.MinEndSize = 2f;
            settings.MaxEndSize = 6f;

            settings.MinPositionOffset = 0;
            settings.MaxPositionOffset = 0;
        }
    }
}