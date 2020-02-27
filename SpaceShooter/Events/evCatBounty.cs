
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evCatBounty : Event
    {
        Rectangle img = sprite.eventSprites.deer;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evCatBounty()
        {
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameDeer;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evCatBounty0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.evAfrodita0Thanks, OnReward);

            base.Activate();
        }

        private void OnReward(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo();

            eventManager.AddLog(img, eResource.logCatBounty);
        }

    }
}