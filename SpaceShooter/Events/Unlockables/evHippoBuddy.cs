
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evHippoBuddy : Event
    {
        Rectangle img = sprite.eventSprites.pigeyeHippo;

        /// <summary>
        /// Players kills a lot of deer, a hippo joins hte flotilla
        /// </summary>
        public evHippoBuddy()
        {
            musicCue = sounds.Music.funky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evHippoBuddy0);
            popup.eventName = eResource.nameHippoBuddy;


            popup.AddItem(eResource.evHippoBuddy0Welcome, OnGetShip);

            base.Activate();
        }

        void OnGetShip(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(new ShipData[]{shipTypes.Dreadnought, shipTypes.BeamGunship});

            eventManager.AddLog(img, eResource.logHippoBuddy);
        }
    }
}