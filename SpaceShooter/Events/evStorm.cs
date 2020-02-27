using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    public class evStorm: Event
    {
        Rectangle img = sprite.eventSprites.cats;

        /// <summary>
        /// UNLOCKABLE. some cats save you from a storm.
        /// </summary>
        public evStorm()
        {
            musicCue = sounds.Music.rasta;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evStorm0);
            popup.eventName = eResource.nameCats;

            popup.AddItem(eResource.evStorm0Thanks, OnOk);

            base.Activate();
        }

        private void OnOk(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evStorm1);

            popup.AddItem(eResource.evStorm1Thanks, base.OnGetCargo);

            eventManager.AddLog(img, eResource.logStorm);
        }        
    }
}