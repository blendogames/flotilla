
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evCatPrototype : Event
    {
        Rectangle img = sprite.eventSprites.catHelmet;

        /// <summary>
        /// Cats with Guns
        /// </summary>
        public evCatPrototype()
        {
            musicCue = sounds.Music.rasta;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evCatPrototype0);
            popup.eventName = eResource.nameCatGuns;

            popup.AddItem(eResource.evCatPrototype0Goodnight, OnDone);

            base.Activate();

            eventManager.AddLog(img, eResource.logCatPrototype);

            

        }

        private void OnDone(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(shipTypes.Fighter);

            Helpers.AddPointBonus();
        }

 

    }
}