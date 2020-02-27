
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evUnicorn : Event
    {
        Rectangle img = sprite.eventSprites.unicorn;

        public evUnicorn()
        {
            musicCue = sounds.Music.dhol;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evUnicorn0);
            popup.eventName = eResource.nameUnicorn;


            popup.AddItem(eResource.evUnicorn0OK, OnGiveCargo);

            eventManager.kHaveGauntlet = true;


            base.Activate();
        }

        private void OnGiveCargo(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo(new itAteneaGauntlet());

            eventManager.AddLog(img, eResource.logUnicorn);
        }

    }
}