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
    public class FleetShip
    {
        public string captainName;
        public ShipData shipData;

        public InventoryItem[] upgradeArray;

        public Vector3 menuPos;
        public Vector3 targetMenuPos;
        public float menuHoverTransition;

        public int veterancy = 0;

        /// <summary>
        /// for Campaign game, this flag determines whether this ship is "childed" to player 2.
        /// </summary>
        public bool childShip;

        public SpaceShipStats stats;

        public FleetShip()
        {
            upgradeArray = new InventoryItem[2] { null, null };
            stats = new SpaceShipStats();
        }
    }
    
    public enum ModeName
    {
        Default,
        MoveMode,
        HeightMode,
        YawMode,
        PitchMode,
        RollMode,
        Playback,
        AimMode,
        ModifyHeightMode,
        ModifyPriorityTarget,
        FacingMode,
    }    

    public class PlayerMode
    {
        public ModeName modeName = ModeName.Default;
        public float transition = 0;
        public string textLabel = "";
        public ModeCategory menuCategory;

        public PlayerMode(ModeName name, string text)
        {
            modeName = name;
            textLabel = text;
        }
    }

    public enum ModeCategory
    {
        Move,
        Aim,
    }



    public enum worldMenuTypes
    {
        WatchReplay,
        EndTurn,
    }

    public class PlayerCommander : Commander
    {
        float desiredGridAltitude = 0;

        float gridAltitude = 0;
        public float GridAltitude
        {
            get { return gridAltitude; }
        }


        bool advancedMode = false;

        bool tutHasShownCameraControls = false;
        bool tutHasShownArmorTips = false;


        AudioListener listener;
        public AudioListener Listener
        {
            get { return listener; }
        }


        public bool mouseEnabled = false;


        SysMenuManager playerMenus;


        public List<InventoryItem> inventoryItems;

        PlayerIndex playerIndex;
        public PlayerIndex playerindex
        {
            get { return playerIndex; }
            set { playerIndex = value; }
        }

        public LockCamera lockCamera;        

        PlayerMode currentMode;

        public PlayerMode CurrentMode
        {
            get { return currentMode; }
        }

        List<PlayerMode> playerModes = new List<PlayerMode>();

        PlayerMode modifyVerticalMode;
        PlayerMode modifyPriorityTarget;

        int rumbleTimer;
        public int RumbleTimer
        {
            get { return rumbleTimer; }
        }

        

        CommandMenu commandMenu;
        PlaybackMenu playbackMenu;
        ShipMenu shipMenu;

        public float playbackMenuTransition
        {
            get { return playbackMenu.Transition; }
        }

        

        Menu activeMenu;

        IngameScoreboard ingameScoreboard;

        /// <summary>
        /// Campaign use only! List of ships in the player's fleet.
        /// </summary>
        public List<FleetShip> campaignShips;

        public int planetsVisited=0;
        public int dangerousPlanetsVisited = 0;

        public int extraPoints = 0;

        public Menu ActiveMenu
        {
            get { return activeMenu; }
            set { activeMenu = value; }
        }



        public Vector2 viewportSize;



        Vector3 currentMovePos = Vector3.Zero;    //current position of the OrientationBall
        Matrix orientationMatrix = Matrix.Identity;  //rotation of the OrientationBall
        float orientationTransition = 0;   //controls acceleration curve when player adjusts yaw/roll/pitch

        const int orientationResetTime = 250; //how long player holds LB+RB to reset the matrix.
        float curOrientationTransition = 0;  //transition time for resetting the matrix when player presses RB+LB


        public Vector3 CurrentMovePos
        {
            get { return currentMovePos; }
        }


        private float crosshairTransition = 0;

        public bool isReady = false;


        InputManager inputManager;

        public InputManager inputmanager
        {
            get { return inputManager; }
        }


        
        public PlayerCommander(PlayerIndex playerIndex, Color teamColor, Color shipColor) : base(teamColor, shipColor)
        {
            ingameScoreboard = new IngameScoreboard();

            playerMenus = new SysMenuManager();

            this.playerIndex = playerIndex;
            

            lockCamera = new LockCamera(FrameworkCore.Game);


            modifyVerticalMode = new PlayerMode(ModeName.ModifyHeightMode, Resource.ModeModifyVertical);
            modifyVerticalMode.transition = 1;

            modifyPriorityTarget = new PlayerMode(ModeName.ModifyPriorityTarget, Resource.ModeModifyPriorityTarget);
            modifyPriorityTarget.transition = 1;


            //modifyVerticalMode = new PlayerMode(ModeName.FacingMode, Resource.ModeFacingMode);
            //modifyVerticalMode.transition = 1;


            inventoryItems = new List<InventoryItem>();



            

            


            PlayerMode p = new PlayerMode(ModeName.Default, "");
            playerModes.Add(p);
            p = new PlayerMode(ModeName.MoveMode, Resource.ModeMovement);
            p.menuCategory = ModeCategory.Move;
            playerModes.Add(p);
            p = new PlayerMode(ModeName.HeightMode, Resource.ModeVerticalMovement);
            p.menuCategory = ModeCategory.Move;
            playerModes.Add(p);



            p = new PlayerMode(ModeName.FacingMode, Resource.ModeFacingMode);
            p.menuCategory = ModeCategory.Move;
            playerModes.Add(p);


            /*p = new PlayerMode(ModeName.YawMode, Resource.ModeYawOrientation);
            p.menuCategory = ModeCategory.Move;
            playerModes.Add(p);
            p = new PlayerMode(ModeName.PitchMode, Resource.ModePitchOrientation);
            p.menuCategory = ModeCategory.Move;
            playerModes.Add(p);
            p = new PlayerMode(ModeName.RollMode, Resource.ModeRollOrientation);
            p.menuCategory = ModeCategory.Move;
            playerModes.Add(p);
            */

            p = new PlayerMode(ModeName.AimMode, Resource.ModeAimMode);
            p.menuCategory = ModeCategory.Aim;
            playerModes.Add(p);




            commandMenu = new CommandMenu(FrameworkCore.Game, this);
            playbackMenu = new PlaybackMenu(FrameworkCore.Game, this);
            shipMenu = new ShipMenu(FrameworkCore.Game, this);

            ChangePlayerMode(ModeName.Default);

            MenuItem k = new MenuItem(Resource.MenuEndTurn);
            k.Selected += SetReady;
            k.iconRect = sprite.clock;
            commandMenu.AddItem(k);

            k = new MenuItem(Resource.MenuPlayback);
            k.Selected += ActivatePlaybackMode;
            k.iconRect = sprite.camera;
            commandMenu.AddItem(k);

            k = new MenuItem(Resource.MenuCancel);
            k.Selected += CloseMenu;
            k.iconRect = sprite.cancel;
            commandMenu.AddItem(k);



            k = new MenuItem(Resource.OrderMove);
            k.Selected += ApplyDefaultMove;
            k.iconRect = sprite.icons.move;
            k.gameEffect = new GE.DefaultMove();
            shipMenu.AddItem(k);

            k = new MenuItem(Resource.OrderFlank);
            k.iconRect = sprite.icons.flankSpeed;
            k.Selected += ApplyFlankSpeed;
            k.gameEffect = new GE.FlankSpeed();
            shipMenu.AddItem(k);

            k = new MenuItem(Resource.OrderFocusFire);
            k.iconRect = sprite.icons.focusFire;
            k.Selected += ApplyFocusFire;
            k.gameEffect = new GE.FocusFire();
            shipMenu.AddItem(k);

            /*
            k = new MenuItem(Resource.OrderRepair);
            k.iconRect = sprite.icons.wrench;
            k.Selected += ApplyRepairs;
            k.gameEffect = new GE.FieldRepairs();
            shipMenu.AddItem(k);
            */

            k = new MenuItem(Resource.MenuDone);
            k.Selected += CloseShipMenu;
            k.iconRect = sprite.icons.done;
            shipMenu.AddItem(k);

            inputManager = new InputManager(playerIndex);

            listener = new AudioListener();


            //for single-player.
            campaignShips = new List<FleetShip>();

            //give the controllingPlayer some default fleet ships.
            if (playerIndex == FrameworkCore.ControllingPlayer)
                GiveDefaultFleet();            

            this.factionName = Faction.Blue;

            
        }

        public void ClearAll()
        {
            commandPoints = 0;            
            expBarHover = false;
            smitePowerActive = false;



            desiredGridAltitude = 0;
            gridAltitude = 0;

            tutHasShownCameraControls = false;
            tutShownNextShipBox = false;

            this.inventoryItems.Clear();
            this.campaignShips.Clear();
            this.planetsVisited = 0;
            this.dangerousPlanetsVisited = 0;
            this.extraPoints = 0;

            this.activeMenu = null;

            this.commandMenu.ForceOff();
            this.shipMenu.ForceOff();
            this.playbackMenu.ForceOff();

            GiveDefaultFleet();

            tutState = TutState.initialize;
            warpCameraTransition = 1;
        }

        private void GiveDefaultFleet()
        {
            Helpers.AddFleetShip(campaignShips, shipTypes.Destroyer);
            Helpers.AddFleetShip(campaignShips, shipTypes.Destroyer);            
        }

        public void AddCargo(InventoryItem item)
        {
            inventoryItems.Add(item);
        }

        private void ApplyDefaultMove(object sender, EventArgs e)
        {
            if (ApplyGameEffect(new GE.DefaultMove()))
            {
                FrameworkCore.PlayCue(sounds.Fanfare.radio);
                ActivateMoveMode(sender, e);
            }
        }

        private void ApplyFlankSpeed(object sender, EventArgs e)
        {
            if (ApplyGameEffect(new GE.FlankSpeed()))
            {
                FrameworkCore.PlayCue(sounds.Fanfare.rev);
                ActivateMoveMode(sender, e);
            }
        }

        private bool ApplyGameEffect(GameEffect effect)
        {
            if (selectedShip == null)
                return false;

            //toggle effect off.
            if (selectedShip.OrderEffect != null && selectedShip.OrderEffect.ToString() == effect.ToString())
            {
                selectedShip.ClearMoveOrder();
                selectedShip.ClearOrderEffect();

                /*
                GamePopup popup = new GamePopup(playerMenus);
                popup.darkenScreen = true;
                popup.hideChildren = false;
                popup.width = 400;

                MenuItem item = new MenuItem(Resource.ModeModifyClear);
                item.Selected += ModifyClear;
                popup.AddItem(item);

                item = new MenuItem(Resource.ModeModifyVertical);
                item.Selected += ModifyVertical;
                popup.AddItem(item);

                if (activeMenu == shipMenu && activeMenu.selectedItem != null &&
                    activeMenu.selectedItem.gameEffect != null &&
                    activeMenu.selectedItem.gameEffect.canFire)
                {
                    item = new MenuItem(Resource.ModeModifyPriorityTarget);
                    item.Selected += ModifyPriorityTarget;
                    popup.AddItem(item);
                }

                playerMenus.AddMenu(popup);
                 */

                return false;
            }

            //turn on effect.
            selectedShip.ApplyEffect(effect);
            selectedShip.orderTransition = 0;

            return true; //apply is successful.
        }

        private void ModifyVertical(object sender, EventArgs e)
        {
            if (selectedShip == null)
                return;

            currentMode = modifyVerticalMode;

            currentMovePos = selectedShip.targetPos;
            orientationMatrix = Matrix.CreateFromQuaternion(selectedShip.targetRotation);

            Helpers.CloseThisMenu(sender); //close the popup.
            CloseMenu(sender, e); //close the shipMenu.
        }

        private void ModifyPriorityTarget(object sender, EventArgs e)
        {
            if (selectedShip == null)
                return;

            currentMode = modifyPriorityTarget;

            currentMovePos = selectedShip.targetPos;
            orientationMatrix = Matrix.CreateFromQuaternion(selectedShip.targetRotation);

            Helpers.CloseThisMenu(sender); //close the popup.
            CloseMenu(sender, e); //close the shipMenu.
        }

        private void ModifyClear(object sender, EventArgs e)
        {
            currentMovePos = selectedShip.Position;

            selectedShip.ClearMoveOrder();
            selectedShip.ClearOrderEffect();

            Helpers.CloseThisMenu(sender);
        }

        private void ApplyFocusFire(object sender, EventArgs e)
        {
            if (ApplyGameEffect(new GE.FocusFire()))
            {
                FrameworkCore.PlayCue(sounds.Fanfare.gunload);
                ActivateMoveMode(sender, e);
            }
        }

        private void ApplyRepairs(object sender, EventArgs e)
        {
            ApplyGameEffect(new GE.FieldRepairs());

            shipMenu.Deactivate();
            ClearSelectedShip();
        }

        public bool IsPlaybackMode()
        {
            if (activeMenu == playbackMenu)
                return true;

            return false;
        }

        public void Initialize()
        {
            // Set the camera

            lockCamera.SetProjectionParams(MathHelper.ToRadians(45.0f),
                (float)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / (float)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height,
                1.0f, 3000000.0f);


            if (mouseEnabled)
                cameraSpeedMultiplier = 4;

            confirmButtonPositions = new Vector2[3];
            UpdateConfirmButtonPositions();
        }

        private void UpdateConfirmButtonPositions()
        {
            confirmButtonPositions[0] = new Vector2(50 + 128,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 90);
            confirmButtonPositions[1] = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 50 - 128,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 90);

            //the advanced/basic orientation controls.
            confirmButtonPositions[2] = new Vector2(50 + 128,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 150);
        }

        public void InitializeScoreboard()
        {
            ingameScoreboard.Initialize();
        }




        public void UpdateCameraFOV(float fov)
        {
            lockCamera.SetProjectionParams(MathHelper.ToRadians(fov),
                //(float)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width /
                //(float)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height,
                viewportSize.X / 
                viewportSize.Y,
                1.0f, 3000000.0f);
        }

        public override void LoadContent()
        {
            playbackMenu.Initialize();            
        }

        public Matrix rotationMatrix = Matrix.Identity;
        float engineSpeed = 0.0f;
        float strafeSpeed = 0.0f;

        
        

        int blinkTime = 0;
        bool blinkOn = false;


        public void ActionUpdate(GameTime gameTime)
        {
            listener.Position = this.position;
            listener.Forward = rotationMatrix.Forward;
            listener.Up = rotationMatrix.Up;

            /*FrameworkCore.audiomanager.ApplyEmitter(this.position, rotationMatrix.Forward, rotationMatrix.Up,
                Vector3.Zero);*/

            
            if (smitePowerActive && ShouldHandleExpBar())
            {
                if (smiteTimer > 0)
                {
                    smiteTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                    return;
                }

                smitePowerActive = false;

                commandPoints = (int)Math.Max(0, commandPoints - 1000);


                FindSmiteShip();

                //some chance that you smite multiple ships.
                if (FrameworkCore.r.Next(10) == 0)
                    FindSmiteShip();
            }
        }

        private void FindSmiteShip()
        {
            if (FrameworkCore.r.Next(2) == 0)
            {
                for (int x = FrameworkCore.level.Ships.Count - 1; x >= 0; x--)
                {
                    if (SmiteCheck(x))
                        return;
                }
            }
            else
            {
                for (int x = 0; x < FrameworkCore.level.Ships.Count; x++)
                {
                    if (SmiteCheck(x))
                        return;
                }
            }
        }

        int smiteTimer = 1500;

        private void ResetSmiteTimer()
        {
            smiteHoverTimer = 0;

            smiteAnimationTimer = 1;
            expBarTransition = 0;

            smiteTimer = 1500;

            expBarHover = false;
        }

        private bool SmiteCheck(int x)
        {
            if (FrameworkCore.level.Ships[x].IsDestroyed)
                return false;

            if (FrameworkCore.level.Ships[x].Health <= 0)
                return false;

            if (FrameworkCore.level.Ships[x].owner != null &&
                FrameworkCore.level.Ships[x].owner.GetType() == typeof(PlayerCommander))
                return false;

            if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[x]))
                return false;

            ((SpaceShip)FrameworkCore.level.Ships[x]).ForceKill();


            FrameworkCore.Particles.CreateSmite(FrameworkCore.level.Ships[x].Position,
                new Vector3(
                    Helpers.randFloat(-1,1),
                    Helpers.randFloat(-1,1),
                    Helpers.randFloat(-1,1))
                    );

            FrameworkCore.WorldtextManager.SmiteMessage(FrameworkCore.level.Ships[x].Position + new Vector3(0,16,0),
                Vector3.Up, Resource.SmiteMessage);

            return true;
        }

        public void UpdateControls(GameTime gameTime, bool updateMouseCam)
        {
            //handle rumble here.
            if (rumbleTimer > 0)
            {
                rumbleTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (rumbleTimer <= 0)
                {
                    try
                    {
                        GamePad.SetVibration(playerIndex, 0, 0);
                    }
                    catch
                    {
                    }
                }
            }

#if DEBUG
            if (FrameworkCore.MainMenuManager.menus.Count > 1)
            {
                if (FrameworkCore.MainMenuManager.menus[1].GetType() == typeof(DebugEventsMenu))
                {
                    updateMouseCam = false;
                }
            }
#endif

            inputManager.Update(gameTime, playerIndex, this, updateMouseCam);
        }

        public void setRumble(int value)
        {
            if (value <= rumbleTimer)
                return;

            rumbleTimer = value;
            try
            {
                GamePad.SetVibration(playerIndex, 0.7f, 0.7f);
            }
            catch
            {
            }
        }

        public void UpdateCamera(GameTime gameTime, Vector3 pos, Quaternion rot)
        {
            if (warpCameraTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(400).TotalMilliseconds);
                warpCameraTransition = MathHelper.Clamp(warpCameraTransition + delta, 0, 1);
                float smoothStep = MathHelper.SmoothStep(0, 1, warpCameraTransition);
                position = Vector3.Lerp(warpCameraOrigin, desiredWarpCameraPosition, smoothStep);

                Rotation = Quaternion.Lerp(warpCameraRotationOrigin, desiredWarpCameraRotation, smoothStep);
                rotationMatrix = Matrix.CreateFromQuaternion(Rotation);

                pos = position;
                rot = Rotation;
            }

            lockCamera.TargetPosition = pos;
            lockCamera.TargetRotation = rot;
            lockCamera.Update(gameTime);
        }

        Vector2 cursorPos = Vector2.Zero;

        public Vector2 CursorPos
        {
            get { return cursorPos; }
        }

        private int cameraSpeedMultiplier = 1;
        float cameraDisplayTimer = 1;





        enum TutState
        {
            initialize,
            showPlayerFleet,
            showEnemyFleet,
            showOrdersIntro,
            doOrdersTransition,
            doOrders,
            done
        }

        Vector3 tutLookAt;
        TutState tutState;
        bool tutShownNextShipBox = false;

        private void UpdateTutRotateCam(GameTime gameTime, Vector3 lookatPos, bool snapTo)
        {
            float dx = (float)-Math.Cos(gameTime.TotalGameTime.TotalSeconds * 0.1f);
            float dz = (float)-Math.Sin(gameTime.TotalGameTime.TotalSeconds * 0.1f);
            
            Vector3 desiredPos = lookatPos + new Vector3(dx, 0, dz) * 64;
            desiredPos.Y = lookatPos.Y + 32;

            if (!snapTo)
                position = Vector3.Lerp(position, desiredPos, 0.02f);
            else
                position = desiredPos;


            Matrix lookAt = Matrix.CreateLookAt(this.position, lookatPos, Vector3.Up);

            if (!snapTo)
            {
                Rotation = Quaternion.Lerp(Rotation,
                    Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt)),
                    0.01f);
                rotationMatrix = Matrix.CreateFromQuaternion(Rotation);
            }
            else
            {
                Rotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));
                rotationMatrix = Matrix.CreateFromQuaternion(Rotation);
            }
        }

        private Vector3 GetEnemyFleetPosition()
        {
            foreach (Collideable ship in FrameworkCore.level.Ships)
            {
                if (ship.owner == null)
                    continue;

                if (ship.owner.GetType() != typeof(PlayerCommander))
                {
                    return ship.Position;
                }
            }

            return Vector3.Zero;
        }

        private Vector3 GetPlayerFleetPosition()
        {
            Vector3 lookat = Vector3.Zero;
            if (FrameworkCore.players.Count > 1)
            {
                //split screen.
                foreach (Collideable ship in FrameworkCore.level.Ships)
                {
                    if (ship.owner == null)
                        continue;

                    if (ship.owner == this)
                    {
                        lookat = ship.Position;
                        break;
                    }
                }
            }
            else
            {
                Vector3[] shipPositions = new Vector3[2] { Vector3.Zero, Vector3.Zero };
                int i = 0;

                //look at a point between the ships.
                foreach (Collideable ship in FrameworkCore.level.Ships)
                {
                    if (ship.owner == null)
                        continue;

                    if (ship.owner == this)
                    {
                        shipPositions[i] = ship.Position;
                        i++;

                        if (i >= 2)
                            break;
                    }
                }

                lookat = Vector3.Lerp(shipPositions[0], shipPositions[1], 0.5f);
            }

            return lookat;
        }

        private bool isTutorialLevel
        {
            get
            {
                if (FrameworkCore.worldMap == null)
                    return false;

                if (!FrameworkCore.worldMap.isTutorial)
                    return false;

                if (!FrameworkCore.isCampaign)
                    return false;

                return true;
            }
        }

        private void ShowNextShipTutorial()
        {
            if (FrameworkCore.players.Count > 1)
                return;

            if (tutShownNextShipBox)
                return;

            tutShownNextShipBox = true;

            FrameworkCore.PlayCue(sounds.Fanfare.cheering);

            SysPopup signPrompt = new SysPopup(playerMenus, Resource.TutorialNextShip);
            signPrompt.transitionOnTime = 200;
            signPrompt.transitionOffTime = 200;
            signPrompt.darkenScreen = false;
            signPrompt.hideChildren = false;
            signPrompt.canBeExited = true;
            signPrompt.sideIconRect = sprite.windowIcon.info;

            MenuItem item = new MenuItem(Resource.MenuOK);
            item.Selected += ClosePopup;
            signPrompt.AddItem(item);

            playerMenus.AddMenu(signPrompt);
        }

        private void ClosePopup(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }

        /// <summary>
        /// Return TRUE if we want to block player input.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private bool UpdateTutorial(GameTime gameTime)
        {
            if (!isTutorialLevel)
                return false;
            

            if (tutState == TutState.initialize)
            {
                FrameworkCore.PlayCue(sounds.Music.raindrops01);

                warpCameraTransition = 1;
                tutLookAt = GetPlayerFleetPosition();

                UpdateTutRotateCam(gameTime, tutLookAt, true);                
                tutState = TutState.showPlayerFleet;
            }
            else if (tutState == TutState.showPlayerFleet)
            {
                UpdateTutRotateCam(gameTime, tutLookAt, false);
                UpdateTutCamera(gameTime, position, Rotation);

                if (tutPlayerProceed())
                {
                    TutGotoEnemyFleet();
                    tutState = TutState.showEnemyFleet;
                }
            }
            else if (tutState == TutState.showEnemyFleet)
            {
                UpdateTutRotateCam(gameTime, tutLookAt, false);
                UpdateTutCamera(gameTime, position, Rotation);

                if (tutPlayerProceed())
                {
                    TutGotoPlayerFleet();
                    tutState = TutState.showOrdersIntro;
                }
            }
            else if (tutState == TutState.showOrdersIntro)
            {
                UpdateTutRotateCam(gameTime, tutLookAt, false);
                UpdateTutCamera(gameTime, position, Rotation);

                if (tutPlayerProceed())
                {
                    

                    tutRotationOrigin = Rotation;
                    tutWarpCamToGoodAngle(gameTime);
                    tutState = TutState.doOrdersTransition;
                }
            }
            else if (tutState == TutState.doOrdersTransition)
            {
                Matrix lookAt = Matrix.CreateLookAt(this.position, tutLookAt, Vector3.Up);

                float smoothTransition = MathHelper.SmoothStep(0, 1, warpCameraTransition);

                

                UpdateTutCamera(gameTime, position, Rotation);

                if (warpCameraTransition >= 1)
                {
                    tutState = TutState.doOrders;
                    
                }
            }
            else if (tutState == TutState.doOrders)
            {
                //player is back in control.
                return false;
            }

            return true;
        }

        Quaternion tutRotationOrigin;

        //get a good camera angle on the player ships and the enemy ship.
        private void tutWarpCamToGoodAngle(GameTime gameTime)
        {
            Vector3 playerPosition = GetCenterFleet(false);
            Vector3 enemyPosition = GetCenterFleet(true);

            //pick whoever is higher.
            Vector3 cameraPosition = Vector3.Zero;
            cameraPosition.Y = Math.Max(playerPosition.Y, enemyPosition.Y);
            cameraPosition.Y += 32;

            //clamp it to above the zero plane.
            cameraPosition.Y = Math.Max(cameraPosition.Y, 32);

            // snap camera X-Z to player fleet.
            cameraPosition.X = playerPosition.X;
            cameraPosition.Z = playerPosition.Z;

            //move camera back. first, find the direction vector.
            Vector3 dirToEnemy = new Vector3(playerPosition.X, 0, playerPosition.Z) -
                new Vector3(enemyPosition.X, 0, enemyPosition.Z);
            dirToEnemy.Normalize();
            cameraPosition = cameraPosition + dirToEnemy * 64;

            //move camera perpendicular.
            dirToEnemy = Vector3.Cross(Vector3.Up, dirToEnemy);
            cameraPosition = cameraPosition + dirToEnemy * 48;

            //tell camera where to look. bias it toward the player.
            tutLookAt = Vector3.Lerp(playerPosition, enemyPosition, 0.4f);

            //push camera back until it can frame every ship.
            cameraPosition = tutMoveCamToSeeAllShips(gameTime, cameraPosition, tutLookAt);


            //give the camera some warp info.
            warpCameraOrigin = this.position;
            desiredWarpCameraPosition = cameraPosition;

            warpCameraRotationOrigin = this.Rotation;
            Matrix lookAt = Matrix.CreateLookAt(desiredWarpCameraPosition, tutLookAt, Vector3.Up);
            desiredWarpCameraRotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));

            warpCameraTransition = 0;
        }

        /// <summary>
        /// move the camera backwards until it can see every ship.
        /// </summary>
        /// <param name="basePosition">Where the camera will be.</param>
        /// <param name="baseLookat">Where the camera will look at.</param>
        /// <returns>position of where the camera needs to be.</returns>
        private Vector3 tutMoveCamToSeeAllShips(GameTime gameTime, Vector3 basePosition, Vector3 baseLookat)
        {
            LockCamera testCamera = new LockCamera(FrameworkCore.Game);
            testCamera.TargetPosition = basePosition;
            testCamera.TargetRotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(Matrix.CreateLookAt(basePosition, baseLookat, Vector3.Up)) );
            testCamera.Update(gameTime);

            foreach (Collideable ship in FrameworkCore.level.Ships)
            {
                if (!Helpers.IsSpaceship(ship))
                    continue;

                if (ship.owner == null)
                    continue;

                testCamera.Update(gameTime);

                if (CanSeeMoveDisc(testCamera, (SpaceShip)ship) && CanSeeShip(testCamera, (SpaceShip)ship))
                    continue;
                
                for (int x = 8; x < 1024; x += 8)
                {
                    testCamera.TargetPosition += Matrix.CreateFromQuaternion(testCamera.TargetRotation).Backward * x;
                    testCamera.Update(gameTime);

                    if (CanSeeMoveDisc(testCamera, (SpaceShip)ship) && CanSeeShip(testCamera, (SpaceShip)ship))
                    {
                        break;
                    }
                }
            }

            //extra nudge.
            testCamera.TargetPosition += Matrix.CreateFromQuaternion(testCamera.TargetRotation).Backward * 16;
            testCamera.Update(gameTime);

            return testCamera.CameraPosition;
        }

        private bool tutPlayerProceed()
        {
            if (warpCameraTransition < 1)
                return false;

            bool proceed = false;
            if (mouseEnabled)
            {
                if (inputManager.mouseLeftClick || inputManager.kbEnter)
                {
                    proceed = true;
                }
            }
            else
            {
                if (inputManager.buttonAPressed)
                {
                    proceed = true;
                }
            }

            return proceed;
        }

        

        private void TutGotoPlayerFleet()
        {
            tutLookAt = GetPlayerFleetPosition();
            WarpCameraToPosition(tutLookAt);            
        }

        private void TutGotoEnemyFleet()
        {
            //go to the next state.
            foreach (Collideable ship in FrameworkCore.level.Ships)
            {
                if (ship.owner == null)
                    continue;

                if (ship.owner.GetType() != typeof(PlayerCommander))
                {
                    tutLookAt = ship.Position;
                    break;
                }
            }

            WarpCameraToPosition(tutLookAt);            
        }

        public void UpdateTutCamera(GameTime gameTime, Vector3 pos, Quaternion rot)
        {
            if (warpCameraTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(1500).TotalMilliseconds);
                warpCameraTransition = MathHelper.Clamp(warpCameraTransition + delta, 0, 1);
                float smoothStep = MathHelper.SmoothStep(0, 1, warpCameraTransition);


                position = Vector3.Lerp(warpCameraOrigin, desiredWarpCameraPosition, smoothStep);


                Rotation = Quaternion.Lerp(warpCameraRotationOrigin,
                                    desiredWarpCameraRotation,
                                    smoothStep);
                rotationMatrix = Matrix.CreateFromQuaternion(Rotation);


                pos = position;
                rot = Rotation;
            }

            lockCamera.TargetPosition = pos;
            lockCamera.TargetRotation = rot;
            lockCamera.Update(gameTime);
        }
















        public override void Update(GameTime gameTime)
        {
            if (desiredGridAltitude != gridAltitude)
            {
                gridAltitude = MathHelper.Lerp(gridAltitude, desiredGridAltitude, 
                    (float)(3 * gameTime.ElapsedGameTime.TotalSeconds));
            }

            
            if (UpdateTutorial(gameTime))
            {
                base.Update(gameTime);
                return;
            }
            

            if (mouseEnabled)
            {
                //PC camera speed controls.
                /*
                if (inputManager.mouseCameraMode)
                {
                    if (inputManager.mouseWheelUp)
                    {
                        if (cameraSpeedMultiplier <= 1)
                            cameraSpeedMultiplier = 8;
                        else if (cameraSpeedMultiplier == 8)
                            cameraSpeedMultiplier = 16;

                        cameraDisplayTimer = 0;
                    }
                    else if (inputManager.mouseWheelDown)
                    {
                        if (cameraSpeedMultiplier >= 16)
                            cameraSpeedMultiplier = 8;
                        else if (cameraSpeedMultiplier == 8)
                            cameraSpeedMultiplier = 1;

                        cameraDisplayTimer = 0;
                    }
                }
                

                float camDisplayDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(1500).TotalMilliseconds);
                cameraDisplayTimer = MathHelper.Clamp(cameraDisplayTimer + camDisplayDelta, 0, 1);
                */

                //PC mouse cursor.
                cursorPos = inputManager.mousePos;

                if (FrameworkCore.players.Count > 1)
                    cursorPos.X *= 2;
            }
            else
            {
                //xbox: cursor is locked to center of screen.
                cursorPos = Helpers.GetScreenCenter();
            }

            if (playerMenus.Update(gameTime, inputmanager))
                return;

            UpdateCameraControls(gameTime);

            UpdateCamera(gameTime, position, Rotation);

            bool boardOn = (inputManager.buttonBackHeld && !IsPlaybackMode() && !isReady &&
                FrameworkCore.level.gamemode == GameMode.Orders);
            ingameScoreboard.Update(gameTime, boardOn);

            commandMenu.UpdateTransition(gameTime);
            playbackMenu.UpdateTransition(gameTime);
            shipMenu.UpdateTransition(gameTime);

#if WINDOWS
            if (activeMenu != commandMenu && activeMenu != playbackMenu && activeMenu != shipMenu &&
                !isReady && FrameworkCore.level.gamemode == GameMode.Orders)
                commandMenu.UpdateMouseButton(gameTime, inputManager);
#endif

            if (activeMenu == commandMenu)
                commandMenu.Update(gameTime, inputManager);
            else if (activeMenu == playbackMenu)
                playbackMenu.Update(gameTime, inputManager);
            else if (activeMenu == shipMenu)
                shipMenu.Update(gameTime, inputManager);

            
#if DEBUG
            if (inputManager.debugButton)
                FrameworkCore.debugMode = true;
            else
                FrameworkCore.debugMode = false;
#endif





            if ((hoverShip != null && selectedShip == null && hoverShip.owner == this) ||
                (currentMode != null && targetShip != null && (currentMode.modeName == ModeName.FacingMode || currentMode.modeName == ModeName.AimMode)))
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(100).TotalMilliseconds);
                crosshairTransition = MathHelper.Clamp(crosshairTransition + delta, 0, 1);
            }
            else
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(100).TotalMilliseconds);
                crosshairTransition = MathHelper.Clamp(crosshairTransition - delta, 0, 1);
            }






            if (selectedShip != null && selectedShip != lastSelectedShip)
            {
                lastSelectedShip = selectedShip;
            }




            //update the dpad timers.
            if (inputManager.buttonUpHeld && curOrientationTransition <= 0)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(700).TotalMilliseconds);

                dpadUpTimer = MathHelper.Clamp(dpadUpTimer + delta, 0, 1);
            }
            else if (dpadUpTimer > 0)
            {
                dpadUpTimer = 0;
            }


            if (inputManager.buttonDownHeld && curOrientationTransition <= 0)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(700).TotalMilliseconds);

                dpadDownTimer = MathHelper.Clamp(dpadDownTimer + delta, 0, 1);
            }
            else if (dpadDownTimer > 0)
            {
                dpadDownTimer = 0;
            }            


            

            //update transitions for playermodes.
            foreach (PlayerMode playerMode in playerModes)
            {
                if (playerMode == currentMode)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(500).TotalMilliseconds);

                    playerMode.transition = MathHelper.Clamp(playerMode.transition + delta, 0, 1);
                }
                else
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                            TimeSpan.FromMilliseconds(400).TotalMilliseconds);

                    playerMode.transition = MathHelper.Clamp(playerMode.transition - delta, 0, 1);
                }
            }


            if (drawHelpOverlay)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                HelpOverlayTransition = MathHelper.Clamp(HelpOverlayTransition + delta, 0, 1);
            }
            else if (HelpOverlayTransition > 0)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(200).TotalMilliseconds);

                HelpOverlayTransition = MathHelper.Clamp(HelpOverlayTransition - delta, 0, 1);
            }           


            base.Update(gameTime);
        }

        /// <summary>
        /// Fly the camera around the battlefield.
        /// </summary>
        /// <param name="gameTime"></param>
        public void DemoUpdate(GameTime gameTime)
        {
            double time = gameTime.TotalGameTime.TotalSeconds;


            Vector3 lookatPos = Vector3.Lerp(
                new Vector3(56, -20, 0),
                new Vector3(0, 0, -128), 0.5f +
                Helpers.Pulse(gameTime, 0.49f, 0.2f));


            float dx = (float)-Math.Cos(time * 0.1f);
            float dz = (float)-Math.Sin(time * 0.1f);

            float camDist = 80 + Helpers.Pulse(gameTime, 64, 0.2f);

            Vector3 newPosition = new Vector3(dx, 0, dz) * camDist;
            newPosition.Y = 4;

            newPosition.Y += Helpers.Pulse(gameTime, 32, 0.3f);

            newPosition += lookatPos;


            // Update entity position and velocity.
            position = newPosition;




            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Quaternion playerDesiredRotation = UpdateCameraRotation(dt);

            if (inputmanager.stickRight.Length() > deadZone || inputManager.mouseRightHeld)
            {
                demoCamTimer = 1500;
            }

            demoCamTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (demoCamTimer <= 0)
            {
                //cinema-controlled camera.
                Matrix lookAt = Matrix.CreateLookAt(this.position, lookatPos, Vector3.Up);
                Rotation = Quaternion.Lerp(Rotation,
                    Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt)),
                    0.01f);
                rotationMatrix = Matrix.CreateFromQuaternion(Rotation);
            }
            else
            {
                //player-controlled camera.
                Rotation = Quaternion.Lerp(Rotation,
                    playerDesiredRotation, 0.1f);
            }

            


            lockCamera.TargetPosition = position;
            lockCamera.TargetRotation = Rotation;
            lockCamera.Update(gameTime);
        }
        int demoCamTimer = 0;


        bool drawHelpOverlay = false;


        private bool SelectShipAvailable()
        {
            if (MoveOrderAvailable())
                return false;

            if (currentMode.modeName != ModeName.Default)
                return false;

            if (hoverShip == null)
                return false;

            if (hoverShip.owner != this)
                return false;

            return true;
        }

        private bool MoveOrderAvailable()
        {
            //if we don't have a ship selected: then move is unavailable.
            if (selectedShip == null)
                return false;

            //if we're hovering over our selected ship: then move is available.
            if (selectedShip == hoverShip)            
                return true;
            
            //if we're hovering over a ship this player does not own: then move is available.
            if (hoverShip != null)
            {
                if (hoverShip.owner != this)
                    return true;
            }

            //if we're hovering over a ship this player DOES own: then move is unavailable.
            if (hoverShip != null)
            {
                if (hoverShip.owner == this)
                    return false;
            }

            //click on empty space.
            return true;
        }

        private void SelectShipOnCrosshair(GameTime gameTime)
        {
            if (hoverShip != null)
            {
                if (hoverShip.owner != this)
                    return;

                if (hoverShip != selectedShip)
                {
                    SelectShip(gameTime, hoverShip, false);
                }
            }
        }

        private void SelectShip(GameTime gameTime, SpaceShip ship, bool getGoodAngle)
        {
            currentMovePos = ship.Position;
            orientationMatrix = Matrix.CreateFromQuaternion(ship.Rotation);

            selectedShip = ship;

            desiredGridAltitude = selectedShip.Position.Y;

            ActivateShipMenu();
            FrameworkCore.PlayCue(sounds.click.activate);

            tutHasShownCameraControls = true;

            if (getGoodAngle)
            {
                //moves camera above the ship.
                GetGoodAngleOnShip(gameTime, ship, false);
            }
        }

        public Vector3 MoveCamToSeeShipAndDisc(GameTime gameTime, Vector3 targetPos, Quaternion targetRotation, SpaceShip ship)
        {
            LockCamera testCamera = new LockCamera(FrameworkCore.Game);
            testCamera.TargetPosition = targetPos;
            testCamera.TargetRotation = targetRotation;

            //can already see both. return default value.
            if (CanSeeMoveDisc(testCamera, ship) && CanSeeShip(testCamera, ship))
                return targetPos;

            Matrix testMatrix = Matrix.CreateFromQuaternion(testCamera.TargetRotation);

            for (int x = 8; x < 1024; x += 8)
            {
                testCamera.TargetPosition += testMatrix.Backward * x;
                testCamera.Update(gameTime);

                if (CanSeeMoveDisc(testCamera, ship) && CanSeeShip(testCamera, ship))
                {
                    return testCamera.TargetPosition;
                }
            }

            //error. return default value.
            return targetPos;
        }

        public void MoveCamToSeeMoveDisc(GameTime gameTime, SpaceShip ship)
        {
            //see if the MoveDisc is in view of the camera. If not in view frustrum, then tell the camera
            // to move back until it can see the MoveDisc.
            if (!CanSeeMoveDisc(lockCamera, ship) || !CanSeeShip(lockCamera, ship))
            {
                LockCamera testCamera = new LockCamera(FrameworkCore.Game);
                testCamera.TargetPosition = lockCamera.CameraPosition;
                testCamera.TargetRotation = lockCamera.CameraRotation;

                for (int x = 8; x < 1024; x += 8)
                {
                    testCamera.TargetPosition += rotationMatrix.Backward * x;
                    testCamera.Update(gameTime);

                    if (CanSeeMoveDisc(testCamera, ship) && CanSeeShip(testCamera, ship))
                    {
                        warpCameraOrigin = this.position;
                        desiredWarpCameraPosition = this.position + rotationMatrix.Backward * (x + 64/*buffer*/);

                        warpCameraRotationOrigin = this.Rotation;
                        desiredWarpCameraRotation = this.Rotation;

                        warpCameraTransition = 0;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// move cam so it can see an arbitrary vector3 location
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spot"></param>
        public Vector3 MoveCamToSeeSpot(GameTime gameTime, Vector3 basePosition, Quaternion baseRotation, Vector3 lookSpot)
        {
            LockCamera testCamera = new LockCamera(FrameworkCore.Game);
            testCamera.TargetPosition = basePosition;
            testCamera.TargetRotation = baseRotation;

            if (!CanSeeSpot(testCamera, lookSpot))
            {
                for (int x = 4; x < 2048; x += 4)
                {
                    testCamera.TargetPosition += Matrix.CreateFromQuaternion(baseRotation).Backward * x;
                    testCamera.Update(gameTime);

                    if (CanSeeSpot(testCamera, lookSpot))
                    {
                        break;
                    }
                }
            }

            return testCamera.TargetPosition;
        }

        private bool CanSeeSpot(Camera camera, Vector3 spot)
        {
            BoundingSphere localSphere = new BoundingSphere(spot, 1f);

            ContainmentType contains = camera.BF.Contains(localSphere);
            if (contains == ContainmentType.Contains)
                return true;

            return false;
        }


        private bool CanSeeMoveDisc(Camera camera, SpaceShip ship)
        {
            BoundingSphere localSphere = new BoundingSphere(ship.Position, 0.01f);
            localSphere.Center.Y = 0;

            ContainmentType contains = camera.BF.Contains(localSphere);
            if (contains == ContainmentType.Contains)
                return true;

            return false;
        }

        private bool CanSeeShip(Camera camera, SpaceShip ship)
        {
            BoundingSphere localSphere = new BoundingSphere(ship.Position, 0.01f);            

            ContainmentType contains = camera.BF.Contains(localSphere);
            if (contains == ContainmentType.Contains)
                return true;

            return false;
        }

        private void ActivateShipMenu()
        {
            if (selectedShip != null)
            {
                currentMovePos = selectedShip.targetPos;                
            }

            activeMenu = shipMenu;
            shipMenu.Activate();
        }

        private bool movespotInitialized = false;

        private void ActivateMoveMode(object sender, EventArgs e)
        {
            if (selectedShip != null)
            {
#if XBOX
                advancedMode = false;
#else
                if (FrameworkCore.options.manualDefault)
                    advancedMode = true;
                else
                    advancedMode = false;

#endif
                PopulatePlayerModes();

                currentMovePos = selectedShip.Position;

                tutHasShownArmorTips = true;

                movespotInitialized = false;
                ChangePlayerMode(ModeName.MoveMode);
                CloseMenu(sender, e);

                centerTitleTransition = 0;
                //MoveCamToSeeMoveDisc(null, selectedShip);
            }
        }

        private void ActivateAimMode(object sender, EventArgs e)
        {
            if (selectedShip != null)
            {
                ChangePlayerMode(ModeName.AimMode);                
                CloseMenu(sender, e);
            }
        }

        private void CloseShipMenu(object sender, EventArgs e)
        {
            tutHasShownArmorTips = true;

            hoverShip = null;
            selectedShip = null;
            CloseMenu(sender, e);
        }

        private void CloseMenu(object sender, EventArgs e)
        {
            activeMenu = null;
            commandMenu.Deactivate();
        }

        public void ForceReady()
        {
            ResetSmiteTimer();

            selectedShip = null;
            hoverShip = null;
            ChangePlayerMode(ModeName.Default);

            isReady = true;

            tutHasShownCameraControls = true;
        }

        private void SetReady(object sender, EventArgs e)
        {
            ResetSmiteTimer();

            CloseMenu(null, null);

            selectedShip = null;
            hoverShip = null;
            ChangePlayerMode(ModeName.Default);
            

            isReady = true;

            tutHasShownCameraControls = true;


            //if it's a 2player game, then set the other player Ready if that player has no ships left.
            //this applies to both campaign and skirmish mode, regardless if player is an ally or enemy.
            if (FrameworkCore.players.Count > 1)
            {
                foreach (PlayerCommander player in FrameworkCore.players)
                {
                    //skip myself.
                    if (player == this)
                        continue;

                    //player is already ready. skip.
                    if (player.isReady == true)
                        continue;

                    bool hasShips = false;

                    foreach (Collideable ship in FrameworkCore.level.Ships)
                    {
                        if (!Helpers.IsSpaceship(ship))
                            continue;

                        if (ship.IsDestroyed)
                            continue;

                        if (ship.owner == null)
                            continue;

                        if (ship.owner != player)
                            continue;

                        hasShips = true;
                        break;
                    }

                    if (!hasShips)
                        player.isReady = true;
                }
            }
        }

        private void SetUnReady()
        {
            isReady = false;

            FrameworkCore.PlayCue(sounds.click.back);
        }

        public void ActivatePlaybackMode(object sender, EventArgs e)
        {
            activeMenu = playbackMenu;
            ChangePlayerMode(ModeName.Playback);
            playbackMenu.Activate();
            selectedShip = null;
            hoverShip = null;
        }




        private void ChangePlayerMode(ModeName name)
        {
            foreach (PlayerMode mode in playerModes)
            {
                if (mode.modeName == name)
                {
                    currentMode = mode;
                    break;
                }
            }
        }


        float ghostDistance; //distance from shipPos to ghostPos
        Vector3 ghostPos;
        Quaternion ghostRotation;

        private void UpdateGhost(GameTime gameTime)
        {
            if (selectedShip == null)
                return;

            if (currentMovePos == Vector3.Zero)
                return;

            if (currentMode.modeName == ModeName.Default && hoverShip == null)
                return;

            if (ghostDistance >= Vector3.Distance(selectedShip.Position, selectedShip.markPos) ||
                ghostDistance >= Vector3.Distance(selectedShip.Position, currentMovePos))
            {
                //arrived at destination, or passed destination. reset ghostShip to its starting pos.
                ghostPos = selectedShip.Position;
                ghostDistance = 0;
                ghostRotation = selectedShip.Rotation;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ghostDistance += selectedShip.GetSpeed() * dt;

            Vector3 moveDir = currentMovePos - selectedShip.Position;
            moveDir.Normalize();

            ghostPos = selectedShip.Position + moveDir * ghostDistance;

            ghostRotation = Quaternion.Lerp(ghostRotation, Quaternion.CreateFromRotationMatrix(orientationMatrix),
                   selectedShip.GetRotationSpeed() * dt);

        }

        private bool CanSwitchShips()
        {
            return (FrameworkCore.isCampaign && FrameworkCore.players.Count > 1 &&
                currentMode.modeName == ModeName.Default && hoverShip != null &&
                hoverShip.owner == this && FrameworkCore.level.gamemode == GameMode.Orders);
        }

        private bool ShowHoverText()
        {
            return (currentMode.modeName == ModeName.Default && hoverShip != null &&
                hoverShip.owner == this && FrameworkCore.level.gamemode == GameMode.Orders);
        }

        int selectedWarpShip = -1;

        public SpaceShip WarpCameraToShip(GameTime gameTime, SpaceShip targetShip)
        {
            SpaceShip ship = null;
            SpaceShip firstShip = null;

            if (targetShip == null)
            {
                foreach (Collideable thing in FrameworkCore.level.Ships)
                {
                    if (!Helpers.IsSpaceship(thing))
                        continue;

                    if (thing.IsDestroyed)
                        continue;

                    if (thing.owner != this)
                        continue;

                    if (firstShip == null)
                        firstShip = (SpaceShip)thing;

                    if (FrameworkCore.level.Ships.IndexOf(thing) > selectedWarpShip)
                    {
                        //keep track of the first friendly ship we find.
                        ship = (SpaceShip)thing;
                        selectedWarpShip = FrameworkCore.level.Ships.IndexOf(thing);
                        break;
                    }
                }
            }
            else
            {
                ship = targetShip;
            }

            if (ship == null)
            {
                if (firstShip != null)
                {
                    selectedWarpShip = FrameworkCore.level.Ships.IndexOf(firstShip);
                    ship = firstShip;
                }
            }

            if (ship != null)
            {
                //get position interpolation info.
                GetGoodAngleOnShip(gameTime, ship, false);
            }

            return ship;
        }


        /// <summary>
        /// find the general center-point of the enemy fleet.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCenterFleet(bool isEnemy)
        {
            int mostPositiveX = -int.MaxValue;
            int mostNegativeX = int.MaxValue;

            int mostPositiveZ = -int.MaxValue;
            int mostNegativeZ = int.MaxValue;

            int mostPositiveY = -int.MaxValue;
            int mostNegativeY = int.MaxValue;


            foreach (Collideable ship in FrameworkCore.level.Ships)
            {
                if (!Helpers.IsSpaceship(ship))
                    continue;

                if (ship.IsDestroyed)
                    continue;

                //sanity check.
                if (ship.owner == null)
                    continue;

                if (isEnemy)
                {
                    //skip friendlies.
                    if (ship.owner.factionName == this.factionName)
                        continue;
                }
                else
                {
                    //skip enemies.
                    if (ship.owner.factionName != this.factionName)
                        continue;
                }

                if (ship.Position.X > mostPositiveX)
                    mostPositiveX = (int)ship.Position.X;

                if (ship.Position.X < mostNegativeX)
                    mostNegativeX = (int)ship.Position.X;

                if (ship.Position.Z > mostPositiveZ)
                    mostPositiveZ = (int)ship.Position.Z;

                if (ship.Position.Z < mostNegativeZ)
                    mostNegativeZ = (int)ship.Position.Z;

                if (ship.Position.Y > mostPositiveY)
                    mostPositiveY = (int)ship.Position.Y;

                if (ship.Position.Y < mostNegativeY)
                    mostNegativeY = (int)ship.Position.Y;
            }

            return new Vector3(
                MathHelper.Lerp(mostPositiveX, mostNegativeX, 0.5f),
                MathHelper.Lerp(mostPositiveY, mostNegativeY, 0.5f),
                MathHelper.Lerp(mostPositiveZ, mostNegativeZ, 0.5f));
        }


        /// <summary>
        /// Tries to find a good place to position the camera.
        /// Rules:
        /// always stays above the Y plane.
        /// rotates to look at the "center" of the enemy fleet.
        /// always looks downward at the selected ship.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="ship"></param>

        private void GetGoodAngleOnShip(GameTime gameTime, SpaceShip ship, bool frameCurrentMovePos)
        {
            warpCameraOrigin = this.position;

            desiredWarpCameraPosition = ship.Position;


            Vector3 enemyPosition = GetCenterFleet(true);
            enemyPosition.Y = ship.Position.Y;
            Vector3 dirToEnemyFleet = ship.Position - enemyPosition;
            dirToEnemyFleet.Normalize();



            desiredWarpCameraPosition += (dirToEnemyFleet * 96) + (Vector3.Up * 1);
            
            desiredWarpCameraPosition.Y = ship.Position.Y + 32;

            desiredWarpCameraPosition.Y = Math.Max(desiredWarpCameraPosition.Y, 32);


            //get rotation interpolation info.
            warpCameraRotationOrigin = this.Rotation;

            Matrix lookAt = Matrix.CreateLookAt(desiredWarpCameraPosition, ship.Position, Vector3.Up);
            desiredWarpCameraRotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));


            desiredWarpCameraPosition = MoveCamToSeeShipAndDisc(gameTime, desiredWarpCameraPosition,
                desiredWarpCameraRotation, ship);



            if (frameCurrentMovePos)
            {
                //frame the currentmovepos.
                //frame the enemies.

                desiredWarpCameraPosition = MoveCamToSeeSpot(gameTime, desiredWarpCameraPosition,
                    desiredWarpCameraRotation, currentMovePos);

                desiredWarpCameraPosition = MoveCamToSeeSpot(gameTime, desiredWarpCameraPosition,
                        desiredWarpCameraRotation,
                        new Vector3(currentMovePos.X,0,currentMovePos.Z));
            }


            /*
            //frame camera to see enemies.
            desiredWarpCameraPosition = MoveCamBackToSeeEnemies(gameTime, desiredWarpCameraPosition,
                desiredWarpCameraRotation);
            desiredWarpCameraPosition += lookAt.Backward * 32;
            */

            warpCameraTransition = 0;
        }

        private Vector3 MoveCamBackToSeeEnemies(GameTime gameTime, Vector3 targetPos, Quaternion targetRotation)
        {
            Vector3 camPos = targetPos;

            foreach (Collideable enemy in FrameworkCore.level.Ships)
            {
                if (!Helpers.IsSpaceship(enemy))
                    continue;

                if (enemy.IsDestroyed)
                    continue;

                if (((SpaceShip)enemy).owner == null)
                    continue;

                if (((SpaceShip)enemy).owner.factionName == this.factionName)
                    continue;

                camPos = MoveCamToSeeShipAndDisc(gameTime, targetPos, targetRotation, (SpaceShip)enemy);
            }

            return camPos;
        }

        public SpaceShip GetShipWithoutOrders(SpaceShip shipToIgnore)
        {
            SpaceShip ship = null;
            
            foreach (Collideable thing in FrameworkCore.level.Ships)
            {
                if (!Helpers.IsSpaceship(thing))
                    continue;

                if (thing.IsDestroyed)
                    continue;

                if (thing.owner != this)
                    continue;

                if (thing == shipToIgnore)
                    continue;

                if (((SpaceShip)thing).targetPos != Vector3.Zero)
                    continue;

                ship = (SpaceShip)thing;
            }            

            return ship;
        }


        public void WarpCameraToPosition(Vector3 camPos)
        {
            warpCameraOrigin = this.position;
            desiredWarpCameraPosition = camPos + (rotationMatrix.Backward * 96) + (rotationMatrix.Down * 1);

            warpCameraRotationOrigin = this.Rotation;
            desiredWarpCameraRotation = this.Rotation;

            warpCameraTransition = 0;
        }



        Vector3 warpCameraOrigin = Vector3.Zero;
        Vector3 desiredWarpCameraPosition = Vector3.Zero;

        Quaternion warpCameraRotationOrigin = Quaternion.Identity;
        Quaternion desiredWarpCameraRotation = Quaternion.Identity;

        float warpCameraTransition = 1;

        public void ActivateCommandMenu()
        {
            if (commandMenu.Transition > 0)
                return;

            activeMenu = commandMenu;
            commandMenu.Activate();
        }

        Vector2 lastMousePos = Vector2.Zero;

        float confirmButtonTransition = 0;
        int confirmButtonHover = -1;



        private void ChangeToNextMode(GameTime gameTime, ModeName currentMode)
        {
            int index = -1;
            foreach (PlayerMode mode in playerModes)
            {
                if (mode.modeName == currentMode)
                {
                    index = playerModes.IndexOf(mode);
                    break;
                }
            }

            //error check.
            if (index < 0)
                return;

            if (selectedShip == null)
                return;

            //error checking.
            if (index + 1 > playerModes.Count - 1)
            {
                //no more modes left in the queue. commit the order.
                Quaternion targetRot = Quaternion.CreateFromRotationMatrix(orientationMatrix);

                Vector3 destination = currentMovePos;
                if (selectedShip.markShouldDraw)
                {
                    Vector3 moveDir = currentMovePos - selectedShip.Position;
                    moveDir.Normalize();

                    destination = selectedShip.Position + (moveDir * (selectedShip.markDist + 0.01f));
                }

                selectedShip.SetTargetMove(destination, targetRot);

                ChangePlayerMode(ModeName.Default);

                SpaceShip curShip = selectedShip;
                ClearSelectedShip();

                //auto-select the next ship.
                if (SelectNextAvailableShip(gameTime, curShip))
                {
                    //selected a new ship. play a whoosh sound.
                    FrameworkCore.PlayCue(sounds.click.whoosh);
                }
                else
                {
                    //no other ship available. return to a default cam angle.
                    //tutWarpCamToGoodAngle(gameTime);

                    //open the End Turn menu.
                    FrameworkCore.PlayCue(sounds.Fanfare.ready);
                    ActivateCommandMenu();
                }

                FrameworkCore.PlayCue(sounds.Fanfare.radio);

                if (isTutorialLevel && !tutShownNextShipBox)
                {
                    ShowNextShipTutorial();
                }

                return;
            }

            centerTitleTransition = 0;
            ChangePlayerMode(playerModes[index + 1].modeName);

            if (selectedShip != null)
            {
                if (playerModes[index + 1].modeName == ModeName.MoveMode || playerModes[index + 1].modeName == ModeName.HeightMode)
                    MoveCamToSeeMoveDisc(null, selectedShip);

                if (playerModes[index + 1].modeName == ModeName.FacingMode)
                    WarpFacingCamera(gameTime);
            }
        }

        private void ChangeToPrevMode(GameTime gameTime, ModeName currentMode)
        {
            int index = -1;
            foreach (PlayerMode mode in playerModes)
            {
                if (mode.modeName == currentMode)
                {
                    index = playerModes.IndexOf(mode);
                    break;
                }
            }

            //error check.
            if (index < 0)
                return;

            //error checking.
            if (index - 1 < 0)
                return;

            centerTitleTransition = 0;
            ChangePlayerMode(playerModes[index - 1].modeName);

            if (selectedShip != null)
            {
                if (playerModes[index - 1].modeName == ModeName.MoveMode || playerModes[index - 1].modeName == ModeName.HeightMode)
                    MoveCamToSeeMoveDisc(null, selectedShip);

                if (playerModes[index - 1].modeName == ModeName.FacingMode)
                    WarpFacingCamera(gameTime);
            }
        }


        private bool SelectNextAvailableShip(GameTime gameTime, SpaceShip shipToIgnore)
        {
            SpaceShip nextShip = GetShipWithoutOrders(shipToIgnore);

            //found a ship that doesn't have orders.
            if (nextShip != null)
            {
                //WarpCameraToShip(gameTime, nextShip);
                SelectShip(gameTime, nextShip, true);
                return true;
            }
            
            //all ships already have orders!
            return false;            
        }


        private Vector3 ClampMoveCursor(Vector3 basePos)
        {
            Vector3 newPos = basePos;

            if (selectedShip != null)
            {
                float distToPos = Vector2.Distance(new Vector2(selectedShip.Position.X, selectedShip.Position.Z),
                    new Vector2(newPos.X, newPos.Z));

                if (distToPos > 250)
                {
                    Vector2 moveDir = new Vector2(newPos.X, newPos.Z) -
                        new Vector2(selectedShip.Position.X, selectedShip.Position.Z);
                    moveDir.Normalize();

                    Vector2 adjustedPos = new Vector2(selectedShip.Position.X, selectedShip.Position.Z) +
                        moveDir * 250;

                    newPos.X = adjustedPos.X;
                    newPos.Z = adjustedPos.Y;
                }
            }

            return newPos;
        }


        bool isMouseMoving = false;

        public void UpdateCursor(GameTime gameTime, List<Collideable> ships)
        {
            if (isTutorialLevel && tutState < TutState.doOrders)
                return;

            UpdateMarkTimes(gameTime);

            if (activeMenu != null)
            {
                hoverShip = null;
                return;
            }

            UpdateExpBar(gameTime);

            if (CommandMenuAvailable())
            {
                if ((inputManager.OpenMenu || inputManager.kbEnter) && shipMenu.Transition <= 0)
                {
                    
                    
                    ActivateCommandMenu();
                    
                }
            }


            if (shipMenu.Transition > 0)
                return;

            if (!mouseEnabled)
            {
                if (currentMode != null && currentMode.modeName == ModeName.MoveMode)
                {
                    if (inputManager.stickLeft.Length() > 0.3f || inputManager.stickRight.Length() > 0.3f
                        || inputManager.cameraLower > 0.3f || inputManager.cameraRaise > 0.3f)
                    {
                        float confirmDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                            TimeSpan.FromMilliseconds(350).TotalMilliseconds);
                        confirmButtonTransition = MathHelper.Clamp(confirmButtonTransition - confirmDelta, 0, 1);
                    }
                    else
                    {
                        float confirmDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                            TimeSpan.FromMilliseconds(150).TotalMilliseconds);
                        confirmButtonTransition = MathHelper.Clamp(confirmButtonTransition + confirmDelta, 0, 1);
                    }
                }
                else
                {
                    confirmButtonTransition = 1;
                }
            }

            

#if WINDOWS
            if (mouseEnabled)
            {
                if (inputManager.mouseLeftHeld || inputManager.mouseRightHeld)
                {
                    float confirmDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(350).TotalMilliseconds);
                    confirmButtonTransition = MathHelper.Clamp(confirmButtonTransition - confirmDelta, 0, 1);
                }
                else
                {
                    float confirmDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                TimeSpan.FromMilliseconds(150).TotalMilliseconds);
                    confirmButtonTransition = MathHelper.Clamp(confirmButtonTransition + confirmDelta, 0, 1);
                }
            }
