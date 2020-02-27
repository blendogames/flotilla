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
using Microsoft.Xna.Framework.Audio;
#endregion

namespace SpaceShooter
{
    public class Turret
    {
        Vector3 position;     //worldPosition
        Quaternion rotateQuat;  //rotation
        Quaternion targetRotateQuat;  //this is the turret's desired rotation.

        Vector3 localOffset;  //position of turret relative to its parent ship.

        Weapon turretWeapon;   //what weapon is assigned to this turret.

        ModelType turretModel;     //mesh.

        Vector3[] muzzleOffsets;

        Vector3 defaultVector = Vector3.Forward;
        Vector3 upAimVector = Vector3.Forward;


        const float rotateSpeed = 0.05f;

        public ModelType TurretModel
        {
            get { return turretModel; }
        }

        public SpaceShip parentShip;

        private Vector3 GetMuzzleVec(int barrelNumber)
        {
            //clamp to max barrel count.
            barrelNumber = Math.Min(barrelNumber, muzzleOffsets.Length - 1);

            Matrix turretOrientation = Matrix.CreateFromQuaternion(rotateQuat); //turret orientation
            Vector3 muzzleVec = position;

            //get the muzzle vector.
            muzzleVec +=
                (turretOrientation.Right * muzzleOffsets[barrelNumber].X) +
                (turretOrientation.Up * muzzleOffsets[barrelNumber].Y) +
                (turretOrientation.Forward * -muzzleOffsets[barrelNumber].Z);

            return muzzleVec;
        }

        private Vector3 GetPredictedPosition(GameTime gameTime, Vector3 muzzleVec, SpaceShip enemyShip)
        {
            Vector3 predictedPosition = Vector3.Zero;

            //Beam weapons do not predict ahead of the enemy ship.
            if (turretWeapon.firetype == Weapon.fireType.Beam)
                return enemyShip.Position;
            

            if (enemyShip.targetPos != Vector3.Zero)
            {
                //predict where the ship is going to be.

                //get what direction the targetship is moving.
                Vector3 targetMovedir = enemyShip.targetPos - enemyShip.Position;

                if (targetMovedir.Length() < 1f || enemyShip.targetPos == Vector3.Zero)
                {
                    //ship isn't moving. just target ship center directly.
                    predictedPosition = enemyShip.Position;
                }
                else
                {
                    //aim ahead of a moving ship.
                    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    targetMovedir.Normalize();

                    float timeToTarget = Vector3.Distance(muzzleVec,
                        enemyShip.Position + targetMovedir * enemyShip.GetSpeed()) / turretWeapon.prjData.speed;

                    //get a rough estimated predicted position.
                    predictedPosition = enemyShip.Position + timeToTarget * targetMovedir * enemyShip.GetSpeed();

                    //We now have a rough estimate position of where the bullet will intercept the enemy.
                    //Now we want to refine that estimate and close in on a more accurate intercept point.
                    

                    //Iterate and check against a bunch of positions.
                    float lowestDelta = float.MaxValue;
                    Vector3 bestPosition = Vector3.Zero;

                    Vector3 predictMoveDir = enemyShip.Position - predictedPosition;
                    predictMoveDir.Normalize();

                    for (int i = 0; i < CHECKAMOUNT; i++)
                    {
                        //this is the candidate position.
                        Vector3 checkPos = predictedPosition + predictMoveDir * (i * INTERVALDIST);

                        //get time-to-arrive data for the bullet and the enemy.
                        float enemyTime = GetPredictedTime(gameTime, enemyShip.GetSpeed(),
                            enemyShip.Position, checkPos);
                        float bulletTime = GetPredictedTime(gameTime, turretWeapon.prjData.speed,
                            muzzleVec, checkPos);

                        //check the delta between the enemy time-to-arrive vs. the bullet time-to-arrive.
                        //hone in on the lowest delta value. store that value.
                        float currentDelta = Math.Abs(enemyTime - bulletTime);
                        if (currentDelta < lowestDelta)
                        {
                            lowestDelta = currentDelta;
                            bestPosition = checkPos;
                        }
                    }

                    predictedPosition = bestPosition;
                }
            }
            else
                predictedPosition = enemyShip.Position;

            return predictedPosition;
        }


        /// <summary>
        /// distance between checks.
        /// </summary>
        const int INTERVALDIST = 6;
        
