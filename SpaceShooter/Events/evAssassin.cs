
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evAssassin : Event
    {
        Rectangle img = sprite.eventSprites.assassin;

        /// <summary>
        /// An assassin appears!
        /// </summary>
        public evAssassin()
        {
            shipList = new ShipData[1] { shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Assassins;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameAssassin;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager, img,
                string.Format(eResource.evAssassin0, FrameworkCore.players[0].commanderName));
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, OnFight);

            eventManager.AddKey(typeof(evAssassinRevenge));

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logAssassin);

            eventManager.AddKey(typeof(evOwl));
        }
    }
}