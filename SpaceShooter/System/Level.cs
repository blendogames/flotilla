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
    public enum GameMode
    {
        Orders,
        Action,
        CarnageReport,
    }

    public class Level
    {
        List<Collideable> ships = new List<Collideable>();

        public List<Collideable> Ships
        {
            get { return ships; }
        }

        List<Commander> enemies = new List<Commander>();
        public List<Commander> Enemies
        {
            get { return enemies; }
            set { enemies = value; }
        }


        RenderTarget2D renderTarget;
        //Texture2D unprocessedScene;

        SysMenuManager levelMenuManager;
        public SysMenuManager LevelMenuManager
        {
            get { return levelMenuManager; }
        }


        SkyBox skyBox;
        MotionField motionField;
        MotionField motionField2;

        GameMode gameMode = GameMode.Orders;

        public GameMode gamemode
        {
            get { return gameMode; }
        }

        Planet earth;
#if UNUSED_GRAPHICS
        Atmosphere air;
#endif
        Sun sun;

        Cue actionMusic;
        public Cue ActionMusic
        {
            get { return actionMusic; }
        }

        public void ClearActionMusic()
        {
            if (actionMusic == null)
                return;

            if (actionMusic.IsPlaying)
                actionMusic.Stop(AudioStopOptions.AsAuthored);

            actionMusic.Dispose();
            actionMusic = null;
        }

        float lastSkyTransition = 1;

        float skyTransition = 1;
        bool hasRocks = false;


        public MessageQueue messageQueue;



        public bool isDemo
        {
            get
            {
                if (FrameworkCore.gameState == GameState.Logos)
                    return true;
                else
                    return false;
            }
        }
        

        //this constructor is only run once, when the game first starts up.
        public Level()
        {
            levelMenuManager = new SysMenuManager();
            messageQueue = new MessageQueue();

            PlayerCommander player = new PlayerCommander(PlayerIndex.Four,
                Faction.Blue.teamColor,
                Faction.Blue.teamColor);

            player.position = new Vector3(60, 20, 100);

            //CREATE PLAYER ONE.
            FrameworkCore.players.Add(player);

#if WINDOWS
            FrameworkCore.players[0].mouseEnabled = FrameworkCore.options.p1UseMouse;
#endif




            Commander enemy = new Commander(Color.Red,
                new Color(255, 90, 90));

            enemy.factionName = Faction.Red;

            enemies.Add(enemy);

            //AddToShiplist(shipTypes.DebugShip, players[0], ships);


            AddToShiplist(shipTypes.Destroyer, FrameworkCore.players[0], ships);
            AddToShiplist(shipTypes.Destroyer, FrameworkCore.players[0], ships);
            AddToShiplist(shipTypes.Battleship, FrameworkCore.players[0], ships);


            AddToShiplist(shipTypes.Battleship, enemies[0], ships);
            AddToShiplist(shipTypes.Destroyer, enemies[0], ships);
            AddToShiplist(shipTypes.Destroyer, enemies[0], ships);


            skyBox = new SkyBox(FrameworkCore.Game);
            skyBox.Visible = false;
            FrameworkCore.Game.Components.Add(skyBox);

            earth = new Planet(FrameworkCore.Game);
#if UNUSED_GRAPHICS
            air = new Atmosphere(FrameworkCore.Game);
#endif
            sun = new Sun(FrameworkCore.Game);

            motionField = new MotionField(FrameworkCore.Game);
            motionField2 = new MotionField(FrameworkCore.Game);



            enemyInventoryItems = new InventoryItem[]
            {
                new itRashadFireCon(2.0f),
                new itRashadFireCon(3.0f),
                new itRashadFireCon(1.5f),
                new itBeamShield(0.3f),
                new itBeamShield(0.5f),
                new itBeamShield(0.7f),
                new itJamalAutoDoc(50),
                new itJamalAutoDoc(100),
                new itKeshiaEngine(1.5f),
                new itKeshiaEngine(1.2f),
                new itMuyoShield(0.3f),
                new itMuyoShield(0.5f),
                new itRoachShield(0.3f),
                new itRoachShield(0.5f),
            };
        }

        

        public void LoadContent()
        {
            foreach (Commander player in FrameworkCore.players)
            {
                player.LoadContent();
            }


            foreach (Collideable ship in ships)
            {
                ship.LoadContent();
            }
            

            earth.LoadContent(FrameworkCore.Game, "earthdiffuse", "earthbump", "earthspec", "earthnight");
#if UNUSED_GRAPHICS
            air.LoadContent(FrameworkCore.Game, "clouds");
#endif
            sun.LoadContent(FrameworkCore.Game, "sun", Color.LightGoldenrodYellow, Color.LightGoldenrodYellow);


            //UpdatePlaybackItems();
            messageQueue.Initialize();
        }

        public void Initialize()
        {
            //Start enemy ship in front of us.
            Matrix playerOrientation = Matrix.CreateFromQuaternion(FrameworkCore.players[0].Rotation);


            foreach (PlayerCommander player in FrameworkCore.players)
            {
                player.Initialize();
            }


            int offset = 32;

            foreach (SpaceShip ship in ships)
            {
                if (ship.owner == FrameworkCore.players[0])
                {

                    ship.Position = playerOrientation.Right * offset;
                    //ship.Position += new Vector3(0, FrameworkCore.r.Next(-16, 16), 0);
                    //ship.Position += playerOrientation.Forward * r.Next(-16,16);
                    ship.Position += new Vector3(0, -20, 0);
                    ship.ResetAngle(30);
                    ship.Initialize();
                }
                else
                {


                    ship.Position = playerOrientation.Forward * offset;
                    //ship.Position += new Vector3(0, FrameworkCore.r.Next(-8, 8), 0);
                    //ship.Position += playerOrientation.Right * r.Next(-8, 8);                    

                    ship.Initialize();
                }

                offset += 24;
            }

            motionField.Initialize();
            motionField2.Initialize();

            Vector3 earthPos = Vector3.Right * 10000.0f;
            float earthSize = 2000;
            //earth.Initialize(earthSize, new Vector4(30.0f / 256.0f, 98 / 256.0f, 142 / 256.0f, 201 / 256.0f),
            earth.Initialize(earthSize, new Vector4(.6f, .2f, 0, 1f),
                earthSize, earthPos);

#if UNUSED_GRAPHICS
            //air.Initialize(earthSize, 35.0f, new Vector4(30.0f / 256.0f, 98 / 256.0f, 142 / 256.0f, 201 / 256.0f),
            air.Initialize(earthSize, 35.0f, new Vector4(.6f, .2f, 0, 1f),
                earthSize, earthPos);
#endif

            sun.Initialize(8000, Vector3.Forward * 1000.0f + Vector3.Up * 300);

            renderTarget = new RenderTarget2D(FrameworkCore.Graphics.GraphicsDevice,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height,
                false,
                FrameworkCore.Graphics.GraphicsDevice.DisplayMode.Format,
                DepthFormat.None);

            FrameworkCore.hulkManager.Initialize(ships);

            Helpers.UpdateCameraProjections(FrameworkCore.players.Count);
        }



        private Cue GetActionCue()
        {
            try
            {
                return FrameworkCore.soundbank.GetCue(sounds.Music.nocturnes);
            }
            catch
            {
                return null;
            }
        }

        public void StartGameplay(GameTime gameTime)
        {
            FrameworkCore.MainMenuManager.ClearAll();

            victoryAchieved = false;
            FrameworkCore.sysMenuManager.ClearAll();
            FrameworkCore.gameState = GameState.Play;

            ActivateOrdersMode();

            ResetFade();

            FrameworkCore.PlayCue(sounds.Music.none);

            if (actionMusic == null)
            {
                actionMusic = GetActionCue();
            }

            FrameworkCore.Particles.ClearAll();

            
            for (int i = 0; i < 512; i++)
            {
                FrameworkCore.Particles.Update(gameTime); //cycle it to clean up lingering particles.
            }

            foreach (PlayerCommander player in FrameworkCore.players)
            {
                SetupGoodCameraPosition(player);
                player.InitializeScoreboard();

                player.roundKills = 0;


                //force camera to be able to see at least one of the player's ships.
                SpaceShip playerShip = null;
                
                foreach(Collideable ship in FrameworkCore.level.ships)
                {
                    if (!Helpers.IsSpaceship(ship))
                        continue;

                    if (ship.owner != player)
                        continue;
                    
                    playerShip = (SpaceShip)ship;
                    break;                    
                }

                if (playerShip != null)
                {
                    player.MoveCamToSeeMoveDisc(gameTime, playerShip);
                }
            }

            foreach (Commander enemy in enemies)
            {
                enemy.roundKills = 0;
            }

            //set up asteroids.
            if (FrameworkCore.worldMap != null && FrameworkCore.worldMap.isTutorial)
                FrameworkCore.hulkManager.GenerateAsteroidField(2, 3);
            else
                FrameworkCore.hulkManager.GenerateAsteroidField(2,5);

            Helpers.UpdateCameraProjections(FrameworkCore.players.Count);


        }

        InventoryItem[] enemyInventoryItems;

        private InventoryItem getRandomEnemyInventory()
        {
            int maxAmount = enemyInventoryItems.Length;
            return enemyInventoryItems[FrameworkCore.r.Next(maxAmount)];
        }

        private void AddEnemyBonuses(FleetShip enemy)
        {
            if (!FrameworkCore.isHardcoreMode)
                return;

            if (FrameworkCore.players[0].planetsVisited < FrameworkCore.r.Next(20))
                return;

            enemy.upgradeArray[0] = getRandomEnemyInventory();

            if (FrameworkCore.players[0].planetsVisited < FrameworkCore.r.Next(40))
                return;

            enemy.upgradeArray[1] = getRandomEnemyInventory();
        }

        public void StartCampaignLevel(GameTime gameTime, Event eventData)
        {
            ClearAll();

            //sanity check. give some default data if none exists.
            if (eventData.shipList == null)
                eventData.shipList = new ShipData[1] { shipTypes.Destroyer };

            if (eventData.shipMinMax == null)
                eventData.shipMinMax = new Point(1, 2);

            if (eventData.faction == null)
                eventData.faction = Faction.Red;

            

            //SET UP ENEMY AND ENEMY SHIPS POSITIONING.
            Commander enemy = new Commander(eventData.faction.teamColor, eventData.faction.teamColor);

            if (eventData.pilotName == null || eventData.pilotName.Length <= 0)
                enemy.commanderName = Helpers.GenerateName("Gamertag");
            else
            {
                //remove the second line of the name.
                string myString = eventData.pilotName;
                int lineEnd = myString.IndexOf('\n');

                if (lineEnd > 0)
                {
                    myString = myString.Substring(0, lineEnd);                    
                }

                enemy.commanderName = myString;
            }

            enemy.factionName = eventData.faction;
            FrameworkCore.level.AddToEnemiesList(enemy);

            Vector3 fleetPos = GetRandomFleetPos();
            fleetPos.Z *= -1.0f; //enemies are always in the negative Z.

            List<FleetShip> fleetShips = new List<FleetShip>(eventData.shipMinMax.Y);
            
            int amountOfShips = FrameworkCore.r.Next(eventData.shipMinMax.X, eventData.shipMinMax.Y);

            for (int i = 0; i < amountOfShips; i++)
            {
                int randomIndex = FrameworkCore.r.Next(eventData.shipList.Length);
                Helpers.AddFleetShip(fleetShips, eventData.shipList[randomIndex]);

                //add bonuses to enemies.
                AddEnemyBonuses(fleetShips[i]);
            }



            SetupCampaignShips(enemy, fleetPos, fleetShips, 180);



            //PLAYER ONE.
            fleetPos = GetRandomFleetPos();
            List<FleetShip> player1Ships = new List<FleetShip>(FrameworkCore.players[0].campaignShips.Count);
            List<FleetShip> player2Ships = new List<FleetShip>(FrameworkCore.players[0].campaignShips.Count);
            foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
            {
                if (!ship.childShip)
                    player1Ships.Add(ship);
                else
                    player2Ships.Add(ship);
            }



            SetupCampaignShips(FrameworkCore.players[0], fleetPos, player1Ships, 0);


            //PLAYER TWO.
            if (FrameworkCore.players.Count > 1)
            {
                if (FrameworkCore.players[1] != null)
                {

                    Vector3 player2Pos = GetRandomFleetPos();

                    if (FrameworkCore.r.Next(2) == 0)
                        player2Pos.Z = fleetPos.Z + Helpers.randFloat(64, 96);
                    else
                        player2Pos.Z = fleetPos.Z + Helpers.randFloat(-96, -64);

                    SetupCampaignShips(FrameworkCore.players[1], player2Pos, player2Ships, 0);
                }
            }


            FrameworkCore.level.LoadShipContent();

            Helpers.UpdateCameraProjections(FrameworkCore.players.Count);

            //set up the asteroid field and start gameplay.
            FrameworkCore.level.StartGameplay(gameTime);
        }




        private void SetupGoodCameraPosition(PlayerCommander commander)
        {
            //Get one of the player's ships.
            SpaceShip playerShip = null;

            foreach(SpaceShip ship in ships)
            {
                if (ship.owner == commander)
                {
                    playerShip = ship;
                    break;
                }
            }

            if (playerShip == null)
                return;

            //Get one of the enemy ships.
            SpaceShip enemyShip = null;

            foreach (SpaceShip ship in ships)
            {
                if (ship.owner.factionName != commander.factionName)
                {
                    enemyShip = ship;
                    break;
                }
            }

            if (enemyShip == null)
                return;

            //we now have a handle on a player ship and an enemy ship.

            float dist = Vector3.Distance(playerShip.Position, enemyShip.Position);
            Vector3 moveDir = playerShip.Position - enemyShip.Position;
            moveDir.Normalize();

            Vector3 finalPos = enemyShip.Position + (moveDir * (dist + 128));
            finalPos.Y = Math.Max(finalPos.Y + 16, 16); //ensure camera is above the zero plane.

            Vector3 lookatPos = Vector3.Lerp(enemyShip.Position, playerShip.Position, 0.6f);

            Matrix lookAt = Matrix.CreateLookAt(finalPos, lookatPos, Vector3.Up);
            Quaternion rotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));

            commander.position = finalPos;
            commander.Rotation = rotation;
            commander.rotationMatrix = Matrix.CreateFromQuaternion(rotation);
        }

        private void SetupCampaignShips(Commander commander, Vector3 fleetCenter, List<FleetShip> fleetShips, float shipAngle)
        {
            Vector3 shipPos = fleetCenter;

            foreach(FleetShip ship in fleetShips)
            {
                //FrameworkCore.level.AddToShiplist(ship.shipData, commander, FrameworkCore.level.ships,
                //    shipPos, shipAngle);
                FrameworkCore.level.AddToShiplist(ship, commander, FrameworkCore.level.ships,
                    shipPos, shipAngle);

                if (commander.GetType() == typeof(PlayerCommander))
                {
                    shipPos.X += Helpers.randFloat(24, 48);
                }
                else
                {
                    //NPC fleet.
                    shipPos.X += Helpers.randFloat(96, 160);
                    shipPos.Y += Helpers.randFloat(-64, 64);
                    shipPos.Z += Helpers.randFloat(-32, 32);
                }
            }
        }

        private Vector3 GetRandomFleetPos()
        {
            Vector3 fleetPos = Vector3.Zero;
            fleetPos.X = Helpers.randFloat(-128, 128);
            fleetPos.Y = Helpers.randFloat(-128, 128); //height
            fleetPos.Z = Helpers.randFloat(80, 128); //distance
            return fleetPos;
        }

        public void LoadShipContent()
        {
            foreach (SpaceShip ship in ships)
            {
                ship.LoadContent();
                ship.Initialize();
            }
        }


        public void AddToEnemiesList(Commander commander)
        {
            Enemies.Add(commander);
        }


        //campaign. stuff the ship name/stats into the spaceship.
        public void AddToShiplist(FleetShip ship, Commander owner, List<Collideable> shiplist, Vector3 pos, float shipAngle)
        {
            SpaceShip s = new SpaceShip(FrameworkCore.Game, FrameworkCore.Particles, owner, ship.shipData, ship);
            s.Position = pos;
            s.ResetAngle(shipAngle);
            
            shiplist.Add(s);

            s.modelMesh = ship.shipData.modelname;
            CreatePlaybackShip(s, owner.ShipColor);
        }

        public void AddToShiplist(ShipData shipdata, Commander owner, List<Collideable> shiplist)
        {
            SpaceShip s = new SpaceShip(FrameworkCore.Game, FrameworkCore.Particles, owner, shipdata, null);
            shiplist.Add(s);

            s.modelMesh = shipdata.modelname;
            CreatePlaybackShip(s, owner.ShipColor);
        }

        public void AddToShiplist(ShipData shipdata, Commander owner)
        {
            SpaceShip s = new SpaceShip(FrameworkCore.Game, FrameworkCore.Particles, owner, shipdata, null);
            ships.Add(s);

            s.modelMesh = shipdata.modelname;
            CreatePlaybackShip(s, owner.ShipColor);
        }

        public void AddToShiplist(ShipData shipdata, Commander owner, List<Collideable> shiplist, Vector3 pos, float shipAngle)
        {
            SpaceShip s = new SpaceShip(FrameworkCore.Game, FrameworkCore.Particles, owner, shipdata, null);
            s.Position = pos;
            s.ResetAngle(shipAngle);
            shiplist.Add(s);

            s.modelMesh = shipdata.modelname;
            CreatePlaybackShip(s, owner.ShipColor);
        }

        private void CreatePlaybackShip(Entity entity, Color color)
        {
            FrameworkCore.playbackSystem.AddItem(entity, objectType.ship, color);
        }


        public void Update(GameTime gameTime)
        {
            if (fadeTransition > 0)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(1300).TotalMilliseconds);
                fadeTransition = MathHelper.Clamp(fadeTransition - delta, 0, 1);
            }


            if (FrameworkCore.sysMenuManager.Update(gameTime, FrameworkCore.players[0].inputmanager))
                return;

