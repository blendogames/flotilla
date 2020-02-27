#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
#endregion

namespace SpaceShooter
{
    public class SkirmishMenu : SysMenu
    {
        int WINDOWWIDTH = 1090;
        public SkirmishMenu()
        {
            MenuItem item = new MenuItem(Resource.MenuSkirmishEmptySlot);
            item.Selected += OnSlot1;
            item.position = new Vector2(100, 250);
            base.AddItem(item);

            item = new MenuItem(Resource.MenuSkirmishEmptySlot);
            item.Selected += OnSlot2;
            item.position = new Vector2(100, 250 + GetGapSize()*1);
            base.AddItem(item);

            item = new MenuItem(Resource.MenuSkirmishEmptySlot);
            item.Selected += OnSlot3;
            item.position = new Vector2(100, 250 + GetGapSize() * 3);
            base.AddItem(item);

            item = new MenuItem(Resource.MenuSkirmishEmptySlot);
            item.Selected += OnSlot4;
            item.position = new Vector2(100, 250 + GetGapSize() * 4);
            base.AddItem(item);

            item = new MenuItem(Resource.MenuSkirmishStart);
            item.Selected += OnSelectStart;
            item.position = new Vector2(100, 250 + GetGapSize() * 5);
            base.AddItem(item);


            item = new MenuItem(Resource.MenuCancel);
            item.Selected += OnClose;
            item.position = new Vector2(100, 250 + GetGapSize() * 6);
            base.AddItem(item);



#if XBOX
            SignedInGamer gamer = SignedInGamer.SignedInGamers[FrameworkCore.ControllingPlayer];
#endif

            menuItems[0].text = FrameworkCore.players[0].commanderName;
            menuItems[0].commander = FrameworkCore.players[0];
            


            if (FrameworkCore.players.Count > 1)
            {
                Player2Join();
            }



            //EnterSlot(menuItems[1], new Commander(Color.White, Color.White));
            EnterSlot(menuItems[2], new Commander(Color.White, Color.White));
            //EnterSlot(menuItems[3], new Commander(Color.White, Color.White));



            //center the elements on the X-axis.
            int screenWidth = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;
            int adjustedX = (screenWidth - WINDOWWIDTH/*width of entire thing*/) / 2;
            foreach (MenuItem i in menuItems)
            {
                i.position.X = Math.Max(adjustedX, 100);
            }

            int shipIndex = 0;
            //give some default ships to each fleet.
            foreach (MenuItem mItem in menuItems)
            {
                if (menuItems.IndexOf(mItem) > 3)
                    continue;

                for (int i = 0; i < mItem.shipArray.Length; i++)
                {
                    mItem.shipArray[i] = Helpers.getShipByNumber(FrameworkCore.skirmishShipArray[shipIndex]);
                    shipIndex++;
                }
            }
        }

        private void OnClose(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }

        private void OnSlot1(object sender, InputArgs e)
        {
            OnSelectSlot(e.index, 0);
        }

        private void OnSlot2(object sender, InputArgs e)
        {
            OnSelectSlot(e.index, 1);
        }

        private void OnSlot3(object sender, InputArgs e)
        {
            OnSelectSlot(e.index, 2);
        }

        private void OnSlot4(object sender, InputArgs e)
        {
            OnSelectSlot(e.index, 3);
        }

        private void OnSelectSlot(PlayerIndex index, int slotNumber)
        {
            if (index == FrameworkCore.ControllingPlayer)
            {
                SlotLogic(slotNumber, FrameworkCore.players[0]);
            }
            else
            {
                SlotLogic(slotNumber, FrameworkCore.players[1]);
            }
        }

