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
    public class Location
    {
        public string name;
        public Vector3 position;
        public Color color;
        public Matrix rotation;

        public Location[] moons;
        public float moonDistance;
        public float moonSpeed;

        public float hoverTransition;
        public float bubbleDisplayAngle;



        public WorldType worldType;
        public float wormholeTransition = 0;

        /// <summary>
        /// Determines whether player has visited this planet.
        /// </summary>
        public bool isExplored = false;

        /// <summary>
        /// Determines whether planet has been discovered by the player.
        /// </summary>
        public bool isVisible = false;

        /// <summary>
        /// Timer controlling the fanfare animation when planet is discovered.
        /// </summary>
        public float visibilityFanfare;

        /// <summary>
        /// Timer controlling the offscreen arrow animation.
        /// </summary>
        public float offScreenArrowTransition = 0;

        public virtual bool CameraVisible(Camera camera)
        {
            foreach (ModelMesh mesh in FrameworkCore.ModelArray[(int)ModelType.planetTiny].Meshes)
            {
                BoundingSphere localSphere = mesh.BoundingSphere;
                localSphere.Center += position;

                ContainmentType contains = camera.BF.Contains(localSphere);
                if (contains == ContainmentType.Contains || contains == ContainmentType.Intersects)
                    return true;
            }

            return false;
        }
    }

    public enum WorldType
    {
        Normal,
        Tutorial,
        Trader,
        Dangerous,
        demoLocked,
        wormhole,
    }

    public class WorldMap
    {
        /// <summary>
        /// How many planets available in the demo.
        /// </summary>
        const int DEMOPLANETS = 8;
        const int PLANETAMOUNT = 48;
        const int MAPWIDTH = 72;
        const int MAPHEIGHT = 24;
        

        public enum WorldState
        {
            /// <summary>
            /// Player can give orders.
            /// </summary>
            Orders,

            /// <summary>
            /// Player is moving to a new planet.
            /// </summary>
            Transition,

            /// <summary>
            /// The Event popup is on-screen (or is transitioning on or off).
            /// </summary>
            Event,

            /// <summary>
            /// Combat is happening (or is transitioning on or off)
            /// </summary>
            Combat,

            Intro,

            ReadyForOrders,
        }



        WorldState worldState;
        public WorldState worldstate
        {
            get { return worldState; }
        }


        CloudManager cloudManager;

        SysMenuManager menuManager;
        public SysMenuManager MenuManager
        {
            get { return menuManager; }
        }

        EventManager eventManager;
        public EventManager evManager
        {
            get { return eventManager; }
        }

        List<Location> locations = new List<Location>();

        public List<Location> Locations
        {
            get { return locations; }
        }

        SkyBox skyBox;
        MotionField motionField;

        Event currentEvent = null;

        Location lastHoverPlanet;
        float introTimer = 0;
        float introTitleTimer = 0;
        

        Location hoverPlanet;

        public readonly Point mapSize = new Point(MAPWIDTH, MAPHEIGHT);
        int numberOfPlanets = PLANETAMOUNT;

        public bool isTutorial
        {
            get
            {
                if (currentLocation == null)
                    return false;

                if (currentLocation.worldType == WorldType.Tutorial)
                    return true;

                return false;
            }
        }

        public WorldMap()
        {
            if (FrameworkCore.isTrialMode())
                gameIsTrial = true;

            menuManager = new SysMenuManager();
            eventManager = new EventManager(menuManager);

            skyBox = new SkyBox(FrameworkCore.Game);
            skyBox.Visible = false;
            FrameworkCore.Game.Components.Add(skyBox);

            motionField = new MotionField(FrameworkCore.Game);
            motionField.Initialize();


            for (int i = 0; i < numberOfPlanets; i++)
            {
                Location newPlanet = new Location();
                newPlanet.position = new Vector3(
                    FrameworkCore.r.Next(-mapSize.X, mapSize.X),  //how WIDE.
                    FrameworkCore.r.Next(-mapSize.Y, mapSize.Y), //how HIGH.
                    FrameworkCore.r.Next(-60, -8)); //how DEEP.

                locations.Add(newPlanet);
            }

            List<Location> deleteList = new List<Location>();

            //ensure none of the planets are too close to one another.
            for (int i = 0; i < locations.Count; i++)
            {
                for (int k = 0; k < locations.Count; k++)
                {
                    //don't compare against self.
                    if (i == k)
                        continue;

                    if (Vector3.Distance(locations[i].position, locations[k].position) < 5f)
                    {
                        deleteList.Add(locations[k]);
                        continue;
                    }
                    
                    //make sure locations have space on the XY axis for easier cursor selection.
                    if (locations[i].position.X > locations[k].position.X - 4 &&
                        locations[i].position.X < locations[k].position.X + 4 &&
                        locations[i].position.Y > locations[k].position.Y - 2 &&
                        locations[i].position.Y < locations[k].position.Y + 2)
                    {
                        deleteList.Add(locations[k]);
                        continue;
                    }
                }
            }

            foreach (Location planet in deleteList)
            {
                locations.Remove(planet);
            }

            //Generate planet details and populate the moons.
            foreach (Location planet in locations)
            {
                planet.name = Helpers.GenerateName("Planet") + Helpers.GenerateName("Planet");
                //planet.name = planet.name.ToUpper();


                //give planet a random color.
                planet.color = getRandomColor();

                //give planet a random angle.
                planet.rotation = Matrix.CreateFromYawPitchRoll(
                    Helpers.randFloat(-3.1f, 3.1f),
                    Helpers.randFloat(-3.1f, 3.1f),
                    Helpers.randFloat(-3.1f, 3.1f));

                int amountOfMoons = FrameworkCore.r.Next(1, 6);
                if (amountOfMoons > 0)
                {
                    planet.moons = new Location[amountOfMoons];
                    float moonDist = Helpers.randFloat(1.0f, 1.5f);
                    for (int k = 0; k < amountOfMoons; k++)
                    {
                        Location newMoon = new Location();
                        newMoon.color = getRandomColor();
                        newMoon.moonSpeed = Helpers.randFloat(1f, 16f);
                        newMoon.moonDistance = moonDist;
                        newMoon.rotation = Matrix.Identity;
                        

                        planet.moons[k] = newMoon;                        

                        moonDist += Helpers.randFloat(0.08f, 0.8f);
                    }
                }
            }


            //populate the Dangerous planets.
            int x = evManager.dangerPoolCount;

            foreach (Location planet in locations)
            {
                if (planet.worldType != WorldType.Normal)
                    continue;

                //bias dangerous planets to the left hemisphere.
                if (planet.position.X > 0 &&
                    FrameworkCore.r.Next(5) > 0)
                    continue;

                planet.worldType = WorldType.Dangerous;

                x--;

                if (x <= 0)
                    break;
            }


            //Populate the trade outpost. The center of the galaxy is 0,0
            if (!FrameworkCore.isTrialMode())
            {
                int midpoint = mapSize.X / 4;
                foreach (Location planet in locations)
                {
                    if (planet.position.X > -midpoint && planet.position.X < 0 &&
                        planet.worldType != WorldType.Tutorial &&
                        planet.worldType != WorldType.wormhole &&
                        planet.worldType != WorldType.Dangerous)
                    {
                        planet.worldType = WorldType.Trader;
                        break;
                    }
                }
            }


            //Populate the DemoPlanets.
            if (FrameworkCore.isTrialMode())
            {
                List<Location> sortedList = locations;

                sortedList.Sort(delegate(Location p1, Location p2) { return p1.position.X.CompareTo(p2.position.X); });

                
                int planetCounter = 0;

                sortedList.ForEach(delegate(Location loc)
                {
                    planetCounter++;

                    if (planetCounter > DEMOPLANETS && loc.worldType != WorldType.Tutorial)
                    {
                        loc.worldType = WorldType.demoLocked;
                    }                    
                });
            }

            
            //generate the strings for player introductions.
            playerFeature1 = string.Format(Resource.GameFeaturingPlayer1,
                Helpers.GenerateName("GameFeatAdj"), FrameworkCore.players[0].commanderName);

            if (FrameworkCore.players.Count > 1)
            {
                playerFeature1 += "\n" + string.Format(Resource.GameFeaturingPlayer2,
                                Helpers.GenerateName("GameFeatAdj"), FrameworkCore.players[1].commanderName);
            }


            for (int i = 0; i < 1; i++)
            {
                Location newPlanet = new Location();
                newPlanet.position = new Vector3(
                    i*10,  //how WIDE.
                    0, //how HIGH.
                    -30); //how DEEP.

                newPlanet.name = Resource.Unknown;
                newPlanet.color = new Color(0,255,255);
                newPlanet.isVisible = true;
                newPlanet.rotation = Matrix.CreateFromYawPitchRoll(
                    Helpers.randFloat(-3.1f, 3.1f),
                    Helpers.randFloat(-3.1f, 3.1f),
                    Helpers.randFloat(-3.1f, 3.1f));
                newPlanet.worldType = WorldType.wormhole;                

                locations.Add(newPlanet);
            }
        }

        Location currentLocation = null;
        Location destinationLocation = null;
        Vector3 currentPosition = Vector3.Zero;

        public bool IsRunningTutorial
        {
            get
            {
                if (this.currentLocation.worldType == WorldType.Tutorial)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Called when a combat encounter is completed. This returns the player to the worldmap state.
        /// </summary>
        public void CombatCompleted()
        {
            List<FleetShip> shipsToDelete = new List<FleetShip>(FrameworkCore.players[0].campaignShips.Count);

            //update the Missions stats of all surviving ships.
            foreach (Collideable collideable in FrameworkCore.level.Ships)
            {
                if (!Helpers.IsSpaceship(collideable))
                    continue;

                SpaceShip ship = (SpaceShip)collideable;

                if (ship.owner == null)
                    continue;

                if (ship.owner.GetType() != typeof(PlayerCommander))
                    continue;

                if (ship.fleetShipInfo == null)
                    continue;

                if (ship.IsDestroyed)
                {
                    //ship is dead. remove it from the roster.                    
                    shipsToDelete.Add(ship.fleetShipInfo);
                    continue;
                }

                //add veterancy once level is done.
                if (currentLocation != null && currentLocation.worldType != WorldType.Tutorial)
                {
                    if (ship.shouldAddVeterancy)
                    {
                        ship.fleetShipInfo.veterancy++;
                    }

                    ship.fleetShipInfo.stats.AddMission();
                }
            }

            //remove any ships that died during the battle.
            foreach (FleetShip ship in shipsToDelete)
            {
                //remove upgrade items associated with this dead ship.
                for (int i = 0; i < ship.upgradeArray.Length; i++)
                {
                    ClearDeadItem(ship.upgradeArray[i]);
                }

                FrameworkCore.players[0].campaignShips.Remove(ship);
            }


            //return player to world state.
            FrameworkCore.gameState = GameState.WorldMap;

            //go to event state so we get the nice camera dolly effect.
            worldState = WorldState.Event;

            //clear the combat encounter data.
            currentEvent = null;

            //enter map.
            EnterMap();

            //do a check to see if player has any ships left.
            if (FrameworkCore.players[0].campaignShips.Count <= 0)
            {
                EndGame();                
            }
            else
            {

                //reward the combat loot.
                RewardCargo();
                
                
            }
        }

        private void RewardCargo()
        {
            //don't reward cargo for tutorial.
            if (currentLocation != null && currentLocation.worldType == WorldType.Tutorial)
            {
                //reset experience points gained from tutorial.
                FrameworkCore.players[0].ResetExperience();


                string exploreString = string.Empty;

#if WINDOWS
                exploreString = Resource.TutorialExplorePC;
#else
                exploreString = Resource.TutorialExploreXBOX;
#endif

                //player just completed the tutorial. pop up a window saying "yay!"
                SysPopup signPrompt = new SysPopup(menuManager, exploreString);
                signPrompt.transitionOnTime = 300;
                signPrompt.transitionOffTime = 200;
                signPrompt.darkenScreen = true;
                signPrompt.hideChildren = false;
                signPrompt.canBeExited = true;
                signPrompt.sideIconRect = sprite.windowIcon.info;

                MenuItem item = new MenuItem(eResource.evTutorial0OK);
                item.Selected += ClosePopup;
                signPrompt.AddItem(item);

                menuManager.AddMenu(signPrompt);

                FrameworkCore.PlayCue(sounds.Fanfare.cheering);

                return;
            }

            eventManager.AddCargo();
        }

        private void ClearDeadItem(InventoryItem item)
        {
            if (item == null)
                return;

            for (int x = FrameworkCore.players[0].inventoryItems.Count - 1; x >= 0; x--)
            {
                if (FrameworkCore.players[0].inventoryItems[x] == null)
                    continue;

                if (item == FrameworkCore.players[0].inventoryItems[x])
                {
                    FrameworkCore.players[0].inventoryItems.RemoveAt(x);
                    break;
                }
            }
        }

        public void EndGame()
        {
            fadeUpTransition = 0;

            //game over!
            menuManager.AddMenu(new GameOverMenu());
        }


        private void DoDemoPlanetCheck()
        {
            if (!FrameworkCore.isTrialMode())
                return;

            bool playerHasValidPlanet = false;
            //now do a sanity check and ensure that there's at least ONE visitable planet for the player.
            foreach (Location loc in locations)
            {
                if (currentLocation != null && currentLocation == loc)
                    continue;

                if (loc.worldType != WorldType.demoLocked && loc.isVisible)
                {
                    playerHasValidPlanet = true;
                }
            }

            //player has no planet to visit. now, we need to find the closest Locked planet and Unlock it.
            if (!playerHasValidPlanet && currentLocation != null)
            {
                int closest = 99999;
                Location closestPlanet = null;

                foreach (Location loc in locations)
                {
                    if (loc.worldType == WorldType.demoLocked)
                    {
                        int distance = (int)Vector3.Distance(currentLocation.position,
                            loc.position);

                        if (distance < closest)
                        {
                            closest = distance;
                            closestPlanet = loc;
                        }
                    }
                }

                if (closestPlanet != null)
                    closestPlanet.worldType = WorldType.Normal;
            }
        }

        public void EnterMap()
        {
            FrameworkCore.MainMenuManager.ClearAll();

            //tell camera that we're no longer using splitscreen.
            Helpers.UpdateCameraProjections(1);

            if (currentLocation == null)
            {
                FrameworkCore.players[0].ResetExperience();
                FrameworkCore.campaignTimer = 0;

                cloudManager = new CloudManager();


                FrameworkCore.PlayCue(sounds.Music.cello);

                //player is entering map for the first time.
                worldState = WorldState.Intro;

                Location leftMostPlanet = locations[0];
                foreach (Location planet in locations)
                {
                    if (planet.position.X < leftMostPlanet.position.X)
                        leftMostPlanet = planet;
                }
                currentLocation = leftMostPlanet;

                //force first planet to be labeled TUTORIAL
                currentLocation.name = Resource.TutorialTutorial;


                //add an initial log entry.                
                string tutorialLog = string.Empty;

                int random = FrameworkCore.r.Next(5);

                if (random == 0)
                    tutorialLog = eResource.logTutorial;
                else if (random == 1)
                    tutorialLog = eResource.logTutorial1;
                else if (random == 2)
                    tutorialLog = eResource.logTutorial2;
                else if (random == 3)
                    tutorialLog = eResource.logTutorial3;
                else
                    tutorialLog = eResource.logTutorial4;

                evManager.AddLog(sprite.eventSprites.destroyers, tutorialLog);

                DoDemoPlanetCheck();
            }
            else
                FrameworkCore.PlayCue(sounds.Music.none);
            
            FrameworkCore.players[0].position = new Vector3(currentLocation.position.X, currentLocation.position.Y - 0.01f, 0);

            FrameworkCore.players[0].Rotation = Quaternion.Identity;
            FrameworkCore.players[0].rotationMatrix = Matrix.CreateFromQuaternion(FrameworkCore.players[0].Rotation);            
        }



        private void UpdateVisibility(Location planet)
        {
            planet.isVisible = true;
            Vector3 planetPos = planet.position;
            planetPos.Z = 0;

            bool foundPlanet = false;

            foreach (Location location in locations)
            {
                //don't check self.
                if (location == planet)
                    continue;

                //already visible and explored. don't bother checking.
                if (location.isExplored && location.isVisible)
                    continue;

                Vector3 locationPos = location.position;
                locationPos.Z = 0;

                if (Vector3.Distance(planetPos, locationPos) <= 24)
                {
                    if (!location.isVisible)
                    {
                        location.isVisible = true;
                        location.visibilityFanfare = 0;
                    }
                    foundPlanet = true;
                }
            }

            if (foundPlanet)
                return;

            Location nearestPlanet = null;

            float nearestDist = 9999;
            //No planets in valid range. So, let's cheat. Reveal the next-nearest planet.
            foreach (Location location in locations)
            {
                if (location.isVisible || location == planet || location.isExplored)
                    continue;

                Vector3 locationPos = location.position;
                locationPos.Z = 0;

                float distance = Vector3.Distance(locationPos, planetPos);
                if (distance < nearestDist)
                {
                    nearestDist = distance;
                    nearestPlanet = location;
                }
            }

            if (nearestPlanet != null)
            {
                nearestPlanet.isVisible = true;
                nearestPlanet.visibilityFanfare = 0;
            }
        }

        

        
        public void EnterCombat(Event eventData)
        {
            FrameworkCore.PlayCue(sounds.Fanfare.ready);

            if (worldState != WorldState.Combat)
                worldState = WorldState.Combat;

            this.currentEvent = eventData;
        }


        private Color getRandomColor()
        {
            Color color = new Color(
                    (byte)FrameworkCore.r.Next(0, 255),
                    (byte)FrameworkCore.r.Next(0, 255),
                    (byte)FrameworkCore.r.Next(0, 255));

            //lighten it up a little.
            color = Color.Lerp(color, Color.White, 0.4f);
            return color;
        }
        

        public void LoadContent()
        {
        }

        public void Initialize()
        {
            
        }

        private void UpdateIntroTitle(GameTime gameTime)
        {
            if (introTitleTimer >= 1)
            {
                if (playerFeature1 != null)
                {
                    playerFeature1 = null;                    
                }

                return;
            }

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(6000).TotalMilliseconds);

            introTitleTimer = MathHelper.Clamp(introTitleTimer + delta, 0, 1);
        }


        Location activeWormhole = null;
        float activeWormholeTransition = 1;
        Vector3 wormholeMoveDir;

        private void StartWormhole()
        {
            //trial mode doesn't have wormholes.
            if (FrameworkCore.isTrialMode())
                return;

            //don't do two wormholes in a row.
            if (currentLocation != null && currentLocation.worldType == WorldType.wormhole)
                return;

            if (eventManager.wormPoolCount <= 0)
                return;

            //small random chance of getting a wormhole event.
            if (FrameworkCore.r.Next(30) > 0)
                return;

            foreach (Location loc in locations)
            {
                if (loc.worldType == WorldType.wormhole)
                {
                    //grab any available wormhole.
                    activeWormhole = loc;
                }
            }

            //no wormholes available.
            if (activeWormhole == null)
                return;

            //move the wormhole close to wehre the player view frustrum is

            //first find the planet closest to center of screen.
            Location closePlanet = closestPlanet();

            if (closePlanet == null)
            {
                activeWormhole = null;
                return;
            }

            activeWormholeTransition = 0;

            int xOffset = FrameworkCore.r.Next(15, 25);
            if (FrameworkCore.r.Next(2) == 0)
                xOffset *= -1;

            int yOffset = FrameworkCore.r.Next(-10, 10);

            activeWormhole.position = closePlanet.position + new Vector3(xOffset, yOffset, -30);

            Vector3 moveDir = closePlanet.position - activeWormhole.position;
            moveDir.Normalize();

            wormholeMoveDir = moveDir;
            

            FrameworkCore.PlayCue(sounds.Fanfare.wormhole);
        }

        private Location closestPlanet()
        {
            int closest = 9999;
            Location closestPlanet = null;

            foreach (Location loc in locations)
            {
                if (loc.worldType == WorldType.wormhole)
                    continue;

                int distance = (int)Vector2.Distance(
                    Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, loc.position),
                    Helpers.GetScreenCenter());

                if (distance < closest)
                {
                    closest = distance;
                    closestPlanet = loc;
                }
            }

            return closestPlanet;
        }

#if DEBUG
        Event[] eventList = new Event[]
            {
                new evAbandoned(),
                new evAbandonZombies(),
                new evAfroDita(),
                new evAssassin(),
                new evAssassinRevenge(),
                new evAurora(),
                new evAuroraResearchBad(),
                new evAuroraResearchGood(),
                new evBabyYetis(),
                new evBandits(),
                new evBitterMelon(),
                new evBitterMelonConcert(),
                new evBruja(),
                new evCasino(),
                new evCasinoJob(),
                new evCasinoRhino(),
                new evCatBounty(),
                new evCatCharity(),
                new evCatGunReward(),
                new evCatGuns(),
                new evCatPrototype(),
                new evCrocodile(),
                new evDeer(),
                new evDeerbruja(),
                new evDeerHitchhikers(),
                new evDeerKatana(),
                new evDeerRevenge(),
                new evDemoEnd(),
                new evDogBoots(),
                new evDogs(),
                new evHippoBuddy(),
                new evHippoPirateBuddy(),
                new evHitchhikerDrop(),
                new evHitchhikerGift(),
                new evHitchhikerRevenge(),
                new evHitchhikers(),
                new evImplementors(),
                new evImplementorsMurder(),
                new evJaguarBuddy(),
                new evKaraoke(),
                new evKaraokeRevenge(),
                new evKatanaShip(),
                new evKoala(),
                new evMurder(),
                new evOwl(),
                new evOwlBribe(),
                new evOwlTattoo(),
                new evPandaFight(),
                new evPandaSlaveBuddy(),
                new evPenguinHitman(),
                new evPenguinReunion(),
                new evPenguins(),
                new evPigReward(),
                new evPigs(),
                new evPirateHostage(),
                new evPirates(),
                new evPrisoner(),
                new evProfessors(),
                new evRuins(),
                new evSlaveFight(),
                new evSlaverRevenge(),
                new evSlavers(),
                new evSpaceHulk(),
                new evSpiderTwain(),
                new evStorm(),
                new evStowaway(),
                new evSwanPolice(),
                new evTerminalDeath(),
                new evToucanRevenge(),
                new evToucanTreasure(),
                new evTutorial(),
                new evUnicorn(),
                new evWine(),
                new evYetiResentment(),
                new evYetiRevenge(),
                new evYetiStarve(),
            };

        int evIndex = 0;
#endif


        Vector2 lastStick = Vector2.Zero;
        float decelerationTransition = 0;

        public void Update(GameTime gameTime)
        {
            
            

            if (FrameworkCore.sysMenuManager.Update(gameTime, FrameworkCore.players[0].inputmanager))
                return;


            cloudManager.Update(gameTime);


            float glowDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                    TimeSpan.FromMilliseconds(1000).TotalMilliseconds);

            planetGlowTransition = MathHelper.Clamp(planetGlowTransition + glowDelta, 0, 1);

            if (planetGlowTransition >= 1)
                planetGlowTransition = 0;



            UpdateIntroTitle(gameTime);

            if (fadeUpTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(500).TotalMilliseconds);
                fadeUpTransition = MathHelper.Clamp(fadeUpTransition + delta, 0, 1);
            }

            //open pause menu.
            if (FrameworkCore.players[0].inputmanager.buttonStartPressed)
            {
                FrameworkCore.sysMenuManager.AddMenu(new PauseMenu());
            }

