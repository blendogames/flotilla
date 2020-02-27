#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion
#region Using
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPointSprite : IVertexType
    {
        public Vector3 Position;
        public float Size;
        public float Opacity;
        public float Rotation;

        public static int SizeInBytes = 24;

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return new VertexDeclaration(VertexElements);
            }
        }

        public VertexPointSprite(Vector3 position, float size)
        {
            this.Position = position;
            this.Size = size;
            this.Opacity = 1.0f;
            this.Rotation = 0.0f;
        }

        public static VertexElement[] VertexElements =
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Single, VertexElementUsage.PointSize, 0),
            new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.Color, 0),
            new VertexElement(20, VertexElementFormat.Single, VertexElementUsage.Color, 1),
        };
    }

    class MotionField : DrawableGameComponent
    {
        VertexDeclaration vertexDeclaration;
        VertexBuffer vertexBuffer;
        Texture2D starTexture;
        Effect starEffect;
        int starCount = 256; //512

        VertexPointSprite[] data;

        Random random = new Random();

        bool motionReady;

        public MotionField(Game game)
            : base(game)
        {
            data = new VertexPointSprite[starCount];
        }

        void GenerateStars(Camera camera)
        {
            Matrix xform = Matrix.CreateFromQuaternion(camera.CameraRotation);
            Vector3 forward = xform.Forward;

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = new VertexPointSprite();
                Vector3 direction = new Vector3(-1.0f + 2.0f * (float)random.NextDouble(),
                                                -1.0f + 2.0f * (float)random.NextDouble(),
                                                -1.0f + 2.0f * (float)random.NextDouble());
                direction.Normalize();

                float distance = 32.0f + 204.80f * (float)random.NextDouble();
                data[i].Position = (forward * 260.0f) + direction * distance;
                data[i].Size = 1.5f;
                data[i].Opacity = 0.5f;
                data[i].Rotation = -1.0f + 2.0f * (float)random.NextDouble();
            }

            motionReady = true;
        }

        void MoveStars(Camera camera)
        {
            Matrix xform = Matrix.CreateFromQuaternion(camera.CameraRotation);
            Vector3 forward = xform.Forward;

            for (int i = 0; i < data.Length; ++i)
            {
                Vector3 pointNormal = data[i].Position - camera.CameraPosition;
                float particleDistance = pointNormal.Length();
                pointNormal.Normalize();

                // Is this particle still in front of the camera
                if (Vector3.Dot(pointNormal, forward) < 0.0f)
                {
                    // No, so we need to throw this point back out in front of camera.
                    Vector3 direction = new Vector3(-1.0f + 2.0f * (float)random.NextDouble(),
                                                    -1.0f + 2.0f * (float)random.NextDouble(),
                                                    -1.0f + 2.0f * (float)random.NextDouble());
                    direction.Normalize();

                    float randomDist = 32.0f + 204.80f * (float)random.NextDouble();
                    data[i].Position = (camera.CameraPosition + (forward * 260.0f)) + (direction * randomDist);
                }
                else
                {
                    if (particleDistance > 400.0f)
                    {
                        // Particle is too far in front, throw it behind us
                        // ship is moving backward
                        Vector3 direction = new Vector3(-1.0f + 2.0f * (float)random.NextDouble(),
                                                        -1.0f + 2.0f * (float)random.NextDouble(),
                                                        -1.0f + 2.0f * (float)random.NextDouble());
                        direction.Normalize();

                        float randomDist = 32.0f + 32.0f * (float)random.NextDouble();
                        data[i].Position = (camera.CameraPosition + (forward * -40.0f)) + (direction * randomDist);
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            vertexDeclaration = new VertexDeclaration(VertexPointSprite.VertexElements);

            starTexture = Game.Content.Load<Texture2D>(@"particles\startexture");
            starEffect = Game.Content.Load<Effect>(@"shaders\motioneffect");

            vertexBuffer = new VertexBuffer(GraphicsDevice, vertexDeclaration, data.Length * VertexPointSprite.SizeInBytes, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPointSprite>(data);

            base.LoadContent();
        }

        public void Draw(GameTime gameTime, Camera camera)
        {
            try
            {
                // First, we need to figure out which stars
                // are behind us and throw them out in front of us again
                // based on how fast the camera is moving
                if (!motionReady)
                {
                    GenerateStars(camera);
                    vertexBuffer.SetData<VertexPointSprite>(data);
                }
                else
                {
                    MoveStars(camera);
                    vertexBuffer.SetData<VertexPointSprite>(data);
                }

                {
                    //
                    // STARS
                    //
                    starEffect.Parameters["World"].SetValue(Matrix.Identity);
                    starEffect.Parameters["View"].SetValue(camera.View);
                    starEffect.Parameters["Projection"].SetValue(camera.Projection);
                    starEffect.Parameters["Texture"].SetValue(starTexture);
                    starEffect.Parameters["ViewportHeight"].SetValue(GraphicsDevice.Viewport.Height);

                    GraphicsDevice.SetVertexBuffer(vertexBuffer);
                    PointSpriteHelper.Enable();

                    // Set the alpha blend mode.
                    BlendStateHelper.BeginApply(GraphicsDevice);
                    BlendStateHelper.AlphaBlendEnable = true;
                    BlendStateHelper.SourceBlend = Blend.SourceAlpha;
                    BlendStateHelper.DestinationBlend = Blend.InverseSourceAlpha;
                    BlendStateHelper.EndApply(GraphicsDevice);

                    // Enable the depth buffer (so particles will not be visible through
                    // solid objects like the space ship), but disable depth writes
                    // (so particles will not obscure other particles).
                    GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

                    for (int i = 0; i < starEffect.CurrentTechnique.Passes.Count; ++i)
                    {
                        starEffect.CurrentTechnique.Passes[i].Apply();

#if SDL2
                        GraphicsDevice.DrawPrimitives(PrimitiveType.PointListEXT, 0, starCount);
#endif
                    }

                    PointSpriteHelper.Disable();

                    GraphicsDevice.SetVertexBuffer(null);

                    BlendStateHelper.BeginApply(GraphicsDevice);
                    BlendStateHelper.AlphaBlendEnable = false;
                    BlendStateHelper.EndApply(GraphicsDevice);
                    if (GraphicsDevice.DepthStencilState.DepthBufferEnable)
                    {
                        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    }
                    else
                    {
                        GraphicsDevice.DepthStencilState = Helpers.DepthWrite;
                    }
                }

                base.Draw(gameTime);
            }
            catch
            {
            }
        }
    }
}