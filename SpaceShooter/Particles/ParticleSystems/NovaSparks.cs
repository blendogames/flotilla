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
    class NovaSparks : ParticleSystem
    {
        public NovaSparks(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.basic;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(6);

            settings.DurationRandomness = 3.0f;

            settings.EmitterVelocitySensitivity = 0.01f;

            settings.MinHorizontalVelocity = -6f;
            settings.MaxHorizontalVelocity = 6f;

            settings.MinVerticalVelocity = -6f;
            settings.MaxVerticalVelocity = 6f;

            settings.MinColor = new Color(255, 255, 0, 255);
            settings.MaxColor = new Color(255, 128, 0, 255);

            settings.MinRotateSpeed = 0;
            settings.MaxRotateSpeed = 0;

            settings.MinStartSize = 0.4f;
            settings.MaxStartSize = 0.7f;

            settings.MinEndSize = 0;
            settings.MaxEndSize = 0;

            settings.MinPositionOffset = -1;
            settings.MaxPositionOffset = 1;
        }
    }
}