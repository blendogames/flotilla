
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evMurder: Event
    {
        Rectangle img = sprite.eventSprites.downey;

        /// <summary>
        /// RDJ killed someone :(
        /// </summary>
        public evMurder()
        {
            musicCue = sounds.Music.funky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evMurder0);
            popup.eventName = eResource.nameWeapOfficer;
            
            popup.AddItem(eResource.evMurder0Authorities, OnAuthorities);
            popup.AddItem(eResource.evMurder0Help, OnHelp);

            base.Activate();
        }

        private void OnAuthorities(object sender, InputArgs e)
        {
            Helpers.EventRumble();

            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evMurder1Authorities);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logMurderAuthorities);
        }

        private void OnHelp(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evMurder1Help);

            popup.AddItem(Resource.MenuOK, base.OnClose);

            eventManager.AddLog(img, eResource.logMurderHelp);

            eventManager.UnlockEvent(new evSwanPolice());
        }

    }
}