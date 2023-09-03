using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooter
{
    public class InputManager
    {

        PlayerIndex index;
        public PlayerIndex playerIndex
        {
            get { return index; }
        }

#if (WINDOWS && STEAM)
        public bool activateSteamOverlay
        {
            get
            {
                return 
                    (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)) &&
                    kbTabPressed;
            }
        }
#endif




        public Vector2 mousePos
        {
            get
            {
#if SDL2
                if (Mouse.IsRelativeMouseModeEXT)
                {
                    return relativeMousePosition;
                }
#endif
                return new Vector2(mouseInfo.X, mouseInfo.Y);
            }
        }


        public bool mouseHasMoved
        {
            get
            {
#if SDL2
                if (Mouse.IsRelativeMouseModeEXT)
                {
                    return (mouseInfo.X != 0) || (mouseInfo.Y != 0);
                }
#endif
                return (lastMouseInfo.X != mouseInfo.X ||
                    lastMouseInfo.Y != mouseInfo.Y);
            }
        }

        public bool mouseWheelPressed
        {
            get
            {
                return mouseInfo.MiddleButton == ButtonState.Pressed && lastMouseInfo.MiddleButton == ButtonState.Released;
            }
        }


        public bool mouseWheelUp
        {
            get
            {
                if (mouseInfo.ScrollWheelValue - lastMouseInfo.ScrollWheelValue > 0)
                    return true;

                return false;
            }
        }

        public bool mouseWheelDown
        {
            get
            {
                if (mouseInfo.ScrollWheelValue - lastMouseInfo.ScrollWheelValue < 0)
                    return true;

                return false;
            }
        }

        public bool dpadUp
        {
            get
            {
                return gamePadState.DPad.Up == ButtonState.Pressed;
            }
        }

        public bool dpadDown
        {
            get
            {
                return gamePadState.DPad.Down == ButtonState.Pressed;
            }
        }

        public bool dpadLeft
        {
            get
            {
                return gamePadState.DPad.Left == ButtonState.Pressed;
            }
        }

        public bool dpadRight
        {
            get
            {
                return gamePadState.DPad.Right == ButtonState.Pressed;
            }
        }

        
        public bool buttonUpHeld
        {
            get
            {
                return gamePadState.DPad.Up == ButtonState.Pressed
                    ||
                    gamePadState.Buttons.RightShoulder == ButtonState.Pressed;
            }
        }

        public bool buttonDownHeld
        {
            get
            {
                return gamePadState.DPad.Down == ButtonState.Pressed
                    ||
                    gamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
            }
        }

        public bool buttonYPressed
        {
            get
            {
                return (gamePadState.Buttons.Y == ButtonState.Pressed &&
                    lastPadState.Buttons.Y == ButtonState.Released);
            }
        }

        public bool buttonAPressed
        {
            get
            {
                return ((gamePadState.Buttons.A == ButtonState.Pressed &&
                    lastPadState.Buttons.A == ButtonState.Released));
            }
        }

        public bool mouseLeftClick
        {
            get
            {
                return (mouseInfo.LeftButton == ButtonState.Pressed &&
                   lastMouseInfo.LeftButton == ButtonState.Released);
            }
        }

        public bool mouseRightClick
        {
            get
            {
                return (mouseInfo.RightButton == ButtonState.Pressed &&
                   lastMouseInfo.RightButton == ButtonState.Released);
            }
        }

        bool MouseCameraMode;

        public bool mouseCameraMode
        {
            get
            {
                return MouseCameraMode;
            }
        }

        public bool mouseRightHeld
        {
            get
            {
                return mouseInfo.RightButton == ButtonState.Pressed;
            }
        }

        public bool mouseLeftHeld
        {
            get
            {
                return mouseInfo.LeftButton == ButtonState.Pressed;
            }
        }

        public bool mouseLeftStartHold
        {
            get
            {
                return (lastMouseInfo.LeftButton == ButtonState.Released &&
                    mouseInfo.LeftButton == ButtonState.Pressed);
            }
        }

        public bool mouseRightStartHold
        {
            get
            {
                return (lastMouseInfo.RightButton == ButtonState.Released &&
                    mouseInfo.RightButton == ButtonState.Pressed);
            }
        }

        public bool mouseRightJustReleased
        {
            get
            {
                return (lastMouseInfo.RightButton == ButtonState.Pressed &&
                    mouseInfo.RightButton == ButtonState.Released);
            }
        }
        

        public bool buttonBPressed
        {
            get
            {
                return ((gamePadState.Buttons.B == ButtonState.Pressed &&
                    lastPadState.Buttons.B == ButtonState.Released));
            }
        }

        public bool camNextTarget
        {
            get
            {
                return (gamePadState.Buttons.RightShoulder == ButtonState.Pressed &&
                    lastPadState.Buttons.RightShoulder == ButtonState.Released)
#if WINDOWS
                    ||
                    (keyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space))
#endif
                    ;
            }
        }

        public bool MouseMiddleClick
        {
            get
            {
                return (mouseInfo.MiddleButton == ButtonState.Pressed && lastMouseInfo.MiddleButton == ButtonState.Released);
            }
        }

        public bool camResetClick
        {
            get
            {
                return (gamePadState.Buttons.RightStick == ButtonState.Pressed &&
                    lastPadState.Buttons.RightStick == ButtonState.Released)
#if WINDOWS
                    ||
                    (mouseInfo.MiddleButton == ButtonState.Pressed && lastMouseInfo.MiddleButton == ButtonState.Released)
#endif                    
                    ;
            }
        }

        public bool camSelectNextShip
        {
            get
            {
                return (gamePadState.Buttons.X == ButtonState.Pressed &&
                    lastPadState.Buttons.X == ButtonState.Released)
#if WINDOWS
                    ||
                    (keyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space))
#endif         
                    ;
            }
        }

        public bool camResetHeld
        {
            get
            {
                return gamePadState.Buttons.RightStick == ButtonState.Pressed;
            }
        }

        public List<Keys> getPressedKeys
        {
            get
            {
                Keys[] keyArray = keyboardState.GetPressedKeys();

                if (keyArray.Length <= 0)
                    return null;

                List<Keys> pressedKeys = new List<Keys>();
                for (int i = 0; i < keyArray.Length; i++)
                {
                    if (lastKeyboardState.IsKeyUp(keyArray[i]) == true)
                    {
                        pressedKeys.Add(keyArray[i]);
                    }
                }

                if (pressedKeys.Count > 0)
                {
                    return pressedKeys;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool kbShiftHeld
        {
            get { return keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift); }
        }

        public bool kbEnter
        {
            get { return keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter); }
        }

        public bool kbSpace
        {
            get { return keyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space); }
        }

        public bool kbSpaceHeld
        {
            get { return keyboardState.IsKeyDown(Keys.Space); }
        }

        public bool kbSkipScreen
        {
            get { return (keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))
                ||
                (keyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space));
            }
        }

        public bool turboHeld
        {
            get
            {
                return gamePadState.Buttons.LeftStick == ButtonState.Pressed
#if WINDOWS
                    ||
                    keyboardState.IsKeyDown(Keys.LeftShift)
                    ||
                    keyboardState.IsKeyDown(Keys.RightShift)
#endif         
                    ;
            }
        }

        public Vector2 stickLeft
        {
            get { return movementControl(); }
        }

        
        private Vector2 movementControl()
        {

            if (keyboardState.IsKeyDown(Keys.W)
                ||
                keyboardState.IsKeyDown(Keys.S)
                ||
                keyboardState.IsKeyDown(Keys.A)
                ||
                keyboardState.IsKeyDown(Keys.D)
                ||
                keyboardState.IsKeyDown(Keys.NumPad8)
                ||
                keyboardState.IsKeyDown(Keys.NumPad4)
                ||
                keyboardState.IsKeyDown(Keys.NumPad5)
                ||
                keyboardState.IsKeyDown(Keys.NumPad6)
                ||
                keyboardState.IsKeyDown(Keys.Up)
                ||
                keyboardState.IsKeyDown(Keys.Down)
                ||
                keyboardState.IsKeyDown(Keys.Left)
                ||
                keyboardState.IsKeyDown(Keys.Right)
                )
            {
                Vector2 moveVec = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.NumPad8))
                    moveVec.Y = 1;
                else if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.NumPad5))
                    moveVec.Y = -1;

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left) ||   keyboardState.IsKeyDown(Keys.NumPad4))
                    moveVec.X = -1;
                else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.NumPad6))
                    moveVec.X = 1;

                return moveVec;
            }
            else
            {
                return gamePadState.ThumbSticks.Left;
            }
        }



        public bool kbBackspaceHold
        {
            get { return keyboardState.IsKeyDown(Keys.Back); }
        }

        public bool kbBackspaceJustPressed
        {
            get
            {
                return (keyboardState.IsKeyDown(Keys.Back) && lastKeyboardState.IsKeyUp(Keys.Back))
                    ||
                    (keyboardState.IsKeyDown(Keys.C) && lastKeyboardState.IsKeyUp(Keys.C));
            }
        }



        public Vector2 stickRight
        {
            get
            {
                return gamePadState.ThumbSticks.Right;
            }
        }

        public bool sysMenuLeft
        {
            get
            {
                return (gamePadState.ThumbSticks.Left.X < -0.7f && lastPadState.ThumbSticks.Left.X > -0.7f)
                    ||
                    (gamePadState.DPad.Left == ButtonState.Pressed && lastPadState.DPad.Left == ButtonState.Released)
#if WINDOWS
                    ||
                    (keyboardState.IsKeyDown(Keys.Left) && lastKeyboardState.IsKeyUp(Keys.Left))
                    
#endif
                    ;
            }
        }

        public bool sysMenuRight
        {
            get
            {
                return (gamePadState.ThumbSticks.Left.X > 0.7f && lastPadState.ThumbSticks.Left.X < 0.7f)
                    ||
                    (gamePadState.DPad.Right == ButtonState.Pressed && lastPadState.DPad.Right == ButtonState.Released)
#if WINDOWS
                    ||
                    (keyboardState.IsKeyDown(Keys.Right) && lastKeyboardState.IsKeyUp(Keys.Right))
#endif
                    ;
            }
        }


        public bool isConnected
        {
            get { return gamePadState.IsConnected; }
        }


        public bool sysMenuUp
        {
            get { return (gamePadState.ThumbSticks.Left.Y > 0.7f && lastPadState.ThumbSticks.Left.Y < 0.7f)
                ||
                (gamePadState.DPad.Up == ButtonState.Pressed && lastPadState.DPad.Up == ButtonState.Released)
#if WINDOWS
                ||
                (keyboardState.IsKeyDown(Keys.Up) && lastKeyboardState.IsKeyUp(Keys.Up))
                ||
                mouseWheelUp
#endif
                ;
            }
        }

        public bool kbEnd
        {
            get
            {

                return keyboardState.IsKeyDown(Keys.End) && lastKeyboardState.IsKeyUp(Keys.End);
            }
        }

        public bool kbHome
        {
            get
            {

                return keyboardState.IsKeyDown(Keys.Home) && lastKeyboardState.IsKeyUp(Keys.Home);
            }
        }

        public bool sysMenuUpHeld
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.PageUp);
            }
        }

        public bool sysMenuDownHeld
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.PageDown);
            }
        }

        public bool sysMenuDown
        {
            get
            { return (gamePadState.ThumbSticks.Left.Y < -0.7f && lastPadState.ThumbSticks.Left.Y > -0.7f)
                ||
                (gamePadState.DPad.Down == ButtonState.Pressed && lastPadState.DPad.Down == ButtonState.Released)
#if WINDOWS
                ||
                (keyboardState.IsKeyDown(Keys.Down) && lastKeyboardState.IsKeyUp(Keys.Down))
                ||
                mouseWheelDown
#endif
                ;
            }
        }





        public bool playbackForwardPress
        {
            get
            {
                return (gamePadState.Buttons.RightShoulder == ButtonState.Pressed
                    &&
                    lastPadState.Buttons.RightShoulder == ButtonState.Released)
                    ||
                    (gamePadState.DPad.Right == ButtonState.Pressed &&
                        lastPadState.DPad.Right == ButtonState.Released)
#if WINDOWS
                        ||
                        (keyboardState.IsKeyDown(Keys.PageDown) &&
                        lastKeyboardState.IsKeyUp(Keys.PageDown))
#endif
                        ;
            }
        }



        public bool playbackBackwardPress
        {
            get
            {
                return (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed
                    &&
                    lastPadState.Buttons.LeftShoulder == ButtonState.Released)
                    ||
                    (gamePadState.DPad.Left == ButtonState.Pressed &&
                        lastPadState.DPad.Left == ButtonState.Released)
#if WINDOWS
                        ||
                        (keyboardState.IsKeyDown(Keys.PageUp) &&
                        lastKeyboardState.IsKeyUp(Keys.PageUp))
#endif
                        ;
            }
        }




        public bool playbackForwardHold
        {
            get
            {
                return gamePadState.Buttons.RightShoulder == ButtonState.Pressed
                    ||
                    gamePadState.DPad.Right == ButtonState.Pressed
#if WINDOWS
                    ||
                    keyboardState.IsKeyDown(Keys.PageDown)
#endif
                    ;
            }
        }


        public bool openLog
        {
            get
            {
                return (gamePadState.Buttons.Back == ButtonState.Pressed && lastPadState.Buttons.Back == ButtonState.Released)
                    ||
                    (keyboardState.IsKeyDown(Keys.Tab) && lastKeyboardState.IsKeyUp(Keys.Tab))
                    ||
                    (keyboardState.IsKeyDown(Keys.L) && lastKeyboardState.IsKeyUp(Keys.L));
            }
        }


        public bool playbackBackwardHold
        {
            get
            {
                return gamePadState.Buttons.LeftShoulder == ButtonState.Pressed
                    ||
                    gamePadState.DPad.Left == ButtonState.Pressed
#if WINDOWS
                    ||
                    keyboardState.IsKeyDown(Keys.PageUp)
#endif
                    ;
            }
        }





        public bool menuNextPressed
        {
            get
            {
                return (gamePadState.Buttons.RightShoulder == ButtonState.Pressed
                    &&
                    lastPadState.Buttons.RightShoulder == ButtonState.Released)
                    ||
                    (gamePadState.DPad.Right == ButtonState.Pressed &&                    
                        lastPadState.DPad.Right == ButtonState.Released)
#if WINDOWS
                    ||
                    (keyboardState.IsKeyDown(Keys.Right) && lastKeyboardState.IsKeyUp(Keys.Right))
#endif
                    ;
            }
        }


        public bool toggleSmite
        {
            get
            {
                return gamePadState.Buttons.LeftShoulder == ButtonState.Pressed &&
                    lastPadState.Buttons.LeftShoulder == ButtonState.Released;

            }
        }


        public bool menuPrevPressed
        {
            get
            {
                return (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed
                    &&
                    lastPadState.Buttons.LeftShoulder == ButtonState.Released)
                    ||
                    (gamePadState.DPad.Left == ButtonState.Pressed &&
                        lastPadState.DPad.Left == ButtonState.Released)
#if WINDOWS
                    ||
                    (keyboardState.IsKeyDown(Keys.Left) && lastKeyboardState.IsKeyUp(Keys.Left))
#endif
                    ;
            }
        }

        public bool menuNextHeld
        {
            get
            {
                return gamePadState.Buttons.RightShoulder == ButtonState.Pressed
                    ||
                    gamePadState.DPad.Right == ButtonState.Pressed;
            }
        }

        public bool menuPrevHeld
        {
            get
            {
                return gamePadState.Buttons.LeftShoulder == ButtonState.Pressed
                    ||
                    gamePadState.DPad.Left == ButtonState.Pressed;
            }
        }

        public bool kbEscPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.Escape) &&
                    lastKeyboardState.IsKeyUp(Keys.Escape);
            }
        }

        public bool kbSlowCam
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.LeftControl) ||
                    keyboardState.IsKeyDown(Keys.RightControl) ||
                    keyboardState.IsKeyDown(Keys.LeftAlt) ||
                    keyboardState.IsKeyDown(Keys.RightAlt);
            }
        }

        public bool kbCameraRaise
        {
            get { return keyboardState.IsKeyDown(Keys.E) || keyboardState.IsKeyDown(Keys.NumPad9); }
        }

        public bool kbCameraLower
        {
            get { return keyboardState.IsKeyDown(Keys.Q) || keyboardState.IsKeyDown(Keys.NumPad7); }
        }

        public float cameraRaise
        {
            get
            {
                return gamePadState.Triggers.Right;
            }
        }

        public float cameraLower
        {
            get
            {
                return gamePadState.Triggers.Left;
            }
        }

        public bool buttonXPressed
        {
            get
            {
                return (gamePadState.Buttons.X == ButtonState.Pressed &&
                    lastPadState.Buttons.X == ButtonState.Released);
            }
        }





        public bool OpenMenu
        {
            get
            {
                return ((gamePadState.Buttons.RightShoulder == ButtonState.Pressed && lastPadState.Buttons.RightShoulder == ButtonState.Released)
#if WINDOWS
                     ||                    
                    (keyboardState.IsKeyDown(Keys.F) && lastKeyboardState.IsKeyUp(Keys.F))
#endif                    
                    );
            }
        }

        public bool advancedMoveToggle
        {
            get
            {
                return (gamePadState.Buttons.X == ButtonState.Pressed && lastPadState.Buttons.X == ButtonState.Released)
#if WINDOWS
                    ||
                    (keyboardState.IsKeyDown(Keys.F) && lastKeyboardState.IsKeyUp(Keys.F))
#endif
                    ;
            }
        }

        public bool kbDelPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.Delete) && lastKeyboardState.IsKeyUp(Keys.Delete);
            }
        }

        public bool kbGPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.G) && lastKeyboardState.IsKeyUp(Keys.G);
            }
        }

        public bool kbTabPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.Tab) && lastKeyboardState.IsKeyUp(Keys.Tab);
            }
        }

        public bool kbResetOrientation
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.R);
            }
        }

        public bool HideHudToggle
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.F12) && kbShiftHeld &&
                    lastKeyboardState.IsKeyUp(Keys.F12);
            }
        }

        public bool kbYPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.Y) && lastKeyboardState.IsKeyUp(Keys.Y);
            }
        }

        public bool kbNPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.N) && lastKeyboardState.IsKeyUp(Keys.N);
            }
        }

        public bool kbFPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.F) && lastKeyboardState.IsKeyUp(Keys.F);
            }
        }

        public bool kb1Pressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.D1) && lastKeyboardState.IsKeyUp(Keys.D1);
            }
        }

        public bool kb2Pressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.D2) && lastKeyboardState.IsKeyUp(Keys.D2);
            }
        }

        public bool kb3Pressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.D3) && lastKeyboardState.IsKeyUp(Keys.D3);
            }
        }

        public bool kb4Pressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.D4) && lastKeyboardState.IsKeyUp(Keys.D4);
            }
        }

