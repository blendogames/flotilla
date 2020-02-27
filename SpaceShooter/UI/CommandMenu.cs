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
    class CommandMenu : Menu
    {
        public CommandMenu(Game game, PlayerCommander owner)
            : base(game, owner)
        {
        }

        private void UpdateItemPositions()
        {
            Vector2 screenViewport = owner.viewportSize;


            
            
            float gapSize = (sprite.tab.Width) + 24;
            Vector2 startPos = Vector2.Zero;
            startPos.Y = screenViewport.Y / 2;            
            startPos.X = screenViewport.X / 2;
            startPos.X -= gapSize * ((float)menuItems.Count / 2f);
            startPos.X += gapSize / 2;

            foreach (MenuItem item in menuItems)
            {
                item.position = startPos;
                item.position.X += gapSize * menuItems.IndexOf(item);
            }
        }

        public void UpdateMouseButton(GameTime gameTime, InputManager inputManager)
        {

#if WINDOWS
            if (Transition <= 0)
            {
                if (commandButtonRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                {
                    if (inputManager.mouseLeftClick)
                    {
                        //open command menu.
                        owner.ActivateCommandMenu();
                    }

                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(100).TotalMilliseconds);

                    commandRectTransition = MathHelper.Clamp(commandRectTransition + delta, 0, 1);
                }
                else
                {
                    float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                        TimeSpan.FromMilliseconds(300).TotalMilliseconds);

                    commandRectTransition = MathHelper.Clamp(commandRectTransition - delta, 0, 1);                    
                }
            }
#endif
        }

        private float glowTransition = 0;

        private void SelectAndRun(int index, InputManager inputManager)
        {
            try
            {
                selectedItem = menuItems[index];
                base.ActivateItem(inputManager);
            }
            catch
            {
            }
        }

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            if (Transition >= 1)
            {
                foreach (MenuItem item in menuItems)
                {
                    Rectangle itemRect = new Rectangle(
                        (int)item.position.X - 35,
                        (int)item.position.Y - 80,
                        70,
                        160);

                    if (itemRect.Contains((int)inputManager.mousePos.X, (int)inputManager.mousePos.Y))
                    {
                        if (inputManager.mouseHasMoved)
                            selectedItem = item;

                        if (inputManager.mouseLeftClick)
                            base.ActivateItem(inputManager);
                    }
                }
            }
