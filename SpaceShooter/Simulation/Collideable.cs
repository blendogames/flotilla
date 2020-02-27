#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class Entity
    {
        int playbackID = -1;
        public void SetPlaybackID(int id)
        {
            this.playbackID = id;
        }
        public int PlaybackID
        {
            get { return playbackID; }
        }


        public bool isStatic = false;


        Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        Quaternion rotateQuat;

        public Quaternion Rotation
        {
            get { return rotateQuat; }
            set { rotateQuat = value; }
        }

        ModelType modelmesh;
        public ModelType modelMesh
        {
            get { return modelmesh; }
            set { modelmesh = value; }
        }

        public virtual void Draw(GameTime gameTime, Camera camera, PlayerCommander curplayer)
        {
        }
    }

    public abstract class Collideable : Entity
    {
        public IAudioEmitter audioEmitter;

        


        CollisionSphere[] collisionSpheres;

        public CollisionSphere[] CollisionSpheres
        {
            get { return collisionSpheres; }
            set { collisionSpheres = value; }
        }

        BoundingSphere bSphere;

        public BoundingSphere BSphere
        {
            get
            {
                bSphere.Center = Position;
                return bSphere;
            }
            set { bSphere = value; }
        }




        bool destroyed;
        public bool IsDestroyed
        {
            get { return destroyed; }
            set { destroyed = value; }
        }

        float health;

        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public Commander owner;

        public virtual bool IsVisible(Camera camera)
        {
            foreach (ModelMesh mesh in FrameworkCore.ModelArray[(int)modelMesh].Meshes)
            {
                BoundingSphere localSphere = mesh.BoundingSphere;
                localSphere.Center += Position;

                ContainmentType contains = camera.BF.Contains(localSphere);
                if (contains == ContainmentType.Contains || contains == ContainmentType.Intersects)
                    return true;
            }

            return false;
        }

        public virtual void Hit(Bolt bolt)
        {
        }

        public virtual void Hit(Collideable ship, Vector3 intersectPoint)
        {
        }

        public virtual void beamHit(Collideable killer, Vector3 hitPos, int minDmg, int maxDmg, Vector3 muzzleVec)
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            Matrix orientation = Matrix.CreateFromQuaternion(this.Rotation);

            if (audioEmitter != null)
            {
                audioEmitter.Position = this.Position;
                audioEmitter.Forward = orientation.Forward;
                audioEmitter.Up = orientation.Up;
            }
        }

        public override void Draw(GameTime gameTime, Camera camera, PlayerCommander curPlayer)
        {
            
        }

        //update the position of the collisionspheres.
        public void UpdateCollisionSpheres()
        {
            Matrix m = Matrix.CreateFromQuaternion(Rotation);

            for (int i = 0; i < CollisionSpheres.Length; i++)
            {
                Vector3 worldPos = Position;

                if (CollisionSpheres[i].offset != Vector3.Zero)
                {
                    Vector3 offset = CollisionSpheres[i].offset;

                    worldPos +=
                        m.Forward * -offset.Z +
                        m.Right * offset.X +
                        m.Up * offset.Y;
                }

                CollisionSpheres[i].sphere.Center = worldPos;
            }
        }
    }
}