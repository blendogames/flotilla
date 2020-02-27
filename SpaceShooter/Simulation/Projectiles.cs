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
    public enum TrailType
    {
        None,
        RocketTrail,
        TorpedoTrail,
    }

    public static class ProjectileTypes
    {
        public static ProjectileData PrjBullet = new ProjectileData(
            ModelType.Bullet, /* Mesh */
            18 /*Speed*/, 0.5f /*Speed Variation*/,
            1 /*MinDamage*/, 5 /*MaxDamage*/,
            1 /*Base Penetration*/,
            new float[,] /*Class Penetration table*/
            {
                {(float)ShipClass.CapitalShip, 0.1f},
                {(float)ShipClass.Destroyer, 0.1f},
                {(float)ShipClass.Fighter, 1},
            },
            false /*Does this projectile visibly deflect away*/,
            TrailType.None
            );

        public static ProjectileData PrjMissile = new ProjectileData(
            ModelType.Missile,
            10, 0.5f,
            40, 60,
            1,
            new float[,]
            {
                {(float)ShipClass.CapitalShip, 1},
                {(float)ShipClass.Destroyer, 1},
                {(float)ShipClass.Fighter, 0},
            },
            true,
            TrailType.RocketTrail
            );

        public static ProjectileData PrjTorpedo = new ProjectileData(
            ModelType.Torpedo,
            16, 0.5f,
            100, 120,
            1,
            new float[,]
            {
                {(float)ShipClass.CapitalShip, 1},
                {(float)ShipClass.Destroyer, 1},
                {(float)ShipClass.Fighter, 0},
            },
            true,
            TrailType.TorpedoTrail
            );
    }



    public class ProjectileData
    {
        public ModelType modelName;
        public float speed;
        public float speedVariance;
        public float minDamage;
        public float maxDamage;
        public float basePenetration;
        public float[,] classModifiers; //penetration modifier of projectile vs. ship classes.

        public bool debrisDeflecter;  //determines if deflection causes projectile to "ping" off and spin away.

        public TrailType trailType;

        public ProjectileData(ModelType model, float Speed, float SpeedVariance, float MinDamage, float MaxDamage,
            float basePen,
            float[,] ClassModifiers,
            bool debrisdeflector, TrailType trail)
        {
            this.modelName = model;
            this.speed = Speed;
            this.speedVariance = SpeedVariance;
            this.minDamage = MinDamage;
            this.maxDamage = MaxDamage;
            this.classModifiers = ClassModifiers;
            this.basePenetration = basePen;
            this.debrisDeflecter = debrisdeflector;
            this.trailType = trail;
        }
    }
}