
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evCasino : Event
    {
        Rectangle img = sprite.eventSprites.queen;

        /// <summary>
        /// Casino gambling
        /// </summary>
        public evCasino()
        {
            musicCue = sounds.Music.funky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager, img,
                eResource.evCasino0);
            popup.eventName = Resource.FactionCasino;

            popup.AddItem(eResource.evCasino0UhOh, OnDescription);

            base.Activate();
        }






        public void OnDescription(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evCasino1);

            popup.AddItem(eResource.evCasino1Agree, OnAgree);
            popup.AddItem(eResource.evCasino1Disagree, OnDisagree);
        }

        private void OnAgree(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evCasino2Agree);

            popup.AddItem(Resource.MenuOK, base.OnClose);
            WorkForCasino();

            eventManager.UnlockEvent(new evCasinoJob());
        }

        private void OnDisagree(object sender, InputArgs e)
        {
            Helpers.EventRumble();

            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                    img, eResource.evCasino2Disagree);

            popup.AddItem(eResource.evCasino2DisagreeYikes, base.OnClose);

            eventManager.AddLog(img, eResource.logCasinoLoseCargo);

            eventManager.UnlockEvent(new evCasinoRhino());
            eventManager.UnlockEvent(new evPenguinHitman());

            /*
            bool lostAllCargo = eventManager.LoseAllCargo();

            if (lostAllCargo)
            {
                //PLAYER LOSES ALL CARGO.
                EventPopup popup = base.CreatePopup(this.manager,
                    img, eResource.evCasino2Disagree);

                popup.AddItem(Resource.MenuOK, base.OnClose);

                eventManager.AddLog(img, eResource.logCasinoLoseCargo);
            }
            else
            {
                //PLAYER HAS NO CARGO.
                EventPopup popup = base.CreatePopup(this.manager,
                    img, eResource.evCasino2NoCargo);

                popup.AddItem(Resource.MenuOK, base.OnClose);
                WorkForCasino();
            }*/
        }

        private void WorkForCasino()
        {
            eventManager.AddLog(img, eResource.logCasinoWork);
        }
    }
}