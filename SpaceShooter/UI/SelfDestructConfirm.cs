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
    public class SelfDestructConfirm : SysMenu
    {
        public SelfDestructConfirm()
        {
            darkenScreen = true;
            int xoffset = 200;

            int fontHeight = (int)menuFont.MeasureString("S").Y;

            Vector2 pos = new Vector2(280, 200);


            MenuItem item = new MenuItem(Resource.MenuYes);
            item.Selected += OnYes;
            item.position = pos;
            base.AddItem(item);

            pos.Y += fontHeight;

            item = new MenuItem(Resource.MenuNo);
            item.Selected += OnNo;
            item.position = pos;
            base.AddItem(item);


            base.RepositionItems();
        }


        private void OnNo(object sender, EventArgs e)
        {
            FrameworkCore.PlayCue(sounds.click.activate);
            Deactivate();
        }

        private void OnYes(object sender, EventArgs e)
        {
            FrameworkCore.PlayCue(sounds.click.activate);
            //Deactivate();

            //clear out all menus.
            if (Owner != null)
            {
                Owner.ClearAll();
            }

            //blow up the player.
            //go to aciton phase.
            if (FrameworkCore.level.gamemode != GameMode.Action)
            {
                for (int k = 0; k < FrameworkCore.players.Count; k++)
                {
                    FrameworkCore.players[k].ForceReady();                    
                }
            }

            for (int k = 0; k < FrameworkCore.level.Ships.Count; k++)
            {
                //only check ships.
                if (!Helpers.IsSpaceship(FrameworkCore.level.Ships[k]))
                    continue;

                //sanity check.
                if (((SpaceShip)FrameworkCore.level.Ships[k]).owner == null)
                    continue;

                //only check human controlled ships.
                if (((SpaceShip)FrameworkCore.level.Ships[k]).owner.GetType() != typeof(PlayerCommander))
                    continue;

                ((SpaceShip)FrameworkCore.level.Ships[k]).ForceKill();
            }

            FrameworkCore.worldMap.evManager.AddLog(sprite.eventSprites.bouquet, eResource.logDestruct);
        }


       
        public override void Update(GameTime gameTime, InputManager inputManager)
        {
#if WINDOWS
            if (Transition >= 1)
            {
                if (inputManager.kbNPressed)
                {
                    OnNo(this, null);
                }
                else if (inputManager.kbYPressed)
                {
                    
                    OnYes(this, null);
                }
            }
#endif

            base.Update(gameTime, inputManager);
            base.UpdateMouseItems(gameTime, inputManager);
        }

        public override void Activate()
        {
            FrameworkCore.PlayCue(sounds.Fanfare.klaxon);

            base.Activate();
        }


        public override void Draw(GameTime gameTime)
        {
            base.DrawDarkenScreen();

            float transitionMod = Helpers.PopLerp(Transition, -100, 40, 0);

            Color descColor = new Color(255, 70, 70);
            descColor = Color.Lerp(Helpers.transColor(descColor), descColor, Transition);

            float glowTrans = 0.5f + Helpers.Pulse(gameTime, 0.49f, 4);

            Color titleColor = Color.Lerp(Color.White, new Color(255,70,70),
                glowTrans);

            titleColor = Color.Lerp(Helpers.transColor(titleColor), titleColor, Transition);
            Color darkColor = Color.Lerp(OldXNAColor.TransparentBlack, new Color(0, 0, 0, 64), Transition);


            Vector2 descVec = FrameworkCore.Serif.MeasureString(Resource.MenuSelfDestructDescription);


            Vector2 titleVec = FrameworkCore.Gothic.MeasureString(Resource.MenuPaused);
            Vector2 titlePos = menuItems[0].position + new Vector2(0, -titleVec.Y);
            titlePos.X += transitionMod;
            titlePos.Y -= 32 +descVec.Y;

            float titleSize = MathHelper.Lerp(1, 1.05f, glowTrans);

            Helpers.DrawOutline(FrameworkCore.Gothic, Resource.MenuSelfDestruct, titlePos, titleColor, darkColor,
                0, Vector2.Zero, titleSize);

            titlePos.Y += titleVec.Y;

            Helpers.DrawOutline(FrameworkCore.Serif, Resource.MenuSelfDestructDescription, titlePos, descColor,
                darkColor, 0, Vector2.Zero, 1);

            base.DrawItems(gameTime, transitionMod);
        }
    }
}