        private void SlotLogic(int slotNumber, PlayerCommander player)
        {
            //let player2 remove himself.
            if (FrameworkCore.players.Count >= 2)
            {
                //player2 clicked himself.
                if (menuItems[slotNumber].commander == player &&
                    player == FrameworkCore.players[1])
                {
                    SkirmishPopup popup = new SkirmishPopup(Owner);
                    popup.screenPos = selectedItem.position + new Vector2(96, 16);
                    popup.hideChildren = false;

                    MenuItem item = new MenuItem(Resource.MenuSkirmishSlotClear);
                    item.Selected += ClearSlot;
                    item.GenericInt1 = slotNumber;
                    popup.AddItem(item);

                    item = new MenuItem(Resource.MenuCancel);
                    item.Selected += closePopup;
                    popup.AddItem(item);

                    Owner.AddMenu(popup);

                    return;
                }
            }

            if (menuItems[slotNumber].commander == player ||
                menuItems[slotNumber].commander == FrameworkCore.players[0])
            {
                //selected slot this player is already in.
            }
            else
            {
                SkirmishPopup popup = new SkirmishPopup(Owner);
                popup.screenPos = selectedItem.position + new Vector2(96, 16);
                popup.hideChildren = false;

                MenuItem item = null;

                //ADD COMPUTER.
                if (menuItems[slotNumber].commander == null)
                {
                    item = new MenuItem(Resource.MenuSkirmishEasyAdd);
                    item.Selected += AddAI;
                    item.GenericInt1 = slotNumber;
                    popup.AddItem(item);
                }

                //MOVE TO SLOT.
                if (menuItems[slotNumber].commander == null ||
                    (menuItems[slotNumber].commander != null &&
                    menuItems[slotNumber].commander.GetType() != typeof(PlayerCommander)))
                {
                    item = new MenuItem(Resource.MenuSkirmishSlotMove);
                    item.commander = player;//stuff data into this menu entry.
                    item.GenericInt1 = slotNumber;
                    item.Selected += MoveToSlot;
                    popup.AddItem(item);
                }

                if (menuItems[slotNumber].commander != null)
                {
                    if (menuItems[slotNumber].commander != FrameworkCore.players[0])
                    {
                        item = new MenuItem(Resource.MenuSkirmishSlotClear);
                        item.Selected += ClearSlot;
                        item.GenericInt1 = slotNumber;
                        popup.AddItem(item);
                    }
                }

                item = new MenuItem(Resource.MenuCancel);
                item.Selected += closePopup;
                popup.AddItem(item);

                Owner.AddMenu(popup);
            }
        }

        private void ClearSlot(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            int slotNumber = ((MenuItem)sender).GenericInt1;

            if (menuItems[slotNumber].commander.GetType() == typeof(PlayerCommander) && FrameworkCore.players.Count > 1)
            {
                //remove player 2.
                FrameworkCore.players.RemoveAt(1);
            }

            menuItems[slotNumber].commander = null;
            menuItems[slotNumber].text = Resource.MenuSkirmishEmptySlot;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }

        private void MoveToSlot(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            if (((MenuItem)sender).commander == null)
                return;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();

            Commander player = ((MenuItem)sender).commander;


            foreach (MenuItem item in menuItems)
            {
                if (item.commander == player)
                {
                    item.selectTransition = 0;
                    item.commander = null;
                    item.text = Resource.MenuSkirmishEmptySlot;
                }
            }

            //assign the player to the new slot location.
            int slotNumber = ((MenuItem)sender).GenericInt1;
            EnterSlot(menuItems[slotNumber], player);
        }

        private void AddAI(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();

            Color teamColor = Color.White;

            int slotNumber = ((MenuItem)sender).GenericInt1;

            if (slotNumber <= 1)
                teamColor = Faction.Blue.teamColor;
            else
                teamColor = Faction.Red.teamColor;

            Commander aiPlayer = new Commander(teamColor, teamColor);
            EnterSlot(menuItems[slotNumber], aiPlayer);
        }

        private void EnterSlot(MenuItem item, Commander player)
        {
            item.selectTransition = 0;
            item.commander = player;

            if (player.GetType() == typeof(PlayerCommander))
            {
                item.text = Helpers.GetPlayerName(((PlayerCommander)player).playerindex);
            }
            else
            {
                //NPC.
                item.text = Helpers.GenerateName("Gamertag");
            }
        }

        private void OnSelectStart(object sender, EventArgs e)
        {
            bool faction1HasCommander = false;
            bool faction2HasCommander = false;

            //check if both sides have at least 1 commander.
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (menuItems[i].commander != null && i <= 1)
                {
                    faction1HasCommander = true;
                }
                else if (menuItems[i].commander != null && (i == 2 || i == 3))
                {
                    faction2HasCommander = true;
                }
            }

            if (!faction1HasCommander || !faction2HasCommander)
            {
                SysPopup signPrompt = new SysPopup(Owner, Resource.MenuSkirmishMissingPlayer);
                signPrompt.transitionOnTime = 200;
                signPrompt.transitionOffTime = 200;
                signPrompt.darkenScreen = true;
                signPrompt.hideChildren = false;
                signPrompt.sideIconRect = sprite.windowIcon.error;

                MenuItem item = new MenuItem(Resource.MenuOK);
                item.Selected += CloseMenu;
                signPrompt.AddItem(item);

                Owner.AddMenu(signPrompt);

                FrameworkCore.PlayCue(sounds.click.error);
                return;
            }







