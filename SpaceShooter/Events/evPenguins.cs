
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evPenguins : Event
    {
        Rectangle img = sprite.eventSprites.penguin;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evPenguins()
        {
            shipList = new ShipData[] { shipTypes.Destroyer, shipTypes.Gunship };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Penguin;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.namePenguins;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager, img, eResource.evPenguins0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, OnFight);

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logPenguins);
        }
    }
}