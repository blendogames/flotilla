
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evPrisoner: Event
    {
        Rectangle img = sprite.eventSprites.deer;

        /// <summary>
        /// Prisoner's dilemma
        /// </summary>
        public evPrisoner()
        {
            musicCue = sounds.Music.cool;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evPrisoner0);
            popup.eventName = eResource.nameDeer;


            
            popup.AddItem(eResource.evPrisoner0StaySilent, OnStaySilent);
            popup.AddItem(eResource.evPrisoner0GiveUp, OnGiveUp);

            base.Activate();
        }

        private void OnStaySilent(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            string itemName = "";
            string itemDesc = "";
            string description = eResource.evPrisoner1Silence;
            if (eventManager.LoseRandomCargo(out itemName, out itemDesc))
            {
                description += "\n\n";
                description += string.Format(eResource.evPrisoner0StaySilentTakeCargo,
                    itemName, itemDesc);
            }

            EventPopup popup = base.CreatePopup(this.manager,
                img, description);


            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logPrisonerSilent);
        }

        private void OnGiveUp(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evPrisoner1GiveUp);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logPrisonerGiveUp);
        }

    }
}