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
    class MissileHitDebris : ParticleSystem
    {
        public MissileHitDebris(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.debris1;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(4);

            settings.DurationRandomness = 2.0f;

            settings.EmitterVelocitySensitivity = -3.0f;

            settings.MinHorizontalVelocity = -0.4f;
            settings.MaxHorizontalVelocity = 0.4f;

            settings.MinVerticalVelocity = -0.4f;
            settings.MaxVerticalVelocity = 0.4f;

            settings.MinColor = new Color(0,0,0);
            settings.MaxColor = new Color(64,64,64);

            settings.MinRotateSpeed = -2.0f;
            settings.MaxRotateSpeed = 2.0f;

            settings.MinStartSize = 1.0f;
            settings.MaxStartSize = 4.0f;

            settings.MinEndSize = 0;
            settings.MaxEndSize = 0;

            settings.MinPositionOffset = -0.1f;
            settings.MaxPositionOffset = 0.1f;
        }
    }
}