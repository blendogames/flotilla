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

#if WINDOWS
using System.Diagnostics;

#endif

#endregion

namespace SpaceShooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class SpaceShooterGame : FrameworkCore
    {
        public SpaceShooterGame()
        {
            Content.RootDirectory = "Content";
            FrameworkCore.MainMenuManager = new SysMenuManager();
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            FrameworkCore.level.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            FrameworkCore.level.LoadContent();

            FrameworkCore.MainMenuManager.AddMenu(new LogoMenu());

#if XBOX
            SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamerSignedIn);
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedInGamerSignedOut);
#endif

            base.LoadContent();
        }

#if XBOX
        void SignedInGamerSignedIn(object sender, SignedInEventArgs e)
        {
            if (e == null)
                return;

            if (e.Gamer.PlayerIndex != FrameworkCore.ControllingPlayer)
                return;

            KickToTitleScreen();
        }

        void SignedInGamerSignedOut(object sender, SignedOutEventArgs e)
        {
            if (e == null)
                return;

            if (e.Gamer.PlayerIndex != FrameworkCore.ControllingPlayer)
                return;

            KickToTitleScreen();
        }
#endif

        private void KickToTitleScreen()
        {
            if (!FrameworkCore.shouldCheckSignIn)
                return;

            //close down everything. Kick player to the title screen.
            FrameworkCore.shouldCheckSignIn = false;

            if (FrameworkCore.worldMap != null)
                FrameworkCore.worldMap = null;

            FrameworkCore.level.ClearActionMusic();

            foreach (PlayerCommander player in FrameworkCore.players)
                player.ClearAll();

            //remove all players except player zero
            if (FrameworkCore.players.Count > 1)
            {
                for (int x = FrameworkCore.players.Count - 1; x >= 1; x--)
                {
                    FrameworkCore.players.RemoveAt(x);
                }
            }

            Helpers.UpdateCameraProjections(1);
            FrameworkCore.PlayCue(sounds.Music.raindrops01);

            FrameworkCore.gameState = GameState.Logos;



            FrameworkCore.sysMenuManager.CloseAll();
            FrameworkCore.MainMenuManager.CloseAll();
            FrameworkCore.MainMenuManager.AddMenu(new TitleMenu());

            SysPopup signPrompt = new SysPopup(FrameworkCore.MainMenuManager, Resource.MenuProfileSigninChange);
            signPrompt.sideIconRect = sprite.windowIcon.exclamation;
            MenuItem item = new MenuItem(Resource.MenuOK);
            item.Selected += SignInChangeDismiss;
            signPrompt.AddItem(item);

            FrameworkCore.MainMenuManager.AddMenu(signPrompt);
        }

        private void SignInChangeDismiss(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(MenuItem))
            {
                if (((MenuItem)sender).owner != null)
                    ((MenuItem)sender).owner.Deactivate();
            }
        }




        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            FrameworkCore.campaignTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

#if XBOX
            if (Helpers.GuideVisible)
            {
                base.Update(gameTime);
                return;
            }
#endif




#if DEBUG
            if (FrameworkCore.players[0].inputmanager.debugF1Pressed)
            {
                MainMenuManager.AddMenu(new DebugEventsMenu());
            }

            if (FrameworkCore.players[0].inputmanager.debugF2Pressed)
            {
                if (isAutotesting)
                    FrameworkCore.PlayCue(sounds.Fanfare.klaxon);
                else
                    FrameworkCore.PlayCue(sounds.Fanfare.timer);

                isAutotesting = !isAutotesting;
            }



            if (isAutotesting)
                RunAdventureTest(gameTime);

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemTilde))
            {
                this.Exit();
            }
