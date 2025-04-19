using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif




namespace SpaceShooter
{
    [Serializable]
    public class OptionsData
    {
        //these are the default values.
        public int VideoWidth = 1280;
        public int VideoHeight = 720;
        public bool isFullscreen = true;
        public bool bloom = true;

        public bool renderPlanets = true;
        public int mousewheel = 0;
        public int sensitivity = 5;
        public bool hardwaremouse = false;
        public bool manualDefault = false;

        public bool player1UseMouse =
#if WINDOWS
            true;
#else
            false;
#endif
        public bool player2UseMouse = false;
    }

    [Serializable]
    public class SaveInfo
    {
        public int adventure = 1;
        public int brightness = 5;
        public int volume=10;
        public int music=10;

        public bool p1InvertY = false;
        public bool p1InvertX = false;
        public bool p1vibration = true;

        public bool p2InvertY = false;
        public bool p2InvertX = false;
        public bool p2vibration = true;

        public int[] skirmishArray = new int[24]{
            0,1,-1,-1,-1,-1,
            0,1,-1,-1,-1,-1,
            0,1,-1,-1,-1,-1,
            0,1,-1,-1,-1,-1};

#if WINDOWS
        public string playerName = Helpers.GenerateName("Gamertag");
#endif
    }


    [Serializable]
    public class HighScoreEntry
    {
        public int[] scores;
        public string[] commanderName;
        public int count;

        public HighScoreEntry()
        {
            int Count = 25;
            commanderName = new string[Count];
            scores = new int[Count];     
            this.count = Count;

            for (int i = 0; i < Count; i++)
            {
                scores[i] = 0;
                commanderName[i] = "";
            }
        }
    }

    [Serializable]
    public class CampaignSave
    {
        public bool isHardcoreMode;
        public WorldMapSave WorldMap { get; set; }
        public PlayerCommanderSave PlayerCommander { get; set; }
        public EventSave Event { get; set; }
    }

    public class CampaignLoad
    {
        public bool isHardcoreMode;
        public WorldMap worldMap;
        public PlayerCommanderLoad playerCommander;
        public EventLoad eventLoad;
    }

    [Serializable]
    public class WorldMapSave
    {
        public List<Location> Locations { get; set; }
        public Location CurrentLocation { get; set; }

        public WorldMapSave()
        {
            // default constructor for XML serialization
        }

        public WorldMapSave(WorldMap worldMap)
        {
            Locations = worldMap.Locations;
            CurrentLocation = worldMap.CurrentLocation;
        }
    }

    [Serializable]
    public class PlayerCommanderSave
    {
        public List<InventoryItemSave> inventoryItems;
        public List<FleetShipSave> campaignShips;
        public int planetsVisited;
        public int dangerousPlanetsVisited;
        public int extraPoints;
        public int commandPoints;
    }

    [Serializable]
    public class FleetShipSave
    {
        public string captainName;
        public ModelType shipType;
        public int?[] upgradeArray;
        public int veterancy;
        public bool childShip;
        public SpaceShipStats stats;
    }

    [Serializable]
    public class EventSave
    {
        public bool kToucansOnboard;
        public bool kPandaOnboard;
        public bool kCrisiumOnBoard;
        public bool kHaveGauntlet;

        public List<InventoryItemSave> tradeItems;
        public List<LogEvent> Logs;

        public List<InventoryItemSave> inventoryPool;

        public List<String> eventPool;
        public List<String> dangerPool;
        public List<String> wormPool;
        public List<String> unlockableEventPool;
    }

    [Serializable]
    public class InventoryItemSave
    {
        public string itemType;
        public float? constructorParam;

        public InventoryItemSave()
        {
            // Default constructor for XML serialization
        }

        public InventoryItemSave(InventoryItem item)
        {
            itemType = item.GetType().FullName;
            constructorParam = item.constructorParam;
        }
    }

    public class PlayerCommanderLoad
    {
        public List<InventoryItem> inventoryItems;
        public List<FleetShip> campaignShips;
        public int planetsVisited;
        public int dangerousPlanetsVisited;
        public int extraPoints;
        public int commandPoints;
    }

