#region Using
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#if XBOX
using Microsoft.Xna.Framework.Net;
#endif
using Microsoft.Xna.Framework.Storage;

using System.IO;
#endregion

namespace SpaceShooter
{
    /// <summary>
    /// Manages the random events.
    /// </summary>
    public class EventManager
    {
        //bank of Global Flags.
        public bool kToucansOnboard = false;
        public bool kPandaOnboard = false;
        public bool kCrisiumOnBoard = false;
        public bool kHaveGauntlet = false;




        public List<InventoryItem> tradeItems;


        InventoryItem[] tradePool = new InventoryItem[]
        {
            new itKeshiaEngine(1.5f),

            new itJamalAutoDoc(120),
            new itJamalAutoDoc(150),

            new itMuyoShield(0.3f),
            new itMuyoShield(0.6f),

            new itRoachShield(0.3f),
            new itRoachShield(0.6f),

            new itRashadFireCon(2.0f),
            new itRashadFireCon(2.5f),

            new itBeamShield(0.3f),
            new itBeamShield(0.5f),
            new itBeamShield(0.7f),
            new itBeamShield(0.9f),            
        };



        List<LogEvent> log;

        public List<LogEvent> Logs
        {
            get { return log; }
        }


        List<InventoryItem> inventoryPool;
        List<Event> eventPool;

        List<Event> dangerPool;
        public int dangerPoolCount
        {
            get { return dangerPool.Count; }
        }


        List<Event> wormPool;
        public int wormPoolCount
        {
            get { return wormPool.Count; }
        }


        List<Event> unlockableEventPool;
        SysMenuManager menuManager;



        



        private void PopulateEvents()
        {         
            //register the base events into the pool.      
            eventPool.Add(new evSpaceHulk());
            eventPool.Add(new evAurora()); 
            eventPool.Add(new evHitchhikers());
            eventPool.Add(new evRuins());
            eventPool.Add(new evProfessors());         
            eventPool.Add(new evMurder());
            eventPool.Add(new evKatanaShip());
            eventPool.Add(new evCrocodile());
            eventPool.Add(new evBandits());
            eventPool.Add(new evPigs());
            eventPool.Add(new evDeer());
            eventPool.Add(new evAssassin());
            eventPool.Add(new evKaraoke());


            //events reserved for the full game.
            if (!FrameworkCore.isTrialMode())
            {
                eventPool.Add(new evCasino());
                eventPool.Add(new evStowaway());
                eventPool.Add(new evAbandoned());
                eventPool.Add(new evBabyYetis());
            }




            //eventPool.Add(new evDebug());
        }



        private void PopulateDangerPool()
        {
            //dangerous events.
            dangerPool.Add(new evSlavers());            
            dangerPool.Add(new evPirates());
            dangerPool.Add(new evPirateHostage());
            dangerPool.Add(new evSlaveFight());
            dangerPool.Add(new evPenguins());
            dangerPool.Add(new evPenguinReunion());
        }

        private void PopulateWormPool()
        {
            //wormhole events.
            wormPool.Add(new evUnicorn());
            wormPool.Add(new evAfroDita());
        }




        private void PopulateInventory()
        {
            //register all our inventory items into the pool.
            inventoryPool.Add(new itMuyoShield(0.1f));
            inventoryPool.Add(new itMuyoShield(0.1f));
            inventoryPool.Add(new itMuyoShield(0.2f));
            inventoryPool.Add(new itMuyoShield(0.3f));


            inventoryPool.Add(new itRoachShield(0.2f));
            inventoryPool.Add(new itRoachShield(0.1f));


            inventoryPool.Add(new itRashadFireCon(1.2f));
            inventoryPool.Add(new itRashadFireCon(1.2f));
            inventoryPool.Add(new itRashadFireCon(1.4f));
            inventoryPool.Add(new itRashadFireCon(1.6f));


            inventoryPool.Add(new itJamalAutoDoc(50));
            inventoryPool.Add(new itJamalAutoDoc(100));

            inventoryPool.Add(new itKeshiaEngine(1.2f));
            inventoryPool.Add(new itKeshiaEngine(1.2f));
            inventoryPool.Add(new itKeshiaEngine(1.3f));
            inventoryPool.Add(new itKeshiaEngine(1.4f));


            inventoryPool.Add(new itBlesoRailChamber(1.1f));
            inventoryPool.Add(new itBlesoRailChamber(1.1f));
        }

        private void PopulateUnlockableEvents()
        {
            unlockableEventPool.Add(new evCatCharity());
            unlockableEventPool.Add(new evAuroraResearchBad());
            unlockableEventPool.Add(new evAuroraResearchGood());
            unlockableEventPool.Add(new evAssassinRevenge());
            unlockableEventPool.Add(new evToucanRevenge());
            unlockableEventPool.Add(new evHippoPirateBuddy());
            unlockableEventPool.Add(new evAbandonZombies());            
        }

        public void UnlockEvent(Event ev)
        {
            InitializeEvent(ev);
            eventPool.Add(ev);
        }


        public void RunEvent(Event ev)
        {
            InitializeEvent(ev);
            ev.Activate();
        }

