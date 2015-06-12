using System;
using System.Collections.Generic;
using System.Data;
using USC.GISResearchLab.Common.Geometries.Hands;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Lines.Slopes;

namespace USC.GISResearchLab.Common.Geometries.Directions
{
    public enum CardinalDirections { 
        Uknown, 
        North, 
        NortNorthEast, 
        NorthEast, 
        NorthEastEast, 
        East, 
        SouthEastEast, 
        SouthEast, 
        SouthSouthEast, 
        South, 
        SouthSouthWest, 
        SouthWest, 
        SouthWestWest,      
        West,
        NorthWestWest,
        NorthWest,
        NorthNorthWest,
    };

    public class CardinalDirection
    {
        public const int DIR_E = 7;
        public const int DIR_N = 5;
        public const int DIR_NE = 1;
        public const int DIR_NW = 2;
        public const int DIR_S = 6;
        public const int DIR_SE = 4;
        public const int DIR_SW = 3;
        public const int DIR_W = 8;


        public static DataTable GetAllCardinalDirections()
        {
            DataTable ret = new DataTable();
            ret.Columns.Add(new DataColumn("value", typeof(string)));

            foreach (CardinalDirections item in Enum.GetValues(typeof(CardinalDirections)))
            {
                DataRow row = ret.NewRow();
                row["value"] = item.ToString();
                ret.Rows.Add(row);
            }

            return ret;
        }

        public static CardinalDirections GetCardinalDirection(Line l)
        {
            int direction = getDirection(l.Start.X, l.Start.Y, l.End.X, l.End.Y);
            return GetCardinalDirectionFromInt(direction);
        }

        public static int getDirection(Line l)
        {
            return getDirection(l.Start.X, l.Start.Y, l.End.X, l.End.Y);
        }

        public static CardinalDirections GetCardinalDirection(double fromX, double fromY, double toX, double toY)
        {
            int direction = getDirection(fromX, fromY, toX, toY);
            return GetCardinalDirectionFromInt(direction);
        }

        public static int getDirection(double fromX, double fromY, double toX, double toY)
        {
            int ret = -1;

            double slope = Slope.getSlope(fromX, fromY, toX, toY);
            // horizontal
            if (slope == Slope.SLOPE_HORIZONTAL)
            {
                if (fromX < toX)
                {
                    ret = DIR_E;
                }
                else
                {
                    ret = DIR_W;
                }
            }
                // veriticle
            else if (slope == Slope.SLOPE_VERTICAL)
            {
                if (fromY < toY)
                {
                    ret = DIR_N;
                }
                else
                {
                    ret = DIR_S;
                }
            }
            else if (slope < 0)
            {
                if (fromX < toX)
                {
                    ret = DIR_SE;
                }
                else
                {
                    ret = DIR_NW;
                }
            }
            else if (slope > 0)
            {
                if (fromX < toX)
                {
                    ret = DIR_NE;
                }
                else
                {
                    ret = DIR_SW;
                }
            }
            return ret;
        }

        public static CardinalDirections GetCardinalDirectionFromInt(int direction)
        {
            CardinalDirections ret;

            switch (direction)
            {
                case DIR_NE:
                    ret = CardinalDirections.NorthEast;
                    break;
                case DIR_NW:
                    ret = CardinalDirections.NorthWest;
                    break;
                case DIR_SW:
                    ret = CardinalDirections.SouthWest;
                    break;
                case DIR_SE:
                    ret = CardinalDirections.SouthEast;
                    break;
                case DIR_N:
                    ret = CardinalDirections.North;
                    break;
                case DIR_S:
                    ret = CardinalDirections.South;
                    break;
                case DIR_E:
                    ret = CardinalDirections.East;
                    break;
                case DIR_W:
                    ret = CardinalDirections.West;
                    break;
                default:
                    ret = CardinalDirections.Uknown;
                    break;
            }
            return ret;
        }

        public static int GetDirectionFromCardinalDirection(CardinalDirections direction)
        {
            int ret = -1;

            switch (direction)
            {
                case CardinalDirections.NorthEast:
                    ret = DIR_NE;
                    break;
                case CardinalDirections.NorthWest:
                    ret = DIR_NW;
                    break;
                case CardinalDirections.SouthWest:
                    ret = DIR_SW;
                    break;
                case CardinalDirections.SouthEast:
                    ret = DIR_SE;
                    break;
                case CardinalDirections.North:
                    ret = DIR_N;
                    break;
                case CardinalDirections.South:
                    ret = DIR_S;
                    break;
                case CardinalDirections.East:
                    ret = DIR_E;
                    break;
                case CardinalDirections.West:
                    ret = DIR_W;
                    break;
                default:
                    ret = -1;
                    break;
            }
            return ret;
        }

