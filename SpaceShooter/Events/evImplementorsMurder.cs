
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evImplementorsMurder : Event
    {
        Rectangle img = sprite.eventSprites.implementors;

        /// <summary>
        /// Implementors Dream.
        /// </summary>
        public evImplementorsMurder()
        {
            musicCue = sounds.Music.phenomena;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evImplementorsMurder0);
            popup.eventName = eResource.nameImplementors;


            popup.AddItem(eResource.evImplementorsMurder0Authorities, OnAuthorities);
            popup.AddItem(eResource.evImplementorsMurder0Help, OnHelp);

            eventManager.AddLog(img, eResource.logImplementors);

            base.Activate();
        }



        private void OnAuthorities(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        private void OnHelp(object sender, InputArgs e)
        {
            eventManager.AddKey(typeof(evDogBoots));
            Helpers.CloseThisMenu(sender);
        }


    }
}