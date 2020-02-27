
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evPigReward : Event
    {
        Rectangle img = sprite.eventSprites.pigs;

        /// <summary>
        /// Swan investigation
        /// </summary>
        public evPigReward()
        {
            musicCue = sounds.Music.bird;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evPigReward0);
            popup.eventName = eResource.namePigs;


            popup.AddItem(eResource.evPigReward0Ship, OnShip);
            popup.AddItem(eResource.evPigReward0Stock, OnStock);

            base.Activate();
        }

        private void OnShip(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(shipTypes.Battleship);

            eventManager.AddLog(img, eResource.logPigRewardShip);
        }

        private void OnStock(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);            

            eventManager.AddLog(img, eResource.logPigRewardStock);
        }

    }
}