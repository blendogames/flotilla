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
    class NovaGlow : ParticleSystem
    {
        public NovaGlow(Game game)
            : base(game)
        { }

        //This is the One-Frame "flash" spark that flicks on.
        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.bigglow;

            settings.MaxParticles = 64;

            settings.Duration = TimeSpan.FromSeconds(5);

            settings.DurationRandomness = 2.0f;

            settings.EmitterVelocitySensitivity = 0;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(255, 160, 0, 80);
            settings.MaxColor = new Color(255, 160, 0, 80);

            settings.MinRotateSpeed = 0;
            settings.MaxRotateSpeed = 0;

            settings.MinStartSize = 1f;
            settings.MaxStartSize = 1f;

            settings.MinEndSize = 100.0f;
            settings.MaxEndSize = 100.0f;

            settings.MinPositionOffset = 0;
            settings.MaxPositionOffset = 0;
        }
    }
}