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
#endregion

namespace SpaceShooter
{
    public class Commander
    {
        #region Fields
        // Ship Simulation data
        public Vector3 position;
        Quaternion rotateQuat;
        Matrix rotationMatrix = Matrix.Identity;
        
        Color teamColor = Color.Fuchsia;

        public SpaceShip selectedShip; //currently selected.
        public SpaceShip hoverShip;

        public SpaceShip lastSelectedShip; // the last selected ship.

        Color shipColor = Color.Black;

        public string commanderName = Resource.GameCommanderDefault;

        public float HelpOverlayTransition = 0;


        public FactionInfo factionName;

        public int roundKills = 0;

        public Color ShipColor
        {
            get { return shipColor; }
            set { shipColor = value; }
        }


        public Color TeamColor
        {
            get { return teamColor; }
            set { teamColor = value; }
        }
        /*
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }*/
        public Quaternion Rotation
        {
            get { return rotateQuat; }
            set { rotateQuat = value; }
        }
#endregion

        #region Constructor
        public Commander(Color TeamColor, Color shipColor)
        {
            pathSpheres = new BoundingSphere[2048];
            pathIndex = 0;

            for (int k = 0; k < pathSpheres.Length; k++)
            {
                pathSpheres[k] = new BoundingSphere();
            }

            this.teamColor = TeamColor;
            this.shipColor = shipColor;
        }
        #endregion

        #region Functions
        public virtual void LoadContent()
        {            
        }
        
        
        
        public virtual void Update(GameTime gameTime)
        {

        }


        public virtual void Draw(GameTime gameTime)
        {
#if DEBUG
            /*
            for (int k = 0; k < pathIndex; k++)
            {
                FrameworkCore.sphereRenderer.Draw(pathSpheres[k], Matrix.Identity, Color.LimeGreen);
            }
             */
#endif
        }
        #endregion


        #region AI Brain

        SpaceShip currentEnemyTarget = null;
        int timeOnCurrentTarget = 0;

        private int pathIndex = 0;
        private BoundingSphere[] pathSpheres;


        /// <summary>
        /// how many times we attempt to find a valid path. if no valid path found, then don't move.
        /// </summary>
        const int PATHSHIPATTEMPTS = 32;
        const int PATHWANDERATTEMPTS = 64;


        private float randCoordinate
        {
            get
            {
                float coord = Helpers.randFloat(512, 2048);

                if (FrameworkCore.r.Next(2) == 0)
                    coord *= -1f;

                return coord;
            }
        }

        /// <summary>
        /// When the Action Round is about to begin, give the NPC ships some move orders.
        /// </summary>
        public void HandleAIOrders(GameTime gameTime)
        {
            pathIndex = 0;

            if (currentEnemyTarget == null || currentEnemyTarget.IsDestroyed)
            {
                timeOnCurrentTarget = 0;
                currentEnemyTarget = acquireEnemyShip();
            }
            else
            {
                //keep track of how many rounds we've been tracking this target.
                timeOnCurrentTarget++;

                if (timeOnCurrentTarget >= 2 && FrameworkCore.r.Next(2) == 0)
                {
                    timeOnCurrentTarget = 0;
                    currentEnemyTarget = acquireEnemyShip();                    
                }
            }            

            //if we don't have a valid enemy target, then do nothing.
            if (currentEnemyTarget == null)
                return;

            //go to every ship that belongs to this commander.
            for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
            {
                //filter out non-ships.
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[k]))
                    continue;

                //skip dead ships.
                if (FrameworkCore.level.Ships[k].IsDestroyed)
                    continue;

                //skip player-controlled ships.
                if (FrameworkCore.level.Ships[k].owner.GetType() == typeof(PlayerCommander))
                    continue;

                //only give orders to this commander's ships.
                if (FrameworkCore.level.Ships[k].owner != this)
                    continue;

                //We now have a handle on a ship belonging to this commander.
                //give it a move order.
                Vector3 roundEndPos = ((SpaceShip)FrameworkCore.level.Ships[k]).Position;

                bool foundValidPath = false;
                int candidateIndex = 0;
                BoundingSphere[] candidateSpheres = new BoundingSphere[64];

