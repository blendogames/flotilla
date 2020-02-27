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
    class DeflectTrailSparks : ParticleSystem
    {
        public DeflectTrailSparks(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.sparkGroup;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(4.0f);
            settings.DurationRandomness = 1.0f;

            settings.EmitterVelocitySensitivity = 0.1f;

            settings.MinHorizontalVelocity = -0.7f;
            settings.MaxHorizontalVelocity = 0.7f;

            settings.MinVerticalVelocity = -0.7f;
            settings.MaxVerticalVelocity = 0.7f;

            settings.MinColor = new Color(80, 160, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 0.7f;
            settings.MaxStartSize = 1.3f;

            settings.MinEndSize = 0;
            settings.MaxEndSize = 0;

            settings.MinPositionOffset = -0.2f;
            settings.MaxPositionOffset = 0.2f;
        }
    }
}