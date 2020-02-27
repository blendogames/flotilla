
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evToucanRevenge : Event
    {
        Rectangle img = sprite.eventSprites.toucans;

        /// <summary>
        /// The toucans want their revenge.
        /// </summary>
        public evToucanRevenge()
        {
            shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Toucans;
            musicCue = sounds.Music.funky;
            pilotName = eResource.nameToucan;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evToucanRevenge0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, StartBattle);

            base.Activate();
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logToucanRevenge);
        }
    }
}