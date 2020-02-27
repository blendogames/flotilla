using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    public class evCatCharity: Event
    {
        Rectangle img = sprite.eventSprites.cats;

        /// <summary>
        /// if PLAYER IS FRIENDS WITH CATS && DEER HAS EXTORTED PLAYER.
        /// </summary>
        public evCatCharity()
        {
            keys = 2;
            musicCue = sounds.Music.rasta;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evCatCharity0);
            popup.eventName = eResource.nameCats;

            popup.AddItem(eResource.evStorm0Thanks, base.OnGetCargo);

            eventManager.AddLog(img, eResource.logCatCharity);

            base.Activate();
        }
    }
}