    public class EventLoad
    {
        public bool kToucansOnboard;
        public bool kPandaOnboard;
        public bool kCrisiumOnBoard;
        public bool kHaveGauntlet;

        public List<InventoryItem> tradeItems;
        public List<LogEvent> Logs;
        public List<InventoryItem> inventoryPool;
        public List<Event> eventPool;
        public List<Event> dangerPool;
        public List<Event> wormPool;
        public List<Event> unlockableEventPool;
    }
    public static class StorageXNA4
    {
        public static StorageContainer OpenContainer(this StorageDevice device, string displayName)
        {
            IAsyncResult result = device.BeginOpenContainer(displayName, null, null);
            return device.EndOpenContainer(result);
        }
    }

    public class StorageManager
    {
        static public readonly string GAMENAME = "Flotilla";
        static public readonly string SAVEFILE= "saveinfo.dat";
        static public readonly string SCOREFILE = "scores.dat";
        static public readonly string PCFILE = "settings.xml";
        static public readonly string CAMPAIGNFILE = "adventure.xml";

        /// <summary>
        /// Location of the player profile's save area.
        /// </summary>
        static StorageDevice device = null;



        static StorageDevice highScoreDevice = null;

        public StorageManager()
        {
#if XBOX
            StorageDevice.DeviceChanged += new EventHandler<EventArgs>(StorageDeviceDeviceChanged);
#endif
        }

        public void SetDevice(StorageDevice Device)
        {
            device = Device;
        }

        //check if the logo or titlemenu is in the mainmenumanager Stack
        private bool LogoOrTitleInStack()
        {
            if (FrameworkCore.gameState != GameState.Logos)
                return false;

            if (FrameworkCore.MainMenuManager == null)
                return false;

            if (FrameworkCore.MainMenuManager.menus == null)
                return false;

            if (FrameworkCore.MainMenuManager.menus.Count <= 0)
                return false;

            for (int i = 0; i < FrameworkCore.MainMenuManager.menus.Count; i++)
            {
                if (FrameworkCore.MainMenuManager.menus[i] == null)
                    continue;

                if (FrameworkCore.MainMenuManager.menus[i].GetType() == typeof(LogoMenu) ||
                    FrameworkCore.MainMenuManager.menus[i].GetType() == typeof(TitleMenu))
                {
                    return true;
                }
            }

            return false;
        }

        private bool StoragePopupInStack()
        {
            for (int i = 0; i < FrameworkCore.sysMenuManager.menus.Count; i++)
            {
                if (FrameworkCore.sysMenuManager.menus[i] == null)
                    continue;

                if (FrameworkCore.sysMenuManager.menus[i].GetType() == typeof(SysPopup))
                {
                    if (((SysPopup)FrameworkCore.sysMenuManager.menus[i]).windowname == "storageerror")
                        return true;
                }
            }

            return false;
        }