#if WINDOWS
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                FrameworkCore.sysMenuManager.AddMenu(new PauseMenu());
            }
#endif

            UpdatePlanets(gameTime, FrameworkCore.players[0].lockCamera);

            if (menuManager.Update(gameTime, FrameworkCore.players[0].inputmanager))
            {
                if (worldState == WorldState.Event)
                    UpdateEventCamera(gameTime, true);

                if (hoverPlanet != null)
                    hoverPlanet = null;
                
                return;
            }
            else
            {
                //menu is done. close teh menu, Return to orders state.
                if (worldState == WorldState.Event)
                {
                    if (!UpdateEventCamera(gameTime, false))
                    {
                        //pop up the log after each encounter.
                        if (currentLocation != null && currentLocation.worldType != WorldType.Tutorial && currentLocation.worldType != WorldType.Trader)
                            menuManager.AddMenu(new LogMenu(false, true));



                        if (currentLocation.worldType == WorldType.Trader && !tradeoutpostDone)
                        {
                            tradeoutpostDone = true;
                            eventManager.AddLog(sprite.eventSprites.flamingo, eResource.logFlamingo);
                            menuManager.AddMenu(new LogMenu(false, true));
                        }

                        worldState = WorldState.ReadyForOrders;
                        FrameworkCore.PlayCue(sounds.Music.none);
                    }
                }
                else if (worldState == WorldState.ReadyForOrders)
                {
                    if (FrameworkCore.players[0].planetsVisited >= Helpers.MAXEVENTS - 1 &&
                        !FrameworkCore.isHardcoreMode)
                    {
                        //end-game timer noise.
                        FrameworkCore.PlayCue(sounds.Fanfare.timer);
                    }

                    if (activeWormhole != null)
                    {
                        activeWormhole.isExplored = false;
                        activeWormhole = null;
                    }
                    activeWormholeTransition = 1;

                    StartWormhole();
                    worldState = WorldState.Orders;
                }
            }


            //open the fleetmenu menu.
            if ((FrameworkCore.players[0].inputmanager.OpenMenu || FrameworkCore.players[0].inputmanager.kbSpace) && worldState == WorldState.Orders)
            {
                menuManager.AddMenu(new FleetMenu());
            }



            #region Eventchecker