        /// <summary>
        /// how much checks to perform.
        /// </summary>
        const int CHECKAMOUNT = 7;


        private float GetPredictedTime(GameTime gameTime, float objectSpeed, Vector3 objectPosition,
            Vector3 destination)
        {
            //generate the time it takes to arrive at Destination.
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float markDistTemp = objectSpeed * elapsed;
            float travelDist = Vector3.Distance(objectPosition, destination) * elapsed;
            float markTimeLength = travelDist / markDistTemp;

            /*
            markDistTemp *= (Helpers.MAXROUNDTIME / 1000f) / elapsed;
            Vector3 markDir = destination - objectPosition;
            markDir.Normalize();
            endPos = objectPosition + markDir * markDistTemp;*/

            return markTimeLength;
        }




        public Turret(Game game, ModelType modelName, SpaceShip owner, Weapon weapon, Vector3 offset,
            Vector3[] MuzzleOffsets, Vector3 defaultVector, Vector3 upAimVector)
            
        {
            this.localOffset = offset;
            this.parentShip = owner;

            turretWeapon = new Weapon();
            FieldInfo[] fieldInfoArray = typeof(Weapon).GetFields();
            foreach (FieldInfo info in fieldInfoArray)
            {
                info.SetValue( this.turretWeapon, info.GetValue(weapon));
            }

            
            this.muzzleOffsets = MuzzleOffsets;            
            this.defaultVector = defaultVector;
            this.upAimVector = upAimVector;
            this.turretModel = modelName;
        }





        public void ResetTurretOrientation()
        {
            Matrix orientation = Matrix.CreateFromQuaternion(parentShip.Rotation);
            Vector3 aimPosition = parentShip.Position + GetOrientationMatrix(defaultVector, orientation) * 128f;
            Matrix desiredRotation = Helpers.RotateToFace(position, aimPosition, orientation.Up);
            rotateQuat = Quaternion.Lerp(rotateQuat, Quaternion.CreateFromRotationMatrix(desiredRotation), rotateSpeed);
        }

        int updateTimer = 0;
        SpaceShip lastSelectedShip = null;

        Cue laserCue = null;

        public void StopBeamSounds()
        {
            if (turretWeapon.firetype != Weapon.fireType.Beam)
                return;

            if (laserCue == null)
                return;

            if (!laserCue.IsPlaying)
                return;

            if (laserCue.IsStopped)
                return;
            
            try
            {
                laserCue.Stop(AudioStopOptions.AsAuthored);
                laserCue.Dispose();
            }
            catch
            {
            }
        }

        public void PauseBeamSounds()
        {
            if (turretWeapon.firetype != Weapon.fireType.Beam)
                return;

            if (laserCue == null)
                return;

            if (laserCue.IsPaused)
                return;
            
            try
            {
                laserCue.Pause();
            }
            catch
            {
            }
        }

        private void PlayBeamSound()
        {
            if (turretWeapon.firetype != Weapon.fireType.Beam)
                return;

            if (!turretWeapon.beamIsFiring())
                return;

            try
            {
                if (laserCue == null)
                    laserCue = FrameworkCore.audiomanager.Play3DCue(sounds.Weapon.laser, parentShip.audioEmitter);

                // a sound can be paused and playing at the same time........ UGH!
                if (laserCue.IsPaused)
                {
                    laserCue.Resume();
                    return;
                }

                if (laserCue.IsPlaying)
                    return;

                laserCue.Play();
            }
            catch
            {
            }
        }

        /// <summary>
        /// tells teh beamsound when to sTOP
        /// </summary>
        private void UpdateBeamSound()
        {
            if (turretWeapon.firetype != Weapon.fireType.Beam)
                return;

            if (parentShip.IsDestroyed)
            {
                StopBeamSounds();                

                return;
            }

            if (laserCue == null)
                return;

            if (turretWeapon.beamIsFiring())
                return;

            if (laserCue.IsPaused)
                return;

            try
            {
                laserCue.Pause();
            }
            catch
            {
            }
        }

