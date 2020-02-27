#region File Description
//-----------------------------------------------------------------------------
// AudioManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace SpaceShooter
{
    //list of sounds.
    public static class sounds
    {
        public static class click
        {
            public static string activate = "clickactivate";
            public static string select = "clickselect";
            public static string back = "clickback";
            public static string beep = "clickbeep";
            public static string whoosh = "whoosh";
            public static string error = "clickerror";
        }

        public static class Fanfare
        {
            public static string item = "fanfareitem";
            public static string ship = "fanfareship";
            public static string veterancy = "veterancy";
            public static string radio = "veterancy";
            public static string cheering = "cheering";
            public static string drill = "drill";
            public static string wormhole = "wormhole";
            public static string ding = "ding";
            public static string chaching = "chaching";
            public static string ready = "ready";
            public static string klaxon = "klaxon";
            public static string timer = "timer";
            public static string gunload = "gunload";
            public static string rev = "rev";
        }

        public static class Weapon
        {
            public static string rifle = "rifle";
            public static string laser = "laser";
        }        

        public static class Engine
        {
            public static string engine1 = "engineloop01";
            public static string engine2 = "engineloop02";
            public static string engine3 = "engineloop03";
        }

        public static class Rocket
        {
            public static string rocket1 = "rocketloop01";
            public static string rocket2 = "rocketloop02";
            public static string rocket3 = "rocketloop03";
        }

        public static class Explosion
        {
            public static string big = "explosionbig";
            public static string tiny = "explosiontiny";
            public static string asteroid = "explosionasteroid";
        }

        public static class Impact
        {
            public static string deflect = "deflect";
        }

        public static class Worldmap
        {
            public static string jet = "jet";
        }

        public static class Music
        {
            /// <summary>
            /// Main Menu music.
            /// </summary>
            public static string raindrops01 = "raindrops01";

            /// <summary>
            /// Action Music.
            /// </summary>
            public static string nocturnes = "nocturnes";

            public static string invaders = "loopinvaders";
            public static string spooky = "loopspooky";
            public static string rasta = "looprasta";
            public static string phenomena = "loopphenomena";
            public static string cool = "loopcool";
            public static string piano = "looppiano";
            public static string bird = "loopbird";
            public static string dhol = "loopdhol";
            public static string funky = "loopfunky";
            public static string guitar = "loopguitar";
            public static string moog = "loopmoog";
            public static string drumbeat = "loopdrumbeat";
            public static string jazz = "loopjazz";
            public static string singing = "loopsinging";




            /// <summary>
            /// Used for Intro when campaign starts. Non-looping.
            /// </summary>
            public static string cello = "cello";
            public static string none = "null";
        }
    }


    /// <summary>
    /// Interface used by the AudioManager to look up the position
    /// and velocity of entities that can emit 3D sounds.
    /// </summary>
    public class IAudioEmitter
    {
        public Vector3 Position;
        public Vector3 Forward;
        public Vector3 Up;
        public Vector3 Velocity;
    }

    /// <summary>
    /// Audio manager keeps track of what 3D sounds are playing, updating
    /// their settings as the camera and entities move around the world,
    /// and automatically disposing cue instances after they finish playing.
    /// </summary>
    public class AudioManager
    {
        #region Fields

        static bool pauseAll = false;

        // The listener describes the ear which is hearing 3D sounds.
        // This is usually set to match the camera.
        public AudioListener Listener
        {
            get { return listener; }
        }

        AudioListener listener = new AudioListener();


        // The emitter describes an entity which is making a 3D sound.
        AudioEmitter emitter = new AudioEmitter();


        // Keep track of all the 3D sounds that are currently playing.
        List<Cue3D> activeCues = new List<Cue3D>();


        // Keep track of spare Cue3D instances, so we can reuse them.
        // Otherwise we would have to allocate new instances each time
        // a sound was played, which would create unnecessary garbage.
        Stack<Cue3D> cuePool = new Stack<Cue3D>();


        #endregion




        public AudioManager()            
        { }





        int soundUpdateTimer = 0;

        /// <summary>
        /// Updates the state of the 3D audio system.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            soundUpdateTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (soundUpdateTimer > 0)
                return;

            soundUpdateTimer = 150; //update sound at this interval.
            



            // Loop over all the currently playing 3D sounds.
            int index = 0;

            while (index < activeCues.Count)
            {
                Cue3D cue3D = activeCues[index];

                if (cue3D.Cue.IsStopped)
                {
                    // If the cue has stopped playing, dispose it.
                    cue3D.Cue.Dispose();

                    // Store the Cue3D instance for future reuse.
                    cuePool.Push(cue3D);

                    // Remove it from the active list.
                    activeCues.RemoveAt(index);
                }
                else
                {
                    // If the cue is still playing, update its 3D settings.
                    Apply3D(cue3D);

                    index++;
                }
            }

            // Update the XACT engine.
            //audioEngine.Update();

        }

        public void ClearAll()
        {
            //activeCues.Clear();
            //cuePool.Clear();
            StopAll();
        }


        /// <summary>
        /// Triggers a new 3D sound.
        /// </summary>
        public Cue Play3DCue(string cueName, IAudioEmitter emitter)
        {
            if (FrameworkCore.gameState != GameState.Play || FrameworkCore.soundbank == null)
                return null;


            Cue3D cue3D;

            if (cuePool.Count > 0)
            {
                // If possible, reuse an existing Cue3D instance.
                cue3D = cuePool.Pop();
            }
            else
            {
                // Otherwise we have to allocate a new one.
                cue3D = new Cue3D();
            }

            // Fill in the cue and emitter fields.
            try
            {
                cue3D.Cue = FrameworkCore.soundbank.GetCue(cueName);
            }
            catch
            {
            }

            cue3D.Emitter = emitter;



            // Set the 3D position of this cue, and then play it.
            Apply3D(cue3D);

            try
            {
                cue3D.Cue.Play();
            }
            catch
            {
            }

            // Remember that this cue is now active.
            activeCues.Add(cue3D);

            return cue3D.Cue;
        }


        /// <summary>
        /// Updates the position and velocity settings of a 3D cue.
        /// </summary>
        private void Apply3D(Cue3D cue3D)
        {
            if (emitter == null || cue3D == null || cue3D.Emitter == null)
                return;

            if (cue3D.Cue.IsDisposed)
                return;

            try
            {
                emitter.Position = cue3D.Emitter.Position;
                emitter.Forward = cue3D.Emitter.Forward;
                emitter.Up = cue3D.Emitter.Up;
                emitter.Velocity = cue3D.Emitter.Velocity;

                cue3D.Cue.Apply3D(FrameworkCore.audio.nearestListener(emitter.Position), emitter);
            }
            catch
            {
            }
        }


        /// <summary>
        /// Internal helper class for keeping track of an active 3D cue,
        /// and remembering which emitter object it is attached to.
        /// </summary>
        private class Cue3D
        {
            public Cue Cue;
            public IAudioEmitter Emitter;
        }

        public void StopAll()
        {
            for (int i = 0; i < activeCues.Count; i++)
            {
                if (activeCues[i].Cue.IsPlaying)
                {
                    activeCues[i].Cue.Stop(AudioStopOptions.AsAuthored);
                }
            }
        }

        /// <summary>
        /// pauses a cue.
        /// </summary>
        /// <param name="sound">playing sound element</param>
        private bool PauseSound(Cue3D cue3D)
        {
            //sanity check.
            if (cue3D.Cue == null || cue3D.Emitter == null || cue3D == null)
                return false;

            if (cue3D.Cue.IsPlaying)
            {
                cue3D.Cue.Pause();
                return true;
            }

            return false;
        }

        /// <summary>
        /// resumes a cue.
        /// </summary>
        /// <param name="sound">paused sound element</param>
        private bool ResumeSound(Cue3D cue3D)
        {
            //sanity check.
            if (cue3D.Cue == null || cue3D.Emitter == null || cue3D == null)
                return false;

            if (cue3D.Cue.IsPaused)
            {
                cue3D.Cue.Resume();
                return true;
            }

            return false;
        }


        /// <summary>
        /// pauses all cues.
        /// </summary>
        public void PauseAll()
        {
            if (pauseAll)
                return;

            for (int i = 0; i < activeCues.Count; i++)
                PauseSound(activeCues[i]);

            pauseAll = true;
        }

        /// <summary>
        /// resumes all cues.
        /// </summary>
        public void ResumeAll()
        {
            if (pauseAll == false)
                return;

            for (int i = 0; i < activeCues.Count; i++)
                ResumeSound(activeCues[i]);

            pauseAll = false;
        }


        public void ApplyEmitter(Vector3 position, Vector3 direction, Vector3 up,
                                 Vector3 velocity)
        {
            //  Update listener by the current camera
            this.listener.Position = position;
            this.listener.Forward = direction;
            this.listener.Up = up;
            this.listener.Velocity = velocity;
        }
    }
}
