
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evStowaway : Event
    {
        Rectangle img = sprite.eventSprites.toucans;

        /// <summary>
        /// Stowaway
        /// </summary>
        public evStowaway()
        {
            musicCue = sounds.Music.bird;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evStowaways0);
            popup.eventName = eResource.nameToucan;


            popup.AddItem(eResource.evStowaways0LetLive, OnLetLive);
            popup.AddItem(eResource.evStowaways0Shoot, OnKill);

            base.Activate();
        }

        private void OnLetLive(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evStowaways1Live);

            popup.AddItem(Resource.MenuOK, base.OnClose);
            eventManager.AddKey(typeof(evAssassinRevenge));

            eventManager.AddLog(img, eResource.logStowawayHelp);

            eventManager.kToucansOnboard = true;
        }

        private void OnKill(object sender, InputArgs e)
        {
            Helpers.EventRumble();

            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evStowaways1Shoot);

            popup.AddItem(Resource.MenuOK, base.OnGetCargo);

            eventManager.UnlockEvent(new evToucanRevenge());

            eventManager.AddLog(img, eResource.logStowawayKill);
        }
    }
}