#if DEBUG
        public bool debugButton
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.RightControl);
            }
        }

        public bool kbIPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.I) && lastKeyboardState.IsKeyUp(Keys.I);
            }
        }

        

        public bool kbLPressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.L) && lastKeyboardState.IsKeyUp(Keys.L);
            }
        }

        public bool debugButtonPressed
        {
            get
            {
                return (keyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space))
                    ||
                    (gamePadState.Buttons.LeftStick == ButtonState.Pressed && gamePadState.Buttons.RightStick == ButtonState.Pressed);
            }
        }

        public bool debugF1Pressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.F1) && lastKeyboardState.IsKeyUp(Keys.F1);
            }
        }

        public bool debugF2Pressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.F2) && lastKeyboardState.IsKeyUp(Keys.F2);
            }
        }

        public bool debugF3Pressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.F3) && lastKeyboardState.IsKeyUp(Keys.F3);
            }
        }

        public bool debugF4Pressed
        {
            get
            {
                return keyboardState.IsKeyDown(Keys.F4) && lastKeyboardState.IsKeyUp(Keys.F4);
            }
        }
#endif

        public bool buttonBackHeld
        {
            get
            {
                return gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Tab);
            }
        }

        public bool buttonBackPressed
        {
            get
            {
                
                return gamePadState.Buttons.Back == ButtonState.Pressed &&
                     lastPadState.Buttons.Back == ButtonState.Released;
            }
        }

        public bool buttonStartHeld
        {
            get
            {
                return gamePadState.Buttons.Start == ButtonState.Pressed;
            }
        }

        public bool buttonStartPressed
        {
            get
            {
                return gamePadState.Buttons.Start == ButtonState.Pressed && lastPadState.Buttons.Start == ButtonState.Released;
            }
        }

        KeyboardState lastKeyboardState;
        KeyboardState keyboardState;


        public KeyboardState rawKeyboardState
        {
            get { return keyboardState; }
        }


        Vector2 mouseDifference = Vector2.Zero;
        public Vector2 MouseDifference
        {
            get { return mouseDifference; }
        }


        GamePadState lastPadState;
        /*public GamePadState LastPadState
        {
            get { return lastPadState; }
        }*/

        GamePadState gamePadState;
        /*public GamePadState GamePadState
        {
            get { return gamePadState; }
        }*/


        MouseState pristineMouseInfo;
        MouseState lastMouseInfo;
        MouseState mouseInfo;
        Vector2 relativeMousePosition;


        //constructor.
        public InputManager(PlayerIndex index)
        {
            mouseSmoothingCache = new Vector2[10];

            mouseMovement = new Vector2[2];
            mouseMovement[0].X = 0.0f;
            mouseMovement[0].Y = 0.0f;
            mouseMovement[1].X = 0.0f;
            mouseMovement[1].Y = 0.0f;

            this.index = index;

        }

        public void Update(GameTime gameTime, PlayerIndex playerIndex, PlayerCommander player, bool clampMouse)
        {
#if WINDOWS
            //if game is not the current focus, then ignore all input devices.
            if (!FrameworkCore.isActive)
            {
                ClearAll();
                return;
            }
            
#endif



            try
            {
                lastPadState = gamePadState;
                gamePadState = GamePad.GetState(playerIndex, GamePadDeadZone.Circular);
            }
            catch
            {
            }
            
#if WINDOWS
            if (pristineMouseInfo == null)
            {
                MouseState maybePristine = Mouse.GetState();
                if (maybePristine.LeftButton == ButtonState.Released &&
                    maybePristine.RightButton == ButtonState.Released)
                {
                    pristineMouseInfo = maybePristine;
                }
            }

            if (player == null || (player != null && player.mouseEnabled))
            {
                lastKeyboardState = keyboardState;
                keyboardState = Keyboard.GetState();

                UpdateMouseInfo();

#if SDL2
                bool resetCursor = false;
                if (Mouse.IsRelativeMouseModeEXT != mouseRightHeld)
                {
                    // To avoid jitter, just wipe the state entirely to benefit the next mode
                    if (mouseRightHeld)
                    {
                        ResetMouseState(0, 0);
                    }
                    else
                    {
                        resetCursor = true;
                    }
                }
                Mouse.IsRelativeMouseModeEXT = mouseRightHeld;
                if (resetCursor)
                {
                    ForceMouseCenter();
                }
                FrameworkCore.Game.IsMouseVisible = FrameworkCore.options.hardwaremouse && !mouseRightHeld;
#endif
                if (mouseRightHeld)
                {
                    ClampMouse(clampMouse);

                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(100).TotalMilliseconds);
                    mouseCameraTimer = MathHelper.Clamp(mouseCameraTimer + delta, 0, 1);

                    if (mouseCameraTimer >= 1 && !MouseCameraMode)
                        MouseCameraMode = true;
                }
                else if (MouseCameraMode || mouseCameraTimer > 0)
                {
                    MouseCameraMode = false;
                    mouseCameraTimer = 0;
                }
            }
