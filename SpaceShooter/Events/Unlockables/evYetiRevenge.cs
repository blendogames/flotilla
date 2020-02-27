
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evYetiRevenge : Event
    {
        Rectangle img = sprite.eventSprites.angryyetis;

        /// <summary>
        /// Rhino attacks you for ripping off casino.
        /// </summary>
        public evYetiRevenge()
        {
            shipList = new ShipData[2] { shipTypes.Dreadnought, shipTypes.Destroyer };
            shipMinMax = new Point(5, 8);
            faction = Faction.Yetis;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameNavOfficer;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evYetiRevenge0);
            popup.eventName = pilotName;

            popup.AddItem(eResource.startBattle, StartBattle);

            base.Activate();
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logYetiRevenge);
        }
    }
}