#endif


            foreach (PlayerCommander player in FrameworkCore.players)
            {
                player.UpdateControls(gameTime, (FrameworkCore.gameState != GameState.WorldMap));
            }


            if (FrameworkCore.gameState == GameState.Logos)
            {
                //make sure the demoThink happens before the menus, because
                // we want the menu to have priority over the demoThink routines.

                DemoThink(gameTime);

                if (!FrameworkCore.sysMenuManager.Update(gameTime, FrameworkCore.controllingInputManager))
                {
                    FrameworkCore.MainMenuManager.Update(gameTime, FrameworkCore.controllingInputManager);
                    UpdatePlayer2Controls(gameTime);
                }
            }
            else if (FrameworkCore.gameState == GameState.Play)
            {
                level.Update(gameTime);
            }
            else
            {
                worldMap.Update(gameTime);
            }

            

            base.Update(gameTime);
        }


        #region  AUTO TEST
#if DEBUG
        bool isAutotesting = false;

        int autotestTimer = 0;

        int autoTestRounds = 0;
        int autotestTurnNumber = 0;



        private void RunAdventureTest(GameTime gameTime)
        {
            if (autotestTimer > 0)
            {
                autotestTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                return;
            }

            autotestTimer = 1500;

            if (FrameworkCore.gameState == GameState.Logos)
            {
                //if i'm in main menu
                if (FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1].GetType()
                    == typeof(MainMenu))
                {
                    //go to skirmish menu
                    Console.WriteLine("Going to Adventure menu...");
                    FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1].menuItems[0].OnSelectEntry(PlayerIndex.Four);
                }
                else if (FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1].GetType()
                    == typeof(CampaignMenu))
                {
                    //start skirmish!
                    Console.WriteLine("\n\n\nStarting Adventure " + autoTestRounds);
                    ((CampaignMenu)FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1]).StartLoad();

                    autotestTurnNumber = 0;
                    autoTestRounds++;
                }
            }
            else if (FrameworkCore.gameState == GameState.WorldMap)
            {
                if (FrameworkCore.worldMap.MenuManager.menus.Count <= 0)
                {
                    //click on random planet.
                    if (FrameworkCore.worldMap.worldstate == WorldMap.WorldState.Orders)
                    {
                        Console.WriteLine("Going to planet...");
                        FrameworkCore.worldMap.DebugGotoPlanet();
                    }
                }
                else
                {
                    //an EVENT menu is up.
                    if (FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].GetType() ==
                        typeof(EventPopup))
                    {
                        Console.WriteLine("Selecting event option...");
                        FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].menuItems[0].OnSelectEntry(PlayerIndex.Four);
                    }
                    else if (FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].GetType() ==
                        typeof(ItemPopup))
                    {
                        //ITEM POPUP.
                        Console.WriteLine("Exiting item popup...");
                        FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].menuItems[0].OnSelectEntry(PlayerIndex.Four);
                    }
                    else if (FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].GetType() ==
                        typeof(ShipPopup))
                    {
                        //ITEM POPUP.
                        Console.WriteLine("Exiting ship popup...");
                        FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].menuItems[0].OnSelectEntry(PlayerIndex.Four);
                    }
                    else if (FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].GetType() ==
                        typeof(LogMenu))
                    {
                        //LOG MENU.
                        if (((LogMenu)FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1]).IsGameOver)
                        {
                            Console.WriteLine("Exiting logmenu...");
                            ((LogMenu)FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1]).DebugGameOver();
                        }
                        else
                        {
                            Console.WriteLine("Exiting logmenu (logmenu) ...");
                            FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].Deactivate();
                        }
                    }
                    else if (FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].GetType() ==
                        typeof(GameOverMenu))
                    {
                        //GAME OVER MENU.
                        Console.WriteLine("Exiting GameOverMenu...");
                        FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].menuItems[0].OnSelectEntry(PlayerIndex.Four);
                    }
                    else if (FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].GetType() ==
                        typeof(FleetMenu))
                    {
                        //GAME OVER MENU.
                        Console.WriteLine("Exiting fleetmenu...");
                        FrameworkCore.worldMap.MenuManager.menus[FrameworkCore.worldMap.MenuManager.menus.Count - 1].Deactivate();
                    }
                }
            }
            else
            {
                AutoCombat();
            }
        }

        private void RunAutotest(GameTime gameTime)
        {
            if (autotestTimer > 0)
            {
                autotestTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                return;
            }

            autotestTimer = 1500;
            

            if (FrameworkCore.gameState == GameState.Logos)
            {
                //if i'm in main menu
                if (FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1].GetType()
                    == typeof(MainMenu))
                {
                    //go to skirmish menu
                    Console.WriteLine("Going to skirmish menu...");
                    FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1].menuItems[2].OnSelectEntry(PlayerIndex.Four);
                }
                else if (FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1].GetType()
                    == typeof(SkirmishMenu))
                {
                    //start skirmish!
                    Console.WriteLine("\n\n\nStarting skirmish round " + autoTestRounds);
                    FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1].menuItems[4].OnSelectEntry(PlayerIndex.Four);

                    autotestTurnNumber = 0;
                    autoTestRounds++;
                }
            }
            else
            {
                AutoCombat();
            }
        }

        private void AutoCombat()
        {
            if (FrameworkCore.level.gamemode == GameMode.Orders)
            {
                if (FrameworkCore.players[0].ActiveMenu == null)
                {
                    //open the commandmenu.
                    Console.WriteLine("opening commandmenu.....");
                    FrameworkCore.players[0].ActivateCommandMenu();
                }
                else if (FrameworkCore.players[0].ActiveMenu.GetType() == typeof(CommandMenu))
                {
                    autotestTurnNumber++;
                    Console.WriteLine("selecting endturn " + autotestTurnNumber);
                    FrameworkCore.players[0].ActiveMenu.menuItems[0].OnSelectEntry(PlayerIndex.Four);
                }
            }
            else if (FrameworkCore.level.gamemode == GameMode.CarnageReport)
            {
                //exit the carnage report.
                Console.WriteLine("exiting carnage report.....");
                if (FrameworkCore.level.LevelMenuManager.menus.Count >= 1)
                    FrameworkCore.level.LevelMenuManager.menus[0].Deactivate();
            }
            else
            {
                //emergency abort after 10 minutes of combat.
                if (FrameworkCore.playbackSystem.WorldTimer >= 58000)
                {
                    Console.WriteLine("aborting round!");

                    for (int i = 0; i < FrameworkCore.level.Ships.Count; i++)
                    {
                        if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[i]))
                            continue;

                        if (FrameworkCore.level.Ships[i].owner == null)
                            continue;

                        if (FrameworkCore.level.Ships[i].owner.GetType() == typeof(PlayerCommander))
                            continue;

                        ((SpaceShip)FrameworkCore.level.Ships[i]).ForceKill();
                    }
                }
            }
        }
