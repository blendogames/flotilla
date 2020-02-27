
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evWine: Event
    {
        Rectangle img = sprite.eventSprites.wine;

        /// <summary>
        /// Baby Yetis.
        /// </summary>
        public evWine()
        {
            musicCue = sounds.Music.guitar;
        }

        public override void Activate()
        {
            string popupTxt = string.Format(eResource.evWine0, Helpers.GetPlayerName().ToUpper());

            EventPopup popup = base.CreatePopup(this.manager,
                img, popupTxt);

            
            popup.AddItem(eResource.evKaraoke0Great, OnPrize);

            base.Activate();
        }

        private void OnPrize(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(shipTypes.BeamGunship);

            eventManager.AddLog(img, eResource.logWine);

            

        }

    }
}