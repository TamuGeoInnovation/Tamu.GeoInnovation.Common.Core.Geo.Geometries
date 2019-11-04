using System;
using System.Text;
using USC.GISResearchLab.Common.Geometries.Points;

namespace USC.GISResearchLab.Common.Geometries.Vectors
{
    /// <summary>
    /// Summary description for Vector.
    /// </summary>
    public class Vector : Geometry, ICloneable
    {
        #region Properties

        public double FromX { get; set; }
        public double FromY { get; set; }
        public double ToX { get; set; }
        public double ToY { get; set; }

        public double DeltaX
        {
            get { return ToX - FromX; }
        }

        public double DeltaY
        {
            get { return ToY - FromY; }
        }

        public double Length
        {
            get { return Math.Sqrt((DeltaX * DeltaX) + (DeltaY * DeltaY)); }
        }

        #endregion

        public Vector()
        {
        }

        public Vector(double fromX, double fromY, double toX, double toY)
        {
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
        }

        public Vector(Point fromPoint, Point toPoint)
        {
            FromX = fromPoint.X;
            FromY = fromPoint.Y;
            ToX = toPoint.X;
            ToY = toPoint.Y;
        }

        public static double DotProduct(Vector v1, Vector v2)
        {
            return (v1.DeltaX * v2.DeltaX) + (v1.DeltaY * v2.DeltaY);
        }

        public static double AngleBetween(Vector v1, Vector v2)
        {
            double cosAngle = (DotProduct(v1, v2)) / (v1.Length * v2.Length);
            double ret = Math.Acos(cosAngle);
            ret = ret * 180 / Math.PI;
            return ret;
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

        public override string CoordinateString
        {
            get { return FromX + " " + FromY + "," + ToX + " " + ToY; }
        }

        public override string ToWKT()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("LINESTRING");
            sb.Append("(");

            sb.Append(FromX);
            sb.Append(" ");
            sb.Append(FromY);

            sb.Append(",");

            sb.Append(ToX);
            sb.Append(" ");
            sb.Append(ToY);

            sb.Append(")");
            return sb.ToString();
        }

        #region Cloning Functions
        object ICloneable.Clone()
        {
            return Clone();
        }

        public virtual Vector Clone()
        {
            Vector v = (Vector)MemberwiseClone();
            if (v != null)
            {
                v.Id = Id;
                v.FromX = FromX;
                v.FromY = FromY;
                v.ToX = ToX;
                v.ToY = ToY;
            }
            return v;
        }
        #endregion
    }
}