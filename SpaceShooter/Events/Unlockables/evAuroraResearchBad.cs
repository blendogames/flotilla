
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evAuroraResearchBad: Event
    {
        Rectangle img = sprite.eventSprites.professors;

        /// <summary>
        /// Professor contract
        /// </summary>
        public evAuroraResearchBad()
        {
            keys = 2;
            musicCue = sounds.Music.guitar;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evAuroraResearchBad0);
            popup.eventName = eResource.nameCrisium;


            
            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logAuroraResearchBad);

            base.Activate();
        }
    }
}