#if DEBUG
            if (FrameworkCore.players[0].inputmanager.debugF2Pressed)
            {
                //eventManager.AddCargo(new itBeamShield(0.7f));
                eventManager.RunEvent(new evBandits());
            }



            if ((FrameworkCore.players[0].inputmanager.kbIPressed || FrameworkCore.players[0].inputmanager.debugButtonPressed) 
                && worldState == WorldState.Orders)
            {
                //eventManager.RunEvent(eventList[evIndex]);
                //evIndex++;


                //eventManager.RunEvent(new evDeerHitchhikers());


                //Helpers.AddPointBonus(8000);
                //menuManager.AddMenu(new GameOverMenu());


                //eventManager.RunEvent(new evBruja());


                //eventManager.AddShip(shipTypes.BeamFrigate);

                //menuManager.AddMenu(new TradeMenu());
                //menuManager.AddMenu(new GameOverMenu());

                

                //eventManager.AddCargo(new itBeamShield(0.7f));
                eventManager.AddLog(sprite.eventSprites.panda, "Jojo");

                //Helpers.EventRumble();

                


                /*
                foreach (Location p in locations)
                {
                    p.isVisible = true;
                }*/
                

                
                for (int i = 0; i < 16; i++)
                {
                    eventManager.AddLog(sprite.eventSprites.panda, "Jojo");
                }
                
                 
                 
                //menuManager.AddMenu(new GameOverMenu());
            }
#endif
            #endregion



            if (worldState == WorldState.Combat)
            {
                UpdateCombat(gameTime, true);
                return;
            }

            if (worldState == WorldState.Transition)
            {
                UpdateTransition(gameTime);
                return;
            }

            if (worldState == WorldState.Intro)
            {
                UpdateIntro(gameTime);
                return;
            }

            if (worldState != WorldState.Orders)
                return;

            if (warpCameraTransition < 1)
            {
                float smoothWarp = MathHelper.SmoothStep(0, 1, warpCameraTransition);
                FrameworkCore.players[0].position = Vector3.Lerp(
                    warpCameraOrigin, desiredWarpCameraPosition, smoothWarp);

                FrameworkCore.players[0].UpdateCamera(gameTime,
                    FrameworkCore.players[0].position,
                    FrameworkCore.players[0].Rotation);

                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(200).TotalMilliseconds);
                warpCameraTransition = MathHelper.Clamp(warpCameraTransition + delta, 0, 1);

                return;
            }

            //orders.
            if (FrameworkCore.players[0].inputmanager.buttonAPressed)
            {
                GoToPlanet(hoverPlanet);                
                return;
            }

            if (FrameworkCore.players[0].inputmanager.camResetClick)
            {
                if (mouseZoomOut)
                    mouseZoomOut = false;

                WarpCamera(gameTime);
                return;
            }

            if (FrameworkCore.players[0].inputmanager.openLog)
            {
                menuManager.AddMenu(new LogMenu(false, false));
                return;
            }
            

            UpdateCamera(gameTime);

            hoverPlanet = UpdateHover(gameTime);

            

            if (lastHoverPlanet != hoverPlanet && hoverPlanet != null && !hoverPlanet.isExplored)
                FrameworkCore.PlayCue(sounds.click.beep);

            if (lastHoverPlanet != hoverPlanet)
                crosshairTransition = 0;            

            lastHoverPlanet = hoverPlanet;

            if (crosshairTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(200).TotalMilliseconds);
                crosshairTransition = MathHelper.Clamp(crosshairTransition + delta, 0, 1);
            }

#if WINDOWS
            
            if (FrameworkCore.players[0].inputmanager.mouseLeftClick)
            {
                if (hoverPlanet != null)
                {
                    //click on planet.
                    GoToPlanet(hoverPlanet);
                }
            }
            
