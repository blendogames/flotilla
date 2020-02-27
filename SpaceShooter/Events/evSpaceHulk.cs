
using Microsoft.Xna.Framework;

namespace SpaceShooter
{
    /// <summary>
    /// A derelict space hulk is discovered and explored. The Bruja lurks in the space hulk.
    /// </summary>
    public class evSpaceHulk : Event
    {
        Rectangle img = sprite.eventSprites.spacehulk;

        public evSpaceHulk()
        {
            musicCue = sounds.Music.spooky;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager);
            popup.image = img;
            popup.description = string.Format(eResource.evSpaceHulk0, Helpers.GenerateName("Adjective"), Helpers.GenerateName("Animal"));

            popup.AddItem(eResource.evSpaceHulk0Investigate, OnInvestigate);
            popup.AddItem(eResource.evSpaceHulk0Leave, OnLeave);

            base.Activate();
        }

        private void OnLeave(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            eventManager.AddLog(img, eResource.logSpacehulkIgnore);
        }

        private void OnInvestigate(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            int rand = FrameworkCore.r.Next(2);

            if (rand == 0)
            {
                EventPopup popup = base.CreatePopup(this.manager);
                popup.image = img;
                popup.description = eResource.evSpaceHulk1BrujaLost;

                popup.AddItem(Resource.MenuOK, base.OnClose);

                eventManager.AddLog(img, eResource.logSpacehulkBruja);
            }
            else
            {
                //GET CARGO.
                EventPopup popup = base.CreatePopup(this.manager);
                popup.image = img;
                popup.description = eResource.evSpaceHulk1Cargo;

                popup.AddItem(Resource.MenuOK, base.OnGetCargo);

                eventManager.AddLog(img, eResource.logSpacehulkCargo);

                eventManager.UnlockEvent(new evBruja());
            }
        }
    }
}