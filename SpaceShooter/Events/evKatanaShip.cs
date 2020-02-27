
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evKatanaShip: Event
    {
        Rectangle img = sprite.eventSprites.spacehulk;

        /// <summary>
        /// katana fleet.
        /// </summary>
        public evKatanaShip()
        {
            musicCue = sounds.Music.spooky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evKatanaShip0);
            
            popup.AddItem(eResource.evKatanaShip0Leave, OnDestroy);
            popup.AddItem(eResource.evKatanaShip0Salvage, OnSalvage);

            base.Activate();
        }

        private void OnDestroy(object sender, InputArgs e)
        {
            eventManager.AddKey(typeof(evAbandonZombies));
            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logKatanaShipDestroy);

            Helpers.EventRumble();
        }

        private void OnSalvage(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(new ShipData[]{shipTypes.Dreadnought, shipTypes.BeamGunship});
            
            eventManager.AddLog(img, eResource.logKatanaShipSalvage);

            eventManager.UnlockEvent(new evDeerKatana());
        }

    }
}