        //targeting brain.
        public void Update(GameTime gameTime, List<Collideable> ships)
        {
            turretWeapon.Update(gameTime);

            BeamDamageCheck(gameTime);


            UpdateBeamSound();
           

            if (updateTimer > 0)
            {
                updateTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (lastSelectedShip != null && !lastSelectedShip.IsDestroyed)
                {
                    UpdateRotation(gameTime, lastSelectedShip.Position);
                }
                else
                {
                    ResetTurretOrientation();
                }

                return;                
            }
            else
            {
                updateTimer = 300;
            }

            if (parentShip.targetShip != null && parentShip.targetShip.IsDestroyed)
            {
                //priority ship is dead. Clear this ship's priority target.
                parentShip.SetTargetShip(null);
            }

            //if we can't fire, then reset all turrets to default orientations.
            if (parentShip.OrderEffect != null && !parentShip.OrderEffect.canFire)
            {
                ResetTurretOrientation();
                return;
            }

            //limit how often the think update happens, limit to the turret weapon's refire rate.
            if (turretWeapon.lastFireTime > 0 && turretWeapon.firetype == Weapon.fireType.Projectile)
            {
                return;
            }


            

            SpaceShip targetShip = null;
            float nearestDistance = float.MaxValue;
            Vector3 finalPredictedPosition = Vector3.Zero;

            Matrix orientation = Matrix.CreateFromQuaternion(parentShip.Rotation); //ship orientation
            Matrix turretOrientation = Matrix.CreateFromQuaternion(rotateQuat); //turret orientation

            
            //does the ship have a priority target? check if target is in firing arc.
            if (parentShip.targetShip != null && !parentShip.targetShip.IsDestroyed)
            {
                Vector3 muzzleVec = GetMuzzleVec(0);
                SpaceShip enemyShip = (SpaceShip)parentShip.targetShip;
                Vector3 predictedPosition = GetPredictedPosition(gameTime, muzzleVec, enemyShip);

                //now check if the predictedPosition is within the valid firing arc.
                Vector3 aimFacing = GetOrientationMatrix(upAimVector, orientation);
                float frontDot = Helpers.GetDot(muzzleVec, predictedPosition, aimFacing);
                if (frontDot > 0)
                {
                    targetShip = enemyShip;
                    finalPredictedPosition = predictedPosition;
                }
            }
            
            //we do not have a priority target. so, let's evaluate every ship.
            if (targetShip == null)
            {
                for (int i = 0; i < ships.Count; i++)
                {
                    //only target spaceships.
                    if (!Helpers.IsSpaceship(ships[i]))
                        continue;

                    SpaceShip enemy = ships[i] as SpaceShip;

                    //don't target dead ships.
                    if (enemy.IsDestroyed)
                        continue;

                    //don't target owner.
                    if (enemy == parentShip)
                        continue;

                    //don't target friendlies.
                    if (enemy.owner == parentShip.owner)
                        continue;

                    //same faction. don't target this ship.
                    if (enemy.owner.factionName == parentShip.owner.factionName)
                        continue;

                    Vector3 aimFacing = GetOrientationMatrix(upAimVector, orientation);

                    float frontDot = Helpers.GetDot(this.position, enemy.Position, aimFacing);

                    if (frontDot < 0)
                        continue;


                    //ok, we now have a LIVE ENEMY ship that is in FRONT of us.
                    //Now get the predicted firing Vector IF the ship is moving.
                    // if the ship isn't moving, then we don't predict anything - we just directly target the ship.

                    SpaceShip enemyShip = (SpaceShip)enemy;



                    Vector3 muzzleVec = GetMuzzleVec(0); //for simplicity sake we only check the first muzzle barrel.
                    Vector3 predictedPosition = Vector3.Zero;

                    //if the weapon is not ready to be fired, don't bother calculating the predictedPosition
                    if (turretWeapon.CurBurstReloadTime <= 0 && turretWeapon.lastFireTime <= 0)
                        predictedPosition = GetPredictedPosition(gameTime, muzzleVec, enemyShip);
                    else
                        predictedPosition = enemy.Position;



                    //now check if the predictedPosition is within the valid firing arc.
                    frontDot = Helpers.GetDot(muzzleVec, predictedPosition, aimFacing);
                    if (frontDot < 0)
                        continue;

                    //keep track of the closest predictedPosition.
                    //enemy ship.
                    float distanceToEnemyShip = Vector3.Distance(predictedPosition, this.position);
                    if (distanceToEnemyShip < nearestDistance)
                    {
                        nearestDistance = distanceToEnemyShip;
                        targetShip = (SpaceShip)enemy;
                        finalPredictedPosition = predictedPosition;
                    }
                }
            }

            if (targetShip == null)
            {
                //no valid target found.
                //I have no target: so reset turret orientation to default aim position.
                ResetTurretOrientation();
                return;
            }

            //now fire the turret.
            for (int i = 0; i < muzzleOffsets.Length; i++)
            {
                Vector3 muzzleVec = position;
                muzzleVec +=
                    (turretOrientation.Right * muzzleOffsets[i].X) +
                    (turretOrientation.Up * muzzleOffsets[i].Y) +
                    (turretOrientation.Forward * -muzzleOffsets[i].Z);

                if (turretWeapon.Fire(parentShip, finalPredictedPosition, muzzleVec))
                {
                    IAudioEmitter audioEmit = new IAudioEmitter();
                    audioEmit.Position = this.position;


                    if (turretWeapon.firetype == Weapon.fireType.Projectile)
                        FrameworkCore.audiomanager.Play3DCue(sounds.Weapon.rifle, audioEmit);


                    PlayBeamSound();


                    //bolt was created. now create the muzzleflash.
                    parentShip.Particles.CreateMuzzleFlash(muzzleVec, (finalPredictedPosition - muzzleVec));

                    if (i >= muzzleOffsets.Length - 1)
                    {
                        //and reset the weapon's refire timer.
                        float fireRateModifier = 1f;
                        if (parentShip.OrderEffect != null)
                        {
                            if (parentShip.OrderEffect.fireRateModifier > 0)
                                fireRateModifier = parentShip.OrderEffect.fireRateModifier;
                        }

                        

                        turretWeapon.lastFireTime = (int)(turretWeapon.refireTime / fireRateModifier);

                        //fire rate modifier controls time between shots.
                        turretWeapon.lastFireTime = Helpers.ApplyFireRateModifier(parentShip, turretWeapon.lastFireTime);
                    }
                }
            }

            lastSelectedShip = targetShip;

            //Handle turret rotation.
            UpdateRotation(gameTime, finalPredictedPosition);
        }