#endif

            UpdateGhost(gameTime);

            #region Controls
            blinkTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (blinkTime > Helpers.BLINKTIME)
            {
                blinkTime = 0;
                blinkOn = !blinkOn;
            }
            #endregion
            
            #region DefaultMode            
            if (currentMode.modeName == ModeName.Default)
            {
#if DEBUG
                if (inputManager.kbIPressed)
                {
                    if (hoverShip != null)
                    {
                        Matrix m = Matrix.CreateFromQuaternion(hoverShip.Rotation);

                        //m = m * Matrix.CreateFromAxisAngle(m.Right, -0.5f);
                        //hoverShip.Rotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(m));

                        Vector3 right = Vector3.Transform(Vector3.Right, m);

                        hoverShip.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, 0)
                            * Quaternion.CreateFromAxisAngle(Vector3.Right, -0.5f)
                            * Quaternion.CreateFromAxisAngle(Vector3.Backward, 0);

                    }
                }
#endif

#if WINDOWS
                if (inputManager.HideHudToggle)
                {
                    FrameworkCore.HideHud = !FrameworkCore.HideHud;
                }
#endif

                if (inputManager.buttonAPressed || inputManager.mouseLeftClick)
                {
                    SelectShipOnCrosshair(gameTime);
                }

                if ((inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed) && isReady && FrameworkCore.level.gamemode != GameMode.Action)
                {
                    SetUnReady();
                }
                