#if XBOX
            //Pause game if it's disconnected.
            if (gameMode == GameMode.Action)
            {
                if (FrameworkCore.players != null && !FrameworkCore.players[0].inputmanager.isConnected)
                {
                    FrameworkCore.sysMenuManager.AddMenu(new PauseMenu());
                }
            }
#endif

            messageQueue.Update(gameTime);

            FrameworkCore.WorldtextManager.Update(gameTime);

            if (FrameworkCore.players != null && FrameworkCore.players.Count > 0)
            {
                if (FrameworkCore.players[0].inputmanager.buttonStartPressed)
                {
                    FrameworkCore.sysMenuManager.AddMenu(new PauseMenu());
                }
            }
            

#if WINDOWS
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                FrameworkCore.sysMenuManager.AddMenu(new PauseMenu());
            }
#endif

            

            foreach (PlayerCommander player in FrameworkCore.players)
            {
                player.Update(gameTime);

                if (gameMode == GameMode.Action)
                    player.ActionUpdate(gameTime);
            }


            levelMenuManager.Update(gameTime, FrameworkCore.players[0].inputmanager);








            if (gameMode == GameMode.Action)
                FrameworkCore.audiomanager.Update(gameTime); //handle 3d audio updates.



            UpdateSkyBrightness(gameTime);

            foreach (Collideable ship in ships)
            {
                ship.Update(gameTime);
            }


            if (gameMode == GameMode.Orders)
            {
                if (actionTransition > 0)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(800).TotalMilliseconds);

                    actionTransition = MathHelper.Clamp(actionTransition - delta, 0, 1);
                }

                OrdersThink(gameTime);
            }
            else if (gameMode == GameMode.Action)
            {
                if (actionTransition < 1)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(800).TotalMilliseconds);

                    actionTransition = MathHelper.Clamp(actionTransition + delta, 0, 1);
                }

                ActionThink(gameTime);
            }

            //Update the background            