#endif
        #endregion


        private void DemoThink(GameTime gameTime)
        {
            if (FrameworkCore.MainMenuManager != null)
            {
                if (FrameworkCore.MainMenuManager.menus.Count > 1)
                {
                    if (FrameworkCore.MainMenuManager.menus[FrameworkCore.MainMenuManager.menus.Count - 1].GetType() ==
                        typeof(SellScreen))
                    {
                        return;
                    }
                }
            }           

            FrameworkCore.level.DemoThink(gameTime);
        }

        private void UpdatePlayer2Controls(GameTime gameTime)
        {
            if (FrameworkCore.players.Count <= 1 || FrameworkCore.players[1] == null)
                return;

            //urg we only want to give player2 menu access if the skirmish menu is active.
            if (!FrameworkCore.MainMenuManager.SkirmishInStack())
                return;

            FrameworkCore.MainMenuManager.UpdateTopControls(gameTime, FrameworkCore.players[1].inputmanager);
        }
 


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (options.bloom)
            {
                Bloomcomponent.SetResolveTarget();
            }

            // Clear screen to black
            FrameworkCore.Graphics.GraphicsDevice.Clear(OldXNAColor.TransparentBlack);



            if (FrameworkCore.gameState == GameState.WorldMap)
                FrameworkCore.worldMap.Draw(gameTime);
            else
            {
                FrameworkCore.level.Draw(gameTime);

                if (FrameworkCore.gameState == GameState.Logos)
                {
                    FrameworkCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                    FrameworkCore.MainMenuManager.Draw(gameTime);


                    Helpers.DrawSystemMenu(gameTime);


                    FrameworkCore.SpriteBatch.End();
                }
            }