#endif

            if (!FrameworkCore.isTrialMode() && gameIsTrial)
            {
                //sniff when the player unlocks the full game. if so, then unlock all the planets.
                gameIsTrial = false;

                foreach (Location planet in locations)
                {
                    if (planet.worldType == WorldType.demoLocked)
                    {
                        planet.worldType = WorldType.Normal;
                    }
                }
            }

        }

        bool gameIsTrial = false;

        float crosshairTransition = 1;



        private void OnBuyGame(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.BuyGame();
        }

        private void ClosePopup(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }



        private void UpdateIntro(GameTime gameTime)
        {
            FrameworkCore.players[0].UpdateCamera(gameTime,
                FrameworkCore.players[0].position,
                FrameworkCore.players[0].Rotation);

            //give a little delay before we display the ship.
            if (introTitleTimer < 0.1f)
                return;

            //allow player to skip the intro sequence.
            if ((FrameworkCore.players[0].inputmanager.buttonBPressed || FrameworkCore.players[0].inputmanager.mouseRightClick)
                && introTimer < 0.99f)
            {
                introTimer = 0.99f;
            }


            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(1500).TotalMilliseconds);

            introTimer = MathHelper.Clamp(introTimer + delta, 0, 1);


            if (introTimer >= 1)
            {
                //start gameplay proper.
                //currentLocation.isExplored = true;

                currentLocation.worldType = WorldType.Tutorial;

                UpdateVisibility(currentLocation);

                worldState = WorldState.Orders;
            }
        }

        private void WarpCamera(GameTime gameTime)
        {
            if (currentLocation == null)
                return;

            warpCameraOrigin = FrameworkCore.players[0].lockCamera.CameraPosition;

            desiredWarpCameraPosition = new Vector3(
                currentLocation.position.X,
                currentLocation.position.Y,
                0);

            warpCameraTransition = 0;
        }

        Vector3 warpCameraOrigin = Vector3.Zero;
        Vector3 desiredWarpCameraPosition = Vector3.Zero;
        float warpCameraTransition = 1;

        private bool UpdateEventCamera(GameTime gameTime, bool ZoomIn)
        {
            if (currentLocation == null)
                return false;

            Vector3 desiredPos = new Vector3(
                currentLocation.position.X-1.8f,
                currentLocation.position.Y-0.1f,
                currentLocation.position.Z + 6);

            float smoothTransition = MathHelper.SmoothStep(0, 1, eventZoomTransition);
            
            Vector3 zoomPos = Vector3.Lerp(FrameworkCore.players[0].position,
                desiredPos, smoothTransition);

            FrameworkCore.players[0].UpdateCamera(gameTime, zoomPos,
                FrameworkCore.players[0].Rotation);

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(700).TotalMilliseconds);

            if (ZoomIn)
                eventZoomTransition = MathHelper.Clamp(eventZoomTransition + delta, 0, 1);
            else
                eventZoomTransition = MathHelper.Clamp(eventZoomTransition - delta, 0, 1);

            if (eventZoomTransition > 0)
                return true;

            return false;
        }

        private float combatTransition;
        private bool UpdateCombat(GameTime gameTime, bool ZoomIn)
        {
            if (currentLocation == null)
                return false;

            Vector3 desiredPos = new Vector3(
                currentLocation.position.X,
                currentLocation.position.Y,
                currentLocation.position.Z+2);

            Vector3 zoomPos = Vector3.Lerp(FrameworkCore.players[0].lockCamera.CameraPosition,
                desiredPos, combatTransition);

            FrameworkCore.players[0].UpdateCamera(gameTime, zoomPos,
                FrameworkCore.players[0].Rotation);

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                TimeSpan.FromMilliseconds(400).TotalMilliseconds);

            if (ZoomIn)
                combatTransition = MathHelper.Clamp(combatTransition + delta, 0, 1);
            else
                combatTransition = MathHelper.Clamp(combatTransition - delta, 0, 1);

            if (combatTransition >= 1 && FrameworkCore.gameState == GameState.WorldMap)
            {
                FrameworkCore.gameState = GameState.Play;

                FrameworkCore.level.StartCampaignLevel(gameTime, this.currentEvent);
            }

            if (combatTransition > 0)
                return true;

            return false;
        }


        float eventZoomTransition = 0;

        float moveTransition = 0;

        private void UpdateTransition(GameTime gameTime)
        {
            if (destinationLocation == null)
                return;

            if (currentLocation == null)
                return;

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(1500).TotalMilliseconds);

            moveTransition = MathHelper.Clamp(moveTransition + delta, 0, 1);

            float smoothTransition = MathHelper.SmoothStep(0, 1, moveTransition);
            currentPosition = Vector3.Lerp(currentLocation.position, destinationLocation.position, smoothTransition);

            if (moveTransition >= 1)
            {
                ArriveAtPlanet(destinationLocation);
            }
        }

        private bool IsLastTrialPlanet(Location planet)
        {
            if (!FrameworkCore.isTrialMode())
                return false;

            int candidates = 0;
            Location lastPlanet = null;

            foreach (Location loc in locations)
            {
                if (loc.worldType == WorldType.wormhole)
                    continue;

                if (loc.worldType == WorldType.demoLocked)
                    continue;

                if (!loc.isExplored && loc.isVisible)
                {
                    lastPlanet = loc;
                    candidates++;
                }
            }

            if (candidates > 1)
                return false;

            if (lastPlanet != null && lastPlanet == planet)
                return true;

            return false;
        }

        private bool IsLastPlanet()
        {
            int candidates = 0;
            Location lastPlanet = null;

            foreach (Location loc in locations)
            {
                if (loc.worldType == WorldType.wormhole)
                    continue;

                if (!loc.isExplored && loc.isVisible)
                {
                    lastPlanet = loc;
                    candidates++;
                }
            }

            if (candidates > 1)
                return false;

            return true;
        }

        private void RevealAllPlanets()
        {
            foreach (Location loc in locations)
            {
                if (loc.worldType == WorldType.wormhole)
                    continue;

                if (!loc.isExplored && !loc.isVisible)
                {
                    loc.isVisible = true;                    
                }
            }
        }

        private void ArriveAtPlanet(Location planet)
        {
            if (planet != null)
                UpdateVisibility(planet);

            //if we're visiting an unexplored planet, then trigger a random event.
            if (!planet.isExplored)
            {
                if (planet.worldType != WorldType.Trader)
                    FrameworkCore.players[0].planetsVisited++;

                if (planet.worldType == WorldType.Dangerous)
                {
                    FrameworkCore.players[0].dangerousPlanetsVisited++;
                }

                //do a check to see if the demo should end.                
                if (IsLastTrialPlanet(planet))
                {
                    eventManager.RunEvent(new evDemoEnd());
                    worldState = WorldState.Event;
                }
                else
                {
                    if (IsLastPlanet())
                    {
                        RevealAllPlanets();
                        return;
                    }

                    if (eventManager.StartEvent(planet.worldType))
                    {
                        worldState = WorldState.Event;

                        if (planet.worldType == WorldType.Trader)
                        {
                            menuManager.AddMenu(new TradeMenu());
                        }
                    }
                    else
                        worldState = WorldState.Orders;
                }
            }
            else
                worldState = WorldState.Orders; //player is revisiting an already-explored planet.

            
                

            moveTransition = 0;

            currentLocation = planet;
            currentPosition = currentLocation.position;

            if (planet.worldType != WorldType.Trader)
                currentLocation.isExplored = true;

            destinationLocation = null;
        }

        bool tutorialDone = false; public bool TutorialDone { get { return this.tutorialDone; } }
        bool tradeoutpostDone = false;

        private void GoToPlanet(Location destination)
        {
            if (destination == null)
                return;

            if (destination.worldType == WorldType.demoLocked)
            {
                //demo lock!
                string planetString = string.Format(Resource.MenuDemoPlanetLocked,
                    destination.name);

                SysPopup signPrompt = new SysPopup(menuManager, planetString);
                signPrompt.transitionOnTime = 300;
                signPrompt.transitionOffTime = 200;
                signPrompt.darkenScreen = true;
                signPrompt.hideChildren = false;
                signPrompt.canBeExited = true;
                signPrompt.sideIconRect = sprite.windowIcon.exclamation;

                MenuItem item = new MenuItem(Resource.MenuUnlockFullGame);
                item.Selected += OnBuyGame;
                signPrompt.AddItem(item);

                item = new MenuItem(Resource.MenuDemoPlanetMaybeLater);
                item.Selected += ClosePopup;
                signPrompt.AddItem(item);

                menuManager.AddMenu(signPrompt);

                FrameworkCore.PlayCue(sounds.click.error);

                return;
            }

            if (destination.worldType == WorldType.Trader &&
                currentLocation == destination)
            {


                moveTransition = 0.9f;
                worldState = WorldState.Transition;
                destinationLocation = destination;
                return;
            }

            if (currentLocation.worldType == WorldType.Tutorial && !currentLocation.isExplored &&
                currentLocation == destination)
            {
                //run the tutorial.
                currentLocation.isExplored = true;
                eventManager.RunTutorial();
                tutorialDone = true;
                return;
            }

            //do not let player travel to current Location.
            if (currentLocation != null && currentLocation == destination)
                return;

            //force the tutorial to be marked "done"
            if (!tutorialDone)
            {
                tutorialDone = true;

                foreach (Location loc in locations)
                {
                    if (loc.worldType == WorldType.Tutorial)
                    {
                        loc.isExplored = true;
                        break;
                    }
                }
            }

            //engine whoooossshh
            FrameworkCore.PlayCue(sounds.Worldmap.jet);

            worldState = WorldState.Transition;
            destinationLocation = destination;
            hoverPlanet = null;
            currentPosition = currentLocation.position;
        }

        

        private Location UpdateHover(GameTime gameTime)
        {
#if WINDOWS
            if (FrameworkCore.players[0].inputmanager.mouseRightHeld)
            {
                return null;
            }

            if (fleetButtonTransition > 0)
                return null;
#endif

            Matrix playerOrientation = Matrix.CreateFromQuaternion(FrameworkCore.players[0].Rotation);
            Ray cursorRay =
#if XBOX
                new Ray(FrameworkCore.players[0].position, playerOrientation.Forward);
#else
                Helpers.CalculateCursorRay(FrameworkCore.players[0].inputmanager.mousePos,
                FrameworkCore.players[0].lockCamera.Projection,
                FrameworkCore.players[0].lockCamera.View);
#endif

            float closestIntersection = float.MaxValue;

            Location hitPlanet = null;

            //First, do a polygon check. Is the crosshair directly hitting any of the planets?
            for (int i = 0; i < locations.Count; i++)
            {
                if (!locations[i].isVisible)
                    continue;

                if (locations[i].isExplored)
                    continue;

                //skip wormholes that are inactive.
                if (locations[i].worldType == WorldType.wormhole && locations[i].wormholeTransition <= 0)
                    continue;

                bool insideBoundingSphere;

                Location planet = locations[i];

                Vector3 vertex1, vertex2, vertex3;
                Matrix shipMatrix = Matrix.Identity;
                shipMatrix = planet.rotation;
                shipMatrix.Translation = planet.position;

                // Perform the ray to model intersection test.
                float? intersection = Helpers.RayIntersectsModel(cursorRay, FrameworkCore.ModelPlanet,
                                                         shipMatrix,
                                                         out insideBoundingSphere,
                                                         out vertex1, out vertex2,
                                                         out vertex3);

                // Do we have a per-triangle intersection with this model?
                if (intersection != null)
                {
                    // If so, is it closer than any other model we might have
                    // previously intersected?
                    if (intersection < closestIntersection)
                    {
                        // Store information about this model.
                        closestIntersection = intersection.Value;
                        hitPlanet = locations[i];
                    }
                }
            }

            if (hitPlanet != null)
                return hitPlanet;

            float closestPlanetDist = float.MaxValue;

            Vector2 screenCenter =
#if XBOX
                new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);
#else
                FrameworkCore.players[0].inputmanager.mousePos;
