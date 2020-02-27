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
    public class CampaignMenu : SysMenu
    {
        Vector2 LINESIZE;

        public CampaignMenu()
        {
            LINESIZE = FrameworkCore.Serif.MeasureString("Sample");

            darkenScreen = true;

            transitionOnTime = 300;
            transitionOffTime = 300;


            if (FrameworkCore.players.Count > 1)
            {
                Player2Joined = true;
                player2Index = FrameworkCore.players[1].playerindex;
                player2Name = FrameworkCore.players[1].commanderName;
            }

            hardcorePos = new Vector2(
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - 500,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - 200 );
        }

        bool hardcoreHover = false;

        bool Player2Joined = false;
        PlayerIndex player2Index;
        float player2Transition = 0;
        string player2Name = "";

        public override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (Transition > 0.2f && !isLoading)
            {
                if (inputManager.buttonAPressed || inputManager.buttonStartPressed)
                {
                    StartLoad();
                }

                for (int i = 0; i < 4; i++)
                {
                    PlayerIndex index = PlayerIndex.One;

                    if (i == 0)
                        index = PlayerIndex.One;
                    else if (i == 1)
                        index = PlayerIndex.Two;
                    else if (i == 2)
                        index = PlayerIndex.Three;
                    else
                        index = PlayerIndex.Four;

                    if ((FrameworkCore.MenuInputs[i].buttonAPressed) && !Player2Joined)
                    {
                        if (index != FrameworkCore.ControllingPlayer && !Player2Joined)
                        {
                            Player2Joined = true;
                            player2Index = index;
                            player2Name = Helpers.GetPlayerName(player2Index);
                        }
                    }

                    if (Player2Joined && player2Index == index)
                    {
                        if (FrameworkCore.MenuInputs[i].buttonBPressed)
                        {
                            Player2Joined = false;

                            if (FrameworkCore.players.Count > 1)
                                FrameworkCore.players.RemoveAt(1);
                        }

                        if (FrameworkCore.MenuInputs[i].buttonAPressed && player2Transition >= 1)
                        {
                            StartLoad();
                        }
                    }
                }

                //TODO: CHECK FOR CONTROLLER DISCONNECT.
            }

#if WINDOWS
            if (Transition >= 1 && !isLoading)
            {
                Vector2 box1Pos = new Vector2(
                (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - 120,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);

                Rectangle box1Rect = new Rectangle(
                    (int)box1Pos.X - 300,
                    (int)box1Pos.Y - 230,
                    600,
                    460);


                if (box1Rect.Contains((int)FrameworkCore.MenuInputs[0].mousePos.X,
                    (int)FrameworkCore.MenuInputs[0].mousePos.Y))
                {
                    hoverOverCard = true;

                    if (FrameworkCore.MenuInputs[0].mouseLeftClick)
                    {
                        if (!hardcoreHover)
                            StartLoad();
                    }
                }
                else
                    hoverOverCard = false;


                Vector2 cancelVec = FrameworkCore.Serif.MeasureString(Resource.MenuCancel);
                Rectangle cancelRect = new Rectangle(
                    110,
                    FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120,
                    (int)cancelVec.X,
                    (int)cancelVec.Y);

                cancelRect.Inflate(16, 16);

                if (cancelRect.Contains((int)FrameworkCore.MenuInputs[0].mousePos.X,
                    (int)FrameworkCore.MenuInputs[0].mousePos.Y))
                {
                    cancelHover = true;

                    if (inputManager.mouseLeftClick)
                        Deactivate();
                }
                else
                {
                    cancelHover = false;
                }



                Rectangle hardcoreRect = new Rectangle(
                    (int)hardcorePos.X - 45,
                    (int)hardcorePos.Y - 15,
                    170,
                    40);

                hardcoreRect.Inflate(16, 16);

                if (hardcoreRect.Contains((int)FrameworkCore.MenuInputs[0].mousePos.X,
                    (int)FrameworkCore.MenuInputs[0].mousePos.Y))
                {
                    hardcoreHover = true;

                    if (inputManager.mouseLeftClick)
                        ToggleHardcore();
                }
                else
                {
                    hardcoreHover = false;
                }

            }
#endif


#if XBOX
            if (Transition >= 1 && !isLoading)
            {
                if (inputManager.buttonXPressed)
                {
                    ToggleHardcore();
                }
            }
#endif

            float delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                TimeSpan.FromMilliseconds(200).TotalMilliseconds);

            if (Player2Joined)            
                player2Transition = MathHelper.Clamp(player2Transition + delta, 0, 1);            
            else            
                player2Transition = MathHelper.Clamp(player2Transition - delta, 0, 1);

            if (isLoading)
            {
                loadTransition = MathHelper.Clamp(loadTransition + delta, 0, 1);

                if (loadTransition >= 1)
                    StartCampaign();                
            }
        
            if (!isLoading)
                base.Update(gameTime, inputManager);
        }

        private void ToggleHardcore()
        {
            if (FrameworkCore.isTrialMode())
            {
                if (FrameworkCore.isHardcoreMode)
                    FrameworkCore.isHardcoreMode = false;

                FrameworkCore.PlayCue(sounds.click.error);


                hardcoreHover = false;

                SysPopup signPrompt = new SysPopup(Owner, Resource.HardcoreTrial);
                signPrompt.transitionOnTime = 200;
                signPrompt.transitionOffTime = 200;
                signPrompt.darkenScreen = true;
                signPrompt.hideChildren = false;
                signPrompt.sideIconRect = sprite.windowIcon.error;

                MenuItem item = new MenuItem(Resource.MenuOK);
                item.Selected += CloseMenu;
                signPrompt.AddItem(item);

                Owner.AddMenu(signPrompt);





                return;
            }

            if (!FrameworkCore.isHardcoreMode)
                FrameworkCore.PlayCue(sounds.Fanfare.ready);
            else
                FrameworkCore.PlayCue(sounds.click.activate);

            FrameworkCore.isHardcoreMode = !FrameworkCore.isHardcoreMode;
        }



        private void CloseMenu(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }



        bool cancelHover = false;


        float loadTransition = 0;
        bool isLoading = false;

        bool isLoadDone = false;

        public void StartLoad()
        {
            if (FrameworkCore.isTrialMode() && FrameworkCore.isHardcoreMode)
            {
                FrameworkCore.isHardcoreMode = false;
            }

            isLoading = true;
            FrameworkCore.PlayCue(sounds.Music.none);
        }

        private void StartCampaign()
        {
            if (isLoadDone)
                return;

            isLoadDone = true;
            Deactivate();

            if (Player2Joined)
            {
                PlayerCommander player2 = new PlayerCommander(player2Index,
                    Faction.Blue.altColor, Faction.Blue.altColor);

#if WINDOWS
                player2.mouseEnabled = FrameworkCore.options.p2UseMouse;
#endif

                if (FrameworkCore.players.Count <= 1)
                    FrameworkCore.players.Add(player2);

                //assign the name to player2.
                FrameworkCore.players[1].commanderName = player2Name;
                FrameworkCore.players[1].ShipColor = Faction.Blue.altColor;
                FrameworkCore.players[1].TeamColor = Faction.Blue.altColor;

                //give player 2 a ship.
                if (FrameworkCore.players[0].campaignShips[1] != null)
                    FrameworkCore.players[0].campaignShips[1].childShip = true;
            }

            FrameworkCore.level.ClearAll();
            FrameworkCore.worldMap = new WorldMap();
            FrameworkCore.gameState = GameState.WorldMap;
            FrameworkCore.worldMap.EnterMap();
        }


        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();




            Vector2 titlePos = new Vector2(FrameworkCore.Graphics.GraphicsDevice.Viewport.Width/2 - 540,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2 - 300);
            titlePos.X += Helpers.PopLerp(Transition, -500, 200, 0);
            string title = (Player2Joined) ?
                string.Format(Resource.MenuCampaignCoop, FrameworkCore.adventureNumber)
                :
                string.Format(Resource.MenuCampaign, FrameworkCore.adventureNumber);

            Color titleColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, title, titlePos, titleColor);



            string player1Name = Helpers.GetPlayerName(FrameworkCore.ControllingPlayer);

            float card1Angle = 0.4f + Helpers.Pulse(gameTime, 0.05f, 0.6f);
            Vector2 box1Pos = new Vector2(
                (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - 120,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);



            
            string player2ButtonText = Resource.MenuCampaignJoin;
            

            if (Player2Joined)
            {
                player2ButtonText = string.Format(Resource.MenuCampaignStart, FrameworkCore.adventureNumber);
            }

            float card2Angle = 0.9f + Helpers.Pulse(gameTime, 0.07f, 0.5f);
            Vector2 box2Pos = new Vector2(
                (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) + 240,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2);


            card1Angle = MathHelper.Lerp(card1Angle, 0, player2Transition);
            card2Angle = MathHelper.Lerp(card2Angle, 0, player2Transition);

            card1Angle += Helpers.Pulse(gameTime, 0.02f, 0.6f);
            card2Angle += Helpers.Pulse(gameTime, 0.04f, 0.4f);

            box1Pos.X = Helpers.PopLerp(player2Transition,
                box1Pos.X,
                (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - 258 - 100,
                (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) - 258);

            box2Pos.X = Helpers.PopLerp(player2Transition,
                box2Pos.X,
                (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) + 258 + 100,
                (FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2) + 258);

            

            box1Pos.X += Helpers.PopLerp(Transition, -500, 60, 0);
            box2Pos.X += Helpers.PopLerp(Transition, 500, -60, 0);

            string player2Tag = string.Empty;
#if XBOX
            player2Tag = Player2Joined ? "" : Resource.MenuCampaignPlugController;
#else
            player2Tag = Player2Joined ? "" : Resource.MenuCampaignPlugControllerPC;
#endif
            
            DrawCard(gameTime, card2Angle, box2Pos, Resource.MenuCampaignPlayer2+"\n"+player2Tag, (Player2Joined ? player2Name : ""),
                player2ButtonText, player2Transition, true);

            string card1Text = 
#if XBOX
                string.Format(Resource.MenuCampaignStart, FrameworkCore.adventureNumber);
#else
                "CLICK HERE\nTO START ADVENTURE "+ FrameworkCore.adventureNumber;
#endif

            DrawCard(gameTime, card1Angle, box1Pos, Resource.MenuCampaignPlayer1, player1Name,
                card1Text, 1,
#if XBOX
                true
#else
                false
#endif
                );



            Vector2 ringPos = adjustedPos(box1Pos, card1Angle, -180, -40);
            Color k = Faction.Blue.teamColor;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, ringPos, sprite.xboxRing,
                new Color(k.R, k.G, k.B, 64),
                Helpers.GetRingAngle(FrameworkCore.ControllingPlayer) + card1Angle,
                Helpers.SpriteCenter(sprite.xboxRing), 4,
                SpriteEffects.None, 0);

            if (Player2Joined)
            {
                Color x = Faction.Blue.altColor;
                ringPos = adjustedPos(box2Pos, card2Angle, -180, -40);
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, ringPos, sprite.xboxRing,
                    new Color(x.R, x.G, x.B, 64),
                    Helpers.GetRingAngle(player2Index) + card2Angle,
                    Helpers.SpriteCenter(sprite.xboxRing), 4,
                    SpriteEffects.None, 0);
            }