#if PREVIEW
            FrameworkCore.SpriteBatch.Begin();
            Vector2 previewPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 30);
            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, "DEVELOPMENT BUILD 022210",
                previewPos, Color.Gray, 0.9f);
            FrameworkCore.SpriteBatch.End();
#endif


            DrawBrightness();



            if (options.bloom)
            {
                Bloomcomponent.DrawResolveTarget();
            }
        }

        private void DrawBrightness()
        {
            if (FrameworkCore.options.brightness == 5)
                return;

            //Ghetto Brightness settings.
            if (FrameworkCore.options.brightness > 5)
            {
                float adjustedBrightness = FrameworkCore.options.brightness - 5;
                adjustedBrightness /= 5;
                int alpha = (int)MathHelper.Lerp(1, 64, adjustedBrightness);

                FrameworkCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                BlendStateHelper.BeginApply(FrameworkCore.Graphics.GraphicsDevice);
                BlendStateHelper.SourceBlend = Blend.One;
                BlendStateHelper.DestinationBlend = Blend.One;
                BlendStateHelper.EndApply(FrameworkCore.Graphics.GraphicsDevice);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                    new Rectangle(0, 0, (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                        (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height),
                    sprite.blank, new Color(255, 255, 255, (byte)alpha));

                FrameworkCore.SpriteBatch.End();
            }
            else if (FrameworkCore.options.brightness < 5)
            {
                float adjustedBrightness = FrameworkCore.options.brightness;
                adjustedBrightness /= 4;
                int alpha = (int)MathHelper.Lerp(128, 32, adjustedBrightness);

                FrameworkCore.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

                BlendStateHelper.BeginApply(FrameworkCore.Graphics.GraphicsDevice);
                BlendStateHelper.SourceBlend = Blend.DestinationColor;
                BlendStateHelper.DestinationBlend = Blend.Zero;
                BlendStateHelper.EndApply(FrameworkCore.Graphics.GraphicsDevice);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                    new Rectangle(0, 0, (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                        (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height),
                    sprite.blank, new Color(0, 0, 0, (byte)alpha));

                FrameworkCore.SpriteBatch.End();
            }
        }

    }


    #region Entry Point

    

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if SDL2
            string baseFolder = GetStorageRoot();
            string oldFolder = System.IO.Path.Combine(baseFolder, "AllPlayers");
            string newFolder = System.IO.Path.Combine(baseFolder, "Flotilla", "AllPlayers");
            if (    System.IO.Directory.Exists(oldFolder) &&
                    !System.IO.Directory.Exists(newFolder)  )
            {
                try
                {
                    Console.Write("MIGRATING XNA3 SAVES TO XNA4...");
                    System.IO.Directory.CreateDirectory(newFolder);
                    System.IO.File.Copy(
                        System.IO.Path.Combine(oldFolder, "saveinfo.dat"),
                        System.IO.Path.Combine(newFolder, "saveinfo.dat")
                    );
                    System.IO.File.Copy(
                        System.IO.Path.Combine(oldFolder, "scores.dat"),
                        System.IO.Path.Combine(newFolder, "scores.dat")
                    );
                    System.IO.File.Copy(
                        System.IO.Path.Combine(oldFolder, "settings.xml"),
                        System.IO.Path.Combine(newFolder, "settings.xml")
                    );
                    Console.WriteLine(" COMPLETE!");
                }
                catch
                {
                    SDL2.SDL.SDL_ShowSimpleMessageBox(
                        SDL2.SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
                        "XNA3->XNA4 Migration Failed!",
                        "We just tried to copy your old XNA3 saves over to the new location,\n" +
                        "but something caused it to fail. We don't know what though.\n\n" +
                        "To migrate by hand, go to this folder...\n\n" + baseFolder + "\n\n" +
                        "... make an extra \"Flotilla\" folder inside, then move the \"AllPlayers\"\n" +
                        "folder inside that extra folder. Your old save data should load after that!",
                        IntPtr.Zero
                    );
                }
            }
#endif

#if WINDOWS
#if !DEBUG
            try
            {
#endif
                using (SpaceShooterGame game = new SpaceShooterGame())
                {
                    game.Run();
                }
#if !DEBUG
            }
            catch (Exception e)
            {
                string userVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() +
                    "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();

                string registered = string.Empty;
                if (System.IO.File.Exists(@"WindowsContent/splash.bmp"))
                    registered = "Registered";
                else
                    registered = "Demo";

                string finalString = registered + " version " + userVersion + "\n\n" + e.ToString();

#if SDL2
                Console.WriteLine(finalString);
                if (YesNoPopup.Show("Flotilla Critical Error", "Flotilla has encountered a catastrophic error:\n\n" + finalString + "\n\n\nSend this crash report to BlendoGames.com?"))
                {
                    SendCrashReport(finalString);
                }
#else
                if (MessageBox.Show(
                    "Flotilla has encountered a catastrophic error:\n\n" + finalString + "\n\n\nSend this crash report to BlendoGames.com?",
                    "Flotilla Critical Error", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    SendCrashReport(finalString);
                }
#endif
            }
#endif
#else

            using (SpaceShooterGame game = new SpaceShooterGame())
            {
                game.Run();                
            }
#endif
        }

        private static void SendCrashReport(string assertText)
        {
#if WINDOWS
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                message.From = new System.Net.Mail.MailAddress("test@test.com");
                message.To.Add(new System.Net.Mail.MailAddress("bugreport@blendogames.com"));

                message.Subject = "[FLOTILLA CRASH]";
                message.Body = assertText;

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Host = "smtp.gmail.com"; //smtp server                    
                client.Port = 587; //Port for TLS/STARTTLS
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("bugreport@blendogames.com", "");

                client.Send(message);
            }
            catch
            {
            }
#endif
        }

