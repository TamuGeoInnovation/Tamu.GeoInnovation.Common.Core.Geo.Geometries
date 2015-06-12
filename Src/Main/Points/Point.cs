using System;
using System.Text;
using System.Xml;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Utils.Strings;

namespace USC.GISResearchLab.Common.Geometries.Points
{
    public class Point : Geometry, ICloneable
    {
        #region Properties

        public LinearUnitTypes Units { get; set; }

        public double[] Coordinates
        {
            get { return new double[] { X, Y }; }
            set
            {
                X = value[0];
                Y = value[1];
            }
        }

        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public override bool Valid
        {
            get { return ((X > -180 && X < 180 && X != 0) && (Y > -180 && Y < 180 && Y != 0)); }
        }

        #endregion

        public Point()
        {
            Area = -1;
            GeometryType = GeometryType.Point;
        }

        public Point(double x, double y)
        {
            Area = -1;
            GeometryType = GeometryType.Point;
            X = x;
            Y = y;
        }

        public Point(double[] coords)
        {
            Area = -1;
            GeometryType = GeometryType.Point;
            if (coords.Length == 2)
            {
                X = coords[0];
                Y = coords[1];
            }
            else
            {
                throw new Exception("Error creating point - input array size != 2");
            }

        }

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        public virtual Point Clone()
        {
            Point p = (Point)MemberwiseClone();

            p.Id = Id;
            p.X = X;
            p.Y = Y;

            return p;
        }

        public override string CoordinateString
        {
            get
            {
                return X + " " + Y;
            }
        }

        public static Point FromString(string xy)
        {
            string[] xyVals = xy.Split(' ');
            return new Point(StringUtils.ToDouble(xyVals[0]), StringUtils.ToDouble(xyVals[1]));
        }

        public static Point FromKMLString(string kml)
        {
            Point ret = null;
            try
            {
                if (!String.IsNullOrEmpty(kml))
                {
                    XmlDocument document = new XmlDocument();
                    XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
                    manager.AddNamespace("g", "http://earth.google.com/kml/2.0");

                    document.LoadXml(kml);

                    XmlNode node = document.SelectSingleNode("//g:Point", manager);
                    if (node != null)
                    {
                        node = document.SelectSingleNode("//g:coordinates", manager);

                        if (node != null)
                        {
                            string coordText = node.InnerText;
                            if (!String.IsNullOrEmpty(coordText) || coordText.IndexOf(",") > -1)
                            {
                                string[] items = coordText.Split(',');
                                if (items.Length > 0)
                                {
                                    double x = Convert.ToDouble(items[0]);
                                    double y = Convert.ToDouble(items[1]);
                                    ret = new Point(x, y);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error creating point from KML string: " + e.Message, e);
            }
            return ret;
        }

        public override string ToWKT()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("POINT");
            sb.Append("(");

            sb.Append(X);
            sb.Append(" ");
            sb.Append(Y);

            sb.Append(")");
            return sb.ToString();
        }
    }
}