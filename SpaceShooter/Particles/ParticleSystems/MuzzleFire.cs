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
    class MuzzleFire : ParticleSystem
    {
        public MuzzleFire(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.muzzleflash;

            settings.MaxParticles = 512;

            settings.Duration = TimeSpan.FromSeconds(0.15f);

            settings.DurationRandomness = 0.05f;

            settings.EmitterVelocitySensitivity = 4f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.MinColor = new Color(255, 160, 0, 255);
            settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinRotateSpeed = -8.0f;
            settings.MaxRotateSpeed = 8.0f;

            settings.MinStartSize = 3;
            settings.MaxStartSize = 4;

            settings.MinEndSize = 0.5f;
            settings.MaxEndSize = 1.5f;

            settings.MinPositionOffset = 0;
            settings.MaxPositionOffset = 0;
        }
    }
}