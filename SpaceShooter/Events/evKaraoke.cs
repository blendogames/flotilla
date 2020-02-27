
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evKaraoke  : Event
    {
        Rectangle img = sprite.eventSprites.fans;

        /// <summary>
        /// Baby Yetis.
        /// </summary>
        public evKaraoke()
        {
            musicCue = sounds.Music.drumbeat;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evKaraoke0);

            
            popup.AddItem(eResource.evKaraoke0Great, OnPrize);

            base.Activate();
        }

        private void OnPrize(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo();

            eventManager.AddLog(img, eResource.logKaraoke);

            eventManager.UnlockEvent(new evKaraokeRevenge());
            eventManager.UnlockEvent(new evWine());

        }

    }
}