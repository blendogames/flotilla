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
    class MissileTrailSparks: ParticleSystem
    {
        public MissileTrailSparks(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.smoke;

            settings.MaxParticles = 40000;

            settings.Duration = TimeSpan.FromSeconds(1.5f);
            settings.DurationRandomness = 1.0f;

            settings.EmitterVelocitySensitivity = 0.05f;

            settings.MinHorizontalVelocity = -0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = -0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(255, 70, 0, 255);
            settings.MaxColor = new Color(255, 200, 0, 255);

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 0.3f;
            settings.MaxStartSize = 0.6f;

            settings.MinEndSize = 0.0f;
            settings.MaxEndSize = 0.0f;

            settings.MinPositionOffset = 0;
            settings.MaxPositionOffset = 0;
        }
    }
}