        private bool deviceDisconnected()
        {
            try
            {
                if (device != null && !device.IsConnected)
                    return true;

                if (highScoreDevice != null && !highScoreDevice.IsConnected)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        void StorageDeviceDeviceChanged(object sender, EventArgs e)
        {
            if (!deviceDisconnected())
                return;

            if (LogoOrTitleInStack())
                return;

            if (FrameworkCore.sysMenuManager == null)
                return;

            if (StoragePopupInStack())
                return;

            SysPopup signPrompt = new SysPopup(FrameworkCore.sysMenuManager,
                Resource.SysDeviceDisconnect);

            signPrompt.transitionOnTime = 200;
            signPrompt.transitionOffTime = 200;
            signPrompt.darkenScreen = true;
            signPrompt.hideChildren = false;
            signPrompt.canBeExited = true;
            signPrompt.sideIconRect = sprite.windowIcon.error;
            signPrompt.windowname = "storageerror";

            MenuItem item = new MenuItem(Resource.MenuOK);
            item.Selected += ClosePopup;
            signPrompt.AddItem(item);

            FrameworkCore.sysMenuManager.AddMenu(signPrompt);
        }

        private void ClosePopup(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        public void SetHighScoreDevice(StorageDevice Device)
        {
            highScoreDevice = Device;
        }

        /*
        public static void SaveData(List<OptionsData> data, string filename)
        {
            if (device == null)
                return;

            using (StorageContainer container = device.OpenContainer(gamename))
            {
                string fullpath = Path.Combine(container.Path, filename);// Get the path of the save game.

                // Open the file, creating it if necessary
                using (FileStream stream = File.Open(fullpath, FileMode.Create))
                {
                    // Convert the object to XML data and put it in the stream
                    XmlSerializer serializer = new XmlSerializer(typeof(List<OptionsData>));
                    serializer.Serialize(stream, data);
                }
            }
        }
        */

        public SaveInfo GetDefaultSaveData()
        {
            SaveInfo save = new SaveInfo();
            save.brightness = FrameworkCore.options.brightness;
            save.adventure = FrameworkCore.adventureNumber;
            save.music = FrameworkCore.options.musicVolume;
            save.volume = FrameworkCore.options.soundVolume;

            save.p1InvertY = FrameworkCore.options.p1InvertY;
            save.p1InvertX = FrameworkCore.options.p1InvertX;
            save.p1vibration = FrameworkCore.options.p1Vibration;

            save.p2InvertY = FrameworkCore.options.p2InvertY;
            save.p2InvertX = FrameworkCore.options.p2InvertX;
            save.p2vibration = FrameworkCore.options.p2Vibration;

            save.skirmishArray = FrameworkCore.skirmishShipArray;

#if WINDOWS
            save.playerName = FrameworkCore.players[0].commanderName;
#endif

            return save;
        }

        public SaveInfo LoadData()
        {
            SaveInfo data = new SaveInfo();

            if (device == null)
                return data;

            bool createFile = false;

            try
            {
                using (StorageContainer container = device.OpenContainer(GAMENAME))
                {
                    using (Stream stream = container.OpenFile(SAVEFILE, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        // Read the data from the file
                        BinaryReader reader = new BinaryReader(stream);

                        try
                        {
                            data.adventure = reader.ReadInt16();
                            data.volume = reader.ReadInt16();
                            data.music = reader.ReadInt16();
                            data.brightness = reader.ReadInt16();

                            for (int i = 0; i < 24; i++)
                            {
                                data.skirmishArray[i] = reader.ReadInt16();
                            }

                            data.p1InvertY = reader.ReadBoolean();
                            data.p1InvertX = reader.ReadBoolean();
                            data.p1vibration = reader.ReadBoolean();

                            data.p2InvertY = reader.ReadBoolean();
                            data.p2InvertX = reader.ReadBoolean();
                            data.p2vibration = reader.ReadBoolean();

#if WINDOWS
                            data.playerName = reader.ReadString();
#endif
                        }
                        catch
                        {
                            //something went caca, so load default data.
                            createFile = true;
                        }

                        reader.Close();
                    }

                    container.Dispose();
                }
            }
            catch
            {
            }


            if (createFile)
            {
                //If the file doesn't exist, make a new one.
                data = new SaveInfo();
                SaveData(data);
            }

            //sanity check the values.
            data.adventure = Math.Max(data.adventure, 1);
            data.brightness = (int)MathHelper.Clamp(data.brightness, 0, 10);
            data.volume = (int)MathHelper.Clamp(data.volume, 0, 10);
            data.music = (int)MathHelper.Clamp(data.music, 0, 10);
                        
            return data;
        }

        public void SaveData(SaveInfo infoData)
        {
            if (device == null)
                return;
            try
            {
                using (StorageContainer container = device.OpenContainer(GAMENAME))
                {
                    using (Stream stream = container.OpenFile(SAVEFILE, FileMode.Create))
                    {
                        BinaryWriter writer = new BinaryWriter(stream);

                        writer.Write((Int16)infoData.adventure);
                        writer.Write((Int16)infoData.volume);
                        writer.Write((Int16)infoData.music);
                        writer.Write((Int16)infoData.brightness);

                        for (int i = 0; i < 24; i++)
                        {
                            writer.Write((Int16)infoData.skirmishArray[i]);
                        }

                        writer.Write((bool)infoData.p1InvertY);
                        writer.Write((bool)infoData.p1InvertX);
                        writer.Write((bool)infoData.p1vibration);

                        writer.Write((bool)infoData.p2InvertY);
                        writer.Write((bool)infoData.p2InvertX);
                        writer.Write((bool)infoData.p2vibration);

#if WINDOWS
                    writer.Write((string)infoData.playerName);
#endif

                        writer.Close();
                        stream.Close();
                    }

                    container.Dispose();
                }
            }
            catch
            {
            }
        }









        public HighScoreEntry LoadHighScores()
        {
            HighScoreEntry data = new HighScoreEntry();

            if (highScoreDevice == null)
            {
                return data;
            }

            bool createFile = false;

            try
            {
                using (StorageContainer container = highScoreDevice.OpenContainer(GAMENAME))
                {
                    using (Stream stream = container.OpenFile(SCOREFILE, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        // Read the data from the file
                        BinaryReader reader = new BinaryReader(stream);

                        try
                        {
                            data.count = reader.ReadInt16();
                            for (int i = 0; i < data.count; i++)
                            {
                                data.commanderName[i] = reader.ReadString();
                                data.scores[i] = reader.ReadInt32();
                            }
                        }
                        catch //(Exception ex)
                        {
                            //something went caca, so load default data.
                            //Console.WriteLine(ex);
                            createFile = true;
                        }

                        reader.Close();
                    }

                    container.Dispose();
                }
            }
            catch
            {
            }

            if (createFile)
            {
                //If the file doesn't exist, make a new one.
                data = new HighScoreEntry();
                SaveHighScores(data);
            }

            return data;
        }

        public void SaveHighScores(HighScoreEntry infoData)
        {
            if (highScoreDevice == null)
                return;

            try
            {
                using (StorageContainer container = highScoreDevice.OpenContainer(GAMENAME))
                {
                    using (Stream stream = container.OpenFile(SCOREFILE, FileMode.Create))
                    {
                        BinaryWriter writer = new BinaryWriter(stream);

                        writer.Write((Int16)infoData.count);

                        for (int i = 0; i < infoData.count; i++)
                        {
                            writer.Write((string)infoData.commanderName[i]);
                            writer.Write((Int32)infoData.scores[i]);
                        }

                        writer.Close();
                        stream.Close();
                    }

                    container.Dispose();
                }
            }
            catch
            {
            }

            FrameworkCore.highScores = infoData;
        }








        


        //do NOT use this on the xbox. this function is PC specific.
        public OptionsData LoadOptionsPC()
        {
                //these 3 lines will make the xbox explode.
                OptionsData data = null;
                IAsyncResult result = StorageDevice.BeginShowSelector(null, null);
                device = StorageDevice.EndShowSelector(result);

                bool createFile = false;

                using (StorageContainer container = device.OpenContainer(GAMENAME))
                {
                    // Open the file, create if necessary.
                    using (Stream stream = container.OpenFile(PCFILE, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        // Read the data from the file

                        try
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(OptionsData));
                            data = (OptionsData)serializer.Deserialize(stream);
                        }
                        catch
                        {
                            //something went caca, so load default data.
                            //data = new OptionsData();
                            createFile = true;
                        }
                    }

                    //if (!File.Exists(fullpath))
                    if (createFile)
                    {
                        //If the file doesn't exist, make a new one.
                        OptionsData newData = new OptionsData();

                        //choose desktop resolution.
                        SDL2.SDL.SDL_DisplayMode mode;
                        SDL2.SDL.SDL_GetCurrentDisplayMode(0, out mode);
                        newData.VideoWidth = mode.w;
                        newData.VideoHeight = mode.h;


                        using (Stream stream = container.OpenFile(PCFILE, FileMode.Create))
                        {
                            try
                            {
                                // Convert the object to XML data and put it in the stream
                                XmlSerializer serializer = new XmlSerializer(typeof(OptionsData));
                                serializer.Serialize(stream, newData);
                            }
                            catch
                            {
                            }
                        }

                        return newData;
                    }
                }

                //sanity check the values.
                data.VideoHeight = Math.Max(480, data.VideoHeight);
                data.VideoWidth = Math.Max(640, data.VideoWidth);


                return data;
        }


        public OptionsData GetDefaultPCOptions()
        {
            OptionsData options = new OptionsData();
            options.VideoHeight = FrameworkCore.options.resolutionY;
            options.VideoWidth = FrameworkCore.options.resolutionX;
            options.bloom = FrameworkCore.options.bloom;
            options.isFullscreen = FrameworkCore.options.fullscreen;
            options.renderPlanets = FrameworkCore.options.renderPlanets;
            options.mousewheel = FrameworkCore.options.mousewheel;
            options.sensitivity = FrameworkCore.options.sensitivity;
            options.hardwaremouse = FrameworkCore.options.hardwaremouse;
            options.manualDefault = FrameworkCore.options.manualDefault;


            

            options.player1UseMouse = FrameworkCore.options.p1UseMouse;
            options.player2UseMouse = FrameworkCore.options.p2UseMouse;




            return options;
        }


        public void SaveOptionsPC(OptionsData optionsData)
        {
            if (device == null)
            {
                IAsyncResult result = StorageDevice.BeginShowSelector(null, null);
                device = StorageDevice.EndShowSelector(result);
            }

            // Open a storage container.
            using (StorageContainer container = device.OpenContainer(GAMENAME))
            {
                // Open the file, creating it if necessary.
                using (Stream stream = container.OpenFile(PCFILE, FileMode.Create))
                {
                    try
                    {
                        // Convert the object to XML data and put it in the stream.
                        XmlSerializer serializer = new XmlSerializer(typeof(OptionsData));
                        serializer.Serialize(stream, optionsData);
                    }
                    catch
                    {
                    }
                }
            }

        }

        public bool CampaignFileExists()
        {
            if (device == null)
                return false;
            using (StorageContainer container = device.OpenContainer(GAMENAME))
            {
                return container.FileExists(CAMPAIGNFILE);
            }
        }

        public void DeleteCampaign()
        {
            if (device == null)
                return;
            using (StorageContainer container = device.OpenContainer(GAMENAME))
            {
                if (container.FileExists(CAMPAIGNFILE))
                {
                    container.DeleteFile(CAMPAIGNFILE);
                }
            }
        }
        public void SaveCampaign(WorldMap worldMap, PlayerCommander player)
        {
            FNALoggerEXT.LogInfo("Saving Adventure");
            if (device == null)
                return;
            using (StorageContainer container = device.OpenContainer(GAMENAME))
            {
                using (Stream stream = container.OpenFile(CAMPAIGNFILE, FileMode.Create))
                {
                    try
                    {
                        CampaignSave campaignSave = new CampaignSave()
                        {
                            isHardcoreMode = FrameworkCore.isHardcoreMode,
                            WorldMap = new WorldMapSave(worldMap),
                            PlayerCommander = CreatePlayerCommanderSave(player),
                            Event = CreateEventSave(worldMap),
                        };

                        XmlSerializer serializer = new XmlSerializer(typeof(CampaignSave));
                        serializer.Serialize(stream, campaignSave);
                    }
                    catch (InvalidOperationException ex)
                    {
                        FNALoggerEXT.LogError("Serialization error: " + ex.Message);
                        if (ex.InnerException != null)
                        {
                            FNALoggerEXT.LogError("Inner exception: " + ex.InnerException.Message);
                        }
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        FNALoggerEXT.LogError(ex.Message);
                        FNALoggerEXT.LogError(ex.StackTrace);
                    }
                }
            }
        }

        private static PlayerCommanderSave CreatePlayerCommanderSave(PlayerCommander player)
        {
            List<InventoryItem> inventoryItems = player.inventoryItems;
            List<FleetShipSave> campaignShips = new List<FleetShipSave>();
            foreach (FleetShip fleetShip in player.campaignShips)
            {
                campaignShips.Add(CreateFleetShipSave(fleetShip, inventoryItems));
            }

            PlayerCommanderSave playerCommanderSave = new PlayerCommanderSave
            {
                inventoryItems = CreateInventoryItemSaveList(inventoryItems),
                campaignShips = campaignShips,
                planetsVisited = player.planetsVisited,
                dangerousPlanetsVisited = player.dangerousPlanetsVisited,
                extraPoints = player.extraPoints,
                commandPoints = player.commandPoints,
            };
            return playerCommanderSave;
        }

        private static FleetShipSave CreateFleetShipSave(FleetShip fleetShip, List<InventoryItem> inventoryItems)
        {
            FleetShipSave fleetShipSave = new FleetShipSave()
            {
                captainName = fleetShip.captainName,
                shipType = fleetShip.shipData.modelname,
                upgradeArray = CreateUpgradeArray(fleetShip.upgradeArray, inventoryItems),
                veterancy = fleetShip.veterancy,
                childShip = fleetShip.childShip,
                stats = fleetShip.stats,
            };
            return fleetShipSave;
        }

        private static List<InventoryItemSave> CreateInventoryItemSaveList(List<InventoryItem> inventoryItems)
        {
            List<InventoryItemSave> inventoryItemSaveList = new List<InventoryItemSave>();
            foreach (InventoryItem item in inventoryItems)
            {
                inventoryItemSaveList.Add(new InventoryItemSave(item));
            }
            return inventoryItemSaveList;
        }

        public static int?[] CreateUpgradeArray(InventoryItem[] upgradeArray, List<InventoryItem> inventoryItems)
        {
            int inventoryItemArrayLength = upgradeArray.Length;
            int?[] upgradeSaveArray = new int?[inventoryItemArrayLength];
            for (int i = 0; i < inventoryItemArrayLength; i++)
            {
                InventoryItem item = upgradeArray[i];
                if (item != null)
                {
                    int? index = inventoryItems.IndexOf(item);
                    // if the item is not found in the inventoryItems list, set index to null
                    upgradeSaveArray[i] = (index != -1) ? index : null;
                }
            }

            return upgradeSaveArray;
        }

        private static EventSave CreateEventSave(WorldMap worldMap)
        {
            EventManager eventManager = worldMap.evManager;
            EventSave eventSave = new EventSave
            {
                kToucansOnboard = eventManager.kToucansOnboard,
                kPandaOnboard = eventManager.kPandaOnboard,
                kCrisiumOnBoard = eventManager.kCrisiumOnBoard,
                kHaveGauntlet = eventManager.kHaveGauntlet,
                tradeItems = CreateInventoryItemSaveList(eventManager.tradeItems),
                Logs = eventManager.Logs,
                inventoryPool = CreateInventoryItemSaveList(eventManager.inventoryPool),
                eventPool = CreateEventPoolSave(eventManager.eventPool),
                dangerPool = CreateEventPoolSave(eventManager.dangerPool),
                wormPool = CreateEventPoolSave(eventManager.wormPool),
                unlockableEventPool = CreateEventPoolSave(eventManager.unlockableEventPool),
            };
            return eventSave;
        }

        public static List<String> CreateEventPoolSave(List<Event> eventPool)
        {
            List<String> eventPoolSave = new List<String>();
            foreach (Event ev in eventPool)
            {
                eventPoolSave.Add(ev.GetType().FullName);
            }
            return eventPoolSave;
        }

        // Load the campaign data from adventure.xml
        public CampaignLoad LoadCampaign()
        {
            using (StorageContainer container = device.OpenContainer(GAMENAME))
            {
                using (Stream stream = container.OpenFile(CAMPAIGNFILE, FileMode.Open))
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(CampaignSave));
                        CampaignSave campaignSave = (CampaignSave)serializer.Deserialize(stream);
                        return new CampaignLoad
                        {
                            isHardcoreMode = campaignSave.isHardcoreMode,
                            worldMap = RestoreWorldMap(campaignSave.WorldMap),
                            playerCommander = RestorePlayerCommander(campaignSave.PlayerCommander),
                            eventLoad = RestoreEvent(campaignSave.Event)
                        };
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            FNALoggerEXT.LogError("Deserialization error. Inner exception: " + ex.InnerException.Message);
                        }
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        FNALoggerEXT.LogError(ex.Message);
                        FNALoggerEXT.LogError(ex.StackTrace);
                        throw ex;
                    }
                }
            }
        }
        private WorldMap RestoreWorldMap(WorldMapSave worldMapSave)
        {
            WorldMap worldMap = new WorldMap();
            worldMap.Locations = worldMapSave.Locations;
            worldMap.CurrentLocation = worldMapSave.CurrentLocation;

            return worldMap;
        }

