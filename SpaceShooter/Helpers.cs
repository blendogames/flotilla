#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using System.Collections;
using System.Resources;
using System.Globalization;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
using System.Net;
#endregion

namespace SpaceShooter
{
    public static class OldXNAColor
    {
        public static Color TransparentWhite = new Color(255, 255, 255, 0);
        public static Color TransparentBlack = new Color(0, 0, 0, 0);
    }

#if SDL2
    public static class YesNoPopup
    {
        public static bool Show(string title, string message)
        {
            SDL2.SDL.SDL_MessageBoxData mbd = new SDL2.SDL.SDL_MessageBoxData();
            mbd.flags = SDL2.SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR;
            mbd.window = IntPtr.Zero;
            mbd.title = title;
            mbd.message = message;
            mbd.numbuttons = 2;
            mbd.buttons = new SDL2.SDL.SDL_MessageBoxButtonData[]
            {
                new SDL2.SDL.SDL_MessageBoxButtonData()
                {
                    buttonid = 0,
                    flags = SDL2.SDL.SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT,
                    text = "Yes"
                },
                new SDL2.SDL.SDL_MessageBoxButtonData()
                {
                    buttonid = 1,
                    flags = SDL2.SDL.SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_ESCAPEKEY_DEFAULT,
                    text = "No"
                },
            };
            mbd.colorScheme = null;
            int button;
            SDL2.SDL.SDL_ShowMessageBox(ref mbd, out button);
            return button == 0;
        }
    }
#endif

    public static class BlendStateHelper
    {
        public static bool AlphaBlendEnable = false;
        public static Blend SourceBlend = Blend.One;
        public static Blend DestinationBlend = Blend.Zero;
        public static ColorWriteChannels ColorWriteChannels = ColorWriteChannels.All;

        private static Dictionary<uint, BlendState> blendCache = new Dictionary<uint, BlendState>();

        public static void BeginApply(GraphicsDevice device)
        {
            Debug.Assert(device.BlendState.ColorSourceBlend == device.BlendState.AlphaSourceBlend);
            Debug.Assert(device.BlendState.ColorDestinationBlend == device.BlendState.AlphaDestinationBlend);
            SourceBlend = device.BlendState.ColorSourceBlend;
            DestinationBlend = device.BlendState.ColorDestinationBlend;
            AlphaBlendEnable = !(SourceBlend == Blend.One && DestinationBlend == Blend.Zero);
            ColorWriteChannels = device.BlendState.ColorWriteChannels;
        }

        public static void EndApply(GraphicsDevice device)
        {
            Blend src, dst;
            if (AlphaBlendEnable)
            {
                src = SourceBlend;
                dst = DestinationBlend;
            }
            else
            {
                src = Blend.One;
                dst = Blend.Zero;
            }
            
            uint hash = (
                (((uint) src) << 0) |
                (((uint) dst) << 8) |
                (((uint) ColorWriteChannels) << 16)
            );

            BlendState state;
            if (!blendCache.TryGetValue(hash, out state))
            {
                state = new BlendState()
                {
                    ColorSourceBlend = src,
                    AlphaSourceBlend = src,
                    ColorDestinationBlend = dst,
                    AlphaDestinationBlend = dst,
                    ColorWriteChannels = ColorWriteChannels
                };
                blendCache.Add(hash, state);
            }
            device.BlendState = state;
        }
    }

    public static class ParticleTexture
    {
        public static string bigglow = "bigglow";
        public static string explosion = "explosiontexture";
        public static string smoke = "smoketexture";
        public static string basic = "particletexture";
        public static string spark = "sparktexture";
        public static string sparkGroup = "sparkgroup";
        public static string debris1 = "debris1";
        public static string muzzleflash = "muzzleflash";
        public static string arc1 = "arctexture1";
    }


    public static class sprite
    {
        public static Rectangle crosshair = new Rectangle(0, 0, 32, 32);
        public static Rectangle dot = new Rectangle(33, 1, 30, 30);
        public static Rectangle blank = new Rectangle(80, 16, 8, 8);

        public static Rectangle barFill = new Rectangle(300, 96, 5, 5);

        public static Rectangle danger = new Rectangle(1, 33, 30, 30);
        public static Rectangle bubble = new Rectangle(32, 32, 64, 32);
        public static Rectangle tab = new Rectangle(0, 64, 64, 96);
        public static Rectangle tabInside = new Rectangle(64, 64, 64, 96);
        public static Rectangle roundBox = new Rectangle(128, 64, 96, 32);
        public static Rectangle roundSquare = new Rectangle(128, 96, 64, 64);
        public static Rectangle giantRectangle = new Rectangle(0, 368, 512, 144);
        public static Rectangle tabRectangle = new Rectangle(144, 336, 200, 32);

        public static Rectangle camera = new Rectangle(0, 160, 64, 64);
        public static Rectangle clock  = new Rectangle(128, 160, 64, 64);
        public static Rectangle cancel = new Rectangle(64, 160, 64, 64);
        public static Rectangle noSign = new Rectangle(256, 160, 64, 64);

        public static Rectangle glow = new Rectangle(224, 32, 32, 32);
        public static Rectangle bigCircle = new Rectangle(512, 0, 512, 512);

        public static Rectangle checkmark = new Rectangle(224, 64, 32, 32);
        public static Rectangle bigCrosshair = new Rectangle(256, 0, 64, 64);
        public static Rectangle tinyHourglass = new Rectangle(256, 64, 32, 32);

        public static Rectangle blendo = new Rectangle(0, 512, 512, 64);
        public static Rectangle games = new Rectangle(0, 576, 160, 32);

        public static Rectangle xboxRing = new Rectangle(288, 64, 32, 32);
        public static Rectangle computer = new Rectangle(320, 64, 32, 32);

        public static Rectangle vistaBox = new Rectangle(512, 512, 512, 48);

        public static Rectangle arrow = new Rectangle(192, 96, 32, 32);

        public static Rectangle playerMarker = new Rectangle(288, 96, 12, 12);


        public static Rectangle checkboxBox = new Rectangle(224, 96, 32, 32);
        public static Rectangle checkboxCheck = new Rectangle(256, 96, 32, 32);

        public static Rectangle tinyLogo = new Rectangle(192, 128, 128, 32);

        public static Rectangle tinyArrow = new Rectangle(153, 238, 22, 20);

        public static Rectangle inventoryBox = new Rectangle(512, 560, 512, 256);
        public static Rectangle sparkle = new Rectangle(384, 240, 128, 128);


        public static Rectangle stamp = new Rectangle(844, 896, 180, 128);



        public static Rectangle roundSquareSelector = new Rectangle(192, 160, 64, 64);


        public static Rectangle roundSquareSelectorHalf1 = new Rectangle(192, 160, 32, 64);
        public static Rectangle roundSquareSelectorHalf2 = new Rectangle(192+32, 160, 32, 64);

        public static Rectangle mouseCursor = new Rectangle(320, 32, 32, 32);
        public static Rectangle fingerCursor = new Rectangle(320, 0, 32, 32);
        public static Rectangle updownCursor = new Rectangle(384, 32, 32, 32);

        public static Rectangle mouseIcon = new Rectangle(352, 64, 32, 32);

        public static Rectangle roundCircle = new Rectangle(144, 272, 64, 64);


        public static Rectangle star = new Rectangle(288, 108, 20, 20);

        public static Rectangle padlock = new Rectangle(352, 128, 32, 32);
        public static Rectangle padlock2 = new Rectangle(384, 128, 32, 32);


        public static Rectangle vignette = new Rectangle(514, 882, 140, 140);


        public static Rectangle cloud = new Rectangle(256, 704, 256,256);

        public static Rectangle helpArrow = new Rectangle(780, 896, 64, 32);
        public static Rectangle flamingo = new Rectangle(723, 1024, 300, 512);




        public static class tutorial
        {
            public static Rectangle messageBox = new Rectangle(512, 816, 512, 64);
        }


        public static class inventory
        {
            public static Rectangle MuyosShield = new Rectangle(0, 960, 64, 64);
            public static Rectangle FireCon = new Rectangle(64, 960, 64, 64);
            public static Rectangle AutoDoc = new Rectangle(128, 960, 64, 64);
            public static Rectangle Engine = new Rectangle(192, 960, 64, 64);
            public static Rectangle RailChamber = new Rectangle(256, 960, 64, 64);

            public static Rectangle Totem = new Rectangle(320, 960, 64, 64);
            public static Rectangle Gauntlet = new Rectangle(384, 960, 64, 64);
            public static Rectangle BlueGauntlet = new Rectangle(448, 960, 64, 64);



            public static Rectangle RoachShield = new Rectangle(0, 896, 64, 64);
            public static Rectangle BeamShield = new Rectangle(64, 896, 64, 64);
            public static Rectangle Eyeball = new Rectangle(128, 896, 64, 64);
            public static Rectangle Boot = new Rectangle(192, 896, 64, 64);
            public static Rectangle Heart = new Rectangle(192, 832, 64, 64);

        }

        /// <summary>
        /// These refer to the EVENTSHEET sprite sheet.
        /// </summary>
        public static class eventSprites
        {
            public static Rectangle mainBG = new Rectangle(512, 0, 512, 320);
            public static Rectangle itemsBG = new Rectangle(512, 320, 512, 120);

