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
    public class TradeMenu : SysMenu
    {
        enum TradeState
        {
            intro,
            selectTraderItem,
            selectPlayerItem,
            fanfare,
            popup,
        }

        int selectedIndex = -1;

        TradeState tradeState;
        float[] stateTransitions;

        List<InventoryMenuItem> traderMenuItems;
        List<InventoryMenuItem> playerMenuItems;

        int ITEMWIDTH = 80;
        int LINESIZE;

        readonly Color pinkColor = new Color(240, 150, 150);

        float fanfareTransition = 0;
        const int FANFAREMAXDELAY = 500;
        int fanfareDelay = 500;

        Vector2[] confirmButtons;
        int hoverConfirmButton = -1;

        public TradeMenu()
        {
            canBeExited = false;

            stateTransitions = new float[3];

            darkenScreen = true;

            LINESIZE = (int)FrameworkCore.Serif.MeasureString("S").Y;

            PopulateMenuItems();

            confirmButtons = new Vector2[1]
            {
                new Vector2(200,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 70),
            };

#if XBOX
            selectedIndex = 0;
#endif
        }

        private bool HasSufficientCargo
        {
            get
            {
                return (FrameworkCore.players[0].inventoryItems.Count >= 2);
            }
        }

        private void PopulateMenuItems()
        {
            //populate the trader's menu items.
            traderMenuItems = new List<InventoryMenuItem>();
            PopulateMenuItems(traderMenuItems, FrameworkCore.worldMap.evManager.tradeItems,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - 160);

            playerMenuItems = new List<InventoryMenuItem>();
            PopulateMenuItems(playerMenuItems, FrameworkCore.players[0].inventoryItems,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 + 150);
        }

        private void PopulateMenuItems(List<InventoryMenuItem> listToPopulate, List<InventoryItem> sourceList, float Y)
        {
            for (int i = 0; i < sourceList.Count; i++)
            {
                InventoryMenuItem newItem = new InventoryMenuItem(sourceList[i].name);
                newItem.inventoryItem = sourceList[i];
                listToPopulate.Add(newItem);
            }

            //position the menu elements.
            Vector2 startPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2, Y);
            
            startPos.X -= (listToPopulate.Count / 2) * ITEMWIDTH/*width of each item*/;

            for (int i = 0; i < listToPopulate.Count; i++)
            {
                listToPopulate[i].position = startPos;
                listToPopulate[i].targetPosition = startPos;

                startPos.X += ITEMWIDTH;
            }
        }

        private void UpdateStateTransitions(GameTime gameTime)
        {
            float stateDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                            TimeSpan.FromMilliseconds(400).TotalMilliseconds);

            for (int i = 0; i < stateTransitions.Length; i++)
            {
                if (i == (int)tradeState)
                {
                    stateTransitions[i] = MathHelper.Clamp(stateTransitions[i] + stateDelta, 0, 1);
                }
                else
                    stateTransitions[i] = MathHelper.Clamp(stateTransitions[i] - stateDelta, 0, 1);
            }
        }

        int mouseScrollTimer = 300;

        private void UpdateMouse(GameTime gameTime, InputManager inputManager, List<InventoryMenuItem> items)
        {
            if (items == null)
                return;

            bool isHovering = false;
            int boxSize = 64;
            for (int i = 0; i < items.Count; i++)
            {
                Rectangle itemRect = new Rectangle(
                    (int)items[i].position.X - boxSize/2,
                    (int)(items[i].position.Y - boxSize*1.5f),
                    boxSize,
                    boxSize*3);

                if (itemRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    isHovering = true;

                    if (inputManager.mouseHasMoved)
                        selectedIndex = items.IndexOf(items[i]);

                    if (inputManager.mouseLeftClick)
                    {
                        FrameworkCore.PlayCue(sounds.click.activate);

                        selectItem(items.IndexOf(items[i]));
                    }
                }
            }

            if (!isHovering && inputManager.mouseHasMoved)
            {
                //nothing selected.
                selectedIndex = -1;
            }

            bool confHover = false;
            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Rectangle confirmRect = new Rectangle(
                    (int)(confirmButtons[i].X - 150),
                    (int)(confirmButtons[i].Y - 30),
                    300,
                    60);

                if (confirmRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    hoverConfirmButton = i;
                    confHover = true;

                    if (inputManager.mouseLeftClick)
                    {
                        if (tradeState == TradeState.selectPlayerItem)
                        {
                            CancelPlayerItemSelect();
                        }
                        else
                        {
                            Deactivate();
                        }
                        
                    }
                }
            }


            //scroll the item lists.
            //if (inputManager.mouseHasMoved)
            {
                mouseScrollTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (mouseScrollTimer <= 0)
                {
                    mouseScrollTimer = 300;

                    int hitboxWidth = (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width * 0.2f);

                    Rectangle leftRect = new Rectangle(
                        0, (int)items[0].position.Y - ITEMWIDTH,
                        (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width * 0.2f),
                        ITEMWIDTH * 2);

                    Rectangle rightRect = new Rectangle(
                        (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - hitboxWidth,
                        (int)items[0].position.Y - ITEMWIDTH,
                        hitboxWidth,
                        ITEMWIDTH * 2);

                    if (leftRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                    {
                        RefocusPositionsByDifference(256, items);
                    }
                    else if (rightRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                    {
                        RefocusPositionsByDifference(-256, items);
                    }
                }
            }

            if (!confHover)
                hoverConfirmButton = -1;
        }

        int selectedTraderItem = -1;
        int selectedPlayerItem1 = -1;
        int selectedPlayerItem2 = -1;

        private void selectItem(int index)
        {
            if (index < 0)
                return;

            if (!HasSufficientCargo)
                return;

            FrameworkCore.PlayCue(sounds.click.activate);

            //select an item.
            if (tradeState == TradeState.selectTraderItem)
            {
                if (index == selectedTraderItem)
                {
                    //player selected already-selected item.
                    selectedTraderItem = -1;
                    return;
                }

                selectedTraderItem = index;

                for (int i = 0; i < traderMenuItems.Count; i++)
                {
                    //clear transitions.
                    traderMenuItems[i].activateTransition = 0;
                }

#if XBOX
                selectedIndex = 0;
#endif

                tradeState = TradeState.selectPlayerItem;
            }
            else if (tradeState == TradeState.selectPlayerItem)
            {
                if (index == selectedPlayerItem1)
                {
                    selectedPlayerItem1 = -1;
                    return;
                }
                else if (index == selectedPlayerItem2)
                {
                    selectedPlayerItem2 = -1;
                    return;
                }

                if (selectedPlayerItem1 < 0)
                    selectedPlayerItem1 = index;
                else if (selectedPlayerItem2 < 0)
                    selectedPlayerItem2 = index;

                if (selectedPlayerItem1 >= 0 && selectedPlayerItem2 >= 0)
                {
                    for (int i = 0; i < playerMenuItems.Count; i++)
                    {
                        //clear transitions.
                        playerMenuItems[i].activateTransition = 0;
                    }

                    tradeState = TradeState.fanfare;

                    FrameworkCore.PlayCue(sounds.Fanfare.chaching);
                }
            }
        }

        private void UpdateItemTransitions(GameTime gameTime, List<InventoryMenuItem> items)
        {
            if (items == null)
                return;

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                TimeSpan.FromMilliseconds(300).TotalMilliseconds);

            for (int i = 0; i < items.Count; i++)
            {
                if (selectedIndex == i)
                    items[i].activateTransition = MathHelper.Clamp(items[i].activateTransition + delta, 0, 1);
                else
                    items[i].activateTransition = MathHelper.Clamp(items[i].activateTransition - delta, 0, 1);

                items[i].position = Vector2.Lerp(items[i].position, items[i].targetPosition,
                    4f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        private void UpdateFanfare(GameTime gameTime)
        {
            if (tradeState != TradeState.fanfare)
                return;

            if (fanfareDelay > 0)
            {
                fanfareDelay -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                return;
            }

            if (fanfareTransition <= 0)
            {
                FrameworkCore.PlayCue(sounds.click.whoosh);
            }

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                TimeSpan.FromMilliseconds(2000).TotalMilliseconds);

            fanfareTransition = MathHelper.Clamp(fanfareTransition + delta, 0, 1);

            if (fanfareTransition >= 1)
            {
                //Add items to the trader.
                FrameworkCore.worldMap.evManager.tradeItems.Add(FrameworkCore.players[0].inventoryItems[selectedPlayerItem1]);
                FrameworkCore.worldMap.evManager.tradeItems.Add(FrameworkCore.players[0].inventoryItems[selectedPlayerItem2]);

                //Remove equipped items from ships.
                for (int x = FrameworkCore.players[0].campaignShips.Count - 1; x >= 0; x--)
                {
                    for (int i = 0; i < FrameworkCore.players[0].campaignShips[x].upgradeArray.Length; i++)
                    {
                        if (FrameworkCore.players[0].campaignShips[x].upgradeArray[i] ==
                            playerMenuItems[selectedPlayerItem1].inventoryItem)
                            FrameworkCore.players[0].campaignShips[x].upgradeArray[i] = null;

                        if (FrameworkCore.players[0].campaignShips[x].upgradeArray[i] ==
                            playerMenuItems[selectedPlayerItem2].inventoryItem)
                            FrameworkCore.players[0].campaignShips[x].upgradeArray[i] = null;

                    }
                }
                

                //Remove items from the player inventory.
                for (int x = FrameworkCore.players[0].inventoryItems.Count - 1; x >= 0; x--)
                {
                    if (x == selectedPlayerItem1)
                        FrameworkCore.players[0].inventoryItems.RemoveAt(x);
                    else if (x == selectedPlayerItem2)
                        FrameworkCore.players[0].inventoryItems.RemoveAt(x);
                }
                

                //Give the player the item. Add item to player inventory.
                FrameworkCore.worldMap.evManager.AddCargo(traderMenuItems[selectedTraderItem].inventoryItem);
                
                //remove the trader item.
                FrameworkCore.worldMap.evManager.tradeItems.RemoveAt(selectedTraderItem);


                ClearStates();

                //repopulate the menu items.
                PopulateMenuItems();                

                

                tradeState = TradeState.popup;
            }
        }

        private void ClearStates()
        {
            //clear out all the trade data.
            selectedIndex = -1;
            selectedTraderItem = -1;
            selectedPlayerItem1 = -1;
            selectedPlayerItem2 = -1;

            //clear out fanfare data.
            fanfareDelay = FANFAREMAXDELAY;
            fanfareTransition = 0;

#if XBOX
            selectedIndex = 0;
#endif
        }

        private void UpdatePopup(GameTime gameTime)
        {
            if (tradeState != TradeState.popup)
                return;


            //check if we're the top menu.
            if (Owner.menus[Owner.menus.Count - 1] == this)
            {
                tradeState = TradeState.intro;
            }
        }

        private void CancelPlayerItemSelect()
        {
            FrameworkCore.PlayCue(sounds.click.back);

            ClearStates();
            tradeState = TradeState.selectTraderItem;
        }

        private void UpdatePad(GameTime gameTime, InputManager inputManager, List<InventoryMenuItem> items)
        {
            if (items.Count <= 0)
                return;

            bool refocus = false;

            if (inputManager.sysMenuRight)
            {
                if (selectedIndex >= items.Count-1)
                    selectedIndex = 0;
                else
                    selectedIndex++;

                refocus = true;
            }
            else if (inputManager.sysMenuLeft)
            {
                if (selectedIndex <= 0)
                    selectedIndex = items.Count - 1;
                else
                    selectedIndex--;

                refocus = true;
            }

            if (refocus)
            {
                RefocusPositionsByIndex(selectedIndex, items);
            }

            if (inputManager.mouseWheelUp)
            {
                selectedIndex = -1;
                //scroll BACK
                RefocusPositionsByDifference(256, items);
            }
            else if (inputManager.mouseWheelDown)
            {
                selectedIndex = -1;

                //scroll FORWARD
                RefocusPositionsByDifference(-256, items);
            }


            if (inputManager.buttonAPressed || inputManager.kbEnter)
            {
                selectItem(selectedIndex);
            }
        }

        private void RefocusPositionsByIndex(int index, List<InventoryMenuItem> items)
        {
            if (items.Count <= 0)
                return;

            int itemX = (int)items[index].position.X;

            int difference = (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - itemX;

            RefocusPositionsByDifference(difference, items);
        }

        private void RefocusPositionsByDifference(int delta, List<InventoryMenuItem> items)
        {
            if (items.Count <= 0)
                return;

            if (items[0].position.X + delta > FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2)
            {
                return;
            }
            else if (items[items.Count-1].position.X + delta < FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2)
            {
                return;
            }
            

            for (int i = 0; i < items.Count; i++)
            {
                items[i].targetPosition.X = items[i].position.X + delta;
            }
        }

        int lastSelectedIndex = -1;

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (Transition >= 1)
            {
                if (lastSelectedIndex != selectedIndex)
                {
                    if (selectedIndex > -1)
                        FrameworkCore.PlayCue(sounds.click.select);

                    lastSelectedIndex = selectedIndex;
                }

                UpdateStateTransitions(gameTime);
                UpdateFanfare(gameTime);
                UpdatePopup(gameTime);

                if (tradeState == TradeState.intro)
                {
                    if (!HasSufficientCargo)
                        FrameworkCore.PlayCue(sounds.click.error);

                    tradeState = TradeState.selectTraderItem;

                    return;
                }
                else if (tradeState == TradeState.selectTraderItem)
                {
                    UpdateItemTransitions(gameTime, traderMenuItems);

                    if (inputManager.kbBackspaceJustPressed || inputManager.buttonBPressed)
                    {
                        ClearStates();
                        Deactivate();
                    }

#if WINDOWS
                    UpdateMouse(gameTime, inputManager, traderMenuItems);
#endif
                    UpdatePad(gameTime, inputManager, traderMenuItems);

                }
                else if (tradeState == TradeState.selectPlayerItem)
                {
                    UpdateItemTransitions(gameTime, playerMenuItems);

                    if (inputManager.kbBackspaceJustPressed || inputManager.buttonBPressed)
                    {
                        CancelPlayerItemSelect();
                    }

#if WINDOWS
                    UpdateMouse(gameTime, inputManager, playerMenuItems);
#endif
                    UpdatePad(gameTime, inputManager, playerMenuItems);

                }
            }

            base.Update(gameTime, inputManager);
        }

        /// <summary>
        /// Close this menu.
        /// </summary>
        public override void Deactivate()
        {
            FrameworkCore.PlayCue(sounds.Music.none);
            FrameworkCore.PlayCue(sounds.click.whoosh);
            base.Deactivate();
        }

        /// <summary>
        /// Activate this menu.
        /// </summary>
        public override void Activate()
        {
            //kill music.
            FrameworkCore.PlayCue(sounds.Music.jazz);

            FrameworkCore.PlayCue(sounds.click.whoosh);

            base.Activate();
        }

        int WINDOWWIDTH = 950;
        int WINDOWHEIGHT = 550;

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            Vector2 tradeModifier = Vector2.Zero;
            Vector2 playerModifier = Vector2.Zero;

            //draw the big squares.
            if (tradeState >= TradeState.selectTraderItem)
            {
                float tradeTransition = 0;

                if (tradeState <= TradeState.selectTraderItem)
                    tradeTransition = stateTransitions[(int)TradeState.selectTraderItem];
                else if (tradeState <= TradeState.fanfare)
                    tradeTransition = 1;

                Rectangle topRect = new Rectangle(
                    (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - WINDOWWIDTH / 2,
                    (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - WINDOWHEIGHT / 2 - 20,
                    WINDOWWIDTH,
                    WINDOWHEIGHT / 2 + 10);

                int modifier = (int)Helpers.PopLerp(tradeTransition, -300, 40, 0);
                tradeModifier = new Vector2(0, modifier);
                topRect.Y += modifier;

                Color pinkBox = pinkColor;
                pinkBox.A = (byte)MathHelper.Lerp(0, 96, tradeTransition);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, topRect, sprite.inventoryBox, pinkBox);


                //draw the title.
                Color txtColor = pinkColor;
                txtColor = Color.Lerp(Helpers.transColor(pinkColor), pinkColor, Transition);
                Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), Transition);

                Vector2 menuPos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - WINDOWHEIGHT / 2);
                menuPos.Y += 32;
                menuPos += tradeModifier;
                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Gothic, Resource.TradingTitle, menuPos, txtColor, darkColor, 0.7f, 0);



                //draw teh shadebox when selecting playerItem
                if (tradeState == TradeState.selectPlayerItem)
                {
                    float playerTransition = 0;

                    if (tradeState <= TradeState.selectPlayerItem)
                        playerTransition = stateTransitions[(int)TradeState.selectPlayerItem];
                    else if (tradeState <= TradeState.fanfare)
                        playerTransition = 1;

                    Color shadeColor = new Color(0, 0, 0, 192);
                    shadeColor = Color.Lerp(OldXNAColor.TransparentBlack, shadeColor, playerTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, topRect, sprite.blank, shadeColor);
                }
            }

            if (tradeState >= TradeState.selectPlayerItem)
            {
                float playerTransition = 0;

                if (tradeState <= TradeState.selectPlayerItem)
                    playerTransition = stateTransitions[(int)TradeState.selectPlayerItem];
                else if (tradeState <= TradeState.fanfare)
                    playerTransition = 1;

                Rectangle bottomRect = new Rectangle(
                    (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - WINDOWWIDTH / 2,
                    (int)FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2,
                    WINDOWWIDTH,
                    WINDOWHEIGHT / 2);

                int modifier = (int)Helpers.PopLerp(playerTransition, 200, -40, 0);
                playerModifier = new Vector2(0, modifier);
                bottomRect.Y += modifier;

                Color blueBox = Faction.Blue.teamColor;
                blueBox.A = (byte)MathHelper.Lerp(0, 128, playerTransition);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, bottomRect, sprite.inventoryBox, blueBox);


                if (tradeState == TradeState.selectPlayerItem)
                {
                    //int playerTxt = (int)(FrameworkCore.SerifBig.MeasureString(Resource.TraderPlayerItem).X);
                    Vector2 playerTxtPos = new Vector2(bottomRect.X, bottomRect.Y);
                    playerTxtPos.Y += 24;
                    playerTxtPos.Y += Helpers.Pulse(gameTime, 4, 5);

                    playerTxtPos.X += 16;
                    

                    float playerTxtSize = Helpers.PopLerp(playerTransition, 4, 0.7f, 0.85f);

                    Helpers.DrawOutline(FrameworkCore.SerifBig, Resource.TraderPlayerItem, playerTxtPos,
                        pinkColor, new Color(0, 0, 0, 128), -0.05f, Vector2.Zero, playerTxtSize);
                }
            }





            if (tradeState >= TradeState.selectTraderItem)
            {
                DrawTraderItems(gameTime, traderMenuItems, tradeModifier);

                if (tradeState >= TradeState.selectPlayerItem && tradeState <= TradeState.fanfare)
                {
                    DrawTraderItems(gameTime, playerMenuItems, playerModifier);
                }
            }

            DrawFlamingo(gameTime);


