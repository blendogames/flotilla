
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evBabyYetis: Event
    {
        Rectangle img = sprite.eventSprites.babyyetis;

        /// <summary>
        /// Baby Yetis.
        /// </summary>
        public evBabyYetis()
        {
            musicCue = sounds.Music.bird;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evBabyYetis0);
            popup.eventName = eResource.nameNavOfficer;
            
            popup.AddItem(eResource.evBabyYetis0Airlock, OnAirlock);
            popup.AddItem(eResource.evBabyYetis0Declaw, OnDeclaw);

            base.Activate();
        }

        private void OnAirlock(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evBabyYetis1Airlock);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logBabyYetisKill);

            eventManager.UnlockEvent(new evYetiRevenge());
        }

        private void OnDeclaw(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evBabyYetis1Declaw);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.UnlockEvent(new evYetiResentment());

            eventManager.AddLog(img, eResource.logBabyYetisSave);
        }

    }
}