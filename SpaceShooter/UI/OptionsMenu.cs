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
    public class OptionsMenu : SysMenu
    {

        int WINDOWWIDTH = 550;

        public OptionsMenu()
        {
            int sectionGap = 14;


            darkenScreen = true;
            int windowHeight = 500;


            Vector2 pos = Vector2.Zero;
            pos.X = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - (WINDOWWIDTH / 2);
            pos.Y = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - (windowHeight/2);

            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");
            pos.Y += titleVec.Y;

            pos.Y = Math.Max(100, pos.Y);
            

            

            MenuItem item = new MenuItem(Resource.MenuOptionsSound);
            item.sliderDecrease += OnSoundVolumeDecrease;
            item.sliderIncrease += OnSoundVolumeIncrease;
            item.Selected += OnSoundVolumeIncrease;
            item.itemType = MenuItem.menuItemType.slider;
            item.position = pos;
            item.setOptionText += OnSoundSetText;
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuOptionsMusic);
            item.sliderDecrease += OnMusicVolumeDecrease;
            item.sliderIncrease += OnMusicVolumeIncrease;
            item.setOptionText += OnMusicSetText;
            item.Selected += OnMusicVolumeIncrease;
            item.itemType = MenuItem.menuItemType.slider;
            item.position = pos;
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuOptionsBrightness);
            item.sliderDecrease += OnBrightnessDecrease;
            item.sliderIncrease += OnBrightnessIncrease;
            item.setOptionText += OnBrightnessSetText;
            item.Selected += OnBrightnessIncrease;
            item.itemType = MenuItem.menuItemType.slider;
            item.position = pos;
            base.AddItem(item);

            pos.Y += GetGapSize();
            pos.Y += sectionGap;

            //========= PLAYER 1 OPTIONS

            item = new MenuItem(Resource.MenuOptionsInvertY);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnP1InvertY;
            item.optionBool = FrameworkCore.options.p1InvertY;
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuOptionsInvertX);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnP1InvertX;
            item.optionBool = FrameworkCore.options.p1InvertX;
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuOptionsVibration);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnP1Vibration;
            item.optionBool = FrameworkCore.options.p1Vibration;
            base.AddItem(item);

            pos.Y += GetGapSize();
            pos.Y += sectionGap;


            //========= PLAYER 2 OPTIONS

            item = new MenuItem(Resource.MenuOptionsInvertY);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnP2InvertY;
            item.optionBool = FrameworkCore.options.p2InvertY;
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuOptionsInvertX);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnP2InvertX;
            item.optionBool = FrameworkCore.options.p2InvertX;
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuOptionsVibration);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnP2Vibration;
            item.optionBool = FrameworkCore.options.p2Vibration;
            base.AddItem(item);


            pos.Y += GetGapSize();
            pos.Y += sectionGap;



#if WINDOWS

            item = new MenuItem(Resource.MenuVideo);
            item.position = pos;
            item.Selected += OnVideo;
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuControls);
            item.position = pos;
            item.Selected += OnControls;
            base.AddItem(item);

            pos.Y += GetGapSize();

#if OLDJOYSTICK
            item = new MenuItem(Resource.MenuJoystick);
            item.position = pos;
            item.Selected += OnJoystick;
            base.AddItem(item);

            pos.Y += GetGapSize();
#endif
#endif

            

            item = new MenuItem(Resource.MenuDone);
            item.position = pos;
            item.Selected += OnDone;
            base.AddItem(item);

            foreach (MenuItem k in menuItems)
            {
                k.OnSetText();
            }

            float longestString = 0;
            foreach (MenuItem x in menuItems)
            {
                Vector2 stringLength = FrameworkCore.SerifBig.MeasureString(x.text);
                if (stringLength.X > longestString)
                    longestString = stringLength.X;
            }

            foreach (MenuItem z in menuItems)
            {
                z.optionPos = z.position + new Vector2(longestString, 0);
                z.optionPos += new Vector2(128, 0);
            }


            RepositionItems();


        }

        /*
        public override void RepositionItems()
        {


            base.RepositionItems();
        }
        */
        private void OnVideo(object sender, EventArgs e)
        {
            Owner.AddMenu(new VideoMenu());
        }

        private void OnControls(object sender, EventArgs e)
        {
            Owner.AddMenu(new ControlsMenu());
        }

#if OLDJOYSTICK
        private void OnJoystick(object sender, EventArgs e)
        {
            Owner.AddMenu(new JoystickMenu());
        }
