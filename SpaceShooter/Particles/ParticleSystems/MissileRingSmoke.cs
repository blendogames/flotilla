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
    class MissileRingSmoke : ParticleSystem
    {
        public MissileRingSmoke(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.smoke;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 0.5f;

            settings.EmitterVelocitySensitivity = 4.0f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(48, 48, 48, 128);
            settings.MaxColor = new Color(96, 96, 96, 128);

            settings.MinRotateSpeed = 0;
            settings.MaxRotateSpeed = 0;

            settings.MinStartSize = 2f;
            settings.MaxStartSize = 3f;

            settings.MinEndSize = 0;
            settings.MaxEndSize = 0;

            settings.MinPositionOffset = 0;
            settings.MaxPositionOffset = 0;
        }
    }
}