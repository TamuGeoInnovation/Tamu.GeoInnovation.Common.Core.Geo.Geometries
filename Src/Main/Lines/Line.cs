using System;
using Microsoft.SqlServer.Types;
using SQLSpatialTools;
using USC.GISResearchLab.Common.Exceptions.Geocoding.Calculations;
using USC.GISResearchLab.Common.Geometries.BoundingBoxes;
using USC.GISResearchLab.Common.Geometries.Directions;
using USC.GISResearchLab.Common.Geometries.Hands;
using USC.GISResearchLab.Common.Geometries.Lines.Slopes;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Common.Geometries.Vectors;

namespace USC.GISResearchLab.Common.Geometries.Lines
{
    /// <summary>
    /// Summary description for Segment.
    /// </summary>
    public class Line : Vector, ICloneable
    {
        #region sortOrder enum

        public enum sortOrder
        {
            sortAsc,
            sortDesc
        }
        #endregion


        #region Properties

        public Point Start { get; set; }
        public Point End { get; set; }

        public int Direction
        {
            get { return CardinalDirection.getDirection(this); }
        }

        public string DirectionString
        {
            get { return CardinalDirection.getDirectionName(Direction); }
        }

        public double Bearing
        {
            get { return Bearings.Bearing.getBearing(this); }
        }

        public bool IsReversed { get; set; }



        public new double FromX
        {
            get { return Start.X; }
        }

        public new double FromY
        {
            get { return Start.Y; }
        }

        public new double ToX
        {
            get { return End.X; }
        }

        public new double ToY
        {
            get { return End.Y; }
        }

        public new double DeltaX
        {
            get { return ((Vector) this).DeltaX; }
        }

        public new double DeltaY
        {
            get { return ((Vector) this).DeltaY; }
        }

        public new double Length
        {
            get { return ((Vector) this).Length; }
        }

        public override bool Valid
        {
            get
            {
                return (Start!=null && End!= null && Start.Valid && End.Valid);
            }
        }

        #endregion

        
        public Line()
        {
            GeometryType = GeometryType.Line;
        }

        public Line(double fromX, double fromY, double toX, double toY)
            : base(fromX, fromY, toX, toY)
        {
            GeometryType = GeometryType.Line;
            Start = new Point(fromX, fromY); ;
            End = new Point(toX, toY);
        }

        public Line(Point start, Point end)
            : base(start, end)
        {
            GeometryType = GeometryType.Line;
            Start = start;
            End = end;
        }

        public string getCoodinateString()
        {
            string ret = "";
            if (Start != null && End != null)
            {
                ret += Start.X + " " + Start.Y;
                ret += ",";
                ret += End.X + " " + End.Y;
            }
            return ret;
        }


        public bool Equals(Line l2)
        {
            bool ret = false;
            if (Start.Equals(l2.Start) && End.Equals(l2.End))
            {
                ret = true;
            }
            return ret;
        }

        public static Line getClockwiseCornerSegment(Line reference, Line[] canidiateSegments)
        {
            Line ret = null;

            for (int i = 0; i < canidiateSegments.Length; i++)
            {
                Line candidate = (Line) canidiateSegments[i];
                if (!reference.Equals(candidate))
                {
                    bool isACorner = isAClockWiseCorner(reference, candidate);

                    if (isACorner)
                    {
                        ret = candidate;
                    }
                }
            }

            return ret;
        }

        public static bool isAClockWiseCorner(Line line1, Line line2)
        {
            bool ret = false;

            double angle = AngleBetween(line1, line2);

            // if the angle is less than 120 we have a corner of some sort
            if (angle > 0 && angle < 120)
            {
                if (Bearings.Bearing.isAClockWiseBearingShiftingCorner(line1.Bearing, line2.Bearing, 15.0))
                {
                    ret = true;
                }
            }
            return ret;
        }

        public static double AngleBetween(Line line1, Line line2)
        {
            return
                AngleBetween(line1.Start.X, line1.Start.Y, line1.End.X, line1.End.Y, line2.Start.X, line2.Start.Y,
                             line2.End.X, line2.End.Y);
        }

