
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{

    public class evHippoPirateBuddy : Event
    {
        Rectangle img = sprite.eventSprites.hippo;

        /// <summary>
        /// Players kills a lot of pirates, a hippo joins hte flotilla
        /// </summary>
        public evHippoPirateBuddy()
        {
            keys = 1;
            musicCue = sounds.Music.funky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evHippoPirateBuddy);
            popup.eventName = eResource.nameHippoPirateBuddy;


            popup.AddItem(eResource.evHippoBuddy0Welcome, OnGetShip);

            base.Activate();
        }

        void OnGetShip(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(new ShipData[]{shipTypes.Destroyer, shipTypes.Gunship});

            eventManager.AddLog(img, eResource.logHippoPirateBuddy);
        }
    }
}