            public static Rectangle spacehulk = new Rectangle(0, 0, 512, 128);
            public static Rectangle cats = new Rectangle(0, 128, 512, 128);
            public static Rectangle aurora = new Rectangle(0, 256, 512, 128);
            public static Rectangle hitchhikers = new Rectangle(0, 384, 512, 128);
            public static Rectangle hippo = new Rectangle(0, 512, 512, 128);
            public static Rectangle pigs = new Rectangle(0, 640, 512, 128);
            public static Rectangle deer = new Rectangle(0, 768, 512, 128);
            public static Rectangle toucans = new Rectangle(0, 896, 512, 128);
            public static Rectangle assassin = new Rectangle(0, 1024, 512, 128);
            public static Rectangle chickens = new Rectangle(0, 1152, 512, 128);
            public static Rectangle babyyetis = new Rectangle(0, 1280, 512, 128);
            public static Rectangle implementors = new Rectangle(0, 1408, 512, 128);
            public static Rectangle yetis = new Rectangle(0, 1536, 512, 128);
            public static Rectangle downey = new Rectangle(0, 1664, 512, 128);
            public static Rectangle queen = new Rectangle(0, 1792, 512, 128);
            public static Rectangle swan = new Rectangle(0, 1920, 512, 128);



            public static Rectangle zombies = new Rectangle(512, 512, 512, 128);
            public static Rectangle professors = new Rectangle(512, 640, 512, 128);
            public static Rectangle riot = new Rectangle(512, 768, 512, 128);
            public static Rectangle lantern = new Rectangle(512, 896, 512, 128);
            public static Rectangle panda = new Rectangle(512, 1024, 512, 128);
            public static Rectangle leopard = new Rectangle(512, 1152, 512, 128);
            public static Rectangle yak = new Rectangle(512, 1280, 512, 128);
            public static Rectangle rhino = new Rectangle(512, 1408, 512, 128);
            public static Rectangle destroyers = new Rectangle(512, 1536, 512, 128);
            public static Rectangle bouquet = new Rectangle(512, 1664, 512, 128);
            public static Rectangle angryyetis = new Rectangle(512, 1792, 512, 128);
            public static Rectangle fish = new Rectangle(512, 1920, 512, 128);


            public static Rectangle catHelmet = new Rectangle(1024, 0, 512, 128);
            public static Rectangle psychicDogs = new Rectangle(1024, 128, 512, 128);
            public static Rectangle spiderTwain = new Rectangle(1024, 256, 512, 128);
            public static Rectangle bitterMelon = new Rectangle(1024, 384, 512, 128);
            public static Rectangle pigeyeHippo = new Rectangle(1024, 512, 512, 128);
            public static Rectangle pandaBitterMelon = new Rectangle(1024, 640, 512, 128);
            public static Rectangle fans = new Rectangle(1024, 768, 512, 128);
            public static Rectangle unicorn = new Rectangle(1024, 896, 512, 128);
            public static Rectangle afrodita = new Rectangle(1024, 1024, 512, 128);
            public static Rectangle koala = new Rectangle(1024, 1152, 512, 128);
            public static Rectangle flamingo = new Rectangle(1024, 1280, 512, 128);
            public static Rectangle penguin  = new Rectangle(1024, 1408, 512, 128);
            public static Rectangle crocodile = new Rectangle(1024, 1536, 512, 128);
            public static Rectangle owl = new Rectangle(1024, 1664, 512, 128);
            public static Rectangle owl2 = new Rectangle(1024, 1792, 512, 128);
            public static Rectangle wine = new Rectangle(1024, 1920, 512, 128);


            public static Rectangle mouse = new Rectangle(1536, 0, 512, 128);
        }

        public static class windowIcon
        {
            public static Rectangle exclamation = new Rectangle(416, 0, 96, 96);
            public static Rectangle error = new Rectangle(416, 96, 96, 96);
            public static Rectangle info = new Rectangle(748, 928, 96, 96);
        }
        
        public static class ships
        {
            public static Rectangle beamFrigate = new Rectangle(160, 576, 80, 50);
            public static Rectangle destroyer = new Rectangle(240, 576, 80, 50);
            public static Rectangle capitalship = new Rectangle(320, 576, 80, 50);
            public static Rectangle gunship = new Rectangle(400, 576, 80, 50);
            public static Rectangle dreadnought = new Rectangle(160, 626, 80, 50);
            public static Rectangle beamGunship = new Rectangle(240, 626, 80, 50);
            public static Rectangle fighter = new Rectangle(320, 626, 80, 50);
        }

        public static class icons
        {
            public static Rectangle circle = new Rectangle(0, 224, 48, 48);
            public static Rectangle circleInvert = new Rectangle(96, 224, 48, 48);
            public static Rectangle circleDotted = new Rectangle(0, 320, 48, 48);
            public static Rectangle circleInvertDotted = new Rectangle(96, 320, 48, 48);

            public static Rectangle trophy = new Rectangle(320, 128, 32, 32);
            public static Rectangle skull = new Rectangle(320, 96, 32, 32);
            public static Rectangle veterancy = new Rectangle(352, 96, 32, 32);

            public static Rectangle wrench = new Rectangle(48, 224, 48, 48);
            public static Rectangle done = new Rectangle(144, 224, 48, 48);
            public static Rectangle flankSpeed = new Rectangle(192, 224, 48, 48);
            public static Rectangle move = new Rectangle(0, 272, 48, 48);
            public static Rectangle focusFire = new Rectangle(48, 272, 48, 48);

            public static Rectangle rewind = new Rectangle(352, 0, 32, 32);
            public static Rectangle play = new Rectangle(384, 0, 32, 32);

            public static Rectangle flamingo = new Rectangle(96, 272, 48, 48);
        }

        public static class buttons
        {
            public static Rectangle a = new Rectangle(96, 0, 32, 32);
            public static Rectangle x = new Rectangle(128, 0, 32, 32);
            public static Rectangle b = new Rectangle(160, 0, 32, 32);
            public static Rectangle y = new Rectangle(192, 0, 32, 32);
            public static Rectangle blank = new Rectangle(224, 0, 32, 32);
            public static Rectangle lb = new Rectangle(96, 32, 64, 32);
            public static Rectangle rb = new Rectangle(160, 32, 64, 32);
            public static Rectangle lbrb = new Rectangle(96, 32, 128, 32);

            

            public static Rectangle rightstick = new Rectangle(240, 224, 48, 48);
            public static Rectangle leftstick = new Rectangle(288, 224, 48, 48);
            public static Rectangle leftstickTiny = new Rectangle(352, 32, 32, 32);


            public static Rectangle lefttrigger = new Rectangle(256, 272, 48, 64);
            public static Rectangle righttrigger = new Rectangle(208, 272, 48, 64);

            public static Rectangle mouseLeftClick = new Rectangle(368, 160, 48, 64);
            public static Rectangle mouseRightClick = new Rectangle(320, 160, 48, 64);
            public static Rectangle mouseMiddleClick = new Rectangle(416, 192, 32, 40);
            public static Rectangle mouseLeftClickTiny = new Rectangle(448, 192, 32, 40);

            public static Rectangle kbWasd = new Rectangle(0, 608, 128, 80);
            public static Rectangle kbShift = new Rectangle(128, 676, 72, 32);

            public static Rectangle leftRight = new Rectangle(0, 688, 40, 20);
            public static Rectangle upDown =    new Rectangle(0, 708, 20, 40);

            public static Rectangle spacebar = new Rectangle(20, 720, 112, 32);
            public static Rectangle bigMouse = new Rectangle(0, 768, 80, 128);


            public static Rectangle xboxController = new Rectangle(0, 1024, 360, 256);
            public static Rectangle xboxControllerTop = new Rectangle(360, 1080, 360, 200);
        }
    }

    public static class Helpers
    {

#if WINDOWS && STEAM
        public const int STEAMAPPID = 55000;
#endif

        public const int GRIDSIZE = 1024;

        public const int ENEMYKILLEDPOINTS = 10;

        public const float DISCSIZE = 1f;
        public const int BLINKTIME = 400;

        public const float FORWARDARROWLENGTH = 16;
        public const float UPARROWLENGTH = 3;

        public const int LEGENDGAPSIZE = 16;

        public const int MAXROUNDTIME = 30000; //how long is the actiontimer.

        public const int MAXEVENTS = 14;

        public static Color ITEMCOLOR = new Color(40, 100, 230);

        public static readonly DepthStencilState DepthWrite = new DepthStencilState()
        {
            DepthBufferEnable = false
        };

        public static Vector2 SpriteCenter(Rectangle rect)
        {
            return new Vector2(rect.Width / 2f, rect.Height / 2f);
        }

        public static Vector2 GetScreenPos(Camera camera, Vector3 worldPos)
        {
            Vector3 screenSpace = FrameworkCore.Graphics.GraphicsDevice.Viewport.Project(
                                worldPos, camera.Projection, camera.View,
                                Matrix.Identity);

            Vector2 screenPos = new Vector2(screenSpace.X, screenSpace.Y);

            screenPos.X -= FrameworkCore.Graphics.GraphicsDevice.Viewport.X;

            return screenPos;
        }

        public static float SizeInPixels(Camera camera, Vector3 meshPosition, float meshRadius)
        {
            float sizeInPixels;
            float distance = (meshPosition - camera.CameraPosition).Length();
            sizeInPixels = 10.0f;
            float radius = meshRadius;
            if (distance > radius)
            {
                float angularSize = (float)Math.Tan(radius / distance);
                sizeInPixels = angularSize * FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / camera.FieldOfView;
            }

            return sizeInPixels;
        }

        public static float GetDelta(GameTime gameTime, int Milliseconds)
        {
            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                TimeSpan.FromMilliseconds(Milliseconds).TotalMilliseconds);

            return delta;
        }

        public static float UnsignedAngleBetweenTwoV3(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();
            double Angle = (float)Math.Acos(Vector3.Dot(v1, v2));
            return (float)Angle;
        }

        public static float GetDot(Vector3 origin, Vector3 target, Vector3 originFacing)
        {
            Vector3 dir = Vector3.Normalize(target - origin);
            float frontDot = Vector3.Dot(originFacing, dir);

            return frontDot;
        }

