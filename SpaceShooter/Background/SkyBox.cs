#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion
#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    class SkyBox : DrawableGameComponent
    {
        SphereBox sphereMesh;
        Texture2D skyBoxTexture;

        Effect effect;
        EffectParameter world;
        EffectParameter view;
        EffectParameter projection;
        EffectParameter diffuseTexture;
        EffectParameter brightness;

        public SkyBox(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            sphereMesh = new SphereBox(2000000, 5, 5);
            sphereMesh.TileUVs(4, 2);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            effect = Game.Content.Load<Effect>(@"shaders\skyeffect");
            skyBoxTexture = Game.Content.Load<Texture2D>(@"textures\starfield");

            world = effect.Parameters["World"];
            view = effect.Parameters["View"];
            projection = effect.Parameters["Projection"];

            diffuseTexture = effect.Parameters["DiffuseTexture"];

            brightness = effect.Parameters["brightness"];

            sphereMesh.LoadContent(Game);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="camera"></param>
        /// <param name="howBright">How bright for the skybox to be. Ranges 0.0f - 1.0f</param>
        public void Draw(GameTime gameTime, Camera camera, float howBright)
        {
            if (effect.IsDisposed)
                return;

            try
            {
                Matrix worldMatrix = Matrix.Identity;

                world.SetValue(worldMatrix);
                view.SetValue(camera.View);
                projection.SetValue(camera.Projection);
                diffuseTexture.SetValue(skyBoxTexture);
                brightness.SetValue(howBright);

                effect.Techniques[0].Passes[0].Apply();

                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                if (GraphicsDevice.DepthStencilState.DepthBufferEnable)
                {
                    GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                }
                else
                {
                    GraphicsDevice.DepthStencilState = DepthStencilState.None;
                }

                sphereMesh.Render(GraphicsDevice, camera);

                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                if (GraphicsDevice.DepthStencilState.DepthBufferEnable)
                {
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                }
                else
                {
                    GraphicsDevice.DepthStencilState = Helpers.DepthWrite;
                }
            }
            catch
            {
            }
        }
    }
}