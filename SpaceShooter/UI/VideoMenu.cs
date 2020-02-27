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
    public class VideoMenu : SysMenu
    {
        int WINDOWWIDTH = 550;


        Point[] resolutions;


        public VideoMenu()
        {
            resolutions = new Point[]
            {
                //new Point(1024, 768),
                //new Point(1152, 864),
                new Point(1280, 960),
                new Point(1280, 1024),
                new Point(1600, 1200),

                new Point(1280, 720),
                new Point(1280, 768),
                new Point(1280, 800),
                new Point(1360, 768),
                new Point(1366, 768),
                new Point(1440, 900),
                new Point(1600, 900),
                new Point(1600, 1050),
                new Point(1920, 1080),
                new Point(1920, 1200),
            };




            //int sectionGap = 18;

            darkenScreen = true;
            int windowHeight = 500;

            Vector2 pos = Vector2.Zero;
            pos.X = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - (WINDOWWIDTH / 2);
            pos.Y = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - (windowHeight/2);

            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");
            pos.Y += titleVec.Y;

            pos.Y = Math.Max(100, pos.Y);


            MenuItem item = new MenuItem(Resource.MenuVideoResolution);
#if !ONLIVE
            item.Selected += OnResolution;
            item.position = pos;
            item.setOptionText += OnResolutionSetText;
            item.itemType = MenuItem.menuItemType.list;
            base.AddItem(item);


            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuVideoFullscreen);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnFullscreen;
            item.optionBool = FrameworkCore.options.fullscreen;
            base.AddItem(item);

            pos.Y += GetGapSize();
#endif

            item = new MenuItem(Resource.MenuVideoBloom);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnBloom;
            item.optionBool = FrameworkCore.options.bloom;
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuVideoPlanets);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnRenderPlanets;
            item.optionBool = FrameworkCore.options.renderPlanets;
            base.AddItem(item);


            pos.Y += GetGapSize();


            item = new MenuItem(Resource.MenuOptionsHardwaremouse);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnHardwaremouse;
            item.optionBool = FrameworkCore.options.hardwaremouse;
            base.AddItem(item);



            pos.Y += GetGapSize()*2;


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
        }

        private void OnHardwaremouse(object sender, EventArgs e)
        {
            FrameworkCore.options.hardwaremouse = !FrameworkCore.options.hardwaremouse;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.hardwaremouse;

            if (FrameworkCore.options.hardwaremouse)
                FrameworkCore.Game.IsMouseVisible = true;
            else
                FrameworkCore.Game.IsMouseVisible = false;
        }
        
        private void OnDone(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            
            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }



        //============ FULLSCREEN
        private void OnFullscreen(object sender, EventArgs e)
        {
            FrameworkCore.options.fullscreen = !FrameworkCore.options.fullscreen;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.fullscreen;

            FrameworkCore.Graphics.IsFullScreen = FrameworkCore.options.fullscreen;
            FrameworkCore.Graphics.ApplyChanges();
        }

        //============ BLOOM
        private void OnBloom(object sender, EventArgs e)
        {
            FrameworkCore.options.bloom = !FrameworkCore.options.bloom;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.bloom;
        }


        //============ renderPlanets
        private void OnRenderPlanets(object sender, EventArgs e)
        {
            FrameworkCore.options.renderPlanets = !FrameworkCore.options.renderPlanets;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.renderPlanets;
        }





        //============== RESOLUTION
        private void OnResolution(object sender, EventArgs e)
        {
            SkirmishPopup popup = new SkirmishPopup(Owner);
            int itemHeight = (int)FrameworkCore.Serif.MeasureString("Sample").Y;
            popup.screenPos = new Vector2(selectedItem.position.X + 96,
                (resolutions.Length / 2) * itemHeight);
            popup.hideChildren = false;

            MenuItem item = null;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string itemName = resolutions[i].X + Resource.MenuVideoDivider + resolutions[i].Y;
                if (resolutions[i].X * 3 <= resolutions[i].Y * 4)
                {
                    //4:3 aspect ratio or lower.
                }
                else
                {
                    itemName += " " + Resource.MenuVideoWidescreen;
                }

                if (FrameworkCore.Graphics.PreferredBackBufferHeight == resolutions[i].Y &&
                    FrameworkCore.Graphics.PreferredBackBufferWidth == resolutions[i].X)
                {
                    itemName += " " + Resource.MenuVideoCurrent;
                }


                item = new MenuItem(itemName);
                item.Selected += OnChooseResolution;
                item.GenericInt1 = resolutions[i].X;
                item.GenericInt2 = resolutions[i].Y;
                popup.AddItem(item);
            }

            item = new MenuItem(Resource.MenuCancel);
            item.Selected += closePopup;
            popup.AddItem(item);

            Owner.AddMenu(popup);
        }

        private void OnChooseResolution(object sender, InputArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();



            FrameworkCore.options.resolutionX = ((MenuItem)sender).GenericInt1;
            FrameworkCore.options.resolutionY = ((MenuItem)sender).GenericInt2;

            if (FrameworkCore.Graphics.PreferredBackBufferHeight != FrameworkCore.options.resolutionY ||
                FrameworkCore.Graphics.PreferredBackBufferWidth != FrameworkCore.options.resolutionX)
            {
                FrameworkCore.Graphics.PreferredBackBufferHeight = FrameworkCore.options.resolutionY;
                FrameworkCore.Graphics.PreferredBackBufferWidth = FrameworkCore.options.resolutionX;

                FrameworkCore.Graphics.ApplyChanges();


                //update the camera fov
                if (FrameworkCore.gameState == GameState.Play)
                    Helpers.UpdateCameraProjections(FrameworkCore.players.Count);
                else
                    Helpers.UpdateCameraProjections(1);

                if (FrameworkCore.sysMenuManager.menus.Count > 0)
                {
                    for (int i = 0; i < FrameworkCore.sysMenuManager.menus.Count; i++)
                    {
                        FrameworkCore.sysMenuManager.menus[i].RepositionItems();
                    }
                }

                if (FrameworkCore.MainMenuManager.menus.Count > 0)
                {
                    for (int i = 0; i < FrameworkCore.MainMenuManager.menus.Count; i++)
                    {
                        FrameworkCore.MainMenuManager.menus[i].RepositionItems();
                    }
                }
            }
        }


        private void closePopup(object sender, InputArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }


        private void OnResolutionSetText(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            //((MenuItem)sender).optionText = FrameworkCore.options.soundVolume.ToString();
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
            if (Transition <= 0)
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
            
            OptionsData options = FrameworkCore.storagemanager.GetDefaultPCOptions();
            FrameworkCore.storagemanager.SaveOptionsPC(options);

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

            pos.Y -= titleVec.Y + 24;

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuVideo, pos, titleColor, darkColor,
                0, Vector2.Zero, 1);

            DrawItems(gameTime, transitionMod);
        }

        private int GetGapSize()
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            return (int)(textVec.Y );
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
                else if (item.itemType == MenuItem.menuItemType.list)
                {
                    Vector2 checkboxPos = optionPos;
                    string resText = "" + FrameworkCore.Graphics.GraphicsDevice.Viewport.Width +
                        Resource.MenuVideoDivider + "" + FrameworkCore.Graphics.GraphicsDevice.Viewport.Height;
                    Helpers.DrawOutline(menuFont, resText, checkboxPos, itemColor, darkColor,
                        0, new Vector2(0, (textVec.Y * itemSize) * 0.5f), itemSize);
                }
            }
        }
    }
}