#endif

            if (Transition >= 1)
            {
                
#if WINDOWS
                if (inputManager.kb1Pressed)
                {
                    SelectAndRun(0, inputManager);
                }
                else if (inputManager.kb2Pressed)
                {
                    SelectAndRun(1, inputManager);
                }
                else if (inputManager.kb3Pressed)
                {
                    SelectAndRun(2, inputManager);
                }
#endif



                if (inputManager.OpenMenu || inputManager.kbBackspaceJustPressed || inputManager.kbSpace)
                {
                    owner.ActiveMenu = null;
                    Deactivate();
                }

                if (inputManager.sysMenuLeft)
                {
                    int index = menuItems.IndexOf(selectedItem);
                    index--;

                    if (index < 0)
                        index = menuItems.Count - 1;

                    selectedItem = menuItems[index];
                    FrameworkCore.PlayCue(sounds.click.select);                    
                }
                else if (inputManager.sysMenuRight)
                {
                    int index = menuItems.IndexOf(selectedItem);
                    index++;

                    if (index > menuItems.Count - 1)
                        index = 0;

                    selectedItem = menuItems[index];

                    FrameworkCore.PlayCue(sounds.click.select);                    
                }

                if (inputManager.buttonBPressed || inputManager.kbBackspaceJustPressed)
                {
                    Deactivate();
                }

                if (inputManager.kbEnter || inputManager.buttonAPressed)
                {
                    if (selectedItem != null)
                    {
                        base.ActivateItem(inputManager);
                    }
                }
            }


            //base.Update(gameTime, inputManager);
        }


        public override void Activate()
        {
            UpdateItemPositions();

            FrameworkCore.PlayCue(sounds.click.whoosh);

            base.Activate();
        }



        public void Draw(GameTime gameTime)
        {
            if ( Transition <= 0)
            {
                float glowDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                TimeSpan.FromMilliseconds(1200).TotalMilliseconds);

                glowTransition = MathHelper.Clamp(glowTransition + glowDelta, 0, 1);

                if (glowTransition >= 1)
                    glowTransition = 0;
            }

            Vector2 screenViewport = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height);

            Rectangle screen = new Rectangle(0, 0, (int)screenViewport.X + 1, (int)screenViewport.Y + 1);
            Color bgColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 128), Transition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, screen, sprite.blank, bgColor);

            Vector2 topRightPosition = new Vector2(screenViewport.X - 150, 100);
            Vector2 finalPosition = menuItems[menuItems.Count - 1].position + new Vector2(sprite.tab.Width,0);
            float smoothTransition = MathHelper.SmoothStep(0, 1, Transition);
            Vector2 rbPosition = Vector2.Lerp(topRightPosition, finalPosition, smoothTransition);
            DrawShoulders(gameTime, rbPosition);

            if (Transition > 0)
            {
                DrawTabs(gameTime, rbPosition);
            }
        }

        Rectangle commandButtonRect = Rectangle.Empty;
        float commandRectTransition=0;

        private void DrawShoulders(GameTime gameTime, Vector2 originPos)
        {
            //draw RB
            Rectangle buttonRect = sprite.buttons.rb;
            Vector2 rbPos = originPos;
            rbPos = Vector2.Lerp(rbPos, rbPos + new Vector2(0, 7), rbTransition);
            rbPos.X += 32;
            

            //draw the "menu" text.
            if (Transition < 1)
            {
                Color fontColor = Color.Lerp(Color.White, Color.Orange, commandRectTransition);
                fontColor = Color.Lerp(fontColor, Helpers.transColor(fontColor), Transition);
                Color bgColor = Color.Lerp(new Color(0, 0, 0, 128), OldXNAColor.TransparentBlack, Transition);
                string textString = Resource.MenuCommandMenu;

#if WINDOWS
                if (owner != null && owner.mouseEnabled)
                {
                    textString += " " + Helpers.GetShortcutAltKey();
                }
#endif

                SpriteFont font = FrameworkCore.Serif;
                Vector2 textSize = font.MeasureString(textString);

                Vector2 textPosition = Vector2.Lerp(originPos, originPos + new Vector2(90, 0), Transition);
                textPosition.X += 24;
                textPosition.X -= buttonRect.Width / 2;
                textPosition.X -= textSize.X;
                textPosition.Y -= textSize.Y / 2;
                //FrameworkCore.SpriteBatch.DrawString(font, textString, textPosition, fontColor);

                Color boxColor = Color.Lerp(Color.Black, owner.TeamColor, 0.7f);
                boxColor = Color.Lerp(boxColor, owner.TeamColor, commandRectTransition);
                boxColor = Color.Lerp(boxColor, Helpers.transColor(boxColor), Transition);

                Rectangle bgRect = new Rectangle(
                    (int)textPosition.X,
                    (int)textPosition.Y,
                    (int)textSize.X,
                    (int)textSize.Y);
                bgRect.Width += sprite.buttons.rb.Width;
                bgRect.Inflate(8, 8);

                commandButtonRect = bgRect;



                //draw the glow if all ships have orders.
                if (owner.TutAllShipsHaveOrders() && Transition <= 0)
                {



                    Rectangle glowRect = bgRect;
                    glowRect.Inflate(
                        (int)MathHelper.Lerp(16, 1, glowTransition),
                        (int)MathHelper.Lerp(24, 1, glowTransition));

                    Color glowRectColor = Color.Lerp(Helpers.transColor(Color.Goldenrod), Color.Goldenrod,
                        glowTransition);

                    
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, glowRect, sprite.vistaBox,
                        glowRectColor);

                    glowRect = bgRect;
                    glowRect.Inflate(
                        (int)MathHelper.Lerp(1, 16, glowTransition),
                        (int)MathHelper.Lerp(1, 24, glowTransition));

                    glowRectColor = Color.Lerp(Color.Gold,Helpers.transColor(Color.Gold),
                                            glowTransition);

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, glowRect, sprite.vistaBox,
                        glowRectColor);


                    
                }



                int inflateAmount = (int)MathHelper.Lerp(0,5,commandRectTransition);
                bgRect.Inflate(inflateAmount, inflateAmount);

#if WINDOWS
                Color glowColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, commandRectTransition);
                glowColor = Color.Lerp(glowColor, OldXNAColor.TransparentWhite, Transition);
                Rectangle glowBox = bgRect;
                glowBox.Inflate(5, 5);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, glowBox, sprite.vistaBox,
                    glowColor);
