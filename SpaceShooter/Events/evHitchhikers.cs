using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    /// <summary>
    /// A derelict space hulk is discovered and explored. The Bruja lurks in the space hulk.
    /// </summary>
    public class evHitchhikers: Event
    {
        Rectangle img = sprite.eventSprites.hitchhikers;
        
        public evHitchhikers()
        {
            musicCue = sounds.Music.cool;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img,
                eResource.evHitchhikers0);
            popup.eventName = eResource.nameHitchhikers;


            popup.AddItem(eResource.evHitchhikers0Yes, OnYes);
            popup.AddItem(eResource.evHitchhikers0No, OnDecline);

            base.Activate();

        }

        void OnDecline(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            eventManager.AddLog(img, eResource.logHitchhikersDecline);
        }

        void OnYes(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img,
                eResource.evHitchhikers1Chat);

            eventManager.UnlockEvent(new evDeerHitchhikers());

            popup.AddItem(eResource.evHitchHikers1ChatSteal, OnStealCargo);
            popup.AddItem(eResource.evHitchHikers1ChatLeave, OnNoSteal);
        }

        /// <summary>
        /// Steal the cargo from the hitchhikers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStealCargo(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            Helpers.EventRumble();

            EventPopup popup = base.CreatePopup(this.manager,
                img,
                eResource.evHitchHikers2StealCargo);

            popup.AddItem(Resource.MenuOK, base.OnGetCargo);

            eventManager.UnlockEvent(new evHitchhikerRevenge());

            eventManager.AddLog(img, eResource.logHitchhikersSteal);
        }

        void OnNoSteal(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.UnlockEvent(new evHitchhikerDrop());

            eventManager.AddLog(img, eResource.logHitchhikersNoSteal);
        }
    }
}