
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evAbandonZombies : Event
    {
        Rectangle img = sprite.eventSprites.zombies;

        /// <summary>
        /// The bruja triggers a zombie infestation
        /// </summary>
        public evAbandonZombies()
        {
            musicCue = sounds.Music.bird;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evAbandonZombies0);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logAbandonZombies);

            base.Activate();
        }
    }
}