#endif

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, bgRect, sprite.vistaBox,
                    boxColor);

                

                Helpers.DrawOutline(font, textString, textPosition, fontColor, bgColor);



                if (owner.TutAllShipsHaveOrders() && Transition <= 0)
                {
                    //draw the bouncy message.
                    Vector2 basePos = new Vector2(bgRect.Center.X, bgRect.Center.Y);

                    Vector2 arrowPos = basePos;
                    arrowPos.Y += 32;
                    arrowPos.Y += Helpers.Pulse(gameTime, 7, 8);

                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, arrowPos, sprite.arrow,
                        Color.Goldenrod, -0.1f + 1.57f, Helpers.SpriteCenter(sprite.arrow), 1, SpriteEffects.None, 0);

                    Vector2 txtPos = basePos;
                    txtPos.Y += 70;
                    txtPos.Y += Helpers.Pulse(gameTime, 3, 8);

                    float bounceSize = MathHelper.Lerp(0.57f, 0.6f, 0.5f + Helpers.Pulse(gameTime, 0.49f, 8));

                    Helpers.stringCenterOutline(FrameworkCore.SpriteBatch, FrameworkCore.Gothic, Resource.MenuEndTurn,
                        txtPos + new Vector2(1.5f, 1.5f), Color.Goldenrod, new Color(0, 0, 0, 128), bounceSize, -0.1f);

                    
                }
            }

            //draw RB

            
            if (!owner.mouseEnabled && Transition <= 0)
            {
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, rbPos, buttonRect, Color.White, 0,
                    Helpers.SpriteCenter(buttonRect), 1, SpriteEffects.None, 0);

                /*
                //draw LB
                if (Transition > 0)
                {
                    buttonRect = sprite.buttons.lb;
                    Vector2 finalPosition = menuItems[0].position;
                    finalPosition.X -= buttonRect.Width;
                    float smoothTransition = MathHelper.SmoothStep(0, 1, Transition);
                    Vector2 lbPosition = Vector2.Lerp(originPos, finalPosition, smoothTransition);
                    lbPosition.X -= 32;
                    lbPosition.Y = originPos.Y;
                    lbPosition.Y = MathHelper.Lerp(lbPosition.Y, lbPosition.Y + 7, lbTransition);
                    FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, lbPosition, buttonRect, Color.White, 0,
                        Helpers.SpriteCenter(buttonRect), 1, SpriteEffects.None, 0);
                }*/
            }
            


            if (Transition > 0 && !owner.mouseEnabled)
            {
                float x = Helpers.DrawLegend(Resource.MenuOK, sprite.buttons.a, Transition);
                Helpers.DrawLegendAt(Resource.MenuSelect, sprite.buttons.leftRight, Transition, x - 32);
            }



        }

        private void DrawTabs(GameTime gameTime, Vector2 originPos)
        {
            Rectangle tabRect = sprite.tab;

            int itemIndex = 1;

            foreach (MenuItem item in menuItems)
            {
                float smoothTransition = MathHelper.SmoothStep(0, 1, Transition);
                Vector2 itemPos = Vector2.Lerp(originPos, item.position, smoothTransition);
                Color tabColor = Color.Lerp(Helpers.transColor(owner.TeamColor), owner.TeamColor, Transition);
                float tabSize = MathHelper.Lerp(0, 1, Transition);
                itemPos.Y = originPos.Y;

                float smoothItemTransition = MathHelper.SmoothStep(0, 1, item.selectTransition);
                //itemPos.Y += MathHelper.Lerp(0, -16, smoothItemTransition);
                itemPos.Y += Helpers.PopLerp(item.selectTransition, 0, -32, -16);

                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, tabRect, tabColor, 0,
                    Helpers.SpriteCenter(tabRect), tabSize, SpriteEffects.None, 0);

                Color tabInsideColor = Color.Lerp(new Color(255, 255, 255, 48), Color.White, item.selectTransition);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemPos, sprite.tabInside, tabInsideColor, 0,
                    Helpers.SpriteCenter(sprite.tabInside), tabSize, SpriteEffects.None, 0);


                //icon
                float iconSize = Helpers.PopLerp(item.selectTransition, 0.85f, 1.7f, 0.85f);
                iconSize = MathHelper.Lerp(0, iconSize, Transition);
                float iconRotation = MathHelper.Lerp(-0.3f, 0, smoothItemTransition);
                Color iconColor = Color.Lerp(Color.White, Color.Black, smoothItemTransition);
                Vector2 itemIconPos = itemPos;
                if (item.selectTransition >= 1)
                {
                    float pulse = (float)(4 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5));
                    itemIconPos.Y += pulse;
                }
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemIconPos, item.iconRect, iconColor,
                    iconRotation, Helpers.SpriteCenter(item.iconRect), iconSize, SpriteEffects.None, 0);


                //the text above the tab.
                Vector2 textPos = itemPos;
                textPos.Y -= sprite.tab.Height * 0.7f;
                textPos.X -= 10;

                Color fontColor = Color.Lerp(Helpers.transColor(owner.TeamColor), owner.TeamColor, Transition);
                fontColor = Color.Lerp(fontColor, Color.White, item.selectTransition);

                float fontAngle = MathHelper.Lerp(-1.57f, -0.85f, smoothTransition);
                float fontSize = MathHelper.Lerp(1, 1.4f, smoothItemTransition);
                fontSize = MathHelper.Lerp(0, fontSize, Transition);

                string tabText = item.text;

#if WINDOWS
                if (owner != null && owner.mouseEnabled)
                {
                    tabText = "" + itemIndex + ". " + item.text;
                }
#endif


                FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, tabText, textPos, fontColor, fontAngle,
                    Vector2.Zero, fontSize, SpriteEffects.None, 0);

                itemIndex++;
            }
        }

        


    }
}