#if SDL2
        private static string GetStorageRoot()
        {
            // Generate the path of the game's savefolder
            string exeName = System.IO.Path.GetFileNameWithoutExtension(
                AppDomain.CurrentDomain.FriendlyName
            ).Replace(".vshost", "");

            // Get the OS save folder, append the EXE name
            string OSVersion = SDL2.SDL.SDL_GetPlatform();
            if (OSVersion.Equals("Windows"))
            {
                return System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SavedGames",
                    exeName
                );
            }
            if (OSVersion.Equals("Mac OS X"))
            {
                string osConfigDir = Environment.GetEnvironmentVariable("HOME");
                if (String.IsNullOrEmpty(osConfigDir))
                {
                    return "."; // Oh well.
                }
                return System.IO.Path.Combine(
                    osConfigDir,
                    "Library/Application Support",
                    exeName
                );
            }
            if (    OSVersion.Equals("Linux") ||
                    OSVersion.Equals("FreeBSD") ||
                    OSVersion.Equals("OpenBSD") ||
                    OSVersion.Equals("NetBSD")  )
            {
                // Assuming a non-macOS Unix platform will follow the XDG. Which it should.
                string osConfigDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                if (String.IsNullOrEmpty(osConfigDir))
                {
                    osConfigDir = Environment.GetEnvironmentVariable("HOME");
                    if (String.IsNullOrEmpty(osConfigDir))
                    {
                        return ".";    // Oh well.
                    }
                    osConfigDir += "/.local/share";
                }
                return System.IO.Path.Combine(osConfigDir, exeName);
            }

            /* There is a minor inaccuracy here: SDL_GetPrefPath
             * creates the directories right away, whereas XNA will
             * only create the directory upon creating a container.
             * So if you create a StorageDevice and hit a property,
             * the game folder is made early!
             * -flibit
             */
            return SDL2.SDL.SDL_GetPrefPath(null, exeName);
        }
#endif
    }

    

    #endregion
}
