#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;



using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SpaceShooter
{
    public class PlaybackMoveEvent
    {
        public float timeStamp;
        public Vector3 position;
        public Quaternion rotation;
        public bool isActive = true;
    }

    public enum objectType
    {
        bolt,
        ship,
        hulk,
        explosion,
        deflection,
        beamHit,
    }

    public class PlaybackItem
    {
        public PlaybackMoveEvent[] moveEvents;
        
        public Vector3 spawnPos = Vector3.Zero;

        public Vector3 position;
        public Quaternion rotation;
        public ModelType modelType;
        public objectType ObjectType;
        public Color color;
        public bool isStatic;

        /// <summary>
        /// Index of the last free slot in the moveArray.
        /// </summary>
        public int moveArrayIndex = 0;

        public PlaybackItem(int moveArraySize)
        {
            moveEvents = new PlaybackMoveEvent[moveArraySize];
        }

        public bool ShouldDraw(float curTime)
        {
            PlaybackMoveEvent activeEvent = null;
            for (int k = 0; k < moveArrayIndex; k++)
            {
                if (moveArrayIndex > moveEvents.Length)
                    break;

                if (moveEvents[k].timeStamp <= curTime)
                {
                    activeEvent = moveEvents[k];
                }
                else if (moveEvents[k].timeStamp > curTime)
                    break;
            }

            if (activeEvent == null)
                return false;

            return activeEvent.isActive;
        }
    }


    //This system records data from the game. ship positions, bullet positions, damage events, etc.
    public class PlaybackSystem
    {
        const int maxRoundtime = Helpers.MAXROUNDTIME;
        int roundNumber = 0;
        float worldTimer = 0;

        const int MOVEARRAYDEFAULT = 10;  //how many moveEvents an item can have.
        const int MOVEARRAYSHIPS = 256;    //how many MoveEvents a SHIP has.
        const int TOTALPLAYBACKITEMS = 30000; //total amount of items that can be recorded.

        /// <summary>
        /// This int determines the last free slot index in the playbackitems array.
        /// </summary>
        int itemIndex = 0;

        public int ItemIndex
        {
            get { return itemIndex; }
        }

        /// <summary>
        /// This GIANT array has a slot for every collideable/bullet that will be spawned in this level's lifetime.
        /// </summary>
        PlaybackItem[] playbackItems;

        public PlaybackSystem()
        {
            //pre allocate a giant array for every item that this level will ever create.
            playbackItems = new PlaybackItem[TOTALPLAYBACKITEMS];

            for (int i = 0; i < playbackItems.Length; i++)
            {
                int amount = MOVEARRAYDEFAULT;

                playbackItems[i] = new PlaybackItem(amount);

                ClearMoveArray(playbackItems[i], MOVEARRAYDEFAULT);
            }
        }

        private void ClearMoveArray(PlaybackItem item, int size)
        {
            item.moveEvents = new PlaybackMoveEvent[size];

            for (int k = 0; k < item.moveEvents.Length; k++)
            {
                item.moveEvents[k] = new PlaybackMoveEvent();
            }
        }

        public int RoundNumber
        {
            get { return roundNumber; }
        }

        public void ResetWorldTimer()
        {
            worldTimer = 0;
            roundNumber = 0;
        }
        
        public int MaxRoundTime
        {
            get { return maxRoundtime; }
        }

        public PlaybackItem[] PlaybackItems
        {
            get { return playbackItems; }
        }

        public float WorldTimer
        {
            get { return worldTimer; }
        }

        public void ClearAll()
        {
            itemIndex = 0;

            for (int i = 0; i < playbackItems.Length; i++)
            {
                playbackItems[i].moveArrayIndex = 0;

                ClearMoveArray(playbackItems[i], MOVEARRAYDEFAULT);
            }
        }


        public void AddExplosion(Vector3 pos)
        {
            if (itemIndex >= playbackItems.Length)
            {
                //uh oh we ran out of free slots.
                Console.WriteLine("ERROR: PLAYBACK SYSTEM HAS RUN OUT OF FREE SLOTS.");
                return;
            }

            playbackItems[itemIndex].ObjectType = objectType.explosion;

            playbackItems[itemIndex].moveEvents[0].timeStamp = worldTimer;
            playbackItems[itemIndex].moveEvents[0].position = pos;
            playbackItems[itemIndex].moveEvents[0].isActive = true;

            playbackItems[itemIndex].moveEvents[1].timeStamp = worldTimer+1000;
            playbackItems[itemIndex].moveEvents[1].position = pos;
            playbackItems[itemIndex].moveEvents[1].isActive = false;

            playbackItems[itemIndex].moveArrayIndex = 2;

            
            itemIndex++;
        }

        public void AddBeamHit(Vector3 hitPos, Vector3 origin, Color killerColor)
        {
            if (itemIndex >= playbackItems.Length)
            {
                //uh oh we ran out of free slots.
                Console.WriteLine("ERROR: PLAYBACK SYSTEM HAS RUN OUT OF FREE SLOTS.");
                return;
            }

            playbackItems[itemIndex].ObjectType = objectType.beamHit;
            playbackItems[itemIndex].spawnPos = origin;
            playbackItems[itemIndex].color = killerColor;

            playbackItems[itemIndex].moveEvents[0].timeStamp = worldTimer;
            playbackItems[itemIndex].moveEvents[0].position = hitPos;
            playbackItems[itemIndex].moveEvents[0].isActive = true;

            playbackItems[itemIndex].moveEvents[1].timeStamp = worldTimer + 300;
            playbackItems[itemIndex].moveEvents[1].position = hitPos;
            playbackItems[itemIndex].moveEvents[1].isActive = false;

            playbackItems[itemIndex].moveArrayIndex = 2;

            itemIndex++;
        }

        public void AddDeflection(Vector3 pos)
        {
            if (itemIndex >= playbackItems.Length)
            {
                //uh oh we ran out of free slots.
                Console.WriteLine("ERROR: PLAYBACK SYSTEM HAS RUN OUT OF FREE SLOTS.");
                return;
            }

            playbackItems[itemIndex].ObjectType = objectType.deflection;

            playbackItems[itemIndex].moveEvents[0].timeStamp = worldTimer;
            playbackItems[itemIndex].moveEvents[0].position = pos;
            playbackItems[itemIndex].moveEvents[0].isActive = true;

            playbackItems[itemIndex].moveEvents[1].timeStamp = worldTimer + 1000;
            playbackItems[itemIndex].moveEvents[1].position = pos;
            playbackItems[itemIndex].moveEvents[1].isActive = false;

            playbackItems[itemIndex].moveArrayIndex = 2;


            itemIndex++;
        }



        /// <summary>
        /// Add and activate a new playbackitem.
        /// </summary>
        /// <param name="newItem"></param>
        public void AddItem(Entity newItem, objectType objType, Color color)
        {
            if (itemIndex >= playbackItems.Length)
            {
                //uh oh we ran out of free slots.
                Console.WriteLine("ERROR: PLAYBACK SYSTEM HAS RUN OUT OF FREE SLOTS.");
                return;
            }

            //give the ship/bullet/whatever an id tag so it knows what playback item it's linked to.
            newItem.SetPlaybackID(itemIndex);

            //reset the movearray start position.
            playbackItems[itemIndex].moveArrayIndex = 0;

            playbackItems[itemIndex].spawnPos = newItem.Position;
            playbackItems[itemIndex].modelType = newItem.modelMesh;
            playbackItems[itemIndex].ObjectType = objType;
            playbackItems[itemIndex].color = color;

            playbackItems[itemIndex].isStatic = newItem.isStatic;


            if (objType == objectType.ship)
            {
                //give this item a much larger moveEvent array, if it isn't already resized.
                if (playbackItems[itemIndex].moveEvents.Length < MOVEARRAYSHIPS)
                    ClearMoveArray(playbackItems[itemIndex], MOVEARRAYSHIPS);
            }

            AddMoveEvent(newItem, false);

            itemIndex++;
        }

        private void AddMoveEvent(Entity newItem, bool killEvent)
        {
            if (newItem.PlaybackID <= -1)
                return;

            int thisIndex = newItem.PlaybackID;

            //do a check: don't add a moveEvent if the item has not moved or rotated.
            /*
            if (playbackItems[thisIndex].moveArrayIndex > 0)
            {
                if (playbackItems[thisIndex].moveEvents[playbackItems[thisIndex].moveArrayIndex - 1].position == newItem.Position
                    &&
                    playbackItems[thisIndex].moveEvents[playbackItems[thisIndex].moveArrayIndex - 1].rotation == newItem.Rotation)
                {
                    return;
                }
            }*/

            if (playbackItems[thisIndex].moveArrayIndex >= playbackItems[thisIndex].moveEvents.Length)
            {
                Console.WriteLine("ERROR: MOVEARRAY RAN OUT OF FREE SLOTS: " + playbackItems[thisIndex].modelType);
                return;
            }


            playbackItems[thisIndex].moveEvents[playbackItems[thisIndex].moveArrayIndex].timeStamp = worldTimer;
            playbackItems[thisIndex].moveEvents[playbackItems[thisIndex].moveArrayIndex].position = newItem.Position;
            playbackItems[thisIndex].moveEvents[playbackItems[thisIndex].moveArrayIndex].rotation = newItem.Rotation;

            playbackItems[thisIndex].moveEvents[playbackItems[thisIndex].moveArrayIndex].isActive = !killEvent;

            playbackItems[thisIndex].moveArrayIndex++;
        }


        public void UpdateItem(Entity entity)
        {
            AddMoveEvent(entity, false);
        }

        public void UpdateItemPositions(List<Collideable> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsDestroyed)
                    continue;

                //see if item has a valid tag.
                if (items[i].PlaybackID <= -1)
                    continue;

                if (items[i].isStatic)
                    continue;

                AddMoveEvent(items[i], false);
            }
        }

        public void UpdateBoltPositions(Bolt[] bolts)
        {
            for (int i = 0; i < bolts.Length; i++)
            {
                if (!bolts[i].isActive)
                    continue;

                //see if item has a valid tag.
                if (bolts[i].PlaybackID <= -1)
                    continue;

                AddMoveEvent(bolts[i], false);
            }
        }


        public void KillItem(Entity targetItem)
        {
            //check if item has valid id tag.
            if (targetItem.PlaybackID <= -1)
                return;

            AddMoveEvent(targetItem, true);
        }
        

        public void IncreaseRound()
        {
            roundNumber++;
        }

        public bool Update(GameTime gameTime)
        {
            //demo mode ignores the playback system.
            if (FrameworkCore.level.isDemo)
                return true;

            worldTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //hit the end of the round timer. 
            if (worldTimer >= roundNumber * maxRoundtime)
            {
                return false;
            }

            return true;
        }

        public void Draw(GameTime gameTime)
        {
        }
    }
}