#if WINDOWS
                if (mouseEnabled && isReady)
                {
                    Rectangle readyRect = new Rectangle(
                        (int)readyButtonPos.X - 140,
                        (int)readyButtonPos.Y - 24,
                        280, 48);

                    if (readyRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                    {
                        if (!readyButtonHover)
                        {
                            FrameworkCore.PlayCue(sounds.click.select);
                            readyButtonHover = true;
                        }

                        if (inputManager.mouseLeftClick)
                            SetUnReady();
                    }
                    else if (readyButtonHover)
                    {
                        readyButtonHover = false;
                    }
                }
#endif

                if (inputManager.camResetClick)
                {
                    tutWarpCamToGoodAngle(gameTime);
                }

                if (inputManager.camSelectNextShip && !isReady && commandMenu.Transition <= 0 && shipMenu.Transition <= 0 && playbackMenu.Transition <= 0)
                {
                    if (!SelectNextAvailableShip(gameTime, null))
                    {
                        //no more ships available. so, open the command menu.
                        ActivateCommandMenu();
                    }
                }

                if (!isReady && CanSwitchShips() && (inputManager.buttonYPressed || inputManager.kbGPressed))
                {
                    if (hoverShip.owner == FrameworkCore.players[1])
                        hoverShip.owner = FrameworkCore.players[0];
                    else
                        hoverShip.owner = FrameworkCore.players[1];
                }

                if (!isReady)
                {
                    SpaceShip hoverTemp = UpdateHover(ships);
                    if (hoverShip != hoverTemp)
                    {
                        hoverShip = hoverTemp;

                        if (hoverShip != null && hoverShip.owner != null && hoverShip.owner == this)
                            FrameworkCore.PlayCue(sounds.click.beep);
                    }
                        
                }
            }             
            #endregion

            #region MoveMode
            else if (currentMode.modeName == ModeName.MoveMode)
            {
                //Matrix playerOrientation = Matrix.CreateFromQuaternion(Rotation);
                //Ray cursorRay = new Ray(position, playerOrientation.Forward);

                
#if WINDOWS
                UpdateConfirmButtons();

                if (mouseEnabled)
                {
                    if (!movespotInitialized)
                    {
                        movespotInitialized = true;
                        Vector3 newPos = GetPickedPosition(cursorPos, 0.1f, lockCamera.Projection, lockCamera.View,
                            -gridAltitude, new Vector3(0f, 1f, 0f));
                        currentMovePos = new Vector3(newPos.X, currentMovePos.Y, newPos.Z);
                    }

                    if (inputManager.mouseLeftHeld && confirmButtonHover < 0)
                    {
                        isMouseMoving = true;
                    }
                    else if (!inputManager.mouseLeftHeld)
                    {
                        isMouseMoving = false;
                    }

                    if (inputManager.mouseLeftHeld && isMouseMoving)
                    {
                        Vector3 newPos = GetPickedPosition(cursorPos, 0.1f, lockCamera.Projection, lockCamera.View,
                            -gridAltitude, new Vector3(0f, 1f, 0f));
                        currentMovePos = new Vector3(newPos.X, currentMovePos.Y, newPos.Z);
                    }
                }
                else
                {
                    Vector3 newPos = GetPickedPosition(cursorPos, 0.1f, lockCamera.Projection, lockCamera.View,
                            -gridAltitude, new Vector3(0f, 1f, 0f));

                    //CLAMP HOW FAR THE CURSOR CAN MOVE.
                    //newPos = ClampMoveCursor(newPos);                    

                    currentMovePos = new Vector3(newPos.X, currentMovePos.Y, newPos.Z);                    
                }
#else
                Vector3 newPos = GetPickedPosition(cursorPos, 0.1f, lockCamera.Projection, lockCamera.View,
                            -gridAltitude, new Vector3(0f, 1f, 0f));

                //CLAMP HOW FAR THE CURSOR CAN MOVE.
                //newPos = ClampMoveCursor(newPos);

                currentMovePos = new Vector3(newPos.X, currentMovePos.Y, newPos.Z);                    
#endif

                if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed ||
                    (confirmButtonHover == 0 && inputManager.mouseLeftClick))
                {
                    //undo the order, revert to the last order.
                    selectedShip.RevertToLastOrderEffect();

                    ChangePlayerMode(ModeName.Default);
                    ActivateShipMenu();
                    FrameworkCore.PlayCue(sounds.click.back);
                }

                if (inputManager.camResetClick)
                    GetGoodAngleOnShip(gameTime, selectedShip, true);

                if (inputManager.buttonAPressed || inputManager.kbEnter || inputManager.kbSpace ||
                    (confirmButtonHover == 1 && inputManager.mouseLeftClick))
                {
                    //lock in the position. now enter heightMode.
                    ChangeToNextMode(gameTime, ModeName.MoveMode);
                    FrameworkCore.PlayCue(sounds.click.select);
                }

                if (selectedShip != null)
                {
                    Matrix lookAt = Matrix.CreateLookAt(selectedShip.Position, currentMovePos, Vector3.Up);
                    orientationMatrix = Matrix.Invert(lookAt);
                }
            }
            #endregion


            #region HeightMode
            //choose vertical movement for ship.
            else if (currentMode.modeName == ModeName.HeightMode)
            {                
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                UpdateConfirmButtons();

                if (inputManager.buttonUpHeld)
                {
                    float moveStep = MathHelper.Lerp(0.1f, 96, dpadUpTimer);
                    currentMovePos.Y += moveStep * dt;
                }
                else if (inputManager.buttonDownHeld)
                {
                    float moveStep = MathHelper.Lerp(0.1f, 96, dpadDownTimer);
                    currentMovePos.Y -= moveStep * dt;
                }

                if (inputManager.camResetClick)
                    WarpCameraToShip(gameTime, selectedShip);

                if (inputManager.buttonAPressed || inputManager.kbEnter || inputManager.kbSpace ||
                    (confirmButtonHover == 1 && inputManager.mouseLeftClick))
                {
                    ChangeToNextMode(gameTime, ModeName.HeightMode);
                    FrameworkCore.PlayCue(sounds.click.select);
                }
                else if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed ||
                    (confirmButtonHover == 0 && inputManager.mouseLeftClick))
                {
                    ChangeToPrevMode(gameTime, ModeName.HeightMode);
                    FrameworkCore.PlayCue(sounds.click.back);
                }