        public static double AngleBetween(double from1X, double from1Y, double to1X, double to1Y, double from2X,
                                          double from2Y, double to2X, double to2Y)
        {
            double ret;

            // create some points to determine if they intersect and what the common origin is
            Point from1 = new Point(from1Y, from1X);
            Point to1 = new Point(to1Y, to1X);
            Point from2 = new Point(from2Y, from2X);
            Point to2 = new Point(to2Y, to2X);

            Point commonFrom = null;
            Point toFirst = null;
            Point toSecond = null;

            if (from1.Equals(from2))
            {
                commonFrom = from1;
                toFirst = to1;
                toSecond = to2;
            }
            else if (from1.Equals(to2))
            {
                commonFrom = from1;
                toFirst = to1;
                toSecond = from2;
            }
            else if (to1.Equals(from2))
            {
                commonFrom = to1;
                toFirst = from1;
                toSecond = to2;
            }
            else if (to1.Equals(to2))
            {
                commonFrom = to1;
                toFirst = from1;
                toSecond = from2;
            }

            // we found a match between the segments
            if (commonFrom != null)
            {
                Vector v1 = new Vector(commonFrom, toFirst);
                Vector v2 = new Vector(commonFrom, toSecond);

                ret = AngleBetween(v1, v2);
            }
            else
            {
                throw new Exception("Angle between lines error: The lines do not have common endpoints");
            }

            return ret;
        }

        public Point Interpolate(double bucketNumber, double numberOfBuckets)
        {
            double bucketPercentage = (bucketNumber / numberOfBuckets);
            return Interpolate(bucketPercentage);
        }

        public Point Interpolate(double percentage)
        {
            
            double xVal = 0;
            double yVal = 0;

            if (SqlGeometry != null && !SqlGeometry.IsNull)
            {
                double geomLength = SqlGeometry.STLength().Value;
                double lengthAlong = geomLength * percentage;
                SqlGeometry pointAlong = SQLSpatialToolsFunctions.LocateAlongGeom(SqlGeometry, lengthAlong);

                if (pointAlong != null && !pointAlong.IsNull)
                {
                    xVal = pointAlong.STX.Value;
                    yVal = pointAlong.STY.Value;
                }

            }
            else
            {
                xVal = Start.X + (percentage * DeltaX);
                yVal = Start.Y + (percentage * DeltaY);
            }
            return new Point(xVal, yVal);
        }

        public BoundingBox GetBoundingBoxFromDropBack(double dropBackValue, bool IsLeftSide)
        {
            BoundingBox ret = new BoundingBox();
            ret.CoordinateUnits = this.CoordinateUnits;
            try
            {
                ret.Expand(Start);
                ret.Expand(End);

                Point interpolatedStartPoint = Interpolate(10);
                Point interpolatedEndPoint = Interpolate(90);

                Point startDropback = new Point(calculateDroppedBack(interpolatedStartPoint, dropBackValue, IsLeftSide));
                Point endDropback = new Point(calculateDroppedBack(interpolatedEndPoint, dropBackValue, IsLeftSide));

                ret.Expand(startDropback);
                ret.Expand(endDropback);

            }
            catch (Exception e)
            {
                throw new Exception("Error occurred calculating GetBoundingBoxFromDropBack: " + e.Message, e);
            }
            return ret;
        }

        public BoundingBox GetBoundingBoxFromBuffer(double bufferValue)
        {
            BoundingBox ret = new BoundingBox();
            ret.CoordinateUnits = this.CoordinateUnits;
            try
            {

                Point bufferStartLeft = new Point(Start.X - bufferValue, Start.Y);
                Point bufferStartRight = new Point(Start.X + bufferValue, Start.Y);
                Point bufferStartUp = new Point(Start.X, Start.Y + bufferValue);
                Point bufferStartDown = new Point(Start.X, Start.Y - bufferValue);

                Point bufferEndLeft = new Point(End.X - bufferValue, End.Y);
                Point bufferEndRight = new Point(End.X + bufferValue, End.Y);
                Point bufferEndUp = new Point(End.X, End.Y + bufferValue);
                Point bufferEndDown = new Point(End.X, End.Y - bufferValue);

                ret.Expand(bufferStartLeft);
                ret.Expand(bufferStartRight);
                ret.Expand(bufferStartUp);
                ret.Expand(bufferStartDown);

                ret.Expand(bufferEndLeft);
                ret.Expand(bufferEndRight);
                ret.Expand(bufferEndUp);
                ret.Expand(bufferEndDown);


            }
            catch (Exception e)
            {
                throw new Exception("Error occurred calculating GetBoundingBoxFromBuffer: " + e.Message, e);
            }
            return ret;
        }


