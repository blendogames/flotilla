
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evBruja : Event
    {
        Rectangle img = sprite.eventSprites.mouse;

        public evBruja()
        {
            musicCue = sounds.Music.singing;
        }

        public override void Activate()
        {
            if (eventManager.kHaveGauntlet)
            {
                string txt = string.Format(eResource.evBruja0, eResource.evBruja0Gauntlet);
                EventPopup popup = base.CreatePopup(this.manager,
                    img, txt);
                popup.eventName = eResource.nameBruja;

                popup.AddItem(eResource.evBruja0OK, onGauntlet);
            }
            else
            {
                //fail.
                string txt = string.Format(eResource.evBruja0, eResource.evBruja0Fail);
                EventPopup popup = base.CreatePopup(this.manager,
                    img, txt);
                popup.eventName = eResource.nameBruja;

                popup.AddItem(eResource.evBruja0OK, onFail);
            }

            base.Activate();
        }

        private void onGauntlet(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logBrujaGauntlet);

            eventManager.AddCargo(new itHeart());
        }

        private void onFail(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logBrujaFail);

            eventManager.UnlockEvent(new evOwlTattoo());
        }
    }
}