        public EventManager(SysMenuManager menu)
        {
            if (menu == null)
                throw new NullReferenceException("eventmanager menu is null");

            this.menuManager = menu;

            unlockableEventPool = new List<Event>();
            PopulateUnlockableEvents();


            inventoryPool = new List<InventoryItem>();
            PopulateInventory();


            eventPool = new List<Event>();
            PopulateEvents();

            dangerPool = new List<Event>();
            PopulateDangerPool();

            wormPool = new List<Event>();
            PopulateWormPool();


            foreach (Event ev in eventPool)
            {
                InitializeEvent(ev);
            }

            foreach (Event ev in dangerPool)
            {
                InitializeEvent(ev);
            }

            foreach (Event ev in wormPool)
            {
                InitializeEvent(ev);
            }

            log = new List<LogEvent>();


            //populate the trade outpost.
            int TRADEITEMQUANTITY = 5;
            tradeItems = new List<InventoryItem>();
            for (int i = 0; i < TRADEITEMQUANTITY; i++)
            {
                bool check = true;
                while (check == true)
                {
                    InventoryItem randItem = tradePool[FrameworkCore.r.Next(tradePool.Length)];

                    bool isDuplicate = false;
                    for (int x = 0; x < tradeItems.Count; x++)
                    {
                        if (tradeItems[x] == randItem)
                        {
                            //found it.
                            isDuplicate = true;
                        }
                    }

                    if (!isDuplicate)
                    {
                        tradeItems.Add(randItem);
                        check = false;
                    }
                }
            }
        }


        private void InitializeEvent(Event ev)
        {
            ev.manager = this.menuManager;
            ev.eventManager = this;
        }

        //check if last planet.
        private bool IsLastPlanet()
        {
            for (int i = 0; i < FrameworkCore.worldMap.Locations.Count; i++)
            {
                if (FrameworkCore.worldMap.Locations[i].worldType == WorldType.Trader)
                    continue;

                if (FrameworkCore.worldMap.Locations[i].worldType == WorldType.wormhole)
                    continue;

                if (FrameworkCore.worldMap.Locations[i].isExplored)
                    continue;

                if (!FrameworkCore.worldMap.Locations[i].isVisible)
                    continue;

                return false;
            }

            return true;
        }

        private bool TerminalCheck()
        {
            if (!FrameworkCore.isHardcoreMode && FrameworkCore.players[0].planetsVisited >= FrameworkCore.r.Next(Helpers.MAXEVENTS, Helpers.MAXEVENTS + 2))
            {
                return true;
            }
            else if (FrameworkCore.isHardcoreMode &&
                (eventPool.Count <= 0 || IsLastPlanet() || FrameworkCore.players[0].planetsVisited > FrameworkCore.worldMap.Locations.Count - 5 ))
            {
                return true;
            }

            return false;
        }

        public bool StartEvent(WorldType worldType)
        {
            if (TerminalCheck())
            {
                //endgame timer.
                Event endGame = new evTerminalDeath();
                InitializeEvent(endGame);
                endGame.Activate();                

                return true;
            }

            if (worldType == WorldType.Normal)
                return AddEvent(eventPool);
            else if (worldType == WorldType.Dangerous)
                return AddEvent(dangerPool);
            else if (worldType == WorldType.wormhole)
                return AddEvent(wormPool);
            else if (worldType == WorldType.Trader)
            {
                //open the trade menu.
                
                return true;
            }

            return false;
        }


        /// <summary>
        /// Trigger a random event.
        /// </summary>
        public bool AddEvent(List<Event> eventList)
        {
            if (eventList.Count <= 0)
                return false;

            int eventIndex = FrameworkCore.r.Next(eventList.Count);

            eventList[eventIndex].Activate();

            //if (eventList[eventIndex].musicCue != null)
            //    FrameworkCore.PlayCue(eventList[eventIndex].musicCue);
            

            //remove event from the pool.
            eventList.RemoveAt(eventIndex);

            return true;
        }


        



        public void RunTutorial()
        {
            Event tutorial = new evTutorial();
            InitializeEvent(tutorial);

            tutorial.Activate();
        }

        /// <summary>
        /// add a new ship to this player's flotilla.
        /// </summary>
        public void AddShip(ShipData shipType)
        {
            FrameworkCore.PlayCue(sounds.Fanfare.ship);

            FleetShip ship = Helpers.AddFleetShip(FrameworkCore.players[0].campaignShips, shipType);

            //create popup.
            if (ship == null)
                return;

            ShipPopup popup = new ShipPopup(menuManager);
            popup.fleetShip = ship;            
            menuManager.AddMenu(popup);
        }



        public void AddShip(ShipData[] shipType)
        {
            FrameworkCore.PlayCue(sounds.Fanfare.ship);

            ShipData shipToAdd = shipType[FrameworkCore.r.Next(shipType.Length)];

            FleetShip ship = Helpers.AddFleetShip(FrameworkCore.players[0].campaignShips, shipToAdd);

            //create popup.
            if (ship == null)
                return;

            ShipPopup popup = new ShipPopup(menuManager);
            popup.fleetShip = ship;
            menuManager.AddMenu(popup);
        }


