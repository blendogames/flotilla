
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evOwlBribe  : Event
    {
        Rectangle img = sprite.eventSprites.owl;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evOwlBribe()
        {
            shipList = new ShipData[2] { shipTypes.BeamGunship, shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Owl;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameOwl3;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager, img, eResource.evOwlBribe0);
            popup.eventName = pilotName;

            popup.AddItem(eResource.startBattle, OnFight);

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logOwlBribe);
        }
    }
}