#if WINDOWS
            DrawConfirmButtons(gameTime);
#else
            DrawLegends();
#endif

        }

        private void DrawLegends()
        {
            if (tradeState > TradeState.selectPlayerItem)
                return;

            string exitString = string.Empty;

            if (tradeState == TradeState.selectTraderItem)
                exitString = Resource.TraderLeave;
            else
                exitString = Resource.MenuCancel;

            if (HasSufficientCargo)
            {
                float y = Helpers.DrawLegend(exitString, sprite.buttons.b, Transition);

                float x = Helpers.DrawLegendAt(Resource.MenuSelect, sprite.buttons.a, Transition, y - 32);

                Helpers.DrawLegendAt(Resource.TradingNextPrev, sprite.buttons.leftRight, Transition, x - 32);
            }
            else
            {
                float y = Helpers.DrawLegend(exitString, sprite.buttons.b, Transition);

                Helpers.DrawLegendAt(Resource.TradingNextPrev, sprite.buttons.leftRight, Transition, y - 32);
            }
        }

        private void DrawFlamingo(GameTime gameTime)
        {
            float flamingoTransition = 0;
            flamingoTransition = Math.Max(stateTransitions[(int)TradeState.intro], stateTransitions[(int)TradeState.selectTraderItem]);

            if (flamingoTransition <= 0)
                return;

            int flamingoHeight = (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Height * 0.7111f);
            int flamingoWidth = (int)(flamingoHeight * 0.5859f);

            Rectangle flamingoRect = new Rectangle(
                (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width - flamingoWidth),
                (int)(FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - flamingoHeight),
                flamingoWidth,
                flamingoHeight);

            flamingoRect.X += (int)MathHelper.Lerp(flamingoWidth, 0, flamingoTransition);

            flamingoRect.Y += 6;
            float modifier = Helpers.Pulse(gameTime, 6, 3);
            flamingoRect.Y += (int)modifier;

#if WINDOWS
            flamingoRect.X = (int)Math.Max(flamingoRect.X,
                FrameworkCore.players[0].inputmanager.mousePos.X + 160);
#endif

            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, flamingoRect, sprite.flamingo, Color.White,
                0, Vector2.Zero, SpriteEffects.None, 0);



            if (flamingoTransition >= 1)
            {
                string flamingoTxt = string.Empty;

                if (HasSufficientCargo)
                    flamingoTxt = Resource.TradingTraderItem;
                else
                    flamingoTxt = Resource.TradingNoCargo;

                float textSize = 0.75f;
                int textWidth = (int)(FrameworkCore.SerifBig.MeasureString(flamingoTxt).X * textSize);
                Vector2 textpos = new Vector2(flamingoRect.X - textWidth, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);
                textpos.Y += 32;
                textpos.Y += modifier;

                Color flamColor = Color.Lerp(Helpers.transColor(pinkColor), pinkColor, flamingoTransition);
                flamColor = Color.Lerp(Helpers.transColor(flamColor), flamColor, Transition);

                Helpers.DrawOutline(FrameworkCore.SerifBig, flamingoTxt,
                    textpos, flamColor, new Color(0, 0, 0, 128), -0.05f, Vector2.Zero, textSize);
            }
        }


        private void DrawTraderItems(GameTime gameTime, List<InventoryMenuItem> items, Vector2 modifier)
        {
            if (items == null)
                return;

            Color whiteColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), Transition);

            int SELECTOFFSET = 70;

            for (int i = 0; i < items.Count; i++)
            {
                bool drawTag = false;
                int index = items.IndexOf(items[i]);

                if (items == traderMenuItems && index == selectedTraderItem)
                    drawTag = true;
                else if (items == playerMenuItems &&
                    (index == selectedPlayerItem1 || index == selectedPlayerItem2))
                    drawTag = true;

                Vector2 itemPos = items[i].position;
                itemPos += modifier;

                if (drawTag)
                {
                    float smoothTrans = MathHelper.SmoothStep(0,1,fanfareTransition);

                    if (items == traderMenuItems)
                    {
                        itemPos.Y += SELECTOFFSET;

                        if (tradeState < TradeState.fanfare)
                            itemPos.Y += Helpers.Pulse(gameTime, 4, 4);
                        else
                        {
                            Vector2 destination = playerMenuItems[selectedPlayerItem1].position;
                            itemPos = Vector2.Lerp(itemPos, destination, smoothTrans);
                        }
                    }
                    else if (items == playerMenuItems)
                    {
                        itemPos.Y -= SELECTOFFSET;

                        if (tradeState < TradeState.fanfare)
                            itemPos.Y -= Helpers.Pulse(gameTime, 4, 5);
                        else
                        {
                            Vector2 destination = Vector2.Zero;

                            if (selectedPlayerItem1 == index)
                            {
                                destination = traderMenuItems[selectedTraderItem].position;
                            }
                            else
                            {
                                destination = traderMenuItems[traderMenuItems.Count - 1].position + new Vector2(ITEMWIDTH, 0);
                            }
                            itemPos = Vector2.Lerp(itemPos, destination, smoothTrans);
                            
                        }
                    }                    
                }

                if (drawTag)
                {
                    //do sparkle.
                    Color sparkleColor = new Color(255, 160, 0, 128);
                    float sparkleSize = 1.1f + Helpers.Pulse(gameTime, 0.2f, 6);

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos,
                        sprite.sparkle, sparkleColor,
                        (float)gameTime.TotalGameTime.TotalSeconds * 0.6f,
                        Helpers.SpriteCenter(sprite.sparkle), sparkleSize, SpriteEffects.None, 0);

                    sparkleSize = 1.1f - Helpers.Pulse(gameTime, 0.2f, 6);                    

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos,
                        sprite.sparkle, sparkleColor,
                        (float)gameTime.TotalGameTime.TotalSeconds * -0.3f,
                        Helpers.SpriteCenter(sprite.sparkle), sparkleSize, SpriteEffects.None, 0);
                }

                Color boxColor = Helpers.ITEMCOLOR;
                Color darkboxColor = Color.Lerp(Color.Black, boxColor, 0.6f);

                if (!drawTag)
                    boxColor = Color.Lerp(darkboxColor, boxColor, items[i].activateTransition);

                boxColor = Color.Lerp(Helpers.transColor(boxColor), boxColor, Transition);
                float boxSize = MathHelper.Lerp(0.85f, 1.0f, items[i].activateTransition);

                float boxAngle = 0;

                if (items[i].activateTransition <= 0)
                    boxAngle = Helpers.Pulse(gameTime, 0.05f, 6);

                //draw the box
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos,
                    sprite.roundSquare, boxColor,
                    boxAngle, Helpers.SpriteCenter(sprite.roundSquare), boxSize, SpriteEffects.None, 0);

                if (items[i].inventoryItem == null)
                    continue;



                InventoryItem item = items[i].inventoryItem;

                //draw the icon
                float iconSize = MathHelper.Lerp(0.8f, 0.95f, items[i].activateTransition);
                float iconAngle = MathHelper.Lerp(-0.1f, 0, items[i].activateTransition) + boxAngle;
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos,
                    item.image, whiteColor,
                    iconAngle, Helpers.SpriteCenter(item.image), iconSize, SpriteEffects.None, 0);


                





                Color textColor1 = Color.Orange;
                textColor1 = Color.Lerp(Helpers.transColor(textColor1), textColor1, items[i].activateTransition);
                
                Color textColor2 = new Color(192, 192, 192);
                textColor2 = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, items[i].activateTransition);
                Color darktextColor = new Color(0, 0, 0, 128);
                darktextColor = Color.Lerp(OldXNAColor.TransparentBlack, darktextColor, items[i].activateTransition);
                Vector2 textPos = items[i].position;

                textPos.Y += MathHelper.Lerp(-20, 0, items[i].activateTransition);

                textPos.Y += 56;

                if (items == traderMenuItems && drawTag && tradeState <= TradeState.selectPlayerItem)
                {
                    textColor1 = OldXNAColor.TransparentBlack;
                    textColor2 = Color.Gray;
                    textPos.Y += SELECTOFFSET - 15;
                }

                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif, item.name, textPos,
                    textColor1, darktextColor, 1, 0);

                textPos.Y += LINESIZE+2;
                Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Serif, item.description, textPos,
                    textColor2, darktextColor, 1, 0);


                


                //draw the selectorbox
                if (items[i].activateTransition > 0)
                {
                    float selectSize = Helpers.PopLerp(items[i].activateTransition, 0.1f, 1.6f, 1.2f);
                    selectSize = Math.Max(0, selectSize + Helpers.Pulse(gameTime, 0.07f, 6));
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos,
                        sprite.roundSquareSelector, Color.Orange,
                        0, Helpers.SpriteCenter(sprite.roundSquareSelector), selectSize, SpriteEffects.None, 0);
                }


                //draw the "tag mark"               

                if (drawTag)
                {
                    Color tagColor = Color.Goldenrod;
                    tagColor = Color.Lerp(Helpers.transColor(tagColor), tagColor, Transition);
                    Vector2 tagPos = itemPos;
                    tagPos += Vector2.Lerp(new Vector2(20, 20), new Vector2(24, 24), items[i].activateTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, tagPos,
                       sprite.checkmark, tagColor,
                       0, Helpers.SpriteCenter(sprite.checkmark), 1, SpriteEffects.None, 0);
                }
            }
        }

        private void DrawConfirmButtons(GameTime gameTime)
        {
            if (tradeState > TradeState.selectPlayerItem)
                return;

            for (int i = 0; i < confirmButtons.Length; i++)
            {
                Vector2 buttonPos = confirmButtons[i];
                buttonPos.Y += (int)Helpers.PopLerp(Transition, 200, -30, 0);

                Rectangle descriptionRect = new Rectangle(
                    (int)(buttonPos.X - 150),
                    (int)(buttonPos.Y - 30),
                    300,
                    60);

                //draw the box.                                
                Color buttonColor = Color.Lerp(Color.Black, Faction.Blue.teamColor, 0.5f);
                Color textColor = Color.White;

                if (hoverConfirmButton == i)
                {
                    buttonColor = Faction.Blue.teamColor;
                    textColor = Color.Orange;

                    Rectangle glowRect = descriptionRect;
                    glowRect.Inflate(6, 6);

                    Color glowColor = Color.Lerp(Color.Goldenrod, Color.White, 0.5f + Helpers.Pulse(gameTime, 0.49f, 12));

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, glowRect,
                                        sprite.vistaBox, glowColor);

                }

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, descriptionRect,
                    sprite.vistaBox, buttonColor);



                //draw the text.
                string buttText = string.Empty;
                if (tradeState == TradeState.selectTraderItem)
                    buttText = Resource.TraderLeave;
                else
                    buttText = Resource.MenuCancel;

                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, buttText,
                    buttonPos, textColor, 1);
            }
        }
     }
}