        private PlayerCommanderLoad RestorePlayerCommander(PlayerCommanderSave playerCommanderSave)
        {
            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            foreach (InventoryItemSave itemSave in playerCommanderSave.inventoryItems)
            {
                inventoryItems.Add(RestoreInventoryItem(itemSave));
            }
            List<FleetShip> campaignShips = new List<FleetShip>();
            foreach (FleetShipSave fleetShipSave in playerCommanderSave.campaignShips)
            {
                campaignShips.Add(RestoreFleetShip(fleetShipSave, inventoryItems));
            }
            return new PlayerCommanderLoad()
            {
                inventoryItems = inventoryItems,
                campaignShips = campaignShips,
                planetsVisited = playerCommanderSave.planetsVisited,
                dangerousPlanetsVisited = playerCommanderSave.dangerousPlanetsVisited,
                extraPoints = playerCommanderSave.extraPoints,
                commandPoints = playerCommanderSave.commandPoints,
            };
        }

        private InventoryItem RestoreInventoryItem(InventoryItemSave itemSave)
        {
            Type itemType = Type.GetType(itemSave.itemType);
            if (itemType != null)
            {
                InventoryItem item;
                float? constructorParam = itemSave.constructorParam;
                if (constructorParam.HasValue)
                {
                    item = (InventoryItem)Activator.CreateInstance(itemType, constructorParam.Value);
                }
                else
                {
                    item = (InventoryItem)Activator.CreateInstance(itemType);
                }

                return item;
            }
            return null;
        }

