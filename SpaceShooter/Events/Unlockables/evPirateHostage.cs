
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// chicken space pirates
    /// </summary>
    public class evPirateHostage : Event
    {
        Rectangle img = sprite.eventSprites.chickens;

        /// <summary>
        /// Pirates want to recruit you
        /// </summary>
        public evPirateHostage()
        {
            shipList = new ShipData[2] { shipTypes.BeamFrigate, shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Chickens;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.namePirates;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager, img,
                eResource.evPirateHostage0 );
            popup.eventName = pilotName;


            popup.AddItem(eResource.evPirateHostage0GiveCargo, OnGiveCargo);
            popup.AddItem(eResource.evPirateHostage0Attack, OnFight);

            eventManager.AddKey(typeof(evHippoPirateBuddy));

            base.Activate();
        }

        private void OnGiveCargo(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            string cargo1, cargo2, cargo3, desc1, desc2, desc3;
            bool hasCargo = eventManager.LoseRandomCargo(out cargo1, out desc1);
            eventManager.LoseRandomCargo(out cargo2, out desc2);
            eventManager.LoseRandomCargo(out cargo3, out desc3);

            string popupText = "";
            if (hasCargo)
            {
                popupText = string.Format(eResource.evPirateHostage1GiveCargo,
                    cargo1 + "\n" + desc1,
                    cargo2 + "\n" + desc2,
                    cargo3 + "\n" + desc3);
                EventPopup popup = base.CreatePopup(this.manager, img,
                popupText);

                popup.AddItem(Resource.MenuOK, base.OnClose);

                eventManager.AddLog(img, eResource.logPirateHostageCargo);
            }
            else
            {
                popupText = eResource.evPirateHostage1NoCargo;
                EventPopup popup = base.CreatePopup(this.manager, img,
                    popupText);

                popup.AddItem(eResource.startBattle, OnFightNoCargo);
            }
        }

        private void OnFightNoCargo(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logPirateHostageNoCargo);
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logPirateHostageAttack);

            eventManager.UnlockEvent(new evKoala());
        }
    }
}