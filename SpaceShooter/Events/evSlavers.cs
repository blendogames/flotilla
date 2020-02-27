
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evSlavers: Event
    {
        Rectangle img = sprite.eventSprites.fish;

        /// <summary>
        /// Slave Ring
        /// </summary>
        public evSlavers()
        {
            shipList = new ShipData[2] { shipTypes.Battleship, shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Slavers;
            musicCue = sounds.Music.bird;
            pilotName = eResource.nameSlaver;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evSlavers0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.evSlavers0Accept, OnBribe);
            popup.AddItem(eResource.evSlavers0Fight, OnFight);

            base.Activate();
        }

        private void OnBribe(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo();

            eventManager.AddLog(img, eResource.logSlaversBribe);

            eventManager.UnlockEvent(new evOwlBribe());
        }

        private void OnFight(object sender, InputArgs e)
        {

            eventManager.AddKey(typeof(evPandaSlaveBuddy));

            eventManager.UnlockEvent(new evSlaverRevenge());

            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logSlaversFight);

            eventManager.AddKey(typeof(evOwl));
        }

    }
}