#if WINDOWS
                if (inputManager.mouseLeftHeld && confirmButtonHover < 0)
                {
                    if (inputManager.mouseLeftStartHold)
                    {
                        lastMousePos = inputManager.mousePos;
                    }

                    currentMovePos.Y += (lastMousePos.Y - inputManager.mousePos.Y)/(128 * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    lastMousePos = inputManager.mousePos;
                }
#endif
            }
            #endregion

            #region YawMode
            else if (currentMode.modeName == ModeName.YawMode)
            {
                UpdateOrientationControls(gameTime);
                UpdateConfirmButtons();

                if (inputManager.buttonAPressed || inputManager.kbEnter || inputManager.kbSpace ||
                    (confirmButtonHover == 1 && inputManager.mouseLeftClick))
                {
                    ChangePlayerMode(ModeName.PitchMode);
                    FrameworkCore.PlayCue(sounds.click.select);
                }
                else if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed ||
                    (confirmButtonHover == 0 && inputManager.mouseLeftClick))
                {
                    ChangePlayerMode(ModeName.HeightMode);
                    FrameworkCore.PlayCue(sounds.click.back);
                }

                if (inputmanager.advancedMoveToggle || (inputManager.mouseLeftClick && confirmButtonHover >= 2))
                {
                    ToggleAdvancedMove();
                }

                if (inputManager.camResetClick)
                    WarpCameraToShip(gameTime, selectedShip);

                orientationMatrix = orientationMatrix * Matrix.CreateFromAxisAngle(orientationMatrix.Up, -orientationTransition);
            }
            #endregion

            #region PitchMode
            else if (currentMode.modeName == ModeName.PitchMode)
            {
                UpdateOrientationControls(gameTime);
                UpdateConfirmButtons();

                if (inputManager.buttonAPressed || inputManager.kbEnter || inputManager.kbSpace ||
                    (confirmButtonHover == 1 && inputManager.mouseLeftClick))
                {
                    ChangePlayerMode(ModeName.RollMode);
                    FrameworkCore.PlayCue(sounds.click.select);
                }
                else if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed ||
                    (confirmButtonHover == 0 && inputManager.mouseLeftClick))
                {
                    ChangePlayerMode(ModeName.YawMode);
                    FrameworkCore.PlayCue(sounds.click.back);
                }

                if (inputManager.camResetClick)
                    WarpCameraToShip(gameTime, selectedShip);

                orientationMatrix = orientationMatrix * Matrix.CreateFromAxisAngle(orientationMatrix.Right, orientationTransition);
            }
            #endregion

            #region RollMode
            else if (currentMode.modeName == ModeName.RollMode)
            {
                UpdateOrientationControls(gameTime);
                UpdateConfirmButtons();

                if (inputManager.buttonAPressed || inputManager.kbEnter || inputManager.kbSpace ||
                    (confirmButtonHover == 1 && inputManager.mouseLeftClick))
                {
                    //commit the move order.
                    Quaternion targetRot = Quaternion.CreateFromRotationMatrix(orientationMatrix);
                    selectedShip.SetTargetMove(currentMovePos, targetRot);

                    ChangeToNextMode(gameTime, ModeName.RollMode);

                    /*
                    if (!selectedShip.OrderEffect.canFire)
                    {
                        ChangePlayerMode(ModeName.Default);
                        ClearSelectedShip();
                    }
                    else
                    {
                        //proceed to aim mode.
                        targetShip = null;
                        ChangePlayerMode(ModeName.AimMode);
                    }
                     */
                     
                    //ActivateShipMenu();
                    FrameworkCore.PlayCue(sounds.click.select);
                }
                else if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed ||
                    (confirmButtonHover == 0 && inputManager.mouseLeftClick))
                {
                    ChangePlayerMode(ModeName.PitchMode);
                    FrameworkCore.PlayCue(sounds.click.back);
                }

                if (inputManager.camResetClick)
                    WarpCameraToShip(gameTime, selectedShip);

                orientationMatrix = orientationMatrix * Matrix.CreateFromAxisAngle(orientationMatrix.Forward, orientationTransition);
            }
            #endregion
            
            else if (currentMode.modeName == ModeName.AimMode)
            {
                UpdateAimMode(gameTime, ships);
            }
            else if (currentMode.modeName == ModeName.FacingMode)
            {
                UpdateFacingMode(gameTime, ships);
            }



            //==================== MODIFY ORDERS.
            else if (currentMode.modeName == ModeName.ModifyHeightMode)
            {

                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (inputManager.buttonUpHeld)
                {
                    float moveStep = MathHelper.Lerp(0.1f, 96, dpadUpTimer);
                    currentMovePos.Y += moveStep * dt;
                }
                else if (inputManager.buttonDownHeld)
                {
                    float moveStep = MathHelper.Lerp(0.1f, 96, dpadDownTimer);
                    currentMovePos.Y -= moveStep * dt;
                }

                if (inputManager.buttonAPressed)
                {
                    Quaternion targetRot = Quaternion.CreateFromRotationMatrix(orientationMatrix);
                    selectedShip.SetTargetMove(currentMovePos, targetRot);

                    ChangePlayerMode(ModeName.Default);
                    FrameworkCore.PlayCue(sounds.click.select);
                    ClearSelectedShip();
                }
                else if (inputManager.buttonBPressed)
                {
                    ChangePlayerMode(ModeName.Default);
                    FrameworkCore.PlayCue(sounds.click.back);

                    if (selectedShip != null)
                        ActivateShipMenu();
                }
            }
            else if (currentMode.modeName == ModeName.ModifyPriorityTarget)
            {
                if (inputManager.buttonBPressed)
                {
                    ChangePlayerMode(ModeName.Default);
                    FrameworkCore.PlayCue(sounds.click.back);

                    if (selectedShip != null)
                        ActivateShipMenu();
                }

                if (inputManager.buttonAPressed)
                {
                    if (targetShip == null)
                    {
                        //error bloop sound.
                        //FrameworkCore.PlayCue(sounds.click.back);
                    }
                    else
                    {
                        selectedShip.SetTargetShip(targetShip);
                        ChangePlayerMode(ModeName.Default);
                        ClearSelectedShip();

                        targetShip = null;
                        //FrameworkCore.PlayCue(sounds.click.select);
                    }
                }
                else if (inputManager.buttonXPressed)
                {
                    selectedShip.SetTargetShip(null);
                    ChangePlayerMode(ModeName.Default);
                    ClearSelectedShip();

                    targetShip = null;
                }

                Collideable curTargetShip = UpdateEnemyHover(ships);

                if (curTargetShip == null)
                {
                    if (targetShip != null)
                    {
                        targetShip = null;
                    }
                    return;
                }

                //don't select teammates.
                if (curTargetShip.owner == this)
                {
                    targetShip = null;
                    return;
                }

                //only target spaceships.
                if (!Helpers.IsSpaceship(curTargetShip))
                    return;

                //do not target friendlies.
                if (curTargetShip.owner.factionName == this.factionName)
                    return;

                //update the targeted ship.
                if (targetShip != curTargetShip)
                {
                    targetShipTransition = 0;
                    targetShip = curTargetShip;

                    if (targetShip != null)
                        FrameworkCore.PlayCue(sounds.click.beep);
                }

                if (targetShipTransition < 1)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(400).TotalMilliseconds);
                    targetShipTransition = MathHelper.Clamp(targetShipTransition + delta, 0, 1);
                }
            }
        }

        private void UpdateConfirmButtons()
        {
            if (confirmButtonPositions == null)
                return;

            bool isHovering = false;
            for (int x = 0; x < confirmButtonPositions.Length; x++)
            {
                if (currentMode != null)
                {
                    if (currentMode.modeName != ModeName.FacingMode &&
                        currentMode.modeName != ModeName.YawMode && x > 1)
                    {
                        continue;
                    }

                    if (x == 1 && currentMode.modeName == ModeName.FacingMode)
                        continue;
                }

                Rectangle buttonRect = new Rectangle(
                    (int)confirmButtonPositions[x].X - 140,
                    (int)confirmButtonPositions[x].Y - 24,
                    280, 48);

                if (buttonRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    isHovering = true;
                    confirmButtonHover = x;
                }
            }

            if (!isHovering)
            {
                confirmButtonHover = -1;
            }
        }

        private void UpdateMarkTimes(GameTime gameTime)
        {

            for (int x = 0; x < FrameworkCore.level.Ships.Count; x++)
            {
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[x]))
                    continue;

                SpaceShip ship = (SpaceShip)FrameworkCore.level.Ships[x];

                if (ship.IsDestroyed)
                    continue;

                if (ship.owner.factionName != this.factionName)
                    continue;

                if (ship.owner != this)
                    continue;

                Vector3 destSpot = Vector3.Zero;

                if (selectedShip != null && selectedShip == ship && currentMode.modeName != ModeName.Default)
                {
                    destSpot = currentMovePos;
                }
                else if (ship.targetPos != Vector3.Zero)
                {
                    destSpot = ship.targetPos;
                }
                else
                {
                    ship.markPos = new Vector3(9,9,9);
                    ship.markShouldDraw = false;
                    continue;
                }                

                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float markDistTemp = ship.GetSpeed() * elapsed;                

                float travelDist = Vector3.Distance(ship.Position, destSpot) * elapsed;
                ship.markTimeLength = travelDist / markDistTemp;

                markDistTemp *= (Helpers.MAXROUNDTIME / 1000f) / elapsed;
                ship.markDist = markDistTemp;

                Vector3 markDir = destSpot - ship.Position;
                markDir.Normalize();
                ship.markPos = ship.Position + markDir * ship.markDist;



                if (Vector3.Distance(ship.Position, destSpot) > ship.markDist)
                {
                    ship.markShouldDraw = true;
                }
                else
                    ship.markShouldDraw = false;
            }


        }

        private void ToggleAdvancedMove()
        {
            FrameworkCore.PlayCue(sounds.click.beep);

            advancedMode = !advancedMode;
            currentMode = null;



            PopulatePlayerModes();

            if (advancedMode)
            {
                //activate manual mode.
                if (selectedShip != null)
                {
                    Matrix m = Matrix.CreateFromQuaternion(selectedShip.Rotation);
                    orientationMatrix = m;
                }


                ChangePlayerMode(ModeName.YawMode);
            }
            else
            {
                //activate basic mode.
                orientationMatrix = Matrix.Identity;


                ChangePlayerMode(ModeName.FacingMode);
            }
        }

        private void PopulatePlayerModes()
        {
            try
            {
                //remove some modes. make sure to go backwards so we don't hit null references.
                for (int x = playerModes.Count - 1; x >= 3; x--)
                {
                    if (playerModes[x] == null)
                        continue;

                    playerModes.RemoveAt(x);
                }

                if (advancedMode)
                {
                    //advanced mode.
                    PlayerMode p = new PlayerMode(ModeName.YawMode, Resource.ModeYawOrientation);
                    p.menuCategory = ModeCategory.Move;
                    playerModes.Add(p);

                    p = new PlayerMode(ModeName.PitchMode, Resource.ModePitchOrientation);
                    p.menuCategory = ModeCategory.Move;
                    playerModes.Add(p);

                    p = new PlayerMode(ModeName.RollMode, Resource.ModeRollOrientation);
                    p.menuCategory = ModeCategory.Move;
                    playerModes.Add(p);

                    if (selectedShip.OrderEffect.canFire && NumberOfEnemies() > 1)
                    {
                        p = new PlayerMode(ModeName.AimMode, Resource.ModeAimMode);
                        p.menuCategory = ModeCategory.Aim;
                        playerModes.Add(p);
                    }
                }
                else
                {
                    //simple mode.
                    PlayerMode p = new PlayerMode(ModeName.FacingMode, Resource.ModeFacingMode);
                    p.menuCategory = ModeCategory.Move;
                    playerModes.Add(p);

                    if (selectedShip.OrderEffect.canFire && NumberOfEnemies() > 1)
                    {
                        p = new PlayerMode(ModeName.AimMode, Resource.ModeAimMode);
                        p.menuCategory = ModeCategory.Aim;
                        playerModes.Add(p);
                    }
                }
            }
            catch
            {
            }

            if (selectedShip == null)
                return;
        }

        private int NumberOfEnemies()
        {
            int numOfShips = 0;
            foreach (Collideable ship in FrameworkCore.level.Ships)
            {
                if (ship.IsDestroyed)
                    continue;

                if (!Helpers.IsSpaceship(ship))
                    continue;

                if (ship.owner == null)
                    continue;

                if (ship.owner.factionName == this.factionName)
                    continue;

                numOfShips++;
            }

            return numOfShips;
        }



        //Scroll through all the enemies in the map.
        public void WarpFacingCamera(GameTime gameTime)
        {
            if (selectedShip == null)
                return;

            SpaceShip ship = null;
            SpaceShip firstShip = null;

            try
            {
                foreach (Collideable thing in FrameworkCore.level.Ships)
                {
                    if (!Helpers.IsSpaceship(thing))
                        continue;

                    if (thing.IsDestroyed)
                        continue;

                    if (thing.owner == null)
                        continue;

                    if (thing.owner.factionName == this.factionName)
                        continue;

                    if (firstShip == null)
                        firstShip = (SpaceShip)thing;

                    if (FrameworkCore.level.Ships.IndexOf(thing) > selectedWarpShip)
                    {
                        //keep track of the first friendly ship we find.
                        ship = (SpaceShip)thing;
                        selectedWarpShip = FrameworkCore.level.Ships.IndexOf(thing);
                        break;
                    }
                }
            }
            catch
            {
            }

            if (ship == null)
            {
                if (firstShip != null)
                {
                    selectedWarpShip = FrameworkCore.level.Ships.IndexOf(firstShip);
                    ship = firstShip;
                }
            }

            if (ship != null)
            {
#if WINDOWS
                if (mouseEnabled)
                {
                    inputManager.ForceMouseCenter();
                }
#endif

                //get position interpolation info.
                Vector3 destination = currentMovePos;

                if (selectedShip.markShouldDraw)
                    destination = selectedShip.markPos;


                Vector3 dirToEnemy = ship.Position - destination;
                dirToEnemy.Normalize();

                warpCameraOrigin = this.position;

                desiredWarpCameraPosition = destination;
                desiredWarpCameraPosition += dirToEnemy * -32;
                desiredWarpCameraPosition.Y += 8;

                Vector3 perpendicularToEnemy = Vector3.Cross(Vector3.Up, dirToEnemy);
                desiredWarpCameraPosition += perpendicularToEnemy * -12;



                warpCameraRotationOrigin = this.Rotation;

                Matrix lookAt = Matrix.CreateLookAt(desiredWarpCameraPosition, ship.Position, Vector3.Up);
                desiredWarpCameraRotation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(lookAt));

                warpCameraTransition = 0;
            }
            
        }


        private void UpdateFacingMode(GameTime gameTime, List<Collideable> ships)
        {
            UpdateConfirmButtons();

            if (inputmanager.advancedMoveToggle || (inputManager.mouseLeftClick && confirmButtonHover >= 2))
            {
                ToggleAdvancedMove();
            }

            if (targetShip != null && selectedShip != null)
            {
                if (targetShip.GetType() == typeof(SpaceShip))
                {
                    Vector3 destination = currentMovePos;

                    if (selectedShip.markShouldDraw)
                        destination = selectedShip.markPos;

                    Matrix lookAt = Matrix.CreateLookAt(destination, targetShip.Position, Vector3.Up);
                    orientationMatrix = Matrix.Invert(lookAt);
                }
            }


            if (inputManager.camNextTarget || inputManager.MouseMiddleClick)
            {
                WarpFacingCamera(gameTime);
            }

            if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed ||
                (confirmButtonHover == 0 && inputManager.mouseLeftClick))
            {
                ChangeToPrevMode(gameTime, ModeName.FacingMode);
                FrameworkCore.PlayCue(sounds.click.back);
            }

            if (inputManager.buttonAPressed || (inputManager.mouseLeftClick && confirmButtonHover < 0))
            {
                if (targetShip == null)
                {
                    //error bloop sound.
                    //FrameworkCore.PlayCue(sounds.click.back);
                }
                else
                {
                    targetShip = null;
                    ChangeToNextMode(gameTime, ModeName.FacingMode);
                    return;
                }
            }

            UpdateTargetShip(gameTime, ships);
        }

        private void UpdateTargetShip(GameTime gameTime, List<Collideable> ships)
        {
            //Collideable curTargetShip = UpdateEnemyHover(ships);
            Collideable curTargetShip = UpdateHover(ships);

            

            if (curTargetShip == null)
            {
                targetShip = null;                
                return;
            }

            //don't select teammates.
            if (curTargetShip.owner == this)
            {
                targetShip = null;
                return;
            }

            //only target spaceships.
            if (!Helpers.IsSpaceship(curTargetShip))
                return;

            //do not target friendlies.
            if (curTargetShip.owner.factionName == this.factionName)
                return;

            //update the targeted ship.
            if (targetShip != curTargetShip)
            {
                targetShipTransition = 0;
                targetShip = curTargetShip;

                if (targetShip != null)
                    FrameworkCore.PlayCue(sounds.click.beep);
            }

            if (targetShipTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(400).TotalMilliseconds);
                targetShipTransition = MathHelper.Clamp(targetShipTransition + delta, 0, 1);
            }
        }

        Collideable targetShip;
        float targetShipTransition;

        private void UpdateAimMode(GameTime gameTime, List<Collideable> ships)
        {
            try
            {
                UpdateConfirmButtons();

                if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed ||
                    (confirmButtonHover == 0 && inputManager.mouseLeftClick))
                {
                    ChangeToPrevMode(gameTime, ModeName.AimMode);
                    FrameworkCore.PlayCue(sounds.click.back);
                }



                if (inputManager.camNextTarget || inputManager.MouseMiddleClick)
                {
                    WarpFacingCamera(gameTime);
                }

                if (inputManager.buttonAPressed || (inputManager.mouseLeftClick && confirmButtonHover < 0))
                {
                    if (targetShip == null)
                    {
                        //error bloop sound.
                        //FrameworkCore.PlayCue(sounds.click.back);
                    }
                    else
                    {
                        selectedShip.SetTargetShip(targetShip);
                        ChangeToNextMode(gameTime, ModeName.AimMode);
                        return;
                    }
                }
                else if (inputManager.buttonXPressed ||
                    inputManager.kbFPressed ||
                    (confirmButtonHover == 1 && inputManager.mouseLeftClick))
                {
                    selectedShip.SetTargetShip(null);
                    ChangeToNextMode(gameTime, ModeName.AimMode);
                    return;
                }

                UpdateTargetShip(gameTime, ships);
            }
            catch
            {
            }
        }

        private void ClearSelectedShip()
        {
            selectedShip = null;
        }

        private void UpdateOrientationControls(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (inputManager.buttonDownHeld && curOrientationTransition <= 0)
            {
                float moveStep = MathHelper.Lerp(-0.0001f, -0.1f, dpadDownTimer);
                orientationTransition = moveStep;
            }

            if (inputManager.buttonUpHeld && curOrientationTransition <= 0)
            {
                float moveStep = MathHelper.Lerp(0.0001f, 0.1f, dpadUpTimer);
                orientationTransition = moveStep;
            }

            if (!inputManager.buttonUpHeld && !inputManager.buttonDownHeld)
            {
                orientationTransition = 0;
            }

            if ((inputManager.buttonUpHeld && inputManager.buttonDownHeld)
#if WINDOWS
                ||
                inputManager.kbResetOrientation
#endif
                )
            {
                orientationTransition = 0;

                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                        TimeSpan.FromMilliseconds(orientationResetTime).TotalMilliseconds);
                curOrientationTransition = MathHelper.Clamp(curOrientationTransition + delta, 0, 1);

                if (curOrientationTransition >= 1)
                {
                    orientationMatrix = Matrix.Lerp(orientationMatrix, Matrix.Identity, 0.2f);
                }
            }
            else if (curOrientationTransition > 0)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                        TimeSpan.FromMilliseconds(100).TotalMilliseconds);
                curOrientationTransition = MathHelper.Clamp(curOrientationTransition - delta, 0, 1);
                orientationTransition = 0;
            }

#if WINDOWS
            if (inputManager.mouseLeftHeld && confirmButtonHover < 0)
            {
                if (inputManager.mouseLeftStartHold)
                {
                    lastMousePos = inputManager.mousePos;
                }

                orientationTransition += (lastMousePos.Y - inputManager.mousePos.Y) / (4096 * (float)gameTime.ElapsedGameTime.TotalSeconds);

                lastMousePos = inputManager.mousePos;
            }
#endif

        }

        float dpadUpTimer;
        float dpadDownTimer;

        private SpaceShip UpdateHover(List<Collideable> ships)
        {
            if (ShouldHandleExpBar() && expBarHover)
                return null;


            Matrix playerOrientation = Matrix.CreateFromQuaternion(Rotation);
            //Ray cursorRay = new Ray(position, playerOrientation.Forward);


            Vector2 adjustedCursorPos = cursorPos;

            //splitscreen.
            //if (FrameworkCore.players.Count > 1)
            //    cursorPos.X *= 2;

            Ray cursorRay = Helpers.CalculateCursorRay(cursorPos,
                lockCamera.Projection, lockCamera.View);



            List<SpaceShip> shipCandidates = new List<SpaceShip>();

            for (int x = 0; x < ships.Count; x++)
            {
                if (!Helpers.IsSpaceship(ships[x]))
                    continue;

                SpaceShip ship = (SpaceShip)ships[x];

                if (ship.IsDestroyed)
                    continue;

                //skip if ship is not in camera frustrum.
                if (!ship.IsVisible(lockCamera))
                    continue;

                if (ship.Shipdata.selectionSphere > 0)
                {
                    BoundingSphere adjustedSphere = new BoundingSphere(ship.CollisionSpheres[0].sphere.Center,
                        ship.CollisionSpheres[0].sphere.Radius * ship.Shipdata.selectionSphere);

                    if (cursorRay.Intersects(adjustedSphere).HasValue)
                    {
                        shipCandidates.Add(ship);
                    }
                }
                else
                {
                    //check every collisionsphere belonging to this ship.
                    for (int i = 0; i < ship.CollisionSpheres.Length; i++)
                    {
                        if (cursorRay.Intersects(ship.CollisionSpheres[i].sphere).HasValue)
                        {
                            shipCandidates.Add(ship);

                            //break out of this ship's collisionsphere check loop.
                            i = ship.CollisionSpheres.Length + 1;
                        }
                    }
                }
            }


            SpaceShip touchedShip = null;

            //first, do a check to see if cursor is intersecting any ship mesh.
            float closestIntersection = float.MaxValue;

            // Loop over all our models.
            for (int i = 0; i < shipCandidates.Count; i++)
            {
                bool insideBoundingSphere;

                SpaceShip ship = shipCandidates[i];
                Vector3 vertex1, vertex2, vertex3;

                Matrix shipMatrix = Matrix.Identity;
                shipMatrix = Matrix.CreateFromQuaternion(ship.Rotation);
                shipMatrix.Translation = ship.Position;

                // Perform the ray to model intersection test.
                float? intersection = Helpers.RayIntersectsModel(cursorRay, FrameworkCore.ModelArray[(int)ship.modelMesh],
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

                        touchedShip = ship;
                    }
                }
            }

            if (touchedShip != null)
                return touchedShip;



            SpaceShip closestShip = null;
            float closestDistance = 9999;

            Vector2 screenCenter = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);

