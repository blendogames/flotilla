
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evPandaFight : Event
    {
        Rectangle img = sprite.eventSprites.panda;

        /// <summary>
        /// The deer know you have a bruja-tainted ship.
        /// </summary>
        public evPandaFight()
        {
            shipList = new ShipData[2] { shipTypes.Battleship, shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Slavers;
            musicCue = sounds.Music.funky;
            pilotName = eResource.namePandaSlave;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evPandaFight0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, StartBattle);

            base.Activate();
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logPandaFight);

        }
    }
}