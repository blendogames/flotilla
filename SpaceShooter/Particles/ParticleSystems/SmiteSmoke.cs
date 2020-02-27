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
    class SmiteSmoke : ParticleSystem
    {
        public SmiteSmoke(Game game)
            : base(game)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = ParticleTexture.smoke;

            settings.MaxParticles = 2000;

            settings.Duration = TimeSpan.FromSeconds(12f);
            settings.DurationRandomness = 1f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.EmitterVelocitySensitivity = 9.0f;
            settings.EndVelocity = 0.5f;

            settings.MinColor = new Color(255, 160, 0, 128);
            settings.MaxColor = new Color(255, 255, 0, 128);

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 2;
            settings.MaxStartSize = 4;

            settings.MinEndSize = 27;
            settings.MaxEndSize = 29;

            settings.MinPositionOffset = 0;
            settings.MaxPositionOffset = 0;

            // Use additive blending.
            //settings.SourceBlend = Blend.SourceAlpha;
            //settings.DestinationBlend = Blend.InverseSourceAlpha;
        }
    }
}