#endif
           

            //Crosshair is not hitting any of the planets. Now, do a ballooned-boundingsphere check.
            for (int i = 0; i < locations.Count; i++)
            {
                if (!locations[i].isVisible)
                    continue;

                //skip wormholes that are inactive.
                if (locations[i].worldType == WorldType.wormhole && locations[i].wormholeTransition <= 0)
                    continue;

                Location planet = locations[i];

                if (planet.isExplored)
                    continue;

                BoundingSphere planetSphere = new BoundingSphere(locations[i].position, 7);

                if (cursorRay.Intersects(planetSphere) < 1024)
                {
                    //we are hitting this planet's sphere. now check how close cursor is to this planet.

                    float curDist = Vector2.Distance(screenCenter,
                        Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, planet.position));

                    if (curDist < closestPlanetDist)
                    {
                        closestPlanetDist = curDist;
                        hitPlanet = planet;
                    }
                }
            }

            return hitPlanet;
        }


        private void UpdatePlanets(GameTime gameTime, Camera lockCam)
        {
            if (worldState == WorldState.Orders)
            {
                if (activeWormholeTransition < 1)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(3500).TotalMilliseconds);
                    activeWormholeTransition = MathHelper.Clamp(activeWormholeTransition + delta,
                        0, 1);

                    if (activeWormhole != null)
                    {
                        activeWormhole.position += wormholeMoveDir * 6f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                else if (activeWormholeTransition >= 1 && activeWormhole != null)
                {
                    activeWormhole = null;
                }
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //foreach (Location planet in locations)
            for (int x = 0; x < locations.Count; x++)            
            {
                Location planet = locations[x];

                //planet.
                planet.rotation = planet.rotation *
                    Matrix.CreateFromAxisAngle(planet.rotation.Up, 0.14f * dt);

                if (planet.moons != null)
                {
                    for (int i = 0; i < planet.moons.Length; i++)
                    {
                        planet.moons[i].rotation = planet.moons[i].rotation *
                            Matrix.CreateFromAxisAngle(planet.rotation.Up, 0.8f * dt);

                        Matrix moonMatrix = planet.rotation *
                            Matrix.CreateFromAxisAngle(planet.rotation.Up,
                            (float)gameTime.TotalGameTime.TotalMinutes * planet.moons[i].moonSpeed);

                        planet.moons[i].position = planet.position + moonMatrix.Right *
                            planet.moons[i].moonDistance;
                    }
                }

                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                if (hoverPlanet != null && hoverPlanet == planet)
                    planet.hoverTransition = MathHelper.Clamp(planet.hoverTransition + delta, 0, 1);
                else
                    planet.hoverTransition = MathHelper.Clamp(planet.hoverTransition - delta, 0, 1);


                if (planet.worldType == WorldType.wormhole)
                {
                    float wormDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(500).TotalMilliseconds);

                    if (activeWormholeTransition >= 1)
                    {
                        planet.wormholeTransition = MathHelper.Clamp(planet.wormholeTransition - delta, 0, 1);
                    }
                    else
                    {
                        if (activeWormhole != null && activeWormhole == planet)
                        {
                            planet.wormholeTransition = MathHelper.Clamp(planet.wormholeTransition + delta, 0, 1);
                        }
                        else
                        {
                            planet.wormholeTransition = MathHelper.Clamp(planet.wormholeTransition - delta, 0, 1);
                        }
                    }
                }



                delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(500).TotalMilliseconds);

                planet.visibilityFanfare = MathHelper.Clamp(planet.visibilityFanfare + delta, 0, 1);


                //update the planet offscreen arrow animation transition.
                if (worldState != WorldState.Orders)
                    continue;

                if (planet.isExplored || !planet.isVisible)
                    continue;

                if (planet.CameraVisible(lockCam))
                {
                    delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(500).TotalMilliseconds);
                    planet.offScreenArrowTransition = MathHelper.Clamp(planet.offScreenArrowTransition - delta, 0, 1);
                }
                else
                {
                    delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(200).TotalMilliseconds);
                    planet.offScreenArrowTransition = MathHelper.Clamp(planet.offScreenArrowTransition + delta, 0, 1);
                }
            }

            
        }

        
        float zoomTransition = 0;

        Vector2 lastMousePos = Vector2.Zero;


        bool mouseZoomOut = false;

        private void UpdateCamera(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputManager inputManager = FrameworkCore.players[0].inputmanager;

            if (inputManager == null)
                return;

            float deadZone = 0.25f;
            float maxEngineSpeed = 10.0f;
            float StrafeOffset = 0;
            float ClimbOffset = 0;

            if (inputManager.turboHeld)
                maxEngineSpeed *= 8;
            


            Vector2 adjustedStick = Vector2.Zero;

            if (inputManager.stickLeft.Length() > deadZone ||
                inputManager.dpadDown || inputManager.dpadUp || inputManager.dpadLeft || inputManager.dpadRight)
            {
                Vector2 stickDirection = inputManager.stickLeft;

                if (inputManager.dpadLeft)
                    stickDirection.X = -1;
                else if (inputManager.dpadRight)
                    stickDirection.X = 1;

                if (inputManager.dpadUp)
                    stickDirection.Y = 1;
                else if (inputManager.dpadDown)
                    stickDirection.Y = -1;

                lastStick = stickDirection;

                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(500).TotalMilliseconds);
                decelerationTransition = MathHelper.Clamp(decelerationTransition + delta, 0, 1);

                adjustedStick = stickDirection;
            }
            else
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(150).TotalMilliseconds);
                decelerationTransition = MathHelper.Clamp(decelerationTransition - delta, 0, 1);

                adjustedStick = Vector2.Lerp(Vector2.Zero, lastStick, decelerationTransition);
            }


            if (Math.Abs(adjustedStick.Y) > deadZone)
                ClimbOffset = adjustedStick.Y;

            if (inputManager.cameraLower > deadZone)
                ClimbOffset = -1.5f;
            if (inputManager.cameraRaise > deadZone)
                ClimbOffset = 1.5f;


            if (Math.Abs(adjustedStick.X) > deadZone)
                StrafeOffset = adjustedStick.X;


            if (inputManager.mouseWheelDown)
                mouseZoomOut = true;
            else if (inputManager.mouseWheelUp)
                mouseZoomOut = false;


            if (inputManager.stickRight.Y < -deadZone || mouseZoomOut)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(500).TotalMilliseconds);
                zoomTransition = MathHelper.Clamp(zoomTransition + delta, 0, 1);
            }
            else
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(400).TotalMilliseconds);
                zoomTransition = MathHelper.Clamp(zoomTransition - delta, 0, 1);
            }
            

            float climbSpeed = ClimbOffset * maxEngineSpeed;
            float strafeSpeed = StrafeOffset * maxEngineSpeed;

            Matrix m = Matrix.CreateFromQuaternion(FrameworkCore.players[0].Rotation);
            Vector3 upForce = m.Up * climbSpeed;
            Vector3 strafeForce = m.Right * strafeSpeed;

