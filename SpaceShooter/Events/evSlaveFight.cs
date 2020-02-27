
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evSlaveFight : Event
    {
        Rectangle img = sprite.eventSprites.fish;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evSlaveFight()
        {
            shipList = new ShipData[] { shipTypes.BeamFrigate, shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Slavers;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameSlaver;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager);
            popup.image = img;
            popup.description = eResource.evSlaveFight0;
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, OnFight);

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddKey(typeof(evPandaSlaveBuddy));

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logSlaveFight);

            eventManager.AddKey(typeof(evOwl));
        }
    }
}