#if WINDOWS

            Color cancelColor = Color.Gray;

            if (cancelHover)
                cancelColor = Color.White;

            Vector2 hoverPos = new Vector2(110, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height - 120);
            hoverPos.Y += Helpers.PopLerp(Transition, 100, -30, 0);
            Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuCancel, hoverPos,
                cancelColor, Color.Black);

#endif



            DrawHardcore(gameTime);



            //fade out when loading.
            int alpha = (int)MathHelper.Lerp(0, 400, loadTransition);
            Helpers.DarkenScreen(alpha);
        }

        Vector2 hardcorePos;

        private void DrawHardcore(GameTime gameTime)
        {
            Vector2 baseHardcorePos = hardcorePos;
            baseHardcorePos.X += Helpers.PopLerp(Transition, -200, 30, 0);

            //draw the hardcore option.
            Color hardcoreColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            if (FrameworkCore.isHardcoreMode)
            {
                hardcoreColor = new Color(255, 70, 70);
                hardcoreColor = Color.Lerp(Helpers.transColor(hardcoreColor), hardcoreColor, Transition);
            }

            if (!hardcoreHover)
            {
                hardcoreColor = Color.Lerp(Color.Black, hardcoreColor, 0.8f);
            }


            Vector2 hardcoreBoxPos = baseHardcorePos;
            hardcoreBoxPos.X -= 25;
            hardcoreBoxPos.Y += 12;
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, hardcoreBoxPos, sprite.checkboxBox, hardcoreColor,
                0, Helpers.SpriteCenter(sprite.checkboxBox), 1, SpriteEffects.None, 0);


