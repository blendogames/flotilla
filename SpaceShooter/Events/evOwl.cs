
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evOwl  : Event
    {
        Rectangle img = sprite.eventSprites.owl;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evOwl()
        {
            //shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.BeamFrigate };
            shipList = new ShipData[2] { shipTypes.BeamGunship, shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Owl;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameOwl;

            keys = 1;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager, img, eResource.evOwl0);
            popup.eventName = pilotName;

            popup.AddItem(eResource.startBattle, OnFight);

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logOwl);
        }
    }
}