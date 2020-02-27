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
    class HulkTrailDebris : ParticleSystem
    {
        public HulkTrailDebris(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.debris1;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(8.0f);
            settings.DurationRandomness = 1.0f;

            settings.EmitterVelocitySensitivity = 0.1f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(0, 0, 0, 255);
            settings.MaxColor = new Color(64, 64, 64, 255);

            settings.MinRotateSpeed = -0.5f;
            settings.MaxRotateSpeed = 0.5f;

            settings.MinStartSize = 1.0f;
            settings.MaxStartSize = 1.5f;

            settings.MinEndSize = 0.8f;
            settings.MaxEndSize = 0.8f;

            settings.MinPositionOffset = -1.0f;
            settings.MaxPositionOffset = 1.0f;
        }
    }
}