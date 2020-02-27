
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evDeer : Event
    {
        Rectangle img = sprite.eventSprites.deer;

        /// <summary>
        /// Crazed bandit with space madness.
        /// </summary>
        public evDeer()
        {
            shipList = new ShipData[2] { shipTypes.Battleship, shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Deer;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameDeer;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evDeer0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.evDeer0Pay, OnPay);
            popup.AddItem(eResource.evDeer0Refuse, OnFight);

            base.Activate();
        }

        private void OnPay(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            string itemName = "";
            string itemDesc = "";
            string description = "";
            if (eventManager.LoseRandomCargo(out itemName, out itemDesc))
            {
                description = string.Format(eResource.evDeer1Pay, itemName+"\n"+itemDesc);
            }
            else
            {
                //you don't have any cargo you bum
                description = eResource.evDeer1PayNoCargo;
            }

            EventPopup popup = base.CreatePopup(this.manager,
                img, description);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddKey(typeof(evCatCharity));

            eventManager.AddLog(img, eResource.logDeerPay);
        }





        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evDeer1Fight);

            popup.AddItem(eResource.startBattle, StartBattle);

            eventManager.AddKey(typeof(evOwl));
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.UnlockEvent(new evDeerRevenge());

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logDeerFight);
        }
    }
}