
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// chicken space pirates
    /// </summary>
    public class evPirates : Event
    {
        Rectangle img = sprite.eventSprites.chickens;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evPirates()
        {
            //shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.BeamFrigate };
            shipList = new ShipData[2] { shipTypes.BeamFrigate, shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Chickens;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.namePirates;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager, img, eResource.evPirates0 );
            popup.eventName = pilotName;



            popup.AddItem(eResource.startBattle, OnFight);

            eventManager.AddKey(typeof(evHippoPirateBuddy));

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logPirates);

            eventManager.AddKey(typeof(evOwl));
        }
    }
}