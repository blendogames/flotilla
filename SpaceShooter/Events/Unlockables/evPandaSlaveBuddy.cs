
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{

    public class evPandaSlaveBuddy : Event
    {
        Rectangle img = sprite.eventSprites.panda;

        /// <summary>
        /// Players kills slavers, panda joins.
        /// </summary>
        public evPandaSlaveBuddy()
        {
            keys = 1;
            musicCue = sounds.Music.funky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evPandaSlaveBuddy0);
            popup.eventName = eResource.namePandaSlave;


            popup.AddItem(eResource.evHippoBuddy0Welcome, OnGetShip);

            base.Activate();
        }

        void OnGetShip(object sender, InputArgs e)
        {
            eventManager.UnlockEvent(new evPandaFight());

            eventManager.kPandaOnboard = true;

            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(new ShipData[] { shipTypes.BeamGunship, shipTypes.Dreadnought });

            eventManager.AddLog(img, eResource.logPandaSlaveBuddy);
        }
    }
}