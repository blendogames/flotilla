
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evJaguarBuddy : Event
    {
        Rectangle img = sprite.eventSprites.leopard;

        /// <summary>
        /// Players kills a lot of deer, a hippo joins hte flotilla
        /// </summary>
        public evJaguarBuddy()
        {
            musicCue = sounds.Music.bird;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evJaguarBuddy0);
            popup.eventName = eResource.nameJaguar;


            popup.AddItem(eResource.evJaguarBuddy0Welcome, OnGetShip);

            base.Activate();
        }

        void OnGetShip(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(new ShipData[] { shipTypes.Destroyer, shipTypes.Gunship });

            eventManager.AddLog(img, eResource.logJaguarBuddy);
        }
    }
}