using Microsoft.SqlServer.Types;
using System;
using USC.GISResearchLab.Common.Geometries.Directions;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Vectors;

namespace USC.GISResearchLab.Common.Geometries.Bearings
{
    public class Bearing
    {
        public const double BEARING_N = 0.0;
        public const double BEARING_NNE = 15.0;
        public const double BEARING_NE = 45.0;
        public const double BEARING_ENE = 60.0;
        public const double BEARING_E = 90.0;
        public const double BEARING_ESE = 105.0;
        public const double BEARING_SE = 120.0;
        public const double BEARING_SSE = 135.0;
        public const double BEARING_S = 180.0;
        public const double BEARING_W = 270.0;
        public const double BEARING_MAX = 360.0;

        // this is from http://social.msdn.microsoft.com/Forums/en-US/sqlspatial/thread/4630ef22-2525-486c-8904-68b47d1c02a3/
        public static double CalculateBearing(SqlGeography from, SqlGeography to)
        {
            double ret = 0;
            try
            {
                // make sure we're dealing with points
                if (!from.InstanceOf("POINT") || !to.InstanceOf("POINT"))
                {
                    throw new ArgumentException("Both arguments to this function must be points.");
                }

                // All in radians
                double latFrom = from.Lat.Value * (Math.PI / 180.0);
                double longFrom = from.Long.Value * (Math.PI / 180.0);
                double latTo = to.Lat.Value * (Math.PI / 180.0);
                double longTo = to.Long.Value * (Math.PI / 180.0);

                double y = Math.Sin(longTo - longFrom) * Math.Cos(latTo);
                double x = (Math.Cos(latFrom) * Math.Sin(latTo)) - (Math.Sin(latFrom) * Math.Cos(latTo) * Math.Cos(longFrom - longTo));

                double rval = Math.Atan2(y, x);

                ret = rval * (180.0 / Math.PI); // back to degrees
            }
            catch (Exception e)
            {
                throw new Exception("Exception in CalculateBearing: " + e.Message, e);
            }

            return ret;
        }

        public static CardinalDirections GetDirectionFromBearing(double bearing)
        {
            CardinalDirections ret = CardinalDirections.North;
            string[] quadrants = new string[] { "NNE", "NE", "NEE", "E", "SEE", "SE", "SSE", "S", "SSW", "SW", "SWW", "W", "NWW", "NW", "NNW" };


            if (bearing > 348.75 || bearing <= 11.25)
            {
                ret = CardinalDirections.North;
            }
            else
            {
                int q = Convert.ToInt32(Math.Floor((bearing - 11.25) / 22.5));
                string dir = quadrants[q];
                ret = CardinalDirection.GetDirectionFromName(dir);
            }

            return ret;
        }

        public static bool isWithinBearingThreshold(double bearing1, double bearing2, double threshold)
        {
            bool ret = false;
            double difference = DifferenceBetween(bearing1, bearing2);
            if (difference < threshold)
            {
                ret = true;
            }
            return ret;
        }

        public static bool isAClockWiseBearingShiftingCorner(double bearing1, double bearing2, double threshold)
        {
            bool ret = false;
            double difference = DifferenceBetween(bearing1, bearing2);
            if ((difference > (90.0 - threshold) && difference < (90.0 + threshold)))
            {
                ret = true;
            }
            return ret;
        }

        public static double DifferenceBetween(double bearing1, double bearing2)
        {
            double difference;

            if (bearing1 < bearing2)
            {
                difference = bearing2 - bearing1;
            }
            else
            {
                difference = (360 - bearing1) + bearing2;
            }

            return difference;
        }

        public static double getCounterClockWisePerpendicularBearing(double bearing)
        {
            double ret = bearing - 90;
            if (ret < 0)
            {
                ret = ret + 360;
            }
            return ret;
        }

        public static double getClockWisePerpendicularBearing(double bearing)
        {
            double ret = bearing + 90.0;
            if (ret >= 360)
            {
                ret = ret - 360;
            }
            return ret;
        }

        public static double getBearing(Line l)
        {
            return getBearing(l.Direction, l.Start.X, l.Start.Y, l.End.X, l.End.Y);
        }

        public static double getBearing(int direction, double fromX, double fromY, double toX, double toY)
        {
            double ret = -1;

            Vector thisVector = new Vector(fromX, fromY, toX, toY);

            switch (direction)
            {
                case CardinalDirection.DIR_N:
                    ret = BEARING_N;
                    break;
                case CardinalDirection.DIR_S:
                    ret = BEARING_S;
                    break;
                case CardinalDirection.DIR_E:
                    ret = BEARING_E;
                    break;
                case CardinalDirection.DIR_W:
                    ret = BEARING_W;
                    break;
                case CardinalDirection.DIR_NE:
                    Vector trueNorth = new Vector(fromX, fromY, fromX, fromY + 15.0);
                    ret = Vector.AngleBetween(trueNorth, thisVector);
                    break;
                case CardinalDirection.DIR_SE:
                    Vector trueEast = new Vector(fromX, fromY, fromX + 15, fromY);
                    ret = Vector.AngleBetween(trueEast, thisVector);
                    ret += 90;
                    break;
                case CardinalDirection.DIR_SW:
                    Vector trueSouth = new Vector(fromX, fromY, fromX, fromY - 15);
                    ret = Vector.AngleBetween(trueSouth, thisVector);
                    ret += 180;
                    break;
                case CardinalDirection.DIR_NW:
                    Vector trueWest = new Vector(fromX, fromY, fromX - 15, fromY);
                    ret = Vector.AngleBetween(trueWest, thisVector);
                    ret += 270.0;
                    break;

                default:
                    break;
            }


            return ret;
        }

        public static double getDropBackBearing(Line l)
        {
            double ret;
            if (l.IsReversed)
            {
                ret = getCounterClockWisePerpendicularBearing(l.Bearing);
            }
            else
            {
                ret = getClockWisePerpendicularBearing(l.Bearing);
            }
            return ret;
        }
    }
}