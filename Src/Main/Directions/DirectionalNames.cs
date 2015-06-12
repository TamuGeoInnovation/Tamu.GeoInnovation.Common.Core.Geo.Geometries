using System.Collections;
using USC.GISResearchLab.Common.Synonyms;

namespace USC.GISResearchLab.Common.Geometries.Directions
{

    public class DirectionalNames
    {

        public static string DIRECTION_NORTH = "NORTH";
        public static string DIRECTION_NTH = "NTH";
        public static string DIRECTION_NRTH = "NRTH";
        public static string DIRECTION_NORHT = "NORHT";
        public static string DIRECTION_NO = "NO";
        public static string DIRECTION_NOR = "NOR";
        public static string DIRECTION_NORT = "NORT";
        public static string DIRECTION_NORTE = "NORTE";
        public static string DIRECTION_NORTH_EAST = "NORTHEAST";
        public static string DIRECTION_NORESTE = "NORESTE";
        public static string DIRECTION_NORTH_WEST = "NORTHWEST";
        public static string DIRECTION_NOROESTE = "NOROESTE";
        public static string DIRECTION_SOUTH = "SOUTH";
        public static string DIRECTION_STH = "STH";
        public static string DIRECTION_SOU = "SOU";
        public static string DIRECTION_SOUT = "SOUT";
        public static string DIRECTION_SO = "SO";
        public static string DIRECTION_SUR = "SUR";
        public static string DIRECTION_SOUTH_EAST = "SOUTHEAST";
        public static string DIRECTION_SUDESTE = "SUDESTE";
        public static string DIRECTION_SOUTH_WEST = "SOUTHWEST";
        public static string DIRECTION_SUDOESTE = "SUDOESTE";
        public static string DIRECTION_EAST = "EAST";
        public static string DIRECTION_EST = "EST";
        public static string DIRECTION_ESTE = "ESTE";
        public static string DIRECTION_WEST = "WEST";
        public static string DIRECTION_WES = "WES";
        public static string DIRECTION_WST = "WST";
        public static string DIRECTION_OESTE = "OESTE";

        public const string ABBRV_NORTH = "N";
        public const string ABBRV_EAST_NORTH = "EN";
        public const string ABBRV_NORTH_NORTH_EAST = "NNE";
        public const string ABBRV_NORTH_EAST = "NE";
        public const string ABBRV_NORTH_EAST_EAST = "NEE";
        public const string ABBRV_NORTH_WEST = "NW";
        public const string ABBRV_NORTH_NORTH_WEST = "NNW";
        public const string ABBRV_NORTH_WEST_WEST = "NWW";
        public const string ABBRV_WEST_NORTH = "WN";
        public const string ABBRV_SOUTH = "S";
        public const string ABBRV_SOUTH_SOUTH_EAST = "SSE";
        public const string ABBRV_SOUTH_EAST = "SE";
        public const string ABBRV_SOUTH_EAST_EAST = "SEE";
        public const string ABBRV_SOUTH_WEST = "SW";
        public const string ABBRV_SOUTH_SOUTH_WEST = "SSW";
        public const string ABBRV_SOUTH_WEST_WEST = "SWW";
        public const string ABBRV_EAST = "E";
        public const string ABBRV_WEST = "W";

        public static string[] SYN_NORTH = { ABBRV_NORTH, DIRECTION_NORTH, DIRECTION_NRTH, DIRECTION_NTH, DIRECTION_NORHT, DIRECTION_NO, DIRECTION_NOR, DIRECTION_NORT, DIRECTION_NORTE };
        public static string[] SYN_NORTH_EAST = { ABBRV_NORTH_EAST, DIRECTION_NORTH_EAST, DIRECTION_NORESTE, ABBRV_EAST_NORTH };
        public static string[] SYN_NORTH_WEST = { ABBRV_NORTH_WEST, DIRECTION_NORTH_WEST, DIRECTION_NOROESTE, ABBRV_WEST_NORTH };
        public static string[] SYN_SOUTH = { ABBRV_SOUTH, DIRECTION_SOUTH, DIRECTION_STH, DIRECTION_SO, DIRECTION_SOU, DIRECTION_SOUT, DIRECTION_SUR };
        public static string[] SYN_SOUTH_EAST = { ABBRV_SOUTH_EAST, DIRECTION_SOUTH_EAST, DIRECTION_SUDESTE };
        public static string[] SYN_SOUTH_WEST = { ABBRV_SOUTH_WEST, DIRECTION_SOUTH_WEST, DIRECTION_SUDOESTE };
        public static string[] SYN_EAST = { ABBRV_EAST, DIRECTION_EAST, DIRECTION_EST, DIRECTION_ESTE };
        public static string[] SYN_WEST = { ABBRV_WEST, DIRECTION_WEST, DIRECTION_WST, DIRECTION_WES, DIRECTION_OESTE };

        public static SynonymSet SYN_NORTH_SET = new SynonymSet(SYN_NORTH);
        public static SynonymSet SYN_NORTH_EAST_SET = new SynonymSet(SYN_NORTH_EAST);
        public static SynonymSet SYN_NORTH_WEST_SET = new SynonymSet(SYN_NORTH_WEST);
        public static SynonymSet SYN_SOUTH_SET = new SynonymSet(SYN_SOUTH);
        public static SynonymSet SYN_SOUTH_EAST_SET = new SynonymSet(SYN_SOUTH_EAST);
        public static SynonymSet SYN_SOUTH_WEST_SET = new SynonymSet(SYN_SOUTH_WEST);
        public static SynonymSet SYN_EAST_SET = new SynonymSet(SYN_EAST);
        public static SynonymSet SYN_WEST_SET = new SynonymSet(SYN_WEST);

        public static ArrayList DIRECTIONAL_SYNS;



        static DirectionalNames()
        {
            DIRECTIONAL_SYNS = new ArrayList();
            DIRECTIONAL_SYNS.Add(SYN_NORTH_SET);
            DIRECTIONAL_SYNS.Add(SYN_NORTH_EAST_SET);
            DIRECTIONAL_SYNS.Add(SYN_NORTH_WEST_SET);
            DIRECTIONAL_SYNS.Add(SYN_SOUTH_SET);
            DIRECTIONAL_SYNS.Add(SYN_SOUTH_EAST_SET);
            DIRECTIONAL_SYNS.Add(SYN_SOUTH_WEST_SET);
            DIRECTIONAL_SYNS.Add(SYN_EAST_SET);
            DIRECTIONAL_SYNS.Add(SYN_WEST_SET);
        }
    }

}