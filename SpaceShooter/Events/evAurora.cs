using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    /// <summary>
    /// A derelict space hulk is discovered and explored. The Bruja lurks in the space hulk.
    /// </summary>
    public class evAurora: Event
    {
        Rectangle img = sprite.eventSprites.aurora;

        public evAurora()
        {
            musicCue = sounds.Music.phenomena;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager);
            popup.image = img;
            popup.description = string.Format(eResource.evAurora0,
                Helpers.GenerateName("Planet"), Helpers.GenerateName("Planet"));

            popup.AddItem(eResource.evAurora0ContinueStaring, OnStare);
            popup.AddItem(eResource.evAurora0StopStaring, OnNoStare);

            base.Activate();
        }

        private void OnStare(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager);
            popup.image = img;
            popup.description = eResource.evAurora1Staring;

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddKey(typeof(evAuroraResearchGood));

            eventManager.AddLog(img, eResource.logAuroraStare);

            eventManager.UnlockEvent(new evImplementors());
            eventManager.UnlockEvent(new evImplementorsMurder());
        }

        private void OnNoStare(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddKey(typeof(evAuroraResearchBad));

            eventManager.AddLog(img, eResource.logAuroraNoStare);
        }        
    }
}