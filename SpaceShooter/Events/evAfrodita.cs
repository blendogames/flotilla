
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evAfroDita : Event
    {
        Rectangle img = sprite.eventSprites.afrodita;

        public evAfroDita()
        {
            musicCue = sounds.Music.dhol;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evAfrodita0);
            popup.eventName = eResource.nameAfrodita;


            popup.AddItem(eResource.evAfrodita0Thanks, OnGiveCargo);

            eventManager.kHaveGauntlet = true;

            base.Activate();
        }

        private void OnGiveCargo(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo(new itGauntlet());

            eventManager.AddLog(img, eResource.logAfrodita);

            eventManager.AddKey(typeof(evDogs));
        }

    }
}