        public double[] calculateDroppedBack(Point point, double dropBackValue, bool IsLeftSide)
        {

            /*
             *  retValues[0] = dropped back longitude
             *  retValues[1] = dropped back latgitude
             * 
             */



            double[] retValues = new double[2];

            try
            {

                if (dropBackValue == 0)
                {
                    retValues[0] = point.X;
                    retValues[1] = point.Y;
                }
                else
                {

                    double tolat = End.Y;
                    double tolon = End.X;
                    double frlat = Start.Y;
                    double frlon = Start.X;

                    //bool isWestern = street.IsWesternHemisphere;
                    //bool isNorthern = street.IsNorthernHemisphere;

                    double interpolatedLat = point.Y;
                    double interpolatedLon = point.X;

                    double droppedBackLat = 0.0;
                    double droppedBackLon = 0.0;

                    HandValues hand;
                    if (IsLeftSide)
                    {
                        hand = HandValues.left;
                    }
                    else
                    {
                        hand = HandValues.right;
                    }

                    // street is a single point case - can not get direction of street to determine left/right signs, so use interpolated result only
                    if ((tolon == frlon) && (tolat == frlat))
                    {
                        droppedBackLat = interpolatedLat;
                        droppedBackLon = interpolatedLon;
                    }

                    //vertical line cases (assume that a vertical change of less than 30 feet is vertical for simplicity)
                    else if ((tolon == frlon))//|| (Math.Abs(ProjectionUtils.decimalDegrees2Meters(Math.Abs(frlon - tolon), frlat)) < 10))
                    {
                        droppedBackLat = interpolatedLat;

                        // northward heading street
                        if (tolat > frlat)
                        {


                            if (IsLeftSide)
                            {
                                /*
                                 *		^
                                 *		|
                                 *		|
                                 *	X<--|
                                 *		|
                                 *		|
                                 * 
                                 */

                                droppedBackLon = interpolatedLon - dropBackValue;
                            }
                            else
                            {
                                /*
                                 *		^
                                 *		|
                                 *		|
                                 *		|-->X
                                 *		|
                                 *		|
                                 * 
                                 */
                                droppedBackLon = interpolatedLon + dropBackValue;
                            }
                        }

                        // southward heading street
                        else if (tolat < frlat)
                        {
                            if (IsLeftSide)
                            {
                                /*
                                 *		|
                                 *		|
                                 *		|
                                 *		|-->X
                                 *		|
                                 *		|
                                 *		V
                                 */
                                droppedBackLon = interpolatedLon + dropBackValue;
                            }
                            else
                            {
                                /*
                                 *		|
                                 *		|
                                 *		|
                                 *	X<--|
                                 *		|
                                 *		|
                                 *		V
                                 */
                                droppedBackLon = interpolatedLon - dropBackValue;
                            }
                        }

                    }
                    //horizontal line cases (assume that a horizontal change of less than 30 feet is horizontal for simplicity)
                    else if ((tolat == frlat))//|| (Math.Abs(ProjectionUtils.decimalDegrees2Meters(Math.Abs(frlat - tolat), frlat)) < 10))
                    {
                        droppedBackLon = interpolatedLon;

                        // eastern heading street
                        if (tolon > frlon)
                        {
                            if (IsLeftSide)
                            {
                                /*			X
                                 *			^
                                 *			|
                                 *  ------------->
                                 * 
                                 */

                                droppedBackLat = interpolatedLat + dropBackValue;
                            }
                            else
                            {
                                /*			
                                 *  ------------->
                                 *		|
                                 *		V
                                 *		X
                                 * 
                                 */
                                droppedBackLat = interpolatedLat - dropBackValue;
                            }
                        }

                            // western heading street
                        else if (tolon < frlon)
                        {
                            if (IsLeftSide)
                            {
                                /*			
                                 *  <-------------
                                 *		|
                                 *		V
                                 *		X
                                 * 
                                 */
                                droppedBackLat = interpolatedLat - dropBackValue;
                            }
                            else
                            {
                                /*			X
                                 *			^
                                 *			|
                                 *  <-------------
                                 * 
                                 */
                                droppedBackLat = interpolatedLat + dropBackValue;
                            }
                        }
                    }

                    //now worry about doing slope
                    else if ((tolat != frlat) && (tolon != frlon))
                    {


                        decimal dSquared = new decimal(dropBackValue);
                        dSquared = Decimal.Multiply(dSquared, dSquared);

                        decimal x1 = new decimal(point.X);
                        decimal y1 = new decimal(point.Y);

                        decimal x3 = new decimal(End.X);
                        decimal y3 = new decimal(End.Y);

                        //decimal d1 = new decimal(dropBackValue);

                        //decimal d1Temp1 = Decimal.Subtract(x1, x3);
                        //d1Temp1 = Decimal.Multiply(d1Temp1, d1Temp1);
                        //decimal d1Temp2 = Decimal.Subtract(y1, y3);
                        //d1Temp2 = Decimal.Multiply(d1Temp2, d1Temp2);
                        //decimal d1Temp3 = Decimal.Add(d1Temp1, d1Temp2);
                        //double d1Temp4 = Math.Sqrt(Decimal.ToDouble(d1Temp3));
                        //decimal d2 = new decimal(d1Temp4);

                        decimal slopeRoad = Slope.getSlopeDecimal(x1, y1, x3, y3);
                        decimal slopeDropBack = Decimal.Negate(Decimal.Divide(new decimal(1.0), slopeRoad));

                        decimal slopeDropBackSquared = Decimal.Multiply(slopeDropBack, slopeDropBack);
                        decimal x1Squared = Decimal.Multiply(x1, x1);

                        decimal a = Decimal.Add(new decimal(1.0), slopeDropBackSquared);

                        decimal bTemp1 = Decimal.Multiply(new decimal(2), x1);
                        bTemp1 = Decimal.Negate(bTemp1);
                        decimal bTemp2 = Decimal.Multiply(new decimal(2), slopeDropBackSquared);
                        bTemp2 = Decimal.Multiply(bTemp2, x1);
                        decimal b = Decimal.Subtract(bTemp1, bTemp2);


                        decimal bSquared = Decimal.Multiply(b, b);

                        decimal c = Decimal.Multiply(slopeDropBackSquared, x1Squared);

                        c = Decimal.Add(x1Squared, c);
                        c = Decimal.Subtract(c, dSquared);

                        decimal sqrtTemp1 = Decimal.Multiply(new decimal(4), a);
                        sqrtTemp1 = Decimal.Multiply(sqrtTemp1, c);
                        sqrtTemp1 = Decimal.Subtract(bSquared, sqrtTemp1);
                        double sqrtTemp2 = Math.Sqrt(Decimal.ToDouble(sqrtTemp1));
                        decimal sqrtTemp3 = new decimal(sqrtTemp2);


                        decimal x2_1 = Decimal.Add(Decimal.Negate(b), sqrtTemp3);
                        x2_1 = Decimal.Divide(x2_1, Decimal.Multiply(new decimal(2), a));
                        decimal x2_2 = Decimal.Subtract(Decimal.Negate(b), sqrtTemp3);
                        x2_2 = Decimal.Divide(x2_2, Decimal.Multiply(new decimal(2), a));

                        decimal y2_1 = Decimal.Add(Decimal.Multiply(slopeDropBack, Decimal.Subtract(x2_1, x1)), y1);
                        decimal y2_2 = Decimal.Add(Decimal.Multiply(slopeDropBack, Decimal.Subtract(x2_2, x1)), y1);

                        decimal newLon1 = Decimal.Round(x2_1, 13);
                        decimal newLat1 = Decimal.Round(y2_1, 13);

                        decimal newLon2 = Decimal.Round(x2_2, 13);
                        decimal newLat2 = Decimal.Round(y2_2, 13);

                        CardinalDirections perpendicularDirection = CardinalDirection.GetPerpendicularCardinalDirection(DirectionString, hand);

                        CardinalDirections tempDirection = CardinalDirection.GetCardinalDirection(point.X, point.Y, Decimal.ToDouble(newLon1), Decimal.ToDouble(newLat1));
                        CardinalDirections tempDirection2 = CardinalDirection.GetCardinalDirection(point.X, point.Y, Decimal.ToDouble(newLon2), Decimal.ToDouble(newLat2));

                        // check which solution to the quadratic (x,y point) is in the expected direction
                        if (tempDirection == perpendicularDirection)
                        {
                            droppedBackLat = Decimal.ToDouble(newLat1);
                            droppedBackLon = Decimal.ToDouble(newLon1);
                        }
                        else if (tempDirection2 == perpendicularDirection)
                        {
                            droppedBackLat = Decimal.ToDouble(newLat2);
                            droppedBackLon = Decimal.ToDouble(newLon2);
                        }
                        else
                        {
                            string msg = "";
                            msg += "Expected direction of dropback: " + CardinalDirection.GetDirectionName(perpendicularDirection) + " - ";
                            msg += "Calculated directions: " + CardinalDirection.GetDirectionName(tempDirection) + " & " + CardinalDirection.GetDirectionName(tempDirection2);
                            msg += " : " + " user input: '" + point + "'";

                            throw new Exception(msg);
                        }

                        //				double dSquared = System.Math.Pow(dropBackValue, 2);
                        //
                        //				double x1 = interpolatedGeopoint.lon;
                        //				double y1 = interpolatedGeopoint.lat;
                        //
                        //				double x3 = street.ToX;
                        //				double y3 = street.ToY;
                        //
                        //				double d1 = dropBackValue;
                        //				double d2 = System.Math.Sqrt(System.Math.Pow(x1 - x3, 2) + System.Math.Pow(y1 - y3, 2));
                        //
                        //				double slopeRoad = GeometryUtils.getSlope(x1, y1, x3, y3);
                        //				double slopeDropBack = (-1.0) * (1/slopeRoad);
                        //
                        //				double slopeDropBackSquared = System.Math.Pow(slopeDropBack, 2);
                        //				double x1Squared = System.Math.Pow(x1, 2);
                        //
                        //				double a = (1 + slopeDropBackSquared);
                        //				double b = NumberUtils.negative(2 * x1) - (2 * slopeDropBackSquared * x1);
                        //				double bSquared = System.Math.Pow(b, 2);
                        //				double c = (x1Squared + (slopeDropBackSquared * x1Squared) - dSquared);
                        //
                        //				double x2_1 = (NumberUtils.negative(b) + System.Math.Sqrt(bSquared - (4 * a * c))) / (2 * a);
                        //				double x2_2 = (NumberUtils.negative(b) - System.Math.Sqrt(bSquared - (4 * a * c))) / (2 * a);
                        //
                        //				double y2_1 = slopeDropBack * (x2_1 - x1) + y1;
                        //				double y2_2 = slopeDropBack * (x2_2 - x1) + y1;
                        //
                        //				double newLon1 = x2_1;
                        //				double newLat1 = y2_1;
                        //
                        //				double newLon2 = x2_2;
                        //				double newLat2 = y2_2;


                        //				int perpendicularDirection = GeometryUtils.getPerpendicularDirection(street.direction, hand);
                        //
                        //				statistics.matchedFeatureStatistics.streetStatistics.setDropbackDirection(perpendicularDirection);
                        //
                        //				int tempDirection = GeometryUtils.getDirection(interpolatedGeopoint.lon, interpolatedGeopoint.lat, newLon1, newLat1);
                        //				int tempDirection2 = GeometryUtils.getDirection(interpolatedGeopoint.lon, interpolatedGeopoint.lat, newLon2, newLat2);
                        //
                        //				// check which solution to the quadratic (x,y point) is in the expected direction
                        //				if (tempDirection == perpendicularDirection)
                        //				{
                        //					droppedBackLat = newLat1;
                        //					droppedBackLon = newLon1;
                        //				}
                        //				else if (tempDirection2 == perpendicularDirection)
                        //				{
                        //					droppedBackLat = newLat2;
                        //					droppedBackLon = newLon2;
                        //				}
                        //				else
                        //				{
                        //					string msg = "";
                        //					msg += "Expected direction of dropback: " + GeometryUtils.getDirectionName(perpendicularDirection) + " - ";
                        //					msg += "Calculated directions: " + GeometryUtils.getDirectionName(tempDirection) + " & " + GeometryUtils.getDirectionName(tempDirection2);
                        //					msg += " : " + address.toString();
                        //
                        //					throw new DropbackDirectionException(msg);
                        //				}

                    }

                    retValues[0] = droppedBackLon;
                    retValues[1] = droppedBackLat;
                }
            }
            catch (Exception e)
            {
                throw new DropbackException("An error occurred calculating the dropback", e);
            }

            return retValues;
        }

        public static Line FromKMLString(string kml)
        {
            throw new NotImplementedException();
        }

        #region Cloning Functions

        object ICloneable.Clone()
        {
            return Clone();
        }

        public new virtual Line Clone()
        {
            Line x = MemberwiseClone() as Line;
            if (x != null)
            {
                x.Start = Start.Clone();
                x.End = End.Clone();
                x.Id = Id;
            }
            return x;
        }

        #endregion
    }
}