        private FleetShip RestoreFleetShip(FleetShipSave fleetShipSave, List<InventoryItem> inventoryItems)
        {
            FleetShip fleetShip = new FleetShip
            {
                captainName = fleetShipSave.captainName,
                shipData = shipTypes.GetShipDataByModelType(fleetShipSave.shipType),
                upgradeArray = RestoreUpgradeArray(fleetShipSave.upgradeArray, inventoryItems),
                veterancy = fleetShipSave.veterancy,
                childShip = fleetShipSave.childShip,
                stats = fleetShipSave.stats
            };
            return fleetShip;
        }

        private InventoryItem[] RestoreUpgradeArray(int?[] upgradeArraySave, List<InventoryItem> inventoryItems)
        {
            int upgradeArrayLength = upgradeArraySave.Length;
            InventoryItem[] upgradeArray = new InventoryItem[upgradeArrayLength];
            for (int i = 0; i < upgradeArrayLength; i++)
            {
                int? index = upgradeArraySave[i];
                if (index != null)
                {
                    upgradeArray[i] = inventoryItems[(int) index];
                }
            }
            return upgradeArray;
        }

        private EventLoad RestoreEvent(EventSave eventSave)
        {
            EventLoad eventLoad = new EventLoad
            {
                kToucansOnboard = eventSave.kToucansOnboard,
                kPandaOnboard = eventSave.kPandaOnboard,
                kCrisiumOnBoard = eventSave.kCrisiumOnBoard,
                kHaveGauntlet = eventSave.kHaveGauntlet,

                tradeItems = RestoreInventoryPool(eventSave.tradeItems),
                Logs = eventSave.Logs,
                inventoryPool = RestoreInventoryPool(eventSave.inventoryPool),
                eventPool = RestoreEventPool(eventSave.eventPool),
                dangerPool = RestoreEventPool(eventSave.dangerPool),
                wormPool = RestoreEventPool(eventSave.wormPool),
                unlockableEventPool = RestoreEventPool(eventSave.unlockableEventPool)
            };

            return eventLoad;
        }

        public List<InventoryItem> RestoreInventoryPool(List<InventoryItemSave> inventoryPool)
        {
            List<InventoryItem> items = new List<InventoryItem>();
            foreach (InventoryItemSave itemSave in inventoryPool)
            {
                InventoryItem item = RestoreInventoryItem(itemSave);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items;
        }

        public List<Event> RestoreEventPool(List<String> eventPoolSave)
        {
            List<Event> eventPool = new List<Event>();
            foreach (string eventTypeName in eventPoolSave)
            {
                Type eventType = Type.GetType(eventTypeName);
                if (eventType != null)
                {
                    Event ev = (Event)Activator.CreateInstance(eventType);
                    eventPool.Add(ev);
                }
            }
            return eventPool;
        }

    }
}