        public static float TurnToFace(float rotation, float target, float turnRate)
        {
            float difference = rotation - target;

            while (difference > MathHelper.Pi)
                difference -= MathHelper.TwoPi;

            while (difference < -MathHelper.Pi)
                difference += MathHelper.TwoPi;

            turnRate *= Math.Abs(difference);

            if (difference < 0)
                return rotation + Math.Min(turnRate, -difference);
            else
                return rotation - Math.Min(turnRate, difference);
        }

        // O is your object's position  
        // P is the position of the object to face  
        // U is the nominal "up" vector (typically Vector3.Y)  
        // Note: this does not work when O is straight below or straight above PMatrix   
        public static Matrix RotateToFace(Vector3 Origin, Vector3 Target, Vector3 UpVec)
        {
            Vector3 D = (Origin - Target);
            Vector3 Right = Vector3.Cross(UpVec, D);
            Vector3.Normalize(ref Right, out Right);
            Vector3 Backwards = Vector3.Cross(Right, UpVec);
            Vector3.Normalize(ref Backwards, out Backwards);
            Vector3 Up = Vector3.Cross(Backwards, Right);
            Matrix rot = new Matrix(Right.X, Right.Y, Right.Z, 0, Up.X, Up.Y, Up.Z, 0, Backwards.X, Backwards.Y, Backwards.Z, 0, 0, 0, 0, 1); return rot;
        }

        public static Color transColor(Color color, float transition)
        {
            return Color.Lerp(new Color(color.R, color.G, color.B, 0),
                new Color(color.R, color.G, color.B),
                transition);
        }

        public static Color transColor(Color color)
        {
            return new Color(color.R, color.G, color.B, 0);
        }

        public static void DrawBar(Vector2 barPos, float amount, int barWidth, int barHeight, int borderWidth, Color barColor, Color bgColor)
        {
            Point boxPos = new Point((int)barPos.X, (int)barPos.Y);

            Point topLeftCorner = boxPos;
            topLeftCorner.X -= barWidth / 2;
            topLeftCorner.Y -= barHeight / 2;

            Rectangle boxRect = new Rectangle();

            //draw the shaded background.
            boxRect = new Rectangle(topLeftCorner.X, topLeftCorner.Y, barWidth, barHeight);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxRect, sprite.blank, bgColor);


