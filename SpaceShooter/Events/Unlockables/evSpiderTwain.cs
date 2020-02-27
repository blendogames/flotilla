
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evSpiderTwain : Event
    {
        Rectangle img = sprite.eventSprites.spiderTwain;

        /// <summary>
        /// Swan investigation
        /// </summary>
        public evSpiderTwain()
        {
            shipList = new ShipData[] { shipTypes.Dreadnought, shipTypes.Gunship };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Spider;

            musicCue = sounds.Music.moog;
            pilotName = eResource.nameSpiderTwain;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evSpiderTwain0);
            popup.eventName = pilotName;



            popup.AddItem(eResource.evSpiderTwain0AcceptBribe, OnAcceptBribe);
            popup.AddItem(eResource.evSpiderTwain0Fight, OnFight);

            base.Activate();

            Helpers.AddPointBonus();
        }


        private void OnAcceptBribe(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo();            

            eventManager.AddLog(img, eResource.logSpiderTwainBribe);
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logSpiderTwainFight);
        }

    }
}