#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using Microsoft.Xna.Framework.Storage;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif

#if SDL2
using SDL2;
#elif WINDOWS
using System.Windows.Forms;
#endif

#if STEAM
using System.Runtime.InteropServices;
#endif

#endregion

namespace SpaceShooter
{
    public enum GameState
    {
        Logos,
        Play,
        WorldMap,
    }

    /// <summary>
    /// We reference meshes through this enum list.
    /// </summary>
    public enum ModelType
    {
        Bullet = 0,
        Missile,
        Torpedo,
        beam,

        debrisAsteroid01,
        debrisAsteroid02,
        debrisDebris01,
        debrisDebris02,
        debrisHulk1,
        debrisHulk2,   
        asteroidchunk,

        planet,
        planetTiny,

        shipBeamFrigate,
        shipCapitalShip,
        shipFighter,
        shipDestroyer,
        shipGunship,
        shipDreadnought,
        shipBeamGunship,

        turretBall,
        turretLong,
        turretBig,

        junk1,
        junk2,
    }

    /// <summary>
    /// There are the engine classes which are independent from the game. 
    /// There are all of the graphic information and the engine’s managers.
    /// It initializes/updates every manager.
    /// </summary>
    public class FrameworkCore : Microsoft.Xna.Framework.Game
    {
        public static string VERSION
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() +
                    "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
            }
        }
        


        /// <summary>
        /// Filenames for all the meshes. This has to correlate exactly to the ModelType list!
        /// </summary>
        static String[] modelFiles = new String[]
        {
            "projectiles/bullet",
            "projectiles/missile",
            "projectiles/torpedo",
            "meshes/beam",

            "meshes/asteroid01",
            "meshes/asteroid02",
            "meshes/debris01",
            "meshes/debris02",
            "Ships/hulk1",
            "Ships/hulk2",      
            "meshes/asteroidchunk",

            "meshes/planet",
            "meshes/planettiny",

            "Ships/beamfrigate",
            "Ships/capitalship",
            "Ships/fighter",
            "Ships/freighter",
            "Ships/gunship",
            "Ships/dreadnought",
            "Ships/beamgunship",
            
            "Ships/turretball",
            "Ships/turretlong",
            "Ships/turretbig",

            "meshes/junk1",
            "meshes/junk2",
        };


        static Model[] modelArray;
        static Texture2D[] textureArray;


        static GraphicsDeviceManager graphics;
        public static GameState gameState;

        static SpriteBatch spriteBatch;
        static BoltManager boltManager;
        static ParticleManager particles;
        static BloomComponent bloomComponent;
        static ContentManager contentManager;
        static Game game;
        public static Random r = new Random();        
        static SpriteFont serif;
        static SpriteFont serifbig;
        static SpriteFont gothic;

        /// <summary>
        /// If True, then signing in or out will kick the controllingPlayer to the titlescreen.
        /// </summary>
        public static bool shouldCheckSignIn = false;

        static LineRenderer linerenderer;
        static DiscRenderer discrenderer;
        static SphereRenderer sphererenderer;
        static PointRenderer pointrenderer;
        static Texture2D hudsheet;
        
        static Texture2D eventsheet;
        public static bool debugMode = false;
        static MeshRenderer Meshrenderer;
        static MeshRenderer playerMeshRenderer;
        public static PlaybackSystem playbackSystem;
        static WorldTextManager worldTextManager;
        static DebrisManager debrismanager;
        static HulkManager hulkmanager;
        static Model modelDebris01;
        static Model modelDebris02;
        static Model modelAsteroid01;
        static Model modelAsteroid02;
        static Model modelBeam01;
        static Model modelPlanet;
        static Model modelPlanetTiny;

        public static HighScoreEntry highScores;


        public static int adventureNumber = 1;

        public static bool HideHud = false;
        

        public static Model[] ModelArray
        {
            get { return modelArray; }
        }

        public static Texture2D[] TextureArray
        {
            get { return textureArray; }
        }


        public static WorldMap worldMap;

        public static int campaignTimer = 0;

        public static Options options;
        public static Level level;
        static PlayerIndex controllingPlayer = PlayerIndex.Four;
        public static PlayerIndex ControllingPlayer
        {
            get { return controllingPlayer; }
            set { controllingPlayer = value; }
        }



        
        static InputManager[] menuInputs = new InputManager[4];
        public static InputManager[] MenuInputs
        {
            get { return menuInputs; }
        }


        public static List<PlayerCommander> players = new List<PlayerCommander>();


        static SysMenuManager mainMenuManager;
        public static SysMenuManager MainMenuManager
        {
            get { return mainMenuManager; }
            set { mainMenuManager = value; }
        }

        static SysMenuManager sysmenumanager;

        public static SysMenuManager sysMenuManager
        {
            get { return sysmenumanager; }
        }

        public static int[] skirmishShipArray = new int[24]
        {
            0,0,1,-1,-1,-1,
            0,0,1,-1,-1,-1,
            0,0,1,-1,-1,-1,
            0,0,1,-1,-1,-1,
        };

        static AudioEngine audioEngine;
        static SoundBank soundBank;
        static WaveBank waveBank;
        static AudioCategory SoundCategory;
        static AudioCategory MusicCategory;

        public static AudioCategory soundCategory
        {
            get { return SoundCategory; }
        }

        public static AudioCategory musicCategory
        {
            get { return MusicCategory; }
        }

        static AudioManager audioManager;
        public static AudioManager audiomanager
        {
            get { return audioManager; }
        }


        public static class audio
        {
            public static AudioListener nearestListener(Vector3 soundPos)
            {
                if (players.Count <= 1)
                {
                    return players[0].Listener;
                }

                float closestDist = float.MaxValue;
                PlayerCommander closestPlayer = null;

                for (int i = 0; i < players.Count; i++)
                {
                    float curDist = Helpers.FastDistanceCheck(players[i].position, soundPos);
                    if (curDist < closestDist)
                    {
                        closestDist = curDist;
                        closestPlayer = players[i];
                    }
                }
                
                return closestPlayer.Listener;
            }
        }



        





        public static SoundBank soundbank
        {
            get { return soundBank; }
        }

        public static Model ModelPlanet
        {
            get { return modelPlanet; }
        }

        public static Model ModelPlanetTiny
        {
            get { return modelPlanetTiny; }
        }

        public static Model ModelAsteroid01
        {
            get { return modelAsteroid01; }
        }

        public static Model ModelAsteroid02
        {
            get { return modelAsteroid02; }
        }

        public static Model ModelBeam01
        {
            get { return modelBeam01; }
        }



        public static Model ModelDebris01
        {
            get { return modelDebris01; }
        }

        public static Model ModelDebris02
        {
            get { return modelDebris02; }
        }

        public static HulkManager hulkManager
        {
            get { return hulkmanager; }
        }

        public static DebrisManager debrisManager
        {
            get { return debrismanager; }
        }

        public static WorldTextManager WorldtextManager
        {
            get { return worldTextManager; }
        }

        public static BloomComponent Bloomcomponent
        {
            get { return bloomComponent; }
        }

        public static MeshRenderer PlayerMeshRenderer
        {
            get { return playerMeshRenderer; }
        }

        public static MeshRenderer meshRenderer
        {
            get { return Meshrenderer; }
        }


        public static Texture2D hudSheet
        {
            get { return hudsheet; }
        }

        public static Texture2D eventSheet
        {
            get { return eventsheet; }
        }

        public static PointRenderer pointRenderer
        {
            get { return pointrenderer; }
        }

        public static SphereRenderer sphereRenderer
        {
            get { return sphererenderer; }
        }

        public static DiscRenderer discRenderer
        {
            get { return discrenderer; }
        }

        public static LineRenderer lineRenderer
        {
            get { return linerenderer; }
        }


        public static SpriteFont Serif
        {
            get { return serif; }
        }

        public static SpriteFont SerifBig
        {
            get { return serifbig; }
        }

        public static SpriteFont Gothic
        {
            get { return gothic; }
        }


        public static Game Game
        {
            get { return game; }
        }

        public static ContentManager content
        {
            get { return contentManager; }
        }

        public static GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public static SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }


        public static BoltManager Bolts
        {
            get { return boltManager; }
        }

        public static ParticleManager Particles
        {
            get { return particles; }
        }


        public static bool isActive
        {
            get { return Game.IsActive; }
        }

        static StorageManager storageManager;
        public static StorageManager storagemanager
        {
            get { return storageManager; }
        }

        public static bool isCampaign
        {
            get { return (worldMap != null); }
        }

        public static bool isHardcoreMode = false;


        public static InputManager controllingInputManager
        {
            get
            {
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

                    if (index == FrameworkCore.ControllingPlayer)
                        return FrameworkCore.menuInputs[i];
                }

                return FrameworkCore.menuInputs[0];
            }
        }

        