                for (int i = 0; i < PATHSHIPATTEMPTS + PATHWANDERATTEMPTS; i++)
                {
                    if (i <= PATHSHIPATTEMPTS)
                    {
                        //get a random move vector
                        roundEndPos = GetRandomDestination(gameTime,
                            (SpaceShip)FrameworkCore.level.Ships[k], currentEnemyTarget.Position);
                    }
                    else
                    {
                        //couldn't find a clean shot to the enemy.
                        //so, wander around randomly.
                        Vector3 wanderPos = new Vector3(randCoordinate, randCoordinate, randCoordinate);

                        roundEndPos = GetRandomDestination(gameTime,
                            (SpaceShip)FrameworkCore.level.Ships[k], wanderPos);
                    }

                    //construct a chain of spheres along this candidate path.
                    candidateIndex = 0;                    
                    Vector3 candidateDir = roundEndPos - ((SpaceShip)FrameworkCore.level.Ships[k]).Position;
                    candidateDir.Normalize();
                    int candidateRadius = (int)Math.Round(((SpaceShip)FrameworkCore.level.Ships[k]).BSphere.Radius) + 3/*buffer*/;
                    int candidateMoveDist = (int)Vector3.Distance(((SpaceShip)FrameworkCore.level.Ships[k]).Position,
                        roundEndPos);
                    for (int w = 0; w < candidateSpheres.Length; w++)
                    {
                        if (candidateIndex >= candidateSpheres.Length)
                            break;

                        Vector3 candidatePos = ((SpaceShip)FrameworkCore.level.Ships[k]).Position +
                            ((candidateDir * candidateRadius) * candidateIndex);

                        if (Vector3.Distance(((SpaceShip)FrameworkCore.level.Ships[k]).Position,
                            candidatePos) > candidateMoveDist)
                            break;

                        candidateSpheres[w] = new BoundingSphere(candidatePos, candidateRadius);
                        candidateIndex++;
                    }

                    //check this candidate path against the pathSpheres array.
                    if (CheckPathSpheres(candidateSpheres, candidateIndex) &&
                        CheckPathHulks(candidateSpheres, candidateIndex))
                    {
                        foundValidPath = true;                        
                        break;
                    }
                }


