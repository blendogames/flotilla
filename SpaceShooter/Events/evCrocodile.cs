
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// time travelling crocs
    /// </summary>
    public class evCrocodile : Event
    {
        Rectangle img = sprite.eventSprites.crocodile;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evCrocodile()
        {
            //shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.BeamFrigate };
            shipList = new ShipData[1] { shipTypes.BeamGunship };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Yellow;
            musicCue = sounds.Music.dhol;
            pilotName = eResource.nameCrocodile;
        }

        public override void Activate()
        {
            if (!eventManager.kCrisiumOnBoard)
            {
                //croc is confused and attacks you.
                EventPopup popup = base.CreatePopup(this.manager, img, eResource.evCrocodile0);
                popup.eventName = pilotName;
                popup.AddItem(eResource.startBattle, OnFight);
            }
            else
            {
                //your scientists are onboard.
                EventPopup popup = base.CreatePopup(this.manager, img, eResource.evCrocodile0Friend);
                popup.eventName = pilotName;
                popup.AddItem(eResource.evHippoBuddy0Welcome, OnJoin);
            }

            base.Activate();
        }

        private void OnJoin(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            eventManager.AddShip(shipTypes.BeamGunship);

            eventManager.AddLog(img, eResource.logCrocodileFriend);
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logCrocodile);

            eventManager.AddKey(typeof(evOwl));
        }
    }
}