#if WINDOWS
            if (inputManager.mouseRightHeld)
            {
                if (inputManager.mouseRightStartHold)
                {
                    lastMousePos = inputManager.mousePos;
                }

                if (inputManager.mouseHasMoved)
                {
                    int climbMod = (int)(128 * Math.Abs(lastMousePos.Y - inputManager.mousePos.Y));                    
                    if (inputManager.mousePos.Y < lastMousePos.Y)
                    {
                        upForce = m.Up * (-climbMod * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    else if (inputManager.mousePos.Y > lastMousePos.Y)
                    {
                        upForce = m.Up * (climbMod * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }

                    int panMod = (int)(128 * Math.Abs(lastMousePos.X - inputManager.mousePos.X));
                    if (inputManager.mousePos.X < lastMousePos.X)
                    {
                        strafeForce = m.Left * (-panMod * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    else if (inputManager.mousePos.X > lastMousePos.X)
                    {
                        strafeForce = m.Left * (panMod * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }

                    lastMousePos = inputManager.mousePos;
                }
            }
            else if (FrameworkCore.isActive &&
                inputManager.mousePos.X > FrameworkCore.Graphics.GraphicsDevice.Viewport.X -1 &&
                inputManager.mousePos.X < FrameworkCore.Graphics.GraphicsDevice.Viewport.X + FrameworkCore.Graphics.GraphicsDevice.Viewport.Width &&
                inputManager.mousePos.Y > FrameworkCore.Graphics.GraphicsDevice.Viewport.Y -1 &&
                inputManager.mousePos.Y < FrameworkCore.Graphics.GraphicsDevice.Viewport.Y + FrameworkCore.Graphics.GraphicsDevice.Viewport.Height)
            {
                int panSpeed = 768;
                //int threshold = (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.19f);
                int threshold = 90;
                if (FrameworkCore.players[0].inputmanager.mousePos.Y < threshold)
                {
                    upForce = m.Up * (panSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if (FrameworkCore.players[0].inputmanager.mousePos.Y > FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - threshold)
                {
                    upForce = m.Up * (-panSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }

                if (FrameworkCore.players[0].inputmanager.mousePos.X < threshold)
                {
                    strafeForce = m.Left * (panSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if (FrameworkCore.players[0].inputmanager.mousePos.X > FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - threshold)
                {
                    strafeForce = m.Left * (-panSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            if (fleetButtonRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(200).TotalMilliseconds);

                fleetButtonTransition = MathHelper.Clamp(fleetButtonTransition + delta, 0, 1);


                if (inputManager.mouseLeftClick)
                {
                    menuManager.AddMenu(new FleetMenu());
                }
            }
            else
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(400).TotalMilliseconds);

                fleetButtonTransition = MathHelper.Clamp(fleetButtonTransition - delta, 0, 1);
            }
            
#endif

            //strafe/climb.
            //mapSize
            Vector3 tempPos = FrameworkCore.players[0].position + (upForce + strafeForce) * dt;

            int mapSizeBuffer = 8;
            if (tempPos.X > -mapSize.X - mapSizeBuffer && tempPos.X < mapSize.X + mapSizeBuffer &&
                tempPos.Y > -mapSize.Y - mapSizeBuffer && tempPos.Y < mapSize.Y + mapSizeBuffer)
            {
                FrameworkCore.players[0].position = tempPos;
            }

            //zoom controls.
            float smoothTransition = MathHelper.SmoothStep(0, 1, zoomTransition);
            FrameworkCore.players[0].position.Z = MathHelper.Lerp(0, 60, smoothTransition);

            FrameworkCore.players[0].UpdateCamera(gameTime,
            FrameworkCore.players[0].position,
            FrameworkCore.players[0].Rotation);
        }



        private void DrawMoon(Location planet, Camera lockCam)
        {
            if (planet.moons == null)
                return;

            for (int i = 0; i < planet.moons.Length; i++)
            {
                Matrix worldMatrix = planet.moons[i].rotation;
                worldMatrix.Translation = planet.moons[i].position;

                FrameworkCore.meshRenderer.Draw(ModelType.planetTiny, worldMatrix,
                    lockCam, planet.moons[i].color);
            }
        }



        Rectangle fleetButtonRect = Rectangle.Empty;

        public Rectangle FleetButtonRect
        {
            get { return fleetButtonRect; }
        }

        float fleetButtonTransition = 0;

        float planetGlowTransition = 0;

        private void DrawPadlocks(GameTime gameTime)
        {
            if (worldState != WorldState.Orders)
                return;

            if (menuManager.menus.Count > 0)
                return;

            foreach (Location planet in locations)
            {
                //skip inactive wormholes.
                if (planet.worldType == WorldType.wormhole && planet.wormholeTransition <= 0)
                    continue;

                if (planet.worldType == WorldType.demoLocked)
                {
                    float sizeInPixels = Helpers.SizeInPixels(FrameworkCore.players[0].lockCamera, planet.position, 0.5f);
                    sizeInPixels += 2;

                    Vector2 lockPos = Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, planet.position);
                    lockPos.Y -= sizeInPixels;

                    lockPos.Y -= sprite.padlock2.Height / 2;

                    float blackSize = 1.5f + Helpers.Pulse(gameTime, 0.1f, 6);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, lockPos,
                        sprite.glow, Color.Black, 0, Helpers.SpriteCenter(sprite.glow), blackSize, SpriteEffects.None, 0);

                    Color padlockColor = Color.White;

                    if (!planet.isVisible)
                    {
                        padlockColor = Color.Gray;
                    }
                    else
                    {
                        padlockColor = Color.Lerp(Color.White, Color.Orange, planet.hoverTransition);
                    }


                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, lockPos,
                        sprite.padlock2, padlockColor, 0, Helpers.SpriteCenter(sprite.padlock2), 1, SpriteEffects.None, 0);
                }
                else if (planet.worldType == WorldType.Trader)
                {
                    if (!planet.isVisible)
                        continue;

                    float sizeInPixels = Helpers.SizeInPixels(FrameworkCore.players[0].lockCamera, planet.position, 0.5f);
                    sizeInPixels += 6;

                    Vector2 lockPos = Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, planet.position);
                    lockPos.Y -= sizeInPixels;
                    //lockPos.Y -= sprite.icons.flamingo.Height / 2;

                    Vector2 flamOrigin = new Vector2(sprite.icons.flamingo.Width / 2, sprite.icons.flamingo.Height);

                    float flamAngle = Helpers.Pulse(gameTime, 0.05f, 4);

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, lockPos,
                        sprite.icons.flamingo, new Color(255, 130, 230), flamAngle, flamOrigin,
                        1, SpriteEffects.None, 0);
                }
                else
                {
                    if (planet.isExplored)
                        continue;

                    if (!planet.isVisible)
                        continue;


                    float sizeInPixels = Helpers.SizeInPixels(FrameworkCore.players[0].lockCamera, planet.position, 0.5f);
                    sizeInPixels += 6;

                    Vector2 lockPos = Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, planet.position);
                    lockPos.Y -= sizeInPixels;


                    Color arrowColor = Color.White;

                    if (!planet.isVisible)
                    {
                        arrowColor = Color.Gray;
                    }
                    else
                    {
                        arrowColor = Color.Lerp(Color.White, Color.Orange, planet.hoverTransition);
                    }

                    float arrowSize = Helpers.PopLerp(planet.hoverTransition, 0.4f, 1.5f, 0.9f);

                    int bounceSpeed = 12;



                    if (planet.hoverTransition > 0)
                        lockPos.Y += Helpers.Pulse(gameTime, 4, bounceSpeed);

                    

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, lockPos,
                        sprite.arrow, arrowColor, -1.57f, new Vector2(6, 16), arrowSize, SpriteEffects.None, 0);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            Camera lockCam = FrameworkCore.players[0].lockCamera;
            skyBox.Draw(gameTime, lockCam, 0.5f);
            motionField.Draw(gameTime, lockCam);

            FrameworkCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);


            cloudManager.Draw(gameTime);


            DrawPadlocks(gameTime);


            foreach(Location planet in locations)
            {
                if (!planet.CameraVisible(lockCam))
                    continue;

                //skip inactive wormholes.
                if (planet.worldType == WorldType.wormhole && planet.wormholeTransition <= 0)
                    continue;

                Matrix worldMatrix = planet.rotation;
                worldMatrix.Translation = planet.position;

                //the planet glow.                
                float gSize = Helpers.SizeInPixels(lockCam, planet.position, 0.5f);
                gSize = Math.Max(gSize / 5.0f, 0.4f);

                if (planet.isVisible && !planet.isExplored)
                    gSize += Helpers.Pulse(gameTime, 0.3f, 5);

                Color gColor = new Color(255, 255, 255, 32);

                if (planet.worldType == WorldType.Dangerous)
                {
                    gColor = new Color(255, 70, 70, 48);
                }

                if (!planet.isVisible)
                    gColor = new Color(0, 0, 0, 128);

                if (!planet.isExplored)
                {
                    if (planetGlowTransition > 0 && planet.isVisible)
                    {
                        float glowSize = Helpers.SizeInPixels(lockCam, planet.position, 0.5f);
                        glowSize /= 5.0f;

                        glowSize *= MathHelper.Lerp(0.5f, 2.5f, planetGlowTransition);
                        
                        Color glowColor = new Color(255, 200, 120, 96);

                        if (planet.worldType == WorldType.Dangerous)
                        {
                            glowColor = new Color(230, 70, 70, 128);
                        }

                        glowColor = Color.Lerp(glowColor, Helpers.transColor(glowColor), planetGlowTransition);

                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                            sprite.glow, glowColor, 0, Helpers.SpriteCenter(sprite.glow), glowSize, SpriteEffects.None, 0);
                    }


                    float sparkleSize = gSize;
                    sparkleSize *= 0.3f;
                    float sparkleAngle = (float)gameTime.TotalGameTime.TotalSeconds * 0.5f;
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                            sprite.sparkle, gColor, sparkleAngle, Helpers.SpriteCenter(sprite.sparkle), sparkleSize, SpriteEffects.None, 0);

                    sparkleSize *= 0.7f;
                    sparkleAngle = (float)gameTime.TotalGameTime.TotalSeconds * -0.3f;
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                            sprite.sparkle, gColor, sparkleAngle, Helpers.SpriteCenter(sprite.sparkle), sparkleSize, SpriteEffects.None, 0);
                }

                if (planet.hoverTransition > 0)
                {
                    float hSize = Helpers.SizeInPixels(lockCam, planet.position, 0.5f);
                    hSize = Math.Max(hSize / 12.0f, 0.4f);
                    hSize = Helpers.PopLerp(planet.hoverTransition, 0, hSize + 2f, hSize);
                    float hoverGlow = MathHelper.Lerp(0, hSize, planet.hoverTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                        sprite.glow, Color.White, 0, Helpers.SpriteCenter(sprite.glow), hoverGlow, SpriteEffects.None, 0);
                }
                
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                        sprite.glow, gColor, 0, Helpers.SpriteCenter(sprite.glow), gSize, SpriteEffects.None, 0);
                


                //render the planet mesh.
                if (planet.worldType != WorldType.wormhole)
                    FrameworkCore.meshRenderer.Draw(ModelType.planet, worldMatrix, lockCam, planet.color);

                DrawMoon(planet, lockCam);

                DrawPlanetLabel(gameTime, planet, lockCam);

                DrawOrbitalRings(planet, lockCam);

                if (planet.visibilityFanfare < 1)
                {
                    //draw the discovery fanfare animation.
                    Color glowColor = Color.Lerp(new Color(255,255,255,64), OldXNAColor.TransparentWhite, planet.visibilityFanfare);
                    float glowSize = MathHelper.Lerp(1, 6, planet.visibilityFanfare);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                        sprite.glow, glowColor, 0, Helpers.SpriteCenter(sprite.glow), glowSize, SpriteEffects.None, 0);
                }

                if (planet.worldType == WorldType.wormhole)
                {
                    Color wormColor = Color.Lerp(new Color(0, 255, 255), new Color(192,255,255), planet.hoverTransition);

                    wormColor = Color.Lerp(Helpers.transColor(wormColor), wormColor, planet.wormholeTransition);

                    for (int i = 0; i < 2; i++)
                    {
                        float wormSize = MathHelper.Lerp(1, 0.4f, planet.wormholeTransition);
                        float wormAngle = 0;

                        if (i == 0)
                        {
                            wormSize += Helpers.Pulse(gameTime, 0.1f, 4);
                            wormAngle = (float)gameTime.TotalGameTime.TotalSeconds * 0.5f;
                        }
                        else
                        {
                            wormSize += Helpers.Pulse(gameTime, -0.1f, 4);
                            wormAngle = (float)gameTime.TotalGameTime.TotalSeconds * -0.5f;
                        }

                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                            sprite.cloud, wormColor, wormAngle, Helpers.SpriteCenter(sprite.cloud), wormSize, SpriteEffects.None, 0);
                    }

                    float sparkleSize = MathHelper.Lerp(1.5f, 1, planet.wormholeTransition);
                    float sparkleAngle = (float)gameTime.TotalGameTime.TotalSeconds;
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                        sprite.sparkle, wormColor, sparkleAngle, Helpers.SpriteCenter(sprite.sparkle), sparkleSize, SpriteEffects.None, 0);

                    //draw a tiny sparkle in the middle
                    sparkleSize = MathHelper.Lerp(1, 0.3f, planet.wormholeTransition);
                    sparkleAngle = (float)gameTime.TotalGameTime.TotalSeconds * 2;
                    Color centerColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, planet.wormholeTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, Helpers.GetScreenPos(lockCam, planet.position),
                        sprite.sparkle, centerColor, sparkleAngle, Helpers.SpriteCenter(sprite.sparkle), sparkleSize, SpriteEffects.None, 0);
                }
            }
            FrameworkCore.SpriteBatch.End();

            FrameworkCore.meshRenderer.EndBatch(lockCam);
            
            


            FrameworkCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);



            FrameworkCore.discRenderer.EndBatch(lockCam);

            //DrawPlanetRing(gameTime, lockCam);

            
#if XBOX
            DrawBubbles(lockCam);
            DrawLegend();
#endif

            //draw label for the selected planet.
            if (hoverPlanet != null)
            {
                DrawPlanetLabel(gameTime, hoverPlanet, lockCam);                
            }

            DrawMoveLine(gameTime);

            //DrawPossibleMoveLines();

            Vector2 playerPos = Vector2.Zero;

            if (worldState == WorldState.Orders && currentLocation != null)
            {
                playerPos = Helpers.GetScreenPos(lockCam, currentLocation.position);
            }
            else if (worldState == WorldState.Transition)
            {
                if (destinationLocation != null)
                {
                    playerPos = Helpers.GetScreenPos(lockCam, currentPosition);
                    Helpers.DrawDottedLine(gameTime, currentPosition, destinationLocation.position,
                        new Color(255, 160, 0), new Color(255, 160, 0), 0.9f, 0.6f);
                }
            }
            else if (worldState == WorldState.Event)
            {
                if (currentLocation != null)
                    playerPos = Helpers.GetScreenPos(lockCam, currentLocation.position);
            }
            
            

            if (playerPos != Vector2.Zero)
                DrawCurrentLocation(gameTime, lockCam, playerPos);

            FrameworkCore.pointRenderer.EndBatch(lockCam);
            FrameworkCore.lineRenderer.EndBatch(lockCam);

            DrawOffscreenArrows(gameTime, lockCam);




            if (worldState == WorldState.Orders && menuManager.menus.Count <= 0)
                DrawFleetButton(fleetButtonTransition);

            if (worldState == WorldState.Orders && menuManager.menus.Count <= 0)
                DrawCrosshair(gameTime, lockCam);


            if (worldState == WorldState.Combat)
            {
                //fade screen out when entering combat state.
                DrawCombat();
            }

            DrawIntroTitle();



            if (menuManager.menus.Count > 0)
            {
                menuManager.Draw(gameTime);

#if WINDOWS
                Helpers.DrawMouseCursor(FrameworkCore.SpriteBatch,
                    FrameworkCore.players[0].inputmanager.mousePos);
#endif
            }
            



            

            if (FrameworkCore.sysMenuManager.menus.Count > 0)
                Helpers.DrawSystemMenu(gameTime);

            if (fadeUpTransition < 1)
            {
                int alpha = (int)MathHelper.Lerp(255, 0, fadeUpTransition);
                Helpers.DarkenScreen(alpha);
            }



#if DEBUG
            UpdateFramerate(gameTime);
