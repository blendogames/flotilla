
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evDogs : Event
    {
        Rectangle img = sprite.eventSprites.psychicDogs;

        public evDogs()
        {
            musicCue = sounds.Music.phenomena;
            keys = 1;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evDogs0);
            popup.eventName = eResource.nameDogs;

            popup.AddItem(eResource.evDogs0OK, onOk);

            base.Activate();
        }

        private void onOk(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddLog(img, eResource.logDogs);

            eventManager.AddCargo(new itEyeball());
        }
    }
}