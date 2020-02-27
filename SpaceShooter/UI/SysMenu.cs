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
    public class SysMenuManager
    {
        List<SysMenu> sysMenus = new List<SysMenu>();
        public List<SysMenu> menus
        {
            get { return sysMenus; }
        }

        float fadeTransition = 0;


        public bool SkirmishInStack()
        {
            for (int x = 0; x < sysMenus.Count; x++)
            {
                if (sysMenus[x].GetType() == typeof(SkirmishMenu))
                    return true;
            }

            return false;
        }

        public void ClearAll()
        {
            sysMenus.Clear();
        }

        public void UpdateTopControls(GameTime gameTime, InputManager input)
        {
            for (int x = 0; x < sysMenus.Count; x++)
            {
                SysMenu menu = sysMenus[x];
                if (menu == sysMenus[sysMenus.Count - 1])
                {
                    //update the menu at the top of the stack.
                    menu.Update(gameTime, input);
                }
            }
        }

        public bool Update(GameTime gameTime, InputManager input)
        {
            if (sysMenus.Count <= 0)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                                TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                fadeTransition = MathHelper.Clamp(fadeTransition - delta, 0, 1);
                return false;
            }
            else
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                                TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                fadeTransition = MathHelper.Clamp(fadeTransition + delta, 0, 1);
            }

            for (int x = 0; x < sysMenus.Count; x++)
            {
                SysMenu menu = sysMenus[x];
                if (menu == sysMenus[sysMenus.Count - 1])
                {
                    //update the menu at the top of the stack.
                    menu.Update(gameTime, input);
                    menu.UpdateTransition(gameTime);
                }
                else if (sysMenus[sysMenus.Count - 1].hideChildren)
                {
                    menu.UpdateTransition(gameTime);
                }
            }

            return true;
        }

        

        public void CloseAll()
        {
            sysMenus.Clear();
        }

        public void AddMenu(SysMenu menu)
        {
            sysMenus.Add(menu);
            menu.SetOwner(this);
            menu.Activate();
        }

        public void CloseMenu(SysMenu menu)
        {
            sysMenus.Remove(menu);
        }

        public void Draw(GameTime gameTime)
        {
            /*
            if (fadeTransition > 0)
            {
                int alpha = (int)MathHelper.Lerp(0, 128, fadeTransition);
                Helpers.DarkenScreen(alpha);
            }
            */

            if (sysMenus == null)
                return;

            if (sysMenus.Count <= 0)
                return;


            for (int x = 0; x < sysMenus.Count; x++)
            {
                if (sysMenus[x] == null)
                    continue;

                sysMenus[x].Draw(gameTime);
            }
        }
    }

    public abstract class baseMenu
    {
        public virtual void Deactivate()
        {
        }

        public virtual void InitializeItems()
        {
        }
    }

    public class SysMenu : baseMenu
    {
        public enum MenuState
        {
            Deactivated,
            TransitionOn,
            Active,
            TransitionOff
        }

        SysMenuManager owner;

        bool mouseHover;
        public bool MouseHover
        {
            get { return mouseHover; }
        }

        /// <summary>
        /// When this menu is active, should menus behind me transition away? Defaults to true.
        /// </summary>
        public bool hideChildren = true;

        public bool darkenScreen = false;

        public SysMenuManager Owner
        {
            get { return owner; }
        }


        public SpriteFont menuFont = FrameworkCore.SerifBig;

        public int transitionOnTime = 300;
        public int transitionOffTime = 300;

        float transition;
        MenuState menustate;

        public bool canBeExited = true;

        public MenuState menuState
        {
            get { return menustate; }
        }

        public float Transition
        {
            get { return transition; }
        }

        public List<MenuItem> menuItems = new List<MenuItem>();
        public MenuItem selectedItem = null;

        public SysMenu()
        {
        }

        public SysMenu(SysMenuManager owner)
        {
            this.owner = owner;

            InitializeItems();
        }


        public void SetOwner(SysMenuManager owner)
        {
            this.owner = owner;
        }

        public virtual void RepositionItems()
        {
            int topY = int.MaxValue;
            int bottomY = -int.MaxValue;

            if (menuItems.Count <= 0)
                return;

            topY = (int)menuItems[0].position.Y;

            bottomY = (int)menuItems[menuItems.Count - 1].position.Y;

            //get total height of all the items.
            int windowHeight = bottomY - topY;

            int newStartY = (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2) - (windowHeight/2);


            int delta = newStartY - topY;
               

            foreach (MenuItem item in menuItems)
            {
                item.position.Y += delta;
                item.optionPos.Y = item.position.Y;
            }
        }


        public void AddItem(MenuItem item)
        {
            item.owner = this;
            menuItems.Add(item);
        }

        public void AddItem(string txt, MenuItem.InputEventHandler handle)
        {
            MenuItem item = new MenuItem(txt);
            item.Selected += handle;
            item.owner = this;
            menuItems.Add(item);

            InitializeItems();
        }

        public void UpdateItemText(object sender)
        {
            if (sender.GetType() != typeof(MenuItem))
                return;

            ((MenuItem)sender).OnSetText();
        }

        private void UpdateSlider(GameTime gameTime, MenuItem item, InputManager inputManager)
        {
            if (inputManager.sysMenuLeft)
            {
                item.onSliderDecrease();
            }
            else if (inputManager.sysMenuRight)
            {
                item.onSliderIncrease();
            }
        }

        public virtual void SelectUp()
        {
        }

        public virtual void SelectDown()
        {
        }

        public virtual void Update(GameTime gameTime, InputManager inputManager)
        {
            if (menustate == MenuState.Deactivated)
                return;

            if (canBeExited)
            {
                if (inputManager.buttonBPressed ||
                    inputManager.kbEscPressed)
                {
                    Deactivate();
                    FrameworkCore.PlayCue(sounds.click.back);
                }
            }

            if (menuItems.Count <= 0)
                return;

            //ignore button inputs that happen before menu transitions in
            if (Transition < 1.0f)
                return;


            if (selectedItem != null && selectedItem.itemType == MenuItem.menuItemType.slider)
            {
                UpdateSlider(gameTime, selectedItem, inputManager);
            }


            if (inputManager.buttonAPressed || inputManager.kbEnter)
            {
                ActivateItem(inputManager);
            }

           
            if (inputManager.sysMenuDown)
            {
                int index = menuItems.IndexOf(selectedItem);
                index++;

                if (index > menuItems.Count - 1)
                    index = 0;

                selectedItem = menuItems[index];

                menuItems[index].shipArraySelection = -1;

                SelectDown();
                
                if (menuItems.Count > 1)
                    FrameworkCore.PlayCue(sounds.click.select);
            }

            if (inputManager.sysMenuUp)
            {
                int index = menuItems.IndexOf(selectedItem);
                index--;

                if (index < 0)
                    index = menuItems.Count - 1;

                selectedItem = menuItems[index];

                menuItems[index].shipArraySelection = -1;

                SelectUp();

                if (menuItems.Count > 1)
                    FrameworkCore.PlayCue(sounds.click.select);
            }
        }

        

        public void UpdateMouseItems(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            MenuItem hoverItem = mouseIsHovering(inputManager);

            if (hoverItem != null)
            {
                mouseHover = true;

                if (inputManager.mouseHasMoved)
                    selectedItem = hoverItem;

                if (inputManager.mouseLeftClick)
                {
                    ActivateItem(inputManager);
                }
            }
            else
            {
                mouseHover = false;

                if (inputManager.mouseHasMoved)
                    selectedItem = null;
            }
#endif
        }

        

        public void ActivateItem(InputManager inputManager)
        {
            if (selectedItem == null)
                return;

            if (selectedItem.shipArraySelection >= 0)
                return;

            selectedItem.activateTransition = 0;
            selectedItem.OnSelectEntry(inputManager.playerIndex);

            //if (!FrameworkCore.isActive)
            //    return;

            FrameworkCore.PlayCue(sounds.click.activate);
        }

        private MenuItem mouseIsHovering(InputManager inputManager)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                Vector2 lineSize = menuFont.MeasureString(menuItems[i].text);

                Rectangle itemRect = new Rectangle(
                    (int)menuItems[i].position.X,
                    (int)(menuItems[i].position.Y - lineSize.Y / 2),
                    (int)lineSize.X,
                    (int)lineSize.Y);

                Point mousePoint = new Point((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y);

                if (itemRect.Contains(mousePoint))
                {
                    return menuItems[i];                    
                }
            }

            return null;
        }

        public bool TopOfStack()
        {
            if (owner.menus.Count <= 0)
                return false;

            if (owner == null)
                return false;
            else
            {
                if (owner.menus[owner.menus.Count - 1] == this)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// determines if the top parent menu has HideChildren flag on.
        /// </summary>
        /// <returns></returns>
        private bool shouldHide()
        {
            if (owner.menus.Count <= 0)
                return false;

            if (owner == null)
                return false;
            else
            {
                if (owner.menus[owner.menus.Count - 1].hideChildren)
                    return true;
            }

            return false;
        }

        public void UpdateTransition(GameTime gameTime)
        {
            if (menustate == MenuState.Active)
                UpdateItemTransition(gameTime);


            if (menustate == MenuState.TransitionOff)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                        TimeSpan.FromMilliseconds(transitionOffTime).TotalMilliseconds);

                transition = MathHelper.Clamp(Transition - delta, 0, 1);

                if (transition <= 0)
                {
                    menustate = MenuState.Deactivated;

                    if (owner != null)
                    {
                        owner.CloseMenu(this);
                    }
                }
            }
            else if (!TopOfStack() && shouldHide())
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                                        TimeSpan.FromMilliseconds(transitionOffTime).TotalMilliseconds);

                transition = MathHelper.Clamp(Transition - delta, 0, 1);
            }
            else if (menustate == MenuState.TransitionOn || menustate == MenuState.Active)
            {
                float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                    TimeSpan.FromMilliseconds(transitionOnTime).TotalMilliseconds);

                transition = MathHelper.Clamp(Transition + delta, 0, 1);

                if (transition >= 1)
                {
                    //sanity check. if there's no menuItem selected, then select one dammit!
                    if (selectedItem == null && menuItems.Count > 0 && menustate == MenuState.TransitionOn)
                    {
                        selectedItem = menuItems[0];
                    }

                    menustate = MenuState.Active;
                }
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

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            menustate = MenuState.TransitionOff;
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public virtual void Activate()
        {
            if (menuItems.Count > 0)
            {
                selectedItem = menuItems[0];
            }

            menustate = MenuState.TransitionOn;
        }

        public virtual void Draw(GameTime gameTime)
        {
            
        }

        public void DrawDarkenScreen()
        {
            if (darkenScreen)
            {
                int alpha = (int)MathHelper.Lerp(0, 192, Transition);
                Helpers.DarkenScreen(alpha);
            }
        }

        public virtual void DrawItems(GameTime gameTime, Vector2 pos)
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            foreach (MenuItem item in menuItems)
            {
                Color itemColor = Color.White;                
                float itemSize = Helpers.PopLerp(item.selectTransition, 0.8f, 1.1f, 1.0f);


                if (selectedItem != null && selectedItem == item)
                {
                    itemColor = Color.Lerp(itemColor, new Color(255, 128, 0), item.selectTransition);
                }

                itemColor = Color.Lerp(OldXNAColor.TransparentWhite, itemColor, transition);

                FrameworkCore.SpriteBatch.DrawString(menuFont, item.text, pos,
                    itemColor, 0, new Vector2(0,(textVec.Y * itemSize)*0.5f), itemSize, SpriteEffects.None, 0);

                pos.Y += textVec.Y;
                pos.Y += 8; //gap between items.
            }
        }

        public virtual void DrawItems(GameTime gameTime, float xOffset)
        {
            Vector2 textVec = menuFont.MeasureString("Sample");

            foreach (MenuItem item in menuItems)
            {
                Color itemColor = Color.White;
                float itemSize = Helpers.PopLerp(item.selectTransition, 0.8f, 1.1f, 1.0f);

                if (selectedItem != null && selectedItem == item)
                {
                    itemColor = Color.Lerp(itemColor, new Color(255, 128, 0), item.selectTransition);
                }

                itemColor = Color.Lerp(OldXNAColor.TransparentWhite, itemColor, transition);

                Vector2 itemPos = item.position;
                itemPos.X += xOffset;

                FrameworkCore.SpriteBatch.DrawString(menuFont, item.text, itemPos,
                    itemColor, 0, new Vector2(0, (textVec.Y * itemSize) * 0.5f), itemSize, SpriteEffects.None, 0);
            }
        }
    }
}
