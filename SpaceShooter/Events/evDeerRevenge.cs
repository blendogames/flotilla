
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evDeerRevenge : Event
    {
        Rectangle img = sprite.eventSprites.deer;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evDeerRevenge()
        {
            shipList = new ShipData[2] { shipTypes.Battleship, shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Deer;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameDeer2;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evDeerRevenge0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, StartBattle);

            eventManager.UnlockEvent(new evHippoBuddy());

            base.Activate();
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logDeerRevenge);
        }
    }
}