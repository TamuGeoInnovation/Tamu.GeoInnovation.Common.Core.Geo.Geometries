using System;
using System.Text;
using USC.GISResearchLab.Common.Geometries.Directions;
using USC.GISResearchLab.Common.Geometries.Points;

namespace USC.GISResearchLab.Common.Geometries.Lines
{
    /// <summary>
    /// Summary description for Polyline.
    /// </summary>
    public class PolyLine : Geometry, ICloneable
    {
        #region Properties


        public override string CoordinateString
        {
            get
            {
                string ret = "";
                if (Points != null)
                {
                    for (int i = 0; i < Points.Length; i++)
                    {
                        if (i > 0)
                        {
                            ret += ",";
                        }

                        ret += Points[i].X;
                        ret += " ";
                        ret += Points[i].Y;
                    }
                }

                return ret;
            }
        }

        public Point[] Points { get; set; }

        public Line[] Lines { get; set; }

        public Point StartingPoint
        {
            get
            {
                Point point = null;
                if (Points != null)
                {
                    point = Points[0];
                }
                return point;
            }
        }

        public Point EndingPoint
        {
            get
            {
                Point point = null;
                if (Points != null)
                {
                    point = Points[Points.Length - 1];
                }
                return point;
            }
        }

        public int PrimaryDirection { get; set; }

        public double PrimaryBearing { get; set; }

        public double Length
        {
            get
            {
                double ret = 0;
                for (int i = 0; i < Lines.Length; i++)
                {
                    ret += Lines[i].Length;
                }
                return ret;
            }
        }

        public int SourceId { get; set; }



        public bool IsReversed { get; set; }

        public override bool Valid
        {
            get { return (Points != null && Points.Length > 0); }
        }


        #endregion

        public PolyLine()
        {
            GeometryType = GeometryType.PolyLine;
        }

        public Line AsLine()
        {
            return new Line(StartingPoint, EndingPoint);
        }

        public bool isAClockwiseSegment(int parcelDirection)
        {
            return CardinalDirection.isAClockWiseCombination(PrimaryDirection, parcelDirection);
        }


        public string GetPointStringReverse()
        {
            string ret = "";
            if (Points != null)
            {
                for (int i = Points.Length - 1; i >= 0; i--)
                {
                    if (i > 0)
                        ret += ",";

                    ret += Points[i];
                }
            }
            return ret;
        }

        public bool Contains(Point p)
        {
            bool ret = false;
            if (Points != null)
            {
                for (int i = 0; i < Points.Length; i++)
                {
                    if (p.Equals(Points[i]))
                    {
                        ret = true;
                    }
                }
            }
            return ret;
        }

        public void AddPoint(double x, double y)
        {
            AddPoint(new Point(x, y));
        }

        public void AddPoint(double[] coords)
        {
            AddPoint(new Point(coords));
        }

        public void AddPoint(Point newPoint)
        {
            if (Points != null && Points.Length > 0)
            {
                Point[] pointsOld = Points;
                Points = new Point[pointsOld.Length + 1];
                for (int i = 0; i < pointsOld.Length; i++)
                    Points[i] = pointsOld[i];

                Points[Points.Length - 1] = newPoint;

                Line newLine = new Line(Points[Points.Length - 2], Points[Points.Length - 1]);
                AddLine(newLine);
            }
            else
            {
                Points = new Point[1];
                Points[0] = newPoint;
            }
        }

        private void AddLine(Line line)
        {
            if (Lines != null)
            {
                Line[] newLines = new Line[Lines.Length + 1];
                for (int i = 0; i < Lines.Length; i++)
                {
                    newLines[i] = Lines[i];
                }
                newLines[newLines.Length - 1] = line;
                Lines = newLines;
            }
            else
            {
                Lines = new Line[1];
                Lines[0] = line;
            }
        }

        public PolyLine Reverse()
        {
            string coordinatesReversed = GetPointStringReverse();
            PolyLine ret = FromCoordinateString(coordinatesReversed);
            ret.SourceId = SourceId;
            ret.Source = Source;
            ret.IsReversed = true;
            return ret;
        }

        public static PolyLine FromCoordinateString(string latLonString)
        {
            PolyLine ret = new PolyLine();
            string polyLineString = latLonString.Trim();
            String[] pointsStrings = polyLineString.Split(',');
            for (int i = 0; i < pointsStrings.Length; i++)
            {
                string pair = pointsStrings[i].Trim();
                Point point = Point.FromString(pair);
                if (!ret.Contains(point))
                {
                    ret.AddPoint(point);
                }
            }

            return ret;
        }

        public Point interpolate(double percentage)
        {

            double totalLength = Length;
            double traveled = 0;
            double nextTraveled = 0;
            double remainingPercentage = percentage;
            double stopLength = Length * percentage;
            int lineIndex = 0;

            for (int i = 0; i < Lines.Length; i++)
            {
                nextTraveled += traveled + Lines[i].Length;
                if (nextTraveled >= stopLength)
                {
                    remainingPercentage = percentage - (traveled / totalLength);
                    lineIndex = i;
                    break;
                }

                traveled = nextTraveled;
            }

            return Lines[lineIndex].Interpolate(remainingPercentage);
        }

        public static Line FromKMLString(string kml)
        {
            throw new NotImplementedException();
        }

        public override string ToWKT()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("LINESTRING");
            sb.Append("(");
            for (int i = 0; i < Lines.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }

                Line line = Lines[i];

                sb.Append(line.Start.X);
                sb.Append(" ");
                sb.Append(line.Start.Y);

                if (i == Lines.Length - 1)
                {
                    sb.Append(line.End.X);
                    sb.Append(" ");
                    sb.Append(line.End.Y);
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        #region Cloning Functions

        object ICloneable.Clone()
        {
            return Clone();
        }

        public virtual PolyLine Clone()
        {
            PolyLine x = MemberwiseClone() as PolyLine;

            if (x != null)
            {
                Point[] clonedPoints = new Point[Points.Length];
                for (int i = 0; i < Points.Length; i++)
                {
                    clonedPoints[i] = Points[i].Clone();
                }
                x.Points = clonedPoints;

                Line[] clonedLines = new Line[Lines.Length];
                for (int i = 0; i < Lines.Length; i++)
                {
                    clonedLines[i] = Lines[i].Clone();
                }
                x.Lines = clonedLines;

                x.PrimaryDirection = PrimaryDirection;
            }
            return x;
        }

        #endregion
    }
}