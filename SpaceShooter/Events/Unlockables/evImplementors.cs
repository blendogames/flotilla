
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evImplementors : Event
    {
        Rectangle img = sprite.eventSprites.implementors;

        /// <summary>
        /// Implementors Dream.
        /// </summary>
        public evImplementors()
        {
            musicCue = sounds.Music.phenomena;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evImplementors0);
            popup.eventName = eResource.nameImplementors;


            popup.AddItem(eResource.evImplementors0Obey, OnObey);
            popup.AddItem(eResource.evImplementors0Defy, OnDefy);

            eventManager.AddLog(img, eResource.logImplementors);

            base.Activate();
        }



        private void OnObey(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
        }


        private void OnDefy(object sender, InputArgs e)
        {
            eventManager.AddKey(typeof(evDogBoots));
            Helpers.CloseThisMenu(sender);
        }


    }
}