#if UNUSED_GRAPHICS
            air.Update(gameTime);
#endif
            sun.Update(gameTime);
            earth.Update(gameTime);
        }

        

        public void ClearAll()
        {
            levelMenuManager.ClearAll();
            hasRocks = false;

            FrameworkCore.playbackSystem.ClearAll();
            messageQueue.ClearAll();
            FrameworkCore.sysMenuManager.ClearAll();

            FrameworkCore.playbackSystem.ResetWorldTimer();
            FrameworkCore.WorldtextManager.ClearAll();
            FrameworkCore.Particles.ClearAll();
            FrameworkCore.Bolts.ClearAll();
            FrameworkCore.hulkManager.ClearAll();
            FrameworkCore.debrisManager.ClearAll();
            FrameworkCore.audiomanager.ClearAll();
            


            ships.Clear();
            enemies.Clear();            
        }

        

        public void DemoThink(GameTime gameTime)
        {
            if (!hasRocks)
            {
                FrameworkCore.hulkManager.GenerateAsteroidField(2,5);
                hasRocks = true;
            }

            foreach (PlayerCommander player in FrameworkCore.players)
            {
                player.DemoUpdate(gameTime);

                player.ActionUpdate(gameTime);
            }




            FrameworkCore.audiomanager.Update(gameTime); //handle 3d audio updates.

            UpdateSkyBrightness(gameTime);

            foreach (Collideable ship in ships)
            {
                ship.Update(gameTime);
            }


            
            if (actionTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(800).TotalMilliseconds);

                actionTransition = MathHelper.Clamp(actionTransition + delta, 0, 1);
            }

            ActionThink(gameTime);            

            //Update the background            