            //draw the bar filling.
            int fillingWidth = (int)((barWidth - borderWidth * 2) * amount);
            boxRect = new Rectangle(topLeftCorner.X + borderWidth, topLeftCorner.Y + borderWidth, fillingWidth, barHeight - borderWidth * 2);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxRect, sprite.blank, barColor);


            //draw the 4 borders.
            boxRect = new Rectangle(topLeftCorner.X, topLeftCorner.Y, barWidth, borderWidth);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxRect, sprite.blank, Color.Black);

            boxRect = new Rectangle(topLeftCorner.X, topLeftCorner.Y + barHeight - borderWidth, barWidth, borderWidth);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxRect, sprite.blank, Color.Black);

            boxRect = new Rectangle(topLeftCorner.X, topLeftCorner.Y, borderWidth, barHeight);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxRect, sprite.blank, Color.Black);

            boxRect = new Rectangle(topLeftCorner.X + barWidth - borderWidth, topLeftCorner.Y, borderWidth, barHeight);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxRect, sprite.blank, Color.Black);
        }





        public static float DrawLegendRow2Left(string text, Rectangle spriteRect, float transition)
        {
            float posX = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;
            posX = 150;
            return DrawLegendRow2LeftAt(text, spriteRect, transition, posX);
        }

        public static float DrawLegendRow2LeftAt(string text, Rectangle spriteRect, float transition, float posX)
        {
            Point safeScreen = new Point(0, -100);
            float gapSize = 3;
            Vector2 textMeasure = FrameworkCore.Serif.MeasureString(text);
            Vector2 rightBottom = new Vector2(posX,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            Vector2 textPos = rightBottom + new Vector2(safeScreen.X, safeScreen.Y);
            
            textPos.Y -= textMeasure.Y * 2.5f;

            textPos = Vector2.Lerp(textPos + new Vector2(0, 50), textPos, transition);

            Color fontColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, transition);
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), transition);
            //FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, text,
            //    textPos, fontColor, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            DrawOutline(FrameworkCore.Serif, text, textPos, fontColor, bgColor, 0, Vector2.Zero, 1);

            Vector2 imagePos = textPos;
            imagePos.Y += textMeasure.Y / 2;
            imagePos.X -= Helpers.SpriteCenter(spriteRect).X;

            imagePos.X -= gapSize;

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, imagePos, spriteRect, fontColor,
                0, Helpers.SpriteCenter(spriteRect), 1, SpriteEffects.None, 0);

            return (textPos.X - gapSize - spriteRect.Width);
        }






        public static float DrawLegendRow2(string text, Rectangle spriteRect, float transition)
        {
            float posX = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;
            posX -= 108;
            return DrawLegendRow2At(text, spriteRect, transition, posX);
        }

        public static float DrawLegendRow2At(string text, Rectangle spriteRect, float transition, float posX)
        {
            Point safeScreen = new Point(0, -100);
            float gapSize = 3;
            Vector2 textMeasure = FrameworkCore.Serif.MeasureString(text);
            Vector2 rightBottom = new Vector2(posX,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            Vector2 textPos = rightBottom + new Vector2(safeScreen.X, safeScreen.Y);
            textPos.X -= textMeasure.X;
            textPos.Y -= textMeasure.Y * 2.5f;

            textPos = Vector2.Lerp(textPos + new Vector2(0, 50), textPos, transition);

            Color fontColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, transition);
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), transition);
            //FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, text,
            //    textPos, fontColor, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            DrawOutline(FrameworkCore.Serif, text, textPos, fontColor, bgColor, 0, Vector2.Zero, 1);

            Vector2 imagePos = textPos;
            imagePos.Y += textMeasure.Y / 2;
            imagePos.X -= Helpers.SpriteCenter(spriteRect).X;

            imagePos.X -= gapSize;

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, imagePos, spriteRect, fontColor,
                0, Helpers.SpriteCenter(spriteRect), 1, SpriteEffects.None, 0);

            return (textPos.X - gapSize - spriteRect.Width);
        }


        public static float DrawLegend(string text, Rectangle spriteRect, float transition)
        {
            float posX = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width;
            posX -= 108;
            return DrawLegendAt(text, spriteRect, transition, posX);            
        }

        public static float DrawLegendAt(string text, Rectangle spriteRect, float transition, float posX)
        {
            Point safeScreen = new Point(0, -100);
            float gapSize = 3;
            Vector2 textMeasure = FrameworkCore.Serif.MeasureString(text);
            Vector2 rightBottom = new Vector2(posX,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            Vector2 textPos = rightBottom + new Vector2(safeScreen.X, safeScreen.Y);
            textPos.X -= textMeasure.X;
            textPos.Y -= textMeasure.Y;

            textPos = Vector2.Lerp(textPos + new Vector2(0, 50), textPos, transition);

            Color fontColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, transition);
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0,0,0,128), transition);
            //FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, text,
            //    textPos, fontColor, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            DrawOutline(FrameworkCore.Serif, text, textPos, fontColor, bgColor, 0, Vector2.Zero, 1);

            Vector2 imagePos = textPos;
            imagePos.Y += textMeasure.Y / 2;
            imagePos.X -= Helpers.SpriteCenter(spriteRect).X;

            imagePos.X -= gapSize;

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, imagePos, spriteRect, fontColor,
                0, Helpers.SpriteCenter(spriteRect), 1, SpriteEffects.None, 0);            

            return (textPos.X - gapSize - spriteRect.Width);
        }




        public static float DrawLegendLeft(string text, Rectangle spriteRect, float transition)
        {
            float posX = 150;
            return DrawLegendLeftAt(text, spriteRect, transition, posX);
        }

        public static float DrawLegendLeftAt(string text, Rectangle spriteRect, float transition, float posX)
        {
            Point safeScreen = new Point(0, -100);
            float gapSize = 3;
            Vector2 textMeasure = FrameworkCore.Serif.MeasureString(text);
            Vector2 leftBottom = new Vector2(posX,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            Vector2 textPos = leftBottom + new Vector2(safeScreen.X, safeScreen.Y);
            textPos.Y -= textMeasure.Y;
            textPos = Vector2.Lerp(textPos + new Vector2(0, 50), textPos, transition);

            Vector2 imagePos = textPos;
            imagePos.Y += textMeasure.Y / 2;
            imagePos.X += Helpers.SpriteCenter(spriteRect).X/2;

            Color fontColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, transition);
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), transition);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, imagePos, spriteRect, fontColor,
                0, Helpers.SpriteCenter(spriteRect), 1, SpriteEffects.None, 0);

            Vector2 stringPos = textPos;
            stringPos.X += gapSize + spriteRect.Width;
            DrawOutline(FrameworkCore.Serif, text, stringPos,
                fontColor, bgColor, 0, Vector2.Zero, 1);
            

            return (textPos.X + gapSize + spriteRect.Width + textMeasure.X);
        }

        public static float PopLerp(float transition, float minSize, float maxSize, float normalSize)
        {
            if (transition >= 1)
                return normalSize;

            if (transition <= 0)
                return minSize;

            float popPoint = 0.6f;
            float popRemnant = 0.4f;

            if (transition < popPoint)
            {
                transition /= popPoint;
                return MathHelper.Lerp(minSize, maxSize, transition);
            }

            transition -= popPoint;
            transition /= popRemnant;

            return MathHelper.Lerp(maxSize, normalSize, transition);
        }

        public static Vector2 PopLerp(float transition, Vector2 minSize, Vector2 maxSize, Vector2 normalSize)
        {
            if (transition >= 1)
                return normalSize;

            if (transition <= 0)
                return minSize;

            float popPoint = 0.6f;
            float popRemnant = 0.4f;

            if (transition < popPoint)
            {
                transition /= popPoint;
                return Vector2.Lerp(minSize, maxSize, transition);
            }

            transition -= popPoint;
            transition /= popRemnant;

            return Vector2.Lerp(maxSize, normalSize, transition);
        }

        public static string GenerateName(string category)
        {
            ResourceManager mgr = Resource.ResourceManager;
            List<string> keys = new List<string>();
            

            ResourceSet set = mgr.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            foreach (DictionaryEntry o in set)
            {
                if (o.Key.ToString().StartsWith(category))
                {
                    keys.Add(o.Value.ToString());
                }
            }
            mgr.ReleaseAllResources();

            //return (getGenderName(r, nameGender, keys) + " " + getSurname(r, keys));

            if (keys.Count <= 0)
            {
                return "Error";
            }

            return keys[FrameworkCore.r.Next(keys.Count-1)];
        }

        public static void DrawDottedLine(GameTime gameTime, Vector3 startPos, Vector3 endPos, Color lineColor, Color lineColor2)
        {
            DrawDottedLine(gameTime, startPos, endPos, lineColor, lineColor2, 2, 0.1f, 0);
        }

        public static void DrawDottedLine(GameTime gameTime, Vector3 startPos, Vector3 endPos, Color lineColor, Color lineColor2, float segmentLength, float gapLength)
        {
            DrawDottedLine(gameTime, startPos, endPos, lineColor, lineColor2, segmentLength, gapLength, 0);
        }

        public static void DrawDottedLine(GameTime gameTime, Vector3 startPos, Vector3 endPos, Color lineColor, Color lineColor2, float flowSpeed)
        {
            DrawDottedLine(gameTime, startPos, endPos, lineColor, lineColor2, 2, 0.1f, flowSpeed);
        }

        public static void DrawDottedLine(GameTime gameTime, Vector3 startPos, Vector3 endPos, Color lineColor, Color lineColor2, float segmentLength, float gapLength, float flowSpeed)
        {
            Vector3 lineDir = endPos - startPos;
            lineDir.Normalize();

            float lineLength = Vector3.Distance(startPos, endPos); //total distance

            int segmentCount = (int)(lineLength / (segmentLength + gapLength));


            Vector3 adjustedStartPos = startPos;

            if (flowSpeed != 0)
            {
                float dt = (float)(gameTime.TotalGameTime.TotalSeconds * flowSpeed) % 1.0f;
                dt = MathHelper.Clamp(dt, 0, 1);

                adjustedStartPos = Vector3.Lerp(startPos,
                    startPos + (lineDir * segmentLength * 2) + (lineDir * gapLength * 2),
                    dt);

                if (dt > 0)
                {
                    float dist = Vector3.Distance(startPos, adjustedStartPos);
                    if (dist > gapLength)
                    {
                        float firstSegmentLength = dist - gapLength;

                        FrameworkCore.lineRenderer.Draw(
                            startPos,
                            startPos + lineDir  * firstSegmentLength,
                            lineColor);
                    }
                }
            }

            float totalLength = Vector3.Distance(adjustedStartPos, endPos);

            for (int i = 0; i < segmentCount; i++)
            {
                Vector3 vecPos = adjustedStartPos + lineDir * (i * (segmentLength + gapLength));

                Color segmentColor = lineColor;

                if (i % 2 == 0)
                    segmentColor = lineColor2;

                float adjustedSegmentLength = totalLength;

                Vector3 adjustedSegmentEndPos = vecPos + lineDir * segmentLength;

                if (i >= segmentCount - 2)
                {
                    if (Vector3.Distance(adjustedStartPos, adjustedSegmentEndPos) > totalLength)
                        adjustedSegmentEndPos = endPos;
                }

                FrameworkCore.lineRenderer.Draw(
                    vecPos,
                    adjustedSegmentEndPos,
                    segmentColor);

            }
        }


        //draw a little box on the bottom-right corner of screen.
        public static void DrawDescription(GameTime gameTime, MenuItem item, float Transition, Color teamColor)
        {
            if (item.gameEffect == null)
                return;

            if (item.gameEffect.description == null)
                return;

            string description = item.gameEffect.description;

            Vector2 screenViewport = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);
            Vector2 textVec = FrameworkCore.Serif.MeasureString(description);
            Vector2 textPos = new Vector2(screenViewport.X - textVec.X - 140, screenViewport.Y - textVec.Y - 140);

            textPos.X = MathHelper.Lerp(screenViewport.X + 20, textPos.X, item.selectTransition);
            textPos.X = MathHelper.Lerp(screenViewport.X + 20, textPos.X, Transition);


            Rectangle boxRect = new Rectangle((int)textPos.X, (int)textPos.Y,
                (int)textVec.X, (int)textVec.Y);
            boxRect.Inflate(20, 0);
            boxRect.Y += 2;

            Color boxColor = Color.Black;

            Vector2 lineVec = FrameworkCore.Serif.MeasureString("SAMPLE");
            Rectangle titleRect = boxRect;
            titleRect.Height = (int)lineVec.Y;

            boxRect.Inflate(2, 2);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, boxRect, sprite.blank, boxColor);


            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, titleRect, sprite.blank, teamColor);

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, description, textPos, Color.White);

        }

        public static void DrawDiamond(Vector3 position, float size, Color lineColor)
        {
            //draw a diamond aligned to xz plane.

            FrameworkCore.lineRenderer.Draw(
                position + new Vector3(-size, 0, 0),
                position + new Vector3(0, 0, size),
                lineColor);

            FrameworkCore.lineRenderer.Draw(
                position + new Vector3(0, 0, size),
                position + new Vector3(size, 0, 0),
                lineColor);

            FrameworkCore.lineRenderer.Draw(
                position + new Vector3(size, 0, 0),
                position + new Vector3(0, 0, -size),
                lineColor);

            FrameworkCore.lineRenderer.Draw(
                position + new Vector3(0, 0, -size),
                position + new Vector3(-size, 0, 0),
                lineColor);
        }

        public static float randFloat(float min, float max)
        {
            return MathHelper.Lerp(min, max, (float)FrameworkCore.r.NextDouble());
        }


        public static bool IsSpaceship(Entity item)
        {
            if (item.GetType() == typeof(SpaceShip))
                return true;
            else
                return false;
        }

        public static bool IsHulk(Collideable item)
        {
            if (item.GetType() == typeof(Hulk))
                return true;
            else
                return false;
        }


        public static void DrawOutline(string text, Vector2 pos)
        {
            SpriteFont font = FrameworkCore.Serif;
            Color color1 = Color.White;
            Color color2 = Color.Black;

            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(1, 1), color2);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(-1, -1), color2);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(1, -1), color2);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(-1, 1), color2);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos, color1);
        }

        public static void DrawOutline(SpriteFont font, string text, Vector2 pos, Color color1, Color color2)
        {
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(1, 1), color2);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(-1, -1), color2);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(1, -1), color2);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(-1, 1), color2);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos, color1);
        }

        public static void DrawOutline(SpriteFont font, string text, Vector2 pos, Color color1, Color color2,
            float rotation, Vector2 origin, float size)
        {
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(1, 1), color2, rotation, origin, size, SpriteEffects.None, 0);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(-1, -1), color2, rotation, origin, size, SpriteEffects.None, 0);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(1, -1), color2, rotation, origin, size, SpriteEffects.None, 0);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos + new Vector2(-1, 1), color2, rotation, origin, size, SpriteEffects.None, 0);
            FrameworkCore.SpriteBatch.DrawString(font, text, pos, color1, rotation, origin, size, SpriteEffects.None, 0);
        }




        /// <summary>
        /// Checks whether a ray intersects a model. This method needs to access
        /// the model vertex data, so the model must have been built using the
        /// custom TrianglePickingProcessor provided as part of this sample.
        /// Returns the distance along the ray to the point of intersection, or null
        /// if there is no intersection.
        /// </summary>
        public static float? RayIntersectsModel(Ray ray, Model model, Matrix modelTransform,
                                         out bool insideBoundingSphere,
                                         out Vector3 vertex1, out Vector3 vertex2,
                                         out Vector3 vertex3)
        {
            vertex1 = vertex2 = vertex3 = Vector3.Zero;

            // The input ray is in world space, but our model data is stored in object
            // space. We would normally have to transform all the model data by the
            // modelTransform matrix, moving it into world space before we test it
            // against the ray. That transform can be slow if there are a lot of
            // triangles in the model, however, so instead we do the opposite.
            // Transforming our ray by the inverse modelTransform moves it into object
            // space, where we can test it directly against our model data. Since there
            // is only one ray but typically many triangles, doing things this way
            // around can be much faster.

            Matrix inverseTransform = Matrix.Invert(modelTransform);

            ray.Position = Vector3.Transform(ray.Position, inverseTransform);
            ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);

            // Look up our custom collision data from the Tag property of the model.
            Dictionary<string, object> tagData = (Dictionary<string, object>)model.Tag;

            if (tagData == null)
            {
                throw new InvalidOperationException(
                    "Model.Tag is not set correctly. Make sure your model " +
                    "was built using the custom TrianglePickingProcessor.");
            }

            // Start off with a fast bounding sphere test.
            BoundingSphere boundingSphere = (BoundingSphere)tagData["BoundingSphere"];

            if (boundingSphere.Intersects(ray) == null)
            {
                // If the ray does not intersect the bounding sphere, we cannot
                // possibly have picked this model, so there is no need to even
                // bother looking at the individual triangle data.
                insideBoundingSphere = false;

                return null;
            }
            else
            {
                // The bounding sphere test passed, so we need to do a full
                // triangle picking test.
                insideBoundingSphere = true;

                // Keep track of the closest triangle we found so far,
                // so we can always return the closest one.
                float? closestIntersection = null;

                // Loop over the vertex data, 3 at a time (3 vertices = 1 triangle).
                Vector3[] vertices = (Vector3[])tagData["Vertices"];

                for (int i = 0; i < vertices.Length; i += 3)
                {
                    // Perform a ray to triangle intersection test.
                    float? intersection;

                    RayIntersectsTriangle(ref ray,
                                          ref vertices[i],
                                          ref vertices[i + 1],
                                          ref vertices[i + 2],
                                          out intersection);

                    // Does the ray intersect this triangle?
                    if (intersection != null)
                    {
                        // If so, is it closer than any other previous triangle?
                        if ((closestIntersection == null) ||
                            (intersection < closestIntersection))
                        {
                            // Store the distance to this triangle.
                            closestIntersection = intersection;

                            // Transform the three vertex positions into world space,
                            // and store them into the output vertex parameters.
                            Vector3.Transform(ref vertices[i],
                                              ref modelTransform, out vertex1);

                            Vector3.Transform(ref vertices[i + 1],
                                              ref modelTransform, out vertex2);

                            Vector3.Transform(ref vertices[i + 2],
                                              ref modelTransform, out vertex3);
                        }
                    }
                }

                return closestIntersection;
            }
        }

        /// <summary>
        /// Close the parent menu.
        /// </summary>
        /// <param name="sender">The child item of the parent menu.</param>
        public static void CloseThisMenu(object sender)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            if (((MenuItem)sender).owner != null)
                ((MenuItem)sender).owner.Deactivate();
        }

        /// <summary>
        /// Generate a ray based on cursor position.
        /// </summary>
        /// <param name="projectionMatrix"></param>
        /// <param name="viewMatrix"></param>
        /// <returns></returns>
        public static Ray CalculateCursorRay(Vector2 cursorPosition, Matrix projectionMatrix, Matrix viewMatrix)
        {
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector3 nearSource = new Vector3(cursorPosition, 0f);
            Vector3 farSource = new Vector3(cursorPosition, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = FrameworkCore.Graphics.GraphicsDevice.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = FrameworkCore.Graphics.GraphicsDevice.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }

        /// <summary>
        /// Checks whether a ray intersects a triangle. This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions of the
        /// XNA math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal pass-by-value
        /// versions. This method can be called very frequently in a tight inner loop,
        /// however, so in this particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </summary>
        public static void RayIntersectsTriangle(ref Ray ray,
                                          ref Vector3 vertex1,
                                          ref Vector3 vertex2,
                                          ref Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }



        public static void DrawMouseCursor(SpriteBatch batch, Vector2 pos)
        {
#if WINDOWS
            if (FrameworkCore.options.hardwaremouse)
                return;
#endif

            batch.Draw(FrameworkCore.hudSheet, pos, sprite.mouseCursor, Color.White, 0,
                new Vector2(10, 5), 1, SpriteEffects.None, 0);
        }


        public static string GetShortcutAltKey()
        {
            return "(F)";
        }

        public static string GetShortcutCancel()
        {
            return "(C)";
        }

        public static string GetShortcutAltCancel()
        {
            return "(SPACE)";
        }


        public static Vector2 stringCenter(SpriteBatch spriteBatch, SpriteFont gameFont, string text, Vector2 pos, Color color, float scale)
        {
            return stringCenter(spriteBatch, gameFont, text, pos, color, scale, 0);
        }

        public static Vector2 stringCenterOutline(SpriteBatch spriteBatch, SpriteFont gameFont, string text, Vector2 pos, Color color, Color backColor, float scale, float angle)
        {
            stringCenter(spriteBatch, gameFont, text, pos + new Vector2(-1, -1), backColor, scale, angle);
            stringCenter(spriteBatch, gameFont, text, pos + new Vector2( 1, -1), backColor, scale, angle);
            stringCenter(spriteBatch, gameFont, text, pos + new Vector2(-1,  1), backColor, scale, angle);
            stringCenter(spriteBatch, gameFont, text, pos + new Vector2( 1,  1), backColor, scale, angle);


            return stringCenter(spriteBatch, gameFont, text, pos, color, scale, angle);
        }

        public static Vector2 stringCenter(SpriteBatch spriteBatch, SpriteFont gameFont, string text, Vector2 pos, Color color, float scale, float angle)
        {
            Vector2 stringVec = gameFont.MeasureString(text);
            //stringVec.X *= scale;
            //stringVec.Y *= scale;


            FrameworkCore.SpriteBatch.DrawString(gameFont, text, pos, color, angle,
                new Vector2(stringVec.X / 2, stringVec.Y/2), scale, SpriteEffects.None, 0);

            /*
            Vector2 adjustedPos = pos;
            adjustedPos.X -= stringVec.X / 2;

            FrameworkCore.SpriteBatch.DrawString(gameFont, text, adjustedPos, color, angle,
                Vector2.Zero,scale,SpriteEffects.None,0);
            */
            return stringVec;
        }

        public static string StringWrap(SpriteFont gameFont, string _text, int _width)
        {
            string[] lines = _text.Split('\n');

            Vector2 ln = Vector2.Zero;

            Vector2 lineSize = gameFont.MeasureString("Sample");

            string message = "";

            for (int x = 0; x < lines.Length; x++)
            {
                Vector2 vW = Vector2.Zero;

                string[] splitArray = lines[x].Split(' ');

                int k = 0;
                for (int i = 0; i < splitArray.Length; i++)
                {
                    vW += gameFont.MeasureString(splitArray[i] + " ");
                    if (vW.X < _width)
                    {
                        message += splitArray[i] + " ";
                    }
                    else
                    {
                        message = message.TrimEnd(' ') + "\n";
                        vW = Vector2.Zero;
                        vW += gameFont.MeasureString(splitArray[i] + " ");
                        message += splitArray[i] + " ";
                        k++;
                    }
                }

                message += "\n";
            }

            message = message.TrimEnd(null);

            return message;
        }

        public static string StringWrap(SpriteBatch spriteBatch, SpriteFont gameFont, string _text,
            int _width, Vector2 _pos, Color _color)
        {
            string[] lines = _text.Split('\n');

            Vector2 ln = _pos;

            Vector2 lineSize = gameFont.MeasureString("Sample");

            string message = "";

            for (int x = 0; x < lines.Length; x++)
            {
                Vector2 vW = Vector2.Zero;

                string[] splitArray = lines[x].Split(' ');

                int k = 0;
                for (int i = 0; i < splitArray.Length; i++)
                {
                    vW += gameFont.MeasureString(splitArray[i] + " ");
                    if (vW.X < _width)
                    {
                        message += splitArray[i] + " ";
                    }
                    else
                    {
                        message = message.TrimEnd(' ') + "\n";
                        vW = Vector2.Zero;
                        vW += gameFont.MeasureString(splitArray[i] + " ");
                        message += splitArray[i] + " ";
                        k++;
                    }
                }

                message += "\n";
            }

            message = message.TrimEnd(null);

            return message;

            //spriteBatch.DrawString(gameFont, message, ln, _color);
        }

        public static Vector2 GetScreenCenter()
        {
            Vector2 screen = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            screen.X /= 2f;
            screen.Y /= 2f;

            screen.X = (int)screen.X;
            screen.Y = (int)screen.Y;

            return screen;
        }

        public static void DrawPlayer2Join(float Transition)
        {
            if (FrameworkCore.players.Count > 1)
                return;

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);

#if XBOX
            Vector2 toJoinVec = FrameworkCore.Serif.MeasureString(Resource.MenuSkirmishPlayer2JoinToJoin);
            Vector2 ToJoinPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 110, 0);

            ToJoinPos.Y = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height -
                70 - 4 - toJoinVec.Y - sprite.tinyLogo.Height * 1.3f;

            ToJoinPos.X -= toJoinVec.X;

            ToJoinPos.X += Helpers.PopLerp(Transition, 300, -50, 0);

            Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuSkirmishPlayer2JoinToJoin,
                ToJoinPos,
                titleColor,
                new Color(0, 0, 0, 64), 0, Vector2.Zero, 1);

            Vector2 buttonPos = ToJoinPos;
            buttonPos.X -= sprite.buttons.a.Width / 2;
            buttonPos.X -= 8;
            buttonPos.Y += toJoinVec.Y / 2;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, sprite.buttons.a,
                Color.White, 0, Helpers.SpriteCenter(sprite.buttons.a), 1, SpriteEffects.None, 0);

            Vector2 pressAVec = FrameworkCore.Serif.MeasureString(Resource.MenuSkirmishPlayer2JoinPress);
            Vector2 pressAPos = buttonPos + new Vector2(-pressAVec.X, 0);
            pressAPos.Y -= pressAVec.Y / 2;
            pressAPos.X -= sprite.buttons.a.Width / 2;
            pressAPos.X -= 8;
            Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuSkirmishPlayer2JoinPress,
                pressAPos,
                titleColor,
                new Color(0, 0, 0, 64), 0, Vector2.Zero, 1);
