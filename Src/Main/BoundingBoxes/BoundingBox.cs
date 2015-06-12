using System;
using System.Text;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Common.Geometries.Polygons;

namespace USC.GISResearchLab.Common.Geometries.BoundingBoxes
{
    public class BoundingBox: Polygon
    {

        public const double defaultBlank = 999999999999;

        #region Properties
      
        private Unit _CoordinateUnits;
        public Unit CoordinateUnits
        {
            get { return _CoordinateUnits; }
            set { _CoordinateUnits = value; }
        }
	

        public double Area
        {
            get
            {   
                double run = Math.Abs(MaxX - MinX); 
                double rise = Math.Abs(MaxY - MinY);
                return rise * run;
            }
        }

        private double _MinX;
        public double MinX
        {
            get { return _MinX; }
            set { _MinX = value; }
        }

        private double _MaxX;
        public double MaxX
        {
            get { return _MaxX; }
            set { _MaxX = value; }
        }

        private double _MinY;
        public double MinY
        {
            get { return _MinY; }
            set { _MinY = value; }
        }

        private double _MaxY;
        public double MaxY
        {
            get { return _MaxY; }
            set { _MaxY = value; }
        }

        public Point BottomLeft
        {
            get{return new Point(MinX, MinY);}
        }

        public Point BottomRight
        {
            get { return new Point(MaxX, MinY); }
        }

        public Point TopLeft
        {
            get{return new Point(MinX, MaxY);}
        }

        public Point TopRight
        {
            get { return new Point(MaxX, MaxY); }
        }

        public new int NumPoints
        {
            get { return 5; }
        }

        public new Point[] Points
        {
            get { return new Point[]{TopLeft, TopRight, BottomRight, BottomLeft, TopLeft}; }
        }

        public new Line[] Segments
        {
            get { return new Line[] { new Line(TopLeft, TopRight), new Line(TopRight, BottomRight), new Line(BottomRight, BottomLeft), new Line(BottomLeft, TopLeft) }; }
        }

        public new Point CentroidPoint
        {
            get { return new Point(Cx, Cy); }
        }

        public new double Cx
        {
            get { return (MinX + MaxX)/2; }
        }

        public new double Cy
        {
            get { return (MinY + MaxY) / 2; }
        }

        #endregion

        public BoundingBox()
        {
        }

        public BoundingBox(double minX, double minY, double maxX, double maxY, Unit coordinateUnits)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;

            Point lowerLeft = new Point(minX, minY);
            Point upperLeft = new Point(minX, maxY);
            Point upperRight = new Point(maxX, maxY);
            Point lowerRight = new Point(minX, maxY);
            CoordinateUnits = coordinateUnits;
        }

        public static BoundingBox FromCoordinateString(string xyString, Unit coordinateUnits)
        {

            bool xIsNegative = false ;
            bool yIsNegative = false;

            double minX = Double.MaxValue;
            double maxX = Double.MinValue;
            double minY = Double.MaxValue;
            double maxY = Double.MinValue;

            BoundingBox ret = null;
            if (xyString != null && xyString != "")
            {
                string polyLineString = xyString.Trim();
                String[] pointsStrings = polyLineString.Split(',');
                if (pointsStrings.Length == 1)
                {
                    string[] pair = pointsStrings[0].Trim().Split(' ');
                    double x = Convert.ToDouble(pair[0].Trim());
                    double y = Convert.ToDouble(pair[1].Trim());

                    if (x < 0)
                    {
                        xIsNegative = true;
                        x = Math.Abs(x);
                    }

                    if (y < 0)
                    {
                        yIsNegative = true;
                        y = Math.Abs(y);
                    }

                    
                    minX = x;
                    maxX = x;
                    minY = y;
                    maxY = y;
                }
                else
                {

                    for (int i = 0; i < pointsStrings.Length; i++)
                    {
                        string[] pair = pointsStrings[i].Trim().Split(' ');
                        double x = Convert.ToDouble(pair[0].Trim());
                        double y = Convert.ToDouble(pair[1].Trim());

                        if (x < 0)
                        {
                            xIsNegative = true;
                            x = Math.Abs(x);
                        }

                        if (y < 0)
                        {
                            yIsNegative = true;
                            y = Math.Abs(y);
                        }

                        if (x <= minX)
                        {
                            minX = x;
                        }
                        if (x >= maxX)
                        {
                            maxX = x;
                        }

                        if (y <= minY)
                        {
                            minY = y;
                        }
                        if (y >= maxY)
                        {
                            maxY = y;
                        }
                    }
                }

                if (xIsNegative)
                {
                    double temp = minX;
                    minX = maxX * -1;
                    maxX = temp * -1;
                }

                if (yIsNegative)
                {
                    double temp = minY;
                    minY = maxY * -1;
                    maxY = temp * -1;
                }

                ret = new BoundingBox(minX, minY, maxX, maxY, coordinateUnits);

            }

            return ret;
        }

        public new string CoordinateString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} {1}, ", MinX, MinY);
                sb.AppendFormat("{0} {1}, ", MinX, MaxY);
                sb.AppendFormat("{0} {1}, ", MaxX, MaxY);
                sb.AppendFormat("{0} {1} ", MaxX, MinY);
                return sb.ToString();
            }
        }

        public void Expand(Geometry geometry)
        {
            if (geometry is Point)
            {
                Expand((Point)geometry);
            }
            else if (geometry is Line)
            {
                Expand((Line)geometry);
            }
            else if (geometry is Polygon)
            {
                Expand((Polygon)geometry);
            }
            else if (geometry is BoundingBox)
            {
                Expand((BoundingBox)geometry);
            }
            else
            {
                throw new Exception("Unexpected geometry type: " + geometry.GetType());
            }
        }

        public void Expand(BoundingBox boundingBox)
        {
            Expand(boundingBox.TopLeft);
            Expand(boundingBox.TopRight);
            Expand(boundingBox.BottomRight);
            Expand(boundingBox.BottomLeft);
        }

        public void Expand(Polygon polygon)
        {
            for (int i = 0; i < polygon.Points.Length; i++)
            {
                Expand(polygon.Points[i]);
            }
        }

        public void Expand(PolyLine polyLine)
        {
            for (int i = 0; i < polyLine.Lines.Length; i++)
            {
                Expand(polyLine.Lines[i]);
            }
        }

        public void Expand(Line line)
        {
            Point start = line.Start;
            Point end = line.End;

            Expand(start);
            Expand(end);
        }

        public void Expand(Point point)
        {
            if (MinX == 0 || MinX == defaultBlank)
            {
                MinX = point.X;
            }
            else
            {
                if (point.X < MinX)
                {
                    MinX = point.X;
                }
            }

            if (MaxX == 0 || MaxX == defaultBlank)
            {
                MaxX = point.X;
            }
            else
            {
                if (point.X > MaxX)
                {
                    MaxX = point.X;
                }
            }

            if (MinY == 0 || MinY == defaultBlank)
            {
                MinY = point.Y;
            }
            else
            {
                if (point.Y < MinY)
                {
                    MinY = point.Y;
                }
            }

            if (MaxY == 0 || MaxY == defaultBlank)
            {
                MaxY = point.Y;
            }
            else
            {
                if (point.Y > MaxY)
                {
                    MaxY = point.Y;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.AppendLine(GetType().Name);
            //string[][] properties = ReflectionUtils.GetObjectProperties(this);
            //for (int i = 0; i < properties.Length; i++)
            //{
            //    ret.AppendFormat("{0}: {1}", properties[i][0], properties[i][1]);
            //    ret.AppendLine();
            //}
            return ret.ToString();
        }


    }
}
