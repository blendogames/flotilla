using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    /// <summary>
    /// A derelict space hulk is discovered and explored. The Bruja lurks in the space hulk.
    /// </summary>
    public class evHitchhikerGift : Event
    {
        Rectangle img = sprite.eventSprites.hitchhikers;

        public evHitchhikerGift()
        {
            musicCue = sounds.Music.dhol;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evHitchhikerGift0);
            popup.eventName = eResource.nameHitchhikers;


            popup.AddItem(eResource.evHitchhikerGift0OK, OnGift);

            base.Activate();
        }

        private void OnGift(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(new ShipData[]{shipTypes.BeamGunship, shipTypes.Battleship});

            eventManager.AddLog(img, eResource.logHitchhikerGift);
        }
    }
}