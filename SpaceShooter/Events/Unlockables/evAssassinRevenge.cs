
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evAssassinRevenge : Event
    {
        Rectangle img = sprite.eventSprites.assassin;

        /// <summary>
        /// Assassin returns
        /// </summary>
        public evAssassinRevenge()
        {
            keys = 2;
            shipList = new ShipData[1] { shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Assassins;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameAssassin;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, string.Format(eResource.evAssassinRevenge0, Helpers.GetPlayerName().ToUpper()));
            popup.eventName = pilotName;

            popup.AddItem(eResource.evAssassinRevenge0GiveToucans, OnGiveToucans);
            popup.AddItem(eResource.evAssassinRevenge0DenyToucans, OnFight);

            

            base.Activate();
        }

        public void OnGiveToucans(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evAssassinRevenge1GiveToucans);

            popup.AddItem(Resource.MenuOK, OnGiveToucansDone);
        }

        public void OnGiveToucansDone(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo();

            eventManager.AddLog(img, eResource.logAssassinRevengeGive);
        }


        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.UnlockEvent(new evToucanTreasure());

            eventManager.AddLog(img, eResource.logAssassinRevengeFight);
        }
    }
}