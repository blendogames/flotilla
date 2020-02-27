using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    /// <summary>
    /// UNLOCKABLE. Hitchhikers exact their revenge.
    /// </summary>
    public class evHitchhikerRevenge: Event
    {
        Rectangle img = sprite.eventSprites.hitchhikers;

        public evHitchhikerRevenge()
        {
            musicCue = sounds.Music.invaders;
            shipList = new ShipData[1] { shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Red;
            pilotName = eResource.nameHitchhikers;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img,
                eResource.evHitchhikerRevenge0);
            popup.eventName = pilotName;


            popup.AddItem(eResource.evHitchhikerRevenge0GiveCargo, OnGiveUp);
            popup.AddItem(eResource.evHitchhikerRevenge0Fight, OnFight);

            base.Activate();
        }

        void OnGiveUp(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img,
                eResource.evHitchhikerRevenge1GiveUp);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.LoseAllCargo();

            eventManager.AddLog(img, eResource.logHitchhikerRevengePay);
        }


        void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img,
                eResource.evHitchhikerRevenge1Struggle);

            popup.AddItem(eResource.startBattle, StartBattle);

            eventManager.AddLog(img, eResource.logHitchhikerRevengeFight);

            eventManager.AddKey(typeof(evDogs));

            eventManager.AddKey(typeof(evOwl));
        }

        public void StartBattle(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            FrameworkCore.worldMap.EnterCombat(this);
        }
    }
}