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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace SpaceShooter
{
    public enum ShipClass
    {
        Destroyer = 0,
        CapitalShip,
        Fighter
    }



    public class SpaceShipInput
    {
        public float PitchAngle;
        public float YawAngle;
        public float RollAngle;
        public float SpeedOffset;
        public bool Fired;

        public void Reset()
        {
            PitchAngle = YawAngle = RollAngle = SpeedOffset = 0.0f;
            Fired = false;
        }
    }

    public class SpaceShipAIData
    {
        // How often do we change what we are doing?
        public float DecisionInterval = 2.0f;
        public float CurrentDecisionTime = 0.0f;

        public float RollInput = 0.0f;
        public float PitchInput = 0.0f;
        public float YawInput = 0.0f;

        public float ThrustInput = 0.0f;
        public Random RandomNumber = new Random();

        public Vector3 targetPos = Vector3.Zero;
        public Quaternion targetRotation = Quaternion.Identity;

        public Collideable targetShip;
    }

    public class SpaceShipStats
    {
        public int kills = 0;
        public int missions = 0;


        public void AddMission()
        {
            missions++;
        }

        public void AddKill()
        {
            kills++;
        }
    }

    

    public class SpaceShip : Collideable
    {
        
        #region Fields
        // Ship Simulation data


        string captainName;
        public string CaptainName
        {
            get { return captainName; }
        }

        

        ShipData shipData;

        



        public ShipData Shipdata
        {
            get { return shipData; }
            set { shipData = value; }
        }

        public string shipName
        {
            get { return shipData.name; }
        }


        


        GameEffect orderEffect;
        public float orderTransition;


        Turret[] turrets;


  


        // Ship structure data
        float maxDamage = 500.0f;

        public float MaxDamage
        {
            get { return maxDamage; }
        }



        
        BoundingSphere selectionSphere;

        SpaceShipAIData spaceShipAIData;

        public Vector3 targetPos
        {
            get { return spaceShipAIData.targetPos; }
        }

        public Quaternion targetRotation
        {
            get { return spaceShipAIData.targetRotation; }
        }

        SpaceShipInput shipInput;

        


        ParticleManager particles;

        public ParticleManager Particles
        {
            get { return particles; }
        }

        //public ParticleEmitter emitter;
        public ParticleEmitter[] damageEmitter;

        float sizeInPixels;


        public float SizeInPixels
        {
            get { return sizeInPixels; }
        }
        
        protected bool drawHudBox = true;

        VertexBuffer hudVB;
        VertexDeclaration hudDecl;

        Effect hudEffect;
        EffectParameter screenSize;

        VertexPositionColor[] hudVertices;


        /// <summary>
        /// This is the data that stays consistent between all campaign encounters.
        /// </summary>
        public FleetShip fleetShipInfo;

        

        const int numHudLines = 4;

        public SpaceShipInput ShipInput
        {
            get { return shipInput; }
            set { shipInput = value; }
        }


        /// <summary>
        /// Position of the dot signifying where the ship will be at the end of the round.
        /// </summary>
        public Vector3 markPos = Vector3.Zero;
        public float markTimeLength = 0;
        public float markDist = 0;

        public bool markShouldDraw = false;


        public bool shouldAddVeterancy = false;




        public BoundingSphere SelectionSphere
        {
            get
            {
                selectionSphere.Center = Position;
                return selectionSphere;
            }
        }

        Cue engineCue = null;

        DamageData damageData;

#endregion

        public SpaceShip(Game game)
        {
        }

        public SpaceShip(Game game, ParticleManager particles, Commander owner, ShipData shipdata, FleetShip fleetShip)
        {
            Reset(Vector3.Zero);


            shipInput = new SpaceShipInput();
            spaceShipAIData = new SpaceShipAIData();
            this.shipData = shipdata;


            this.damageData = shipData.damageData;

            maxDamage = shipData.maxDamage;

            Health = maxDamage;

            IsDestroyed = false;
            this.particles = particles;
            
            //emitter = particles.CreateRockEmitter(Vector3.Zero);

            this.owner = owner;
            
            hudVertices = new VertexPositionColor[numHudLines + 1];
            for (int i = 0; i < numHudLines + 1; i++)
                hudVertices[i] = new VertexPositionColor(Vector3.Zero, owner.TeamColor);


            Matrix m = Matrix.CreateFromQuaternion(Rotation);
            spaceShipAIData.targetRotation = Rotation;


            if (shipData.turretDatas != null)
            {
                turrets = new Turret[shipData.turretDatas.Length];
                for (int i = 0; i < shipData.turretDatas.Length; i++)
                {
                    turrets[i] = new Turret(game, shipdata.turretDatas[i].modelName, this,
                        shipData.turretDatas[i].turretWeapon, shipData.turretDatas[i].localOffset,
                        shipData.turretDatas[i].muzzleOffsets,
                        shipData.turretDatas[i].defaultVector,
                        shipData.turretDatas[i].upAimVector);

                    turrets[i].ResetOrientation();
                }
            }

            if (fleetShip == null)
            {
                captainName = Helpers.GenerateShipName();
                this.fleetShipInfo = new FleetShip(); //dummy info.
            }
            else
            {
                captainName = fleetShip.captainName;
                this.fleetShipInfo = fleetShip;
            }

            audioEmitter = new IAudioEmitter();


        }

        public void Initialize()
        {
            CollisionSpheres = new CollisionSphere[shipData.collisionSpheres.Length];
            for (int i = 0; i < shipData.collisionSpheres.Length; i++)
            {
                CollisionSpheres[i] = new CollisionSphere(shipData.collisionSpheres[i].sphere, shipData.collisionSpheres[i].offset);
            }

            if (turrets != null)
                UpdateTurretPosition();
            
            UpdateCollisionSpheres();
            ResetTurretsOrientation();

            spaceShipAIData.targetRotation = Rotation;

        }

        ModelType hulkModel1;
        ModelType hulkModel2;

        public override void LoadContent()
        {
            modelMesh = shipData.modelname;
            hulkModel1 = ModelType.debrisHulk1;
            hulkModel2 = ModelType.debrisHulk2;

            BSphere = FrameworkCore.ModelArray[(int)modelMesh].Meshes[0].BoundingSphere;
            selectionSphere = FrameworkCore.ModelArray[(int)modelMesh].Meshes[0].BoundingSphere;

            selectionSphere.Radius = MathHelper.Clamp(selectionSphere.Radius * 3, 0.1f, 22);


            hudEffect = FrameworkCore.Game.Content.Load<Effect>(@"shaders\simplescreen");
            screenSize = hudEffect.Parameters["screenSize"];

            hudDecl = VertexPositionColor.VertexDeclaration;

            hudVB = new VertexBuffer(FrameworkCore.Graphics.GraphicsDevice, typeof(VertexPositionColor), numHudLines + 1, BufferUsage.None);
        }

        public void ClearMoveOrder()
        {
            spaceShipAIData.targetPos = Vector3.Zero;
        }

        public void ClearOrderEffect()
        {
            if (orderEffect != null)
            {
                orderEffect = null;
            }
        }

        private void PlayLoop()
        {
            if (engineCue != null)
                return;

            engineCue = FrameworkCore.audiomanager.Play3DCue(sounds.Engine.engine1, this.audioEmitter);
        }

        public void ForceKill()
        {
            if (this.IsDestroyed)
                return;

            this.Health = 0;
            DeathCheck(null);
        }

        //the ship's Brain.
        public void ActionThink(GameTime gameTime, List<Collideable> ships)
        {

#if DEBUG
            if (FrameworkCore.players[0].inputmanager.kbBackspaceJustPressed)
            {
                if (owner != null && owner.GetType() != typeof(PlayerCommander) && this.Health > 0)
                //if (owner != null && owner.GetType() == typeof(PlayerCommander) && this.Health > 0)
                {
                    this.Health = 0;
                    DeathCheck(null);
                }                 
            }
#endif
            PlayLoop();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool shouldUpdateCollision = false;

            if (HandleRotation(gameTime, dt))
                shouldUpdateCollision = true;

            //move toward targetposition.
            if (HandleVelocity(gameTime, dt))
                shouldUpdateCollision = true;

            if (shouldUpdateCollision)
            {
                //these calls are only made when the spaceship changes its position or rotation.
                if (turrets != null)
                    UpdateTurretPosition(); //update position of the turrets.

                UpdateCollisionSpheres(); //update the Collision Balls.
            }


            RepairUpdate(gameTime);

            if (turrets != null)
            {
                if (orderEffect != null)
                {
                    if (!orderEffect.canFire)
                    {
                        //turret brain is off. just make them point straight.
                        for (int i = 0; i < turrets.Length; i++)
                        {
                            turrets[i].ResetTurretOrientation();
                        }
                        return; //don't fire guns.
                    }
                }

                //Weapon Firing brain.

                UpdateTurretBrain(gameTime, ships);
            }
        }

        private bool HandleRotation(GameTime gameTime, float elapsedTime)
        {
            if (orderEffect != null)
            {
                if (!orderEffect.canRotate)
                    return false;
            }

            bool shouldUpdateCollision = false;
            //rotate toward the target orientation.
            Matrix rotateMatrix = Matrix.CreateFromQuaternion(Rotation);
            Matrix targetMatrix = Matrix.CreateFromQuaternion(spaceShipAIData.targetRotation);

            //find rotationdifference between ship rotation and target rotation.
            float rotationDifference = Vector3.Distance(rotateMatrix.Forward, targetMatrix.Forward);

            //only rotate if we're beyond the tolerance difference.
            if (rotationDifference >= 0.005f)
            {
                float adjustedTurnSpeed = GetRotationSpeed();

                shouldUpdateCollision = true;
                Rotation = Quaternion.Lerp(Rotation, spaceShipAIData.targetRotation,
                    adjustedTurnSpeed * elapsedTime);
            }


            return shouldUpdateCollision;
        }

        public float GetRotationSpeed()
        {
            float adjustedTurnSpeed = shipData.turnSpeed;

            if (orderEffect != null)
            {
                adjustedTurnSpeed *= orderEffect.rotationModifier;
            }

            return adjustedTurnSpeed;
        }

        public float GetSpeed()
        {
            float speed = shipData.maxSpeed;

            if (orderEffect != null)
            {
                speed *= orderEffect.speedModifier;
                speed = Math.Min(speed, 6 /*max flank speed*/);
            }

            //apply powerups.
            speed = Helpers.ApplySpeedModifier(this, speed);

            return speed;
        }

        private bool HandleVelocity(GameTime gameTime, float elapsedTime)
        {
            if (orderEffect != null)
            {
                if (!orderEffect.canMove)
                    return false;
            }

            bool shouldUpdateCollision = false;
            if (spaceShipAIData.targetPos != Vector3.Zero)
            {
                //we have a target destination. execute the move order.
                Vector3 moveDir = spaceShipAIData.targetPos - Position;

                if (moveDir.Length() > 0.5f) //we haven't reached destination yet.
                {
                    float adjustedMoveSpeed = GetSpeed();

                    //move along path.
                    moveDir.Normalize();
                    Position += moveDir * adjustedMoveSpeed * elapsedTime;

                    shouldUpdateCollision = true;

                    //update particle emitter.
                    //emitter.Update(gameTime, position);                    
                }
                else if (spaceShipAIData.targetPos != Vector3.Zero)
                {
                    //arrived at location.
                    spaceShipAIData.targetPos = Vector3.Zero;

                    if (orderEffect != null)
                    {
                        if (orderEffect.name == Resource.OrderFlank ||
                            orderEffect.name == Resource.OrderMove)
                        {
                            orderEffect = null;
                        }
                    }


                    //ship has stopped moving. make a new move event.
                    FrameworkCore.playbackSystem.UpdateItem(this);
                }
            }

            //damage particle effects.
            if (damageEmitter != null && !IsDestroyed)
            {
                for (int i = 0; i < damageEmitter.Length; i++)
                {
                    //get the position of the damage emitter.
                    Vector3 smokeVec = this.Position;
                    Matrix rotationMatrix = Matrix.CreateFromQuaternion(Rotation);

                    //get the muzzle vector.
                    smokeVec +=
                        (rotationMatrix.Right * damageData.damagePositions[i].X) +
                        (rotationMatrix.Up * damageData.damagePositions[i].Y) +
                        (rotationMatrix.Forward * -damageData.damagePositions[i].Z);

                    damageEmitter[i].Update(gameTime, smokeVec);
                }
            }

            return shouldUpdateCollision;
        }






        private void RepairUpdate(GameTime gameTime)
        {
            if (IsDestroyed)
                return;

            if (orderEffect != null)
            {
                if (orderEffect.repairRate > 0)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                        TimeSpan.FromMilliseconds(orderEffect.repairRate).TotalMilliseconds);

                    this.Health = MathHelper.Clamp(this.Health + delta, 0, this.maxDamage);
                }
            }

            this.Health = Helpers.ApplyRepairModifier(gameTime, this);
        }

        //update the targeting systems.
        private void UpdateTurretBrain(GameTime gameTime, List<Collideable> ships)
        {
            for (int i = 0; i < turrets.Length; i++)
            {
                turrets[i].Update(gameTime, ships);
            }
        }

        private void UpdateTurretPosition()
        {
            for (int i = 0; i < turrets.Length; i++)
            {
                turrets[i].UpdatePosition();
            }
        }




        public override void Update(GameTime gameTime)
        {
            if (owner.selectedShip != this && owner.hoverShip == this)
            {
                //I am selected.
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(400).TotalMilliseconds);

                hoverTransition = MathHelper.Clamp(hoverTransition + delta, 0, 1);
            }
            else
            {
                //not selected.
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                hoverTransition = MathHelper.Clamp(hoverTransition - delta, 0, 1);
            }

            if (orderTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                    TimeSpan.FromMilliseconds(800).TotalMilliseconds);

                orderTransition = MathHelper.Clamp(orderTransition + delta, 0, 1);
            }

            base.Update(gameTime);
        }

        public float hoverTransition = 0;



        public void Reset(Vector3 pos)
        {
            Position = pos;
            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(210));
            

            
        }

        public void ResetAngle(float angle)
        {
            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(angle));
        }

        private void ResetTurretsOrientation()
        {
            if (turrets != null)
            {
                for (int i = 0; i < turrets.Length; i++)
                {
                    turrets[i].ResetOrientation();
                }
            }
        }


        private void HandleVeterancy(SpaceShip killer)
        {
            if (killer.shouldAddVeterancy)
                return;

            if (killer.owner == null)
                return;

            //npcs don't get veterancy.
            if (killer.owner.GetType() != typeof(PlayerCommander))
                return;

            if (!FrameworkCore.isCampaign)
                return;

            killer.shouldAddVeterancy = true;
            /*
            killer.fleetShipInfo.veterancy++;

            FrameworkCore.level.messageQueue.AddVeterancyMessage(
                killer.captainName, killer.owner.ShipColor);

            FrameworkCore.PlayCue(sounds.Fanfare.veterancy);
             */
        }

        private float VeterancyDamage(int veterancyLevel, float baseDamage)
        {
            return (veterancyLevel * 0.1f) * baseDamage;
        }

        private float VeterancyArmor(int veterancyLevel)
        {
            return (veterancyLevel * 0.01f);
        }


        //Projectile Collision.
        public override void Hit(Bolt bolt)
        {
            if (IsDestroyed)
                return;

            if (bolt.Owner != null)
            {
                if (bolt.Owner.owner != null)
                {
                    if (bolt.Owner.owner.factionName == this.owner.factionName)
                    {
                        //friendly fire. ignore.
                        particles.CreateMissileDeflect(bolt.Position, bolt.Velocity);
                        FrameworkCore.audiomanager.Play3DCue(sounds.Impact.deflect, this.audioEmitter);
                        return;
                    }
                }
            }

            if (bolt.Owner != null)
            {
                //reward the Shooter with a veterancy upgrade.
                HandleVeterancy(bolt.Owner);
            }


            Vector3 accurateHitPos = Helpers.ShipMeshHit(bolt.Position, bolt.Velocity, this);


            Matrix m = Matrix.CreateFromQuaternion(Rotation);
            float forwardDot = Helpers.GetDot(this.Position, accurateHitPos, m.Forward);  //forward dotproduct
            float upDot = Helpers.GetDot(this.Position, accurateHitPos, m.Up);   //up dotproduct
            float distanceToCenter = Vector3.Distance(this.Position, accurateHitPos); //distance of: BulletCollision to ship center.

            float shipQuarterLength = this.BSphere.Radius / 2.0f;  //ship's full length, divided by four.
            float shipThreeQtrLength= this.BSphere.Radius * 0.75f;  //3/4 of ship's length.

            float locationPenModifier = 1;
            int locationHit = 0;





            if (isFrontBoltAngle(m, bolt.Rotation))
            {
                //if the bolt angle is facing the front/top armor, then deflect it.
                locationPenModifier = this.shipData.armorPen[(int)locationType.Top];
                locationHit = (int)locationType.Top;
            }
            else if (forwardDot < 0 && distanceToCenter >= shipQuarterLength && isValidRearBolt(m, bolt.Rotation))
            {
                //REAR
                locationPenModifier = this.shipData.armorPen[(int)locationType.Rear];
                locationHit = (int)locationType.Rear;
            }
            else if (upDot < 0 && isValidBottomBolt(m, bolt.Rotation) &&
                ((forwardDot <= 0 /*Rear*/) || (forwardDot > 0 /*Front*/ && distanceToCenter <= shipThreeQtrLength)))
            {
                //BOTTOM
                locationPenModifier = this.shipData.armorPen[(int)locationType.Bottom];
                locationHit = (int)locationType.Bottom;
            }
            else
            {
                //TOP/FRONT
                locationPenModifier = this.shipData.armorPen[(int)locationType.Top];
                locationHit = (int)locationType.Top;
            }






            //calculate penetration.
            float finalPenetration =
                bolt.PrjData.basePenetration *
                locationPenModifier *
                bolt.PrjData.classModifiers[(int)this.shipData.shipClass, 1];


            finalPenetration = Helpers.ApplyArmorModifier(this, finalPenetration, (int)locationHit);
            finalPenetration = HeroArmorCheck(finalPenetration);
            finalPenetration += VeterancyArmor(fleetShipInfo.veterancy);


            


            //do penetration dice roll check. finalPen = 0.0 - 1.0. Do dice roll to see if finalPen >= randomNumber.
            // High Final Penetration = more likely to penetrate the armor. 0.0 = weak, 1.0 = strong.
            if (finalPenetration < (float)FrameworkCore.r.NextDouble())
            {
                //fail! do deflection.
                particles.CreateMissileDeflect(accurateHitPos, bolt.Velocity);

                FrameworkCore.audiomanager.Play3DCue(sounds.Impact.deflect, this.audioEmitter);

                if (bolt.PrjData.debrisDeflecter)
                    FrameworkCore.debrisManager.AddRocketDeflect(bolt.modelMesh, accurateHitPos, -bolt.Velocity);

                FrameworkCore.playbackSystem.AddDeflection(accurateHitPos);

                return;
            }

            

            //penetration successful. do damage routine.
            if (bolt.Owner != null)
            {
                if (bolt.Owner.owner != null)
                {
                    if (bolt.Owner.owner.GetType() == typeof(PlayerCommander))
                    {
                        int xpAmount = 8; //bottom hit.

                        if (locationHit == (int)locationType.Rear)
                            xpAmount = 12;  //rear hit.

                        //a player shot this bolt.
                        FrameworkCore.players[0].AddExperience(xpAmount);
                    }
                }
            }



            //do the damage messages.
            if (locationHit == (int)locationType.Rear)
                FrameworkCore.WorldtextManager.RearArmorHit(bolt.Position, -bolt.Velocity);
            else if (locationHit == (int)locationType.Bottom)
                FrameworkCore.WorldtextManager.BottomArmorHit(bolt.Position, -bolt.Velocity);
            else
                FrameworkCore.WorldtextManager.ArmorHit(bolt.Position, -bolt.Velocity);


            particles.CreateMissileHit(accurateHitPos, bolt.Velocity);

            FrameworkCore.audiomanager.Play3DCue(sounds.Explosion.tiny, this.audioEmitter);
            FrameworkCore.debrisManager.AddMissleHitDebris(accurateHitPos, -bolt.Velocity);



            //play a Ding sound.
            PlayDingSound(bolt);




            FrameworkCore.playbackSystem.AddExplosion(accurateHitPos);

            float adjustedBoltDamage = MathHelper.Lerp(bolt.PrjData.minDamage, bolt.PrjData.maxDamage,
                (float)FrameworkCore.r.NextDouble());

            try
            {
                if (FrameworkCore.isCampaign && FrameworkCore.isHardcoreMode &&
                    bolt.Owner != null && bolt.Owner.GetType() != typeof(PlayerCommander))
                {
                    //enemies get bonus dmg in hardcore mode.
                    adjustedBoltDamage *= Helpers.randFloat(1.1f, 1.3f);
                }
            }
            catch
            {
            }

            if (adjustedBoltDamage > 0)
            {
                //add veterancy bonus damage.
                if (bolt.Owner.fleetShipInfo != null)
                {
                    if (bolt.Owner.fleetShipInfo.veterancy > 0)
                    {
                        adjustedBoltDamage += VeterancyDamage(bolt.Owner.fleetShipInfo.veterancy, bolt.PrjData.minDamage);
                    }
                }

                this.Health -= adjustedBoltDamage;
            }

            if (DeathCheck(bolt.Owner.owner))
            {
                //ship is dead.
                if (bolt.Owner.fleetShipInfo != null)
                    bolt.Owner.fleetShipInfo.stats.AddKill();
            }
        }


        private void PlayDingSound(Bolt bolt)
        {
            if (FrameworkCore.HideHud)
            {
                return;
            }

            if (FrameworkCore.level.isDemo)
                return;

            //sanity check.
            if (bolt.Owner == null)
                return;

            if (bolt.Owner.owner == null)
                return;

            //if an NPC owns me, then bail.
            if (bolt.Owner.owner.GetType() != typeof(PlayerCommander))
                return;

            //don't play dings if players are on opposing teams.
            if (!FrameworkCore.isCampaign && FrameworkCore.players.Count > 1)
            {
                if (FrameworkCore.players[0].factionName != FrameworkCore.players[1].factionName)
                    return;
            }

            //play the sound.
            FrameworkCore.PlayCue(sounds.Fanfare.ding);
        }

        private void PlayDingSound(Collideable killer)
        {
            if (FrameworkCore.HideHud)
            {
                return;
            }

            if (FrameworkCore.level.isDemo)
                return;

            //sanity check.
            if (killer.owner == null)
                return;

            //if an NPC owns me, then bail.
            if (killer.owner.GetType() != typeof(PlayerCommander))
                return;

            //don't play dings if players are on opposing teams.
            if (!FrameworkCore.isCampaign && FrameworkCore.players.Count > 1)
            {
                if (FrameworkCore.players[0].factionName != FrameworkCore.players[1].factionName)
                    return;
            }

            //play the sound.
            FrameworkCore.PlayCue(sounds.Fanfare.ding);
        }

        /// <summary>
        /// Checks if the bolt is facing toward the front/top
        /// </summary>
        /// <param name="shipMatrix"></param>
        /// <param name="boltQuaternion"></param>
        /// <returns></returns>
        bool isFrontBoltAngle(Matrix shipMatrix, Quaternion boltQuaternion)
        {
            Matrix boltMatrix = Matrix.CreateFromQuaternion(boltQuaternion);

            float frontAngle = Helpers.UnsignedAngleBetweenTwoV3(shipMatrix.Forward, boltMatrix.Forward);

            float topAngle = Helpers.UnsignedAngleBetweenTwoV3(shipMatrix.Up, boltMatrix.Forward);


            if (frontAngle > 1.5f && topAngle > 1.5f)
            {
                return true;
            }           

            return false;            
        }

        /// <summary>
        /// Determines if bolt angle is perpendicular or less than ship's forward facing.
        /// </summary>
        /// <param name="bolt"></param>
        /// <returns></returns>
        bool isValidRearBolt(Matrix shipMatrix, Quaternion boltQuaternion)
        {
            Matrix boltMatrix = Matrix.CreateFromQuaternion(boltQuaternion);
            //float rearAimDot = Vector3.Dot(boltMatrix.Forward, shipMatrix.Forward);
            float angle = Helpers.UnsignedAngleBetweenTwoV3(shipMatrix.Forward, boltMatrix.Forward);

            return (angle < 1.5f);
        }


        bool isValidBottomBolt(Matrix shipMatrix, Quaternion boltQuaternion)
        {
            Matrix boltMatrix = Matrix.CreateFromQuaternion(boltQuaternion);
            //float rearAimDot = Vector3.Dot(boltMatrix.Forward, shipMatrix.Forward);
            float angle = Helpers.UnsignedAngleBetweenTwoV3(shipMatrix.Up, boltMatrix.Forward);

            return (angle < 1.5f);
            //return (angle < 1.5f);
        }

        /// <summary>
        /// We cheat a little here for the player. When the player ship is very low on health, the ship magically becomes more resistant to taking damage.
        /// </summary>
        /// <returns></returns>
        private float HeroArmorCheck(float basePenetration)
        {
            

            //this is the threshold for "low health"
            if (this.Health > 64)
                return basePenetration;

            if (this.owner == null)
                return basePenetration;

            //NPCs are not allowed this bonus.
            if (this.owner.GetType() != typeof(PlayerCommander))
                return basePenetration;

            //only in campaign mode.
            if (FrameworkCore.worldMap == null)
                return basePenetration;

            //ok now we have a player controlled ship. do a dice roll to see if the player gets a "lucky save".
            if (FrameworkCore.r.Next(4) > 0)
                return Math.Min(basePenetration, 0.0f);

            //player failed the lucky save check.
            return basePenetration;
        }

        public override void beamHit(Collideable killer, Vector3 hitPos, int minDmg, int maxDmg, Vector3 muzzleVec)
        {
            if (killer.owner.factionName == this.owner.factionName)
            {
                //friendly fire.
                return;
            }


            float adjustedBoltDamage = Helpers.randFloat(minDmg, maxDmg);

            if (killer.owner != null)
            {
                if (killer.GetType() == typeof(SpaceShip))
                {
                    //reward shooter with a veterancy upgrade.
                    HandleVeterancy((SpaceShip)killer);

                    //handle veterancy bonus damage.
                    adjustedBoltDamage += VeterancyDamage(((SpaceShip)killer).fleetShipInfo.veterancy, minDmg);
                }
            }


            adjustedBoltDamage = Helpers.ApplyBeamModifier(this, adjustedBoltDamage);


            PlayDingSound(killer);

            Health -= adjustedBoltDamage;

            Vector3 hitDir = hitPos - killer.Position;
            Vector3 explosionPos = Helpers.ShipMeshHit(muzzleVec, hitDir, this);
            particles.CreateMissileHit(explosionPos, hitDir);

            FrameworkCore.playbackSystem.AddBeamHit(hitPos, muzzleVec, killer.owner.TeamColor);


            if (killer.owner != null)
            {
                
                if (killer.owner.GetType() == typeof(PlayerCommander))
                {
                    int xpAmount = 7;
                    FrameworkCore.players[0].AddExperience(xpAmount);
                }                
            }


            if (DeathCheck(killer.owner))
            {
                if (!Helpers.IsSpaceship(killer))
                    return;

                ((SpaceShip)killer).fleetShipInfo.stats.AddKill();
            }
        }








        //Ship Collision.
        public override void Hit(Collideable ship, Vector3 intersectPoint)
        {
            if (IsDestroyed)
                return;

            Health -= FrameworkCore.r.Next(64, 128);
            ship.Health -= FrameworkCore.r.Next(64, 128);


            particles.CreatePlanetExplosion(intersectPoint, Vector3.Zero);

            FrameworkCore.playbackSystem.AddExplosion(intersectPoint);

            if (DeathCheck(ship.owner))
            {
                if (!Helpers.IsSpaceship(ship))
                    return;

                //ship is dead.
                SpaceShip killerShip = (SpaceShip)ship;
                killerShip.fleetShipInfo.stats.AddKill();
            }
        }

        private void DamageEffectCheck()
        {
            if (this.damageData == null)
                return;

            if (damageEmitter != null)
                return;

            if (this.Health > this.damageData.damageThreshold)
                return;

            damageEmitter = new ParticleEmitter[damageData.damagePositions.Length];

            for (int i = 0; i < damageData.damagePositions.Length; i++)
            {
                damageEmitter[i] = FrameworkCore.Particles.CreateDamageSmokeEmitter(this.Position);
            }
        }

        public void PauseBeamSounds()
        {
            if (turrets != null)
            {
                for (int i = 0; i < turrets.Length; i++)
                {
                    turrets[i].PauseBeamSounds();
                }
            }
        }

        private bool DeathCheck(Commander killer)
        {
            //do the "damage effect" check here.
            DamageEffectCheck();

            if (this.Health > 0)
                return false;


            //turn off turret sound effects.
            if (turrets != null)
            {
                for (int i = 0; i < turrets.Length; i++)
                {
                    turrets[i].StopBeamSounds();
                }
            }

            Helpers.SetRumbleExplosion();

            if (killer != null)
            {
                killer.roundKills++;

                if (killer.GetType() == typeof(PlayerCommander))
                {
                    FrameworkCore.players[0].AddExperience(this.shipData.XPAmount);
                    
                    //killer is a player. add the points for killing an enemy.
                    FrameworkCore.players[0].extraPoints += Helpers.ENEMYKILLEDPOINTS;                    
                }
                else
                {
                    FrameworkCore.players[0].AddExperiencePlayerDeath(this.shipData.XPAmount);
                }
            }
            else
            {
                if (this.owner != null)
                {
                    if (this.owner.GetType() == typeof(PlayerCommander))
                    {
                        FrameworkCore.players[0].AddExperiencePlayerDeath(this.shipData.XPAmount);
                    }
                    else
                    {
                        FrameworkCore.players[0].AddExperience(this.shipData.XPAmount);
                    }
                }
            }

            FrameworkCore.level.messageQueue.AddMessage(captainName, owner.ShipColor);

            if (engineCue != null && engineCue.IsPlaying)
            {
                engineCue.Stop(AudioStopOptions.AsAuthored);
                engineCue = null;
            }

            FrameworkCore.playbackSystem.KillItem(this);

            //at zero health, start death sequence.
            IsDestroyed = true;

            Matrix facing = Matrix.CreateFromQuaternion(Rotation);
            Vector3 velocity = facing.Forward;

            particles.CreateNova(Position, velocity);

            FrameworkCore.audiomanager.Play3DCue(sounds.Explosion.big, this.audioEmitter);
            


            FrameworkCore.hulkManager.GenerateShipHulk(this, shipData, hulkModel1, hulkModel2);
           
            return true;
        }



        



        public Quaternion GetTargetOrientation()
        {
            return spaceShipAIData.targetRotation;
        }

        public Vector3 GetTargetPos()
        {
            return spaceShipAIData.targetPos;
        }

        public void SetTargetMove(Vector3 pos, Quaternion targetRot)
        {
            spaceShipAIData.targetPos = pos;
            spaceShipAIData.targetRotation = targetRot;
        }


        public Collideable targetShip
        {
            get { return spaceShipAIData.targetShip; }
        }

        public void SetTargetShip(Collideable ship)
        {
            spaceShipAIData.targetShip = ship;
        }

        int blinkTime = 0;
        bool blinkOn = false;

        private void UpdateHeadsUpColor()
        {
            for (int i = 0; i < numHudLines + 1; i++)
            {
                Color lineColor = owner.TeamColor;

                if (this == owner.selectedShip)
                    lineColor = Color.White;

                if (hudVertices[i].Color == lineColor)
                    return;

                hudVertices[i] = new VertexPositionColor(Vector3.Zero, lineColor);
            }
        }

        public virtual void DrawHeadsUpDisplay(GameTime gameTime, Camera camera, int blinkInterval, bool drawBox, float helpOverlay)
        {
            if (blinkInterval > 0)
            {
                blinkTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (blinkTime > blinkInterval)
                {
                    blinkTime = 0;
                    blinkOn = !blinkOn;
                }
                

                if (!blinkOn)
                    return;
            }

            UpdateHeadsUpColor();


            if (IsVisible(camera) && drawHudBox)
            {
                // Draw ship as a dot with a text tag in the distance
                Matrix viewProj = camera.View * camera.Projection;
                Vector4 screenPos = Vector4.Transform(Position, viewProj);

                Vector2 screenViewport = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

                float halfScreenY = screenViewport.Y / 2.0f;
                float halfScreenX = screenViewport.X / 2.0f;

                float screenY = ((screenPos.Y / screenPos.W) * halfScreenY) + halfScreenY;
                float screenX = ((screenPos.X / screenPos.W) * halfScreenX) + halfScreenX;
                if (drawBox)
                {
                    hudVertices[0].Position.X = screenX - sizeInPixels;
                    hudVertices[0].Position.Y = screenY - sizeInPixels / 2;

                    hudVertices[1].Position.X = screenX + sizeInPixels;
                    hudVertices[1].Position.Y = screenY - sizeInPixels / 2;

                    hudVertices[2].Position.X = screenX + sizeInPixels;
                    hudVertices[2].Position.Y = screenY + sizeInPixels / 2;

                    hudVertices[3].Position.X = screenX - sizeInPixels;
                    hudVertices[3].Position.Y = screenY + sizeInPixels / 2;

                    hudVertices[4].Position.X = screenX - sizeInPixels;
                    hudVertices[4].Position.Y = screenY - sizeInPixels / 2;

                    hudVB.SetData<VertexPositionColor>(hudVertices);

                    screenSize.SetValue(screenViewport);
                    hudEffect.Techniques[0].Passes[0].Apply();

                    FrameworkCore.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;

                    FrameworkCore.Graphics.GraphicsDevice.SetVertexBuffer(hudVB);

                    FrameworkCore.Graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, numHudLines);

                    FrameworkCore.Graphics.GraphicsDevice.SetVertexBuffer(null);

                    FrameworkCore.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                }


                

                if (hoverTransition > 0)
                {
                    if (owner.GetType() != typeof(PlayerCommander))
                        return;

                    Vector2 cursorPos = ((PlayerCommander)owner).CursorPos;

                    //splitscreen.
                    if (FrameworkCore.players.Count > 1)
                        cursorPos.X *= 0.5f;

                    Vector2 shipPos = Helpers.GetScreenPos(camera, this.Position);
                    


                    bool shouldReverse = false;

                    if (CollisionSpheres.Length > 1)
                        shouldReverse = true;

                    
                    DrawBubbleButton(shipPos, cursorPos, sprite.buttons.a, hoverTransition, shouldReverse);
                }
            }
        }



        private void DrawHealthBar(Camera camera)
        {
            if (FrameworkCore.HideHud)
            {
                return;
            }

            //don't draw healthbar if it's ultra tiny.
            if (this.maxDamage < 100)
                return;

            //draw health bar.
            int barHeight = 8;
            int borderWidth = 2;

            Vector2 barPos = Helpers.GetScreenPos(camera, this.Position);
            barPos.Y -= Helpers.SizeInPixels(camera, this.Position, this.BSphere.Radius) / 2;
            barPos.Y -= barHeight;

            Color barColor = owner.TeamColor;

            if (owner.selectedShip == this)
                barColor = Color.White;

            Helpers.DrawBar(barPos, Health / maxDamage, (int)(maxDamage / 10), barHeight, borderWidth, barColor, new Color(0, 0, 0, 128));

            //debug text.
            /*
            if (owner.GetType() != typeof(PlayerCommander))
                return;
            
            Helpers.DrawOutline("markdraw: " + markShouldDraw + 
                "\ncurrentMovePos: " + ((PlayerCommander)owner).CurrentMovePos + 
                "\nship.targetPos: " + targetPos, barPos);            */
        }
        
        float bubbleDisplayAngle = 0;
        private void DrawBubbleButton(Vector2 targetPos, Vector2 originPos, Rectangle buttonRect, float transition, bool isReversed)
        {
            if (owner.GetType() == typeof(PlayerCommander))
            {
                if (((PlayerCommander)owner).mouseEnabled)
                    return;
            }


            Color bubbleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, transition);
            float buttonSize = 1;
            float bubbleSize = 1;

            if (owner.hoverShip == this)
            {
                buttonSize = MathHelper.Lerp(2, 1, transition);
            }
            else
            {
                //deselecting.
                buttonSize = MathHelper.Lerp(0, 1, transition);
                bubbleSize = MathHelper.Lerp(0, 1, transition);
            }

            Vector2 direction = targetPos - originPos;

            if (isReversed)
                direction = originPos - targetPos;

            direction.Normalize();
            float adjacent = direction.X;
            float opposite = direction.Y;
            float TargetAngle = (float)System.Math.Atan2(opposite, adjacent);
            bubbleDisplayAngle = Helpers.TurnToFace(bubbleDisplayAngle, TargetAngle, 0.1f);

            if (isReversed)
                targetPos = originPos;

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, targetPos, sprite.bubble, bubbleColor,
                bubbleDisplayAngle, new Vector2(0, 16), bubbleSize, SpriteEffects.None, 0);

            float TagAngle = bubbleDisplayAngle;
            float x = (float)(Math.Cos(TagAngle) * (48 * bubbleSize));
            float y = (float)(Math.Sin(TagAngle) * (48 * bubbleSize));
            Vector2 TagPos = new Vector2(targetPos.X + x, targetPos.Y + y);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, TagPos, buttonRect, bubbleColor,
                0, Helpers.SpriteCenter(buttonRect), buttonSize, SpriteEffects.None, 0);

            if (transition > 0)
            {
                Color flashColor = Color.Lerp(Color.White, OldXNAColor.TransparentWhite, transition);
                if (owner.hoverShip != this)
                {
                    flashColor = OldXNAColor.TransparentWhite;
                }
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, TagPos, sprite.buttons.blank, flashColor,
                    0, Helpers.SpriteCenter(buttonRect), buttonSize, SpriteEffects.None, 0);
            }
        }

        

        public GameEffect OrderEffect
        {
            get { return orderEffect; }
        }


        GameEffect lastOrderEffect = null;
        public GameEffect LastOrderEffect
        {
            get { return lastOrderEffect; }
        }

        public void RevertToLastOrderEffect()
        {
            if (lastOrderEffect != null)
            {
                //revert back to previous order.
                orderEffect = lastOrderEffect;
                lastOrderEffect = null;
            }
            else
            {
                //there is no last order to revert to. clear out everything.
                orderEffect = null;
            }
        }

        public void ApplyEffect(GameEffect gameEffect)
        {
            if (orderEffect != null)
            {
                //save a reference of the current order.
                lastOrderEffect = orderEffect;
            }

            orderEffect = gameEffect;
        }


        public override void Draw(GameTime gameTime, Camera camera, PlayerCommander curPlayer)
        {
            if (!IsDestroyed /*&& IsVisible(camera)*/)
            {
                float distance = (Position - camera.CameraPosition).Length();
                sizeInPixels = 10.0f;
                float radius = FrameworkCore.ModelArray[(int)modelMesh].Meshes[0].BoundingSphere.Radius;
                if (distance > radius)
                {
                    float angularSize = (float)Math.Tan(radius / distance);
                    sizeInPixels = angularSize * FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / camera.FieldOfView;
                }

                if (sizeInPixels > 0.0f)
                {
                    Matrix viewProj = camera.View * camera.Projection;
                    Vector4 screenPos = Vector4.Transform(Position, viewProj);

                    Vector2 screenViewport = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);
                    
                    Matrix worldMatrix = Matrix.CreateFromQuaternion(Rotation);
                    worldMatrix.Translation = Position;

                    if (IsVisible(camera))
                    {
                        float pulseWidth = 0.15f + (float)(0.05f * Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * 0.003));
                        float celOutlineWidth = MathHelper.Lerp(0.06f, pulseWidth, hoverTransition);
                        Color celOutlineColor = Color.Lerp(Color.Black, Color.White, hoverTransition);

                        /*
                        SpaceShooter.SpaceShooterGame.DrawModel(shipModel, worldMatrix, camera,
                            celOutlineColor, celOutlineWidth, owner.ShipColor);
                        */

                        FrameworkCore.meshRenderer.Draw(modelMesh, worldMatrix, camera, owner.ShipColor);
                    }

                    if (turrets != null)
                    {
                        for (int i = 0; i < turrets.Length; i++)
                        {
                            turrets[i].Draw(gameTime, camera);
                        }
                    }
                    


                    if (FrameworkCore.level.isDemo)
                        return;

                    if (!FrameworkCore.HideHud)
                    {
                        //RODS.
                        Vector3 endVec = Position + new Vector3(0, curPlayer.GridAltitude - Position.Y, 0);

                        Color lineColor = owner.TeamColor;

                        if (this == owner.selectedShip)
                            lineColor = Color.White;

                        FrameworkCore.lineRenderer.Draw(Position, endVec, lineColor);

                        //the plane disc has translucent alpha.
                        if (this != owner.selectedShip)
                            lineColor.A = 128;

                        FrameworkCore.discRenderer.Draw(BSphere.Radius, endVec, lineColor);
                    }



#if DEBUG
                    if (FrameworkCore.debugMode)
                    {
                        for (int i = 0; i < CollisionSpheres.Length; i++)
                        {
                            BoundingSphere sphere = CollisionSpheres[i].sphere;
                            FrameworkCore.sphereRenderer.Draw(sphere, Matrix.Identity, Color.Orange);
                        }
                    }
#endif
                    
                }
            }

        }

        public void DrawShipIcons(GameTime gameTime, Camera camera, PlayerCommander player, bool isActionMode)
        {
            if (IsDestroyed)
                return;

            if (!IsVisible(camera))
                return;

            //don't draw icons in the playback mode.
            if (player.IsPlaybackMode())
                return;

            if (player == owner || isActionMode)
                DrawStatusIcon(gameTime, camera);


            DrawHealthBar(camera);
        }

        private void DrawStatusIcon(GameTime gameTime, Camera camera)
        {
            if (FrameworkCore.HideHud)
            {
                return;
            }

            if (orderEffect == null)
                return;

            if (!IsVisible(camera))
                return;

            FrameworkCore.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            /*
            Vector3 iconPos = this.position + new Vector3(0, 5, 0);

            Vector3 screenPos = FrameworkCore.Graphics.GraphicsDevice.Viewport.Project(
                                iconPos, camera.Projection, camera.View,
                                Matrix.Identity);
            */

            float nearRange = 60;
            float farRange = 150;
            float movePosDist = Vector3.Distance(this.Position, camera.CameraPosition);
            float distTransition = movePosDist - nearRange;
            distTransition = MathHelper.Clamp(distTransition, 0, farRange);
            distTransition /= farRange;
            distTransition = MathHelper.Clamp(distTransition, 0, 1);
            float iconSize = MathHelper.Lerp(1.0f, 0.4f, distTransition);

            Vector2 iconPos = Helpers.GetScreenPos(camera, this.Position);

            

            iconPos.Y -= Helpers.SizeInPixels(camera, this.Position, this.BSphere.Radius) / 2;
            iconPos.Y -= 10; //gap.
            iconPos.Y -= (sprite.icons.circle.Height / 2) * iconSize;




            //the big circle animation.
            if (orderTransition < 1)
            {
                //iconSize = MathHelper.Lerp(iconSize * 3f, iconSize, orderTransition);
                iconSize = Helpers.PopLerp(orderTransition, iconSize * 5f, iconSize * 0.5f, iconSize);
                float glowSize = MathHelper.Lerp(1.5f, 0.01f, orderTransition);


                Color glowColor = Color.Lerp(Color.White, new Color(255,255,255,128), orderTransition);
                float glowAngle = (float)gameTime.TotalGameTime.TotalSeconds * 2f;
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, new Vector2(iconPos.X, iconPos.Y),
                    sprite.bigCircle, glowColor, glowAngle, Helpers.SpriteCenter(sprite.bigCircle), glowSize,
                    SpriteEffects.None, 0);                
            }


            float circleAngle = (float)gameTime.TotalGameTime.TotalSeconds;

            Rectangle iconRect = sprite.icons.circleDotted;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, new Vector2(iconPos.X, iconPos.Y),
                iconRect, owner.TeamColor, circleAngle, Helpers.SpriteCenter(iconRect), iconSize,
                SpriteEffects.None, 0);

            iconRect = orderEffect.iconRect;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, new Vector2(iconPos.X, iconPos.Y),
                iconRect, Color.White, 0, Helpers.SpriteCenter(iconRect), iconSize,
                SpriteEffects.None, 0);
        }


    }
}