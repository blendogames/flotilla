using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    public class evRuins: Event
    {
        Rectangle img = sprite.eventSprites.cats;

        /// <summary>
        /// some cats are found in burning ruins.
        /// </summary>
        public evRuins()
        {
            musicCue = sounds.Music.rasta;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evRuins0);
            popup.eventName = eResource.nameCats;

            popup.AddItem(eResource.evRuins0Help, OnHelp);
            popup.AddItem(eResource.evRuins0Turnin, OnTurnin);

            base.Activate();
        }

        private void OnHelp(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evRuins1Help);

            popup.AddItem(Resource.MenuOK, base.OnGetCargo);

            eventManager.AddKey(typeof(evCatCharity));
            eventManager.UnlockEvent(new evStorm());

            eventManager.UnlockEvent(new evPrisoner());

            eventManager.UnlockEvent(new evCatGuns());
            

            eventManager.AddLog(img, eResource.logRuinsHelp);
        }
        
        private void OnTurnin(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                            img, eResource.evRuins1Turnin);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logRuinsAuthorities);

            eventManager.UnlockEvent(new evCatBounty());
        }
    }
}