        public void AddCargo(InventoryItem item)
        {
            FrameworkCore.PlayCue(sounds.Fanfare.item);

            ItemPopup popup = new ItemPopup(menuManager);
            popup.inventoryItem = item;
            menuManager.AddMenu(popup);

            FrameworkCore.players[0].AddCargo(item);
        }


        /// <summary>
        /// give inventory item to player
        /// </summary>
        public void AddCargo()
        {
            if (inventoryPool.Count <= 0)
            {
                //inventory pool is empty. refill it back up!
                PopulateInventory();
            }

            FrameworkCore.PlayCue(sounds.Fanfare.item);

            int itemIndex = FrameworkCore.r.Next(inventoryPool.Count);
            FrameworkCore.players[0].AddCargo(inventoryPool[itemIndex]);

            ItemPopup popup = new ItemPopup(menuManager);
            popup.inventoryItem = inventoryPool[itemIndex];
            menuManager.AddMenu(popup);

            //now remove the item from the pool.
            inventoryPool.RemoveAt(itemIndex);
        }

        /// <summary>
        /// Deletes one piece of cargo. Returns TRUE if successful.
        /// </summary>
        /// <returns></returns>
        public bool LoseRandomCargo(out string itemName, out string itemDescription)
        {
            if (FrameworkCore.players[0].inventoryItems.Count <= 0)
            {
                itemName = "";
                itemDescription = "";
                return false;
            }

            int indexToDelete = FrameworkCore.r.Next(FrameworkCore.players[0].inventoryItems.Count);

            foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
            {
                for (int i = 0; i < ship.upgradeArray.Length; i++)
                {
                    if (ship.upgradeArray[i] == FrameworkCore.players[0].inventoryItems[indexToDelete])
                        ship.upgradeArray[i] = null;
                }
            }

            itemName = FrameworkCore.players[0].inventoryItems[indexToDelete].name;
            itemDescription = FrameworkCore.players[0].inventoryItems[indexToDelete].description;

            FrameworkCore.players[0].inventoryItems.RemoveAt(indexToDelete);
            return true;
        }

        /// <summary>
        /// Events can be locked by Keys. This allows an Event to be unlocked by two or more specific Events.
        /// </summary>
        /// <param name="ev"></param>
        public void AddKey(Type ev)
        {
            for (int i = unlockableEventPool.Count - 1; i >= 0; i--)
            {
                if (unlockableEventPool[i].GetType() == ev)
                {
                    unlockableEventPool[i].keys--;

                    if (unlockableEventPool[i].keys <= 0)
                    {
                        UnlockEvent(unlockableEventPool[i]);
                        unlockableEventPool.RemoveAt(i);
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// clear out all items in player's inventory, ouch
        /// </summary>
        public bool LoseAllCargo()
        {
            //remove upgrades from ships.
            foreach (FleetShip ship in FrameworkCore.players[0].campaignShips)
            {
                for (int i = 0; i < ship.upgradeArray.Length; i++)
                {
                    ship.upgradeArray[i] = null;                    
                }
            }

            if (FrameworkCore.players[0].inventoryItems.Count > 0)
            {
                FrameworkCore.players[0].inventoryItems.Clear();
                return true;
            }

            return false;
        }


        public void AddLog(Rectangle img, string txt)
        {
            log.Add(new LogEvent(img, txt));
        }
    }

    public class LogEvent
    {
        public Rectangle image;
        public string description;

        public LogEvent(Rectangle img, string txt)
        {
            this.image = img;
            this.description = txt;
        }
    }

    /// <summary>
    /// A random adventure event.
    /// </summary>
    public class Event
    {
        public Rectangle image = Rectangle.Empty;
        public SysMenuManager manager;
        public EventManager eventManager;
        //event MenuItem.InputEventHandler ev;

        public int keys = int.MaxValue;

        /// <summary>
        /// Array of ShipDatas for what ships can be spawned in this encounter.
        /// </summary>
        public ShipData[] shipList;

        /// <summary>
        /// The Min and Max of how many ships are in this encounter.
        /// </summary>
        public Point shipMinMax;

        /// <summary>
        /// What faction this encounter belongs to.
        /// </summary>
        public FactionInfo faction;

        public string pilotName;

        /// <summary>
        /// What music is associated with this encounter.
        /// </summary>
        public string musicCue;

        public virtual void Activate()
        {
            if (musicCue != null)
                FrameworkCore.PlayCue(musicCue);            
        }

        public virtual void Deactivate()
        {

        }

        protected EventPopup CreatePopup(SysMenuManager manager)
        {
            EventPopup popup = new EventPopup(manager);
            manager.AddMenu(popup);
            return popup;
        }

        protected EventPopup CreatePopup(SysMenuManager manager, Rectangle img, string txt)
        {
            EventPopup popup = new EventPopup(manager);
            popup.image = img;
            popup.description = txt;
            manager.AddMenu(popup);
            return popup;
        }

        public void OnGetCargo(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            eventManager.AddCargo();
        }

        public void OnClose(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }

    }
}