            if (FrameworkCore.isTrialMode())
            {
                bool hasInvalidShip = false;

                //check if user has any non-trial ships in the ship array.
                foreach (MenuItem mItem in menuItems)
                {
                    if (menuItems.IndexOf(mItem) > 3)
                        continue;

                    for (int i = 0; i < mItem.shipArray.Length; i++)
                    {
                        if (Helpers.getShipByType(mItem.shipArray[i]) > 1)
                        {
                            hasInvalidShip = true;

                            mItem.shipArray[i] = null;
                        }
                    }
                }

                if (hasInvalidShip)
                {
                    SysPopup signPrompt = new SysPopup(Owner, Resource.MenuSkirmishTrialShipError);
                    signPrompt.transitionOnTime = 200;
                    signPrompt.transitionOffTime = 200;
                    signPrompt.darkenScreen = true;
                    signPrompt.hideChildren = false;
                    signPrompt.sideIconRect = sprite.windowIcon.error;

                    MenuItem item = new MenuItem(Resource.MenuOK);
                    item.Selected += CloseMenuStartSkirmish;
                    signPrompt.AddItem(item);

                    Owner.AddMenu(signPrompt);

                    return;
                }
            }


            //check if all slots have at least one ship.            
            bool showNoShipError = false;
            string missingShipPlayerName = "";
            foreach (MenuItem mItem in menuItems)
            {
                //only check player slots.
                if (menuItems.IndexOf(mItem) > 3)
                    continue;

                if (mItem.commander == null)
                    continue;

                bool PlayerHasNoShip = true;

                for (int i = 0; i < mItem.shipArray.Length; i++)
                {
                    if (Helpers.getShipByType(mItem.shipArray[i]) >= 0)
                    {
                        PlayerHasNoShip = false;
                    }
                }

                if (PlayerHasNoShip)
                {
                    showNoShipError = true;

                    if (mItem.commander.commanderName != null)
                        missingShipPlayerName = mItem.commander.commanderName;

                    break;
                }
            }

            if (showNoShipError)
            {
                String errorString = string.Format(Resource.MenuSkirmishNoShips, missingShipPlayerName);
                SysPopup signPrompt = new SysPopup(Owner, errorString);
                signPrompt.transitionOnTime = 200;
                signPrompt.transitionOffTime = 200;
                signPrompt.darkenScreen = true;
                signPrompt.hideChildren = false;
                signPrompt.sideIconRect = sprite.windowIcon.error;

                MenuItem item = new MenuItem(Resource.MenuOK);
                item.Selected += CloseMenu;
                signPrompt.AddItem(item);

                Owner.AddMenu(signPrompt);

                FrameworkCore.PlayCue(sounds.click.error);
                return;
            }
            


            IsLoading = true;

            //player wants to start the skirmish.

        }

        bool IsLoading = false;

        private void CloseMenuStartSkirmish(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            IsLoading = true;
        }



        private MenuItem getFirstUnoccupiedSlot()
        {
            foreach (MenuItem item in menuItems)
            {
                if (item.commander == null && menuItems.IndexOf(item) <= 3)
                    return item;
            }

            return null;
        }

        private void StartSkirmish(GameTime gameTime)
        {
            FrameworkCore.level.ClearAll();

            

            

            //populate the level with playerCommanders/npcCommanders.
            foreach (MenuItem item in menuItems)
            {
                //only check the first 4 slots.
                int slot = menuItems.IndexOf(item);
                if (slot >= 4)
                    continue;

                if (item.commander == null)
                    continue;
                
                //set the right team colors.
                if (slot <= 1)
                {
                    if (slot == 0)
                    {
                        item.commander.TeamColor = Faction.Blue.teamColor;
                        item.commander.ShipColor = Faction.Blue.teamColor;
                    }
                    else
                    {
                        item.commander.TeamColor = Faction.Blue.altColor;
                        item.commander.ShipColor = Faction.Blue.altColor;
                    }

                    item.commander.factionName = Faction.Blue;
                }
                else
                {
                    if (slot == 2)
                    {
                        item.commander.TeamColor = Faction.Red.teamColor;
                        item.commander.ShipColor = Faction.Red.teamColor;
                    }
                    else
                    {
                        item.commander.TeamColor = Faction.Red.altColor;
                        item.commander.ShipColor = Faction.Red.altColor;
                    }

                    item.commander.factionName = Faction.Red;
                }


                //found a valid commander. Add ships to this commander's fleet.
                Vector3 pos = Vector3.Zero;
                pos.Y += FrameworkCore.r.Next(-64, 64);   //altitude.
                
                pos.Z = FrameworkCore.r.Next(96, 160);  //initial distance between the fleets.
                //pos.Z = FrameworkCore.r.Next(32, 64);
                
                

                if (slot >= 2)
                    pos.Z *= -1.0f;

                float shipAngle = 0;
                if (slot >= 2)
                    shipAngle = 180;


                if (slot == 0 || slot == 2)
                {
                    pos.X = FrameworkCore.r.Next(-96, -64);
                }
                else
                {
                    pos.X = FrameworkCore.r.Next(64, 96);
                }


                Vector3 offset = new Vector3(0, 0, FrameworkCore.r.Next(-16, 16));


                for (int i = 0; i < item.shipArray.Length; i++)
                {
                    if (item.shipArray[i] == null)
                        continue;

                    FrameworkCore.level.AddToShiplist(item.shipArray[i],
                        item.commander,
                        FrameworkCore.level.Ships, pos + offset, shipAngle);

                    pos.X += 24;
                }
                
                if (item.commander != null && item.commander.GetType() != typeof(PlayerCommander))
                {
                    item.commander.commanderName = item.text;
                    FrameworkCore.level.AddToEnemiesList(item.commander);
                }
            }


            //great, all the ships are in place. Now, move the camera to a logical place.

            



            FrameworkCore.level.LoadShipContent();

            Helpers.UpdateCameraProjections(FrameworkCore.players.Count);
            FrameworkCore.level.StartGameplay(gameTime);
        }