#if UNUSED_GRAPHICS
            air.Update(gameTime);
#endif
            sun.Update(gameTime);
            earth.Update(gameTime);
        }




        #region collision
        //detect if one ship is intersecting another ship.
        private bool DetectShipCollision(Collideable ship1, Collideable ship2, out Vector3 intersectPoint)
        {
            if (!ship1.BSphere.Intersects(ship2.BSphere))
            {
                intersectPoint = Vector3.Zero;
                return false;
            }

            for (int i = 0; i < ship1.CollisionSpheres.Length; i++)
            {
                BoundingSphere ship1Sphere = ship1.CollisionSpheres[i].sphere;

                for (int k = 0; k < ship2.CollisionSpheres.Length; k++)
                {
                    //first do a preliminary check to see if the boundingsphere balls touch.

                    
                    //now do the detailed check.
                    BoundingSphere ship2Sphere = ship2.CollisionSpheres[k].sphere;

                    if (ship1Sphere.Intersects(ship2Sphere))
                    {
                        intersectPoint = Vector3.Lerp(
                            ship1Sphere.Center,
                            ship2Sphere.Center,
                            0.5f);
                        return true;
                    }
                }
            }
            intersectPoint = Vector3.Zero;
            return false;
        }






        private void DetectBulletCollision(GameTime gameTime)
        {
            if (bulletCollisionTimer > 0)
            {
                bulletCollisionTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                return;
            }


            bulletCollisionTimer = 8; //update bullet collision every X amount of cycles.
            
            //iterate through every bullet.
            for (int i = 0; i < FrameworkCore.Bolts.ActiveBolts.Count; i++)
            {
                for (int s = 0; s < ships.Count; s++)
                {
                    if (ships[s].IsDestroyed)
                        continue;
                    
                    //do not hit myself!!
                    if (FrameworkCore.Bolts.Bolts[FrameworkCore.Bolts.ActiveBolts[i]].Owner == ships[s])
                        continue;

                    //first check boundingsphere.
                    if (ships[s].BSphere.Contains(FrameworkCore.Bolts.Bolts[FrameworkCore.Bolts.ActiveBolts[i]].Position) == ContainmentType.Disjoint)
                        continue;

                    //now do detailed check.
                    for (int k = 0; k < ships[s].CollisionSpheres.Length; k++)
                    {
                        //if not hitting this sphere, then continue.
                        if (ships[s].CollisionSpheres[k].sphere.Contains(FrameworkCore.Bolts.Bolts[FrameworkCore.Bolts.ActiveBolts[i]].Position) == ContainmentType.Disjoint)
                            continue;

                        ships[s].Hit(FrameworkCore.Bolts.Bolts[FrameworkCore.Bolts.ActiveBolts[i]]);
                        FrameworkCore.Bolts.Bolts[FrameworkCore.Bolts.ActiveBolts[i]].Hit(FrameworkCore.Bolts.Bolts[FrameworkCore.Bolts.ActiveBolts[i]].Position);

                        //break out of loop. continue to next bolt.
                        s = ships.Count + 1;
                        break;
                    }
                }
            }
        }

        int shipCollisionTimer = 0;
        int bulletCollisionTimer = 0;

        /// <summary>
        /// Detects Collisions between game objects
        /// </summary>
        /// <param name="gameTime"></param>
        void HandleCollisions(GameTime gameTime)
        {

            #region Ship-to-ship Collisions
            if (shipCollisionTimer <= 0)
            {
                shipCollisionTimer = 300; //how often to update ship collisions.

                for (int i = ships.Count - 1; i >= 0; i--)
                {
                    if (ships[i] == null)
                        continue;

                    if (ships[i].IsDestroyed)
                        continue;

                    //foreach (Collideable ship2 in ships)
                    for (int k = ships.Count - 1; k >= 0; k--)
                    {
                        if (ships[k] == null)
                            continue;

                        if (ships[k].IsDestroyed)
                            continue;

                        //??? the i value keeps trying to grab an invalid index.
                        int iNum = (int)MathHelper.Clamp(i, 0, ships.Count - 1);

                        //don't do a collision check on myself.....
                        if (ships[iNum] == ships[k])
                            continue;

                        Vector3 intersectPoint;
                        if (DetectShipCollision(ships[iNum], ships[k], out intersectPoint))
                        {
                            ships[iNum].Hit(ships[k], intersectPoint);
                        }
                    }
                }
            }
            else
                shipCollisionTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            #endregion
            
            DetectBulletCollision(gameTime);
        }
        #endregion


        private void UpdateSkyBrightness(GameTime gameTime)
        {
            if (gameMode == GameMode.Orders)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(1400).TotalMilliseconds);

                skyTransition = MathHelper.Clamp(skyTransition - delta, 0, 1);
            }
            else
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(800).TotalMilliseconds);

                skyTransition = MathHelper.Clamp(skyTransition + delta, 0, 1);
            }

            if (lastSkyTransition != skyTransition)
            {
                foreach (PlayerCommander player in FrameworkCore.players)
                {
                    float fov = MathHelper.Lerp(45, 60, skyTransition);
                    player.UpdateCameraFOV(fov);
                }
            }

            lastSkyTransition = skyTransition;
        }


        private void OrdersThink(GameTime gameTime)
        {
            int numPlayersReady = 0;
            foreach (PlayerCommander player in FrameworkCore.players)
            {
                player.UpdateCursor(gameTime, ships);

                if (player.isReady)
                    numPlayersReady++;
            }

            if (numPlayersReady >= FrameworkCore.players.Count)
            {
                ActivateActionMode(gameTime);
            }
        }


        public void ActivateActionMode(GameTime gameTime)
        {
            victoryUpdateTimer = 1000;

            FrameworkCore.playbackSystem.IncreaseRound();
            gameMode = GameMode.Action;

            foreach (PlayerCommander player in FrameworkCore.players)
            {                
                FrameworkCore.audiomanager.ResumeAll();
            }

            if (gameTime != null)
            {
                foreach (Commander commander in enemies)
                {
                    commander.HandleAIOrders(gameTime);
                }
            }

            try
            {
                if (actionMusic != null)
                {
                    //Jan 11: this is where it was crashing!!!
                    if (actionMusic.IsPaused)
                        actionMusic.Resume();
                    else if (actionMusic.IsStopped)
                    {
                        actionMusic = GetActionCue();
                        try
                        {
                            actionMusic.Play();
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        try
                        {
                            actionMusic.Play();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void EndOfActionMode()
        {
            if (FrameworkCore.level.isDemo)
                return;

            try
            {
                if (actionMusic != null && actionMusic.IsPlaying)
                    actionMusic.Pause();
            }
            catch
            {
            }

            bool playerWin = false;
            if (VictoryCheck(out playerWin))
            {
                StartCarnageMenu(playerWin);
            }
            else
            {
                ActivateOrdersMode();
            }

            foreach (Collideable ship in FrameworkCore.level.ships)
            {
                if (!Helpers.IsSpaceship(ship))
                    continue;

                if (ship.IsDestroyed)
                    continue;

                //pause any beam looping sounds.
                ((SpaceShip)ship).PauseBeamSounds();

                //clear priority target.
                ((SpaceShip)ship).SetTargetShip(null);
            }
        }

        private bool VictoryCheck(out bool playerWin)
        {
            playerWin = false;

            int faction1Ships = 0;
            int faction2Ships = 0;

            

            foreach (Collideable ship in ships)
            {
                if (!Helpers.IsSpaceship(ship))
                    continue;

                if (!ship.IsDestroyed)
                {
                    if (ship.owner.factionName == Faction.Blue)
                        faction1Ships++;
                    else if (ship.owner.factionName != Faction.Blue)
                        faction2Ships++;
                }
            }

            //check whether any faction has zero ships alive.
            if (faction1Ships <= 0 || faction2Ships <= 0)
            {
                playerWin = faction1Ships > 0;
                return true;
            }

            return false;
        }

        private void StartCarnageMenu(bool playerWin)
        {
            if (isDemo)
                return;

            foreach (Collideable ship in FrameworkCore.level.ships)
            {
                if (!Helpers.IsSpaceship(ship))
                    continue;

                if (ship.IsDestroyed)
                    continue;

                //pause any beam looping sounds.
                ((SpaceShip)ship).PauseBeamSounds();
            }

            UpdatePlaybackItems();
            FrameworkCore.audiomanager.PauseAll();
            ClearActionMusic();
            gameMode = GameMode.CarnageReport;
            levelMenuManager.AddMenu(new CarnageReport(playerWin));
        }

        private void ActivateOrdersMode()
        {
            //end of round. reset things.
            foreach (PlayerCommander player in FrameworkCore.players)
            {
                player.isReady = false;                
            }

            FrameworkCore.audiomanager.PauseAll();

            UpdatePlaybackItems();

            gameMode = GameMode.Orders;
        }


        private void UpdatePlaybackItems()
        {
            FrameworkCore.playbackSystem.UpdateItemPositions(ships);
            FrameworkCore.playbackSystem.UpdateBoltPositions(FrameworkCore.Bolts.Bolts);
            FrameworkCore.playbackSystem.UpdateItemPositions(FrameworkCore.hulkManager.Hulks);
        }

        bool victoryAchieved = false;
        float victoryTimer = 0;
        int victoryUpdateTimer = 1000;

        private void ActionThink(GameTime gameTime)
        {
            //update the round timer.
            if (!FrameworkCore.playbackSystem.Update(gameTime))
            {
                EndOfActionMode();
                return;
            }


            //victory check.
            if (victoryAchieved)
            {
                if (victoryTimer + 3500/*buffer time*/ <= FrameworkCore.playbackSystem.WorldTimer)
                {
                    bool playerWin = false;
                    if (VictoryCheck(out playerWin))
                    {
                        StartCarnageMenu(playerWin);
                    }
                }
            }
            else if (victoryUpdateTimer <= 0)
            {
                victoryUpdateTimer = 1000;

                bool p = false;
                if (VictoryCheck(out p))
                {
                    victoryAchieved = true;
                    victoryTimer = FrameworkCore.playbackSystem.WorldTimer;

                    //play the cheer sound.  
                    if (!FrameworkCore.level.isDemo)
                    {
                        bool humanAlive = false;

                        //determine if any HUMANS are alive. if so, play a cheer.
                        foreach (Collideable ship in ships)
                        {
                            if (!Helpers.IsSpaceship(ship))
                                continue;

                            if (ship.IsDestroyed)
                                continue;

                            if (ship.owner.GetType() != typeof(PlayerCommander))
                                continue;

                            humanAlive = true;
                            break;
                        }
                        if (!FrameworkCore.HideHud)
                        {
                            if (humanAlive)
                                FrameworkCore.PlayCue(sounds.Fanfare.cheering);
                        }
                    }
                    
                }
            }
            else
                victoryUpdateTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;


            
            FrameworkCore.debrisManager.Update(gameTime);
            FrameworkCore.hulkManager.Update(gameTime);

            //Update the Bolts
            FrameworkCore.Bolts.Update(gameTime);

            //Run collision detection.
            HandleCollisions(gameTime);

            //update particle simulation.
            FrameworkCore.Particles.Update(gameTime);

            for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
            {
                Collideable ship = FrameworkCore.level.Ships[k];

                if (ship.IsDestroyed)
                    continue;

                if (!Helpers.IsSpaceship(ship))
                    continue;


                SpaceShip curShip = (SpaceShip)ship;
                curShip.ActionThink(gameTime, ships);
            }
        }

        

#if DEBUG
        #region FramerateCounter
        int framerateSecondTimer = 0;
        int framerateCounter = 0;
        int curFramerate = 0;

        private void UpdateFramerate(GameTime gameTime)
        {
            framerateSecondTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            framerateCounter++;

            if (framerateSecondTimer >= 1000)
            {
                curFramerate = framerateCounter;

                framerateCounter = (int)MathHelper.Clamp(framerateCounter, 0, 60);
                float node = framerateCounter / 60f;
                framerateArray.Enqueue(node);


                framerateCounter = 0;
                framerateSecondTimer = 0;
            }

            Vector2 resolution = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                                             FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            //if (FrameworkCore.debugMode)
            {


                Point graphStart = new Point((int)resolution.X - 400, 110);

                int index = 0;
                int barWidth = 4;
                int barHeight = 64;
                foreach (float node in framerateArray)
                {
                    Rectangle barRect = new Rectangle(graphStart.X + index * barWidth,
                        (int)(graphStart.Y - barHeight * node),
                        barWidth,
                        (int)(barHeight * node));
                    Color barColor = Color.Lerp(Color.Red, new Color(0, 255, 0), node);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, barRect, sprite.blank, barColor);

                    index++;
                }

                if (index > 60)
                    framerateArray.Dequeue();

                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.SerifBig, "" + curFramerate,
                    new Vector2(resolution.X - 120, 40), Color.Yellow);

            }
        }

        Queue<float> framerateArray = new Queue<float>();
        #endregion
#endif

        #region DrawScene

        private void DrawGrid(PlayerCommander curPlayer)
        {
            if (FrameworkCore.level.isDemo)
                return;

            if (FrameworkCore.HideHud)
            {
                return;
            }

            //draw grid.            
            Color gridColor = new Color(255, 128, 0, 160);
            int gridSize = Helpers.GRIDSIZE;

            //the two main lines.
            FrameworkCore.lineRenderer.Draw(
                new Vector3(gridSize, curPlayer.GridAltitude, 0),
                new Vector3(-gridSize, curPlayer.GridAltitude, 0),
                gridColor);

            FrameworkCore.lineRenderer.Draw(
                new Vector3(0, curPlayer.GridAltitude, -gridSize),
                new Vector3(0, curPlayer.GridAltitude, gridSize),
                gridColor);


            if (curPlayer.CurrentMode != null)
            {
                if (curPlayer.CurrentMode.modeName == ModeName.MoveMode)
                {
                    //the inner gridlines.
                    gridColor = new Color(255, 128, 0, 96); //BC 3-28-2019 Increase alpha of lines.
                    int gridInterval = Helpers.GRIDSIZE / 32;
                    for (int i = -Helpers.GRIDSIZE; i <= Helpers.GRIDSIZE; i += gridInterval)
                    {
                        if (i == 0)
                            continue;

                        FrameworkCore.lineRenderer.Draw(
                            new Vector3(i, curPlayer.GridAltitude, -gridSize),
                            new Vector3(i, curPlayer.GridAltitude, gridSize),
                            gridColor);

                        FrameworkCore.lineRenderer.Draw(
                            new Vector3(-gridSize, curPlayer.GridAltitude, i),
                            new Vector3(gridSize, curPlayer.GridAltitude, i),
                            gridColor);
                    }
                }
            }



            gridColor = new Color(255, 128, 0, 48);

            //the outer lines.
            FrameworkCore.lineRenderer.Draw(
                new Vector3(gridSize, curPlayer.GridAltitude, gridSize),
                new Vector3(-gridSize, curPlayer.GridAltitude, gridSize),
                gridColor);
            FrameworkCore.lineRenderer.Draw(
                new Vector3(gridSize, curPlayer.GridAltitude, -gridSize),
                new Vector3(-gridSize, curPlayer.GridAltitude, -gridSize),
                gridColor);

            FrameworkCore.lineRenderer.Draw(
                new Vector3(gridSize, curPlayer.GridAltitude, gridSize),
                new Vector3(gridSize, curPlayer.GridAltitude, -gridSize),
                gridColor);
            FrameworkCore.lineRenderer.Draw(
                new Vector3(-gridSize, curPlayer.GridAltitude, gridSize),
                new Vector3(-gridSize, curPlayer.GridAltitude, -gridSize),
                gridColor);
        }


        private void DrawScene(GameTime gameTime, PlayerCommander curPlayer, MotionField mField)
        {
            Camera lockCamera = curPlayer.lockCamera;

            //Draw the skybox
            float skyBrightness = MathHelper.Lerp(0.75f, 1.0f, skyTransition);
            skyBox.Draw(gameTime, lockCamera, skyBrightness);

            // Turn on depth buffering for the models, effects and local backgrounds
            FrameworkCore.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

#if WINDOWS
            if (FrameworkCore.options.renderPlanets)
            {
#endif
                // Draw the local planet(s) and sun(s)
                earth.Draw(gameTime, lockCamera);
#if UNUSED_GRAPHICS
                air.Draw(gameTime, lockCamera);
#endif
                sun.Draw(gameTime, lockCamera);
#if WINDOWS
            }
#endif

            mField.Draw(gameTime, lockCamera);
            

            if (!curPlayer.IsPlaybackMode())
            {
                foreach (Collideable ship in ships)
                {
                    ship.Draw(gameTime, lockCamera, curPlayer);
                }

                // Draw the weapons
                FrameworkCore.Bolts.Draw(gameTime, lockCamera);
                FrameworkCore.debrisManager.Draw(gameTime, curPlayer.lockCamera);
                FrameworkCore.hulkManager.Draw(gameTime, curPlayer.lockCamera, curPlayer);
            }




            DrawGrid(curPlayer);
            
            

            
            FrameworkCore.meshRenderer.EndBatch(curPlayer.lockCamera);

            FrameworkCore.Particles.Camera = curPlayer.lockCamera;
            if (!curPlayer.IsPlaybackMode())
            {
                FrameworkCore.Particles.Draw(gameTime);
            }


            FrameworkCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            if (!isDemo)
            {
                if (!curPlayer.IsPlaybackMode())
                    FrameworkCore.WorldtextManager.Draw(gameTime, curPlayer.lockCamera);

                foreach (Collideable ship in ships)
                {
                    if (!Helpers.IsSpaceship(ship))
                        continue;

                    SpaceShip curShip = (SpaceShip)ship;

                    bool isActionMode = false;
                    if (gameMode == GameMode.Action)
                        isActionMode = true;

                    curShip.DrawShipIcons(gameTime, curPlayer.lockCamera, curPlayer, isActionMode);
                }

                if (gameMode == GameMode.Orders)
                {
                    curPlayer.DrawUI(gameTime, ships);
                }
            }

            FrameworkCore.lineRenderer.EndBatch(curPlayer.lockCamera);
            FrameworkCore.discRenderer.EndBatch(curPlayer.lockCamera);
            FrameworkCore.sphereRenderer.EndBatch(curPlayer.lockCamera);
            FrameworkCore.pointRenderer.EndBatch(curPlayer.lockCamera);

#if DEBUG
            UpdateFramerate(gameTime);
#endif
            FrameworkCore.SpriteBatch.End();

            FrameworkCore.PlayerMeshRenderer.EndBatch(curPlayer.lockCamera);

        }

        private void DrawHUD(GameTime gameTime, PlayerCommander curPlayer)
        {
            FrameworkCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);


            curPlayer.viewportSize = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            curPlayer.Draw(gameTime);
            FrameworkCore.SpriteBatch.End();
        }
        #endregion

        #region Timer
        float actionTransition = 0;
        void DrawTimer()
        {
            if (FrameworkCore.HideHud)
            {
                return;
            }

            if (actionTransition <= 0)
                return;

            if (levelMenuManager.menus.Count > 0)
                return;

            if (FrameworkCore.sysMenuManager.menus.Count > 0)
                return;

            Vector2 timeSize = FrameworkCore.Serif.MeasureString("00:");
            Vector2 timePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2, 85);
            timePos.X -= timeSize.X;

            timePos = Vector2.Lerp(timePos + new Vector2(-50,-100),
                timePos,
                actionTransition);

            float timeAngle = MathHelper.Lerp(0.4f, 0, actionTransition);

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(FrameworkCore.playbackSystem.WorldTimer + 200);
            DateTime dt = new DateTime(timeSpan.Ticks);
            String timeDisplay = dt.ToString("mm:ss");
            Color timeColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, actionTransition);

            Point rectBorder = new Point(24, 4);
            Vector2 timeVec = FrameworkCore.Serif.MeasureString("00:00");
            Rectangle timeRect = new Rectangle(
                (int)timePos.X,
                (int)timePos.Y,
                (int)timeVec.X,
                (int)timeVec.Y);
            timeRect.Width += rectBorder.X;
            timeRect.Height += rectBorder.Y;
            Color rectColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, actionTransition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, timeRect, sprite.roundBox, rectColor, timeAngle,
                new Vector2(rectBorder.X / 2, 0), SpriteEffects.None, 0);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, "" + timeDisplay,
                timePos, timeColor, timeAngle, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
        #endregion

        #region Fade
        private float fadeTransition = 0;

        public void ResetFade()
        {
            fadeTransition = 1;
        }
        #endregion


        #region Draw

        public void Draw(GameTime gameTime)
        {
#if DEBUG
            foreach (Commander enemy in enemies)
            {
                enemy.Draw(gameTime);
            }
#endif



            if (FrameworkCore.players.Count <= 1 || FrameworkCore.gameState != GameState.Play)
            {
                //this line draws the ship meshes in the playback system. Ensure this line
                //precedes the DrawScene call, because DrawScene holds the meshEndBatch call.
                FrameworkCore.players[0].DrawPlaybackShips(gameTime);

                DrawScene(gameTime, FrameworkCore.players[0], motionField);

                if (FrameworkCore.options.bloom)
                    FrameworkCore.Bloomcomponent.Draw(gameTime);


                
                DrawHUD(gameTime, FrameworkCore.players[0]);
            }
            else
            {

                int viewportWidth = 0;
                int windowWidth = 0;
                viewportWidth = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;
                windowWidth = viewportWidth / 2;


                //Left Viewport.
                Viewport viewport = FrameworkCore.Graphics.GraphicsDevice.Viewport;
                viewport.X = 0;
                viewport.Width = windowWidth;
                FrameworkCore.Graphics.GraphicsDevice.Viewport = viewport;

                BlendStateHelper.BeginApply(FrameworkCore.Graphics.GraphicsDevice);
                BlendStateHelper.ColorWriteChannels = ColorWriteChannels.All;
                BlendStateHelper.EndApply(FrameworkCore.Graphics.GraphicsDevice);

                FrameworkCore.players[0].DrawPlaybackShips(gameTime);
                DrawScene(gameTime, FrameworkCore.players[0], motionField);



                BlendStateHelper.BeginApply(FrameworkCore.Graphics.GraphicsDevice);
                BlendStateHelper.ColorWriteChannels = ColorWriteChannels.Blue | ColorWriteChannels.Green | ColorWriteChannels.Red;
                BlendStateHelper.EndApply(FrameworkCore.Graphics.GraphicsDevice);

                
                DrawHUD(gameTime, FrameworkCore.players[0]);
                


                
                //Right Viewport.
                int rightWindowX = windowWidth;
                viewport.X = rightWindowX + 2;
                viewport.Width = windowWidth;
                FrameworkCore.Graphics.GraphicsDevice.Viewport = viewport;

                BlendStateHelper.BeginApply(FrameworkCore.Graphics.GraphicsDevice);
                BlendStateHelper.ColorWriteChannels = ColorWriteChannels.All;
                BlendStateHelper.EndApply(FrameworkCore.Graphics.GraphicsDevice);
                FrameworkCore.players[1].DrawPlaybackShips(gameTime);
                DrawScene(gameTime, FrameworkCore.players[1], motionField2);

                BlendStateHelper.BeginApply(FrameworkCore.Graphics.GraphicsDevice);
                BlendStateHelper.ColorWriteChannels = ColorWriteChannels.Blue | ColorWriteChannels.Green | ColorWriteChannels.Red;
                BlendStateHelper.EndApply(FrameworkCore.Graphics.GraphicsDevice);
                DrawHUD(gameTime, FrameworkCore.players[1]);
                
                

                //reset viewport.
                viewport.X = 0;
                viewport.Width = viewportWidth;
                FrameworkCore.Graphics.GraphicsDevice.Viewport = viewport;

                if (FrameworkCore.options.bloom)
                    FrameworkCore.Bloomcomponent.Draw(gameTime);
            }


            if (isDemo)
                return;

            FrameworkCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            

            DrawTimer();

            if (fadeTransition > 0)
            {
                int alpha = (int)MathHelper.Lerp(0, 255, fadeTransition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                    new Rectangle(0, 0, (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width, (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height),
                    sprite.blank, new Color(0, 0, 0, (byte)alpha));
            }


            if (levelMenuManager.menus.Count <= 0)
                messageQueue.Draw(gameTime);


            


            levelMenuManager.Draw(gameTime);

#if WINDOWS
            if (levelMenuManager.menus.Count > 0 && FrameworkCore.sysMenuManager.menus.Count <= 0)
            {                
                Helpers.DrawMouseCursor(FrameworkCore.SpriteBatch,
                    FrameworkCore.players[0].inputmanager.mousePos);
            }
#endif



            Helpers.DrawSystemMenu(gameTime);




            FrameworkCore.SpriteBatch.End();

            
        }



        #endregion


    }
}