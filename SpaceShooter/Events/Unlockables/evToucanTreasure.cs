
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evToucanTreasure : Event
    {
        Rectangle img = sprite.eventSprites.toucans;

        /// <summary>
        /// Rhino attacks you for ripping off casino.
        /// </summary>
        public evToucanTreasure()
        {
            musicCue = sounds.Music.funky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evToucanTreasure0);
            popup.eventName = eResource.nameToucan;


            popup.AddItem(eResource.evToucanTreasure0Yay, OnTreasure);

            base.Activate();
        }

        private void OnTreasure(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);            

            eventManager.AddLog(img, eResource.logToucanTreasure);

            //give the inventory item.
            eventManager.AddCargo(new itTotem());

            Helpers.AddPointBonus();
        }
    }
}