        private void BeamDamageCheck(GameTime gameTime)
        {
            if (turretWeapon.firetype != Weapon.fireType.Beam)
                return;

            if (!turretWeapon.beamIsFiring())
                return;

            if (turretWeapon.curBeamDamageTime > 0)
            {
                turretWeapon.curBeamDamageTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                return;
            }

            int rayLength = turretWeapon.beamRange;

            turretWeapon.curBeamDamageTime = turretWeapon.beamDamageTime;

            Matrix turretOrientation = Matrix.CreateFromQuaternion(rotateQuat);

            //now let's check if the beam damaged any of the ships.
            for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
            {
                Collideable ship = FrameworkCore.level.Ships[k];
                
                //don't hit myself.
                if (ship == parentShip)
                    continue;

                //don't hit destroyed ships.
                if (ship.IsDestroyed)
                    continue;


                //no friendly fire.
                if (ship.owner != null &&
                    ship.owner.factionName == parentShip.owner.factionName)
                    continue;

                for (int i = 0; i < muzzleOffsets.Length; i++)
                {
                    Vector3 muzzleVec = position;
                    muzzleVec +=
                        (turretOrientation.Right * muzzleOffsets[i].X) +
                        (turretOrientation.Up * muzzleOffsets[i].Y) +
                        (turretOrientation.Forward * -muzzleOffsets[i].Z);

                    Vector3 finalPredictedPosition = muzzleVec + turretOrientation.Forward * 128;

                    Vector3 rayDir = finalPredictedPosition - muzzleVec;
                    rayDir.Normalize();
                    Ray beamRay = new Ray(muzzleVec, rayDir);                    

                    
                    if (ship.CollisionSpheres.Length > 1)
                    {
                        //if this is a multi-sphere ship, then first do a
                        //preliminary check on the ship's all-encompassing bSphere.
                        //if it comes false, then skip doing the detailed check.

                        if (ship.BSphere.Intersects(beamRay) > rayLength)
                            continue;
                    }


                    for (int m = 0; m < ship.CollisionSpheres.Length; m++)
                    {
                        if (ship.CollisionSpheres[m].sphere.Intersects(beamRay) <= rayLength)
                        {
                            ship.beamHit(parentShip, ship.CollisionSpheres[m].sphere.Center,
                                turretWeapon.beamMinDamage, turretWeapon.beamMaxDamage, muzzleVec);
                            break;
                        }
                    }
                }
            }
        }

        

