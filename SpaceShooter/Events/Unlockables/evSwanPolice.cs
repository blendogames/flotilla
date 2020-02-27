
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evSwanPolice : Event
    {
        Rectangle img = sprite.eventSprites.swan;

        /// <summary>
        /// Swan investigation
        /// </summary>
        public evSwanPolice()
        {
            shipList = new ShipData[2] { shipTypes.Destroyer, shipTypes.BeamFrigate };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.SecretPolice;

            musicCue = sounds.Music.cool;
            pilotName = eResource.nameSwan;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evSwanPolice0);
            popup.eventName = pilotName;



            popup.AddItem(eResource.evSwanPolice0Attack, OnAttack);
            popup.AddItem(eResource.evSwanPolice0Bribe, OnBribe);

            base.Activate();
        }

        private void OnAttack(object sender, InputArgs e)
        {
            eventManager.AddLog(img, eResource.logSwanPoliceAttack);

            OnStartCombat(sender, e);
        }

        private void OnStartCombat(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);
        }

        private void OnBribe(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);            

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evSwanPolice1Bribe);

            popup.AddItem(eResource.startBattle, OnStartCombat);

            eventManager.AddLog(img, eResource.logSwanPoliceBribe);
        }

    }
}