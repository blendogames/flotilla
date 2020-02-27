
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evDemoEnd : Event
    {
        Rectangle img = sprite.eventSprites.deer;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evDemoEnd()
        {
            musicCue = sounds.Music.drumbeat;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evDemoEnd0);

            popup.AddItem(Resource.MenuUnlockFullGame, OnBuy);
            popup.AddItem(Resource.MenuDemoPlanetMaybeLater, OnLater);

            base.Activate();
        }

        private void OnBuy(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logDemoEnd);

            FrameworkCore.worldMap.EndGame();

            FrameworkCore.BuyGame();
        }

        private void OnLater(object sender, InputArgs e)
        {
            Helpers.EventRumble();

            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logDemoEnd);

            FrameworkCore.worldMap.EndGame();
        }

    }
}