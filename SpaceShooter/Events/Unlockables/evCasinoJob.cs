
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{

    public class evCasinoJob : Event
    {
        Rectangle img = sprite.eventSprites.queen;

        /// <summary>
        /// Players kills slavers, panda joins.
        /// </summary>
        public evCasinoJob()
        {
            musicCue = sounds.Music.funky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evCasinoJob0);
            popup.eventName = Resource.FactionCasino;


            popup.AddItem(Resource.MenuOK, OnTakeJob);

            base.Activate();
        }

        void OnTakeJob(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logCasinoJob);

            eventManager.UnlockEvent(new evSpiderTwain());
        }
    }
}