
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evSlaverRevenge : Event
    {
        Rectangle img = sprite.eventSprites.fish;

        /// <summary>
        /// Rhino attacks you for ripping off casino.
        /// </summary>
        public evSlaverRevenge()
        {
            shipList = new ShipData[2] { shipTypes.Dreadnought, shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Slavers;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameSlaver;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evSlaverRevenge0);
            popup.eventName = pilotName;



            popup.AddItem(eResource.startBattle, StartBattle);

            base.Activate();
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logSlaverRevenge);
        }
    }
}