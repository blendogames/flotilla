
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    public class evTutorial: Event
    {
        Rectangle img = sprite.eventSprites.destroyers;

        /// <summary>
        /// Baby Yetis.
        /// </summary>
        public evTutorial()
        {
            shipList = new ShipData[1] { shipTypes.Drone };
            shipMinMax = new Point(1,1);
            faction = Faction.Red;
            musicCue = sounds.Music.guitar;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evTutorial0);

            popup.AddItem(eResource.evTutorial1RunTutorial, OnDoTutorial);
            popup.AddItem(eResource.evTutorial1SkipTutorial, OnSkipTutorial);

            base.Activate();
        }

        /*
        private void OnOK(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            EventPopup popup = base.CreatePopup(this.manager,
                img, eResource.evTutorial1);

            popup.AddItem(eResource.evTutorial1RunTutorial, OnDoTutorial);
            popup.AddItem(eResource.evTutorial1SkipTutorial, base.OnClose);
        }
        */

        private void OnDoTutorial(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);
        }

        private void OnSkipTutorial(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);
            FrameworkCore.PlayCue(sounds.Music.none);
        }

    }
}