#if WINDOWS && STEAM
        public class ManagedSteam
        {
            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            private static extern bool SteamAPI_Init();

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern void SteamAPI_Shutdown();

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern void SteamAPI_RunCallbacks();

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr SteamInternal_CreateInterface(
                [MarshalAs(UnmanagedType.LPStr)]
                    string pchVersion
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern int SteamAPI_GetHSteamUser();

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern int SteamAPI_GetHSteamPipe();

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr SteamAPI_ISteamClient_GetISteamUserStats(
                IntPtr steamClient,
                int steamUser,
                int steamPipe,
                [MarshalAs(UnmanagedType.LPStr)]
                    string pchVersion
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            private static extern bool SteamAPI_ISteamUserStats_RequestCurrentStats(
                IntPtr instance
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            private static extern bool SteamAPI_ISteamUserStats_StoreStats(
                IntPtr instance
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            private static extern bool SteamAPI_ISteamUserStats_SetAchievement(
                IntPtr instance,
                [MarshalAs(UnmanagedType.LPStr)]
                    string name
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr SteamAPI_ISteamClient_GetISteamFriends(
                IntPtr steamClient,
                int steamUser,
                int steamPipe,
                [MarshalAs(UnmanagedType.LPStr)]
                    string pchVersion
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr SteamAPI_ISteamFriends_GetPersonaName(
                IntPtr instance
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern int SteamAPI_ISteamFriends_GetFriendCount(
                IntPtr instance,
                int iFriendFlags
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern ulong SteamAPI_ISteamFriends_GetFriendByIndex(
                IntPtr instance,
                int index,
                int iFriendFlags
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr SteamAPI_ISteamFriends_GetFriendPersonaName(
                IntPtr instance,
                ulong steamID
            );

            [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
            private static extern void SteamAPI_ISteamFriends_ActivateGameOverlayToStore(
                IntPtr instance,
                int appid,
                int eFlag
            );

            private bool wasInit;
            private IntPtr steamUserStats;
            private IntPtr steamFriends;

            private static unsafe string UTF8_ToManaged(IntPtr s)
            {
                if (s == IntPtr.Zero)
                {
                    return null;
                }

                /* We get to do strlen ourselves! */
                byte* ptr = (byte*) s;
                while (*ptr != 0)
                {
                    ptr++;
                }

                /* TODO: This #ifdef is only here because the equivalent
                 * .NET 2.0 constructor appears to be less efficient?
                 * Here's the pretty version, maybe steal this instead:
                 *
                string result = new string(
                    (sbyte*) s, // Also, why sbyte???
                    0,
                    (int) (ptr - (byte*) s),
                    System.Text.Encoding.UTF8
                );
                 * See the CoreCLR source for more info.
                 * -flibit
                 */
    #if NETSTANDARD2_0
                /* Modern C# lets you just send the byte*, nice! */
                string result = System.Text.Encoding.UTF8.GetString(
                    (byte*) s,
                    (int) (ptr - (byte*) s)
                );
    #else
                /* Old C# requires an extra memcpy, bleh! */
                int len = (int) (ptr - (byte*) s);
                char* chars = stackalloc char[len];
                int strLen = System.Text.Encoding.UTF8.GetChars((byte*) s, len, chars, len);
                string result = new string(chars, 0, strLen);
    #endif
                return result;
            }

            public ManagedSteam()
            {
                try
                {
                    wasInit = SteamAPI_Init();
                }
                catch
                {
                    wasInit = false;
                }
                if (wasInit)
                {
                    IntPtr steamClient = SteamInternal_CreateInterface(
                        "SteamClient017"
                    );
                    int steamUser = SteamAPI_GetHSteamUser();
                    int steamPipe = SteamAPI_GetHSteamPipe();
                    steamUserStats = SteamAPI_ISteamClient_GetISteamUserStats(
                        steamClient,
                        steamUser,
                        steamPipe,
                        "STEAMUSERSTATS_INTERFACE_VERSION011"
                    );
                    steamFriends = SteamAPI_ISteamClient_GetISteamFriends(
                        steamClient,
                        steamUser,
                        steamPipe,
                        "SteamFriends017"
                    );

                    SteamAPI_ISteamUserStats_RequestCurrentStats(steamUserStats);
                }
            }

            public void Shutdown()
            {
                if (wasInit)
                {
                    SteamAPI_Shutdown();
                }
            }

            public void Update()
            {
                if (wasInit)
                {
                    SteamAPI_RunCallbacks();
                }
            }

            public string GetPersonaName()
            {
                if (wasInit)
                {
                    return UTF8_ToManaged(SteamAPI_ISteamFriends_GetPersonaName(steamFriends));
                }
                return "STEAMUSER";
            }

            public bool SetAchievement(string achievementName)
            {
                if (wasInit)
                {
                    SteamAPI_ISteamUserStats_SetAchievement(steamUserStats, achievementName);
                    bool result = SteamAPI_ISteamUserStats_StoreStats(steamUserStats);
                    SteamAPI_RunCallbacks();
                    return result;
                }
                return false;
            }

            public int GetFriendCount()
            {
                if (wasInit)
                {
                    return SteamAPI_ISteamFriends_GetFriendCount(steamFriends, 4);
                }
                return 0;
            }

            public string GetPersonaNameByFriendIndex(int index)
            {
                if (wasInit)
                {
                    ulong id = SteamAPI_ISteamFriends_GetFriendByIndex(steamFriends, index, 4);
                    return UTF8_ToManaged(SteamAPI_ISteamFriends_GetFriendPersonaName(steamFriends, id));
                }
                return "STEAMFRIEND";
            }

            public void ActivateGameOverlayToStore(int appid)
            {
                if (wasInit)
                {
                    SteamAPI_ISteamFriends_ActivateGameOverlayToStore(steamFriends, appid, 0);
                }
            }
        }

        public static ManagedSteam steam;

        public static string GetSteamName()
        {
            try
            {
                return steam.GetPersonaName();
            }
            catch
            {
                return null;
            }
        }

        public static bool SetAchievement(string achievementName)
        {
            try
            {
                return steam.SetAchievement(achievementName);
            }
            catch
            {
                return false;
            }
        }

        static string[] steamFriendNames;
        public static string[] SteamFriendNames
        {
            get { return steamFriendNames; }
        }

        public static void RefreshSteamFriendArray()
        {
            try
            {
                int numFriends = steam.GetFriendCount();

                steamFriendNames = new string[numFriends];

                for (int i = 0; i < numFriends; i++)
                {
                    steamFriendNames[i] = steam.GetPersonaNameByFriendIndex(i);
                }
            }
            catch
            {
            }
        }
#endif
        

        public FrameworkCore()
            : base()
        {
#if WINDOWS && STEAM
            steam = new ManagedSteam();
#endif

                        

            game = this;

#if LiveEnabled
            Components.Add(new GamerServicesComponent(this));

            //DELETE ME
            //Guide.SimulateTrialMode = true;
#endif

            options = new Options();
            storageManager = new StorageManager();


            highScores = new HighScoreEntry();

            
#if WINDOWS
            OptionsData optionsData = storageManager.LoadOptionsPC();
            options.bloom = optionsData.bloom;
            options.fullscreen = optionsData.isFullscreen;

            options.renderPlanets = optionsData.renderPlanets;
            options.mousewheel = optionsData.mousewheel;
            options.sensitivity = optionsData.sensitivity;
            options.hardwaremouse = optionsData.hardwaremouse;
            options.manualDefault = optionsData.manualDefault;

            options.resolutionX = optionsData.VideoWidth;
            options.resolutionY = optionsData.VideoHeight;

            options.p1UseMouse = optionsData.player1UseMouse;
            options.p2UseMouse = optionsData.player2UseMouse;


            if (options.hardwaremouse)
                Game.IsMouseVisible = true;
#endif



            graphics = new GraphicsDeviceManager(this);
#if XBOX
            if (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width < 1920)
            {
                graphics.PreferredBackBufferHeight = 720;
                graphics.PreferredBackBufferWidth = 1280;
            }
            else
            {
                graphics.PreferredBackBufferHeight = 1080;
                graphics.PreferredBackBufferWidth = 1920;
            }
#elif ONLIVE
            
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.IsFullScreen = true;
#else
            //PC
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;


            graphics.PreferredBackBufferHeight = optionsData.VideoHeight;
            graphics.PreferredBackBufferWidth = optionsData.VideoWidth;
            graphics.IsFullScreen = optionsData.isFullscreen;

            

            
#endif





            particles = new ParticleManager(this);

            contentManager = new ContentManager(Services, "Content");

            boltManager = new BoltManager(this, particles);            
            

            
            bloomComponent = new BloomComponent(this);


            Meshrenderer = new MeshRenderer();
            playerMeshRenderer = new MeshRenderer();

            playbackSystem = new PlaybackSystem();
            worldTextManager = new WorldTextManager(this);
            debrismanager = new DebrisManager();
            hulkmanager = new HulkManager();
            


            audioManager = new AudioManager();

            sysmenumanager = new SysMenuManager();
            level = new Level();

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

                menuInputs[i] = new InputManager(index);
            }

#if WINDOWS
    #if !TRIAL
            //check for full version files.
            isPirated = IsMissingFiles();
    #endif
#endif
        }

        /// <summary>
        /// returns TRUE if the system is missing files pertinent to the full version.
        /// </summary>
        /// <returns></returns>
        private bool IsMissingFiles()
        {
            //BC 3-25-2019 Remove the pirate file check.
            return false;

            if (!System.IO.File.Exists(@"WindowsContent/splash.bmp"))
                return true;

            if (!System.IO.File.Exists(@"Content/textures/cloudbump.xnb"))
                return true;

            if (!System.IO.File.Exists(@"Content/particles/glow.xnb"))
                return true;

            return false;
        }

        static bool isPirated = false;

        protected override void Dispose(bool disposing)
        {
#if (STEAM && WINDOWS)
            steam.Shutdown();
#endif

            //clean up.
            if (FrameworkCore.graphics.GraphicsDevice != null)
                FrameworkCore.graphics.GraphicsDevice.Dispose();

            audiomanager.ClearAll();

            if (soundbank != null)
                soundbank.Dispose();

            if (waveBank != null)
                waveBank.Dispose();

            if (audioEngine != null)
                audioEngine.Dispose();            

            base.Dispose(disposing);
        }


        public static bool isTrialMode()
        {
#if XBOX
            return Guide.IsTrialMode;
#else
    #if TRIAL
            return true;
    #else
            return isPirated;
    #endif
#endif
        }

        public static void DrawTrialMode(GameTime gameTime)
        {
            if (!FrameworkCore.isTrialMode())
                return;

            float trialSize = 0.6f;
            Vector2 trialVec = FrameworkCore.Gothic.MeasureString(Resource.MenuTrialMode);
            trialVec.X *= trialSize;
            trialVec.Y *= trialSize;

            Vector2 trialPos = new Vector2(
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 140 - trialVec.X / 2,
                90 + trialVec.Y);
            trialSize += Helpers.Pulse(gameTime, 0.02f, 4);
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuTrialMode, trialPos,
                new Color(255, 160, 0), new Color(0, 0, 0, 128), 0.1f,
                new Vector2(trialVec.X / 2, trialVec.Y), trialSize);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Initialize()
        {
            linerenderer = new LineRenderer((SpaceShooterGame)game);
            linerenderer.OnCreateDevice();

            discrenderer = new DiscRenderer((SpaceShooterGame)game);
            discrenderer.OnCreateDevice();

            sphererenderer = new SphereRenderer((SpaceShooterGame)game);
            sphererenderer.OnCreateDevice();

            pointrenderer = new PointRenderer(Game.Content.Load<Effect>("shaders/pointeffect"));

            particles.Initialize();
            boltManager.Initialize();


            spriteBatch = new SpriteBatch(GraphicsDevice);

            serif = Game.Content.Load<SpriteFont>(@"fonts\serif");
            serifbig = Game.Content.Load<SpriteFont>(@"fonts\serifbig");
            gothic = Game.Content.Load<SpriteFont>(@"fonts\gothic");
            

            hudsheet = FrameworkCore.content.Load<Texture2D>(@"textures\hudsheet");
            eventsheet = FrameworkCore.content.Load<Texture2D>(@"textures\eventsheet");

                


            modelDebris01 = FrameworkCore.Game.Content.Load<Model>(@"meshes\debris01");
            modelDebris02 = FrameworkCore.Game.Content.Load<Model>(@"meshes\debris02");

            modelAsteroid01 = FrameworkCore.Game.Content.Load<Model>(@"meshes\asteroid01");
            modelAsteroid02 = FrameworkCore.Game.Content.Load<Model>(@"meshes\asteroid02");

            modelBeam01 = FrameworkCore.Game.Content.Load<Model>(@"meshes\beam");

            modelPlanet = FrameworkCore.Game.Content.Load<Model>(@"meshes\planet");
            modelPlanetTiny = FrameworkCore.Game.Content.Load<Model>(@"meshes\planettiny");


            if (modelArray == null)
            {
                int i, j = modelFiles.GetLength(0);
                modelArray = new Model[j];
                for (i = 0; i < j; i++)
                    modelArray[i] = content.Load<Model>(modelFiles[i]);
            }

            //pre load all the textures.
            if (textureArray == null)
            {
                int i, j = modelFiles.GetLength(0);
                textureArray = new Texture2D[j];
                for (i = 0; i < j; i++)
                    textureArray[i] = ((BasicEffect)FrameworkCore.ModelArray[i].Meshes[0].MeshParts[0].Effect).Texture;
            }

            Meshrenderer.Initialize();
            playerMeshRenderer.Initialize();


            meshRenderer.LoadContent();
            playerMeshRenderer.LoadContent();


#if XBOX
            audioEngine = new AudioEngine("XboxContent/xbox/serp6.xgs");
            soundBank = new SoundBank(audioEngine, "XboxContent/xbox/Sound Bank.xsb");
            waveBank = new WaveBank(audioEngine, "XboxContent/xbox/Wave Bank.xwb");
#else
            try
            {
                audioEngine = new AudioEngine("WindowsContent\\win\\serp6.xgs");
                soundBank = new SoundBank(audioEngine, "WindowsContent\\win\\Sound Bank.xsb");
                waveBank = new WaveBank(audioEngine, "WindowsContent\\win\\Wave Bank.xwb");
            }
            catch
            {
#if SDL2
                if (!YesNoPopup.Show(
                    "Audio Error",
                    "There was a problem initializing the audio engine.\n\nTo resolve this:\n1. Right-click on your volume control.\n2. Select \"Playback devices\"\n3. Right-click on \"Digital Output\"\n4. Select \"Set as Default Device\"\n\nDo you want to continue with sound disabled?"))
#else
                if (MessageBox.Show(
                    "There was a problem initializing the audio engine.\n\nTo resolve this:\n1. Right-click on your volume control.\n2. Select \"Playback devices\"\n3. Right-click on \"Digital Output\"\n4. Select \"Set as Default Device\"\n\nDo you want to continue with sound disabled?",
                    "Audio Error", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error) == DialogResult.No)
#endif
                {
                    this.Exit();
                }
            }
#endif

            try
            {
                SoundCategory = audioEngine.GetCategory("Default");
                MusicCategory = audioEngine.GetCategory("Music");
            }
            catch
            {
            }




            SetSoundVolume();
            SetMusicVolume();



            
            bloomComponent.Initialize();


            base.Initialize();
        }

        public static void UpdateVolumes()
        {
            SetSoundVolume();
            SetMusicVolume();
        }

        public static void PlayCue(string cueName)
        {
            try
            {
                soundbank.PlayCue(cueName);
            }
            catch
            {
            }
        }

        private static void SetSoundVolume()
        {
            float adjustedVolume = (float)options.soundVolume / 10.0f;

            try
            {
                SoundCategory.SetVolume(adjustedVolume);
            }
            catch
            {
            }
        }

        private static void SetMusicVolume()
        {
            float adjustedVolume = (float)options.musicVolume / 10.0f;

            try
            {
                MusicCategory.SetVolume(adjustedVolume);
            }
            catch
            {
            }
        }


        protected override void Update(GameTime gameTime)
        {

            if (FrameworkCore.sysmenumanager.menus.Count > 0 || FrameworkCore.mainMenuManager.menus.Count > 0)
            {
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

                    bool clampMouse = true;

#if DEBUG
                    if (FrameworkCore.MainMenuManager.menus.Count > 1)
                    {
                        if (FrameworkCore.MainMenuManager.menus[1].GetType() == typeof(DebugEventsMenu))
                        {
                            clampMouse = false;
                        }
                    }
#endif

                    menuInputs[i].Update(gameTime, index, null, clampMouse);
                }
            }


#if WINDOWS && STEAM

            /*
            if (FrameworkCore.players[0].inputmanager.kbGPressed)
            {
                if (FrameworkCore.players[0].inputmanager.kbSlowCam)
                {
                    steam.ResetStats();
                }
                else
                {
                    FrameworkCore.SetAchievement("ach_hardcore");
                }
            }
             */

            /*
            if (steamTimer <= 0)
            {
                if (FrameworkCore.players[0].inputmanager.activateSteamOverlay)
                {
                    steamTimer = 300;
                    steam.ActivateOverlay();
                }
            }
            else
            {
                steamTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
             */
#endif            



            if (options.bloom)
                bloomComponent.Update(gameTime);

            try
            {
                audioEngine.Update();
            }
            catch
            {
            }
            
            base.Update(gameTime);

#if WINDOWS && STEAM
            steam.Update();
#endif
        }

#if WINDOWS && STEAM
        int steamTimer = 0;
#endif

        public static void ExitToMainMenu(SysMenu menuToAdd)
        {
            if (FrameworkCore.worldMap != null)
                FrameworkCore.worldMap = null;

            FrameworkCore.level.ClearActionMusic();

            foreach (PlayerCommander player in FrameworkCore.players)            
                player.ClearAll();           


            //clear out some spaceships.
            for (int i = 0; i < FrameworkCore.level.Ships.Count; i++)
            {
                //check every other.
                if (i % 2 == 0)
                    continue;

                //only check spaceships.
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[i]))
                    continue;

                if (FrameworkCore.level.Ships[i].IsDestroyed)
                    continue;

                ((SpaceShip)FrameworkCore.level.Ships[i]).ForceKill();
            }


            FrameworkCore.MainMenuManager.ClearAll();
            FrameworkCore.sysMenuManager.ClearAll();
            FrameworkCore.gameState = GameState.Logos;

            FrameworkCore.PlayCue(sounds.Music.raindrops01);
            FrameworkCore.MainMenuManager.AddMenu(new MainMenu());

            Helpers.UpdateCameraProjections(1);

            if (menuToAdd == null)
                return;

            FrameworkCore.MainMenuManager.AddMenu(menuToAdd);

            
        }

        public static void BuyGame()
        {
            BuyGame(FrameworkCore.controllingPlayer);
        }

        public static void BuyGame(PlayerIndex index)
        {

#if LiveEnabled
            if (Guide.IsVisible)
                return;

            if (!Guide.IsTrialMode)
                return;

            SignedInGamer gamer = SignedInGamer.SignedInGamers[index];

            //found signed in player. do stuff..
            if (gamer != null && gamer.Privileges.AllowPurchaseContent)
            {
                try
                {
                    Guide.ShowMarketplace(FrameworkCore.controllingPlayer);
                }
                catch
                {
                }
            }
            else
            {
                //player is attempting to buy game, but profile doesn't have purchasing priviledges.
                SysPopup signPrompt = new SysPopup(FrameworkCore.sysmenumanager, Resource.MenuBuyProfileError);
                signPrompt.sideIconRect = sprite.windowIcon.exclamation;     
                signPrompt.darkenScreen = true;
                signPrompt.transitionOnTime = 200;
                MenuItem item = new MenuItem(Resource.MenuProfileNotSignedinConfirm);
                item.Selected += ChooseProfile;
                signPrompt.AddItem(item);
                item = new MenuItem(Resource.MenuCancel);
                item.Selected += OnDoNotSignin;
                signPrompt.AddItem(item);

                FrameworkCore.sysmenumanager.AddMenu(signPrompt);
            }

            return;
#endif


#if WINDOWS && STEAM
            steam.ActivateGameOverlayToStore( Helpers.STEAMAPPID );
            return;
#endif


#if WINDOWS
            try
            {
                //clear the inputManagers.
                for (int i = 0; i < 4; i++)
                {
                    menuInputs[i].ClearAll();
                }

                if (FrameworkCore.game.IsActive)
                {
                    //open the browser.
                    Process.Start("http://www.blendogames.com/flotilla");

#if SDL2
                    SDL.SDL_MinimizeWindow(FrameworkCore.game.Window.Handle);
#else
                    //minimize the game.
                    Form MyGameForm = (Form)Form.FromHandle(FrameworkCore.game.Window.Handle);
                    MyGameForm.WindowState = FormWindowState.Minimized;
#endif
                }
            }
            catch (Exception ex)
            {
                SysPopup signPrompt = new SysPopup(FrameworkCore.sysmenumanager, "VISIT http://www.blendogames.com FOR MORE INFORMATION!");
                signPrompt.sideIconRect = sprite.windowIcon.exclamation;
                signPrompt.darkenScreen = true;
                signPrompt.transitionOnTime = 200;
                MenuItem item = new MenuItem(Resource.MenuOK);
                item.Selected += CloseMenu;
                signPrompt.AddItem(item);
                FrameworkCore.sysmenumanager.AddMenu(signPrompt);
            }
#endif
        }


        private static void ChooseProfile(object sender, EventArgs e)
        {
            if (Helpers.GuideVisible)
                return;

            //close the popup window.
            Helpers.CloseThisMenu(sender);

#if XBOX
            try
            {
                Guide.ShowSignIn(1, true);
            }
            catch
            {
            }
#endif
        }

        private static void OnDoNotSignin(object sender, EventArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }

        private static void CloseMenu(object sender, EventArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        protected override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }

    }
}