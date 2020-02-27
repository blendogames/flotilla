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
    class MissileHitFire : ParticleSystem
    {
        public MissileHitFire(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.explosion;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(5);

            settings.DurationRandomness = 4.0f;

            settings.EmitterVelocitySensitivity = -0.2f;

            settings.MinHorizontalVelocity = -0.2f;
            settings.MaxHorizontalVelocity = 0.2f;

            settings.MinVerticalVelocity = -0.2f;
            settings.MaxVerticalVelocity = 0.2f;

            settings.MinColor = new Color(255, 90, 0, 255);
            settings.MaxColor = new Color(255, 255, 0, 255);

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1.5f;
            settings.MaxStartSize = 2.5f;

            settings.MinEndSize = 4.0f;
            settings.MaxEndSize = 6.0f;

            settings.MinPositionOffset = 0f;
            settings.MaxPositionOffset = 0f;

        }
    }
}