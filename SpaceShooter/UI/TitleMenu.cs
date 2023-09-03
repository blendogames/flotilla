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

using Microsoft.Xna.Framework.Storage;
using System.IO;
#if XBOX

#endif
#endregion

namespace SpaceShooter
{
    public class TitleMenu : SysMenu
    {
        bool checkControllingPlayer = false;

        public TitleMenu()
        {
            transitionOnTime = 600;
            transitionOffTime = 300;

            canBeExited = false;           
        }
        
        
        public override void Update(GameTime gameTime, InputManager inputManager)
        {

#if LiveEnabled
            if (Transition >= 1 && !checkControllingPlayer)
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

                    if (FrameworkCore.MenuInputs[i].buttonAPressed ||
                        FrameworkCore.MenuInputs[i].buttonStartPressed)
                    {
                        checkControllingPlayer = true;
                        FrameworkCore.ControllingPlayer = index;
                        FrameworkCore.players[0].playerindex = index;
                    }
                }
            }

            if (Helpers.GuideVisible)
                return;

            if (checkControllingPlayer)
            {
                SignedInGamer gamer = SignedInGamer.SignedInGamers[FrameworkCore.ControllingPlayer];

                if (gamer != null)
                {
                    FrameworkCore.shouldCheckSignIn = true;
                    ChooseStorageDevice();
                }
                else
                {
                    //controllingplayer is not signed in.

                    if (Owner != null)
                    {
                        SysPopup signPrompt = new SysPopup(Owner, Resource.MenuProfileNotSignedin);
                        signPrompt.canBeExited = false;
                        signPrompt.darkenScreen = true;
                        signPrompt.sideIconRect = sprite.windowIcon.exclamation;
                        MenuItem item = new MenuItem(Resource.MenuProfileNotSignedinConfirm);
                        item.Selected += ChooseProfile;
                        signPrompt.AddItem(item);
                        item = new MenuItem(Resource.MenuProfileNotSignedinIgnore);
                        item.Selected += OnDoNotSignin;
                        signPrompt.AddItem(item);

                        Owner.AddMenu(signPrompt);
                    }
                }
            }
#else
            //PC build.

            if (Transition >= .5f)
            {
                if (inputManager.buttonAPressed || inputManager.kbSkipScreen || inputManager.mouseLeftClick || inputManager.kbEscPressed)
                {
                    OpenMainMenu();
                }
            }
#endif