#if WINDOWS
            if (mouseEnabled)
            {
                screenCenter = cursorPos;
            }
#endif

            for (int x = 0; x < shipCandidates.Count; x++)
            {
                float curDist = Vector2.Distance(screenCenter,
                    Helpers.GetScreenPos(lockCamera, shipCandidates[x].Position));

                if (curDist < closestDistance)
                {
                    closestDistance = curDist;
                    closestShip = shipCandidates[x];
                }
            }
            
            //cursor not hovering over anything. null out the hovership parameter.
            return closestShip;
        }

        //only check enemy ships.
        private Collideable UpdateEnemyHover(List<Collideable> ships)
        {
            Matrix playerOrientation = Matrix.CreateFromQuaternion(Rotation);
            //Ray cursorRay = new Ray(position, playerOrientation.Forward);

            Ray cursorRay = Helpers.CalculateCursorRay(cursorPos,
                lockCamera.Projection, lockCamera.View);

            foreach (Collideable ship in ships)
            {
                if (ship.IsDestroyed)
                    continue;

                if (!Helpers.IsSpaceship(ship))
                    continue;

                //skip if ship is not in camera frustrum.
                if (!ship.IsVisible(lockCamera))
                    continue;

                if (ship.owner == this)
                    continue;



                if (((SpaceShip)ship).Shipdata.selectionSphere > 0)
                {
                    //ship only has one collisionsphere. balloon the size of the one sphere.
                    BoundingSphere adjustedSphere = new BoundingSphere(ship.CollisionSpheres[0].sphere.Center,
                        ship.CollisionSpheres[0].sphere.Radius * ((SpaceShip)ship).Shipdata.selectionSphere);

                    if (cursorRay.Intersects(adjustedSphere).HasValue)
                    {
                        return ship;
                    }
                }
                else
                {
                    //check every collisionsphere belonging to this ship.
                    for (int i = 0; i < ship.CollisionSpheres.Length; i++)
                    {
                        if (cursorRay.Intersects(ship.CollisionSpheres[i].sphere).HasValue)
                        {
                            return ship;
                        }
                    }
                }
            }


            //cursor not hovering over anything. null out the hovership parameter.
            return null;
        }






        private Vector3 GetPickedPosition(Vector2 mousePosition, float nearClip, Matrix projectionMatrix, Matrix viewMatrix, float planeHeight, Vector3 planeVector)
        {
            // create 2 positions in screenspace using the cursor position. 0 is as  
            // close as possible to the camera, 10 is as far away as possible  
            Vector3 nearSource = new Vector3(mousePosition, 0f);
            Vector3 farSource = new Vector3(mousePosition, nearClip);

            // find the two screen space positions in world space  
            Vector3 nearPoint = FrameworkCore.Graphics.GraphicsDevice.Viewport.Unproject(nearSource,
                                                                                         lockCamera.Projection,
                                                                                         lockCamera.View,
                                                                                         Matrix.Identity);

            Vector3 farPoint = FrameworkCore.Graphics.GraphicsDevice.Viewport.Unproject(farSource,
                                                                                         lockCamera.Projection,
                                                                                         lockCamera.View,
                                                                                        Matrix.Identity);

            // normalized direction vector from nearPoint to farPoint  
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // create a ray using nearPoint as the source  
            Ray r = new Ray(nearPoint, direction);

            // calculate the ray-plane intersection point  
            
            Plane p = new Plane(planeVector, planeHeight);
            

            // calculate distance of intersection point from r.origin  
            float denominator = Vector3.Dot(p.Normal, r.Direction);
            float numerator = Vector3.Dot(p.Normal, r.Position) + p.D;
            float t = -(numerator / denominator);

            // calculate the picked position on the y = 0 plane  
            Vector3 pickedPosition = nearPoint + direction * t;

            pickedPosition.X = MathHelper.Clamp(pickedPosition.X, -Helpers.GRIDSIZE + 8, Helpers.GRIDSIZE-8);
            pickedPosition.Z = MathHelper.Clamp(pickedPosition.Z, -Helpers.GRIDSIZE + 8, Helpers.GRIDSIZE-8);

            return pickedPosition;
        }

        private bool CommandMenuAvailable()
        {
            if (currentMode.modeName == ModeName.Default && !isReady && activeMenu == null && selectedShip == null)
            {
                return true;
            }

            return false;
        }

        private float turboTransition = 0;

        private float UpdateTurbo(GameTime gameTime, float baseSpeed)
        {
            if (inputManager.turboHeld)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                        TimeSpan.FromMilliseconds(800).TotalMilliseconds);

                turboTransition = MathHelper.Clamp(turboTransition + delta, 0, 1);
            }
            else
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                        TimeSpan.FromMilliseconds(200).TotalMilliseconds);

                turboTransition = MathHelper.Clamp(turboTransition - delta, 0, 1);
            }

            float turboSpeed = MathHelper.Lerp(1, 12, turboTransition);
            baseSpeed *= turboSpeed;
            return baseSpeed;
        }

        const float deadZone = 0.2f;

        //controls the mousewheel raise/lower cam.
        float mouseClimbTimer = 1;
        bool mouseClimbUp = true;

        private float smoothSpeed;
        private float smoothStrafe;

        private void UpdateCameraControls(GameTime gameTime)
        {
            if (activeMenu != null && activeMenu == commandMenu)
                return;

#if DEBUG
            //make the camera shoot missiles.
            if (this.inputmanager.buttonXPressed || this.inputmanager.mouseWheelPressed)
            {
                SpaceShip s = new SpaceShip(FrameworkCore.Game);
                s.Rotation = this.Rotation;
                FrameworkCore.Bolts.FireBolt(ProjectileTypes.PrjTorpedo, rotationMatrix.Forward * 128,
                    24, s, Vector2.Zero, 0, 0, 0, 0, this.position, this.position + (rotationMatrix.Forward * 128));
            }
#endif

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            float maxEngineSpeed = 60.0f;

            
            float ClimbOffset = 0;



            maxEngineSpeed = UpdateTurbo(gameTime, maxEngineSpeed);

#if WINDOWS
            if (mouseEnabled)
                maxEngineSpeed *= cameraSpeedMultiplier;

            if (inputmanager.kbSpaceHeld && FrameworkCore.level.gamemode == GameMode.Action)
            {
                maxEngineSpeed *= 0.08f;
            }

            if (inputManager.kbSlowCam)
            {
                maxEngineSpeed *= 0.1f;
            }
#endif
            
            #region Rotation
            Rotation = UpdateCameraRotation(dt);

            #endregion

            

            #region Velocity
            if (Math.Abs(inputManager.stickLeft.Y) > deadZone)
            {
                smoothSpeed = inputManager.stickLeft.Y;
            }
            else if (smoothSpeed != 0)
            {
                smoothSpeed = MathHelper.Lerp(smoothSpeed, 0,
                    12 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (Math.Abs(inputManager.stickLeft.X) > deadZone)
            {
                smoothStrafe = inputManager.stickLeft.X;
            }
            else if (smoothStrafe != 0)
            {
                smoothStrafe = MathHelper.Lerp(smoothStrafe, 0,
                    12 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }


            engineSpeed = smoothSpeed * maxEngineSpeed;
            strafeSpeed = smoothStrafe * maxEngineSpeed;

            if (inputManager.cameraRaise > deadZone && inputManager.cameraLower > deadZone)
            {
                engineSpeed *= 0.2f;
                strafeSpeed *= 0.2f;
            }

            Matrix m = Matrix.CreateFromQuaternion(Rotation);
            Vector3 forwardForce = m.Forward * engineSpeed;
            Vector3 strafeForce = m.Right * strafeSpeed;

            position += (forwardForce + strafeForce) * dt;
            #endregion


            #region Climbing
            float triggerDeadZone = 0.1f;
            if (inputManager.cameraRaise > triggerDeadZone && inputManager.cameraLower > triggerDeadZone)
            {
                //both triggers are pressed.
                if (inputManager.cameraRaise > 0.9f && inputManager.cameraLower > 0.9f)
                {
                }
                else if (inputManager.cameraRaise > inputManager.cameraLower)
                    ClimbOffset = inputManager.cameraRaise * 0.2f;
                else
                    ClimbOffset = inputManager.cameraLower * -0.2f;
            }
            else if (inputManager.cameraRaise > triggerDeadZone)
                ClimbOffset = inputManager.cameraRaise;
            else if (inputManager.cameraLower > triggerDeadZone)
                ClimbOffset = inputManager.cameraLower * -1.0f;




            if (mouseEnabled)
            {
                float pushOffset = 0;

                if (inputManager.mouseWheelUp)
                {
                    mouseClimbTimer = 0;
                    mouseClimbUp = true;
                }
                else if (inputManager.mouseWheelDown)
                {
                    mouseClimbTimer = 0;
                    mouseClimbUp = false;
                }

                if (mouseClimbUp && mouseClimbTimer < 1) //raise
                {
                    if (FrameworkCore.options.mousewheel == 0)
                    {
                        ClimbOffset = MathHelper.Lerp(2, 0, mouseClimbTimer);
                    }
                    else
                    {
                        pushOffset = MathHelper.Lerp(5, 0, mouseClimbTimer);
                    }
                }
                else if (!mouseClimbUp && mouseClimbTimer < 1) //lower
                {
                    if (FrameworkCore.options.mousewheel == 0)
                    {
                        ClimbOffset = MathHelper.Lerp(-2, 0, mouseClimbTimer);
                    }
                    else
                    {
                        pushOffset = MathHelper.Lerp(-5, 0, mouseClimbTimer);
                    }
                }

                if (inputManager.kbCameraRaise)
                {
                    ClimbOffset = 1;
                }
                else if (inputManager.kbCameraLower)
                {
                    ClimbOffset = -1;
                }

                if (mouseClimbTimer < 1)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                    TimeSpan.FromMilliseconds(150).TotalMilliseconds);
                    mouseClimbTimer = MathHelper.Clamp(mouseClimbTimer + delta, 0, 1);
                }

                if (Math.Abs(pushOffset) > 0)
                {
                    position += m.Forward * pushOffset * maxEngineSpeed * dt;
                }
            }

            if (Math.Abs(ClimbOffset) > 0)
            {
                //position.Y += ClimbOffset * maxEngineSpeed * dt;
                position += m.Up * ClimbOffset * maxEngineSpeed * dt;
            }
            #endregion
        }

        public void UpdateRotation(Matrix rot)
        {
            this.rotationMatrix = rot;
            this.Rotation = Quaternion.CreateFromRotationMatrix(rot);
        }

        private float adjustedRotation(float baseSensitivity)
        {
            if (FrameworkCore.options.sensitivity == 5)
                return baseSensitivity;

            if (FrameworkCore.options.sensitivity > 5)
            {
                int adjustedFactor = FrameworkCore.options.sensitivity - 5;
                return baseSensitivity + (adjustedFactor * 0.3f);
            }
            else
            {
                int adjustedFactor = (int)MathHelper.Clamp(FrameworkCore.options.sensitivity, 0, 4);
                adjustedFactor = 4 - adjustedFactor;

                return MathHelper.Max(baseSensitivity - (adjustedFactor * 0.3f), 0.1f);
            }
        }

        private Quaternion UpdateCameraRotation(float dt)
        {
            float rotationSpeed = 1.5f;

            if (FrameworkCore.HideHud)
            {
                if (inputManager.kbSpaceHeld)
                    rotationSpeed = 0.4f;
            }

#if WINDOWS
            if (mouseEnabled)
            {
                rotationSpeed = adjustedRotation(rotationSpeed);
            }
#endif

            if (!mouseEnabled)
            {
                if (targetShip != null && currentMode != null &&
                    (currentMode.modeName == ModeName.AimMode || currentMode.modeName == ModeName.FacingMode))
                {
                    //how much to sticky the crosshair.
                    rotationSpeed = 0.5f;
                }

                if (hoverShip != null && hoverShip.owner == this && currentMode.modeName == ModeName.Default)
                {
                    rotationSpeed = 0.8f;
                }
            }

            if (inputManager.cameraRaise > 0.1f && inputManager.cameraLower > 0.1f)
            {
                rotationSpeed = 0.3f;
            }

            if ((Math.Abs(inputManager.stickRight.X) > deadZone) || (inputManager.mouseCameraMode))
            {
                float turnspeed = inputManager.stickRight.X * -rotationSpeed * dt;
                float mousespeed = inputManager.MouseDifference.X * -rotationSpeed * dt;

                float finalSpeed = turnspeed;

#if WINDOWS
                if (mouseEnabled && inputManager.mouseHasMoved)
                    finalSpeed = mousespeed;
#endif

                //OPTIONS: invert X axis.
                if ((FrameworkCore.players.IndexOf(this) == 0 && FrameworkCore.options.p1InvertX) ||
                    (FrameworkCore.players.IndexOf(this) == 1 && FrameworkCore.options.p2InvertX))
                    finalSpeed *= -1.0f;


                rotationMatrix = rotationMatrix * Matrix.CreateFromAxisAngle(Vector3.Up, finalSpeed);
            }

            if ((Math.Abs(inputManager.stickRight.Y) > deadZone) || (inputManager.mouseCameraMode))
            {
                //do a gimbal lock check.

                float turnspeed = inputManager.stickRight.Y * rotationSpeed * dt;
                float mousespeed = inputManager.MouseDifference.Y * rotationSpeed * dt;

                float finalSpeed = turnspeed;
#if WINDOWS
                if (mouseEnabled && inputManager.mouseHasMoved)
                    finalSpeed = mousespeed;
#endif

                //OPTIONS: invert X axis.
                if ((FrameworkCore.players.IndexOf(this) == 0 && FrameworkCore.options.p1InvertY) ||
                    (FrameworkCore.players.IndexOf(this) == 1 && FrameworkCore.options.p2InvertY))
                    finalSpeed *= -1.0f;

                Matrix tempMatrix = rotationMatrix * Matrix.CreateFromAxisAngle(rotationMatrix.Right, finalSpeed);

                if (tempMatrix.Up.Y > 0.01f)
                    rotationMatrix = tempMatrix;
            }

            
            return Quaternion.CreateFromRotationMatrix(rotationMatrix);
        }

        public void DrawPlaybackShips(GameTime gameTime)
        {
            playbackMenu.DrawShips(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (FrameworkCore.level.isDemo)
                return;

            DrawSwitchText();

            DrawSecondsLabel(gameTime);

            DrawShipLabel(gameTime);

            if (currentMode.modeName == ModeName.Default)
            {
                DrawCrosshair();
            }
            else
            {
                DrawMoveLegends(gameTime);
            }

            if (currentMode.modeName == ModeName.AimMode || currentMode.modeName == ModeName.ModifyPriorityTarget ||
                currentMode.modeName == ModeName.FacingMode)
            {
                DrawCrosshair();
            }

            if (!IsPlaybackMode() && !isReady && FrameworkCore.level.gamemode == GameMode.Orders)
                ingameScoreboard.Draw(gameTime);

            CommandMenuDraw(gameTime);

            playbackMenu.Draw(gameTime);

            shipMenu.Draw(gameTime);

            playerMenus.Draw(gameTime);

            DrawReady();

#if WINDOWS
            if (mouseEnabled && cameraDisplayTimer < 1)
            {
                Vector2 cameraStringPos = new Vector2(120, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2);
                string cameraString = string.Format(Resource.GameCameraSpeed,
                    cameraSpeedMultiplier);
                Helpers.DrawOutline(cameraString, cameraStringPos);
            }
#endif

            DrawExpBar(gameTime);

            DrawTutorials(gameTime);

            DrawLevelTutorial(gameTime);

#if WINDOWS
            if (mouseEnabled && FrameworkCore.sysMenuManager.menus.Count <= 0 &&
                FrameworkCore.level.LevelMenuManager.menus.Count <= 0)
            {
                if (inputManager.mouseCameraMode)
                {
                    
                    //Cam Mode, center it on the screen.
                    if (FrameworkCore.level.gamemode != GameMode.Action)
                    {
                        Helpers.DrawMouseCursor(FrameworkCore.SpriteBatch,
                            Helpers.GetScreenCenter());
                    }
                    
                }
                else
                {
                    if (IsMouseDragMode() && confirmButtonHover < 0 &&
                        inputmanager.mouseLeftHeld)
                    {
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, inputmanager.mousePos, sprite.updownCursor,
                            Color.White, 0, Helpers.SpriteCenter(sprite.updownCursor), 1, SpriteEffects.None, 0);
                    }
                    else
                    {
                        Helpers.DrawMouseCursor(FrameworkCore.SpriteBatch,
                            inputmanager.mousePos);
                    }
                }
            }
#endif

        }

        int commandPoints = 2000;
        Vector2 expBarPos = new Vector2(150, 515);
        bool expBarHover = false;
        bool smitePowerActive = false;

        float expBarTransition = 0;
        float smiteAnimationTimer = 1;

        private void UpdateExpBar(GameTime gameTime)
        {
            smiteHoverTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (smiteAnimationTimer < 1)
            {
                float smiteAnimDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(300).TotalMilliseconds);
                smiteAnimationTimer = MathHelper.Clamp(smiteAnimationTimer + smiteAnimDelta, 0, 1);
            }

            

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                    TimeSpan.FromMilliseconds(200).TotalMilliseconds);

#if WINDOWS
            if (!expBarHover || !ShouldHandleExpBar())
            {
                expBarTransition = MathHelper.Clamp(expBarTransition - delta, 0, 1);
            }
            else if (expBarHover)
            {
                expBarTransition = MathHelper.Clamp(expBarTransition + delta, 0, 1);
            }
#else
            if (smiteHoverTimer <= 0)
            {
                expBarTransition = MathHelper.Clamp(expBarTransition - delta, 0, 1);
            }
            else
            {
                expBarTransition = MathHelper.Clamp(expBarTransition + delta, 0, 1);
            }
#endif



            if (!ShouldHandleExpBar())
                return;

            //input.
            if (isReady)
                return;

#if WINDOWS
            if (Vector2.Distance(expBarPos, new Vector2(inputManager.mousePos.X, inputManager.mousePos.Y)) < 40)
            {
                if (!expBarHover)
                {
                    expBarHover = true;
                    FrameworkCore.PlayCue(sounds.click.select);
                }

                if (inputManager.mouseLeftClick && CommandPointsAvailable() > 0)
                {
                    //play animation


                    ToggleSmite();
                }
            }
            else
            {
                expBarHover = false;
            }
#else

            //xbox LB
            if (inputManager.toggleSmite)
            {
                smiteHoverTimer = 3500;

                ToggleSmite();
            }
#endif
        }

        public int smiteHoverTimer = 0;

        private void ToggleSmite()
        {
            if (CommandPointsAvailable() <= 0)
            {
                if (smitePowerActive)
                    smitePowerActive = false;

                FrameworkCore.PlayCue(sounds.click.error);
                return;
            }


            if (!smitePowerActive)
            {
                smiteAnimationTimer = 0;
                FrameworkCore.PlayCue(sounds.Explosion.tiny);
            }
            else
                FrameworkCore.PlayCue(sounds.click.back);

            //toggle the Smite.
            smitePowerActive = !smitePowerActive;
        }

        public void AddExperiencePlayerDeath(int amount)
        {
            int adjusted = (int)(amount * 0.3f);
            AddExperience(adjusted);
        }

        public void ResetExperience()
        {
            commandPoints = 0;
        }

        /// <summary>
        /// Add XP points. Note XP gets diminishing returns.
        /// </summary>
        /// <param name="amount"></param>
        public void AddExperience(int amount)
        {
            int curPoints = CommandPointsAvailable();

            if (curPoints <= 0)
            {
                //get the full amount.
                commandPoints += amount;
                return;
            }
            else if (curPoints == 1)
            {
                commandPoints += (int)(amount * 0.85f);
                return;
            }
            else if (curPoints == 2)
            {
                commandPoints += (int)(amount * 0.7f);
                return;
            }
            else
            {
                commandPoints += (int)(amount * 0.5f);
            }
        }

        private bool ShouldHandleExpBar()
        {
            if (isTutorialLevel)
                return false;

            if (FrameworkCore.gameState != GameState.Play)
                return false;

            if (activeMenu != null)
                return false;

            if (currentMode.modeName != ModeName.Default)
                return false;

            if (!FrameworkCore.isCampaign)
                return false;

            if (this != FrameworkCore.players[0])
                return false;

            return true;
        }

        private int CommandPointsAvailable()
        {
            return (int)(commandPoints / 1000);            
        }

        private void DrawExpBar(GameTime gameTime)
        {
            if (!ShouldHandleExpBar())
                return;

            expBarPos.Y = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 205;
            
            if (smiteAnimationTimer < 1)
            {
                Color glowColor = Color.Gold;
                glowColor = Color.Lerp( glowColor, Helpers.transColor(glowColor), smiteAnimationTimer);

                float glowSize = MathHelper.Lerp(4, 8, smiteAnimationTimer);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                    expBarPos,
                    sprite.glow,
                    glowColor, 0,
                    Helpers.SpriteCenter(sprite.glow), glowSize,
                    SpriteEffects.None, 0);
            }


            //the glow sparkles.
            if (smitePowerActive)
            {
                Color glowColor = Color.Goldenrod;

                for (int i = 0; i < 2; i++)
                {
                    float glowSize = 1.3f + Helpers.Pulse(gameTime, 0.1f, 6);
                    float angle = (float)gameTime.TotalGameTime.TotalSeconds * 0.5f;
                    if (i >= 1)
                    {
                        glowSize *= -1f;
                        angle *= -0.4f;
                    }

                    if (expBarHover)
                        glowSize *= 1.3f;

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                        expBarPos,
                        sprite.sparkle,
                        glowColor, angle,
                        Helpers.SpriteCenter(sprite.sparkle), glowSize,
                        SpriteEffects.None, 0);
                }
            }

            int pointsAvailable = CommandPointsAvailable();
            if (expBarTransition > 0)
            {
                //hover circle.
                Color glowColor = Faction.Blue.teamColor;
                glowColor = Color.Lerp(Helpers.transColor(glowColor), glowColor, expBarTransition);
                float glowSize = 4f + Helpers.Pulse(gameTime, 0.3f, 8);
                glowSize = MathHelper.Lerp(glowSize - 0.5f, glowSize, expBarTransition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                    expBarPos,
                    sprite.glow,
                    glowColor, 0,
                    Helpers.SpriteCenter(sprite.glow), glowSize,
                    SpriteEffects.None, 0);

                //draw the text description.
                string title = smitePowerActive ? Resource.SmiteTitleActive : Resource.SmiteTitle;
                Vector2 textPos = expBarPos + new Vector2(60, -60);
                textPos.X += MathHelper.Lerp(-30, 0, expBarTransition);
                textPos.Y += MathHelper.Lerp(5, 0, expBarTransition);
                Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0,0,0,160),expBarTransition);
                Color textColor = Color.Gold;

                if (pointsAvailable <= 0)
                    textColor = new Color(160, 160, 160);

                textColor = Color.Lerp(Helpers.transColor(textColor),textColor,expBarTransition);
                Helpers.DrawOutline(FrameworkCore.Gothic, title,
                    textPos, textColor, darkColor, -0.1f, Vector2.Zero, 0.65f);

                textPos.Y += 50;
                textPos.X += 5;
                textColor = Color.Goldenrod;

                if (pointsAvailable <= 0)
                    textColor = new Color(160, 160, 160);

                textColor = Color.Lerp(Helpers.transColor(textColor), textColor, expBarTransition);

                string desc = smitePowerActive ? Resource.SmiteDescriptionActive : Resource.SmiteDescription;

                if (pointsAvailable <= 0)
                {
                    desc = Resource.SmiteDescriptionRequirements;
                }

                Helpers.DrawOutline(FrameworkCore.Serif, desc,
                    textPos, textColor, darkColor, -0.1f, Vector2.Zero, 1.05f);
            }

            //dark circle.
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                expBarPos,
                sprite.roundCircle,
                new Color(0,0,0,128), 0,
                Helpers.SpriteCenter(sprite.roundCircle), 1.2f,
                SpriteEffects.None, 0);

            

            int remainder = commandPoints - (pointsAvailable * 1000);
            if (remainder > 0)
            {
                Color barFillColor = Color.White;

                if (!expBarHover)
                    barFillColor = new Color(192,192,192);

                Rectangle fillRect = sprite.barFill;

                float angleMax = (6.2831853f * (float)remainder) / 1000.0f;

                for (float i = 0; i < angleMax; i += 0.05f)
                {
                    Vector2 fillPos = expBarPos;
                    fillPos.X += (float)(Math.Cos(i - 1.57f) * 35);
                    fillPos.Y += (float)(Math.Sin(i - 1.57f) * 35);

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                        fillPos,
                        fillRect,
                        barFillColor, i,
                        Helpers.SpriteCenter(fillRect), 1,
                        SpriteEffects.None, 0);
                }
            }

            //main circle.
            Color mainCircleColor = Faction.Blue.teamColor;

            if (pointsAvailable <= 0)
                mainCircleColor = Color.Gray;
            else if (smitePowerActive)
                mainCircleColor = Color.Goldenrod;
            else if (pointsAvailable >= 1 && !smitePowerActive)
            {
                //a point is available. glow the button.
                Color brightColor = new Color(190,220,255);

                mainCircleColor = Color.Lerp(mainCircleColor, brightColor,
                    0.5f + Helpers.Pulse(gameTime, 0.49f, 7));
            }

            if (!expBarHover)
                mainCircleColor = Color.Lerp(Color.Black, mainCircleColor, 0.7f);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                expBarPos,
                sprite.roundCircle,
                mainCircleColor, 0,
                Helpers.SpriteCenter(sprite.roundCircle), 1,
                SpriteEffects.None, 0);

            //points available
            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.SerifBig,
                "" + pointsAvailable, expBarPos + new Vector2(1,-3), Color.White, new Color(0, 0, 0, 128), 1.2f, 0);