#else
            Vector2 ToJoinPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - 110, 0);
            Vector2 toJoinVec = FrameworkCore.Serif.MeasureString(Resource.MenuSkirmishPlayer2JoinPressPC);
            ToJoinPos.Y = FrameworkCore.Graphics.GraphicsDevice.Viewport.Height -
                70 - 4 - toJoinVec.Y - sprite.tinyLogo.Height * 1.3f;

            ToJoinPos.X -= toJoinVec.X;

            Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuSkirmishPlayer2JoinPressPC,
                ToJoinPos,
                titleColor,
                new Color(0, 0, 0, 64), 0, Vector2.Zero, 1);
#endif


        }


        //use this distance check if actual distance between objects is NOT important.
        //returns a "relative" distance between objects.
        public static float FastDistanceCheck(Vector3 start, Vector3 end)
        {
            float x1, x2, y1, y2, z1, z2;

            x1 = start.X;
            y1 = start.Y;
            z1 = start.Z;


            x2 = end.X;
            y2 = end.Y;
            z2 = end.Z;


            return ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
        }

        /// <summary>
        /// Find the intersection point from the bolt to the ship mesh.
        /// </summary>
        /// <param name="origin">Start position of the raycast.</param>
        /// <param name="moveDir">Direction of the raycast.</param>
        /// <returns></returns>
        public static bool MeshHit(Vector3 origin, Vector3 moveDir, SpaceShip ship, out Vector3 hitPos)
        {
            moveDir.Normalize();
            Ray cursorRay = new Ray(origin, moveDir);
            bool insideBoundingSphere;

            Vector3 vertex1, vertex2, vertex3;

            Matrix shipMatrix = Matrix.Identity;
            shipMatrix = Matrix.CreateFromQuaternion(ship.Rotation);
            shipMatrix.Translation = ship.Position;

            // Perform the ray to model intersection test.
            float? intersection = Helpers.RayIntersectsModel(cursorRay, FrameworkCore.ModelArray[(int)ship.modelMesh],
                                                     shipMatrix,
                                                     out insideBoundingSphere,
                                                     out vertex1, out vertex2,
                                                     out vertex3);


            // Do we have a per-triangle intersection with this model?
            if (intersection != null)
            {
                moveDir.Normalize();
                hitPos = origin + moveDir * (float)intersection;

                return true;
            }

            hitPos = origin;
            return false;
        }

        /// <summary>
        /// alpha = 0-255
        /// </summary>
        /// <param name="alpha"></param>
        public static void DarkenScreen(int alpha)
        {
            alpha = (int)MathHelper.Clamp(alpha, 0, 255);

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                new Rectangle(0, 0, (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width, (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height),
                sprite.blank, new Color(0, 0, 0, (byte)alpha));
            
        }


        public static float Pulse(GameTime gameTime, float amount, float pulseSpeed)
        {
            return (float)(amount * Math.Sin(gameTime.TotalGameTime.TotalSeconds * pulseSpeed));
        }

        public static Vector2 stringCenter(SpriteFont font, string text)
        {
            Vector2 titleVec = font.MeasureString(text);
            

            titleVec.X /= 2.0f;
            titleVec.Y /= 2.0f;


            return titleVec;
        }



        public static ShipData getShipByNumber(int number)
        {
            if (number == 0)
                return shipTypes.Destroyer;
            else if (number == 1)
                return shipTypes.BeamFrigate;
            else if (number == 2)
                return shipTypes.Battleship;
            else if (number == 3)
                return shipTypes.Gunship;
            else if (number == 4)
                return shipTypes.Dreadnought;
            else if (number == 5)
                return shipTypes.DebugShip;
            else if (number == 6)
                return shipTypes.BeamGunship;
            else if (number == 7)
                return shipTypes.Fighter;

            return null;
        }


        public static int getShipByType(ShipData ship)
        {
            if (ship == shipTypes.Destroyer)
                return 0;
            if (ship == shipTypes.BeamFrigate)
                return 1;
            if (ship == shipTypes.Battleship)
                return 2;
            if (ship == shipTypes.Gunship)
                return 3;
            if (ship == shipTypes.Dreadnought)
                return 4;
            if (ship == shipTypes.DebugShip)
                return 5;
            if (ship == shipTypes.BeamGunship)
                return 6;
            if (ship == shipTypes.Fighter)
                return 7;

            return -1;
        }

        public static void DrawClickMessage(GameTime gameTime, float Transition)
        {
            DrawClickMessage(gameTime, Transition, Resource.MenuClickWhenDone, FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2);
        }

        public static void DrawClickMessage(GameTime gameTime, float Transition, string text)
        {
            DrawClickMessage(gameTime, Transition, text, FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2);
        }

        public static void DrawClickMessage(GameTime gameTime, float Transition, string text, float xOffset)
        {
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);
            Color doneColor = Color.Lerp(Helpers.transColor(Color.White), Color.White, Transition);
            float doneSize = MathHelper.Lerp(1.17f, 1.2f,
                0.5f + Helpers.Pulse(gameTime, 0.49f, 6));
            Vector2 donePosition = new Vector2(xOffset,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 100);

            

            donePosition.Y += MathHelper.Lerp(10, 0,
                0.5f + Helpers.Pulse(gameTime, 0.49f, 2));
            donePosition.Y += Helpers.PopLerp(Transition, 200, -40, 0);

            Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif,
                text, donePosition, doneColor, bgColor, doneSize, 0);
        }

        public static void DrawVeterancy(Vector2 position, int starQuantity, Color starColor)
        {
            if (starQuantity <= 0)
                return;

            position.X -= (float)(starQuantity/2.0f) * sprite.star.Width;
            position.X += sprite.star.Width/2.0f;

            for (int i = 0; i < starQuantity; i++)
            {
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, position, sprite.star,
                    starColor, 0, Helpers.SpriteCenter(sprite.star), 1, SpriteEffects.None, 0);

                position.X += sprite.star.Width;
            }
        }

        /// <summary>
        /// First, see if the raycast hits the ship. If it doesn't, then raycast toward the ship's origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="moveDir"></param>
        /// <param name="ship"></param>
        /// <returns></returns>
        public static Vector3 ShipMeshHit(Vector3 origin, Vector3 moveDir, SpaceShip ship)
        {
            Vector3 hitPos = Vector3.Zero;
            if (MeshHit(origin, moveDir, ship, out hitPos))
            {
                return hitPos;
            }
            else
            {
                Vector3 adjustedDir = ship.Position - origin;
                adjustedDir.Normalize();

                if (MeshHit(origin, adjustedDir, ship, out hitPos))
                {
                    return hitPos;
                }
            }

            return origin;
        }

        public static bool GuideVisible
        {
            get
            {
#if LiveEnabled
            return Guide.IsVisible;
#else
            return false;
#endif
                 
            }

        }



        public static Vector2 IntersectionPoint(Line firstLine, Line secondLine)
        {
            // Equations to determine whether lines intersect
            double Ua = ((secondLine.EndPos.X - secondLine.StartPos.X) *
                (firstLine.StartPos.Y - secondLine.StartPos.Y) -
                (secondLine.EndPos.Y - secondLine.StartPos.Y) *
                (firstLine.StartPos.X - secondLine.StartPos.X)) /
                ((secondLine.EndPos.Y - secondLine.StartPos.Y) *
                (firstLine.EndPos.X - firstLine.StartPos.X) -
                (secondLine.EndPos.X - secondLine.StartPos.X) *
                (firstLine.EndPos.Y - firstLine.StartPos.Y));

            double Ub = ((firstLine.EndPos.X - firstLine.StartPos.X) *
                (firstLine.StartPos.Y - secondLine.StartPos.Y) -
                (firstLine.EndPos.Y - firstLine.StartPos.Y) *
                (firstLine.StartPos.X - secondLine.StartPos.X)) /
                ((secondLine.EndPos.Y - secondLine.StartPos.Y) *
                (firstLine.EndPos.X - firstLine.StartPos.X) -
                (secondLine.EndPos.X - secondLine.StartPos.X) *
                (firstLine.EndPos.Y - firstLine.StartPos.Y));

            if (Ua >= 0.0f && Ua <= 1.0f && Ub >= 0.0f && Ub <= 1.0f)
            {
                double x = firstLine.StartPos.X + Ua * (firstLine.EndPos.X - firstLine.StartPos.X);
                double y = firstLine.StartPos.Y + Ua * (firstLine.EndPos.Y - firstLine.StartPos.Y);

                return new Vector2((float)x, (float)y);
            }

            else
            {
                return Vector2.Zero;
            }
        }

        public static float GetRingAngle(PlayerIndex index)
        {
            if (index == PlayerIndex.One)
                return 0;
            else if (index == PlayerIndex.Two)
                return ((float)Math.PI * 0.5f);
            else if (index == PlayerIndex.Three)
                return ((float)Math.PI * 1.5f);
            else
                return ((float)Math.PI * 1.0f);
        }

        public static string GenerateShipName()
        {
            return Helpers.GenerateName("Number") + " " + Helpers.GenerateName("Adjective") + " " + Helpers.GenerateName("Animal");
        }

        public static string GetPlayerName()
        {
            return GetPlayerName(FrameworkCore.ControllingPlayer);
        }

        public static string GetPlayerName(PlayerIndex index)
        {
#if XBOX
            SignedInGamer gamer = SignedInGamer.SignedInGamers[index];

            if (gamer != null)
                return gamer.Gamertag;
            else
#endif
            {
                for (int i = 0; i < FrameworkCore.players.Count; i++)
                {
                    if (FrameworkCore.players[i].playerindex == index)
                        return FrameworkCore.players[i].commanderName;
                }
            }

            return Helpers.GenerateName("Gamertag");
        }

        public static string StripOutAmpersands(string baseName)
        {
#if XBOX
            return baseName;
#endif

            try
            {
                string pattern = "[&]"; //regex pattern 
                return System.Text.RegularExpressions.Regex.Replace(baseName, pattern, "");
            }
            catch
            {
                return baseName;
            }
        }

        public static bool IsValidPlayerName(string name)
        {
            name = name.Trim();

            if (name.Length <= 0)
                return false;

            if (0 <= name.IndexOf("&&", StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (0 <= name.IndexOf("&name", StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (0 <= name.IndexOf("&score", StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (0 <= name.IndexOf("&name=", StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (0 <= name.IndexOf("&score=", StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (0 <= name.IndexOf("&id=", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        public enum HighScoreType
        {
            Normal,
            Hardcore
        }

        /// <summary>
        /// Returns TRUE if a new/better score was added.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private static bool GlobalHighScore(string name, int score, HighScoreType type)
        {
#if WINDOWS
            try
            {
                string addscore = string.Empty;

                if (type == HighScoreType.Normal)
                    addscore = "addScore";
                else
                    addscore = "addHardcoreScore";

                //don't record scores <= 0
                if (score <= 0)
                    return false;

                if (FrameworkCore.isTrialMode())
                    return false;

                //no funny business.
                if (!Helpers.IsValidPlayerName(name))
                    return false;

                //assemble the php arguments.
                string url =
                    "http://www.blendogames.com/flotilla/scores/" + addscore + ".php?name=" + name +
                        "&score=" + score + "&id=passwordhere";

                //send it.
                WebClient client = new WebClient();
                string success = client.DownloadString(url);

                //check the return string. 1 = new score was added. 0 = no new score.
                if (success != null)
                {
                    if (0 <= success.IndexOf("1", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }

                return false;
            }
            catch
            {
                


                return false;
            }
#else
            return false;
#endif
        }

        

        static string[] globalHighScoreNames;
        static int[] globalHighScoreScores;

        public static string[] GlobalHighscoreNames
        {
            get { return globalHighScoreNames; }
        }

        public static int[] GlobalHighscoreScores
        {
            get { return globalHighScoreScores; }
        }

        public static bool GetGlobalHighScores(HighScoreType type)
        {
#if WINDOWS
            if (FrameworkCore.isTrialMode())
                return false;

            try
            {
                string getString = string.Empty;
                if (type == HighScoreType.Normal)
                    getString = "getScores";
                else
                    getString = "getHardcoreScores";

                WebClient client = new WebClient();

                string rawScores = client.DownloadString("http://www.blendogames.com/flotilla/scores/" + getString + ".php");

                if (rawScores == null)
                    return false;

                int scoreQuantity = -1;
                int idx = 0;
                int fIdx = 0;
                string strScoreQuantity = Helpers.GetSubString(rawScores, "totalscores=", "&", out idx, out fIdx);
                if (strScoreQuantity != null)
                {
                    scoreQuantity = Convert.ToInt32(strScoreQuantity, CultureInfo.InvariantCulture);
                }

                //strip out the score data from the string.
                rawScores = rawScores.Remove(0, idx);

                if (scoreQuantity <= 0)
                    return false;

                globalHighScoreNames = new string[scoreQuantity];
                globalHighScoreScores = new int[scoreQuantity];

                for (int i = 0; i < scoreQuantity; i++)
                {
                    int lastIndex = 0;
                    int firstIndex = 0;
                    string strCurScore = Helpers.GetSubString(rawScores, "score" + (i + 1) + "=", "&", out lastIndex, out firstIndex);
                    if (strCurScore != null)
                    {
                        globalHighScoreScores[i] = Convert.ToInt32(strCurScore, CultureInfo.InvariantCulture);

                        //strip out the score.
                        rawScores = rawScores.Remove(0, lastIndex);
                    }

                    lastIndex = 0;
                    firstIndex = 0;
                    string strCurName = Helpers.GetSubString(rawScores, "name" + (i + 1) + "=", "&", out lastIndex, out firstIndex);
                    if (strCurName != null)
                    {
                        globalHighScoreNames[i] = strCurName;

                        //strip out this name.
                        rawScores = rawScores.Remove(0, lastIndex);
                    }
                }

              


                return true;
            }
            catch
            {
                return false;
            }
#else
            return false;
#endif
        }

        public static int AddHighScore(string curName, int curScore, out bool newGlobalHigh)
        {
            newGlobalHigh = false;
#if WINDOWS
            //send score.

            HighScoreType type = HighScoreType.Normal;

            if (FrameworkCore.isHardcoreMode)
                type = HighScoreType.Hardcore;
            else
                type = HighScoreType.Normal;

            newGlobalHigh = GlobalHighScore(curName, curScore, type);

            //refresh the scoreboard.
            GetGlobalHighScores(type);
#endif

            int scoreIndex = -1;

            for (int i = 0; i < FrameworkCore.highScores.count; i++)
            {
                if (curScore > FrameworkCore.highScores.scores[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = FrameworkCore.highScores.count - 1; i > scoreIndex; i--)
                {
                    FrameworkCore.highScores.commanderName[i] = FrameworkCore.highScores.commanderName[i - 1];
                    FrameworkCore.highScores.scores[i] = FrameworkCore.highScores.scores[i - 1];
                }

                FrameworkCore.highScores.commanderName[scoreIndex] = curName;
                FrameworkCore.highScores.scores[scoreIndex] = curScore;

                FrameworkCore.storagemanager.SaveHighScores(FrameworkCore.highScores);
            }

            return scoreIndex;
        }





        public static string GetSubString(string stream, string start, string end, out int index, out int firstIndex)
        {
            index = 0;
            firstIndex = -1;

            try
            {
                int first = stream.IndexOf(start, StringComparison.CurrentCultureIgnoreCase);
                int last = stream.IndexOf(end, first + start.Length, StringComparison.CurrentCultureIgnoreCase);

                //didn't find any matches.
                if (first < 0)
                    return null;

                //???
                if (last < 0)
                    return null;

                string tempString = stream.Substring(first, last - first);

                index = last; //index of the second word.
                firstIndex = first;

                tempString = tempString.Remove(0, start.Length); //remove the start word.
                tempString = tempString.Trim(); //remove trailing spaces.
                return tempString;
            }
            catch
            {
                //something went caca
                return null;
            }
        }





        public static void AddPointBonus()
        {
            AddPointBonus(40000);
        }

        public static void AddPointBonus(int amount)
        {
            FrameworkCore.players[0].extraPoints += amount;
        }

        public static int GenerateFinalScore()
        {
            int finalValue = 0;

            finalValue += FrameworkCore.players[0].campaignShips.Count * 9000; //SHIPS IN FLOTILLA.
            finalValue += FrameworkCore.players[0].inventoryItems.Count * 500; //INVENTORY ITEMS.
            finalValue += FrameworkCore.players[0].planetsVisited * 2000; //PLANETS VISITED.
            finalValue += FrameworkCore.players[0].dangerousPlanetsVisited * 10000; //DANGEROUS PLANETS BONUS.

            finalValue += FrameworkCore.players[0].extraPoints;

            return finalValue;
        }


        public static void UpdateCameraProjections(int panes)
        {
            float nFov = 45;// MathHelper.Lerp(70, 80, viewPortTransition);
            
            float width = (float)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / panes;
            foreach (PlayerCommander player in FrameworkCore.players)
            {
                player.lockCamera.SetProjectionParams(MathHelper.ToRadians(nFov),
                    width / (float)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height, 1.0f, 3000000.0f);
            }
        }

        public static void EventRumble()
        {
            FrameworkCore.PlayCue(sounds.Explosion.big);

            SetRumbleExplosion();
        }


        public static void SetRumbleExplosion()
        {
            if (FrameworkCore.level.isDemo)
                return;

            foreach (PlayerCommander player in FrameworkCore.players)
            {
                if (FrameworkCore.players.IndexOf(player) == 0 && !FrameworkCore.options.p1Vibration)
                    continue;

                if (FrameworkCore.players.IndexOf(player) == 1 && !FrameworkCore.options.p2Vibration)
                    continue;

                player.setRumble(300);
            }
        }


        public static void DrawTurrets(MeshRenderer renderer, ShipData ship, Matrix shipMatrix, Color teamColor,
            float Transition)
        {
            if (ship.turretDatas == null)
                return;

            for (int i = 0; i < ship.turretDatas.Length; i++)
            {
                Vector3 shipPos = shipMatrix.Translation;
                shipPos +=
                    shipMatrix.Forward * -ship.turretDatas[i].localOffset.Z +
                    shipMatrix.Right * ship.turretDatas[i].localOffset.X +
                    shipMatrix.Up * ship.turretDatas[i].localOffset.Y;

                Matrix worldMatrix = shipMatrix;
                worldMatrix.Translation = shipPos;

                renderer.Draw(ship.turretDatas[i].modelName,
                    worldMatrix, null,
                    teamColor, MathHelper.Lerp(0, 1, Transition));
            }
        }

        public static FleetShip AddFleetShip(List<FleetShip> fleetList, ShipData data)
        {
            FleetShip ship = new FleetShip();
            ship.captainName = Helpers.GenerateShipName();
            ship.shipData = data;
            fleetList.Add(ship);

            return ship;
        }

        public static int ApplyFireRateModifier(SpaceShip ship, int baseReloadTime)
        {
            if (ship.fleetShipInfo == null)
                return baseReloadTime;

            float returnValue = baseReloadTime;

            for (int i = 0; i < ship.fleetShipInfo.upgradeArray.Length; i++)
            {
                if (ship.fleetShipInfo.upgradeArray[i] == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect.fireRateModifier != 0)
                    returnValue /= ship.fleetShipInfo.upgradeArray[i].gameEffect.fireRateModifier;
            }

            return (int)returnValue;
        }

        public static void DrawSystemMenu(GameTime gameTime)
        {
            FrameworkCore.sysMenuManager.Draw(gameTime);

#if WINDOWS
            if (FrameworkCore.sysMenuManager.menus.Count <= 0 &&
                FrameworkCore.MainMenuManager.menus.Count <= 0)
                return;
            
            Helpers.DrawMouseCursor(FrameworkCore.SpriteBatch,
                FrameworkCore.MenuInputs[0].mousePos);
#endif
        }

        public static bool UpdateTiltedMouseMenu(List<MenuItem> menuItems, Vector2 mousePos, float itemAngle,
            bool isHorizontal,
            Point inflateRect,
            SpriteFont itemFont,            
            bool handleSplitscreen,
            InputManager inputManager,
            MenuItem currentSelectedItem,
            out bool mouseIsHovering, out MenuItem selectedItem)
        {
            selectedItem = currentSelectedItem;
            bool tempMouseIsHovering = false;

            if (handleSplitscreen)
            {
                if (FrameworkCore.players.Count > 1)
                    mousePos.X *= 0.5f;
            }

            foreach (MenuItem item in menuItems)
            {
                int bound = (int)sprite.icons.circle.Width;
                int boxLength = 0;
                
                if (itemFont != null)
                    boxLength = (int)itemFont.MeasureString(item.text).X + bound;

                int width=0, height=0;

                if (isHorizontal)
                {
                    width = boxLength;
                    height = bound;
                }
                else
                {
                    width = bound;
                    height = boxLength;
                }


                Rectangle itemRect = new Rectangle(
                    (int)item.position.X - bound / 2,
                    (int)item.position.Y - bound / 2,
                    width,
                    height);
                itemRect.Inflate(inflateRect.X, inflateRect.Y);

#if DEBUG
                //deleteme.
                item.hitBox = itemRect;
#endif


                

                float distanceToCursor = Vector2.Distance(item.position, new Vector2(mousePos.X, mousePos.Y));
                float XDistance = mousePos.X - item.position.X;
                float YDistance = mousePos.Y - item.position.Y;
                float angleToCursor = (float)Math.Atan2(YDistance, XDistance);

                if (isHorizontal)
                    angleToCursor -= itemAngle;
                else
                    angleToCursor += itemAngle;

                float x = (float)(Math.Cos(angleToCursor) * distanceToCursor);
                float y = (float)(Math.Sin(angleToCursor) * distanceToCursor);
                Vector2 adjustedCursorPos = item.position + new Vector2(x, y);

                

#if DEBUG
                //deleteme
                item.hitCursor = adjustedCursorPos;
#endif

                if (itemRect.Contains(new Point((int)adjustedCursorPos.X, (int)adjustedCursorPos.Y)))
                {
                    if (inputManager.mouseHasMoved)
                        selectedItem = item;

                    tempMouseIsHovering = true;
                    mouseIsHovering = true;
                }
            }

            if (tempMouseIsHovering)
            {
                mouseIsHovering = true;
                return true;
            }
            else
            {
                mouseIsHovering = false;

                if (inputManager.mouseHasMoved)
                    selectedItem = null;

                return false;
            }
        }

        public static Point GetAdjustedEnemyFleetSize()
        {
            int playerFleetSize = FrameworkCore.players[0].campaignShips.Count;
            
            int minBound = (int)MathHelper.Max(playerFleetSize - 1, 2);
            int maxBound = (int)MathHelper.Max(playerFleetSize + 1, 2);

            return new Point(minBound, maxBound);            
        }

        public static void DrawDebugRectangle(Rectangle rectangle, Color color)
        {
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rectangle,
                        sprite.blank, new Color(color.R, color.G, color.B, 128));
        }

        public static float ApplyRepairModifier(GameTime gameTime, SpaceShip ship)
        {
            if (ship.fleetShipInfo == null)
                return ship.Health;

            if (ship.Health >= ship.MaxDamage)
                return ship.Health;

            float returnValue = ship.Health;

            for (int i = 0; i < ship.fleetShipInfo.upgradeArray.Length; i++)
            {
                if (ship.fleetShipInfo.upgradeArray[i] == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect.repairRate != 0)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(ship.fleetShipInfo.upgradeArray[i].gameEffect.repairRate).TotalMilliseconds);

                    returnValue = MathHelper.Clamp(returnValue + delta, 0, ship.MaxDamage);
                }
            }

            return returnValue;
        }

        public static float ApplySpeedModifier(SpaceShip ship, float baseSpeed)
        {
            if (ship.fleetShipInfo == null)
                return baseSpeed;

            float returnValue = baseSpeed;

            for (int i = 0; i < ship.fleetShipInfo.upgradeArray.Length; i++)
            {
                if (ship.fleetShipInfo.upgradeArray[i] == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect.speedModifier != 0)
                {
                    returnValue *= ship.fleetShipInfo.upgradeArray[i].gameEffect.speedModifier;
                }
            }

            return returnValue;
        }

        

        public static float ApplyBulletSpeedModifier(SpaceShip ship, float baseBulletSpeed)
        {
            if (ship.fleetShipInfo == null)
                return baseBulletSpeed;

            float returnValue = baseBulletSpeed;

            for (int i = 0; i < ship.fleetShipInfo.upgradeArray.Length; i++)
            {
                if (ship.fleetShipInfo.upgradeArray[i] == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect.bulletSpeedModifier != 0)
                {
                    returnValue *= ship.fleetShipInfo.upgradeArray[i].gameEffect.bulletSpeedModifier;
                }
            }

            return returnValue;
        }

        public static float ApplyBeamModifier(SpaceShip ship, float baseDamage)
        {
            if (ship.fleetShipInfo == null)
                return baseDamage;

            float returnValue = baseDamage;

            for (int i = 0; i < ship.fleetShipInfo.upgradeArray.Length; i++)
            {
                if (ship.fleetShipInfo.upgradeArray[i] == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect.BeamArmor > 0)
                {
                    float armor = MathHelper.Clamp(ship.fleetShipInfo.upgradeArray[i].gameEffect.BeamArmor, 0, 1);

                    returnValue *= (1.0f - armor);
                }
            }

            return returnValue;
        }

        public static float ApplyArmorModifier(SpaceShip ship, float basePenetration, int locationHit)
        {
            if (ship.fleetShipInfo == null)
                return basePenetration;

            float returnValue = basePenetration;

            for (int i = 0; i < ship.fleetShipInfo.upgradeArray.Length; i++)
            {
                if (ship.fleetShipInfo.upgradeArray[i] == null)
                    continue;

                if (ship.fleetShipInfo.upgradeArray[i].gameEffect == null)
                    continue;

                if (locationHit == (int)locationType.Bottom)
                {
                    if (ship.fleetShipInfo.upgradeArray[i].gameEffect.armorModifierBottom != 0)
                    {
                        returnValue -= ship.fleetShipInfo.upgradeArray[i].gameEffect.armorModifierBottom;
                    }
                }

                if (locationHit == (int)locationType.Rear)
                {
                    if (ship.fleetShipInfo.upgradeArray[i].gameEffect.armorModifierRear != 0)
                    {
                        returnValue -= ship.fleetShipInfo.upgradeArray[i].gameEffect.armorModifierRear;
                    }
                }
            }

            return returnValue;
        }



        public static string ConvertKeyToChar(Keys key, bool shift, bool allowCarriageReturn)
        {
            switch (key)
            {
                case Keys.Space: return " ";

                // Escape Sequences 
                case Keys.Enter:
                    {
                        if (allowCarriageReturn)
                            return "\n"; // Create a new line 
                        else
                            return "";
                    }                         
                //case Keys.Tab: return "\t";                           // Tab to the right 

                // D-Numerics (strip above the alphabet) 
                case Keys.D0: return shift ? ")" : "0";
                case Keys.D1: return shift ? "!" : "1";
                case Keys.D2: return shift ? "@" : "2";
                case Keys.D3: return shift ? "#" : "3";
                case Keys.D4: return shift ? "$" : "4";
                case Keys.D5: return shift ? "%" : "5";
                case Keys.D6: return shift ? "^" : "6";
                case Keys.D7: return shift ? "&" : "7";
                case Keys.D8: return shift ? "*" : "8";
                case Keys.D9: return shift ? "(" : "9";

                // Numpad 
                case Keys.NumPad0: return "0";
                case Keys.NumPad1: return "1";
                case Keys.NumPad2: return "2";
                case Keys.NumPad3: return "3";
                case Keys.NumPad4: return "4";
                case Keys.NumPad5: return "5";
                case Keys.NumPad6: return "6";
                case Keys.NumPad7: return "7";
                case Keys.NumPad8: return "8";
                case Keys.NumPad9: return "9";
                case Keys.Add: return "+";
                case Keys.Subtract: return "-";
                case Keys.Multiply: return "*";
                case Keys.Divide: return "/";
                case Keys.Decimal: return ".";

                // Alphabet 
                case Keys.A: return "A";
                case Keys.B: return "B" ;
                case Keys.C: return "C" ;
                case Keys.D: return "D" ;
                case Keys.E: return  "E" ;
                case Keys.F: return  "F" ;
                case Keys.G: return  "G" ;
                case Keys.H: return  "H" ;
                case Keys.I: return  "I" ;
                case Keys.J: return  "J" ;
                case Keys.K: return  "K" ;
                case Keys.L: return  "L" ;
                case Keys.M: return  "M" ;
                case Keys.N: return  "N" ;
                case Keys.O: return  "O" ;
                case Keys.P: return  "P" ;
                case Keys.Q: return  "Q" ;
                case Keys.R: return  "R" ;
                case Keys.S: return  "S" ;
                case Keys.T: return  "T" ;
                case Keys.U: return  "U" ;
                case Keys.V: return  "V" ;
                case Keys.W: return  "W" ;
                case Keys.X: return  "X" ;
                case Keys.Y: return  "Y" ;
                case Keys.Z: return  "Z" ;

                // Oem 
                case Keys.OemOpenBrackets: return shift ? "{" : "[";
                case Keys.OemCloseBrackets: return shift ? "}" : "]";
                case Keys.OemComma: return shift ? "<" : ",";
                case Keys.OemPeriod: return shift ? ">" : ".";
                case Keys.OemMinus: return shift ? "_" : "-";
                case Keys.OemPlus: return shift ? "+" : "=";
                case Keys.OemQuestion: return shift ? "?" : "/";
                case Keys.OemSemicolon: return shift ? ":" : ";";
                case Keys.OemQuotes: return shift ? "\"" : "'";
                case Keys.OemPipe: return shift ? "|" : "\\";
                case Keys.OemTilde: return shift ? "~" : "`";
            }

            return string.Empty;
        } 
    }

    public class Line
    {
        public Vector2 StartPos;
        public Vector2 EndPos;

        public Line(Vector2 start, Vector2 end)
        {
            this.StartPos = start;
            this.EndPos = end;
        }
    }
}