#endif

        private void OnCredits(object sender, EventArgs e)
        {
            Owner.AddMenu(new CreditsMenu(false));
        }

        private void OnDone(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }



        //============ INVERT Y
        private void OnP1InvertY(object sender, EventArgs e)
        {
            FrameworkCore.options.p1InvertY = !FrameworkCore.options.p1InvertY;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.p1InvertY;
        }

        //============ INVERT Y
        private void OnP2InvertY(object sender, EventArgs e)
        {
            FrameworkCore.options.p2InvertY = !FrameworkCore.options.p2InvertY;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.p2InvertY;
        }






        //============ INVERT X
        private void OnP1InvertX(object sender, EventArgs e)
        {
            FrameworkCore.options.p1InvertX = !FrameworkCore.options.p1InvertX;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.p1InvertX;
        }

        //============ INVERT X
        private void OnP2InvertX(object sender, EventArgs e)
        {
            FrameworkCore.options.p2InvertX = !FrameworkCore.options.p2InvertX;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.p2InvertX;
        }


        //============ VIBRATION
        private void OnP1Vibration(object sender, EventArgs e)
        {
            FrameworkCore.options.p1Vibration = !FrameworkCore.options.p1Vibration;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.p1Vibration;
        }

        //============ VIBRATION
        private void OnP2Vibration(object sender, EventArgs e)
        {
            FrameworkCore.options.p2Vibration = !FrameworkCore.options.p2Vibration;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.p2Vibration;
        }




        //============= SOUND VOLUME
        private void OnSoundSetText(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionText = FrameworkCore.options.soundVolume.ToString();
        }

        private void OnSoundVolumeDecrease(object sender, EventArgs e)
        {
            FrameworkCore.options.soundVolume = UpdateGenericSlider(sender,
                FrameworkCore.options.soundVolume, -1);

            base.UpdateItemText(sender);
            float adjustedVolume = (float)FrameworkCore.options.soundVolume / ((MenuItem)sender).maxValue;
            FrameworkCore.soundCategory.SetVolume(adjustedVolume);
        }

        private void OnSoundVolumeIncrease(object sender, EventArgs e)
        {
            FrameworkCore.options.soundVolume = UpdateGenericSlider(sender,
                FrameworkCore.options.soundVolume, 1);

            base.UpdateItemText(sender);
            float adjustedVolume = (float)FrameworkCore.options.soundVolume / ((MenuItem)sender).maxValue;
            FrameworkCore.soundCategory.SetVolume(adjustedVolume);
        }



        //============= MUSIC VOLUME
        private void OnMusicSetText(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionText = FrameworkCore.options.musicVolume.ToString();
        }

        private void OnMusicVolumeDecrease(object sender, EventArgs e)
        {
            FrameworkCore.options.musicVolume = UpdateGenericSlider(sender,
                FrameworkCore.options.musicVolume, -1);

            base.UpdateItemText(sender);
            float adjustedVolume = (float)FrameworkCore.options.musicVolume / ((MenuItem)sender).maxValue;
            FrameworkCore.musicCategory.SetVolume(adjustedVolume);
        }

        private void OnMusicVolumeIncrease(object sender, EventArgs e)
        {
            FrameworkCore.options.musicVolume = UpdateGenericSlider(sender,
                FrameworkCore.options.musicVolume, 1);

            base.UpdateItemText(sender);
            float adjustedVolume = (float)FrameworkCore.options.musicVolume / ((MenuItem)sender).maxValue;
            FrameworkCore.musicCategory.SetVolume(adjustedVolume);
        }



        private void OnBrightnessIncrease(object sender, EventArgs e)
        {
            FrameworkCore.options.brightness = UpdateGenericSlider(sender,
                FrameworkCore.options.brightness, 1);

            base.UpdateItemText(sender);
        }

        private void OnBrightnessDecrease(object sender, EventArgs e)
        {
            FrameworkCore.options.brightness = UpdateGenericSlider(sender,
                FrameworkCore.options.brightness, -1);

            base.UpdateItemText(sender);
        }

        

        private void OnBrightnessSetText(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionText = FrameworkCore.options.brightness.ToString();
        }


        private int UpdateGenericSlider(object sender, int baseValue, int direction)
        {
            if (sender.GetType() != typeof(MenuItem))
                return baseValue;

            baseValue += direction;

            if (baseValue > ((MenuItem)sender).maxValue)
                baseValue = ((MenuItem)sender).minValue;
            else if (baseValue < ((MenuItem)sender).minValue)
                baseValue = ((MenuItem)sender).maxValue;

            return baseValue;
        }




        public override void Update(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            HandleOptionsMouse(gameTime, inputManager);
#endif

            base.Update(gameTime, inputManager);
        }

        private void HandleOptionsMouse(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;

            int lineHeight = (int)menuFont.MeasureString("Sample").Y;

            foreach (MenuItem item in menuItems)
            {
                int optionWidth = (int)(item.optionPos.X - item.position.X) + 96;

                Rectangle itemRect = new Rectangle(
                    (int)item.position.X,
                    (int)item.position.Y - lineHeight/2,
                    optionWidth,
                    lineHeight);

                itemRect.Inflate(32, 0);


                if (itemRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    if (inputManager.mouseHasMoved)
                        selectedItem = item;

                    bool slideLeft = false;
                    bool slideRight = false;
                    if (item.itemType == MenuItem.menuItemType.slider)
                    {
                        int sliderButtonWidth = sprite.arrow.Width * 2;

                        int itemValueWidth = (int)FrameworkCore.SerifBig.MeasureString(item.optionText).X;

                        if (inputManager.mousePos.X > item.optionPos.X + itemValueWidth)
                        {
                            slideRight = true;
                        }

                        if (inputManager.mousePos.X > item.optionPos.X - sliderButtonWidth &&
                            inputManager.mousePos.X < item.optionPos.X)
                        {
                            slideLeft = true;
                        }
                    }

                    if (inputManager.mouseLeftClick)
                    {
                        if (slideLeft)
                            item.onSliderDecrease();
                        else if (slideRight)
                            item.onSliderIncrease();
                        else
                            ActivateItem(inputManager);
                    }
                }
            }
        }

        

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            SaveInfo settings = FrameworkCore.storagemanager.GetDefaultSaveData();
            FrameworkCore.storagemanager.SaveData(settings);

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
            base.DrawDarkenScreen();

            if (menuItems.Count <= 0)
                return;


            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");
            Vector2 pos = menuItems[0].position;
            float transitionMod = Helpers.PopLerp(Transition, -300, 60, 0);
            pos.X += transitionMod;

            pos.Y -= titleVec.Y;

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuOptions, pos, titleColor, darkColor,
                0, Vector2.Zero, 1);

            DrawItems(gameTime, transitionMod);


            Vector2 lineVec = FrameworkCore.SerifBig.MeasureString("Sample");
            lineVec.Y *= 0.95f;

            Vector2 player1Pos = menuItems[3].position + new Vector2(transitionMod - 4, -12);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuCampaignPlayer1,
                player1Pos, titleColor, 1.57f, Vector2.Zero, 0.77f, SpriteEffects.None, 0);

            Vector2 player2Pos = menuItems[6].position + new Vector2(transitionMod - 4, -12);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuCampaignPlayer2,
                player2Pos, titleColor, 1.57f, Vector2.Zero, 0.77f, SpriteEffects.None, 0);
        }

        private int GetGapSize()
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            return (int)(textVec.Y - 4);
        }

        public override void DrawItems(GameTime gameTime, float transitionMod)
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            Vector2 posModifier = new Vector2(transitionMod, 0);

            foreach (MenuItem item in menuItems)
            {
                Color itemColor = Color.White;
                float itemSize = MathHelper.Lerp(0.95f, 1.0f, item.selectTransition);

                if (selectedItem != null && selectedItem == item)
                {
                    itemColor = Color.Lerp(itemColor, new Color(255, 128, 0), item.selectTransition);
                }
                
                itemColor = Color.Lerp(OldXNAColor.TransparentWhite, itemColor, Transition);
                Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0,0,0,64), Transition);

                Helpers.DrawOutline(menuFont, item.text, item.position + posModifier, itemColor, darkColor,
                    0, new Vector2(0, (textVec.Y * itemSize) * 0.5f), itemSize);


                Vector2 optionPos = item.optionPos + posModifier;
                Helpers.DrawOutline(menuFont, item.optionText, optionPos,
                    itemColor, darkColor, 0, new Vector2(0, (textVec.Y * itemSize) * 0.5f), itemSize);

                if (item.selectTransition > 0 && item.itemType == MenuItem.menuItemType.slider)
                {
                    float arrowSize = Helpers.PopLerp(item.selectTransition, 0, 1.4f, 1);
                    Vector2 arrowPos = optionPos - new Vector2(sprite.arrow.Width / 2, 0);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, arrowPos, sprite.arrow,
                        itemColor, 0, new Vector2(sprite.arrow.Width, sprite.arrow.Height / 2), arrowSize,
                        SpriteEffects.None, 0);

                    Vector2 optionSize = FrameworkCore.SerifBig.MeasureString(item.optionText);
                    arrowPos = optionPos + new Vector2(sprite.arrow.Width / 2, 0) + new Vector2(optionSize.X, 0);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, arrowPos, sprite.arrow,
                        itemColor, 0, new Vector2(0, sprite.arrow.Height / 2), arrowSize,
                        SpriteEffects.FlipHorizontally, 0);
                }
                else if (item.itemType == MenuItem.menuItemType.checkbox)
                {
                    Vector2 checkboxPos = optionPos;
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, checkboxPos, sprite.checkboxBox,
                        itemColor, 0, new Vector2(0, sprite.checkboxBox.Height / 2), itemSize,
                        SpriteEffects.None, 0);

                    if (item.optionBool)
                    {
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, checkboxPos, sprite.checkboxCheck,
                            itemColor, 0, new Vector2(0, sprite.checkboxCheck.Height / 2), itemSize,
                            SpriteEffects.None, 0);
                    }
                }


            }
        }
    }
}
