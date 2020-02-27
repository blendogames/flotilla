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
    class MissileHitSmoke : ParticleSystem
    {
        public MissileHitSmoke(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.smoke;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(5);

            settings.DurationRandomness = 4.0f;

            settings.EmitterVelocitySensitivity = -0.8f;

            settings.MinHorizontalVelocity = -1f;
            settings.MaxHorizontalVelocity = 1f;

            settings.MinVerticalVelocity = -1f;
            settings.MaxVerticalVelocity = 1f;

            settings.MinColor = new Color(0, 0, 0, 48);
            settings.MaxColor = new Color(64, 64, 64, 128);

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1f;
            settings.MaxStartSize = 2f;

            settings.MinEndSize = 5f;
            settings.MaxEndSize = 8f;

            settings.MinPositionOffset = -0.1f;
            settings.MaxPositionOffset = 0.1f;
        }
    }
}