
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evBitterMelon : Event
    {
        Rectangle img = sprite.eventSprites.bitterMelon;

        /// <summary>
        /// Rhino attacks you for ripping off casino.
        /// </summary>
        public evBitterMelon()
        {
            musicCue = sounds.Music.drumbeat;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evBitterMelon0);
            popup.eventName = eResource.nameBitterMelon;

            popup.AddItem(eResource.evHippoBuddy0Welcome, bitterBuddies);

            base.Activate();
        }

        private void bitterBuddies(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(shipTypes.Battleship);

            eventManager.AddLog(img, eResource.logBitterMelon);

            eventManager.UnlockEvent(new evBitterMelonConcert());
        }
    }
}