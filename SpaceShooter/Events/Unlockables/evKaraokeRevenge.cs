
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{

    public class evKaraokeRevenge : Event
    {
        Rectangle img = sprite.eventSprites.owl2;

        /// <summary>
        /// The deer know you have a bruja-tainted ship.
        /// </summary>
        public evKaraokeRevenge()
        {
            shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.Dreadnought };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Owl;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameOwl2;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evKaraokeRevenge0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, StartBattle);

            base.Activate();
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logKaraokeRevenge);

        }
    }
}