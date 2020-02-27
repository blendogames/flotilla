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
    public class ControlsMenu : SysMenu
    {
        int WINDOWWIDTH = 1000;


        public ControlsMenu()
        {
            //int sectionGap = 18;

            darkenScreen = true;
            int windowHeight = 500;

            Vector2 pos = Vector2.Zero;
            pos.X = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - (WINDOWWIDTH / 2);
            pos.Y = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - (windowHeight/2);

            Vector2 titleVec = FrameworkCore.Gothic.MeasureString("Sample");
            pos.Y += titleVec.Y;

            pos.Y = Math.Max(100, pos.Y);

            MenuItem item = new MenuItem(Resource.MenuOptionsSensitivity);
            item.sliderDecrease += OnSensitivityDecrease;
            item.sliderIncrease += OnSensitivityIncrease;
            item.Selected += OnSensitivityIncrease;
            item.itemType = MenuItem.menuItemType.slider;
            item.position = pos;
            item.setOptionText += OnSensitivitySetText;
            base.AddItem(item);


            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuOptionsMousewheel);
            item.sliderDecrease += OnWheelDecrease;
            item.sliderIncrease += OnWheelIncrease;
            item.minValue = 0;
            item.maxValue = 1;            
            item.Selected += OnWheelIncrease;
            item.itemType = MenuItem.menuItemType.slider;
            item.position = pos;
            item.setOptionText += OnWheelSetText;
            base.AddItem(item);





            pos.Y += GetGapSize();

            item = new MenuItem(Resource.MenuOptionsManualDefault);
            item.position = pos;
            item.itemType = MenuItem.menuItemType.checkbox;
            item.Selected += OnManualDefault;
            item.optionBool = FrameworkCore.options.manualDefault;
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

        
        private void OnDone(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            
            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }

        private void OnManualDefault(object sender, EventArgs e)
        {
            FrameworkCore.options.manualDefault = !FrameworkCore.options.manualDefault;

            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionBool = FrameworkCore.options.manualDefault;
        }





        private void OnWheelDecrease(object sender, EventArgs e)
        {
            FrameworkCore.options.mousewheel = UpdateGenericSlider(sender,
                FrameworkCore.options.mousewheel, -1);

            base.UpdateItemText(sender);
        }

        private void OnWheelIncrease(object sender, EventArgs e)
        {
            FrameworkCore.options.mousewheel = UpdateGenericSlider(sender,
                FrameworkCore.options.mousewheel, 1);

            base.UpdateItemText(sender);
        }

        private void OnWheelSetText(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            string txt = string.Empty;
            if (FrameworkCore.options.mousewheel == 0)
                txt = Resource.MenuOptionsMousewheelRaise;
            else
                txt = Resource.MenuOptionsMousewheelForward;

            ((MenuItem)sender).optionText = txt;
        }





        private void OnSensitivityDecrease(object sender, EventArgs e)
        {
            FrameworkCore.options.sensitivity = UpdateGenericSlider(sender,
                FrameworkCore.options.sensitivity, -1);

            base.UpdateItemText(sender);
        }

        private void OnSensitivityIncrease(object sender, EventArgs e)
        {
            FrameworkCore.options.sensitivity = UpdateGenericSlider(sender,
                FrameworkCore.options.sensitivity, 1);

            base.UpdateItemText(sender);
        }

        private void OnSensitivitySetText(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).optionText = FrameworkCore.options.sensitivity.ToString();
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

                if (item.itemType == MenuItem.menuItemType.slider)
                {
                    int itemValueWidth = (int)FrameworkCore.SerifBig.MeasureString(item.optionText).X;
                    optionWidth += itemValueWidth;
                }

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
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuControls, pos, titleColor, darkColor,
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
