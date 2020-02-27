
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evYetiStarve: Event
    {
        Rectangle img = sprite.eventSprites.yetis;

        /// <summary>
        /// Yetis eat your food stores.
        /// </summary>
        public evYetiStarve()
        {
            musicCue = sounds.Music.bird;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evYetiStarve0);
            popup.eventName = eResource.nameYetis;


            popup.AddItem(eResource.evYetiStarve0Elderly, OnElderly);
            popup.AddItem(eResource.evYetiStarve0Young, OnYouth);

            base.Activate();
        }

        private void OnElderly(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evYetiStarve1Elderly);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logYetiStarveOld);
        }

        private void OnYouth(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evYetiStarve1Young);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logYetiStarveYouth);
        }

    }
}