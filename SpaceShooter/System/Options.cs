using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//using System.Text;

namespace SpaceShooter
{
    public class Options
    {
        public int soundVolume = 10;
        public int musicVolume = 10;

        public bool p1InvertY = false;
        public bool p1InvertX = false;
        public bool p1Vibration = true;

        public bool p2InvertY = false;
        public bool p2InvertX = false;
        public bool p2Vibration = true;

        public int brightness = 5;

        public bool bloom = true;

        public bool p1UseMouse;
        public bool p2UseMouse;

        public bool fullscreen = false;

        public bool renderPlanets = true;
        public int mousewheel = 0;
        public int sensitivity = 5;
        public bool hardwaremouse = false;
        public bool manualDefault = false;

        public int resolutionX;
        public int resolutionY;
    }
}