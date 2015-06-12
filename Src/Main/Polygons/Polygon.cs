using System;
using System.Text;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Points;

namespace USC.GISResearchLab.Common.Geometries.Polygons
{
    public class Polygon : Geometry
    {
        #region Properties

        
        
        //private LinearUnitTypes _Units;

        //public LinearUnitTypes Units
        //{
        //    get { return _Units; }
        //    set { _Units = value; }
        //}

        private string _SRS;
        public string SRS
        {
            get { return _SRS; }
            set { _SRS = value; }
        }

        private double _Area;
        public double Area
        {
            get { return _Area; }
            set { _Area = value; }
        }

        public Point CentroidPoint
        {
            get { return new Point( Cx, Cy ); }
        }

        public double[] Centroid
        {
            get { return new double[] { Cx, Cy }; }
            set 
            { 
                Cx = value[0];
                Cy = value[1]; 
            }
        }

        private double _Cx;
        public double Cx
        {
            get { return _Cx; }
            set { _Cx = value; }
        }

        private double _Cy;
        public double Cy
        {
            get { return _Cy; }
            set { _Cy = value; }
        }

        private int _Id;
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private int _NumParts;
        public int NumParts
        {
            get { return _NumParts; }
            set { _NumParts = value; }
        }

        private int _NumPoints;
        public int NumPoints
        {
            get { return _NumPoints; }
            set { _NumPoints = value; }
        }

        private int[] _Parts;
        public int[] Parts
        {
            get { return _Parts; }
            set { _Parts = value; }
        }

        private Point[] _Points;
        public Point[] Points
        {
            get { return _Points; }
            set { _Points = value; }
        }

        private Line[] _Segments;
        public Line[] Segments
        {
            get { return _Segments; }
            set { _Segments = value; }
        }

        public override bool Valid
        {
            get { return (Points != null && Points.Length > 0); }
        }

        #endregion

        public Polygon()
        {
            GeometryType = GeometryType.Polygon;
        }

        public Polygon(int numParts, int numPoints)
        {
            GeometryType = GeometryType.Polygon;
            NumParts = numParts;
            NumPoints = numPoints;
        }

        /*
        public bool SetPart(int i)
        {
            return true;
        }

        public bool SetPoint(int i)
        {
            return true;
        }
        */

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

        public void AddPoint(double [] coords)
        {
            AddPoint(new Point(coords));
        }

        public void AddPoint(Point newPoint)
        {
            if (Points != null && Points.Length > 0)
            {
                // copy all of the old points (without the last one which is the first repeated), 
                // add the new one to the end, 
                // and then close the loop by adding the first one as the last

                Point[] pointsOld = Points;
                Points = new Point[pointsOld.Length + 1];
                for (int i = 0; i < pointsOld.Length - 1; i++)
                {
                    Points[i] = pointsOld[i];
                }

                Points[Points.Length - 2] = newPoint;
                Points[Points.Length - 1] = Points[0];


                // now do the segments
                Segments = new Line[Points.Length - 1];

                for (int i = 0; i < Points.Length - 1; i++)
                {
                    Point start = Points[i];
                    Point end = Points[i + 1];
                    Line line = new Line(start, end);
                    Segments[i] = line;
                }
            }
            else
            {
                Points = new Point[2];
                Points[0] = newPoint;
                Points[1] = newPoint;

                Segments = new Line[1];
                Segments[0] = new Line(Points[0], Points[1]);
            }
        }

        //public double GetArea(LinearUnitTypes outputUnits)
        //{
        //    double ret = -1;
        //    if (CoordinateUnits.UnitType == outputUnits)
        //    {
        //        ret = BoundingBox.Area;
        //    }
        //    else
        //    {
        //        ret = UnitConverter.ConvertArea(CoordinateUnits.UnitType, outputUnits, BoundingBox.Area);
        //    }
        //    return ret;
        //}

        public static Polygon FromCoordinateString(string xyString)
        {
            Polygon ret = new Polygon();
            if (xyString != null && xyString != "")
            {
                string polyLineString = xyString.Trim();
                String[] pointsStrings = xyString.Split(',');
                for (int i = 0; i < pointsStrings.Length; i++)
                {
                    string pair = pointsStrings[i].Trim();
                    Point point = Point.FromString(pair);

                    if (!ret.Contains(point))
                    {
                        ret.AddPoint(point);
                    }
                }
            }

            return ret;
        }

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

        public override string ToWKT()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("POLYGON");
            sb.Append("(");
            sb.Append("(");
            for (int i = 0; i < Points.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }

                sb.Append(Points[i].X);
                sb.Append(" ");
                sb.Append(Points[i].Y);
            }
            sb.Append(")");
            sb.Append(")");
            return sb.ToString();
        }

        public string ToGeoJSON()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            //sb.Append("\"type\": \"Polygon\",");
            //sb.Append("\"coordinates\":");
            sb.Append("type: \"Polygon\", ");
            sb.Append("coordinates:");
            sb.Append("[");
            sb.Append("[");
            for (int i = 0; i < Points.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(" , ");
                }

                sb.Append("[");
                sb.Append(Points[i].X);
                sb.Append(" , ");
                sb.Append(Points[i].Y);
                sb.Append("]");
            }
            sb.Append("]");
            sb.Append("]");

            sb.Append("}");
            return sb.ToString();
        }

        //public string ToGeoJSON()
        //{
        //    return "{'lon':2.285930, 'lat':48.857320}";
        //}

        public override string ToString()
        {
            return ToString(false);
        }
        

        public override string ToString(bool verbose)
        {
            StringBuilder ret = new StringBuilder();
            if (!verbose)
            {
                ret.AppendFormat("{0}, {1}, {2} {3}", GeometryType, CoordinateString, Cx, Cy);
            }
            else
            {
                ret.AppendLine(GetType().Name);
                //string[][] properties = ReflectionUtils.GetObjectProperties(this);
                //for (int i = 0; i < properties.Length; i++)
                //{
                //    ret.AppendFormat("{0}: {1}", properties[i][0], properties[i][1]);
                //    ret.AppendLine();
                //}
            }
            return ret.ToString();
        }

        public static Polygon FromKMLString(string kml)
        {
            throw new NotImplementedException();
        }
    }
}