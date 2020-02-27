
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evCatGunReward : Event
    {
        Rectangle img = sprite.eventSprites.catHelmet;

        /// <summary>
        /// Cats with Guns
        /// </summary>
        public evCatGunReward()
        {
            musicCue = sounds.Music.rasta;
        }

        public override void Activate()
        {
            if (!eventManager.kToucansOnboard)
            {
                EventPopup popup = base.CreatePopup(this.manager,
                    img, eResource.evCatGunReward0);
                popup.eventName = eResource.nameCatGuns;
                popup.AddItem(eResource.evCatGunReward0Thanks, OnGunReward);
            }
            else
            {
                //toucans are onboard.
                EventPopup popup = base.CreatePopup(this.manager,
                    img, eResource.evCatGunReward0Toucans);
                popup.eventName = eResource.nameCatGuns;
                popup.AddItem(eResource.evCatGunReward0DamnToucans, OnToucanStole);
            }




            base.Activate();
        }

        private void OnGunReward(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            eventManager.AddCargo();
            eventManager.AddLog(img, eResource.logCatGunsHide);

            eventManager.UnlockEvent(new evCatPrototype());
        }

        private void OnToucanStole(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logCatGunRewardToucanStole);
        }

    }
}