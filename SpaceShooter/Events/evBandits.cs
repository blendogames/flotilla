
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evBandits : Event
    {
        Rectangle img = sprite.eventSprites.hippo;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evBandits()
        {
            //shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.BeamFrigate };
            shipList = new ShipData[1] { shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Red;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameBandit;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager);
            popup.image = img;
            popup.description = eResource.evBandits0;
            popup.eventName = pilotName;


            popup.AddItem(eResource.startBattle, OnFight);

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logBandit);

            eventManager.AddKey(typeof(evOwl));
        }
    }
}