#endif

            FrameworkCore.SpriteBatch.End();

            if (FrameworkCore.sysMenuManager.menus.Count <= 0)
                FrameworkCore.PlayerMeshRenderer.EndBatch(lockCam, true);

            if (worldState == WorldState.Intro)
                DrawIntroShip(gameTime);
        }

        float fadeUpTransition = 0;


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






        public void DrawFleetButton(float Transition)
        {   
            


            Vector2 buttonPos = new Vector2(
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 120 - sprite.buttons.rb.Width,
                120);


            Vector2 textPos = buttonPos;
            Vector2 textVec = FrameworkCore.Serif.MeasureString(Resource.MenuFleet);
            textPos.X -= textVec.X + 8;
            textPos.Y += sprite.buttons.rb.Height / 2;
            textPos.Y -= textVec.Y / 2;

            //draw bg rectangle.
            Color buttonColor = Color.Lerp(Color.Black, FrameworkCore.players[0].TeamColor, 0.7f);

#if WINDOWS
            buttonColor = Color.Lerp(buttonColor, FrameworkCore.players[0].TeamColor, Transition);
#endif

            Rectangle bgRect = new Rectangle(
                (int)textPos.X - 8,
                (int)buttonPos.Y,
                (int)textVec.X + 16 + sprite.buttons.rb.Width,
                sprite.buttons.rb.Height);
            bgRect.Inflate(2, 2);
            Rectangle boundBox = bgRect;
            boundBox.Inflate(8, 8);
            fleetButtonRect = boundBox;

#if WINDOWS
            int inflate = (int)MathHelper.Lerp(0, 6, Transition);
            bgRect.Inflate(inflate, inflate);

            if (Transition > 0)
            {
                Color glowColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
                Rectangle glowRect = bgRect;
                glowRect.Inflate(5, 5);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, glowRect, sprite.vistaBox,
                    glowColor);
            }
#endif


            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, bgRect, sprite.vistaBox,
                buttonColor);


            

#if XBOX
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, sprite.buttons.rb, Color.White);
#endif

            Color textColor = Color.White;

#if WINDOWS
            textColor = Color.Lerp(Color.White, Color.Orange, Transition);
#endif

            //Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuFleet, textPos, Color.White, new Color(0, 0, 0, 64));

            string fleetText = Resource.MenuFleet;

#if WINDOWS
            fleetText += " " + Helpers.GetShortcutAltKey();
#endif

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, fleetText, textPos, textColor);
        }

        private void DrawIntroTitle()
        {
            if (worldState == WorldState.Combat)
                return;

            if (introTitleTimer >= 1)
                return;

            int midPoint = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2;
            
            Vector2 titlePos = Vector2.Lerp(
                new Vector2(90, midPoint), 
                new Vector2(150, midPoint),
                introTitleTimer);


            float sizeMod = MathHelper.Lerp(0, 0.2f, introTitleTimer);

            Color textColor = Color.White;
            Color subtitleColor = Color.Orange;

            if (introTitleTimer > 0.8f)
            {
                float adjustedTime = 1.0f - introTitleTimer;
                adjustedTime *= 5f;
                textColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, adjustedTime);
                subtitleColor = Color.Lerp(Helpers.transColor(subtitleColor), subtitleColor, adjustedTime);
            }


            Vector2 preTitleVec = FrameworkCore.Serif.MeasureString(Resource.MenuTitleByline);
            preTitleVec.X = 0;
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuTitleByline,
                titlePos, textColor, 0, preTitleVec, 1 + sizeMod, SpriteEffects.None, 0);

            titlePos = Vector2.Lerp(
                            new Vector2(90, midPoint),
                            new Vector2(200, midPoint),
                            introTitleTimer);

            float gothicSize = 1.2f + sizeMod;
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, Resource.MenuTitle,
                titlePos, textColor, 0, Vector2.Zero, gothicSize, SpriteEffects.None, 0);

            if (playerFeature1 == null)
                return;

            Vector2 gothicVec = FrameworkCore.Gothic.MeasureString("S");
            gothicVec.Y *= gothicSize;

            Vector2 subtitlePos = titlePos;
            subtitlePos.Y += gothicVec.Y;
            subtitlePos.Y -= 16;            
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, playerFeature1,
                subtitlePos, subtitleColor, 0, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
        }

        string playerFeature1;

        private void DrawIntroShip(GameTime gameTime)
        {
            if (currentLocation == null)
                return;

            
            
            Vector3 startPos = FrameworkCore.players[0].position;
            startPos.Z += 24;
            startPos.X -= 3;
            startPos.Y += 1;
            Vector3 destination = currentLocation.position;
            destination.Z -= 128;
            Vector3 shipPos = Vector3.Lerp(startPos,
                destination, introTimer);

            Matrix worldMatrix = Matrix.CreateRotationZ(1.3f + (float)gameTime.TotalGameTime.TotalSeconds * -0.3f);
            worldMatrix.Translation = shipPos;

            FrameworkCore.PlayerMeshRenderer.Draw(ModelType.shipDestroyer, worldMatrix, null,
                FrameworkCore.players[0].factionName.teamColor);
        }

        private void DrawCombat()
        {
            int alpha = (int)MathHelper.Lerp(0, 255, combatTransition);
            Helpers.DarkenScreen(alpha);
        }

        private void DrawOffscreenArrows(GameTime gameTime, Camera lockCam)
        {
            if (worldState != WorldState.Orders)
                return;

            int safeScreen = 80;

            int height = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height;
            int width = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;

            Line leftLine = new Line(new Vector2(safeScreen, safeScreen),
                new Vector2(safeScreen, height - safeScreen));

            Line rightLine = new Line(new Vector2(width - safeScreen, safeScreen),
                new Vector2(width - safeScreen, height - safeScreen));

            Line topLine = new Line(new Vector2(safeScreen, safeScreen),
                new Vector2(width - safeScreen, safeScreen));

            Line bottomLine = new Line(new Vector2(safeScreen, height - safeScreen),
                new Vector2(width - safeScreen, height - safeScreen));

            foreach(Location planet in locations)
            {
                //don't draw arrows for padlocked planets.
                if (planet.worldType == WorldType.demoLocked)
                    continue;

                //skip wormholes that are inactive.
                if (planet.worldType == WorldType.wormhole && planet.wormholeTransition <= 0)
                    continue;

                //if (planet.CameraVisible(lockCam))
                //    continue;
                if (planet.offScreenArrowTransition <= 0)
                    continue;

                if (!planet.isVisible || planet.isExplored)
                    continue;

                Vector2 screenPos = Helpers.GetScreenPos(lockCam, planet.position);

                Vector2 screenCenter = Helpers.GetScreenCenter();

                Vector2 direction = screenPos - screenCenter;

                direction.Normalize();
                float adjacent = direction.X;
                float opposite = direction.Y;
                float TargetAngle = (float)System.Math.Atan2(opposite, adjacent);

                Vector2 arrowPos = Vector2.Zero;
                Line planetLine = new Line(screenCenter, screenPos);

                arrowPos = Helpers.IntersectionPoint(planetLine, leftLine);

                if (arrowPos == Vector2.Zero)
                    arrowPos = Helpers.IntersectionPoint(planetLine, rightLine);

                if (arrowPos == Vector2.Zero)
                    arrowPos = Helpers.IntersectionPoint(planetLine, topLine);

                if (arrowPos == Vector2.Zero)
                    arrowPos = Helpers.IntersectionPoint(planetLine, bottomLine);

                if (arrowPos == Vector2.Zero)
                    continue;

                Vector2 arrowOrigin = new Vector2(sprite.tinyArrow.Width, sprite.tinyArrow.Height/2f);

                float dist = Vector2.Distance(arrowPos, screenPos);

                int minDist = 150;
                int maxDist = 500;
                dist = MathHelper.Clamp(dist, minDist, maxDist);
                dist -= minDist;
                dist /= (maxDist-minDist);

                Color arrowColor = Color.Lerp(new Color(255, 255, 255, 255), new Color(255, 255, 255, 64), dist);

                arrowColor.A = (byte)MathHelper.Clamp(arrowColor.A +
                    Helpers.Pulse(gameTime, 32, 12),
                    0, 255);

                float arrowSize = 1.5f + Helpers.Pulse(gameTime, 0.1f, 8);
                arrowSize = Helpers.PopLerp(planet.offScreenArrowTransition, 0, 3, 1.5f);

                arrowSize += Helpers.Pulse(gameTime, 0.07f, 10);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, arrowPos, sprite.tinyArrow,
                    arrowColor, TargetAngle, arrowOrigin, arrowSize, SpriteEffects.None, 0);
            }
        }

        private void DrawPossibleMoveLines()
        {
            if (worldState != WorldState.Orders)
                return;

            if (currentLocation == null)
                return;

            foreach (Location planet in locations)
            {
                if (!planet.isVisible)
                    continue;

                if (planet.isExplored)
                    continue;

                if (planet == currentLocation)
                    continue;

                if (hoverPlanet != null && hoverPlanet == planet)
                    continue;

                Color lineColor = new Color(80, 80, 80);

                if ((hoverPlanet != null && hoverPlanet != currentLocation) || hoverPlanet == null)
                    lineColor = new Color(80, 80, 80, 96);

                FrameworkCore.lineRenderer.Draw(currentLocation.position, planet.position,
                    lineColor);
            }
        }

        private void DrawMoveLine(GameTime gameTime)
        {
            if (currentLocation == null)
                return;

            if (hoverPlanet == null)
                return;

            Vector3 startPos = currentLocation.position;
            Vector3 endPos = Vector3.Lerp(currentLocation.position, hoverPlanet.position, hoverPlanet.hoverTransition);

            Vector3 lineDir = endPos - startPos;
            lineDir.Normalize();
            //startPos += lineDir * Helpers.Pulse(gameTime, 0.4f, 5);

            Color lineColor = new Color(255,160,0);

            Helpers.DrawDottedLine(gameTime, startPos,
                endPos, lineColor, lineColor, 0.9f, 0.6f, 1);
        }

        

        private void DrawCurrentLocation(GameTime gameTime, Camera lockCam, Vector2 pos)
        {
            if (FrameworkCore.players[0].campaignShips.Count <= 0)
                return;
            
            int pulseRate = 8;

            float pulse = Helpers.Pulse(gameTime, 1, pulseRate);
            if (pulse < 0)
                return;

            if (currentLocation == null)
                return;

            

            for (int i = 0; i < FrameworkCore.players[0].campaignShips.Count; i++)
            {
                Vector2 finalDotPos = pos;

                if (worldState != WorldState.Transition)
                {
                    float sizeInPixels = Helpers.SizeInPixels(lockCam, currentLocation.position, 0.5f);
                    sizeInPixels += 4;

                    float angleModifier = i * 0.8f;

                    float x = (float)Math.Cos((gameTime.TotalGameTime.TotalSeconds / 2f) + angleModifier) * sizeInPixels;
                    float y = (float)Math.Sin((gameTime.TotalGameTime.TotalSeconds / 2f) + angleModifier) * sizeInPixels;

                    finalDotPos += new Vector2(x, y);
                }
                else
                {
                    //in transit! draw a conga-line of ships.
                    if (destinationLocation != null && currentLocation != null)
                    {
                        Vector2 moveDir = Helpers.GetScreenPos(lockCam, destinationLocation.position) -
                            Helpers.GetScreenPos(lockCam, currentLocation.position);

                        moveDir.Normalize();

                        finalDotPos += moveDir * (i* -16.0f);
                    }
                }

                float glowSize = 1.1f + Helpers.Pulse(gameTime, 0.5f, pulseRate);                
                Color glowColor = Faction.Blue.teamColor;

                if (FrameworkCore.players[0].campaignShips[i].childShip)
                    glowColor = Faction.Blue.altColor;

                glowColor.A = 128;
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, finalDotPos, sprite.glow,
                    glowColor, 0, Helpers.SpriteCenter(sprite.glow), glowSize, SpriteEffects.None, 0);

                glowSize = 0.4f + Helpers.Pulse(gameTime, 0.3f, pulseRate);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, finalDotPos, sprite.sparkle,
                    glowColor, (float)gameTime.TotalGameTime.TotalSeconds, Helpers.SpriteCenter(sprite.sparkle), glowSize, SpriteEffects.None, 0);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, finalDotPos, sprite.playerMarker,
                    glowColor, 0, Helpers.SpriteCenter(sprite.playerMarker), 1, SpriteEffects.None, 0);
            }
        }

        private void DrawOrbitalRings(Location planet, Camera lockCam)
        {
            if (planet.hoverTransition <= 0)
                return;

            if (planet.moons == null)
                return;

            if (planet.moons.Length <= 0)
                return;
            
            Matrix planetRot = planet.rotation;
            Color ringColor = Color.Lerp(Color.White,
                new Color(255,255,255,32),
                planet.hoverTransition);

            for (int i = 0; i < planet.moons.Length; i++)
            {
                float dist = planet.moons[i].moonDistance;
                dist = Helpers.PopLerp(planet.hoverTransition, dist * 0.5f, dist * 1.5f, dist);
                FrameworkCore.discRenderer.Draw(dist, planet.position, ringColor, planetRot);
            }
        }

        private void DrawPlanetRing(GameTime gameTime, Camera lockCam)
        {
            if (hoverPlanet == null)
                return;
            
            float planetSize = Helpers.SizeInPixels(lockCam, hoverPlanet.position, 0.5f);
            float crosshairSize = Math.Max(planetSize / 26.0f, 0.4f);

            float ringAngle = (float)gameTime.TotalGameTime.TotalSeconds;
            crosshairSize = Helpers.PopLerp(hoverPlanet.hoverTransition, crosshairSize * 2, crosshairSize * 0.5f, crosshairSize);
            Vector2 planetScreenPos = Helpers.GetScreenPos(lockCam, hoverPlanet.position);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, planetScreenPos, sprite.bigCrosshair,
                Color.White, ringAngle, Helpers.SpriteCenter(sprite.bigCrosshair),
                crosshairSize, SpriteEffects.None, 0);            
        }

        private void DrawPlanetLabel(GameTime gameTime, Location planet, Camera lockCamera)
        {
            /*
            if (worldState == WorldState.Event && currentLocation != null && currentLocation == planet
                ||
                worldState != WorldState.Event)
            { }
            else
                return;*/

            if (worldState != WorldState.Orders)
                return;

            if (menuManager.menus.Count > 0)
                return;

            if (menuManager.menus.Count > 0)
            {
                if (menuManager.menus[0].GetType() == typeof(FleetMenu))
                {
                    return;
                }
            }


            Vector2 shipPos = Helpers.GetScreenPos(lockCamera, planet.position);
            shipPos.X += Helpers.SizeInPixels(lockCamera, planet.position, 0.5f);
            shipPos.X += 8;


            Color labelColor = Color.White;

            if (planet.worldType == WorldType.Dangerous)
            {
                labelColor = new Color(230, 70, 70);
            }

            if (planet.isExplored)
                labelColor = Color.Lerp(new Color(96, 96, 96,128), Color.Black, planet.hoverTransition);
            else
                labelColor = Color.Lerp(labelColor, Color.Black, planet.hoverTransition);
                
                

            Color labelBG = new Color(0,0,0,64);

            String planetName = planet.name;

            if (!planet.isVisible)
                return;

            Vector2 nameVec = FrameworkCore.Serif.MeasureString(planetName);

            float size = Helpers.PopLerp(planet.hoverTransition, 1.0f, 1.5f, 1.2f);
            Vector2 nameOrigin = new Vector2(0, nameVec.Y/2);

            if (planet.hoverTransition > 0)
            {
                Color finalBoxColor = new Color(128, 128, 128);

                if (!planet.isExplored)
                {
                    //Unexplored box color.
                    finalBoxColor = Color.Lerp(new Color(255, 120, 0), new Color(255, 160, 0),
                        0.5f + Helpers.Pulse(gameTime, 0.49f, 5));
                }

                Color boxColor = Color.Lerp(
                    Helpers.transColor(new Color(255, 255, 255)),
                    finalBoxColor,
                    planet.hoverTransition);

                Rectangle rect = new Rectangle(
                    (int)shipPos.X - 3,
                    (int)(shipPos.Y - nameVec.Y / 2),
                    (int)(nameVec.X * size) + 9,
                    (int)(nameVec.Y * size));

                rect.Width = (int)MathHelper.Lerp(1, rect.Width, planet.hoverTransition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rect, sprite.tabRectangle,
                    boxColor);
            }

            if (planet.hoverTransition <= 0 && !planet.isExplored)
            {
                Helpers.DrawOutline(FrameworkCore.Serif, planetName, shipPos, labelColor, Color.Black,
                    0, nameOrigin, size);
            }
            else
            {
                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, planetName, shipPos, labelColor,
                    0, nameOrigin, size, SpriteEffects.None, 0);
            }
        }

        private void DrawCrosshair(GameTime gameTime, Camera lockCam)
        {
            Vector2 centerScreen = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2f,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2f);

