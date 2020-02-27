using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    /// <summary>
    /// A derelict space hulk is discovered and explored. The Bruja lurks in the space hulk.
    /// </summary>
    public class evHitchhikerDrop: Event
    {
        Rectangle img = sprite.eventSprites.hitchhikers;

        public evHitchhikerDrop()
        {
            musicCue = sounds.Music.cool;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img,
                eResource.evHitchhikerDrop0);
            popup.eventName = eResource.nameHitchhikers;


            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logHitchhikerDrop);

            eventManager.UnlockEvent(new evHitchhikerGift());

            base.Activate();
        }

    }
}