
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evKoala : Event
    {
        Rectangle img = sprite.eventSprites.koala;

        /// <summary>
        /// Rhino attacks you for ripping off casino.
        /// </summary>
        public evKoala()
        {
            shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.None;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameKoala;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evKoala0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, StartBattle);

            base.Activate();
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logKoala);
        }
    }
}