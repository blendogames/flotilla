
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evCatGuns : Event
    {
        Rectangle img = sprite.eventSprites.catHelmet;

        /// <summary>
        /// Cats with Guns
        /// </summary>
        public evCatGuns()
        {
            musicCue = sounds.Music.rasta;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evCatGuns0);
            popup.eventName = eResource.nameCatGuns;

            popup.AddItem(eResource.evCatGuns0HideGuns, OnHideGuns);
            popup.AddItem(eResource.evCatGuns0Refuse, OnRefuse);

            base.Activate();
        }

        private void OnHideGuns(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo();

            eventManager.UnlockEvent(new evCatGunReward());

            eventManager.AddLog(img, eResource.logCatGunsHide);
        }

        private void OnRefuse(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);            

            eventManager.AddLog(img, eResource.logCatGunsRefuse);
        }

    }
}