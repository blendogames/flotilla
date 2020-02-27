#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using
using System;
using System.Collections.Generic;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#if XBOX
using Microsoft.Xna.Framework.Net;
#endif
using Microsoft.Xna.Framework.Storage;



#endregion

namespace SpaceShooter
{
    public struct RenderableMesh
    {
        public Matrix worldMatrix;
        public Color diffuseColor;
        public Camera camera;
        public float alpha;
    }

    public struct EffectItem
    {
        public ModelType model;
        public RenderableMesh[] meshes;
        public int lastSlotIndex;
    }

    public class MeshRenderer
    {
        BasicEffect effect = null;

        EffectItem[] effectList;

        /// <summary>
        /// Max number of any mesh instance.
        /// </summary>
        const int SLOTMAXMESHES = 1024;

        public MeshRenderer()
        {

        }

        public void Initialize()
        {
            //make a bucket of all the meshes in the game.
            effectList = new EffectItem[FrameworkCore.ModelArray.GetLength(0)];

            for (int i = 0; i < effectList.Length; i++)
            {
                effectList[i].model = (ModelType)Enum.ToObject(typeof(ModelType), i);
                effectList[i].meshes = new RenderableMesh[SLOTMAXMESHES];
                effectList[i].lastSlotIndex = 0;
            }
        }

        public void LoadContent()
        {
            effect = new BasicEffect(FrameworkCore.Graphics.GraphicsDevice);
        }

        public void Draw(ModelType model, Matrix worldMatrix, Camera camera)
        {
            Draw(model, worldMatrix, camera, Color.White, 1.0f);
        }


        public void Draw(ModelType model, Matrix worldMatrix, Camera camera, Color meshColor)
        {
            Draw(model, worldMatrix, camera, meshColor, 1.0f);
        }

        public void Draw(ModelType model, Matrix worldMatrix, Camera camera, Color meshColor, float Alpha)
        {
            if (effectList[(int)model].lastSlotIndex >= SLOTMAXMESHES)
            {
                //uh oh, ran out of slots for rendering this object. don't draw it.
                return;
            }

            //plug data into free slot.
            effectList[(int)model].meshes[effectList[(int)model].lastSlotIndex].worldMatrix = worldMatrix;
            effectList[(int)model].meshes[effectList[(int)model].lastSlotIndex].camera = camera;
            effectList[(int)model].meshes[effectList[(int)model].lastSlotIndex].diffuseColor = meshColor;
            effectList[(int)model].meshes[effectList[(int)model].lastSlotIndex].alpha = Alpha;

            //increase slot count.
            effectList[(int)model].lastSlotIndex++;
        }

        //initialize effect settings.
        public void RenderStart(ModelType model)
        {
            BlendStateHelper.BeginApply(FrameworkCore.Graphics.GraphicsDevice);
            if (!BlendStateHelper.AlphaBlendEnable)
            {
                BlendStateHelper.AlphaBlendEnable = true;
                BlendStateHelper.EndApply(FrameworkCore.Graphics.GraphicsDevice);
            }

            FrameworkCore.Graphics.GraphicsDevice.Indices = FrameworkCore.ModelArray[(int)model].Meshes[0].MeshParts[0].IndexBuffer;
            FrameworkCore.Graphics.GraphicsDevice.SetVertexBuffer(FrameworkCore.ModelArray[(int)model].Meshes[0].MeshParts[0].VertexBuffer);



            effect.Texture = FrameworkCore.TextureArray[(int)model];
            effect.CurrentTechnique.Passes[0].Apply();
        }

        //render the mesh.
        public void RenderMesh(ModelType model, Matrix worldMatrix, Color meshColor, float Alpha)
        {
            effect.Alpha = Alpha;

            effect.DiffuseColor = meshColor.ToVector3();
            
            effect.World = worldMatrix;
            effect.CurrentTechnique.Passes[0].Apply();

            FrameworkCore.Graphics.GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                FrameworkCore.ModelArray[(int)model].Meshes[0].MeshParts[0].VertexOffset, 0,
                FrameworkCore.ModelArray[(int)model].Meshes[0].MeshParts[0].NumVertices,
                FrameworkCore.ModelArray[(int)model].Meshes[0].MeshParts[0].StartIndex,
                FrameworkCore.ModelArray[(int)model].Meshes[0].MeshParts[0].PrimitiveCount);
        }

        public void RenderEnd()
        {
        }

        public void EndBatch(Camera camera)
        {
            EndBatch(camera, false);
        }

        //set Cull to true for things like hud elements. they have draw priority over everything else.
        public void EndBatch(Camera camera, bool cull)
        {
            if (cull)
                FrameworkCore.Graphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            // FIXME: Why is this always read/write when XNA3 wasn't like that? -flibit
            FrameworkCore.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            FrameworkCore.Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;



            //now, iterate through the list and draw everything.
            effect.TextureEnabled = true;
            effect.View = camera.View;
            effect.Projection = camera.Projection;

            for (int i = 0; i < effectList.Length; i++)
            {
                if (effectList[i].lastSlotIndex <= 0)
                    continue;

                try
                {
                    RenderStart(effectList[i].model);

                    //foreach (RenderableMesh item in effectList[i].meshes)
                    for (int k = 0; k < effectList[i].lastSlotIndex; k++)
                    {
                        RenderMesh(effectList[i].model,
                            effectList[i].meshes[k].worldMatrix,
                            effectList[i].meshes[k].diffuseColor,
                            effectList[i].meshes[k].alpha);
                    }
                }
                finally
                {
                    RenderEnd();
                }

                effectList[i].lastSlotIndex = 0;
            }

        }
    }
}