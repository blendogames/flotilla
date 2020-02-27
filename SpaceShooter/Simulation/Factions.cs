
#region Using
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace SpaceShooter
{
    public class FactionInfo
    {
        string name;
        Color teamcolor;
        Color altcolor;

        public Color teamColor
        {
            get { return teamcolor; }
        }

        public Color altColor
        {
            get { return altcolor; }
        }

     
        public string Name
        {
            get { return name; }
        }

        public FactionInfo(string txt, Color TeamColor, Color AltColor)
        {
            this.name = txt;
            this.teamcolor = TeamColor;
            this.altcolor = AltColor;
        }
    }

    public static class Faction
    {
        public static FactionInfo Blue = new FactionInfo(
            Resource.FactionBlueName,
            new Color(60, 160, 255),
            new Color(130, 160, 50));

        public static FactionInfo Red = new FactionInfo(
            Resource.FactionRedName,
            new Color(220, 80, 60),
            new Color(210, 120, 20));

        public static FactionInfo Owl = new FactionInfo(
            Resource.FactionOwl,
            new Color(220, 80, 60),
            new Color(210, 120, 20));

        public static FactionInfo SecretPolice = new FactionInfo(
            Resource.FactionSecretPolice,
            new Color(190, 50, 30),
            new Color(180, 90, 0));

        public static FactionInfo Deer = new FactionInfo(
            Resource.FactionDeerName,
            new Color(128, 128, 128),
            new Color(128, 128, 128));


        public static FactionInfo Slavers = new FactionInfo(
            Resource.FactionSlavers,
            new Color(160, 110, 30),
            new Color(160, 110, 30));

        public static FactionInfo Assassins = new FactionInfo(
            Resource.FactionAssassins,
            new Color(190, 90, 210),
            new Color(190, 90, 210));


        public static FactionInfo Yetis = new FactionInfo(
            Resource.FactionYetis,
            new Color(190, 90, 210),
            new Color(190, 90, 210));


        public static FactionInfo Toucans = new FactionInfo(
            Resource.FactionToucans,
            new Color(220, 80, 60),
            new Color(210, 120, 20));

        public static FactionInfo Chickens = new FactionInfo(
            Resource.FactionChickens,
            new Color(210, 200, 60),
            new Color(210, 200, 60));

        public static FactionInfo Casino = new FactionInfo(
            Resource.FactionCasino,
            new Color(255, 130, 0),
            new Color(255, 130, 0));

        public static FactionInfo None = new FactionInfo(
            "",
            new Color(255, 130, 0),
            new Color(255, 130, 0));

        public static FactionInfo Yellow = new FactionInfo(
            "",
            new Color(255, 224, 0),
            new Color(255, 224, 0));

        public static FactionInfo Groupies = new FactionInfo(
            Resource.FactionGroupies,
            new Color(190, 90, 210),
            new Color(190, 90, 210));

        public static FactionInfo Spider = new FactionInfo(
            Resource.FactionSpider,
            new Color(255, 130, 0),
            new Color(255, 130, 0));

        public static FactionInfo Penguin = new FactionInfo(
            Resource.FactionPenguin,
            new Color(160, 160, 192),
            new Color(160, 160, 192));
        
    }
}