
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evCasinoRhino : Event
    {
        Rectangle img = sprite.eventSprites.rhino;

        /// <summary>
        /// Rhino attacks you for ripping off casino.
        /// </summary>
        public evCasinoRhino()
        {
            shipList = new ShipData[2] { shipTypes.Dreadnought, shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Casino;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameRhino;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evCasinoRhino0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, StartBattle);

            base.Activate();
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logCasinoRhino);
        }
    }
}