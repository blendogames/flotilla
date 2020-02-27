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
        public int VideoHeight = 800;
        public bool isFullscreen = false;
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









        
    }
}