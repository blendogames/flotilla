using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    public class DiscItem    
    {
        public Vector3 position;
        public float size;
        public Color discColor;
        public Matrix orientation = Matrix.Identity;
    }

    public class DiscRenderer
    {
        public static float RADIANS_FOR_90DEGREES = MathHelper.ToRadians(90);//(float)(Math.PI / 2.0);
        public static float RADIANS_FOR_180DEGREES = RADIANS_FOR_90DEGREES * 2;

        private SpaceShooterGame _gameInstance = null;

        protected VertexBuffer buffer;
        protected VertexDeclaration vertexDecl;

        private BasicEffect basicEffect;

        private const int CIRCLE_NUM_POINTS = 24;
        private IndexBuffer _indexBuffer;
        private VertexPositionNormalTexture[] _vertices;

        int freeIndex;
        DiscItem[] discItems;

        public DiscRenderer(SpaceShooterGame game)
        {
            _gameInstance = game;

            freeIndex = 0;
            discItems = new DiscItem[32]; //MAX NUMBER OF ELEMENTS

            for (int i = 0; i < discItems.Length; i++)
            {
                discItems[i] = new DiscItem();
            }
        }

        public void OnCreateDevice()
        {
            basicEffect = new BasicEffect(FrameworkCore.Graphics.GraphicsDevice);

            CreateShape();
        }

        public void CreateShape()
        {
            vertexDecl = VertexPositionNormalTexture.VertexDeclaration;

            double angle = MathHelper.TwoPi / CIRCLE_NUM_POINTS;

            _vertices = new VertexPositionNormalTexture[CIRCLE_NUM_POINTS + 1];

            _vertices[0] = new VertexPositionNormalTexture(
                Vector3.Zero, Vector3.Forward, Vector2.One);

            for (int i = 1; i <= CIRCLE_NUM_POINTS; i++)
            {
                float x = (float)Math.Round(Math.Sin(angle * i), 4);
                float y = (float)Math.Round(Math.Cos(angle * i), 4);
                Vector3 point = new Vector3(x, y, 0.0f);

                _vertices[i] = new VertexPositionNormalTexture(
                    point,
                    Vector3.Forward,
                    new Vector2());
            }

            // Initialize the vertex buffer, allocating memory for each vertex
            buffer = new VertexBuffer(FrameworkCore.Graphics.GraphicsDevice,
                VertexPositionNormalTexture.VertexDeclaration, _vertices.Length,
                BufferUsage.None);

            // Set the vertex buffer data to the array of vertices
            buffer.SetData<VertexPositionNormalTexture>(_vertices);

            InitializeLineStrip();
        }

        private void InitializeLineStrip()
        {

            // Initialize an array of indices of type short
            short[] lineStripIndices = new short[CIRCLE_NUM_POINTS + 1];

            // Populate the array with references to indices in the vertex buffer
            for (int i = 0; i < CIRCLE_NUM_POINTS; i++)
            {
                lineStripIndices[i] = (short)(i + 1);
            }

            lineStripIndices[CIRCLE_NUM_POINTS] = 1;

            // Initialize the index buffer, allocating memory for each index
            _indexBuffer = new IndexBuffer(
                FrameworkCore.Graphics.GraphicsDevice,
                IndexElementSize.SixteenBits,
                lineStripIndices.Length,
                BufferUsage.None
                );

            // Set the data in the index buffer to our array
            _indexBuffer.SetData<short>(lineStripIndices);

        }


        
        public void StartDraw(Camera camera)
        {
            GraphicsDevice device = FrameworkCore.Graphics.GraphicsDevice;

            device.DepthStencilState = DepthStencilState.Default;
            BlendStateHelper.BeginApply(device);
            BlendStateHelper.AlphaBlendEnable = true;
            BlendStateHelper.SourceBlend = Blend.SourceAlpha;
            BlendStateHelper.DestinationBlend = Blend.InverseSourceAlpha;
            BlendStateHelper.EndApply(device);
            device.RasterizerState = RasterizerState.CullCounterClockwise;

            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;

            if (Helpers.CheckMatrixNans(basicEffect.Projection))
                return;

            basicEffect.CurrentTechnique.Passes[0].Apply();
        }

        public void EndBatch(Camera camera)
        {
            try
            {
                StartDraw(camera);

                GraphicsDevice device = FrameworkCore.Graphics.GraphicsDevice;
                using (VertexDeclaration vertexDecl = VertexPositionNormalTexture.VertexDeclaration)
                {
                    device.SetVertexBuffer(buffer);
                    device.Indices = _indexBuffer;

                    for (int i = 0; i < freeIndex; i++)
                    {
                        DrawDisc(discItems[i].size, discItems[i].position, discItems[i].discColor, discItems[i].orientation);
                    }
                }
            }
            finally
            {
                EndDraw();
            }

            freeIndex = 0;
        }

        private void EndDraw()
        {
        }

        public void DrawDisc(float size, Vector3 position, Color color, Matrix orient)
        {
            Matrix scaleMatrix = Matrix.CreateScale(size);
            Matrix translateMat = Matrix.CreateTranslation(position);
            Matrix rotateYMatrix = Matrix.CreateRotationY(RADIANS_FOR_90DEGREES);
            Matrix rotateXMatrix = Matrix.CreateRotationX(RADIANS_FOR_90DEGREES);

            GraphicsDevice device = FrameworkCore.Graphics.GraphicsDevice;

            //BC 3-28-2019 Brute-force increase the alpha value of the discs so they're more visible.
            //basicEffect.Alpha = ((float)color.A / (float)byte.MaxValue);            
            basicEffect.Alpha = (float)(Math.Min(255.0f, color.A * 1.5f)) / (float)byte.MaxValue;


            basicEffect.World = rotateXMatrix * orient *  scaleMatrix * translateMat;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.CurrentTechnique.Passes[0].Apply();

            device.DrawIndexedPrimitives(
                PrimitiveType.LineStrip,
                0,  // vertex buffer offset to add to each element of the index buffer
                0,  // minimum vertex index
                CIRCLE_NUM_POINTS + 1, // number of vertices. If this gets an exception for you try changing it to 0.  Seems to work just as well.
                0,  // first index element to read
                CIRCLE_NUM_POINTS); // number of primitives to draw
        }

        public void Draw(float size, Vector3 position, Color color)
        {
            Draw(size, position, color, Matrix.Identity);
        }

        public void Draw(float size, Vector3 position, Color color, Matrix orient)
        {
            if (size <= 0)
                return;

            if (freeIndex >= discItems.Length)
            {
                //overflow
                Console.WriteLine("ERROR: TOO MUCH DISC ELEMENTS. INCREASE ARRAY SIZE");
                return;
            }

            discItems[freeIndex].size = size;
            discItems[freeIndex].position = position;
            discItems[freeIndex].discColor = color;
            discItems[freeIndex].orientation = orient;

            freeIndex++;
        }

    }
}
