
#region Using

using Microsoft.Xna.Framework;

#endregion



namespace SpaceShooter
{
    /// <summary>
    /// The player is encountered with Space Bandits
    /// </summary>
    public class evOwlTattoo  : Event
    {
        Rectangle img = sprite.eventSprites.owl;

  
        public evOwlTattoo()
        {
            shipList = new ShipData[2] { shipTypes.BeamGunship, shipTypes.Destroyer };
            shipMinMax = Helpers.GetAdjustedEnemyFleetSize();
            faction = Faction.Owl;
            musicCue = sounds.Music.invaders;
            pilotName = eResource.nameOwlTattoo;
        }

        public override void Activate()
        {
            EventPopup popup = base.CreatePopup(this.manager, img, eResource.evOwlTattoo0);
            popup.eventName = pilotName;

            popup.AddItem(eResource.startBattle, OnFight);

            base.Activate();
        }

        private void OnFight(object sender, InputArgs e)
        {
            Helpers.CloseThisMenu(sender);

            FrameworkCore.worldMap.EnterCombat(this);

            eventManager.AddLog(img, eResource.logOwlTattoo);
        }
    }
}