        float loadTransition = 0;
        bool hasLoadedLevel = false;

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (IsLoading)
            {
                if (loadTransition >= 1 && !hasLoadedLevel)
                {
                    hasLoadedLevel = true;
                    StartSkirmish(gameTime);
                    Deactivate();
                }

                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(400).TotalMilliseconds);
                loadTransition = MathHelper.Clamp(loadTransition + delta, 0, 1);
                
                return;
            }

            if (Transition >= 1)
            {
                if (inputManager.buttonStartPressed && !IsLoading)
                {
                    IsLoading = true;
                }

                for (int i = 0; i < 4; i++)
                {
                    PlayerIndex index = PlayerIndex.One;

                    if (i == 0)
                        index = PlayerIndex.One;
                    else if (i == 1)
                        index = PlayerIndex.Two;
                    else if (i == 2)
                        index = PlayerIndex.Three;
                    else
                        index = PlayerIndex.Four;

                    if (FrameworkCore.MenuInputs[i].buttonAPressed)
                    {
                        if (index == FrameworkCore.ControllingPlayer)
                            continue;
                        
                        //another player wants to join.

                        if (getFirstUnoccupiedSlot() == null)
                            return;

                        //we need to add another player to the player list.
                        if (FrameworkCore.players.Count <= 1)
                        {
                            PlayerCommander player2 = new PlayerCommander(index, Color.White, Color.White);

#if WINDOWS
                            player2.mouseEnabled = FrameworkCore.options.p2UseMouse;
#endif
                            if (FrameworkCore.players.Count <= 1)
                                FrameworkCore.players.Add(player2);

                            player2.commanderName = Helpers.GenerateName("Gamertag");
                        }

                        bool player2HasJoined = false;
                        foreach (MenuItem item in menuItems)
                        {
                            if (item.commander == FrameworkCore.players[1])
                            {
                                player2HasJoined = true;
                                break;
                            }
                        }

                        if (!player2HasJoined)
                            Player2Join();
                        
                    }
                }


                //do the fleet setup logic.
                if (inputManager.sysMenuRight)
                {
                    if (selectedItem.commander == null)
                        return;

                    selectedItem.shipArraySelection = (int)MathHelper.Clamp(
                        selectedItem.shipArraySelection + 1,
                        -1,
                        selectedItem.shipArray.Length - 1);
                }
                else if (inputManager.sysMenuLeft)
                {
                    if (selectedItem.commander == null)
                        return;

                    selectedItem.shipArraySelection = (int)MathHelper.Clamp(
                        selectedItem.shipArraySelection - 1,
                        -1,
                        selectedItem.shipArray.Length - 1);
                }

                if (inputManager.buttonAPressed && selectedItem != null && selectedItem.shipArraySelection >= 0)
                {
                    if (selectedItem.commander == null)
                        return;

                    //modify what ship is in this slot.
                    OpenShipSelection(inputManager.playerIndex);
                }


                if (inputManager.buttonXPressed && selectedItem != null && selectedItem.shipArraySelection >= 0)
                {                    
                    if (selectedItem.shipArray[selectedItem.shipArraySelection] != null)
                        FrameworkCore.PlayCue(sounds.click.beep);

                    selectedItem.shipArray[selectedItem.shipArraySelection] = null;                   
                }                

#if WINDOWS
                foreach (MenuItem item in menuItems)
                {
                    Rectangle itemRect = new Rectangle(
                        (int)item.position.X,
                        (int)item.position.Y - 24,
                        512,
                        48);

                    if (itemRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                    {
                        if (inputManager.mouseHasMoved)
                        {
                            selectedItem = item;
                            item.shipArraySelection = -1;
                        }

                        if (inputManager.mouseLeftClick)
                        {
                            ActivateItem(inputManager);
                        }
                    }

                    if (menuItems.IndexOf(item) <= 3)
                    {
                        //one of the player slots.
                        if (item.commander == null)
                            continue;

                        for (int i = 0; i < item.shipArray.Length; i++)
                        {
                            Rectangle shipRect = new Rectangle(
                                (int)item.position.X + sprite.vistaBox.Width + 8 + (i*96),
                                (int)item.position.Y - 32,
                                96,
                                64);

                            if (shipRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                            {
                                selectedItem = item;
                                item.shipArraySelection = i;

                                if (inputManager.kbDelPressed)
                                {
                                    if (selectedItem != null)
                                    {
                                        if (selectedItem.shipArray[item.shipArraySelection] != null)
                                            FrameworkCore.PlayCue(sounds.click.beep);

                                        selectedItem.shipArray[item.shipArraySelection] = null;                                        
                                    }                                    
                                }

                                if (inputManager.mouseLeftClick)
                                {
                                    OpenShipSelection(FrameworkCore.ControllingPlayer);
                                }                                
                            }
                        }
                    }
                }
#endif
            }

            //This is an ugly hack! We need a short period of time we ignore the inputstates
            // because the player2join buttonpress is interfering with standard buttonpresses.
            if (player2JoinTimer > 0)
            {
                player2JoinTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds; 
                    return;
            }
            
            base.Update(gameTime, inputManager);
        }

        Rectangle debug1 = Rectangle.Empty;
        Rectangle debug2 = Rectangle.Empty;

        private void OpenShipSelection(PlayerIndex index)
        {
            if (selectedItem.commander == null)
                return;

            int playerIndex = (int)index;

            SkirmishPopup popup = new SkirmishPopup(Owner);            
            popup.hideChildren = false;
            popup.darkenScreen = true;

            MenuItem item = null;

            item = new MenuItem(Resource.ShipDestroyer);
            item.shipArray[0] = shipTypes.Destroyer;
            item.Selected += AddShip;
            item.GenericInt1 = playerIndex;
            popup.AddItem(item);

            item = new MenuItem(Resource.ShipBeamFrigate);
            item.shipArray[0] = shipTypes.BeamFrigate;
            item.Selected += AddShip;
            item.GenericInt1 = playerIndex;
            popup.AddItem(item);

            item = new MenuItem(Resource.ShipBeamGunship);
            item.shipArray[0] = shipTypes.BeamGunship;
            item.Selected += AddShip;
            item.GenericInt1 = playerIndex;
            popup.AddItem(item);

            item = new MenuItem(Resource.ShipGunship);
            item.shipArray[0] = shipTypes.Gunship;
            item.Selected += AddShip;
            item.GenericInt1 = playerIndex;
            popup.AddItem(item);


            item = new MenuItem(Resource.ShipDreadnought);
            item.shipArray[0] = shipTypes.Dreadnought;
            item.Selected += AddShip;
            item.GenericInt1 = playerIndex;
            popup.AddItem(item);


            item = new MenuItem(Resource.ShipBattleship);
            item.shipArray[0] = shipTypes.Battleship;
            item.Selected += AddShip;
            item.GenericInt1 = playerIndex;
            popup.AddItem(item);

            item = new MenuItem(Resource.ShipFighter);
            item.shipArray[0] = shipTypes.Fighter;
            item.Selected += AddShip;
            item.GenericInt1 = playerIndex;
            popup.AddItem(item);



#if DEBUG
            item = new MenuItem("DEBUG");
            item.shipArray[0] = shipTypes.DebugShip;
            item.Selected += AddShip;
            item.GenericInt1 = playerIndex;
            popup.AddItem(item);
#endif


            int shipsInList = 0;
            for (int i = 0; i < selectedItem.shipArray.Length; i++)
            {
                if (selectedItem.shipArray[i] != null)
                    shipsInList++;
            }

            if (selectedItem != null && selectedItem.shipArray[selectedItem.shipArraySelection] != null && shipsInList > 1)
            {
                item = new MenuItem(Resource.MenuSkirmishSlotClear);
                item.Selected += AddShip;
                popup.AddItem(item);
            }

            item = new MenuItem(Resource.MenuCancel);
            item.Selected += closePopup;
            popup.AddItem(item);

            Owner.AddMenu(popup);
        }



        private void closePopup(object sender, InputArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }

        private void AddShip(object sender, InputArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            int index = ((MenuItem)sender).GenericInt1;

            ShipData ship = ((MenuItem)sender).shipArray[0];

            if (FrameworkCore.isTrialMode() && ship != null)
            {
                if (Helpers.getShipByType(ship) > 1)
                {
#if WINDOWS
                    string errorString = string.Format(Resource.MenuSkirmishTrialSelection, ship.name);

                    SysPopup signPrompt = new SysPopup(Owner, errorString);
                    signPrompt.transitionOnTime = 200;
                    signPrompt.transitionOffTime = 200;
                    signPrompt.darkenScreen = true;
                    signPrompt.hideChildren = false;
                    signPrompt.sideIconRect = sprite.windowIcon.error;
                    
                    MenuItem item = new MenuItem(Resource.MenuUnlockFullGame);
                    item.Selected += OnBuyGame;
                    item.GenericInt1 = index;
                    signPrompt.AddItem(item);

                    item = new MenuItem(Resource.MenuDemoPlanetMaybeLater);
                    item.Selected += CloseMenu;
                    signPrompt.AddItem(item);

                    Owner.AddMenu(signPrompt);

                    FrameworkCore.PlayCue(sounds.click.error);
                    return;
#else
                    SignedInGamer gamer = SignedInGamer.SignedInGamers[(PlayerIndex)index];

                    //found signed in player. do stuff..
                    if (gamer != null && gamer.Privileges.AllowPurchaseContent)
                    {
                        string errorString = string.Format(Resource.MenuSkirmishTrialSelection, ship.name);

                        SysPopup signPrompt = new SysPopup(Owner, errorString);
                        signPrompt.transitionOnTime = 200;
                        signPrompt.transitionOffTime = 200;
                        signPrompt.darkenScreen = true;
                        signPrompt.hideChildren = false;
                        signPrompt.sideIconRect = sprite.windowIcon.error;

                        MenuItem item = new MenuItem(Resource.MenuUnlockFullGame);
                        item.Selected += OnBuyGame;
                        item.GenericInt1 = index;
                        signPrompt.AddItem(item);

                        item = new MenuItem(Resource.MenuDemoPlanetMaybeLater);
                        item.Selected += CloseMenu;
                        signPrompt.AddItem(item);

                        Owner.AddMenu(signPrompt);
                    }
                    else
                    {
                        //player has no purchasing privileges
                        string errorString = string.Format(Resource.MenuSkirmishTrialSelectionNoSignin, ship.name);

                        SysPopup signPrompt = new SysPopup(Owner, errorString);
                        signPrompt.transitionOnTime = 200;
                        signPrompt.transitionOffTime = 200;
                        signPrompt.darkenScreen = true;
                        signPrompt.hideChildren = false;
                        signPrompt.sideIconRect = sprite.windowIcon.error;

                        MenuItem item = new MenuItem(Resource.MenuOK);
                        item.Selected += CloseMenu;
                        signPrompt.AddItem(item);

                        Owner.AddMenu(signPrompt);
                    }

                    FrameworkCore.PlayCue(sounds.click.error);
                    return;
#endif
                }                
            }

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();


            int slotNumber = selectedItem.shipArraySelection;

            if (slotNumber < 0)
                return;


            selectedItem.shipArray[slotNumber] = ship;
        }

        private void OnBuyGame(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            PlayerIndex index = (PlayerIndex)((MenuItem)sender).GenericInt1;

            FrameworkCore.BuyGame(index);
        }


        private void CloseMenu(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }

        int player2JoinTimer = 0;
        private void Player2Join()
        {
            MenuItem item = getFirstUnoccupiedSlot();

            if (item == null)
                return;

            player2JoinTimer = 200;
            EnterSlot(item, FrameworkCore.players[1]);
        }




        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            int shipIndex = 0;
            foreach (MenuItem mItem in menuItems)
            {
                if (menuItems.IndexOf(mItem) > 3)
                    continue;

                for (int i = 0; i < mItem.shipArray.Length; i++)
                {
                    FrameworkCore.skirmishShipArray[shipIndex] = Helpers.getShipByType(mItem.shipArray[i]);
                    shipIndex++;
                }
            }

            SaveInfo save = FrameworkCore.storagemanager.GetDefaultSaveData();
            save.skirmishArray = FrameworkCore.skirmishShipArray;
            FrameworkCore.storagemanager.SaveData(save);

            base.Deactivate();
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {
            base.Activate();
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(100, 90);

            int screenWidth = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;
            int adjustedX = (screenWidth - 1090/*width of entire thing*/) / 2;                        
            pos.X = Math.Max(adjustedX, 100);



            
            float transitionMod = Helpers.PopLerp(Transition, -100, 40, 0);
            pos.X += transitionMod;
            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0,0,0,64), Transition);
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuSkirmish, pos, titleColor, darkColor,
                0, Vector2.Zero, 1);



            float titleSize = 0.5f;
            Vector2 faction1Pos = menuItems[0].position + new Vector2(transitionMod,-70);
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.FactionBlueName, faction1Pos,
                titleColor,
                new Color(0, 0, 0, 64), 0, Vector2.Zero, titleSize);

            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.FactionRedName, menuItems[2].position + new Vector2(transitionMod, -70),
                titleColor,
                new Color(0, 0, 0, 64), 0, Vector2.Zero, titleSize);

            DrawItems(gameTime, transitionMod);

            if (!IsLoading)
            {
                bool allSlotsFull = true;
                foreach (MenuItem item in menuItems)
                {
                    if (menuItems.IndexOf(item) > 3)
                        continue;

                    if (item.commander == null)
                        allSlotsFull = false;
                }

                if (!allSlotsFull)
                    Helpers.DrawPlayer2Join(Transition);
            }

            if (IsLoading)
            {
                Vector2 screenSize = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

                float adjustedTransition = loadTransition;

                adjustedTransition /= 0.8f;
                adjustedTransition = MathHelper.Clamp(adjustedTransition, 0, 1);

                Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, adjustedTransition);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                        new Rectangle(0, 0, (int)screenSize.X,
                            (int)screenSize.Y),
                        sprite.blank, bgColor);

                Color fgColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, adjustedTransition);
                Vector2 fgPos = new Vector2(screenSize.X / 2, screenSize.Y / 2);
                fgPos.Y -= MathHelper.Lerp(30, 0, loadTransition);
                Vector2 stringVec = FrameworkCore.Serif.MeasureString(Resource.MenuLoading);
                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuLoading,
                    fgPos - new Vector2(stringVec.X / 2, stringVec.Y / 2), fgColor);

                float ringSize = Helpers.PopLerp(adjustedTransition, 0.3f, 1.2f, 1.0f);
                Color ringColor = Color.Lerp(Helpers.transColor(Color.Gray), Color.Gray, adjustedTransition);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, fgPos, sprite.bigCircle,
                    ringColor, 0, Helpers.SpriteCenter(sprite.bigCircle), ringSize, SpriteEffects.None, 0);
            }


        }

        private int GetGapSize()
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            return (int)(textVec.Y + 16);
        }



        private void DrawFleet(GameTime gameTime, Vector2 pos, MenuItem item)
        {
            Vector2 fleetPos = pos + new Vector2(sprite.vistaBox.Width, 0);
            fleetPos.X += sprite.roundSquare.Width / 2;
            fleetPos.X += 24;
            
            Vector2 fleetSize = new Vector2(1.5f, 0.8f);
            fleetSize = Vector2.Lerp(
                new Vector2(0.8f, 0.4f),
                fleetSize,
                item.selectTransition);

            Color fleetColor = Color.Lerp(new Color(128, 128, 128, 0), new Color(128, 128, 128), item.selectTransition);

            fleetColor = Color.Lerp(Helpers.transColor(fleetColor), fleetColor, Transition);

            for (int i = 0; i < item.shipArray.Length; i++)
            {
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, fleetPos, sprite.roundSquare,
                    fleetColor, 0, Helpers.SpriteCenter(sprite.roundSquare), fleetSize, SpriteEffects.None, 0);

                if (item.shipArraySelection == i && item.selectTransition >= 1)
                {
                    float pulse = 0.5f + Helpers.Pulse(gameTime, 0.49f, 5);
                    Color pulseColor = Color.Lerp(
                        new Color(255,128,0),
                        new Color(255,160,0),
                        pulse);

                    Vector2 pulseSize = Vector2.Lerp(fleetSize, fleetSize + new Vector2(0.15f, 0.15f), pulse);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, fleetPos, sprite.roundSquare,
                        pulseColor, 0, Helpers.SpriteCenter(sprite.roundSquare),
                        pulseSize,
                        SpriteEffects.None, 0);
                }

                //draw the ship icon.
                if (item.shipArray[i] != null)
                {
                    ShipData data= item.shipArray[i];

                    Color shipColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, fleetPos, data.iconRect,
                     shipColor, 0, Helpers.SpriteCenter(data.iconRect),
                     1,
                     SpriteEffects.None, 0);
                }


                fleetPos.X += sprite.roundSquare.Width * 1.5f;
            }
        }

        public override void DrawItems(GameTime gameTime, float transitionMod)
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            foreach (MenuItem item in menuItems)
            {
                Vector2 pos = item.position;
                pos.X += transitionMod;

#if DEBUG
                Helpers.DrawDebugRectangle(item.hitBox, Color.Green);
#endif

                if (menuItems.IndexOf(item) <= 3)
                {
                    Color boxColor = Color.White;

                    if (menuItems.IndexOf(item) <= 1)
                        boxColor = Color.Lerp(Faction.Blue.teamColor, Color.Black, 0.4f); //new Color(0, 90, 170);
                    else
                        boxColor = Color.Lerp(Faction.Red.teamColor, Color.Black, 0.4f); //new Color(170, 0, 0);

                    Vector2 boxSize = new Vector2(
                        1.0f,
                        Helpers.PopLerp(item.selectTransition, 1, 1.6f, 1.35f));

                    boxColor = Color.Lerp(Helpers.transColor(boxColor), boxColor, Transition);

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, pos, sprite.vistaBox,
                        boxColor, 0, new Vector2(0, sprite.vistaBox.Height / 2), boxSize, SpriteEffects.None, 0);




                    
                    if (item.commander != null)
                    {
                        DrawFleet(gameTime, pos, item);


                        Rectangle spriteRect = Rectangle.Empty;
                        float ringAngle = 0;

                        if (item.commander.GetType() == typeof(PlayerCommander))
                        {
                            spriteRect = sprite.xboxRing;
                            ringAngle = Helpers.GetRingAngle(((PlayerCommander)item.commander).playerindex);
                        }
                        else
                        {
                            spriteRect = sprite.computer;
                        }

                        
                        Vector2 ringPos = new Vector2(pos.X + 8 + (spriteRect.Width / 2), pos.Y);

                        float ringSize = 1;

                        ringSize += Helpers.PopLerp(item.selectTransition, 0, 0.5f, 0.1f);




                        Color swatchColor = Color.White;
                        if (menuItems.IndexOf(item) == 0)
                            swatchColor = Faction.Blue.teamColor;
                        else if (menuItems.IndexOf(item) == 1)
                            swatchColor = Faction.Blue.altColor;
                        else if (menuItems.IndexOf(item) == 2)
                            swatchColor = Faction.Red.teamColor;
                        else
                            swatchColor = Faction.Red.altColor;

                        float swatchSize = 0.6f;
                        swatchSize += Helpers.PopLerp(item.selectTransition, 0, 0.4f, 0.1f);
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, ringPos, sprite.roundSquare,
                            swatchColor, 0, Helpers.SpriteCenter(sprite.roundSquare),
                            swatchSize, SpriteEffects.None, 0);



#if WINDOWS
                        if (item.commander == FrameworkCore.players[0])
                        {
                            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, ringPos, sprite.mouseIcon,
                                Color.White,
                                0, Helpers.SpriteCenter(sprite.mouseIcon), ringSize,
                                SpriteEffects.None, 0);
                        }
                        else
                        {
                            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, ringPos, spriteRect,
                                Color.White,
                                ringAngle, Helpers.SpriteCenter(spriteRect), ringSize,
                                SpriteEffects.None, 0);
                        }
#else
                            
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, ringPos, spriteRect,
                            Color.White,
                            ringAngle, Helpers.SpriteCenter(spriteRect), ringSize,
                            SpriteEffects.None, 0);
#endif


                    }
                }



                Color itemColor = Color.White;

                if (selectedItem != null && selectedItem == item)
                {
                    itemColor = Color.Lerp(itemColor, new Color(255, 128, 0), item.selectTransition);
                }

                itemColor = Color.Lerp(OldXNAColor.TransparentWhite, itemColor, Transition);
                Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);

                float itemSize = Helpers.PopLerp(item.selectTransition, 0.8f, 0.94f, 0.9f);

                if (menuItems.IndexOf(item) <= 3)
                    pos.X += 16 + sprite.xboxRing.Width;


                if (selectedItem == null || selectedItem.shipArraySelection >= 0)
                    itemColor = Color.White;

                Helpers.DrawOutline(menuFont, item.text, pos + new Vector2(0,-3), itemColor, darkColor,
                    0, new Vector2(0, textVec.Y / 2), itemSize);

            }
        }
    }
}
