using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    public class PointRenderer
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PointVertex : IVertexType
        {
            public Vector4 Position;
            public Color Color;
            public float Size;

            public VertexDeclaration VertexDeclaration
            {
                get
                {
                    return new VertexDeclaration(elements);
                }
            }

            private static VertexElement[] elements = new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
                new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(20, VertexElementFormat.Single, VertexElementUsage.PointSize, 0)
            };
        }

        private const int NUMPOINTS = 512;

        private DynamicVertexBuffer buffer;
        private PointVertex[] vertices;
        private int pointCount = 0;

        private Effect pointEffect;
        private EffectParameter wvpParameter;
        private Matrix world, view, projection, worldView, worldViewProj;

        public PointRenderer(Effect shader)
        {
            pointEffect = shader;
            wvpParameter = pointEffect.Parameters["WorldViewProj"];
            vertices = new PointVertex[NUMPOINTS];
            buffer = new DynamicVertexBuffer(
                FrameworkCore.Graphics.GraphicsDevice,
                vertices[0].VertexDeclaration,
                NUMPOINTS,
                BufferUsage.WriteOnly
            );
        }

#if SDL2
        public unsafe void Draw(Vector3 startVec, float Size, Color color)
        {
            if (pointCount >= NUMPOINTS)
            {
                Console.WriteLine("ERROR: TOO MUCH POINT ELEMENTS. INCREASE ARRAY SIZE");
                return;
            }

            fixed (PointVertex* p = &vertices[pointCount])
            {
                p->Position.X = startVec.X;
                p->Position.Y = startVec.Y;
                p->Position.Z = startVec.Z;
                p->Position.W = 1.0f;
                p->Color = color;
                p->Size = Size;
            }

            pointCount += 1;
        }

        public unsafe void EndBatch(Camera camera)
        {
            if (pointCount == 0)
            {
                return;
            }

            GraphicsDevice device = FrameworkCore.Graphics.GraphicsDevice;

            // Update render state...
            BlendStateHelper.BeginApply(device);
            BlendStateHelper.AlphaBlendEnable = true;
            BlendStateHelper.SourceBlend = Blend.SourceAlpha;
            BlendStateHelper.DestinationBlend = Blend.InverseSourceAlpha;
            BlendStateHelper.EndApply(device);
            device.DepthStencilState = DepthStencilState.Default;
            device.RasterizerState = RasterizerState.CullCounterClockwise;

            // Update vertex buffer...
            fixed (PointVertex* ptr = &vertices[0])
            {
                buffer.SetDataPointerEXT(
                    0,
                    (IntPtr) ptr,
                    pointCount * vertices[0].VertexDeclaration.VertexStride,
                    SetDataOptions.Discard
                );
            }
            device.SetVertexBuffer(buffer);

            // Update Effect...
            world = Matrix.Identity;
            view = camera.View;
            projection = camera.Projection;
            Matrix.Multiply(ref world, ref view, out worldView);
            Matrix.Multiply(ref worldView, ref projection, out worldViewProj);
            wvpParameter.SetValue(worldViewProj);
            pointEffect.CurrentTechnique.Passes[0].Apply();

            // Draw points, finally.
            device.DrawPrimitives(PrimitiveType.PointListEXT, 0, pointCount);

            pointCount = 0;
        }
#else
        public void Draw(Vector3 startVec, float Size, Color color)
        {
            // Can't do point rendering in XNA4...
        }
        public void EndBatch(Camera camera)
        {
            // Can't do point rendering in XNA4...
        }
#endif
    }
}
