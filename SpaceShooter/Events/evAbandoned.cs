
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evAbandoned: Event
    {
        Rectangle img = sprite.eventSprites.spacehulk;

        /// <summary>
        /// space fleet, you get free ship.
        /// </summary>
        public evAbandoned()
        {
            musicCue = sounds.Music.spooky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evAbandoned0);
            
            popup.AddItem(eResource.evAbandoned0CleanShip, OnClean);
            popup.AddItem(eResource.evAbandoned0Leave, OnLeave);

            base.Activate();
        }

        private void OnClean(object sender, InputArgs e)
        {
            Helpers.EventRumble();

            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evAbandoned1GrueEat);

            popup.AddItem(Resource.MenuOK, OnCleanDone);

            eventManager.AddLog(img, eResource.logAbandonedKill);
        }

        private void OnCleanDone(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.UnlockEvent(new evDeerbruja());


        }



        private void OnLeave(object sender, InputArgs e)
        {
            eventManager.AddLog(img, eResource.logAbandonedSave);

            eventManager.AddKey(typeof(evAbandonZombies));
            Helpers.CloseThisMenu(sender);
        }

    }
}