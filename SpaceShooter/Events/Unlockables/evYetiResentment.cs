
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evYetiResentment: Event
    {
        Rectangle img = sprite.eventSprites.babyyetis;

        /// <summary>
        /// Baby Yetis.
        /// </summary>
        public evYetiResentment()
        {
            musicCue = sounds.Music.cool;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evYetiResentment0);
            popup.eventName = eResource.nameNavOfficer;


            popup.AddItem(eResource.evYetiResentment0Ok, base.OnClose);

            eventManager.UnlockEvent(new evYetiStarve());

            eventManager.AddLog(img, eResource.logYetiResentment);

            base.Activate();
        }
    }
}