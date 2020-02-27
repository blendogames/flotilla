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
    class MissileDeflectSparks : ParticleSystem
    {
        public MissileDeflectSparks(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.basic;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(6);

            settings.DurationRandomness = 3.0f;

            settings.EmitterVelocitySensitivity = -1.0f;

            settings.MinHorizontalVelocity = -1.8f;
            settings.MaxHorizontalVelocity = 1.8f;

            settings.MinVerticalVelocity = -1.8f;
            settings.MaxVerticalVelocity = 1.8f;

            settings.MinColor = new Color(80, 160, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinRotateSpeed = 0;
            settings.MaxRotateSpeed = 0;

            settings.MinStartSize = 0.2f;
            settings.MaxStartSize = 0.4f;

            settings.MinEndSize = 0;
            settings.MaxEndSize = 0;

            settings.MinPositionOffset = -0f;
            settings.MaxPositionOffset = 0f;
        }
    }
}