            base.Update(gameTime, inputManager);
        }

        private void OnChooseStorageDevice(object sender, EventArgs e)
        {            
            ChooseStorageDevice();
        }

        private void ChooseStorageDevice()
        {
            if (!Helpers.GuideVisible)
            {
                try
                {
                    StorageDevice.BeginShowSelector(FrameworkCore.ControllingPlayer,
                        new AsyncCallback(DeviceCallback), null);
                    checkControllingPlayer = false;
                }
                catch
                {
                }
            }
        }

        private void ChooseProfile(object sender, EventArgs e)
        {
            if (Helpers.GuideVisible)
                return;

            Helpers.CloseThisMenu(sender);
            /*
            //close the popup window.
            if (sender.GetType() == typeof(MenuItem))
            {
                if (((MenuItem)sender).owner != null)
                    ((MenuItem)sender).owner.Deactivate();
            }*/

#if XBOX
            try
            {
                Guide.ShowSignIn(1, false);
            }
            catch
            {
            }
#endif
        }

        

        private void DeviceCallback(IAsyncResult result)
        {
            try
            {
                StorageDevice deviceTemp = StorageDevice.EndShowSelector(result);

                if (deviceTemp != null && deviceTemp.IsConnected)
                {
                    //a storage device WAS chosen.
                    FrameworkCore.storagemanager.SetDevice(deviceTemp);
                    OpenMainMenu();
                }
                else
                {
                    //device was NOT chosen.
                    SysPopup devicePrompt = new SysPopup(Owner, Resource.SysDeviceChoose);
                    devicePrompt.sideIconRect = sprite.windowIcon.exclamation;
                    MenuItem item = new MenuItem(Resource.SysDeviceChooseConfirm);
                    item.Selected += OnChooseStorageDevice;
                    devicePrompt.AddItem(item);
                    item = new MenuItem(Resource.SysDeviceChooseIgnore);
                    item.Selected += OnNoStorageDevice;
                    devicePrompt.AddItem(item);
                    Owner.AddMenu(devicePrompt);
                }
            }
            catch
            {
            }
        }

        private void OnNoStorageDevice(object sender, EventArgs e)
        {
            FrameworkCore.storagemanager.SetDevice(null);
            OpenMainMenu();
        }

        /// <summary>
        /// Player chooses to not sign in. Skip player to the main menu. Don't choose storage device,
        /// since player cannot save any information w/o a signed-in profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDoNotSignin(object sender, EventArgs e)
        {
            OpenMainMenu();
        }


        bool hasOpenedMainMenu = false;

        private void OpenMainMenu()
        {
            if (hasOpenedMainMenu)
                return;

            hasOpenedMainMenu = true;

            if (Owner != null)
            {
#if XBOX
                SignedInGamer gamer = SignedInGamer.SignedInGamers[FrameworkCore.ControllingPlayer];

                Owner.AddMenu(new MainMenu());
                if (gamer != null)
                {
                    //player is signed in. load player's data.
                    FrameworkCore.players[0].commanderName = gamer.Gamertag;
                    LoadData();
                }
                else
                {
                    //unsigned player. assign random name.
                    FrameworkCore.players[0].commanderName = Helpers.GenerateName("Gamertag");
                }
#elif (WINDOWS && STEAM)
                
                //the steam build uses the player's Steam persona name.
                //if there's an error for whatever reason, we let the player type in any name.

                LoadData();

                string playerName = FrameworkCore.GetSteamName();

                if (playerName == null)
                {
                    FrameworkCore.players[0].commanderName = Helpers.GenerateName("Gamertag");
                    Owner.AddMenu(new NameMenu());
                }
                else
                {
                    FrameworkCore.players[0].commanderName = playerName;
                    Owner.AddMenu(new MainMenu());
                }                

#else
                SaveInfo optionsData = LoadData();
                
                if (optionsData.playerName == null)
                    FrameworkCore.players[0].commanderName = Helpers.GenerateName("Gamertag");
                else
                    FrameworkCore.players[0].commanderName = optionsData.playerName;

                Owner.AddMenu(new NameMenu());
#endif
            }

            Deactivate();
        }

        private SaveInfo LoadData()
        {
            SaveInfo optionsData = FrameworkCore.storagemanager.LoadData();
            FrameworkCore.adventureNumber = optionsData.adventure;
            FrameworkCore.options.soundVolume = optionsData.volume;
            FrameworkCore.options.musicVolume = optionsData.music;
            FrameworkCore.options.brightness = optionsData.brightness;
            FrameworkCore.skirmishShipArray = optionsData.skirmishArray;
            FrameworkCore.UpdateVolumes();
            FrameworkCore.options.p1InvertY = optionsData.p1InvertY;
            FrameworkCore.options.p1InvertX = optionsData.p1InvertX;
            FrameworkCore.options.p1Vibration = optionsData.p1vibration;

            FrameworkCore.options.p2InvertY = optionsData.p2InvertY;
            FrameworkCore.options.p2InvertX = optionsData.p2InvertX;
            FrameworkCore.options.p2Vibration = optionsData.p2vibration;

            return optionsData;
        }

        

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            FrameworkCore.shouldCheckSignIn = true;
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
            Vector2 screenSize = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);


            Vector2 titleVec = FrameworkCore.Gothic.MeasureString(Resource.MenuTitle);
            titleVec.X /= 2;
            Vector2 titlePos = new Vector2(screenSize.X / 2, 110 + titleVec.Y);
            float titleSize = MathHelper.Lerp(1.2f, 1.4f, Transition);

            Color whiteColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color shadeColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), Transition);

            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuTitle,titlePos,
                whiteColor, shadeColor,
                0, titleVec, titleSize);

            float subtitleSize = MathHelper.Lerp(0.8f, 1.1f, Transition);
            Vector2 subtitleVec = FrameworkCore.Serif.MeasureString(Resource.MenuSubtitle);
            subtitleVec.X /= 2;
            subtitleVec.Y = 0;
            Vector2 subtitlePos = titlePos + new Vector2(0, -15);

            Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuSubtitle, subtitlePos,
                whiteColor, shadeColor,
                0, subtitleVec, subtitleSize);

            if (FrameworkCore.isTrialMode())
            {
                Color trialColor = Color.Lerp(Helpers.transColor(Color.Orange), Color.Orange, Transition);
                Vector2 trialPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                    subtitlePos.Y + subtitleVec.Y + 64);
                trialPos.Y += MathHelper.Lerp(-40, 0, Transition);
                float trialSize = MathHelper.Lerp(0.3f, 0.6f, Transition);
                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Gothic,
                    Resource.MenuTrialMode, trialPos, trialColor, shadeColor, trialSize, 0);
            }




            Vector2 startPos = new Vector2(screenSize.X / 2, screenSize.Y - (screenSize.Y * 0.15f));
            startPos.Y += Helpers.Pulse(gameTime, 3, 4);

            startPos.Y += MathHelper.Lerp(90, 0, Transition);

            float presentsSize = MathHelper.Lerp(1.1f, 1, 0.5f + Helpers.Pulse(gameTime, 0.49f, 4));

            if (Helpers.GuideVisible)
                return;

            string startText = 
#if XBOX
                Resource.MenuTitlePressStart;
#else
                Resource.MenuClickToStart;
#endif

            Vector2 startVec = Helpers.stringCenter(FrameworkCore.Serif, startText);

            Helpers.DrawOutline(FrameworkCore.Serif, startText,
                startPos,
                whiteColor, shadeColor,
                0, startVec, presentsSize);
        }

    }
}
