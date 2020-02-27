
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evProfessors: Event
    {
        Rectangle img = sprite.eventSprites.professors;

        /// <summary>
        /// Professor contract
        /// </summary>
        public evProfessors()
        {
            musicCue = sounds.Music.guitar;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evProfessors0);
            popup.eventName = eResource.nameCrisium;


            popup.AddItem(eResource.evProfessors0Ship, OnGiveShip);
            popup.AddItem(eResource.evProfessors0Cargo, OnGiveCargo);

            eventManager.AddKey(typeof(evAuroraResearchBad));
            eventManager.AddKey(typeof(evAuroraResearchGood));

            eventManager.kCrisiumOnBoard = true;

            base.Activate();
        }

        private void OnGiveShip(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddShip(new ShipData[]{shipTypes.Destroyer, shipTypes.Gunship});

            eventManager.AddLog(img, eResource.logProfessorsShip);
        }

        private void OnGiveCargo(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            eventManager.AddCargo();

            eventManager.AddLog(img, eResource.logProfessorsItem);
        }

    }
}