#endif
        }



        public void ClearAll()
        {
            //special case when the "buy" button is pressed and app gets minimized.
            //clear all inputstates.
            if (pristineMouseInfo == null)
                return;

            mouseInfo = pristineMouseInfo;
            lastMouseInfo = pristineMouseInfo;
        }



        float mouseCameraTimer = 0;




        private void UpdateMouseInfo()
        {
            lastMouseInfo = mouseInfo;
            mouseInfo = Mouse.GetState();
        }
        
        private void ResetMouseState(int x, int y)
        {
            mouseInfo = new MouseState(
                x,
                y,
                mouseInfo.ScrollWheelValue,
                mouseInfo.LeftButton,
                mouseInfo.MiddleButton,
                mouseInfo.RightButton,
                mouseInfo.XButton1,
                mouseInfo.XButton2
            );
            lastMouseInfo = new MouseState(
                x,
                y,
                lastMouseInfo.ScrollWheelValue,
                lastMouseInfo.LeftButton,
                lastMouseInfo.MiddleButton,
                lastMouseInfo.RightButton,
                lastMouseInfo.XButton1,
                lastMouseInfo.XButton2
            );

            // Also wipe the smoothing caches, otherwise previous
            // smoothing will cause jumps in future frames
            Array.Clear(mouseSmoothingCache, 0, mouseSmoothingCache.Length);
            Array.Clear(mouseMovement, 0, mouseMovement.Length);
        }

        public void ForceMouseCenter()
        {
            Vector2 screenCenter = Helpers.GetScreenCenter();

            if (FrameworkCore.players.Count > 1 && FrameworkCore.gameState == GameState.Play)
                screenCenter.X /= 2;

            Mouse.SetPosition((int)screenCenter.X, (int)screenCenter.Y);
            ResetMouseState((int)screenCenter.X, (int)screenCenter.Y);
        }

        private void ClampMouse(bool centerClamp)
        {
#if SDL2
            // With relative mouse mode, SDL does the cursor warping for us.
            // Instead, we just get the relative state and pass that directly
            // to the filtering/smoothing functions.
            float deltaX = mouseInfo.X;
            float deltaY = -mouseInfo.Y;
#else
            int screenWidth = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;

            if (FrameworkCore.players.Count > 1)
            {
                screenWidth /= 2;
            }

            int centerX = screenWidth / 2;
            int centerY = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2;
            float deltaX = (centerX - mouseInfo.X) * -1;
            float deltaY = centerY - mouseInfo.Y;

            if (centerClamp)
                Mouse.SetPosition(centerX, centerY);
#endif

            //mouse sensitivity.
            deltaY *= 0.16f;
            deltaX *= 0.16f;

            PerformMouseFiltering(deltaX, deltaY);
            PerformMouseSmoothing(mouseDifference.X, mouseDifference.Y);
        }





        #region Mouse Functions
        float mouseSmoothingSensitivity = 0.5f;
        private Vector2[] mouseSmoothingCache;
        private void PerformMouseFiltering(float x, float y)
        {
            // Shuffle all the entries in the cache.
            // Newer entries at the front. Older entries towards the back.
            for (int i = mouseSmoothingCache.Length - 1; i > 0; --i)
            {
                mouseSmoothingCache[i].X = mouseSmoothingCache[i - 1].X;
                mouseSmoothingCache[i].Y = mouseSmoothingCache[i - 1].Y;
            }

            // Store the current mouse movement entry at the front of cache.
            mouseSmoothingCache[0].X = x;
            mouseSmoothingCache[0].Y = y;

            float averageX = 0.0f;
            float averageY = 0.0f;
            float averageTotal = 0.0f;
            float currentWeight = 1.0f;

            // Filter the mouse movement with the rest of the cache entries.
            // Use a weighted average where newer entries have more effect than
            // older entries (towards the back of the cache).
            for (int i = 0; i < mouseSmoothingCache.Length; ++i)
            {
                averageX += mouseSmoothingCache[i].X * currentWeight;
                averageY += mouseSmoothingCache[i].Y * currentWeight;
                averageTotal += 1.0f * currentWeight;
                currentWeight *= mouseSmoothingSensitivity;
            }

            // Calculate the new smoothed mouse movement.
            mouseDifference.X = averageX / averageTotal;
            mouseDifference.Y = averageY / averageTotal;
        }

        private Vector2[] mouseMovement;
        private int mouseIndex = 0;
        private void PerformMouseSmoothing(float x, float y)
        {
            mouseMovement[mouseIndex].X = x;
            mouseMovement[mouseIndex].Y = y;

            mouseDifference.X = (mouseMovement[0].X + mouseMovement[1].X) * 0.5f;
            mouseDifference.Y = (mouseMovement[0].Y + mouseMovement[1].Y) * 0.5f;

            mouseIndex ^= 1;
            mouseMovement[mouseIndex].X = 0.0f;
            mouseMovement[mouseIndex].Y = 0.0f;
        }
        #endregion

    }
}
