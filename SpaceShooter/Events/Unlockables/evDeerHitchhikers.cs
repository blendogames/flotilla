
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evDeerHitchhikers : Event
    {
        Rectangle img = sprite.eventSprites.deer;

        /// <summary>
        /// The deer know you have a bruja-tainted ship.
        /// </summary>
        public evDeerHitchhikers()
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
                img, eResource.evDeerHitchhikers0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.evDeer0Pay, OnPay);
            popup.AddItem(eResource.evDeer0Refuse, OnFight);

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            eventManager.AddLog(img, eResource.logDeerHitchhikerFight);
            StartBattle(sender, e);
        }

        private void OnPay(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            string itemName = "";
            string itemDesc = "";
            string description = "";
            if (eventManager.LoseRandomCargo(out itemName, out itemDesc))
            {
                description = string.Format(eResource.evDeer1Pay, itemName + "\n" + itemDesc);

                EventPopup popup = base.CreatePopup(this.manager,
                    img, description);

                popup.AddItem(Resource.MenuOK, base.OnClose);

                eventManager.AddLog(img, eResource.logDeerHitchhikerPay);
            }
            else
            {
                //you don't have any cargo you bum
                description = eResource.evDeerHitchhikers0NoCargo;                

                EventPopup popup = base.CreatePopup(this.manager,
                    img, description);

                popup.AddItem(eResource.startBattle, StartBattle);

                eventManager.AddLog(img, eResource.logDeerHitchhikerPayNoCargo);
            }            
        }

        private void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);
        }
    }
}