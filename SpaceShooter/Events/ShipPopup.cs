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
    public class ShipPopup : GamePopup
    {
        public FleetShip fleetShip;

        public ShipPopup(SysMenuManager owner)
            : base(owner)
        {
            screenPos = new Vector2(
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2 - 256,
                FrameworkCore.Graphics.GraphicsDevice.Viewport.Height / 2 - 128);

            transitionOnTime = 500;
            transitionOffTime = 300;

            darkenScreen = true;

            MenuItem item = new MenuItem("");
            item.Selected += OnClose;
            base.AddItem(item);

            canBeExited = false;
        }

        //dummy button so player can press A to exit the popup.
        private void OnClose(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        public override void Update(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            if (Transition >= 1)
            {
                if (inputManager.mouseLeftClick || inputManager.kbSpace)
                {
                    Deactivate();
                }
            }
#endif

            base.Update(gameTime, inputManager);
        }

        public override void Activate()
        {
            //kill music.
            FrameworkCore.PlayCue(sounds.Music.none);

            base.Activate();
        }

        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            if (fleetShip == null)
                return;

            Color boxColor = Color.Lerp(OldXNAColor.TransparentWhite, Color.White, Transition);
            Color backColor = Color.Lerp(OldXNAColor.TransparentBlack, Color.Black, Transition);

            //render the ship mesh.
            Color teamColor = FrameworkCore.players[0].ShipColor;
            Vector3 shipPos = FrameworkCore.players[0].lockCamera.CameraPosition;
            shipPos.Z -= 48;
            shipPos.Y += 6.4f;
            Matrix worldMatrix = Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds * 0.5f);
            worldMatrix.Translation = shipPos;

            worldMatrix.Translation -= new Vector3(0,0,fleetShip.shipData.displayDistanceModifier);

            FrameworkCore.PlayerMeshRenderer.Draw(fleetShip.shipData.modelname, worldMatrix,
                FrameworkCore.players[0].lockCamera, teamColor,
                MathHelper.Lerp(0,1,Transition));

            Helpers.DrawTurrets(FrameworkCore.PlayerMeshRenderer, fleetShip.shipData, worldMatrix,
                teamColor, Transition);



            //where the ship is, in 2d space.
            Vector2 shipScreenPos = Helpers.GetScreenPos(FrameworkCore.players[0].lockCamera, shipPos);


            //where to draw the rectangle.
            Vector2 drawPos = Vector2.Zero;
            drawPos.X = FrameworkCore.Graphics.GraphicsDevice.Viewport.Width / 2;
            drawPos.Y = Math.Max(shipScreenPos.Y, FrameworkCore.Graphics.GraphicsDevice.Viewport.Height/2 - 128);
            

            Rectangle itemRect = new Rectangle(
                (int)drawPos.X - 256,
                (int)drawPos.Y,
                512,
                256);

            itemRect.Y += (int)Helpers.PopLerp(Transition, 400, -50, 0);

            //draw the border rectangle.
            Rectangle backRectangle = itemRect;
            backRectangle.Inflate(4, 4);
            DrawRawRectangle(backRectangle, backColor);

            //draw the white rectangle.
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, itemRect, sprite.inventoryBox, boxColor);



            //draw glow.            
            Color glowColor = Color.Lerp(new Color(255, 128, 0, 255), new Color(255, 128, 0, 128),
                0.5f + Helpers.Pulse(gameTime, 0.49f, 5));
            glowColor = Color.Lerp(Helpers.transColor(glowColor), glowColor, Transition);
            float glowAngle = (float)gameTime.TotalGameTime.TotalSeconds * 0.5f;
            float glowSize = MathHelper.Lerp(2.5f, 4, 0.5f + Helpers.Pulse(gameTime, 0.49f, 4));
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, shipScreenPos, sprite.sparkle, glowColor,
                glowAngle, Helpers.SpriteCenter(sprite.sparkle), glowSize, SpriteEffects.None, 0);

            glowSize = MathHelper.Lerp(3, 1.5f, 0.5f + Helpers.Pulse(gameTime, 0.49f, 4));
            glowAngle = (float)gameTime.TotalGameTime.TotalSeconds * -0.2f;            
            FrameworkCore.SpriteBatch.Draw(FrameworkCore.hudSheet, shipScreenPos, sprite.sparkle, glowColor,
                glowAngle, Helpers.SpriteCenter(sprite.sparkle), glowSize, SpriteEffects.None, 0);




            //draw the fanfare text.
            Vector2 fanfarePos = new Vector2(
                shipScreenPos.X,
                shipScreenPos.Y - 100);
            fanfarePos.Y += Helpers.PopLerp(Transition, -100, 30, 0);
            float fanfareAngle = MathHelper.Lerp(-0.2f, -0.08f, Transition);
            fanfarePos.Y += Helpers.Pulse(gameTime, 8, 3);
            FrameworkCore.SpriteBatch.DrawString(FrameworkCore.Gothic, Resource.GameNewShip, fanfarePos, boxColor,
                fanfareAngle, Helpers.stringCenter(FrameworkCore.Gothic, Resource.GameNewShip), 0.7f, SpriteEffects.None, 0);





           
            //draw the item name.
            Vector2 textPos = new Vector2(itemRect.X + 256, itemRect.Y + 130);
            Vector2 titleSize = Vector2.Zero;
            if (fleetShip.captainName != null)
            {
                titleSize = Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, fleetShip.captainName,
                    textPos, backColor, 1.2f, 0);
            }

            textPos.Y += titleSize.Y;
            textPos.Y += 16;

            
            //draw the description.
            if (fleetShip.shipData.name != null)
            {
                Color descColor = new Color(80, 80, 60);
                descColor = Color.Lerp(Helpers.transColor(descColor), descColor, Transition);
                Helpers.stringCenter(FrameworkCore.SpriteBatch, FrameworkCore.Serif, fleetShip.shipData.name,
                    textPos, descColor, 1);
            }
            
#if WINDOWS
            Helpers.DrawClickMessage(gameTime, Transition);
#else
            Helpers.DrawLegend(Resource.MenuOK, sprite.buttons.a, Transition);
#endif
        }
    }
}
