
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evBitterMelonConcert : Event
    {

        public evBitterMelonConcert()
        {
            shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.Gunship };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Groupies;

            musicCue = sounds.Music.drumbeat;
        }

        public override void Activate()
        {
            if (!eventManager.kPandaOnboard)
            {
                EventPopup popup = base.CreatePopup(this.manager,
                    sprite.eventSprites.bitterMelon, eResource.evBitterMelonConcert0);
                popup.eventName = eResource.nameBitterMelon;

                popup.AddItem(eResource.startBattle, startBattle);
            }
            else
            {
                EventPopup popup = base.CreatePopup(this.manager,
                    sprite.eventSprites.pandaBitterMelon, eResource.evBitterMelonPanda0);
                popup.eventName = eResource.nameBitterMelon;

                popup.AddItem(Resource.MenuOK, OnPanda);
            }

            base.Activate();

            Helpers.AddPointBonus();
        }

        private void startBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            FrameworkCore.worldMap.EnterCombat(this);
            eventManager.AddLog(sprite.eventSprites.bitterMelon, eResource.logBitterMelonConcert);
        }

        private void OnPanda(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            FrameworkCore.worldMap.EnterCombat(this);
            eventManager.AddLog(sprite.eventSprites.pandaBitterMelon, eResource.logBitterMelonPanda);

            eventManager.AddCargo();
        }
    }
}