#if WINDOWS
            centerScreen = FrameworkCore.players[0].inputmanager.mousePos;
#endif

            float crosshairSize = MathHelper.Lerp(1.8f, 1.2f, 0.5f + Helpers.Pulse(gameTime, 0.49f, 12));

            Rectangle crosshairRect = sprite.dot;

            Vector2 origin = Helpers.SpriteCenter(sprite.dot);
#if WINDOWS
            crosshairRect = sprite.mouseCursor;
            crosshairSize = 1;
            origin = new Vector2(10, 5);

#endif

            

            if (hoverPlanet != null)
            {
                crosshairRect = sprite.fingerCursor;
                origin = new Vector2(13, 6);
                crosshairSize = MathHelper.Lerp(2,1,crosshairTransition);
            }

#if WINDOWS
            if (FrameworkCore.options.hardwaremouse)
                return;
#endif

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, centerScreen, crosshairRect, Color.White, 0,
                origin, crosshairSize, SpriteEffects.None, 0);
        }

        private void DrawLegend()
        {
            if (worldState != WorldState.Orders)
                return;

            if (menuManager.menus.Count > 0)
                return;


            Helpers.DrawLegendRow2(Resource.MenuWorldMoveCursor, sprite.buttons.leftstickTiny, 1);


            if (hoverPlanet == null)
                return;

            if (hoverPlanet.name == null)
                return;

            if (currentLocation == null)
                return;

            if (hoverPlanet.worldType == WorldType.Tutorial && !hoverPlanet.isExplored)
            {
                Helpers.DrawLegend(Resource.TutorialTutorial, sprite.buttons.a, hoverPlanet.hoverTransition);
                return;
            }

            if (currentLocation != null && currentLocation == hoverPlanet)
                return;

            string legendText = string.Format(Resource.MenuCampaignTravelTo, hoverPlanet.name);
            Helpers.DrawLegend(legendText, sprite.buttons.a, hoverPlanet.hoverTransition);
        }

        private void DrawBubbles(Camera lockCam)
        {
            foreach (Location planet in locations)
            {
                if (planet.hoverTransition <= 0)
                    continue;

                if (currentLocation != null && planet == currentLocation)
                    continue;

                Vector2 mousePos = 
#if WINDOWS
                    FrameworkCore.players[0].inputmanager.mousePos;
#else
                    Helpers.GetScreenCenter();
#endif

                DrawBubbleButton(Helpers.GetScreenPos(lockCam, planet.position),
                    mousePos,
                    sprite.buttons.a,
                    planet, false);
            }
        }

        
        private void DrawBubbleButton(Vector2 targetPos, Vector2 originPos, Rectangle buttonRect, Location planet, bool isReversed)
        {
            Color bubbleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, planet.hoverTransition);
            float buttonSize = 1;
            float bubbleSize = 1;

            //deselecting.
            buttonSize = Helpers.PopLerp(planet.hoverTransition, 0, 2f, 1);
            bubbleSize = MathHelper.Lerp(0, 1, planet.hoverTransition);

            Vector2 direction = targetPos - originPos;

            if (isReversed)
                direction = originPos - targetPos;

            direction.Normalize();
            float adjacent = direction.X;
            float opposite = direction.Y;
            float TargetAngle = (float)System.Math.Atan2(opposite, adjacent);
            planet.bubbleDisplayAngle = Helpers.TurnToFace(planet.bubbleDisplayAngle, TargetAngle, 0.08f);

            if (isReversed)
                targetPos = originPos;

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, targetPos, sprite.bubble, bubbleColor,
                planet.bubbleDisplayAngle, new Vector2(0, 16), bubbleSize, SpriteEffects.None, 0);

            float TagAngle = planet.bubbleDisplayAngle;
            float x = (float)(Math.Cos(TagAngle) * (48 * bubbleSize));
            float y = (float)(Math.Sin(TagAngle) * (48 * bubbleSize));
            Vector2 TagPos = new Vector2(targetPos.X + x, targetPos.Y + y);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, TagPos, buttonRect, bubbleColor,
                0, Helpers.SpriteCenter(buttonRect), buttonSize, SpriteEffects.None, 0);

            if (planet.hoverTransition > 0)
            {
                Color flashColor = Color.Lerp(Color.White, OldXNAColor.TransparentWhite, planet.hoverTransition);
                
                flashColor = OldXNAColor.TransparentWhite;
                
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, TagPos, sprite.buttons.blank, flashColor,
                    0, Helpers.SpriteCenter(buttonRect), buttonSize, SpriteEffects.None, 0);
            }
        }


#if DEBUG
        public void DebugGotoPlanet()
        {
            //debug. pick a random planet and go to it.
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].worldType == WorldType.Trader)
                    continue;

                if (locations[i].worldType == WorldType.Tutorial)
                    continue;

                if (locations[i].worldType == WorldType.wormhole)
                    continue;

                if (!locations[i].isVisible)
                    continue;

                if (locations[i].isExplored)
                    continue;

                GoToPlanet(locations[i]);
                return;
            }
        }
#endif

    }
}