#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace SpaceShooter
{


    #region Collision Data
    public class CollisionSphere
    {
        public BoundingSphere sphere;
        public Vector3 offset; //Local offset of the sphere.

        public CollisionSphere(BoundingSphere Sphere, Vector3 Offset)
        {
            sphere = Sphere;
            offset = Offset;
        }
    }
    
    public static class collisions
    {
        public static CollisionSphere[] Battleship = new CollisionSphere[] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.4f), new Vector3(2,0,-21)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.4f), new Vector3(-2,0,-21)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.9f), new Vector3(1.9f,0,-17.5f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.9f), new Vector3(-1.9f,0,-17.5f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.4f), new Vector3(2.4f,0,-13.2f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.4f), new Vector3(-2.4f,0,-13.2f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.7f), new Vector3(2.9f,0,-8.1f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.7f), new Vector3(-2.9f,0,-8.1f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 4.2f), new Vector3(3.2f,0,-2.7f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 4.2f), new Vector3(-3.2f,0,-2.7f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 4.2f), new Vector3(2.4f,0,2.7f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 4.2f), new Vector3(-2.4f,0,2.7f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 4.2f), new Vector3(2.5f,0,9.0f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 4.2f), new Vector3(-2.5f,0,9.0f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 4.2f), new Vector3(3.4f,0,14f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 4.2f), new Vector3(-3.4f,0,14f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3f), new Vector3(2.6f,0,19f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3f), new Vector3(-2.6f,0,19f)),
            };

        public static CollisionSphere[] Destroyer = new CollisionSphere[] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 1.3f), new Vector3(0,-0.3f,-2.4f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 1.5f), new Vector3(0,-0.3f,-1.3f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 1.9f), new Vector3(0,-0.3f,0.1f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 1.8f), new Vector3(0,-0.3f,1.7f)),
            };

        public static CollisionSphere[] Empty = new CollisionSphere[] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 0.01f), Vector3.Zero),
            };


        public static CollisionSphere[] Fighter = new CollisionSphere[1] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 1.3f), Vector3.Zero)
            };

        public static CollisionSphere[] BeamFrigate = new CollisionSphere[6] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3), new Vector3(0, 2, -3.7f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3), new Vector3(0, 2, 4.1f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3), new Vector3(0, 2, 0.2f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3), new Vector3(0, -2.3f, -3.7f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3), new Vector3(0, -2.3f, 4.1f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3), new Vector3(0, -2.3f, 0.2f)),
            };

        public static CollisionSphere[] Gunship = new CollisionSphere[] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.5f), new Vector3(1.3f, -0.2f, -2.9f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.5f), new Vector3(-1.3f, -0.2f, -2.9f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.7f), new Vector3(1.5f, 0, 1.1f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.7f), new Vector3(-1.5f, 0, 1.1f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.7f), new Vector3(1.5f, 0, 6.2f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 3.7f), new Vector3(-1.5f, 0, 6.2f)),

                //nose.
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(-0.4f, -0.7f, -7.1f)),
            };

        public static CollisionSphere[] Dreadnought = new CollisionSphere[] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.4f), new Vector3(-5.0f, 0, -2.7f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.4f), new Vector3(5.0f, 0, -2.7f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.4f), new Vector3(-1.5f, 0, -3.3f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.4f), new Vector3(1.5f, 0, -3.3f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.4f), new Vector3(0, 0.5f, 0.15f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.5f), new Vector3(0, 0.8f, 3.3f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.3f), new Vector3(-2.6f, 0.55f, 3.6f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.3f), new Vector3(2.6f, 0.55f, 3.6f)),
            };

        public static CollisionSphere[] BeamGunship = new CollisionSphere[] 
            {
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, 1.1f, -6.4f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, 1.1f, -3.5f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, 1.1f, -1.0f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, 1.1f, 1.5f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, 1.1f, 4)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, 1.1f, 6.5f)),

                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, -1.1f, -6.4f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, -1.1f, -3.5f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, -1.1f, -1.0f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, -1.1f, 1.5f)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, -1.1f, 4)),
                new CollisionSphere(new BoundingSphere(Vector3.Zero, 2.8f), new Vector3(0, -1.1f, 6.5f)),
            };

    }
    #endregion

    public enum locationType
    {
        Top = 0,
        Bottom = 1,
        Rear = 2
    }

    public static class shipTypes
    {
        public static ShipData DebugShip = new ShipData("DEBUGSHIP", ModelType.shipDestroyer,
            64f, 0.4f, collisions.Destroyer, 10,
            null,
            new float[3] { 0.05f, 0.9f, 1.0f },
            ShipClass.Destroyer,
            2,
            6, 1,3,
            sprite.icons.wrench,
            0,
            DamageTypes.Destroyer,
            16,
            100
            );







        public static ShipData Drone = new ShipData(Resource.ShipTrainingDrone, ModelType.shipDestroyer,
            2.5f, 0.11f, collisions.Destroyer, 120,
            null,
            new float[3] { 0.05f, 0.9f, 1.0f },
            ShipClass.Destroyer,
            2,
            6, 1, 3,
            sprite.icons.wrench,
            0,
            DamageTypes.Destroyer,
            16,
            0
            );



        public static ShipData BeamFrigate = new ShipData(Resource.ShipBeamFrigate, ModelType.shipBeamFrigate,
            3f,         /* Max Speed */
            0.15f,      /* Rotation Speed */
            collisions.BeamFrigate, /* Collision Data set */
            300,       /* Max Health */
            new TurretData[]
            {
                new TurretTypes.BeamTurret(new Vector3(0, 0.56f, -6f),
                    Vector3.Forward, Vector3.Forward),
            },
            new float[3] { 0.05f, 0.9f, 1.0f },  /*front armor, bottom armor, rear armor */
            ShipClass.Destroyer,   /* ship class */
            4,
            12, 3,10,
            sprite.ships.beamFrigate,
            20,
            DamageTypes.BeamFrigate,
            8,
            250
        );

        public static ShipData BeamGunship = new ShipData(Resource.ShipBeamGunship, ModelType.shipBeamGunship,
            2.5f,         /* Max Speed */
            0.12f,      /* Rotation Speed */
            collisions.BeamGunship, /* Collision Data set */
            400,       /* Max Health */
            new TurretData[]
            {
                new TurretTypes.BeamTurret(new Vector3(-1.8f, 0, -5),
                    Vector3.Left, Vector3.Forward),
                new TurretTypes.BeamTurret(new Vector3(1.8f, 0, -5),
                    Vector3.Right, Vector3.Forward),
            },
            new float[3] { 0.05f, 0.9f, 1.0f },  /*front armor, bottom armor, rear armor */
            ShipClass.Destroyer,   /* ship class */
            6,
            16, 4, 7,
            sprite.ships.beamGunship,
            20,
            DamageTypes.BeamGunship,
            8,
            350
        );

        public static ShipData Battleship = new ShipData(Resource.ShipBattleship, ModelType.shipCapitalShip,
            1.5f,         /* Max Speed */
            0.07f,      /* Rotation Speed */
            collisions.Battleship, /* Collision Data set */
            1000,       /* Max Health */
            new TurretData[]
            {
                new TurretTypes.HeavyCannon(new Vector3(0, 2.2f, 3.7f),
                    Vector3.Up, Vector3.Forward),
                new TurretTypes.HeavyCannon(new Vector3(0, 2.2f, -5.2f),
                    Vector3.Up, Vector3.Forward),

                new TurretTypes.BallTurret(new Vector3(0, 1.1f, -19f),
                    Vector3.Up, Vector3.Forward),
                new TurretTypes.BallTurret(new Vector3(0, -1.2f, -19f),
                    Vector3.Down, Vector3.Forward),
            },
            new float[3] { 0.01f, 0.9f, 1.0f },  /*front armor, bottom armor, rear armor */
            ShipClass.CapitalShip,   /* ship class */
            8,
            42, 12, 4,
            sprite.ships.capitalship,
            16,
            DamageTypes.Battleship,
            0,
            400
            );

        public static ShipData Destroyer = new ShipData(Resource.ShipDestroyer, ModelType.shipDestroyer,
            4f, 0.18f, collisions.Destroyer,
            200,
            new TurretData[]
            {
                new TurretTypes.LongCannon(new Vector3(0.3f, -0.32f, -3.0f),
                    Vector3.Forward, Vector3.Forward),
            },
            new float[3] { 0.05f, 0.7f, 0.95f },
            ShipClass.Destroyer,
            3,
            6, 1, 3,
            sprite.ships.destroyer,
            0,
            DamageTypes.Destroyer,
            16,
            200
            );


        public static ShipData Gunship = new ShipData(Resource.ShipGunship, ModelType.shipGunship,
            3f, 0.12f, collisions.Gunship,
            500,
            new TurretData[]
            {
                new TurretTypes.TorpedoTurret(new Vector3(-1.5f, -0.95f, -8.85f),
                    Vector3.Forward, Vector3.Forward),

                new TurretTypes.TorpedoTurret(new Vector3(0, 3.7f, -0.15f),
                    Vector3.Up, Vector3.Forward),
            },
            new float[3] { 0.01f, 0.7f, 0.95f },
            ShipClass.Destroyer,
            4,
            18, 9, 7,
            sprite.ships.gunship,
            16,
            DamageTypes.Gunship,
            8,
            250
            );

        public static ShipData Dreadnought = new ShipData(Resource.ShipDreadnought, ModelType.shipDreadnought,
            2f, 0.11f, collisions.Dreadnought,
            400,
            new TurretData[]
            {
                new TurretTypes.BeamTurret(new Vector3(-5.2f, -0.4f, -4.5f),
                    Vector3.Forward, Vector3.Forward),

                new TurretTypes.BeamTurret(new Vector3(5.2f, -0.4f, -4.5f),
                    Vector3.Forward, Vector3.Forward),
            },
            new float[3] { 0.01f, 0.7f, 0.95f },
            ShipClass.Destroyer,
            5,
            11, 14, 5,
            sprite.ships.dreadnought,
            1,
            DamageTypes.Dreadnought,
            8,
            300
            );




        public static ShipData Fighter = new ShipData(Resource.ShipFighter, ModelType.shipFighter,
            5f, 0.3f, collisions.Fighter, 50,
            new TurretData[]
            {
                new TurretTypes.InvisibleBeamTurret(Vector3.Zero,
                    Vector3.Forward, Vector3.Forward),

                new TurretTypes.InvisibleBeamTurret(Vector3.Zero,
                    Vector3.Backward, Vector3.Backward),
            },
            new float[3]{1, 1, 1},
            ShipClass.Destroyer,
            0,
            1,1,1,
            sprite.ships.fighter,
            0,
            DamageTypes.Fighter,
            8,
            150
            );

        /*
        public static class Cars
        {
            public static Rectangle Dorado = new Rectangle(0, 0, 64, 64);
        }
        */
    }

    public static class DamageTypes
    {
        public static DamageData Destroyer = new DamageData(
            new Vector3[]
            {
                new Vector3(-0.4f, 0, 2.7f),
            },
            200);

        public static DamageData Fighter = new DamageData(
                    new Vector3[]
            {
                Vector3.Zero,
            },
                    200);

        public static DamageData Gunship = new DamageData(
            new Vector3[]
            {
                new Vector3(0, 0, 9.1f),
            },
            200);

        public static DamageData BeamGunship = new DamageData(
                    new Vector3[]
            {
                new Vector3(0, -1.5f, 8),
                new Vector3(0, 2.2f, 7.6f),
            },
                    200);

        public static DamageData BeamFrigate = new DamageData(
            new Vector3[]
            {
                new Vector3(0, 1.8f, 6.4f),
                new Vector3(0, -2.3f, 6.4f),
            },
            200);

        public static DamageData Battleship = new DamageData(
            new Vector3[]
            {
                new Vector3(-2, 0, 21.1f),
                new Vector3(2, 0, 21.1f),
            },
            200);

        public static DamageData Dreadnought = new DamageData(
            new Vector3[]
            {
                new Vector3(-2, 0, 5.6f),
                new Vector3(2, 0, 5.6f),
            },
            200);
    }

    #region Turret Data
    public class TurretTypes
    {
        public class HeavyCannon : TurretData
        {
            public HeavyCannon(Vector3 localOffset, Vector3 upAimVector, Vector3 defaultVector)
                : base()
            {
                this.localOffset = localOffset;
                this.upAimVector = upAimVector;
                this.defaultVector = defaultVector;

                this.modelName = ModelType.turretBig;
                this.turretWeapon = new WeaponTorpedo();
                this.muzzleOffsets = new Vector3[2]
                {
                    new Vector3(-0.5f, 0.4f, -2.7f),
                    new Vector3(0.5f,  0.4f, -2.7f),
                };
            }
        }

        public class TorpedoTurret : TurretData
        {
            public TorpedoTurret(Vector3 localOffset, Vector3 upAimVector, Vector3 defaultVector)
                : base()
            {
                this.localOffset = localOffset;
                this.upAimVector = upAimVector;
                this.defaultVector = defaultVector;

                this.modelName = ModelType.turretLong;
                this.turretWeapon = new WeaponTorpedo();
                this.muzzleOffsets = new Vector3[1]
                {
                    new Vector3(0, 0, -2.5f),
                };
            }
        }

        public class LongCannon : TurretData
        {
            public LongCannon(Vector3 localOffset, Vector3 upAimVector, Vector3 defaultVector)
                : base()
            {
                this.localOffset = localOffset;
                this.upAimVector = upAimVector;
                this.defaultVector = defaultVector;

                this.modelName = ModelType.turretLong;
                this.turretWeapon = new WeaponSwarmRocket();
                this.muzzleOffsets = new Vector3[1]
                {
                    new Vector3(0, 0, -2.5f),
                };
            }
        }


        public class BallTurret : TurretData
        {
            public BallTurret(Vector3 localOffset, Vector3 upAimVector, Vector3 defaultVector)
                : base()
            {
                this.localOffset = localOffset;
                this.upAimVector = upAimVector;
                this.defaultVector = defaultVector;

                this.modelName = ModelType.turretBall;
                this.turretWeapon = new WeaponMachineGun();
                this.muzzleOffsets = new Vector3[1]
                {
                    new Vector3(0, 0, -2.0f),
                };
            }
        }


        public class InvisibleBeamTurret : TurretData
        {
            public InvisibleBeamTurret(Vector3 localOffset, Vector3 upAimVector, Vector3 defaultVector)
                : base()
            {
                this.localOffset = localOffset;
                this.upAimVector = upAimVector;
                this.defaultVector = defaultVector;

                this.turretWeapon = new BeamGun();
                this.muzzleOffsets = new Vector3[1]
                {
                    new Vector3(0, 0, -0.0f),
                };
            }
        }


        public class BeamTurret : TurretData
        {
            public BeamTurret(Vector3 localOffset, Vector3 upAimVector, Vector3 defaultVector)
                : base()
            {
                this.localOffset = localOffset;
                this.upAimVector = upAimVector;
                this.defaultVector = defaultVector;

                this.modelName = ModelType.turretBall;
                this.turretWeapon = new BeamGun();
                this.muzzleOffsets = new Vector3[1]
                {
                    new Vector3(0, 0, -2.0f),
                };
            }
        }
    }    

    public class TurretData
    {
        public Weapon turretWeapon;
        public Vector3[] muzzleOffsets;
        public ModelType modelName;

        //these are specific to each ship.
        public Vector3 localOffset; //relative position to parent ship.
        public Vector3 upAimVector;  //the "up" vector for this turret's aim cone.
        public Vector3 defaultVector; //where turret aims when not firing.
    }
    #endregion



    public class ShipData
    {
        public float maxSpeed = 4f;
        public float turnSpeed = 0.05f;
        public string name = "Interceptor";
        public ModelType modelname;
        public CollisionSphere[] collisionSpheres;
        public int maxDamage = 500;
        public TurretData[] turretDatas;

        public int numChunks;
        public int shipLength;
        public int shipWidth;
        public int shipHeight;

        public float displayDistanceModifier;

        //Penetration values. Determines how strong the armor is.
        //LOW NUMBER = MORE POWERFUL ARMOR.
        public float[] armorPen = new float[3];   // 0 = Front/Top.   1 = Bottom.    2 = Rear.

        public ShipClass shipClass;
        public Rectangle iconRect;

        public DamageData damageData;

        public float selectionSphere;

        public int XPAmount;

        public ShipData(string Name, ModelType model, float MaxSpeed, float TurnSpeed, CollisionSphere[] spheres,
            int MaxDamage, TurretData[] turretDatas, float[] ArmorPen, ShipClass shipclass,
            int numChunks, int shipLength, int shipWidth, int shipheight, Rectangle icon,
            float displayMod, DamageData dmgData, float selectSphere, int xpamount)
        {
            this.maxDamage = MaxDamage;
            this.collisionSpheres = spheres;
            this.maxSpeed = MaxSpeed;
            this.turnSpeed = TurnSpeed;
            this.name = Name;
            this.modelname = model;
            this.turretDatas = turretDatas;
            this.armorPen = ArmorPen;
            this.shipClass = shipclass;

            this.numChunks = numChunks; //how much destructionChunks to generate.
            this.shipLength = shipLength;
            this.shipWidth = shipWidth;
            this.shipHeight = shipheight;
            this.iconRect = icon;
            this.displayDistanceModifier = displayMod;
            this.damageData = dmgData;
            this.selectionSphere = selectSphere;
            this.XPAmount = xpamount;
        }
    }

    public class DamageData
    {
        public Vector3[] damagePositions;
        public int damageThreshold;

        public DamageData(Vector3[] dmgPositions, int dmgThreshold)
        {
            this.damagePositions = dmgPositions;
            this.damageThreshold = dmgThreshold;
        }
    }
}