#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#if OLDJOYSTICK

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
    public enum JoyAxis
    {
        xAxis,
        yAxis,
        zAxis,
        xRot,
        yRot,
        zRot,
    }

    public class JoystickMenu : SysMenu
    {
        int WINDOWWIDTH = 950;
        GameButton gameButton = GameButton.moveforward;

        public enum GameButton
        {
            moveforward,
            movestrafe,
            lookhoriz,
            lookvert,
            confirm,
            cancel,
            altA,
            giveToAlly,
            prev,
            next,
            camReset,
            openMenu,
            turbo,
            climb,
            lower,
        }



        private string GetAxisName(JoyAxis axis)
        {
            if (axis == JoyAxis.xAxis)
                return Resource.JoyXAxis;
            else if (axis == JoyAxis.yAxis)
                return Resource.JoyYAxis;
            else if (axis == JoyAxis.zAxis)
                return Resource.JoyZAxis;
            else if (axis == JoyAxis.xRot)
                return Resource.JoyXRot;
            else if (axis == JoyAxis.yRot)
                return Resource.JoyYRot;
            else if (axis == JoyAxis.zRot)
                return Resource.JoyZRot;

            return "";
        }

        private string GetGameButtonName(GameButton btn)
        {
            if (btn == GameButton.moveforward)
                return Resource.JoyMoveForward;
            else if (btn == GameButton.movestrafe)
                return Resource.JoyMoveStrafe;
            else if (btn == GameButton.lookhoriz)
                return Resource.JoyLookHoriz;
            else if (btn == GameButton.lookvert)
                return Resource.JoyLookVert;
            else if (btn == GameButton.confirm)
                return Resource.JoyConfirm;
            else if (btn == GameButton.cancel)
                return Resource.JoyCancel;
            else if (btn == GameButton.altA)
                return Resource.JoyAltA;
            else if (btn == GameButton.giveToAlly)
                return Resource.JoyGiveToAlly;
            else if (btn == GameButton.prev)
                return Resource.JoyPrevMenu;
            else if (btn == GameButton.next)
                return Resource.JoyNextMenu;
            else if (btn == GameButton.camReset)
                return Resource.JoyCamReset;
            else if (btn == GameButton.openMenu)
                return Resource.JoyOpenMenu;
            else if (btn == GameButton.turbo)
                return Resource.JoyTurbo;
            else if (btn == GameButton.climb)
                return Resource.JoyClimb;
            else if (btn == GameButton.lower)
                return Resource.JoyLower;

            return "";
        }





        public JoystickMenu()
        {
            darkenScreen = true;
            

            menuFont = FrameworkCore.Serif;
            
            Vector2 pos = Vector2.Zero;
            pos.X = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - (WINDOWWIDTH / 2);
            pos.Y = 0;

            MenuItem item = new MenuItem(Resource.JoyMoveForward);
            item.position = pos;
            item.Selected += OnMoveForward;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetAxisName((JoyAxis)FrameworkCore.options.joyMoveForwardAxis);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyMoveStrafe);
            item.position = pos;
            item.Selected += OnMoveStrafe;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetAxisName((JoyAxis)FrameworkCore.options.joyMoveStrafeAxis);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyLookVert);
            item.position = pos;
            item.Selected += OnLookVert;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetAxisName((JoyAxis)FrameworkCore.options.joyLookForwardAxis);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyLookHoriz);
            item.position = pos;
            item.Selected += OnLookHoriz;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetAxisName((JoyAxis)FrameworkCore.options.joyLookStrafeAxis);
            base.AddItem(item);



            //BUTTONS
            pos.Y += GetGapSize() * 2;

            item = new MenuItem(Resource.JoyConfirm);
            item.position = pos;
            item.Selected += OnConfirm;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyConfirm);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyAltA);
            item.position = pos;
            item.Selected += OnAltA;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyAltA);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyCancel);
            item.position = pos;
            item.Selected += OnCancel;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyCancel);
            base.AddItem(item);



            pos.Y += GetGapSize() * 2;


            item = new MenuItem(Resource.JoyOpenMenu);
            item.position = pos;
            item.Selected += OnOpenMenu;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyOpenMenu);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyPrevMenu);
            item.position = pos;
            item.Selected += OnPrev;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyPrev);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyNextMenu);
            item.position = pos;
            item.Selected += OnNext;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyNext);
            base.AddItem(item);

            pos.Y += GetGapSize() * 2;



            item = new MenuItem(Resource.JoyCamReset);
            item.position = pos;
            item.Selected += OnCamReset;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyCamReset);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyClimb);
            item.position = pos;
            item.Selected += OnClimb;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyClimb);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyLower);
            item.position = pos;
            item.Selected += OnLower;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyLower);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyTurbo);
            item.position = pos;
            item.Selected += OnTurbo;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyTurbo);
            base.AddItem(item);

            pos.Y += GetGapSize();

            item = new MenuItem(Resource.JoyGiveToAlly);
            item.position = pos;
            item.Selected += OnGiveToAlly;
            item.itemType = MenuItem.menuItemType.list;
            item.optionText = GetButtonName(FrameworkCore.options.joyGiveToAlly);
            base.AddItem(item);

         
            


            pos.Y += GetGapSize()*2;

            item = new MenuItem(Resource.MenuDone);
            item.position = pos;
            item.Selected += OnDone;
            base.AddItem(item);



            //set up optionPos for each menuItem.
            float longestString = 0;
            foreach (MenuItem x in menuItems)
            {
                Vector2 stringLength = FrameworkCore.Serif.MeasureString(x.text);
                if (stringLength.X > longestString)
                    longestString = stringLength.X;
            }

            foreach (MenuItem z in menuItems)
            {
                z.optionPos = z.position + new Vector2(longestString, 0);
                z.optionPos += new Vector2(48, 0);
            }


            base.RepositionItems();
        }

        private string GetButtonName(int button)
        {
            return string.Format(Resource.JoyButton, (button+1));
        }


        private void OnMoveForward(object sender, EventArgs e)
        {
            gameButton = GameButton.moveforward;
            StartWaitingInput(gameButton);
        }

        private void OnMoveStrafe(object sender, EventArgs e)
        {
            gameButton = GameButton.movestrafe;
            StartWaitingInput(gameButton);
        }

        private void OnLookHoriz(object sender, EventArgs e)
        {
            gameButton = GameButton.lookhoriz;
            StartWaitingInput(gameButton);
        }

        private void OnLookVert(object sender, EventArgs e)
        {
            gameButton = GameButton.lookvert;
            StartWaitingInput(gameButton);
        }



        private void OnConfirm(object sender, EventArgs e)
        {
            gameButton = GameButton.confirm;
            StartWaitingInput(gameButton);
        }

        private void OnCancel(object sender, EventArgs e)
        {
            gameButton = GameButton.cancel;
            StartWaitingInput(gameButton);
        }



        private void OnAltA(object sender, EventArgs e)
        {
            gameButton = GameButton.altA;
            StartWaitingInput(gameButton);
        }

        private void OnGiveToAlly(object sender, EventArgs e)
        {
            gameButton = GameButton.giveToAlly;
            StartWaitingInput(gameButton);
        }

        private void OnPrev(object sender, EventArgs e)
        {
            gameButton = GameButton.prev;
            StartWaitingInput(gameButton);
        }

        private void OnNext(object sender, EventArgs e)
        {
            gameButton = GameButton.next;
            StartWaitingInput(gameButton);
        }

        private void OnCamReset(object sender, EventArgs e)
        {
            gameButton = GameButton.camReset;
            StartWaitingInput(gameButton);
        }

        private void OnOpenMenu(object sender, EventArgs e)
        {
            gameButton = GameButton.openMenu;
            StartWaitingInput(gameButton);
        }


        private void OnTurbo(object sender, EventArgs e)
        {
            gameButton = GameButton.turbo;
            StartWaitingInput(gameButton);
        }

        private void OnClimb(object sender, EventArgs e)
        {
            gameButton = GameButton.climb;
            StartWaitingInput(gameButton);
        }

        private void OnLower(object sender, EventArgs e)
        {
            gameButton = GameButton.lower;
            StartWaitingInput(gameButton);
        }





        private void StartWaitingInput(GameButton btn)
        {
            if ((int)btn <= 3)
                waitingLine1 = Resource.JoyWaitingForStick;
            else
                waitingLine1 = Resource.JoyWaitingForKey;

            waitingLine2 = GetGameButtonName(btn);

            waitingForInput = true;

            FrameworkCore.PlayCue(sounds.click.select);
        }

        string waitingLine1 = "";
        string waitingLine2 = "";
        
        private void OnDone(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            OptionsData options = FrameworkCore.storagemanager.GetDefaultPCOptions();
            FrameworkCore.storagemanager.SaveOptionsPC(options);


            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }



        bool waitingForInput = false;

        private void OnButtonChange(GameButton gameBtn, JoyAxis axis)
        {
            //change the option.
            if (gameBtn == GameButton.moveforward)
                FrameworkCore.options.joyMoveForwardAxis = (int)axis;
            else if (gameBtn == GameButton.movestrafe)
                FrameworkCore.options.joyMoveStrafeAxis = (int)axis;
            else if (gameBtn == GameButton.lookhoriz)
                FrameworkCore.options.joyLookStrafeAxis = (int)axis;
            else if (gameBtn == GameButton.lookvert)
                FrameworkCore.options.joyLookForwardAxis = (int)axis;

            //change the menu item's text.
            if (selectedItem != null)
                selectedItem.optionText = GetAxisName(axis);

            //close the popup.
            waitingForInput = false;

            FrameworkCore.PlayCue(sounds.click.activate);
        }




        private void OnButtonChange(GameButton gameBtn, int buttonNumber)
        {
            if (gameBtn == GameButton.confirm)
                FrameworkCore.options.joyConfirm = buttonNumber;
            else if (gameBtn == GameButton.cancel)
                FrameworkCore.options.joyCancel = buttonNumber;
            else if (gameBtn == GameButton.altA)
                FrameworkCore.options.joyAltA = buttonNumber;
            else if (gameBtn == GameButton.giveToAlly)
                FrameworkCore.options.joyGiveToAlly = buttonNumber;
            else if (gameBtn == GameButton.prev)
                FrameworkCore.options.joyPrev = buttonNumber;
            else if (gameBtn == GameButton.next)
                FrameworkCore.options.joyNext = buttonNumber;
            else if (gameBtn == GameButton.camReset)
                FrameworkCore.options.joyCamReset = buttonNumber;
            else if (gameBtn == GameButton.openMenu)
                FrameworkCore.options.joyOpenMenu = buttonNumber;
            else if (gameBtn == GameButton.turbo)
                FrameworkCore.options.joyTurbo = buttonNumber;
            else if (gameBtn == GameButton.climb)
                FrameworkCore.options.joyClimb = buttonNumber;
            else if (gameBtn == GameButton.lower)
                FrameworkCore.options.joyLower = buttonNumber;



            if (selectedItem != null)
                selectedItem.optionText = GetButtonName(buttonNumber);


            waitingForInput = false;

            FrameworkCore.PlayCue(sounds.click.activate);
        }        

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (waitingForInput)
            {
                //wait for axis input.
                if ((int)gameButton <= 3)
                {
                    if (Math.Abs(xAxis) > 0.7f)
                    {
                        OnButtonChange(gameButton, JoyAxis.xAxis);
                    }
                    else if (Math.Abs(yAxis) > 0.7f)
                    {
                        OnButtonChange(gameButton, JoyAxis.yAxis);
                    }
                    else if (Math.Abs(zAxis) > 0.7f)
                    {
                        OnButtonChange(gameButton, JoyAxis.zAxis);
                    }
                    else if (Math.Abs(xRot) > 0.7f)
                    {
                        OnButtonChange(gameButton, JoyAxis.xRot);
                    }
                    else if (Math.Abs(yRot) > 0.7f)
                    {
                        OnButtonChange(gameButton, JoyAxis.yRot);
                    }
                    else if (Math.Abs(zRot) > 0.7f)
                    {
                        OnButtonChange(gameButton, JoyAxis.zRot);
                    }
                }
                else
                {
                    //waiting for ANY buttonpress.
                    for (int i = 0; i < joybuttons.Length; i++)
                    {
                        if (joybuttons[i] && !lastJoybuttons[i])
                        {
                            OnButtonChange(gameButton, i);
                            return;
                        }
                    }                
                }

                //Cancel.
                if (inputManager.kbEscPressed)
                {
                    waitingForInput = false;
                }

                return;
            }

            HandleMouse(gameTime, inputManager);

            base.Update(gameTime, inputManager);
        }

        int ITEMWIDTH = 540;

        private void HandleMouse(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;

            int lineHeight = (int)menuFont.MeasureString("Sample").Y;

            bool isHovering = false;

            foreach (MenuItem item in menuItems)
            {
                Rectangle itemRect = new Rectangle(
                    (int)item.position.X,
                    (int)item.position.Y - (lineHeight / 2) + 6,
                    ITEMWIDTH,
                    lineHeight);

                itemRect.Inflate(1, 1);

                if (itemRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    isHovering = true;

                    if (inputManager.mouseHasMoved)
                        selectedItem = item;

                    if (inputManager.mouseLeftClick)
                    {
                        if (menuItems.IndexOf(item) <= 3 &&
                            inputManager.mousePos.X >= (item.position.X + ITEMWIDTH - (sprite.checkboxBox.Width*CHECKBOXSIZE)))
                        {
                            FlipAxis(menuItems.IndexOf(item));
                        }
                        else
                            ActivateItem(inputManager);
                    }
                }
            }

            if (!isHovering && inputManager.mouseHasMoved)
                selectedItem = null;
        }

        private void FlipAxis(int index)
        {
            FrameworkCore.PlayCue(sounds.click.activate);

            if (index == 0)
                FrameworkCore.options.joyMoveForwardFlip = !FrameworkCore.options.joyMoveForwardFlip;
            else if (index==1)
                FrameworkCore.options.joyMoveStrafeFlip = !FrameworkCore.options.joyMoveStrafeFlip;
            else if (index == 2)
                FrameworkCore.options.joyLookVertFlip = !FrameworkCore.options.joyLookVertFlip;
            else if (index == 3)                
                FrameworkCore.options.joyLookHorizFlip = !FrameworkCore.options.joyLookHorizFlip;
        }



        bool[] lastJoybuttons = new bool[32];
        bool[] joybuttons = new bool[32];

        float xAxis = 0;
        float yAxis = 0;
        float zAxis = 0;

        float xRot = 0;
        float yRot = 0;
        float zRot = 0;






        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            //SaveInfo settings = FrameworkCore.storagemanager.GetDefaultSaveData();
            //FrameworkCore.storagemanager.SaveData(settings);

            base.Deactivate();
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {
            base.Activate();
        }


        #region DRAW
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
            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuJoystick, pos, titleColor, darkColor,
                0, Vector2.Zero, 1);

            DrawKnobs();

            DrawItems(gameTime, transitionMod);

            DrawWaitingBox(gameTime);
        }

        private void DrawWaitingBox(GameTime gameTime)
        {
            if (!waitingForInput)
                return;

            Helpers.DarkenScreen(160);

            Vector2 boxCenter = Helpers.GetScreenCenter();
            boxCenter.Y += Helpers.Pulse(gameTime, 8, 3);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxCenter,
                sprite.inventoryBox, Color.White, 0, Helpers.SpriteCenter(sprite.inventoryBox),
                1, SpriteEffects.None, 0);

            Vector2 linePos = boxCenter;
            linePos.Y -= 48;

            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, waitingLine1,
                linePos, new Color(96, 96,96), 1.1f);

            linePos.Y += 48;
            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, waitingLine2,
                 linePos, Color.Black, 1.2f);

            linePos.Y = boxCenter.Y + sprite.inventoryBox.Height / 2 - 48;
            Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, Resource.JoyEscCancel,
                linePos, Color.Gray, 1);
        }

        private int GetGapSize()
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            return (int)(textVec.Y );
        }

        const int BARWIDTH = 150;

        private void DrawKnobs()
        {
            Vector2 pos = menuItems[0].position;            

            pos.Y -= 18;
            
            int lineHeight = (int)FrameworkCore.Serif.MeasureString("S").Y;
            lineHeight -= 2;

            pos.X = (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) + (WINDOWWIDTH / 2);
            pos.X -= BARWIDTH;

            pos.X += Helpers.PopLerp(Transition, 200, -50, 0);

            float glowTolerance = 0.15f;

            DrawStringRight(pos, Resource.JoyXAxis, (Math.Abs(xAxis) > glowTolerance));
            DrawBar(pos, xAxis);
            pos.Y += lineHeight;

            DrawStringRight(pos, Resource.JoyYAxis, (Math.Abs(yAxis) > glowTolerance));
            DrawBar(pos, yAxis);
            pos.Y += lineHeight;

            DrawStringRight(pos, Resource.JoyZAxis, (Math.Abs(zAxis) > glowTolerance));
            DrawBar(pos, zAxis);
            pos.Y += lineHeight;


            DrawStringRight(pos, Resource.JoyXRot, (Math.Abs(xRot) > glowTolerance));
            DrawBar(pos, xRot);
            pos.Y += lineHeight;

            DrawStringRight(pos, Resource.JoyYRot, (Math.Abs(yRot) > glowTolerance));
            DrawBar(pos, yRot);
            pos.Y += lineHeight;

            DrawStringRight(pos, Resource.JoyZRot, (Math.Abs(zRot) > glowTolerance));
            DrawBar(pos, zRot);
            pos.Y += lineHeight;

            
            for (int i = 0; i < Math.Min(joybuttons.Length,16); i++)
            {
                string buttonText = "" + (i+1);
                DrawStringRight(pos, buttonText, (joybuttons[i] == true));
                DrawButton(pos, (joybuttons[i] == true));
                pos.Y += lineHeight;
            }
             
        }

        private void DrawBar(Vector2 pos, float value)
        {
            //background.
            Color bgColor = Color.Lerp(OldXNAColor.TransparentWhite, new Color(255, 255, 255, 48), Transition);
            Rectangle bgRect = new Rectangle(
                (int)pos.X,
                (int)pos.Y+7,
                BARWIDTH,
                13);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, bgRect, sprite.blank, bgColor);


            //now draw the actual bar.
            Color barColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);

            float adjustedTransition = MathHelper.Clamp(value + 1, 0,2);
            adjustedTransition = MathHelper.Clamp(adjustedTransition/2, 0, 1);
            int barvalue = (int)MathHelper.Lerp(1, BARWIDTH, adjustedTransition);

            Rectangle barRect = new Rectangle(
                (int)pos.X,
                (int)pos.Y + 7,
                barvalue,
                13);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, barRect, sprite.blank, barColor);
        }

        private void DrawButton(Vector2 pos, bool isPressed)
        {
            //background.
            Color bgColor = Color.Lerp(OldXNAColor.TransparentWhite, new Color(255, 255, 255, 48), Transition);
            Rectangle bgRect = new Rectangle(
                (int)pos.X,
                (int)pos.Y + 7,
                13,
                13);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, bgRect, sprite.blank, bgColor);

            if (!isPressed)
                return;

            //now draw the actual bar.
            Color barColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
           
            Rectangle barRect = new Rectangle(
                (int)pos.X,
                (int)pos.Y + 7,
                13,
                13);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, barRect, sprite.blank, barColor);
        }

        private void DrawStringRight(Vector2 pos, string txt, bool glow)
        {
            Color color = Color.Lerp(OldXNAColor.TransparentWhite, new Color(160,160,160), Transition);
            Color bgcolor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0,128), Transition);

            if (glow)
                color = Color.White;

            int width = (int)FrameworkCore.Serif.MeasureString(txt).X;
            pos.X -= width;
            pos.X -= 8;

            Helpers.DrawOutline(FrameworkCore.Serif, txt, pos, color, bgcolor);
        }

        const float CHECKBOXSIZE = 0.65f;

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


                Color actionName = new Color(192, 192, 192);
                actionName = Color.Lerp(actionName, new Color(255, 128, 0), item.selectTransition);
                actionName = Color.Lerp(Helpers.transColor(actionName), actionName, Transition);                
                Helpers.DrawOutline(menuFont, item.text, item.position + posModifier, actionName, darkColor,
                    0, new Vector2(0, (textVec.Y * itemSize) * 0.5f), itemSize);

                

                if (item.itemType == MenuItem.menuItemType.list &&
                    item.optionText != null&& item.optionText.Length > 0)
                {
                    Vector2 optionPos = item.optionPos + posModifier;    
                    Vector2 checkboxPos = optionPos;                    
                    Helpers.DrawOutline(menuFont, item.optionText, checkboxPos, itemColor, darkColor,
                        0, new Vector2(0, (textVec.Y * itemSize) * 0.5f), itemSize);
                }

                if (menuItems.IndexOf(item) <= 3)
                {
                    //draw the AxisInvert checkbox.
                    //ITEMWIDTH
                    Vector2 checkboxPos = item.position;
                    checkboxPos.X += Helpers.PopLerp(Transition, -300, 50, 0);

                    checkboxPos.X += ITEMWIDTH;
                    checkboxPos.X -= ((sprite.checkboxBox.Width / 2) * CHECKBOXSIZE);
                    checkboxPos.Y += 2;


                    //draw the title.
                    if (menuItems.IndexOf(item) <= 0)
                    {
                        Color invertColor = new Color(192,192,192);
                        invertColor = Color.Lerp(Helpers.transColor(invertColor), invertColor, Transition);
                        Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                            Resource.JoyInvert,
                            checkboxPos + new Vector2(0,-GetGapSize()),
                            invertColor, darkColor, 0.85f, 0);
                    }


                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, checkboxPos,
                        sprite.checkboxBox, itemColor, 0, Helpers.SpriteCenter(sprite.checkboxBox),
                        CHECKBOXSIZE, SpriteEffects.None, 0);

                    if (GetAxisFlip(menuItems.IndexOf(item)))
                    {
                        FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, checkboxPos,
                            sprite.checkboxCheck, itemColor, 0, Helpers.SpriteCenter(sprite.checkboxBox),
                            CHECKBOXSIZE, SpriteEffects.None, 0);
                    }
                }
            }
        }

        private bool GetAxisFlip(int index)
        {
            if (index == 0)
                return FrameworkCore.options.joyMoveForwardFlip;
            else if (index == 1)
                return FrameworkCore.options.joyMoveStrafeFlip;
            else if (index == 2)
                return FrameworkCore.options.joyLookVertFlip;                
            else if (index == 3)
                return FrameworkCore.options.joyLookHorizFlip;

            return false;
        }

        #endregion
    }
}

#endif // OLDJOYSTICK