
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evPigs : Event
    {
        Rectangle img = sprite.eventSprites.pigs;

        /// <summary>
        /// Baby Yetis.
        /// </summary>
        public evPigs()
        {
            musicCue = sounds.Music.invaders;

            shipList = new ShipData[] { shipTypes.BeamFrigate, shipTypes.Destroyer, shipTypes.Dreadnought, shipTypes.Gunship };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Chickens;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evPigs0);
            popup.eventName = eResource.namePigs;


            
            popup.AddItem(eResource.evPigs0Help, OnHelp);
            popup.AddItem(eResource.evPigs0Leave, OnBetray);

            base.Activate();
        }

        private void OnHelp(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);
            eventManager.AddLog(img, eResource.logPigsHelp);

            eventManager.UnlockEvent(new evPigReward());

            eventManager.UnlockEvent(new evBitterMelon());

            eventManager.AddKey(typeof(evOwl));
        }

        private void OnBetray(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evPigs1Betray);

            popup.AddItem(Resource.MenuOK, OnKilledPigs);

            Helpers.EventRumble();
        }

        private void OnKilledPigs(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo();
            eventManager.AddLog(img, eResource.logPigsKill);

            eventManager.UnlockEvent(new evJaguarBuddy());
        }
    }
}