        public static int rotate90DegreesClockWise(int direction)
        {
            int ret;

            switch (direction)
            {
                case DIR_NE:
                    ret = DIR_SE;
                    break;
                case DIR_NW:
                    ret = DIR_NE;
                    break;
                case DIR_SW:
                    ret = DIR_NW;
                    break;
                case DIR_SE:
                    ret = DIR_SW;
                    break;
                case DIR_N:
                    ret = DIR_E;
                    break;
                case DIR_S:
                    ret = DIR_W;
                    break;
                case DIR_E:
                    ret = DIR_S;
                    break;
                case DIR_W:
                    ret = DIR_N;
                    break;
                default:
                    ret = -1;
                    break;
            }
            return ret;
        }

        public static bool isAClockWiseCombination(int vectorDirection, int perpDirection)
        {
            bool ret;

            switch (vectorDirection)
            {
                case DIR_NE:
                    ret = (perpDirection == DIR_SE);
                    break;
                case DIR_NW:
                    ret = (perpDirection == DIR_NE);
                    break;
                case DIR_SW:
                    ret = (perpDirection == DIR_NW);
                    break;
                case DIR_SE:
                    ret = (perpDirection == DIR_SW);
                    break;
                case DIR_N:
                    ret = (perpDirection == DIR_E);
                    break;
                case DIR_S:
                    ret = (perpDirection == DIR_W);
                    break;
                case DIR_E:
                    ret = (perpDirection == DIR_S);
                    break;
                case DIR_W:
                    ret = (perpDirection == DIR_N);
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        public static string getDirectionName(int directionValue)
        {
            string ret;
            switch (directionValue)
            {
                case DIR_NE:
                    ret = "DIR_NE";
                    break;
                case DIR_NW:
                    ret = "DIR_NW";
                    break;
                case DIR_SW:
                    ret = "DIR_SW";
                    break;
                case DIR_SE:
                    ret = "DIR_SE";
                    break;
                case DIR_N:
                    ret = "DIR_N";
                    break;
                case DIR_S:
                    ret = "DIR_S";
                    break;
                case DIR_E:
                    ret = "DIR_E";
                    break;
                case DIR_W:
                    ret = "DIR_W";
                    break;
                default:
                    ret = "UNKNOWN DIRECTION: " + directionValue;
                    break;
            }
            return ret;
        }

        public static string GetDirectionName(CardinalDirections dir)
        {
            string ret;
            switch (dir)
            {
                case CardinalDirections.North:
                    ret = DirectionalNames.ABBRV_NORTH;
                    break;
                case CardinalDirections.NortNorthEast:
                    ret = DirectionalNames.ABBRV_NORTH_NORTH_EAST;
                    break;
                case CardinalDirections.NorthEast:
                    ret = DirectionalNames.ABBRV_NORTH_EAST;
                    break;
                case CardinalDirections.NorthEastEast:
                    ret = DirectionalNames.ABBRV_NORTH_EAST_EAST;
                    break;
                case CardinalDirections.East:
                    ret = DirectionalNames.ABBRV_EAST;
                    break;
                case CardinalDirections.SouthEastEast:
                    ret = DirectionalNames.ABBRV_SOUTH_EAST_EAST;
                    break;
                case CardinalDirections.SouthEast:
                    ret = DirectionalNames.ABBRV_SOUTH_EAST;
                    break;
                case CardinalDirections.SouthSouthEast:
                    ret = DirectionalNames.ABBRV_SOUTH_SOUTH_EAST;
                    break;
                case CardinalDirections.South:
                    ret = DirectionalNames.ABBRV_SOUTH;
                    break;
                case CardinalDirections.SouthSouthWest:
                    ret = DirectionalNames.ABBRV_SOUTH_SOUTH_WEST;
                    break;
                case CardinalDirections.SouthWest:
                    ret = DirectionalNames.ABBRV_SOUTH_WEST;
                    break;
                case CardinalDirections.SouthWestWest:
                    ret = DirectionalNames.ABBRV_SOUTH_WEST_WEST;
                    break;
                case CardinalDirections.West:
                    ret = DirectionalNames.ABBRV_WEST;
                    break;
                case CardinalDirections.NorthWestWest:
                    ret = DirectionalNames.ABBRV_NORTH_WEST_WEST;
                    break;
                case CardinalDirections.NorthWest:
                    ret = DirectionalNames.ABBRV_NORTH_WEST;
                    break;
                case CardinalDirections.NorthNorthWest:
                    ret = DirectionalNames.ABBRV_NORTH_NORTH_WEST;
                    break;

                default:
                    ret = "Unexpected or unimplemented CardinalDirections: " + dir;
                    break;
            }
            return ret;
        }

        public static CardinalDirections GetDirectionFromName(string name)
        {
            CardinalDirections ret;
            switch (name.ToUpper())
            {
                case DirectionalNames.ABBRV_NORTH:
                    ret = CardinalDirections.North;
                    break;
                case DirectionalNames.ABBRV_NORTH_NORTH_EAST:
                    ret = CardinalDirections.NortNorthEast;
                    break;
                case DirectionalNames.ABBRV_NORTH_EAST:
                    ret = CardinalDirections.NorthEast;
                    break;
                case DirectionalNames.ABBRV_NORTH_EAST_EAST:
                    ret = CardinalDirections.NorthEastEast;
                    break;
                case DirectionalNames.ABBRV_EAST:
                    ret = CardinalDirections.East;
                    break;
                case DirectionalNames.ABBRV_SOUTH_EAST_EAST:
                    ret = CardinalDirections.SouthEastEast;
                    break;
                case DirectionalNames.ABBRV_SOUTH_EAST:
                    ret = CardinalDirections.SouthEast;
                    break;
                case DirectionalNames.ABBRV_SOUTH_SOUTH_EAST:
                    ret = CardinalDirections.SouthSouthEast;
                    break;
                case DirectionalNames.ABBRV_SOUTH:
                    ret = CardinalDirections.South;
                    break;
                case DirectionalNames.ABBRV_SOUTH_SOUTH_WEST:
                    ret = CardinalDirections.SouthSouthWest;
                    break;
                case DirectionalNames.ABBRV_SOUTH_WEST:
                    ret = CardinalDirections.SouthWest;
                    break;
                case DirectionalNames.ABBRV_SOUTH_WEST_WEST:
                    ret = CardinalDirections.SouthWestWest;
                    break;
                case DirectionalNames.ABBRV_WEST:
                    ret = CardinalDirections.West;
                    break;
                case DirectionalNames.ABBRV_NORTH_WEST_WEST:
                    ret = CardinalDirections.NorthWestWest;
                    break;
                case DirectionalNames.ABBRV_NORTH_WEST:
                    ret = CardinalDirections.NorthWest;
                    break;
                case DirectionalNames.ABBRV_NORTH_NORTH_WEST:
                    ret = CardinalDirections.NorthNorthWest;
                    break;

                default:
                    throw new Exception("Unexpected or unimplemented CardinalDirection nam: " + name);
            }
            return ret;
        }

        public static List<CardinalDirections> GetDirectionComponents(CardinalDirections direction)
        {
            List<CardinalDirections> ret = new List<CardinalDirections>();
            switch (direction)
            {
                case CardinalDirections.North:
                    ret.Add(CardinalDirections.North);
                    break;
                case CardinalDirections.NortNorthEast:
                    ret.Add(CardinalDirections.North);
                    ret.Add(CardinalDirections.East);
                    break;
                case CardinalDirections.NorthEast:
                    ret.Add(CardinalDirections.North);
                    ret.Add(CardinalDirections.East);
                    break;
                case CardinalDirections.NorthEastEast:
                    ret.Add(CardinalDirections.North);
                    ret.Add(CardinalDirections.East);
                    break;
                case CardinalDirections.East:
                    ret.Add(CardinalDirections.East);
                    break;
                case CardinalDirections.SouthEastEast:
                    ret.Add(CardinalDirections.South);
                    ret.Add(CardinalDirections.East);
                    break;
                case CardinalDirections.SouthEast:
                    ret.Add(CardinalDirections.South);
                    ret.Add(CardinalDirections.East);
                    break;
                case CardinalDirections.SouthSouthEast:
                    ret.Add(CardinalDirections.South);
                    ret.Add(CardinalDirections.East);
                    break;
                case CardinalDirections.South:
                    ret.Add(CardinalDirections.South);
                    break;
                case CardinalDirections.SouthSouthWest:
                    ret.Add(CardinalDirections.South);
                    ret.Add(CardinalDirections.West);
                    break;
                case CardinalDirections.SouthWest:
                    ret.Add(CardinalDirections.South);
                    ret.Add(CardinalDirections.West);
                    break;
                case CardinalDirections.SouthWestWest:
                    ret.Add(CardinalDirections.South);
                    ret.Add(CardinalDirections.West);
                    break;
                case CardinalDirections.West:
                    ret.Add(CardinalDirections.West);
                    break;
                case CardinalDirections.NorthWestWest:
                    ret.Add(CardinalDirections.North);
                    ret.Add(CardinalDirections.West);
                    break;
                case CardinalDirections.NorthWest:
                    ret.Add(CardinalDirections.North);
                    ret.Add(CardinalDirections.West);
                    break;
                case CardinalDirections.NorthNorthWest:
                    ret.Add(CardinalDirections.North);
                    ret.Add(CardinalDirections.West);
                    break;

                default:
                    throw new Exception ("Unexpected or unimplemented CardinalDirections: " + direction);
                    break;
            }
            return ret;
        }

        public static int getDirectionValue(string directionName)
        {
            int ret = -1;
            if (directionName.ToUpper().Equals("DIR_NE"))
            {
                ret = DIR_NE;
            }
            else if (directionName.ToUpper().Equals("DIR_NW"))
            {
                ret = DIR_NW;
            }
            else if (directionName.ToUpper().Equals("DIR_SW"))
            {
                ret = DIR_SW;
            }
            else if (directionName.ToUpper().Equals("DIR_SE"))
            {
                ret = DIR_SE;
            }
            else if (directionName.ToUpper().Equals("DIR_N"))
            {
                ret = DIR_N;
            }
            else if (directionName.ToUpper().Equals("DIR_S"))
            {
                ret = DIR_S;
            }
            else if (directionName.ToUpper().Equals("DIR_E"))
            {
                ret = DIR_E;
            }
            else if (directionName.ToUpper().Equals("DIR_W"))
            {
                ret = DIR_W;
            }

            return ret;
        }

        public static CardinalDirections GetPerpendicularCardinalDirection(string direction, HandValues hand)
        {
            int handDirection = Hand.GetHandIntFromHandValue(hand);
            int perpDir = getPerpendicularDirection(getDirectionValue(direction), handDirection);
            return GetCardinalDirectionFromInt(perpDir);
        }

        public static CardinalDirections GetPerpendicularCardinalDirection(CardinalDirections cardinalDirection, HandValues hand)
        {
            int direction = GetDirectionFromCardinalDirection(cardinalDirection);
            int handDirection = Hand.GetHandIntFromHandValue(hand);
            int perpDir = getPerpendicularDirection(direction, handDirection);
            return GetCardinalDirectionFromInt(perpDir);
        }

        public static int getPerpendicularDirection(string direction, int hand)
        {
            return getPerpendicularDirection(getDirectionValue(direction), hand);
        }


        public static int getPerpendicularDirection(int direction, int hand)
        {
            int ret = -1;

            switch (direction)
            {
                case DIR_NE:
                    if (hand == Hand.HAND_LEFT)
                    {
                        ret = DIR_NW;
                    }
                    else
                    {
                        ret = DIR_SE;
                    }
                    break;
                case DIR_SE:
                    if (hand == Hand.HAND_LEFT)
                    {
                        ret = DIR_NE;
                    }
                    else
                    {
                        ret = DIR_SW;
                    }
                    break;
                case DIR_SW:
                    if (hand == Hand.HAND_LEFT)
                    {
                        ret = DIR_SE;
                    }
                    else
                    {
                        ret = DIR_NW;
                    }
                    break;
                case DIR_NW:
                    if (hand == Hand.HAND_LEFT)
                    {
                        ret = DIR_SW;
                    }
                    else
                    {
                        ret = DIR_NE;
                    }
                    break;
                case DIR_N:
                    if (hand == Hand.HAND_LEFT)
                    {
                        ret = DIR_W;
                    }
                    else
                    {
                        ret = DIR_E;
                    }
                    break;
                case DIR_S:
                    if (hand == Hand.HAND_LEFT)
                    {
                        ret = DIR_E;
                    }
                    else
                    {
                        ret = DIR_W;
                    }
                    break;
                case DIR_E:
                    if (hand == Hand.HAND_LEFT)
                    {
                        ret = DIR_N;
                    }
                    else
                    {
                        ret = DIR_S;
                    }
                    break;
                case DIR_W:
                    if (hand == Hand.HAND_LEFT)
                    {
                        ret = DIR_S;
                    }
                    else
                    {
                        ret = DIR_N;
                    }
                    break;
                default:
                    break;
            }

            return ret;
        }

        public static CardinalDirections calculateDropBackCardinalDirection(Line line, bool IsLeftSide)
        {
            int dropbackDirection = calculateDropBackDirection(line.FromY, line.FromX, line.ToY, line.ToX, IsLeftSide);
            return GetCardinalDirectionFromInt(dropbackDirection);
        }

        public static int calculateDropBackDirection(Line line, bool IsLeftSide)
        {
            return calculateDropBackDirection(line.FromY, line.FromX, line.ToY, line.ToX, IsLeftSide);
        }

        public static int calculateDropBackDirection(double fromLat, double fromLon, double toLat, double toLon, bool IsLeftSide)
        {

            int hand;
            if (IsLeftSide)
            {
                hand = Hand.HAND_LEFT;
            }
            else
            {
                hand = Hand.HAND_RIGHT;
            }

            return getPerpendicularDirection(getDirection(fromLon, fromLat, toLon, toLat), hand);
        }
    }
}