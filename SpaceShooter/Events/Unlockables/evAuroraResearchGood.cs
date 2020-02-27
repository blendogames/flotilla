
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evAuroraResearchGood: Event
    {
        Rectangle img = sprite.eventSprites.professors;

        /// <summary>
        /// Professor contract
        /// </summary>
        public evAuroraResearchGood()
        {
            keys = 2;
            musicCue = sounds.Music.guitar;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evAuroraResearchGood0);
            popup.eventName = eResource.nameCrisium;



            
            popup.AddItem(eResource.evAuroraResearchGood0Indeed, OnDone);

            base.Activate();
        }


        private void OnDone(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evAuroraResearchGood1Study);

            popup.AddItem(Resource.MenuOK, base.OnGetCargo);

            eventManager.AddLog(img, eResource.logAuroraResearchGood);
        }

    }
}