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
    /// Custom particle system for creating the fiery part of the explosions.
    /// </summary>
    class NovaFire : ParticleSystem
    {
        public NovaFire(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.explosion;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(4);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = -3;
            settings.MaxHorizontalVelocity = 3;

            settings.MinVerticalVelocity = -3;
            settings.MaxVerticalVelocity = 3;

            settings.EndVelocity = -0.5f;

            settings.MinColor = new Color(128, 64, 64);
            settings.MaxColor = new Color(255, 128, 128);

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 4;
            settings.MaxStartSize = 8;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 16;

            settings.MinPositionOffset = -1f;
            settings.MaxPositionOffset = 1f;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
    }
}