                if (foundValidPath)
                {
                    //found a valid path vector!
                    //stuff the candidateSpheres into the pathSpheres array.
                    for (int h = 0; h < candidateIndex; h++)
                    {
                        pathSpheres[pathIndex] = candidateSpheres[h];
                        pathIndex++;
                    }

                    //commit the move order to the spaceship.
                    CommitMoveOrder((SpaceShip)FrameworkCore.level.Ships[k], currentEnemyTarget,
                        roundEndPos);
                }
            }
        }

        /// <summary>
        /// This function prevents ships from pathing into each other. Check the candidates spheres against committed spheres from other ships.
        /// </summary>
        /// <param name="candidateSpheres"></param>
        /// <param name="candidateIndex"></param>
        /// <returns>TRUE if NO COLLISIONS</returns>
        private bool CheckPathSpheres(BoundingSphere[] candidateSpheres, int candidateIndex)
        {
            for (int h = 0; h < candidateIndex; h++)
            {
                //test path candidate against the pathSpheres array.
                for (int x = 0; x < pathIndex; x++)
                {
                    if (candidateSpheres[h].Intersects(pathSpheres[x]))
                    {
                        //path intersection. this path is no good, try again.
                        return false;
                    }
                }
            }

            //path is good.
            return true;
        }

        /// <summary>
        /// Ensure the ship doesn't collide into asteroids/hulks. Check candidate spheres against all asteroids/hulks.
        /// </summary>
        /// <param name="candidateSpheres"></param>
        /// <param name="candidateIndex"></param>
        /// <returns>TRUE if NO COLLISIONS</returns>
        private bool CheckPathHulks(BoundingSphere[] candidateSpheres, int candidateIndex)
        {
            for (int h = 0; h < candidateIndex; h++)
            {
                for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
                {
                    //filter hulks
                    if (!Helpers.IsHulk(FrameworkCore.level.Ships[k]))
                        continue;

                    if (candidateSpheres[h].Intersects(FrameworkCore.level.Ships[k].BSphere))
                    {
                        return false;
                    }
                }
            }

            //path is good.
            return true;
        }








        /// <summary>
        /// Find a suitable enemy target.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        private SpaceShip acquireEnemyShip()
        {
            try
            {
                //BC Sep 5 2010

                List<SpaceShip> enemyShips = new List<SpaceShip>();

                for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
                {
                    //filter out non-ships.
                    if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[k]) ||
                        FrameworkCore.level.Ships[k].IsDestroyed ||
                        FrameworkCore.level.Ships[k].owner.factionName == this.factionName)
                        continue;

                    //get handle on NPC ship.
                    SpaceShip NPCShip = (SpaceShip)FrameworkCore.level.Ships[k];
                    enemyShips.Add(NPCShip);
                }

                int randomShipIndex = FrameworkCore.r.Next(enemyShips.Count);
                return enemyShips[randomShipIndex];

                /*
                //Generate a list of all friendly ships.
                List<SpaceShip> friendlyShips = new List<SpaceShip>();

                for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
                {
                    //filter out non-ships.
                    if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[k]))
                        continue;

                    //skip dead ships.
                    if (FrameworkCore.level.Ships[k].IsDestroyed)
                        continue;

                    //skip player-controlled ships.
                    if (FrameworkCore.level.Ships[k].owner.GetType() == typeof(PlayerCommander))
                        continue;

                    //only give orders to this commander's ships.
                    if (FrameworkCore.level.Ships[k].owner != this)
                        continue;

                    friendlyShips.Add((SpaceShip)FrameworkCore.level.Ships[k]);
                }

                //pick a random friendly.
                int randomShipIndex = FrameworkCore.r.Next(friendlyShips.Count);
                SpaceShip randomFriendly = friendlyShips[randomShipIndex];

                //find the ship closest to this friendly.
                SpaceShip nearestShip = GetNearestEnemy(randomFriendly);
                return nearestShip;*/
            }
            catch
            {
                return null;
            }
        }

        

        private Vector3 GetRandomDestination(GameTime gameTime, SpaceShip origin, Vector3 destination)
        {
            //find a random location to move to. Move to a spot near the enemy.
            Vector3 finalPos = new Vector3(
                FrameworkCore.r.Next(16, 80),
                FrameworkCore.r.Next(16, 80),
                FrameworkCore.r.Next(16, 80));

            if (FrameworkCore.r.Next(2) == 0)
                finalPos.X *= -1f;

            if (FrameworkCore.r.Next(2) == 0)
                finalPos.Y *= -1f;

            if (FrameworkCore.r.Next(2) == 0)
                finalPos.Z *= -1f;

            //final Position. This disregards the round timer.
            finalPos = destination + finalPos;

            //calculate where the ship will be at the end of this round.
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float markDistTemp = origin.GetSpeed() * elapsed;
            float travelDist = Vector3.Distance(origin.Position, finalPos) * elapsed;
            float markTimeLength = travelDist / markDistTemp;
            markDistTemp *= (Helpers.MAXROUNDTIME / 1000f) / elapsed;
            Vector3 markDir = finalPos - origin.Position;
            markDir.Normalize();
            Vector3 roundEndPos = origin.Position + markDir * markDistTemp;

            //our calculation above assumes the ship doesn't stop prematurely. do a check to see if
            //the final position is actually short of the full round move time.
            if (Vector3.Distance(origin.Position, finalPos) < Vector3.Distance(origin.Position, roundEndPos))
                roundEndPos = finalPos;

            return roundEndPos;
        }


        private void CommitMoveOrder(SpaceShip origin, SpaceShip enemyShip, Vector3 roundEndPos)
        {
            //from the end-of-round position, rotate ship to look at the enemy.
            Matrix lookAt = Matrix.CreateLookAt(roundEndPos, enemyShip.Position, Vector3.Up);

            //determine if ship should roll.
            float frontDot = Helpers.GetDot(roundEndPos, enemyShip.Position, lookAt.Up);
            if (frontDot < 0)
            {
                lookAt = Matrix.CreateLookAt(roundEndPos, enemyShip.Position, Vector3.Down);
            }


            Quaternion quatRot = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));

            //tilt nose down a little bit.
            quatRot *= Quaternion.CreateFromAxisAngle(Vector3.Up, 0)
                * Quaternion.CreateFromAxisAngle(Vector3.Right, Helpers.randFloat(-0.6f, -0.3f))
                * Quaternion.CreateFromAxisAngle(Vector3.Backward, 0);


            //Generate position of ship. 
            origin.SetTargetMove(roundEndPos, quatRot);
        }
        











        
        private SpaceShip GetNearestEnemy(SpaceShip origin)
        {
            SpaceShip closestShip = null;
            float closest = int.MaxValue;

            for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
            {
                //filter out non-ships.
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[k]) ||
                    FrameworkCore.level.Ships[k].IsDestroyed ||
                    FrameworkCore.level.Ships[k].owner.factionName == origin.owner.factionName)
                    continue;

                //get handle on NPC ship.
                SpaceShip NPCShip = (SpaceShip)FrameworkCore.level.Ships[k];

                float curDist = Vector3.Distance(origin.Position, NPCShip.Position);

                if (curDist < closest)
                {
                    curDist = closest;
                    closestShip = NPCShip;
                }
            }

            return closestShip;
        }

        private SpaceShip GetLowestHealthEnemy(SpaceShip origin)
        {
            SpaceShip lowHealthShip = null;
            float lowestHealth = int.MaxValue;

            for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
            {
                //filter out non-ships.
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[k]) ||
                    FrameworkCore.level.Ships[k].IsDestroyed ||
                    FrameworkCore.level.Ships[k].owner.factionName == origin.owner.factionName)
                    continue;

                //get handle on NPC ship.
                SpaceShip NPCShip = (SpaceShip)FrameworkCore.level.Ships[k];

                float curHealth = NPCShip.Health;

                if (curHealth < lowestHealth)
                {
                    curHealth = lowestHealth;
                    lowHealthShip = NPCShip;
                }
            }

            return lowHealthShip;
        }
        
        #endregion
    }
}