        /// <summary>
        /// Instantly snap turrets to their default orientation. This function does not interpolate the movement!
        /// </summary>
        public void ResetOrientation()
        {
            Matrix orientation = Matrix.CreateFromQuaternion(parentShip.Rotation);
            Vector3 aimPosition = parentShip.Position + GetOrientationMatrix(defaultVector, orientation) * 128f;
            Matrix desiredRotation = Helpers.RotateToFace(position, aimPosition, orientation.Up);
            rotateQuat = Quaternion.CreateFromRotationMatrix(desiredRotation);
        }




        private Vector3 GetOrientationMatrix(Vector3 upAimVector, Matrix orientation)
        {
            if (upAimVector == Vector3.Up)
                return orientation.Up;
            else if (upAimVector == Vector3.Forward)
                return orientation.Forward;
            else if (upAimVector == Vector3.Down)
                return orientation.Down;
            else if (upAimVector == Vector3.Left)
                return orientation.Left;
            else if (upAimVector == Vector3.Right)
                return orientation.Right;

            return orientation.Forward;
        }


        private void UpdateRotation(GameTime gameTime, Vector3 targetPos)
        {
            //do a check to see if the turret can validly move to the desired orientation.
            Matrix orientation = Matrix.CreateFromQuaternion(parentShip.Rotation);
            Vector3 aimFacing = GetOrientationMatrix(upAimVector, orientation);
            float frontDot = Helpers.GetDot(this.position, targetPos, aimFacing);
            if (frontDot < 0)
            {
                //target is out of firing cone. reset turret to forward position.
                ResetTurretOrientation();
                return;
            }

            //target's still in valid firing cone. orient turret toward the target.
            Matrix lookAt = Matrix.CreateLookAt(this.position, targetPos, Vector3.Up);
            targetRotateQuat = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));
            rotateQuat = Quaternion.Lerp(rotateQuat, targetRotateQuat, rotateSpeed);
        }
        


        public void UpdatePosition()
        {
            Matrix m = Matrix.CreateFromQuaternion(parentShip.Rotation);

            Vector3 shipPos = parentShip.Position;
            shipPos +=
                m.Forward * -localOffset.Z +
                m.Right * localOffset.X +
                m.Up * localOffset.Y;

            this.position = shipPos;
        }

           

        public virtual void Draw(GameTime gameTime, Camera camera)
        {
            Matrix worldMatrix = Matrix.CreateFromQuaternion(rotateQuat);
            worldMatrix.Translation = position;

            FrameworkCore.meshRenderer.Draw(turretModel, worldMatrix, camera, parentShip.owner.ShipColor);

            /*
            if (lastSelectedShip != null)
            {
                FrameworkCore.SpriteBatch.Begin();
                Vector2 textPos = Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, position);
                Helpers.DrawOutline("" + lastSelectedShip, textPos);
                FrameworkCore.SpriteBatch.End();
            }
            */

            BeamRender(camera);


            //debug info on turrets.
            /*
            if (turretWeapon.firetype == Weapon.fireType.Beam && laserCue != null)
            {
                FrameworkCore.SpriteBatch.Begin();
                Vector2 textPos = Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, position);


                string txt = "PLAY: " + laserCue.IsPlaying + "  PAUS: " + laserCue.IsPaused + "  STOP: " + laserCue.IsStopped;



                Helpers.DrawOutline("" + txt, textPos);
                FrameworkCore.SpriteBatch.End();
            }*/
        }

        private void BeamRender(Camera camera)
        {
            if (turretWeapon.firetype == Weapon.fireType.Projectile)
                return;

            if (!turretWeapon.beamIsFiring())
                return;

            Matrix turretOrientation = Matrix.CreateFromQuaternion(rotateQuat); //turret orientation

            for (int i = 0; i < muzzleOffsets.Length; i++)
            {
                Vector3 muzzleVec = position;
                muzzleVec +=
                    (turretOrientation.Right * muzzleOffsets[i].X) +
                    (turretOrientation.Up * muzzleOffsets[i].Y) +
                    (turretOrientation.Forward * -muzzleOffsets[i].Z);

                Matrix worldMatrix = Matrix.CreateFromQuaternion(rotateQuat);
                worldMatrix.Translation = muzzleVec;

                FrameworkCore.meshRenderer.Draw(ModelType.beam, worldMatrix, camera);
            }
        }
    }
}