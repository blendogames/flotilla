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
    class MissileHitSparks : ParticleSystem
    {
        public MissileHitSparks(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.basic;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(6);

            settings.DurationRandomness = 2.0f;

            settings.EmitterVelocitySensitivity = -5f;
            settings.EndVelocity = 0.1f;

            settings.MinHorizontalVelocity = -1.5f;
            settings.MaxHorizontalVelocity = 1.5f;

            settings.MinVerticalVelocity = -1.5f;
            settings.MaxVerticalVelocity = 1.5f;

            settings.MinColor = new Color(255, 160, 0, 255);
            settings.MaxColor = new Color(255, 255, 0, 255);

            settings.MinRotateSpeed = 0;
            settings.MaxRotateSpeed = 0;

            settings.MinStartSize = 0.1f;
            settings.MaxStartSize = 0.3f;

            settings.MinEndSize = 0;
            settings.MaxEndSize = 0;

            settings.MinPositionOffset = -0.1f;
            settings.MaxPositionOffset = 0.1f;

        }
    }
}