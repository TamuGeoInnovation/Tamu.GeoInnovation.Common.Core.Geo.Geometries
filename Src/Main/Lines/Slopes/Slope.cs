using System;
using USC.GISResearchLab.Common.Geometries.Points;

namespace USC.GISResearchLab.Common.Geometries.Lines.Slopes
{
    public class Slope
    {
        public const double SLOPE_HORIZONTAL = 99999.0;
        public const double SLOPE_VERTICAL = -99999.0;

        public static double getSlope(Line line)
        {
            return getSlope(line.Start, line.End);
        }

        public static double getSlope(Point from, Point to)
        {
            return getSlope(from.X, from.Y, to.X, to.Y);
        }

        public static double getSlope(double fromX, double fromY, double toX, double toY)
        {
            return
                Decimal.ToDouble(
                    getSlopeDecimal(new decimal(fromX), new decimal(fromY), new decimal(toX), new decimal(toY)));
        }

        public static decimal getSlopeDecimal(decimal fromXDec, decimal fromYDec, decimal toXDec, decimal toYDec)
        {
            decimal rise = Decimal.Subtract(toYDec, fromYDec);
            decimal run = Decimal.Subtract(toXDec, fromXDec);
            ;
            decimal slope;

            rise = Decimal.Round(rise, 12);
            run = Decimal.Round(run, 12);

            if (rise == 0)
            {
                slope = new decimal(SLOPE_HORIZONTAL);
            }
            else if (run == 0)
            {
                slope = new decimal(SLOPE_VERTICAL);
            }
            else
            {
                slope = Decimal.Divide(rise, run);
                slope = Decimal.Round(slope, 12);
            }

            return slope;
        }
    }
}