using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShooter
{
    public class LineItem
    {
        public Vector3 start;
        public Vector3 end;
        public Color lineColor;
    }

    public class LineRenderer
    {
        private SpaceShooterGame _gameInstance = null;

        protected VertexBuffer buffer;
        protected VertexDeclaration vertexDecl;

        private BasicEffect basicEffect;

        private const int NUMPOINTS = 2;

        private IndexBuffer _indexBuffer;
        private VertexPositionNormalTexture[] _vertices;

        int freeIndex;
        LineItem[] lineItems;

        public LineRenderer(SpaceShooterGame game)
        {
            _gameInstance = game;

            freeIndex = 0;
            lineItems = new LineItem[1024]; //MAX NUMBER OF LINE ELEMENTS

            for (int i = 0; i < lineItems.Length; i++)
            {
                lineItems[i] = new LineItem();
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

            _vertices = new VertexPositionNormalTexture[NUMPOINTS + 1];


            _vertices[0] = new VertexPositionNormalTexture(
                Vector3.Zero, Vector3.Forward, Vector2.One);

            _vertices[1] = new VertexPositionNormalTexture(
                                            Vector3.Zero, Vector3.Forward, new Vector2());

            _vertices[2] = new VertexPositionNormalTexture(
                                            new Vector3(0, 1, 0), Vector3.Forward, new Vector2());

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
            short[] lineStripIndices = new short[NUMPOINTS + 1];

            // Populate the array with references to indices in the vertex buffer
            for (int i = 0; i < NUMPOINTS; i++)
            {
                lineStripIndices[i] = (short)(i + 1);
            }

            lineStripIndices[NUMPOINTS] = 1;

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


        public void Draw(Vector3 startVec, Vector3 endVec, Color color)
        {
            if (freeIndex >= lineItems.Length)
            {
                //overflow
                Console.WriteLine("ERROR: TOO MUCH LINE ELEMENTS. INCREASE LINE ARRAY SIZE");
                return;
            }

            lineItems[freeIndex].start = startVec;
            lineItems[freeIndex].end = endVec;
            lineItems[freeIndex].lineColor = color;

            freeIndex++;
        }

        //final call, that renders every line in the line list.
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
                        DrawLine(lineItems[i].start, lineItems[i].end, lineItems[i].lineColor);
                    }
                }                
            }
            finally
            {
                EndDraw();
            }

            //reset the index.
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

            // effect is a compiled effect created and compiled elsewhere
            // in the application
            //basicEffect.EnableDefaultLighting();
            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;

            basicEffect.CurrentTechnique.Passes[0].Apply();
        }

        private void DrawLine(Vector3 startVec, Vector3 endVec, Color color)
        {
            //THE MOTHER OF ALL HACKS
            if (endVec.X == startVec.X && startVec.Z == endVec.Z)
            {
                endVec.X += 0.001f;
            }

            Vector3 moveDir = endVec - startVec;
            moveDir.Normalize();

            Vector3 up = Vector3.Cross(Vector3.Up, moveDir);
            up.Normalize();

            float angleUp = (float)Math.Acos(Vector3.Dot(moveDir, Vector3.Up));

            Matrix orientation = Matrix.CreateFromAxisAngle(up, angleUp);
            orientation.Translation = Vector3.Zero;

            Matrix scaleMatrix = Matrix.CreateScale(Vector3.Distance(startVec, endVec));
            Matrix translateMat = Matrix.CreateTranslation(startVec);

            GraphicsDevice device = FrameworkCore.Graphics.GraphicsDevice;


            //BC 3-28-2019 Brute-force increase the alpha of the lines.
            //basicEffect.Alpha = ((float)color.A / (float)byte.MaxValue);
            basicEffect.Alpha = ((float)Math.Min(255.0f, color.A * 1.5f) / (float)byte.MaxValue);


            basicEffect.World = orientation * scaleMatrix * translateMat;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.CurrentTechnique.Passes[0].Apply();

            device.DrawIndexedPrimitives(
                PrimitiveType.LineStrip,
                0,  // vertex buffer offset to add to each element of the index buffer
                0,  // minimum vertex index
                NUMPOINTS, // number of vertices. If this gets an exception for you try changing it to 0.  Seems to work just as well.
                0,  // first index element to read
                1); // number of primitives to draw
            
        }

        private void EndDraw()
        {
        }

    }
}
