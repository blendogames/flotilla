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
#endregion

namespace SpaceShooter
{
    public class MenuItem
    {
        public Rectangle hitBox;
        public Vector2 hitCursor;


        public enum menuItemType
        {
            button,
            slider,
            checkbox,
            list,
        }

        public menuItemType itemType = menuItemType.button;

        public delegate void InputEventHandler(object sender, InputArgs e);


        public string text;
        public Vector2 position;
        public float selectTransition = 0;
        public event InputEventHandler Selected;
        public Rectangle iconRect;

        public event EventHandler sliderDecrease;
        public event EventHandler sliderIncrease;

        public event EventHandler setOptionText;
        

        public GameEffect gameEffect = null;  //a game effect can be associated with menu items. i.e. Special Orders.
        public float activateTransition = 0;

        public Vector2 basePosition;
        public Vector2 targetPosition;

        public baseMenu owner;

        public Commander commander;

        /// <summary>
        /// generic integer data.
        /// </summary>
        public int GenericInt1;

        public int GenericInt2;


        public int minValue = 0;
        public int maxValue = 10;
        public int stepInterval = 1;

        public string optionText = "";
        public Vector2 optionPos = Vector2.Zero;
        public bool optionBool = false;


        public ShipData[] shipArray;
        public int shipArraySelection = -1;
        

        /// <summary>
        /// An item in the menu.
        /// </summary>
        /// <param name="Text">Text string displayed on the item.</param>
        public MenuItem(string Text)
        {
            this.text = Text;
            this.shipArray = new ShipData[6];
        }

        protected internal virtual void OnSelectEntry(PlayerIndex index)
        {
            if (Selected != null)
                Selected(this, new InputArgs(index));
        }

        protected internal virtual void OnSetText()
        {
            if (setOptionText != null)
                setOptionText(this, null);
        }

        protected internal virtual void onSliderDecrease()
        {
            if (sliderDecrease != null)
                sliderDecrease(this, null);
        }

        protected internal virtual void onSliderIncrease()
        {
            if (sliderIncrease != null)
                sliderIncrease(this, null);
        }
    }

    public class InputArgs : EventArgs
    {
        public InputArgs(PlayerIndex index)
        {
            this.index = index;
        }

        public PlayerIndex index;
    }  

    public class Menu : baseMenu
    {
        public bool mouseIsHovering = false;

        public float lbTransition = 0;
        public float rbTransition = 0;

        public PlayerCommander owner;
        float transition;

        public float Transition
        {
            get { return transition; }
        }

        public List<MenuItem> menuItems = new List<MenuItem>();
        public MenuItem selectedItem = null;


        bool hasFocus;
        public bool HasFocus
        {
            get { return hasFocus; }
        }


        public Menu(Game game, PlayerCommander owner)
        {
            this.owner = owner;
        }



        public void AddItem(MenuItem item)
        {
            item.owner = this;
            menuItems.Add(item);
        }



        public void ForceOff()
        {
            if (transition > 0)
                transition = 0.01f;

            Deactivate();
        }


        public void ActivateItem(InputManager inputManager)
        {
            if (selectedItem == null)
                return;

            selectedItem.activateTransition = 0;
            selectedItem.OnSelectEntry(inputManager.playerIndex);

            FrameworkCore.PlayCue(sounds.click.activate);
        }

        public virtual void Update(GameTime gameTime, InputManager inputManager)
        {
            if (Transition < 1)
                return;
            
            if (inputManager.buttonBPressed)
            {
                Deactivate();
                FrameworkCore.PlayCue(sounds.click.back);
            }

            if (inputManager.buttonAPressed || (mouseIsHovering && inputManager.mouseLeftClick) ||
                inputManager.kbEnter)
            {
                ActivateItem(inputManager);
            }

            if (inputManager.menuNextPressed)
            {
                int index = menuItems.IndexOf(selectedItem);
                index++;

                if (index > menuItems.Count - 1)
                    index = 0;

                selectedItem = menuItems[index];

                FrameworkCore.PlayCue(sounds.click.select);
            }

            if (inputManager.menuPrevPressed)
            {
                int index = menuItems.IndexOf(selectedItem);
                index--;

                if (index < 0)
                    index = menuItems.Count - 1;

                selectedItem = menuItems[index];
                FrameworkCore.PlayCue(sounds.click.select);
            }


            if (!inputManager.menuPrevHeld)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(150).TotalMilliseconds);

                lbTransition = MathHelper.Clamp(lbTransition - delta, 0, 1);
            }
            else if (lbTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(40).TotalMilliseconds);

                lbTransition = MathHelper.Clamp(lbTransition + delta, 0, 1);
            }

            if (!inputManager.menuNextHeld)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(150).TotalMilliseconds);

                rbTransition = MathHelper.Clamp(rbTransition - delta, 0, 1);
            }
            else if (rbTransition < 1)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(40).TotalMilliseconds);

                rbTransition = MathHelper.Clamp(rbTransition + delta, 0, 1);
            }
        }

        public void UpdateTransition(GameTime gameTime)
        {
            if (owner.ActiveMenu == this)
                hasFocus = true;
            else
                hasFocus = false;
            

            if (HasFocus)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(400).TotalMilliseconds);

                transition = MathHelper.Clamp(Transition + delta, 0, 1);

                UpdateItemTransition(gameTime);
            }
            else
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                        TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                transition = MathHelper.Clamp(Transition - delta, 0, 1);
            }

            foreach (MenuItem item in menuItems)
            {
                if (item.activateTransition < 1)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                        TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                    item.activateTransition = MathHelper.Clamp(item.activateTransition + delta, 0, 1);
                }
            }
        }

        public void UpdateItemTransition(GameTime gameTime)
        {
            foreach (MenuItem item in menuItems)
            {
                if (selectedItem == item)
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(200).TotalMilliseconds);

                    item.selectTransition = MathHelper.Clamp(item.selectTransition + delta, 0, 1);
                }
                else
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(200).TotalMilliseconds);

                    item.selectTransition = MathHelper.Clamp(item.selectTransition - delta, 0, 1);
                }
            }
        }

        public override void Deactivate()
        {
            owner.ActiveMenu = null;
        }

        public virtual void Activate()
        {
            if (menuItems.Count > 0)
            {
                selectedItem = menuItems[0];
            }
        }
        
    }
}
