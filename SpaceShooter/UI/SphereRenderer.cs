using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    public class SphereItem
    {
        public BoundingSphere sphere;
        public Matrix orientation;
        public Color sphereColor;
    }

    public class SphereRenderer
    {
        public static float RADIANS_FOR_90DEGREES = MathHelper.ToRadians(90);//(float)(Math.PI / 2.0);
        public static float RADIANS_FOR_180DEGREES = RADIANS_FOR_90DEGREES * 2;

        private SpaceShooterGame _gameInstance = null;

        protected VertexBuffer buffer;
        protected VertexDeclaration vertexDecl;

        private BasicEffect basicEffect;

        private const int CIRCLE_NUM_POINTS = 32;
        private IndexBuffer _indexBuffer;
        private VertexPositionNormalTexture[] _vertices;

        int freeIndex;
        SphereItem[] sphereItems;

        public SphereRenderer(SpaceShooterGame game)
        {
            _gameInstance = game;

            freeIndex = 0;
#if DEBUG
            sphereItems = new SphereItem[2048]; //MAX NUMBER OF ELEMENTS
#else
            sphereItems = new SphereItem[256]; //MAX NUMBER OF ELEMENTS
#endif

            for (int i = 0; i < sphereItems.Length; i++)
            {
                sphereItems[i] = new SphereItem();
            }
        }

        public void OnCreateDevice()
        {
            basicEffect = new BasicEffect(FrameworkCore.Graphics.GraphicsDevice);

            CreateShape();
        }

        public void CreateShape()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)_gameInstance.Services.GetService(typeof(IGraphicsDeviceService));

            vertexDecl = VertexPositionNormalTexture.VertexDeclaration;

            double angle = MathHelper.TwoPi / CIRCLE_NUM_POINTS;

            _vertices = new VertexPositionNormalTexture[CIRCLE_NUM_POINTS + 1];

            _vertices[0] = new VertexPositionNormalTexture(
                Vector3.Zero, Vector3.Forward, Vector2.One);

            for (int i = 1; i <= CIRCLE_NUM_POINTS; i++)
            {
                float x = (float)Math.Round(Math.Sin(angle * i), 4);
                float y = (float)Math.Round(Math.Cos(angle * i), 4);
                Vector3 point = new Vector3(
                                 x,
                                 y,
                                  0.0f);

                _vertices[i] = new VertexPositionNormalTexture(
                    point,
                    Vector3.Forward,
                    new Vector2());
            }

            // Initialize the vertex buffer, allocating memory for each vertex
            buffer = new VertexBuffer(graphicsService.GraphicsDevice,
                VertexPositionNormalTexture.VertexDeclaration, _vertices.Length,
                BufferUsage.None);


            // Set the vertex buffer data to the array of vertices
            buffer.SetData<VertexPositionNormalTexture>(_vertices);

            InitializeLineStrip();
        }

        private void InitializeLineStrip()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)_gameInstance.Services.GetService(typeof(IGraphicsDeviceService));

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
                graphicsService.GraphicsDevice,
                IndexElementSize.SixteenBits,
                lineStripIndices.Length,
                BufferUsage.None
                );

            // Set the data in the index buffer to our array
            _indexBuffer.SetData<short>(lineStripIndices);
        }

        public void Draw(BoundingSphere bs, Matrix orientation, Color color)
        {
            if (bs.Radius <= 0)
                return;

            if (freeIndex >= sphereItems.Length)
            {
                //overflow
                Console.WriteLine("ERROR: TOO MUCH SPHERE ELEMENTS. INCREASE ARRAY SIZE");
                return;
            }

            sphereItems[freeIndex].sphere = bs;
            sphereItems[freeIndex].orientation = orientation;
            sphereItems[freeIndex].sphereColor = color;

            freeIndex++;
        }

        public void EndBatch(Camera camera)
        {
            try
            {
                GraphicsDevice device = FrameworkCore.Graphics.GraphicsDevice;

                StartDraw(camera);
                using (VertexDeclaration vertexDecl = VertexPositionNormalTexture.VertexDeclaration)
                {
                    device.SetVertexBuffer(buffer);
                    device.Indices = _indexBuffer;

                    for (int i = 0; i < freeIndex; i++)
                    {
                        DrawSphere(sphereItems[i].sphere, sphereItems[i].orientation, sphereItems[i].sphereColor);
                    }
                }
            }
            finally
            {
                EndDraw();
            }

            freeIndex = 0;
        }

        private void StartDraw(Camera camera)
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

        private void EndDraw()
        {
        }


        private void DrawSphere(BoundingSphere bs, Matrix orientation, Color color)
        {
            if (bs == null)
                return;

            GraphicsDevice device = FrameworkCore.Graphics.GraphicsDevice;

            Matrix scaleMatrix = Matrix.CreateScale(bs.Radius);
            Matrix translateMat = Matrix.CreateTranslation(bs.Center);
            Matrix rotateYMatrix = Matrix.CreateRotationY(RADIANS_FOR_90DEGREES);
            Matrix rotateXMatrix = Matrix.CreateRotationX(RADIANS_FOR_90DEGREES);




            

            basicEffect.Alpha = ((float)color.A / (float)byte.MaxValue);
            basicEffect.World = orientation * scaleMatrix * translateMat;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.CurrentTechnique.Passes[0].Apply();

            device.DrawIndexedPrimitives(
                PrimitiveType.LineStrip,
                0,  // vertex buffer offset to add to each element of the index buffer
                0,  // minimum vertex index
                CIRCLE_NUM_POINTS + 1, // number of vertices. If this gets an exception for you try changing it to 0.  Seems to work just as well.
                0,  // first index element to read
                CIRCLE_NUM_POINTS); // number of primitives to draw

            basicEffect.World = rotateYMatrix * orientation * scaleMatrix * translateMat;
            //basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.CurrentTechnique.Passes[0].Apply();

            device.DrawIndexedPrimitives(
                PrimitiveType.LineStrip,
                0,  // vertex buffer offset to add to each element of the index buffer
                0,  // minimum vertex index
                CIRCLE_NUM_POINTS + 1, // number of vertices. If this gets an exception for you try changing it to 0.  Seems to work just as well.
                0,  // first index element to read
                CIRCLE_NUM_POINTS); // number of primitives to draw

            basicEffect.World = rotateXMatrix * orientation * scaleMatrix * translateMat;
            //basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.CurrentTechnique.Passes[0].Apply();

            device.DrawIndexedPrimitives(
                PrimitiveType.LineStrip,
                0,  // vertex buffer offset to add to each element of the index buffer
                0,  // minimum vertex index
                CIRCLE_NUM_POINTS + 1, // number of vertices. If this gets an exception for you try changing it to 0.  Seems to work just as well.
                0,  // first index element to read
                CIRCLE_NUM_POINTS); // number of primitives to draw
                

            
        }
    }
}
