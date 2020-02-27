#region Using

using Microsoft.Xna.Framework;

#endregion

namespace SpaceShooter
{
    public class evDebug: Event
    {
        public evDebug()
        {
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager);
            popup.image = sprite.eventSprites.cats;
            popup.description = "+1 SHIP";

            popup.AddItem(Resource.MenuOK, OnEnd);
        }

        private void OnEnd(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            eventManager.AddShip(shipTypes.BeamFrigate);
        }
    }
}