#if XBOX
            //draw LB
            if (FrameworkCore.level.gamemode == GameMode.Orders && !isReady)
            {
                Vector2 lbPos = expBarPos;
                lbPos.Y -= 60;

                if (pointsAvailable >= 1 && !smitePowerActive)
                    lbPos.Y += Helpers.Pulse(gameTime, 4, 3.5f);

                float buttonSize = 1;
                if (smitePowerActive)
                    buttonSize = 0.8f;

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                    lbPos,
                    sprite.buttons.lb,
                    Color.White, 0,
                    Helpers.SpriteCenter(sprite.buttons.lb), buttonSize,
                    SpriteEffects.None, 0);
            }
#endif
        }




        public bool TutAllShipsHaveOrders()
        {
            bool allHaveOrders = true;

            foreach (Collideable ship in FrameworkCore.level.Ships)
            {
                if (!Helpers.IsSpaceship(ship))
                    continue;

                if (ship.owner == null)
                    continue;

                if (ship.owner != this)
                    continue;

                if (ship.IsDestroyed)
                    continue;

                if (((SpaceShip)ship).targetPos == Vector3.Zero)
                {
                    allHaveOrders = false;
                    break;
                }
            }

            return allHaveOrders;
        }

        private bool TutNoShipsHaveOrders()
        {
            bool noShipHasOrders = true;

            foreach (Collideable ship in FrameworkCore.level.Ships)
            {
                if (!Helpers.IsSpaceship(ship))
                    continue;

                if (ship.owner == null)
                    continue;

                if (ship.owner != this)
                    continue;

                if (ship.IsDestroyed)
                    continue;

                if (((SpaceShip)ship).targetPos != Vector3.Zero)
                {
                    noShipHasOrders = false;
                    break;
                }
            }

            return noShipHasOrders;
        }

        private float GetTutorialBounce(GameTime gameTime)
        {
            return Helpers.Pulse(gameTime, 6, 3);
        }


        private void DrawLevelTutorial(GameTime gameTime)
        {
            if (FrameworkCore.level.gamemode != GameMode.Orders)
            {
                if (FrameworkCore.level.gamemode == GameMode.Action &&
                    FrameworkCore.playbackSystem.WorldTimer > 7000 &&
                    FrameworkCore.playbackSystem.WorldTimer < 25000 &&
                    isTutorialLevel)
                {
                    Vector2 pos = Helpers.GetScreenPos(lockCamera, GetEnemyFleetPosition());

                    pos.Y += FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.24f;

                    DrawTutorialMessage(gameTime, Resource.TutorialDeflection, pos, 1);
                }

                return;
            }

            if (isReady)
                return;

            if (!isTutorialLevel)
                return;

            if (tutState == TutState.showPlayerFleet)
            {
                //THIS IS YOUR FLEET.
                Vector2 pos = Helpers.GetScreenPos(lockCamera, GetCenterFleet(false));

                pos.Y += FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.24f;


                

                pos.Y = MathHelper.Clamp(pos.Y, 130, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 165);
                pos.X = MathHelper.Clamp(pos.X, 0 + 100 + 256,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 256);

                pos.Y += GetTutorialBounce(gameTime);


                string txt = Resource.TutorialPlayerFleet;

                if (FrameworkCore.players.Count > 1)
                    txt = Resource.TutorialPlayerShip;

                DrawTutorialMessage(gameTime, txt, pos, 1);

                if (mouseEnabled)
                {
                    Helpers.DrawClickMessage(gameTime, 1, Resource.MenuClickToContinue,
                        pos.X);
                }
                else
                {
                    Helpers.DrawLegend(Resource.MenuContinue, sprite.buttons.a, 1);
                }
            }
            else if (tutState == TutState.showEnemyFleet && warpCameraTransition >= 1)
            {
                //THIS IS THE ENEMY.
                Vector2 pos = Helpers.GetScreenPos(lockCamera, tutLookAt);

                pos.Y += FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.24f;

                pos.Y = MathHelper.Clamp(pos.Y, 130, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 130);

                pos.X = MathHelper.Clamp(pos.X, 0 + 100 + 256,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 256);


                pos.Y += GetTutorialBounce(gameTime);


                DrawTutorialMessage(gameTime, Resource.TutorialEnemyFleet, pos, 1);

                if (mouseEnabled)
                {
                    Helpers.DrawClickMessage(gameTime, 1, Resource.MenuClickToContinue, pos.X);
                }
                else
                {
                    Helpers.DrawLegend(Resource.MenuContinue, sprite.buttons.a, 1);
                }
            }
            else if (tutState == TutState.showOrdersIntro && warpCameraTransition >= 1)
            {
                //LET'S BLOW UP THAT ENEMY
                Vector2 pos = Helpers.GetScreenPos(lockCamera, GetCenterFleet(false));

                pos.Y += FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.24f;

                pos.Y = MathHelper.Clamp(pos.Y, 130, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 130 - 40);

                pos.X = Helpers.GetScreenPos(lockCamera, tutLookAt).X;
                pos.X = MathHelper.Clamp(pos.X, 0 + 100 + 256,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 256);



                pos.Y += GetTutorialBounce(gameTime);


                DrawTutorialMessage(gameTime, Resource.TutorialOrdersIntro, pos, 1);

                if (mouseEnabled)
                {
                    Helpers.DrawClickMessage(gameTime, 1, Resource.MenuClickToContinue, pos.X);
                }
                else
                {
                    Helpers.DrawLegend(Resource.MenuContinue, sprite.buttons.a, 1);
                }
            }
            else if (tutState == TutState.doOrders && warpCameraTransition >= 1)
            {
                if (currentMode.modeName == ModeName.Default && activeMenu == null)
                {
                    //SELECT ONE OF YOUR SHIPS.
                    Vector2 pos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                        100 + 32 + 32);

                    pos.X = Helpers.GetScreenPos(lockCamera, GetCenterFleet(false)).X;
                    pos.X = MathHelper.Clamp(pos.X, 0 + 100 + 256,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 256);

                    pos.Y += GetTutorialBounce(gameTime);


                    string txt = string.Empty;

                    if (TutAllShipsHaveOrders())
                    {
                        //every ship has an order.
                        if (!isReady)
                        {
                            txt = Resource.TutorialOpenCommandMenu;
                            pos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 246,
                                250);

                            pos.Y += GetTutorialBounce(gameTime);
                        }
                    }
                    else if (TutNoShipsHaveOrders())
                    {
                        //SELECT ONE OF YOUR SHIPS.
                        txt = Resource.TutorialGiveOrderShips;
                        if (FrameworkCore.players.Count > 1)
                            txt = Resource.TutorialGiveOrderShip;
                    }
                    else
                    {
                        //GIVE ORDERS TO YOUR OTHER SHIP.
                        txt = Resource.TutorialGiveOrderOtherShip;
                    }

                    DrawTutorialMessage(gameTime, txt, pos, 1);
                }
                else if (currentMode.modeName == ModeName.Default && activeMenu == shipMenu)
                {
                    //SELECT ONE OF THE ORDERS.
                    Vector2 pos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 256,
                        (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.7f) - 120);

                    pos.Y += GetTutorialBounce(gameTime);

                    DrawTutorialMessage(gameTime, Resource.TutorialSelectOrder, pos, 1);
                }
                else if (currentMode.modeName == ModeName.MoveMode)
                {
                    if (tutLongString == null)
                    {
                        tutLongString = Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                            Resource.TutorialPlanarMove, 460, Vector2.Zero, Color.White);
                    }

                    //HOW TO MOVE SHIPS.
                    Vector2 pos = new Vector2(100 + 256 - 32,
                        FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 100 - 128 - 40);

                    

                    DrawTutorialBigMessage(tutLongString, pos, confirmButtonTransition);
                }
                else if (currentMode.modeName == ModeName.HeightMode)
                {
                    //VERTICAL MOVEMENT.
                    Vector2 pos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 100 - 256,
                        FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.70f);

                    pos.Y += GetTutorialBounce(gameTime);

                    DrawTutorialMessage(gameTime, Resource.TutorialHeightMode, pos, confirmButtonTransition);
                }
                else if (currentMode.modeName == ModeName.FacingMode)
                {
                    //FACING MODE
                    Vector2 pos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                        FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 100 - 64 - 40);

                    pos.X -= 24;
                    pos.Y += GetTutorialBounce(gameTime);

                    DrawTutorialMessage(gameTime, Resource.TutorialFacing, pos, 1);
                }
            }
        }

        private string tutLongString;

        private void DrawTutorialBigMessage(string text, Vector2 pos, float transition)
        {
            if (text == null)
                return;

            Color boxColor = new Color(255, 255, 255, 160);
            Color textColor = Color.Black;


            boxColor = Color.Lerp(Helpers.transColor(boxColor), boxColor, transition);
            textColor = Color.Lerp(Helpers.transColor(textColor), textColor, transition);


            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.inventoryBox,
                boxColor, 0, Helpers.SpriteCenter(sprite.inventoryBox), 1, SpriteEffects.None, 0);

            int textHeight = (int)FrameworkCore.Serif.MeasureString("S").Y;

            Vector2 textPos = pos;
            textPos.X -= 225;
            textPos.Y -= 105;
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, text, textPos,
                textColor);
        }

        private void DrawTutorialMessage(GameTime gameTime, string text, Vector2 pos, float transition)
        {
            float alpha = MathHelper.Lerp(0, 160, transition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.tutorial.messageBox,
                new Color(255, 255, 255, (byte)alpha), 0, Helpers.SpriteCenter(sprite.tutorial.messageBox), 1, SpriteEffects.None, 0);

            int textHeight = (int)FrameworkCore.Serif.MeasureString("S").Y;

            Color textColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, transition);

            Vector2 textPos = pos;
            textPos.X -= 233;
            textPos.Y -= textHeight / 2;
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, text, textPos,
                textColor, 0, Vector2.Zero, 0.92f, SpriteEffects.None, 0);
        }






        private void DrawSwitchText()
        {
            if (!TutAllShipsHaveOrders() && activeMenu == null && selectedShip == null &&
                !isReady)
            {
                if (isTutorialLevel && tutState < TutState.doOrders)
                {
                }
                else
                {

                    if (mouseEnabled)
                        Helpers.DrawLegend(Resource.MenuNextAvailableShip, sprite.buttons.spacebar, 1);
                    else
                        Helpers.DrawLegend(Resource.MenuNextAvailableShip, sprite.buttons.x, 1);
                }
            }

            if (!ShowHoverText())
                return;

            if (!mouseEnabled)
            {
                float x = Helpers.DrawLegendRow2(Resource.MenuSelectShip, sprite.buttons.a, hoverShip.hoverTransition);

                if (!CanSwitchShips())
                    return;

                //x -= Helpers.LEGENDGAPSIZE;

                Helpers.DrawLegendRow2Left(Resource.MenuGiveToAlly, sprite.buttons.y, hoverShip.hoverTransition);
            }
            else
            {
                if (!CanSwitchShips())
                    return;

                Color darkColor = new Color(0, 0, 0, 128);
                Vector2 switchPos = new Vector2(60, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 160);
                int GVec = (int)FrameworkCore.Serif.MeasureString("G ").X;

                Helpers.DrawOutline(FrameworkCore.Serif, "G ", switchPos, Color.Orange, darkColor);
                switchPos.X += GVec;
                Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuGiveToAlly, switchPos, Color.White, darkColor);
            }            
        }

        

        private void DrawTutorials(GameTime gameTime)
        {
            if (!isTutorialLevel)
                return;

            if (tutHasShownCameraControls && tutHasShownArmorTips)
                return;

            if (tutState != TutState.doOrders)
                return;

            /*
            if (!tutHasShownArmorTips)
            {
                DrawArmorTip(gameTime, this.selectedShip);
            }
            */

            if (!tutHasShownCameraControls && commandMenu.Transition <= 0)
            {
                if (!mouseEnabled)
                {
                    //XBOX CONTROLS
                    int startY = (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.6f);
                    DrawTutorialLine(gameTime, startY,
                        new Rectangle[]{
                        sprite.buttons.leftstick,
                        sprite.buttons.rightstick
                    },
                    Resource.TutorialMoveCamera);

                    startY += sprite.roundCircle.Height;

                    DrawTutorialLine(gameTime, startY,
                                        new Rectangle[]{
                        sprite.buttons.lefttrigger,
                        sprite.buttons.righttrigger
                    },
                    Resource.TutorialElevateCamera);
                }
                else
                {
                    //PC controls.
                    int startY = (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.6f);
                    DrawTutorialLine(gameTime, startY,
                        new Rectangle[]{
                        sprite.buttons.mouseRightClick                        
                    },
                    Resource.TutorialPCMouseCam);

                    startY += sprite.roundCircle.Height / 2;
                    startY += sprite.buttons.kbWasd.Height / 2;

                    startY += 8;

                    float iconSize = 1 + Helpers.Pulse(gameTime, 0.05f, 6);
                    Vector2 startPos = new Vector2(150, startY);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, startPos, sprite.buttons.kbWasd,
                        Color.White, 0,
                        Helpers.SpriteCenter(sprite.buttons.kbWasd), iconSize, SpriteEffects.None, 0);

                    startPos.X += sprite.buttons.kbWasd.Width / 2;
                    startPos.X += 8;

                    DrawTutorialText(gameTime, startPos, Resource.TutorialMoveCamera, true);

                    if (hoverShip != null)
                    {
                        DrawShipTip(gameTime, hoverShip);
                    }
                }
            }
        }

        private void DrawTutorialLines(GameTime gameTime)
        {
            /*

            if (tutHasShownArmorTips)
                return;

            if (selectedShip == null)
                return;

            if (!isTutorialLevel)
                return;

            if (tutState != TutState.doOrders)
                return;

            SpaceShip ship = selectedShip;

            Matrix shipMatrix = Matrix.CreateFromQuaternion(ship.Rotation);
            int arrowLength = 14;
            Vector3 arrowEndPos = ship.Position + shipMatrix.Backward * (ship.BSphere.Radius + arrowLength);
            arrowEndPos = Vector3.Lerp(arrowEndPos, arrowEndPos + shipMatrix.Backward * 2,
                Helpers.Pulse(gameTime, 0.49f, 8));
            DrawArrow(arrowLength,
                arrowEndPos,
                shipMatrix.Forward, Color.LimeGreen, 5, shipMatrix.Up);


            arrowEndPos = ship.Position + shipMatrix.Down * (ship.BSphere.Radius + arrowLength);
            arrowEndPos += shipMatrix.Backward * (ship.BSphere.Radius / 2);
            arrowEndPos = Vector3.Lerp(arrowEndPos, arrowEndPos + shipMatrix.Down * 2,
                            Helpers.Pulse(gameTime, 0.49f, 8));
            DrawArrow(arrowLength,
                arrowEndPos,
                shipMatrix.Up, Color.LimeGreen, 5, shipMatrix.Forward);
             */
        }

        private void DrawArmorTip(GameTime gameTime, Collideable ship)
        {
            if (ship == null)
                return;

            //draw the tip text.
            Vector2 shipPos = Helpers.GetScreenPos(lockCamera, ship.Position);
            shipPos.X += Helpers.SizeInPixels(lockCamera, ship.Position, ship.BSphere.Radius) / 2;
            shipPos.X += 12;
            /*
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, shipPos, sprite.arrow,
                Color.LimeGreen, 0,
                new Vector2(6, 16), 1, SpriteEffects.None, 0);
            */
            //shipPos.X += sprite.arrow.Width;

            shipPos.Y -= 36;
            DrawTutorialText(gameTime, shipPos, Resource.TutorialArmorTip, false);
        }

        private Collideable GetAnyEnemyShip()
        {
            foreach (Collideable ship in FrameworkCore.level.Ships)
            {
                if (ship.owner == null)
                    continue;

                if (ship.owner.factionName == this.factionName)
                    continue;

                if (ship.IsDestroyed)
                    continue;

                return ship;
            }

            return null;
        }

        private void DrawShipTip(GameTime gameTime, Collideable ship)
        {
            if (inputManager.mouseCameraMode)
                return;

            if (ship.owner == null)
                return;

            if (ship.owner != this)
                return;

            Vector2 shipPos = Helpers.GetScreenPos(lockCamera, ship.Position);
            shipPos.Y -= Helpers.SizeInPixels(lockCamera, ship.Position, ship.BSphere.Radius) / 2;
            shipPos.Y -= 12;

            Vector2 arrowPos = shipPos;
            arrowPos.Y += Helpers.Pulse(gameTime, 3, 6);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, arrowPos, sprite.arrow,
                Color.LimeGreen, -1.57f,
                new Vector2(6,16), 1, SpriteEffects.None, 0);

            shipPos.Y -= sprite.arrow.Width * 0.7f;

            shipPos.Y -= sprite.roundCircle.Height / 2;

            float circleSize = 1 + Helpers.Pulse(gameTime, 0.04f, 6);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, shipPos, sprite.roundCircle,
                Color.LimeGreen, 0,
                Helpers.SpriteCenter(sprite.roundCircle), circleSize, SpriteEffects.None, 0);

            float iconSize = 1 + Helpers.Pulse(gameTime, 0.1f, 6);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, shipPos, sprite.buttons.mouseLeftClick,
                Color.White, 0,
                Helpers.SpriteCenter(sprite.buttons.mouseLeftClick), iconSize, SpriteEffects.None, 0);
        }

        private void DrawTutorialLine(GameTime gameTime, int startY, Rectangle[] images, string text)
        {
            Vector2 startPos = new Vector2(150, startY);

            for (int i = 0; i < images.Length; i++)
            {
                float circleSize = 1 + Helpers.Pulse(gameTime, 0.04f, 6);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, startPos, sprite.roundCircle,
                    Color.LimeGreen, 0,
                    Helpers.SpriteCenter(sprite.roundCircle), circleSize, SpriteEffects.None, 0);

                float iconSize = 1 + Helpers.Pulse(gameTime, 0.1f, 6);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, startPos, images[i],
                    Color.White, 0,
                    Helpers.SpriteCenter(images[i]), iconSize, SpriteEffects.None, 0);
                
                startPos.X += sprite.roundCircle.Width;
            }

            Vector2 textPos = new Vector2(150, startY);
            textPos.X += images.Length * sprite.roundCircle.Width;
            textPos.X -= sprite.roundCircle.Width * 0.5f;
            textPos.X += 8;

            int textHeight = (int)FrameworkCore.Serif.MeasureString("S").Y;
            textPos.Y -= textHeight / 2;

            DrawTutorialText(gameTime, textPos, text, true);
        }

        private void DrawTutorialText(GameTime gameTime, Vector2 textPos, string text, bool bounce)
        {
            if (bounce)
                textPos.X += Helpers.Pulse(gameTime, 3, 6);

            Helpers.DrawOutline(FrameworkCore.Serif, text, textPos, Color.Lime, Color.Black, -0.1f,
                Vector2.Zero, 1);
        }

        private void DrawReady()
        {
            if (!isReady)
                return;

            if (FrameworkCore.players.Count <= 1)
                return;

            if (currentMode.modeName != ModeName.Default)
                return;

            if (FrameworkCore.level.gamemode != GameMode.Orders)
                return;

            float angle = -0.2f;            

            Vector2 pos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 4f);

            Vector2 vec = FrameworkCore.Gothic.MeasureString(Resource.MenuReady);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, Resource.MenuReady, pos+new Vector2(2,2),
                            Color.Black, angle, new Vector2(vec.X / 2, vec.Y / 2), 1, SpriteEffects.None, 0);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, Resource.MenuReady, pos,
                Color.White, angle, new Vector2(vec.X / 2, vec.Y / 2), 1, SpriteEffects.None, 0);

            string otherPlayerName;

            if (this == FrameworkCore.players[0])
                otherPlayerName = FrameworkCore.players[1].commanderName;
            else
                otherPlayerName = FrameworkCore.players[0].commanderName;

            string waitingLine = string.Format(Resource.MenuReadyWaitingFor, otherPlayerName);
            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif, waitingLine,
                pos + new Vector2(0, vec.Y / 2), Color.White, Color.Black, 1, angle);

            if (mouseEnabled)
            {
                readyButtonPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 100);

                Rectangle buttonRect = new Rectangle(
                                    (int)readyButtonPos.X - 140,
                                    (int)readyButtonPos.Y - 24,
                                    280, 48);

                Color buttonColor = Color.Lerp(Color.Black, TeamColor, 0.5f);
                Color txtColor = Color.White;

                if (readyButtonHover)
                {
                    buttonColor = TeamColor;
                    txtColor = Color.Orange;
                }

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonRect, sprite.vistaBox,
                    buttonColor);

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, Resource.MenuCancel,
                    readyButtonPos, txtColor, 1);
            }
            else
            {
                Helpers.DrawLegend(Resource.MenuCancel, sprite.buttons.b, 1);
            }
        }

        Vector2 readyButtonPos;
        bool readyButtonHover;



        private void DrawShipLabel(GameTime gameTime)
        {
            if (hoverShip == null)
                return;

            SpaceShip ship = hoverShip;

            Vector2 shipPos = Helpers.GetScreenPos(lockCamera, ship.Position);

            //draw the veterancy stars.
            Color labelColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, hoverShip.hoverTransition);
            
            shipPos.Y += Helpers.SizeInPixels(lockCamera, ship.Position, ship.BSphere.Radius) / 2;
            shipPos.Y += 12;
            Helpers.DrawVeterancy(shipPos, ship.fleetShipInfo.veterancy, labelColor);
            shipPos.Y += 4;
            
            Color labelBG = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, hoverShip.hoverTransition);

            Vector2 nameVec = FrameworkCore.Serif.MeasureString(hoverShip.CaptainName);

            float size = Helpers.PopLerp(ship.hoverTransition, 0, 1.3f, 1);

            //labelColor = Color.Lerp(Color.TransparentWhite, Color.White, hoverShip.hoverTransition);
            if (ship.owner.factionName != this.factionName)
            {
                size = 1;
                labelColor = ship.owner.ShipColor;
                labelBG = Color.Black;
            }

            Helpers.DrawOutline(FrameworkCore.Serif, hoverShip.CaptainName, shipPos, labelColor, labelBG, 0,
                new Vector2(nameVec.X / 2, 0), size);

            try
            {
                //draw the ship type if I don't own teh ship
                //if (hoverShip.owner != this)
                {
                    string shipType = hoverShip.shipName;
                    Vector2 shipNameVec = FrameworkCore.Serif.MeasureString(shipType);

                    Color shipTypeColor = Color.Lerp(Color.Black, labelColor, 0.8f);

                    Vector2 shipTypePos = shipPos;
                    shipTypePos.Y += shipNameVec.Y;

                    Helpers.DrawOutline(FrameworkCore.Serif, shipType, shipTypePos,
                        shipTypeColor, labelBG, 0,
                        new Vector2(shipNameVec.X / 2, 0), size);                    
                }
            }
            catch
            {
            }

            if (ship.fleetShipInfo == null)
                return;

            if (ship.fleetShipInfo.upgradeArray == null)
                return;

            shipPos.Y += nameVec.Y + 5;


            shipPos.Y += 20;

            //draw upgrades underneath teh ship.
            for (int i = 0; i < ship.fleetShipInfo.upgradeArray.Length; i++)
            {
                if (ship.fleetShipInfo.upgradeArray[i] == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect == null)
                    continue;

                Vector2 descVec = FrameworkCore.Serif.MeasureString(ship.fleetShipInfo.upgradeArray[i].description);

                Rectangle destRect = new Rectangle(
                    (int)(shipPos.X - descVec.X/2),
                    (int)(shipPos.Y+2),
                    (int)(descVec.X),
                    (int)(descVec.Y));
                destRect.Inflate(5, 0);
                destRect.Height = (int)Helpers.PopLerp(hoverShip.hoverTransition, 0, 
                    destRect.Height * 1.5f,
                    destRect.Height);
                    //(int)MathHelper.Lerp(0, destRect.Height, hoverShip.hoverTransition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, destRect, sprite.tabRectangle, labelBG);


                Helpers.DrawOutline(FrameworkCore.Serif, ship.fleetShipInfo.upgradeArray[i].description,
                    shipPos, labelColor, labelBG, 0, new Vector2(descVec.X / 2, 0), size);

                shipPos.Y += nameVec.Y + 5;
            }


        }

        Vector2[] confirmButtonPositions;

        private void DrawConfirmButton()
        {
#if XBOX
            return;
#endif


            if (confirmButtonPositions == null)
                return;

            if (confirmButtonTransition <= 0)
                return;

            UpdateConfirmButtonPositions();

            for (int x = 0; x < confirmButtonPositions.Length; x++)
            {
                if (currentMode != null)
                {
                    if (currentMode.modeName != ModeName.FacingMode &&
                        currentMode.modeName != ModeName.YawMode && x > 1)
                    {
                        continue;
                    }

                    if (x == 1 && currentMode.modeName == ModeName.FacingMode)
                        continue;
                }
                

                Rectangle buttonRect = new Rectangle(
                    (int)confirmButtonPositions[x].X - 140,
                    (int)confirmButtonPositions[x].Y - 24,
                    280, 48);

                Color buttonColor = Color.Lerp(Color.Black, TeamColor, 0.5f);

                if (confirmButtonHover == x)
                    buttonColor = TeamColor;

                buttonColor = Color.Lerp(Helpers.transColor(buttonColor), buttonColor, confirmButtonTransition);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonRect, sprite.vistaBox,
                    buttonColor);

                string text = Resource.MenuCancel;

#if WINDOWS
                if (mouseEnabled)
                {
                    text += " " + Helpers.GetShortcutCancel();
                }
#endif

                if (x == 1)
                {
                    if (currentMode.modeName == ModeName.AimMode)
                    {
                        text = Resource.MenuConfirmPriorityTargetSkip;

#if WINDOWS
                        if (mouseEnabled)
                        {
                            text += " " + Helpers.GetShortcutAltKey();
                        }
#endif
                    }
                    else
                        text = Resource.MenuConfirm + " " + Resource.MenuSpaceBar;
                }
                else if (x > 1)
                {
                    if (advancedMode)
                        text = Resource.MenuAdvancedMoveOn;
                    else
                        text = Resource.MenuAdvancedMoveOff;

#if WINDOWS
                    if (mouseEnabled)
                    {
                        text += " " + Helpers.GetShortcutAltKey();
                    }
#endif
                }



                Color textColor = Color.White;

                if (confirmButtonHover == x)
                    textColor = Color.Orange;

                textColor = Color.Lerp(Helpers.transColor(textColor), textColor, confirmButtonTransition);

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                    text, confirmButtonPositions[x], textColor, 0.9f);
            }
        }


        /// <summary>
        /// Draw the little hourglass and estimated seconds.
        /// </summary>
        /// <param name="gameTime"></param>
        private void DrawSecondsLabel(GameTime gameTime)
        {
            if (hoverShip == null && selectedShip == null)
                return;

            if (currentMode.modeName == ModeName.Default || currentMode.modeName == ModeName.MoveMode ||
                currentMode.modeName == ModeName.HeightMode)
            { }
            else
                return;

            Color labelColor = Color.White;
            Color labelBG = Color.Black;
            Vector2 labelPosModifier = Vector2.Zero;

            for (int x = 0; x < FrameworkCore.level.Ships.Count; x++)
            {
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[x]))
                    continue;

                SpaceShip ship = (SpaceShip)FrameworkCore.level.Ships[x];

                if ((hoverShip != null && hoverShip == ship) ||
                    (selectedShip != null && selectedShip == ship))
                {
                }
                else
                    continue;


                if (ship.IsDestroyed)
                    continue;

                if (ship.owner.factionName != this.factionName)
                    continue;

                if (ship.markTimeLength <= 0)
                    continue;

                Vector3 destSpot = Vector3.Zero;

                if (selectedShip != null && selectedShip == ship && currentMode.modeName != ModeName.Default)
                    destSpot = currentMovePos;
                else if (ship.targetPos != Vector3.Zero)
                    destSpot = ship.targetPos;
                else
                    continue;

                Vector2 shipPos = Helpers.GetScreenPos(lockCamera, destSpot);
                if (hoverShip != null && hoverShip == ship)
                {
                    labelPosModifier = Helpers.PopLerp(ship.hoverTransition,
                        new Vector2(-20, -20),
                        new Vector2(10, 10),
                        Vector2.Zero);
                }
                shipPos += labelPosModifier;

                shipPos.Y += Helpers.SizeInPixels(lockCamera, ship.Position, ship.BSphere.Radius) / 2;
                shipPos.X += Helpers.SizeInPixels(lockCamera, ship.Position, ship.BSphere.Radius) / 2;



                DrawOffscreenTime(shipPos, ship);                
            }
        }


        private void DrawOffscreenTime(Vector2 shipPos, SpaceShip ship)
        {
            if (ship == null)
                return;

            Color labelColor = Color.White;
            Color labelBG = Color.Black;


            int safeScreen = 100;

            shipPos.X = MathHelper.Clamp(shipPos.X,
                safeScreen, FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - safeScreen);

            shipPos.Y = MathHelper.Clamp(shipPos.Y,
                            safeScreen, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - safeScreen);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, shipPos, sprite.tinyHourglass, labelColor, 0.0f,
                    Vector2.Zero, 1f, SpriteEffects.None, 0);

            string travelTime = ship.markTimeLength.ToString("N1", CultureInfo.InvariantCulture);
            Helpers.DrawOutline(FrameworkCore.Serif, travelTime, shipPos + new Vector2(27, 5),
                labelColor, labelBG);
        }



        private void DrawAimModeSprites(GameTime gameTime)
        {
            /*
            if (targetShip == null)
                return;

            Vector2 screenPos = Helpers.GetScreenPos(lockCamera, targetShip.Position);

            float distance = (targetShip.Position - lockCamera.CameraPosition).Length();
            float radius = targetShip.BSphere.Radius;
            float sizeInPixels = 10;
            if (distance > radius)
            {
                float angularSize = (float)Math.Tan(radius / distance);
                sizeInPixels = angularSize * FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / lockCamera.FieldOfView;
            }

            Rectangle circleRect = new Rectangle(
                (int)(screenPos.X),
                (int)(screenPos.Y),
                (int)(sizeInPixels * 2),
                (int)(sizeInPixels * 2));

            Rectangle spriteRect = sprite.bigCrosshair;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, circleRect, spriteRect, Color.Red,
                (float)gameTime.TotalGameTime.TotalSeconds, Helpers.SpriteCenter(spriteRect), 
                SpriteEffects.None, 0);*/
        }

        private bool IsMouseDragMode()
        {
            if (!mouseEnabled)
                return false;

            if (currentMode.modeName == ModeName.HeightMode ||
                    currentMode.modeName == ModeName.YawMode || currentMode.modeName == ModeName.PitchMode ||
                    currentMode.modeName == ModeName.RollMode)
                return true;

            return false;
        }

        private void DrawMoveLegends(GameTime gameTime)
        {

            if (currentMode.modeName == ModeName.FacingMode)
            {
                DrawModeTip(gameTime,  Resource.MenuFacingTip);
            }
            else if (currentMode.modeName == ModeName.AimMode)
            {
                DrawModeTip(gameTime, Resource.MenuAimTip);
            }


#if WINDOWS
            if (mouseEnabled)
            {
                if (currentMode.modeName == ModeName.FacingMode || currentMode.modeName == ModeName.AimMode)
                {
                    float legendTransition = Math.Min(confirmButtonTransition, currentMode.transition);
                    Helpers.DrawLegendRow2(Resource.MenuNextTarget, sprite.buttons.spacebar,
                        legendTransition);
                }
                else if (IsMouseDragMode() || 
                    (currentMode.modeName != null && currentMode.modeName == ModeName.MoveMode))
                {
                    float iconTransition = Math.Min(currentMode.transition, confirmButtonTransition);
                    Helpers.DrawLegendRow2(Resource.MenuPressAndHold, sprite.buttons.mouseLeftClickTiny,
                        iconTransition);
                }
                

                return;
            }
#endif
            if (currentMode.modeName == ModeName.MoveMode)
            {
                Helpers.DrawLegend(Resource.MenuConfirm, sprite.buttons.a, currentMode.transition);
            }
            else if (currentMode.modeName == ModeName.HeightMode || currentMode.modeName == ModeName.ModifyHeightMode)
            {
                float legendX = Helpers.DrawLegend(Resource.MenuConfirm, sprite.buttons.a, currentMode.transition);
                Helpers.DrawLegendAt(Resource.MenuLegendVertical, sprite.buttons.lbrb, currentMode.transition,
                    legendX - Helpers.LEGENDGAPSIZE);                
            }
            else if (currentMode.modeName == ModeName.YawMode)
            {
                float legendX = Helpers.DrawLegend(Resource.MenuConfirm, sprite.buttons.a, currentMode.transition);
                float legendZ = Helpers.DrawLegendAt(Resource.MenuLegendYaw, sprite.buttons.lbrb, currentMode.transition,
                    legendX - Helpers.LEGENDGAPSIZE);

                Helpers.DrawLegendRow2Left(Resource.MenuAdvancedMoveOn, sprite.buttons.x, currentMode.transition);
            }
            else if (currentMode.modeName == ModeName.PitchMode)
            {
                float legendX = Helpers.DrawLegend(Resource.MenuConfirm, sprite.buttons.a, currentMode.transition);
                Helpers.DrawLegendAt(Resource.MenuLegendPitch, sprite.buttons.lbrb, currentMode.transition,
                    legendX - Helpers.LEGENDGAPSIZE);
            }
            else if (currentMode.modeName == ModeName.RollMode)
            {
                float legendX = Helpers.DrawLegend(Resource.MenuConfirm, sprite.buttons.a, currentMode.transition);
                Helpers.DrawLegendAt(Resource.MenuLegendRoll, sprite.buttons.lbrb, currentMode.transition,
                    legendX - Helpers.LEGENDGAPSIZE);
            }
            else if (currentMode.modeName == ModeName.AimMode || currentMode.modeName == ModeName.ModifyPriorityTarget)
            {
                Helpers.DrawLegend(Resource.MenuConfirmPriorityTargetSkip, sprite.buttons.x, currentMode.transition);
                Helpers.DrawLegendRow2(Resource.MenuNextTarget, sprite.buttons.rb, currentMode.transition);

                if (targetShip != null)
                {
                    DrawAimCrosshair(gameTime, Resource.ModeAimMode);
                    //Helpers.DrawLegendAt(Resource.MenuConfirm, sprite.buttons.a, currentMode.transition,
                    //    legendX - Helpers.LEGENDGAPSIZE);  
                }
                
            }
            else if (currentMode.modeName == ModeName.FacingMode)
            {
                float x = Helpers.DrawLegend(Resource.MenuNextTarget, sprite.buttons.rb, currentMode.transition);

                Helpers.DrawLegendRow2Left(Resource.MenuAdvancedMoveOff, sprite.buttons.x, currentMode.transition);


                if (targetShip != null)
                    DrawAimCrosshair(gameTime, Resource.MenuConfirmFacing);
                    //Helpers.DrawLegend(Resource.MenuConfirmFacing, sprite.buttons.a, currentMode.transition);
            }
        }

        private void DrawModeTip(GameTime gameTime, string text)
        {
            if (currentMode == null)
                return;

            if (targetShip != null)
                return;

            Vector2 screenCenter = Helpers.GetScreenCenter();
            screenCenter.Y = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.65f;

            Color textColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, currentMode.transition);
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0,0,0,128), currentMode.transition);


            float textSize = Helpers.PopLerp(currentMode.transition, 0.7f, 1.8f, 1.3f);
            textSize += Helpers.Pulse(gameTime, 0.02f, 4);

            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                text, screenCenter, textColor, bgColor, textSize, 0);
        }

        /// <summary>
        /// SHOW THE "A" BUTTON UNDER CROSSHAIR DURING AIM MODE.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="text"></param>
        private void DrawAimCrosshair(GameTime gameTime, string text)
        {
            Vector2 screenCenter = Helpers.GetScreenCenter();

            screenCenter.Y += FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.05f;

            float buttSize = 1.1f + Helpers.Pulse(gameTime, 0.08f, 10);
            buttSize = Helpers.PopLerp(targetShipTransition,
                Math.Max(0.5f, buttSize - 0.5f), buttSize+0.3f, buttSize);

            Color buttColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, targetShipTransition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, screenCenter, sprite.buttons.a,
                buttColor, 0, Helpers.SpriteCenter(sprite.buttons.a), buttSize, SpriteEffects.None, 0);

            screenCenter.Y += sprite.buttons.a.Height;

            Vector2 buttonText = screenCenter;
            buttonText.Y += Helpers.Pulse(gameTime, 2, 10);

            float textSize = Helpers.PopLerp(targetShipTransition, 0.8f, 1.1f, 1.0f);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), targetShipTransition);
            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                text, buttonText, buttColor, darkColor, textSize, 0);
        }






        private void DrawCrosshair()
        {
#if XBOX
            if (isTutorialLevel && tutState < TutState.doOrders)
                return;
#endif

            if (isReady)
                return;

            Vector2 crosshairPos = cursorPos;
#if WINDOWS
            if (mouseEnabled && crosshairTransition <= 0)
            {
                return;
            }
#endif

            if (FrameworkCore.players.Count > 1)
                crosshairPos.X *= 0.5f;
            


            float crosshairSize = MathHelper.Lerp(0.1f, 1.0f, crosshairTransition);
            Rectangle crosshairRect = sprite.crosshair;

            if (crosshairTransition <= 0)
            {
                crosshairSize = 1;
                crosshairRect = sprite.dot;
            }

            if (!commandMenu.HasFocus)
            {
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, crosshairPos, crosshairRect, Color.White, 0, Helpers.SpriteCenter(crosshairRect), crosshairSize, SpriteEffects.None, 0);
            }
        }


        private void CommandMenuDraw(GameTime gameTime)
        {
            if (isReady)
                return;

            if (currentMode.modeName != ModeName.Default)
                return;

            if (activeMenu != commandMenu && activeMenu != null)
                return;

            if (isTutorialLevel && tutState < TutState.doOrders)
                return;

            commandMenu.Draw(gameTime);

            if (isTutorialLevel && commandMenu.Transition >= 1)
            {
                if (tutEndTurnString == null)
                {
                    tutEndTurnString = Helpers.StringWrap(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                            Resource.TutorialEndTurn, 450, Vector2.Zero, Color.White);
                }

                Vector2 boxPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.76f);


                DrawTutorialBigMessage(tutEndTurnString, boxPos, 1);
            }
        }

        private string tutEndTurnString;


        private void DrawArrow(float arrowLength, Vector3 startPos, Vector3 direction, Color color, float arrowheadSize, Vector3 arrowheadVector)
        {
            Vector3 arrowStart = startPos;
            Vector3 arrowEnd = arrowStart + direction * arrowLength;
            FrameworkCore.lineRenderer.Draw(arrowStart, arrowEnd, color);

            if (arrowheadSize > 0)
            {
                Vector3 arrowheadStart = arrowEnd;
                Vector3 arrowheadEnd = arrowEnd + (direction * -arrowheadSize) + (arrowheadVector * arrowheadSize/2);
                FrameworkCore.lineRenderer.Draw(arrowheadStart, arrowheadEnd, color);

                arrowheadStart = arrowEnd;
                arrowheadEnd = arrowEnd + (direction * -arrowheadSize) + (arrowheadVector * -arrowheadSize/2);
                FrameworkCore.lineRenderer.Draw(arrowheadStart, arrowheadEnd, color);
            }
        }

        private void DrawMoveDisc(SpaceShip ship, Vector3 moveDiscPos, Color discColor, float discModifier)
        {
            moveDiscPos.Y = gridAltitude;
            FrameworkCore.discRenderer.Draw(ship.BSphere.Radius + discModifier, moveDiscPos, discColor);

        }

        private void DrawMoveArrows(GameTime gameTime, Vector3 arrowPos)
        {
            float sizeOffset = (float)(0.3f * Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * 0.01));
            float arrowLength = 4;
            float gapSize = 1; //gap between arrow and sphere.
            float arrowheadSize = 1.5f;
            //draw the orientation arrows.

            Color arrowColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, currentMode.transition);
            if (currentMode.modeName == ModeName.MoveMode)
            {
                DrawArrow(arrowLength, arrowPos + Vector3.Forward * (selectedShip.BSphere.Radius + gapSize + sizeOffset * 3),
                    Vector3.Forward, arrowColor, arrowheadSize, Vector3.Right);

                DrawArrow(arrowLength, arrowPos + Vector3.Forward * (-selectedShip.BSphere.Radius - gapSize - sizeOffset * 3),
                    -Vector3.Forward, arrowColor, arrowheadSize, Vector3.Right);

                DrawArrow(arrowLength, arrowPos + Vector3.Right * (selectedShip.BSphere.Radius + gapSize + sizeOffset * 3),
                    Vector3.Right, arrowColor, arrowheadSize, Vector3.Forward);

                DrawArrow(arrowLength, arrowPos + Vector3.Right * (-selectedShip.BSphere.Radius - gapSize - sizeOffset * 3),
                    -Vector3.Right, arrowColor, arrowheadSize, Vector3.Forward);
            }
            else if (currentMode.modeName == ModeName.HeightMode)
            {
                //we shift the arrow a little so that it doesn't overlap with the movedisc rod.
                Vector3 adjustedMovePos = arrowPos + new Vector3(0.1f, 0, 0.1f);
                DrawArrow(arrowLength, adjustedMovePos + Vector3.Up * (selectedShip.BSphere.Radius + gapSize + sizeOffset * 4),
                    Vector3.Up, arrowColor, arrowheadSize, Vector3.Forward);

                DrawArrow(arrowLength, adjustedMovePos + Vector3.Up * (-selectedShip.BSphere.Radius - gapSize - sizeOffset * 4),
                    -Vector3.Up, arrowColor, arrowheadSize, Vector3.Forward);
            }
        }

        private void DrawOrientationArrows(GameTime gameTime, Vector3 ghostPosition)
        {
            Color forwardColor = TeamColor;
            Color upColor = TeamColor;
            Color glowColor = Color.White;

            float forwardLength = Helpers.FORWARDARROWLENGTH;
            float upLength = Helpers.UPARROWLENGTH;

            float lineMod = (float)(0.6f * Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * 0.01));

            if (currentMode.modeName == ModeName.YawMode)
            {
                forwardColor = glowColor;

                forwardLength += lineMod;
            }
            else if (currentMode.modeName == ModeName.PitchMode)
            {
                forwardColor = glowColor;
                upColor = glowColor;

                forwardLength += lineMod;
                upLength += lineMod;
            }
            else if (currentMode.modeName == ModeName.RollMode)
            {
                upColor = glowColor;

                upLength += lineMod;
            }
            //forward line.                
            DrawArrow(forwardLength, ghostPosition + orientationMatrix.Forward * selectedShip.BSphere.Radius,
                orientationMatrix.Forward, forwardColor, 3f, orientationMatrix.Right);

            float offset = 7;
            float nearRange = 80;
            float farRange = 150;
            float movePosDist = Vector3.Distance(ghostPosition, position);
            float distTransition = movePosDist - nearRange;
            distTransition = MathHelper.Clamp(distTransition, 0, farRange);
            distTransition /= farRange;
            distTransition = MathHelper.Clamp(distTransition, 0, 1);
            float textSize = MathHelper.Lerp(1.0f, 0.6f, distTransition);


            Vector3 labelVec = ghostPosition + orientationMatrix.Forward * (forwardLength + offset + selectedShip.BSphere.Radius);
            DrawLabelCenter(gameTime, FrameworkCore.Serif, Resource.DirForward, forwardColor, labelVec, lockCamera, textSize);


            //the up arrow.
            Vector3 adjustedMovePos = ghostPosition + new Vector3(0.1f, 0, 0.1f);
            DrawArrow(upLength, adjustedMovePos + orientationMatrix.Up * selectedShip.BSphere.Radius,
                orientationMatrix.Up, upColor, 1f, orientationMatrix.Right);

            labelVec = ghostPosition + orientationMatrix.Up * (upLength + offset / 2 + selectedShip.BSphere.Radius);
            DrawLabelCenter(gameTime, FrameworkCore.Serif, Resource.DirUp, upColor, labelVec, lockCamera, textSize);
        }

        private void DrawMarkDot(GameTime gameTime, SpaceShip ship)
        {
            Color dotColor = Color.White;
            float dotSize = 1;

            if ((selectedShip != null && selectedShip == ship) ||
                (hoverShip != null && hoverShip == ship))
            {
                //dotColor = Color.Lerp(Color.White, TeamColor, 0.5f + Helpers.Pulse(gameTime, 0.49f, 10));
                dotSize = 1.3f;
            }

            dotSize += Helpers.Pulse(gameTime, 0.08f, 8);
            Vector3 ballPos = ship.markPos;
            ballPos.Y += Helpers.Pulse(gameTime, 0.2f, 4);
            BoundingSphere ball = new BoundingSphere(ballPos, dotSize);
            

            Matrix sphereRot = Matrix.Identity;
            sphereRot = sphereRot * Matrix.CreateFromAxisAngle(Vector3.Up, (float)gameTime.TotalGameTime.TotalSeconds * 1.0f);
            sphereRot = sphereRot * Matrix.CreateFromAxisAngle(Vector3.Forward, (float)gameTime.TotalGameTime.TotalSeconds * 0.4f);

            FrameworkCore.sphereRenderer.Draw(ball, sphereRot, dotColor);


            /*
            Color dotColor = Color.White;
            int dotSize = 10;

            if ((selectedShip != null && selectedShip == ship) ||
                (hoverShip != null && hoverShip == ship))
            {
                dotColor = Color.Lerp(Color.White, TeamColor, 0.5f + Helpers.Pulse(gameTime,0.49f, 10));
            }

            FrameworkCore.pointRenderer.Draw(ship.markPos, dotSize + 2, Color.Black);
            FrameworkCore.pointRenderer.Draw(ship.markPos, dotSize, dotColor);
             */
        }

        private void DrawGhostShipAndRod(Vector3 destination, Vector3 rodPosition)
        {
            Color greyColor = new Color(255, 255, 255, 96);

            Matrix worldMatrix = orientationMatrix;
            worldMatrix.Translation = destination;
            FrameworkCore.PlayerMeshRenderer.Draw(selectedShip.modelMesh, worldMatrix, lockCamera, Color.White, 0.3f);

            Helpers.DrawTurrets(FrameworkCore.PlayerMeshRenderer, selectedShip.Shipdata,
                worldMatrix, Color.White, 0.3f);




            //plane disc.

            //BC 3-28-2019 Make the disc move along with the rod during HeightMode, instead of staying static.
            if (currentMode.modeName == ModeName.HeightMode)
            {
                DrawMoveDisc(selectedShip, destination, greyColor, 0);
            }
            else
            {
                DrawMoveDisc(selectedShip, rodPosition, greyColor, 0);
            }

            //draw the rod.
            Color rodColor = new Color(255, 255, 255, 48);
            FrameworkCore.lineRenderer.Draw(
                new Vector3(rodPosition.X, gridAltitude, rodPosition.Z),
                rodPosition, rodColor);
        }





        private void DrawMoveLine(GameTime gameTime, Color primaryColor, SpaceShip ship, Vector3 finalPos)
        {
            if (!ship.markShouldDraw)
                FrameworkCore.lineRenderer.Draw(ship.Position, finalPos, primaryColor);
            else
            {
                FrameworkCore.lineRenderer.Draw(ship.Position, ship.markPos, primaryColor);

                Color darkColor = Color.Lerp(Color.Black, TeamColor, 0.3f);
                Color lightColor = Color.Lerp(Color.Black, TeamColor, 0.7f);
                Helpers.DrawDottedLine(gameTime, ship.markPos, finalPos,
                    darkColor, lightColor, 0.3f);

                //the little "knob" at the end of the dangling moveorder.
                //FrameworkCore.pointRenderer.Draw(finalPos, 6, Color.Black);
                //FrameworkCore.pointRenderer.Draw(finalPos, 4, TeamColor);
            }
        }

        //draw the movement cursor.
        public void DrawUI(GameTime gameTime, List<Collideable> ships)
        {
            if (IsPlaybackMode())
                return;


            DrawTutorialLines(gameTime);

            //foreach (Collideable ship in ships)
            for (int i = 0; i < ships.Count; i++)
            {
                if (!Helpers.IsSpaceship(ships[i]))
                    continue;

                SpaceShip ship = (SpaceShip)ships[i];

                if (ship.IsDestroyed)
                    continue;

                if (selectedShip == ship && currentMode.modeName != ModeName.Default)
                    continue;

                int blinkInterval = 0;
                if (ship == selectedShip)
                    blinkInterval = Helpers.BLINKTIME;

                bool shouldDrawBox = false;
                if (ship == selectedShip || ship == hoverShip)
                    shouldDrawBox = true;


                
                

                if (ship.owner == this)
                    ship.DrawHeadsUpDisplay(gameTime, lockCamera, blinkInterval, shouldDrawBox, HelpOverlayTransition);               
                




                if (ship.GetTargetPos() != Vector3.Zero && ship.owner == this)
                {
                    Color greyColor = Color.Lerp(OldXNAColor.TransparentWhite, new Color(112, 112, 112), 1);
                    Color primaryColor = greyColor;

                    if (ship == selectedShip)
                        primaryColor = Color.White;
                    else if (ship == hoverShip)
                    {
                        float pulse = 0.5f + (float)(0.49f * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5));
                        primaryColor = Color.Lerp(Color.White, greyColor, pulse);
                    }

                    DrawMoveOrder(gameTime, ship, greyColor, primaryColor, 1);

                    //draw targetship line.
                    if (ship.targetShip != null)
                    {
                        Helpers.DrawDottedLine(gameTime, ship.Position, ship.targetShip.Position, Color.Red, primaryColor);
                    }
                }

                //Color transparentColor = new Color(ship.owner.TeamColor.R, ship.owner.TeamColor.G, ship.owner.TeamColor.B, 0);
                //Color textColor = Color.Lerp(transparentColor, ship.owner.TeamColor, HelpOverlayTransition);
                //DrawLabel(gameTime, ship, lockCamera, ship.shipName, textColor);
                //DrawScaledText(gameTime, ship, ship.shipName, textColor, HelpOverlayTransition);
            }





            if (currentMode.modeName == ModeName.YawMode || currentMode.modeName == ModeName.PitchMode || currentMode.modeName == ModeName.RollMode ||
                currentMode.modeName == ModeName.MoveMode || currentMode.modeName == ModeName.HeightMode || 
                currentMode.modeName == ModeName.ModifyHeightMode || currentMode.modeName == ModeName.ModifyPriorityTarget)
            {
                if (selectedShip == null)
                {
                    ChangePlayerMode(ModeName.Default);
                    return;
                }


                DrawMoveLine(gameTime, Color.White, selectedShip, currentMovePos);
                


                Vector3 ghostShipPos = currentMovePos;

                if (selectedShip.markShouldDraw)
                    ghostShipPos = selectedShip.markPos;

                DrawGhostShipAndRod(ghostShipPos, currentMovePos);


                //draw the MarkPos ship's dot on the zero plane.
                if (currentMode.modeName == ModeName.HeightMode || currentMode.modeName == ModeName.MoveMode)
                {
                    if (selectedShip.markShouldDraw)
                    {
                        Vector3 dotStart = selectedShip.markPos;
                        Vector3 endDot = dotStart;
                        endDot.Y = GridAltitude;

                        Color lineColor = Color.Lerp(Color.Gray, TeamColor, 0.4f);
                        lineColor.A = 128;//BC 3-28-2019 Increase alpha.

                        FrameworkCore.lineRenderer.Draw(dotStart, endDot, lineColor);

                        FrameworkCore.pointRenderer.Draw(endDot, 7, Color.Black);
                        FrameworkCore.pointRenderer.Draw(endDot, 5, TeamColor);
                    }
                }



                if (currentMode.modeName == ModeName.MoveMode || currentMode.modeName == ModeName.HeightMode)
                {
                    DrawMoveArrows(gameTime, ghostShipPos);
                }
                else
                {
                    DrawOrientationArrows(gameTime, ghostShipPos);
                }

                if (currentMode.modeName == ModeName.ModifyHeightMode ||
                    currentMode.modeName == ModeName.ModifyPriorityTarget)
                {
                    Vector2 textPos = new Vector2(70, 50);
                    FrameworkCore.SpriteBatch.DrawString(FrameworkCore.SerifBig, currentMode.textLabel,
                        textPos, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
                else
                    DrawMoveTitles(gameTime);

                DrawGhostShip(gameTime);

#if WINDOWS
                DrawConfirmButton();
#endif
            }

            //draw a dot at the 30 second mark.
            for (int x = 0; x < ships.Count; x++)
            {
                if (!Helpers.IsSpaceship(ships[x]))
                    continue;

                SpaceShip ship = (SpaceShip)ships[x];

                if (ship.IsDestroyed || ship.owner.factionName != this.factionName || !ship.markShouldDraw)
                    continue;

                if (ship.owner != this)
                    continue;

                DrawMarkDot(gameTime, ship);
            }
            

            if (currentMode.modeName == ModeName.AimMode || currentMode.modeName == ModeName.ModifyPriorityTarget)
            {
                DrawAimMode(gameTime);
            }
            else if (currentMode.modeName == ModeName.FacingMode)
            {
                DrawFacingMode(gameTime);
            }            
        }

        private void DrawAimUI(GameTime gameTime)
        {
            if (selectedShip == null)
                return;

            DrawMoveTitles(gameTime);
            DrawConfirmButton();

            Vector3 destination = currentMovePos;

            if (selectedShip.markShouldDraw)
                destination = selectedShip.markPos;

            //DRAW THE GHOST SHIP            
            DrawGhostShipAndRod(destination, destination);


            DrawOrientationArrows(gameTime, destination);

            //DRAW THE LINE FROM THE SELECTEDSHIP TO THE MARK POSITION.
            FrameworkCore.lineRenderer.Draw(selectedShip.Position, destination, Color.White);

            //DRAW THE MARKDOTS.
            if (selectedShip.markShouldDraw)
                DrawMarkDot(gameTime, selectedShip);
        }

        private void DrawFacingMode(GameTime gameTime)
        {
            if (selectedShip == null)
                return;

            Vector3 destination = currentMovePos;

            if (selectedShip.markShouldDraw)
                destination = selectedShip.markPos;

            DrawAimUI(gameTime);

            DrawGhostShip(gameTime);

            if (targetShip == null)
                return;

            if (orientationMatrix == null)
                return;            
            
            //DRAW THE ARROW FROM THE SELECTEDSHIP TO THE TARGETSHIP.
            int arrowLength = (int)Vector3.Distance(targetShip.Position, destination);

            Color arrowColor = Color.Lerp(new Color(192, 192, 192), Color.White,
                0.5f + Helpers.Pulse(gameTime, 0.49f, 16));
            arrowColor = Color.Lerp(Helpers.transColor(arrowColor), arrowColor, targetShipTransition);

            Helpers.DrawDottedLine(gameTime, destination + new Vector3(0,0.1f,0),
                targetShip.Position + new Vector3(0, 0.1f, 0),
                arrowColor, OldXNAColor.TransparentWhite,
                1.5f, 1, 4/*flow speed*/);

            DrawAimSphere(gameTime);
        }

        private void DrawGhostShip(GameTime gameTime)
        {
            if (selectedShip == null)
                return;

            if (shipMenu.Transition > 0)
                return;

            Matrix worldMatrix = Matrix.CreateFromQuaternion(ghostRotation);
            worldMatrix.Translation = ghostPos;

            FrameworkCore.PlayerMeshRenderer.Draw(
                selectedShip.modelMesh, worldMatrix, lockCamera, TeamColor, 0.3f);
        }

        private void DrawAimMode(GameTime gameTime)
        {
            DrawAimUI(gameTime);

            DrawGhostShip(gameTime);

            if (targetShip == null || selectedShip == null)
                return;            

            Vector3 destVec = Vector3.Lerp(selectedShip.Position, targetShip.Position, targetShipTransition);
            Helpers.DrawDottedLine(gameTime, selectedShip.Position, destVec, Color.Red, Color.White, 2);

            DrawAimSphere(gameTime);
        }

        private void DrawAimSphere(GameTime gameTime)
        {
            if (targetShip == null)
                return;

            float sphereSize = MathHelper.Lerp(targetShip.BSphere.Radius * 8, targetShip.BSphere.Radius, targetShipTransition);
            Color sphereColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, targetShipTransition);
            BoundingSphere aimSphere = new BoundingSphere(targetShip.Position, sphereSize);
            Matrix shipRotation = Matrix.CreateFromQuaternion(targetShip.Rotation);
            shipRotation = shipRotation * Matrix.CreateFromAxisAngle(Vector3.Up, (float)gameTime.TotalGameTime.TotalSeconds * 0.4f);
            shipRotation = shipRotation * Matrix.CreateFromAxisAngle(Vector3.Forward, (float)gameTime.TotalGameTime.TotalSeconds * 0.05f);
            FrameworkCore.sphereRenderer.Draw(aimSphere, shipRotation, sphereColor);

        }

        private float centerTitleTransition;

        private void DrawCenterTitle(GameTime gameTime)
        {
            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                    TimeSpan.FromMilliseconds(1500).TotalMilliseconds);
            centerTitleTransition = MathHelper.Clamp(centerTitleTransition + delta, 0, 1);

            if (centerTitleTransition > 0 && currentMode != null)
            {
                Vector2 textPos = Helpers.GetScreenCenter();
                float textSize = 1;
                Color textColor = Color.White;
                Color bgColor = new Color(0, 0, 0, 128);

                if (centerTitleTransition < 0.3f)
                {
                    float adjustedTransition = centerTitleTransition * (1.0f / 0.3f);
                    adjustedTransition = MathHelper.Clamp(adjustedTransition, 0.0f, 1.0f);

                    textSize = Helpers.PopLerp(adjustedTransition, 0.3f, 1.4f, 1.0f);
                }
                else if (centerTitleTransition > 0.6f)
                {
                    float adjustedTransition = centerTitleTransition - 0.6f;
                    adjustedTransition *= (1.0f / 0.3f);
                    adjustedTransition = MathHelper.Clamp(adjustedTransition, 0.0f, 1.0f);

                    float smoothStep = MathHelper.SmoothStep(0, 1, adjustedTransition);
                    textPos = Vector2.Lerp(textPos, Vector2.Zero, smoothStep);

                    textColor = Color.Lerp(textColor, Helpers.transColor(textColor), smoothStep);
                    bgColor = Color.Lerp(bgColor, Helpers.transColor(bgColor), smoothStep);

                    textSize = MathHelper.Lerp(textSize, 0.01f, smoothStep);
                }

                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.SerifBig,
                    currentMode.textLabel, textPos, textColor, bgColor, textSize, 0);
            }
        }

        private void DrawMoveTitles(GameTime gameTime)
        {
            DrawCenterTitle(gameTime);


            Vector2 textPos = new Vector2(110, 90);
            foreach (PlayerMode playerMode in playerModes)
            {
                if (selectedShip == null)
                    return;

                if (selectedShip.OrderEffect == null)
                    return;

                //do not draw Fire Orders if the order has CanFire = false.
                if (!selectedShip.OrderEffect.canFire && playerMode.menuCategory == ModeCategory.Aim)
                    continue;


                SpriteFont font = FrameworkCore.Serif;
                float fontSize = 1;
                Color fontColor = new Color(255, 255, 255, 64);

                if (playerMode.transition > 0)
                {
                    font = FrameworkCore.SerifBig;
                    fontSize = MathHelper.Lerp(0.5f, 1, playerMode.transition);
                    fontColor = Color.Lerp(fontColor, Color.White, playerMode.transition);
                }

                FrameworkCore.SpriteBatch.DrawString(font, playerMode.textLabel, textPos,
                    fontColor, 0, Vector2.Zero, fontSize, SpriteEffects.None, 0);

                Vector2 textVec = font.MeasureString(playerMode.textLabel);
                textPos.Y += textVec.Y * fontSize;
            }
        }

        private void DrawMoveOrder(GameTime gameTime, SpaceShip currentShip, Color greyColor, Color primaryColor, float transition)
        {
            //this ship has a move order. Draw it.

            Vector3 meshPos = currentShip.GetTargetPos();

            if (currentShip.markShouldDraw)
                meshPos = currentShip.markPos;

            meshPos = Vector3.Lerp(currentShip.Position, meshPos, transition);


            Vector3 halfwayPos = Vector3.Lerp(currentShip.Position, currentShip.GetTargetPos(), 0.5f);
            Vector3 targetPos = Vector3.Lerp(halfwayPos, currentShip.GetTargetPos(), transition);

            DrawMoveLine(gameTime, primaryColor, currentShip, targetPos);

            
            

            float sphereRadius = MathHelper.Lerp(0.01f, currentShip.BSphere.Radius, transition);
            BoundingSphere moveSphere = new BoundingSphere(targetPos, sphereRadius);


            
            Matrix worldMatrix = Matrix.CreateFromQuaternion( currentShip.targetRotation );
            worldMatrix.Translation = meshPos;
            FrameworkCore.PlayerMeshRenderer.Draw(currentShip.modelMesh, worldMatrix, lockCamera, primaryColor, 0.6f);

            Helpers.DrawTurrets(FrameworkCore.PlayerMeshRenderer, currentShip.Shipdata,
                worldMatrix, primaryColor, 0.6f);


            float forwardLength = MathHelper.Lerp(0.01f, Helpers.FORWARDARROWLENGTH, transition) ;
            float upLength = MathHelper.Lerp(0.01f, Helpers.UPARROWLENGTH, transition);


            //the vertical line from ship to disc.
            Vector3 discPos = meshPos;
            discPos.Y = gridAltitude;
            Color lineColor = new Color(greyColor.R, greyColor.G, greyColor.B, 128); //BC 3-28-2019 Increase alpha of line.
            FrameworkCore.lineRenderer.Draw(discPos, meshPos, lineColor);



            DrawMoveDisc(currentShip, meshPos, greyColor, 0);

            Matrix orderOrientation = Matrix.CreateFromQuaternion(currentShip.GetTargetOrientation());

            DrawArrow(forwardLength, meshPos + orderOrientation.Forward * currentShip.BSphere.Radius,
                orderOrientation.Forward, primaryColor, 3f, orderOrientation.Right);

            targetPos.X += 0.1f;
            targetPos.Z += 0.1f;

            DrawArrow(upLength, meshPos + orderOrientation.Up * currentShip.BSphere.Radius,
                orderOrientation.Up, primaryColor, 1f, orderOrientation.Right);

        }



        private void DrawLabel(GameTime gameTime, SpaceShip ship, Camera camera, string helpText, Color textColor, float size)
        {
            Vector2 screenPos = Helpers.GetScreenPos(lockCamera, ship.Position);

            Vector2 stringCenter =
                FrameworkCore.Serif.MeasureString(helpText) / 2;

            screenPos.X += Helpers.SizeInPixels(lockCamera, ship.Position, ship.BSphere.Radius);
            screenPos.X += 4;
            screenPos.Y -= stringCenter.Y;

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, helpText, screenPos, textColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
        }

        private void DrawScaledText(GameTime gameTime, SpaceShip ship, string text, Color textColor, float transition)
        {
            float nearRange = 80;
            float farRange = 150;
            float movePosDist = Vector3.Distance(currentMovePos, position);
            float distTransition = movePosDist - nearRange;
            distTransition = MathHelper.Clamp(distTransition, 0, farRange);
            distTransition /= farRange;
            distTransition = MathHelper.Clamp(distTransition, 0, 1);
            float textSize = MathHelper.Lerp(1.0f, 0.6f, distTransition);



            Vector2 screenPos = Helpers.GetScreenPos(lockCamera, ship.Position);

            Vector2 stringCenter =
                FrameworkCore.Serif.MeasureString(text) / 2;

            screenPos.X += Helpers.SizeInPixels(lockCamera, ship.Position, ship.BSphere.Radius);
            screenPos.X += 4;
            screenPos.Y -= stringCenter.Y;

            screenPos = Vector2.Lerp(screenPos + new Vector2(-30, 0), screenPos, transition);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, text, screenPos, textColor, 0, Vector2.Zero, textSize, SpriteEffects.None, 0);            
        }



        private void DrawLabelCenter(GameTime gameTime, SpriteFont font, string text, Color fontColor,
            Vector3 labelPos, Camera camera, float size)
        {
            Vector2 screenPos = Helpers.GetScreenPos(lockCamera, labelPos);

            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, font, text, screenPos, fontColor, new Color(0, 0, 0, 128),
                size, 0);
        }





    }
}