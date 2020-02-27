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
    class DeflectTrailSmoke : ParticleSystem
    {
        public DeflectTrailSmoke(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.arc1;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(0.2f);
            settings.DurationRandomness = 0;

            settings.EmitterVelocitySensitivity = 0.3f;

            settings.MinHorizontalVelocity = -8;
            settings.MaxHorizontalVelocity = 8;

            settings.MinVerticalVelocity = -8;
            settings.MaxVerticalVelocity = 8;

            settings.MinColor = new Color(110, 170, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinRotateSpeed = -12f;
            settings.MaxRotateSpeed = 12f;

            settings.MinStartSize = 0.5f;
            settings.MaxStartSize = 1f;

            settings.MinEndSize = 2f;
            settings.MaxEndSize = 2f;

            settings.MinPositionOffset = -0.3f;
            settings.MaxPositionOffset = 0.3f;
        }
    }
}