#if XBOX
            //draw x button
            float buttonSize = 1 + Helpers.Pulse(gameTime, 0.05f, 8);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet,
                hardcoreBoxPos + new Vector2(-32, 0),
                sprite.buttons.x, Color.White,
                0, Helpers.SpriteCenter(sprite.buttons.x), buttonSize, SpriteEffects.None, 0);
#endif



            if (FrameworkCore.isHardcoreMode)
            {
                FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, hardcoreBoxPos, sprite.icons.skull, hardcoreColor,
                    Helpers.Pulse(gameTime, 0.05f, 6), Helpers.SpriteCenter(sprite.icons.skull), 1, SpriteEffects.None, 0);
            }

            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, Resource.MenuCampaignHardcore,
                baseHardcorePos, hardcoreColor);


            if (hardcoreHover)
            {
                hardcoreColor = Color.Lerp(Color.Black, hardcoreColor, 0.85f);

                string line2String = FrameworkCore.isHardcoreMode ? Resource.MenuCampaignHardcoreGoodLuck : Resource.MenuCampaignHardcoreDescription;
                Vector2 line1Length = FrameworkCore.Serif.MeasureString(Resource.MenuCampaignHardcore);
                //Vector2 line2Length = FrameworkCore.Serif.MeasureString(line2String);

                Vector2 line2Pos = baseHardcorePos;
                line2Pos.Y += line1Length.Y;
                line2Pos.Y += 10;
                line2Pos.X -= 42;

                line2Pos.Y += Helpers.Pulse(gameTime, 4, 5);

                Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0,0,0,128), Transition);

                Helpers.DrawOutline(FrameworkCore.Serif, line2String, line2Pos, hardcoreColor,
                    darkColor, 0, Vector2.Zero, 0.9f);
                
            }

        }



        bool hoverOverCard = false;
        

        private void DrawCard(GameTime gameTime, float angle, Vector2 position,
            string playerText, string playerName, string buttonText, float transition, bool drawAButton)
        {
            //draw the card.
            Color cardColor = Color.Lerp(new Color(160,160,160), Color.White, transition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, position, sprite.inventoryBox, cardColor,
                angle, Helpers.SpriteCenter(sprite.inventoryBox), 1, SpriteEffects.None, 0);

            //draw the stamp.
            Vector2 stampPos = adjustedPos(position, angle, 150, 20);
            Color stampColor = Color.Lerp(new Color(128, 0, 0,0), new Color(128,0,0,128), transition);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, stampPos, sprite.stamp, stampColor,
                            angle + 0.3f, Helpers.SpriteCenter(sprite.stamp), 1, SpriteEffects.None, 0);

            //Player Number.
            Vector2 textPos = adjustedPos(position, angle, -220, 90);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, playerText, textPos, new Color(45, 43, 26), angle,
                Vector2.Zero, 1, SpriteEffects.None, 0);

            //Player Name.
            textPos = adjustedPos(position, angle, -220, 90 - LINESIZE.Y);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.SerifBig, playerName, textPos, Color.Black, angle,
                            Vector2.Zero, 0.9f, SpriteEffects.None, 0);


            //button Text
            Vector2 buttonTextVec = FrameworkCore.Serif.MeasureString(buttonText);
            textPos = adjustedPos(position, angle, 220 - buttonTextVec.X, -90 + buttonTextVec.Y);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Serif, buttonText, textPos, Color.Black, angle,
                            Vector2.Zero, 1.0f, SpriteEffects.None, 0);

            if (!drawAButton)
                return;

            //draw button
            Vector2 buttonPos = adjustedPos(textPos, angle, (-sprite.buttons.a.Width / 2) - 4, -buttonTextVec.Y / 2);
            float buttonSize = 1.2f + Helpers.Pulse(gameTime, 0.1f, 12);
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, buttonPos, sprite.buttons.a, Color.White,
                angle, Helpers.SpriteCenter(sprite.buttons.a), buttonSize, SpriteEffects.None, 0);

            
        }

        private Vector2 adjustedPos(Vector2 origin, float angle, float upDown, float leftRight)
        {
            if (upDown != 0)
            {
                origin.X += (float)(Math.Cos(angle) * upDown);
                origin.Y += (float)(Math.Sin(angle) * upDown);
            }

            if (leftRight != 0)
            {
                origin.X += (float)(Math.Cos(angle - 1.57f) * leftRight);
                origin.Y += (float)(Math.Sin(angle - 1.57f) * leftRight);
            }

            return origin;
        }

     }
}
