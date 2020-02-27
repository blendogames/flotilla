
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evTerminalDeath: Event
    {
        Rectangle img = sprite.eventSprites.bouquet;

        /// <summary>
        /// Baby Yetis.
        /// </summary>
        public evTerminalDeath()
        {
            musicCue = sounds.Music.raindrops01;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evTerminalDeath0);

            popup.AddItem(eResource.evTerminalDeath0TheEnd, OnEnd);

            base.Activate();
        }



        private void OnEnd(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logTerminalDeath);

            FrameworkCore.worldMap.EndGame();
        }

    }
}