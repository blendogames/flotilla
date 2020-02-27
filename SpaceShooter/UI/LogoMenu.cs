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
using Microsoft.Xna.Framework.Storage;


#if WINDOWS
#if SDL2
using SDL2;
#else
using System.Windows.Forms;
using Microsoft.Win32;
#endif
using System.Net;
using System.Diagnostics;
using System.Globalization;
#endif

#endregion

namespace SpaceShooter
{
    /// <summary>
    /// Splash logo.
    /// </summary>
    public class LogoMenu : SysMenu
    {
        public LogoMenu()
        {
            transitionOnTime = 500;
            transitionOffTime = 300;

            canBeExited = false;           
        }


        private float sizeTimer = 0;

        private bool hasPlayedMusic = false;

        public bool hasLoadedStorage = false;




        void LoadHighScoresCallback(IAsyncResult result)
        {
            StorageDevice highScoreDevice = null;

            if (result != null && result.IsCompleted)
            {
                try
                {
                    highScoreDevice = StorageDevice.EndShowSelector(result);
                }
                catch
                {
                }
            }

            if (highScoreDevice != null && highScoreDevice.IsConnected)
            {
                FrameworkCore.storagemanager.SetHighScoreDevice(highScoreDevice);
                FrameworkCore.highScores = FrameworkCore.storagemanager.LoadHighScores();
            }
        }


        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (!hasLoadedStorage && !Helpers.GuideVisible)
            {
                try
                {
                    hasLoadedStorage = true;
                    StorageDevice.BeginShowSelector(new AsyncCallback(LoadHighScoresCallback), null);
                }
                catch
                {
                    hasLoadedStorage = false;
                }
                return;
            }
            

            sizeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (sizeTimer < 0.3f)
                return;

            if (!hasPlayedMusic)
            {
                hasPlayedMusic = true;
                FrameworkCore.PlayCue(sounds.Music.raindrops01);                
            }

            if (Transition >= 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (FrameworkCore.MenuInputs[i].buttonAPressed ||
                        FrameworkCore.MenuInputs[i].buttonStartPressed)
                    {
                        DoneTitle();
                    }
                }


                if (inputManager.buttonAPressed || inputManager.buttonStartPressed
                    || inputManager.kbSkipScreen
                    || inputManager.mouseLeftClick ||
                    inputManager.kbEscPressed)
                    DoneTitle();
            }


            if (sizeTimer >= 7)
                DoneTitle();
            

            base.Update(gameTime, inputManager);
        }





        bool titleIsDone = false;
        private void DoneTitle()
        {
            if (titleIsDone)
                return;

            titleIsDone = true;

            Deactivate();

            if (Owner != null)
                Owner.AddMenu(new TitleMenu());
        }








        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {
#if WINDOWS && !STEAM
            CheckNewestVersion();
#endif

            base.Activate();
        }
#if WINDOWS
        private void CheckNewestVersion()
        {
            //demo doesn't auto update.
            if (FrameworkCore.isTrialMode())
                return;

            //OPEN THE TXT FILE THAT HAS THE NEWEST FLOTILLA VERSION.
            string url = "http://www.blendogames.com/flotilla/version.txt";
            string result = null;

            try
            {
                WebClient client = new WebClient();
                result = client.DownloadString(url);

                HandlePatchCheck(result);
            }
            catch //(Exception ex)
            {
                //handle error
                //Console.WriteLine(ex.Message);
            }
        }

        private void HandlePatchCheck(string result)
        {

            if (result == null)
                return;

            //convert string to float.
            float newVersion = (float)Convert.ToDouble(result, CultureInfo.InvariantCulture);

            string strUserVersion = FrameworkCore.VERSION;
            float userVersion = (float)Convert.ToDouble(strUserVersion, CultureInfo.InvariantCulture);

            //compare version numbers.
            if (newVersion <= userVersion)
                return;

            //new version is available.
            string patchString = string.Format(Resource.MenuPatchAvailable, FrameworkCore.VERSION, result);

            SysPopup signPrompt = new SysPopup(Owner, patchString);
            signPrompt.canBeExited = false;
            signPrompt.darkenScreen = true;
            signPrompt.sideIconRect = sprite.windowIcon.exclamation;
            MenuItem item = new MenuItem(Resource.MenuPatchGet);
            item.Selected += OnPatch;
            signPrompt.AddItem(item);
            item = new MenuItem(Resource.MenuPatchLater);
            item.Selected += OnNoPatch;
            signPrompt.AddItem(item);

            Owner.AddMenu(signPrompt);

        }

        private void OnPatch(object sender, EventArgs e)
        {
            try
            {
                //Helpers.CloseThisMenu(sender);

                //OPEN THE WEB PAGE THAT HAS THE FLOTILLA PATCHES.
                Process process = Process.Start("http://www.blendogames.com/flotilla/help.htm");

                if (process != null)
                {
                    FrameworkCore.Game.Exit();
                }
            }
            catch
            {
#if SDL2
                SDL.SDL_ShowSimpleMessageBox(
                    SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION,
                    "Flotilla Patch",
                    "Visit http://www.blendogames.com/flotilla for Flotilla patches.",
                    IntPtr.Zero
                );
#else
                MessageBox.Show(
                    "Visit http://www.blendogames.com/flotilla for Flotilla patches.",
                    "Flotilla patch", MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
#endif
            }
        }

        private void OnNoPatch(object sender, EventArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }
#endif

        public override void Draw(GameTime gameTime)
        {
            Vector2 screenSize = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);


            int backAlpha = 255;

            if (Transition < 1 && menuState == MenuState.TransitionOff)
                backAlpha = (int)MathHelper.Lerp(0, 255, Transition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y),
                sprite.blank, new Color(255, 255, 255, (byte)backAlpha));



            float size = 1 + sizeTimer * 0.03f;



            Vector2 logoPos = new Vector2(screenSize.X / 2, screenSize.Y / 2);
            logoPos.Y -= 40;


            Color logoColor = new Color(207, 77, 41);
            logoColor = Color.Lerp(Helpers.transColor(logoColor), logoColor, Transition);
            Vector2 logoVec = Helpers.SpriteCenter(sprite.blendo);
            logoVec.Y *= size;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, logoPos, sprite.blendo, logoColor, 0,
                logoVec, size, SpriteEffects.None, 0);


            Color gameColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, Transition);
            Vector2 gamePos = logoPos + new Vector2(logoVec.X, logoVec.Y);
            gamePos.X -= Helpers.SpriteCenter(sprite.games).X;
            gamePos.Y += 16;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, gamePos, sprite.games, gameColor, 0,
                Helpers.SpriteCenter(sprite.games), size, SpriteEffects.None, 0);


            Color presentsColor = new Color(64, 64, 64);
            presentsColor = Color.Lerp(Helpers.transColor(presentsColor), presentsColor, Transition);
            Vector2 presentsVec = Helpers.stringCenter(FrameworkCore.Serif, Resource.MenuTitlePresents);            
            Vector2 presentsPos = new Vector2(screenSize.X / 2, screenSize.Y - (screenSize.Y * 0.2f));            
            presentsPos.X += (sizeTimer * 32f);
            presentsPos.Y -= (sizeTimer * 4f);
            
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuTitlePresents,
                presentsPos, presentsColor, 0,
                presentsVec, size * 1.3f, SpriteEffects.None, 0);
        }

        

    }
}
