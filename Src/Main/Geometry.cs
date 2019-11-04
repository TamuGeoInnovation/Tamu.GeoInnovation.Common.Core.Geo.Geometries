using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Xml.Serialization;
using USC.GISResearchLab.Common.Geographics.Units;
using USC.GISResearchLab.Common.Geometries.Bearings;
using USC.GISResearchLab.Common.Geometries.BoundingBoxes;
using USC.GISResearchLab.Common.Geometries.Lines;
using USC.GISResearchLab.Common.Geometries.Points;
using USC.GISResearchLab.Common.Geometries.Polygons;

namespace USC.GISResearchLab.Common.Geometries
{
    [XmlInclude(typeof(Point))]
    [XmlInclude(typeof(Line))]
    [XmlInclude(typeof(Polygon))]
    public abstract class Geometry
    {
        #region Properties

        public string Id { get; set; }
        public string Source { get; set; }

        public int SRID { get; set; }
        public SqlGeometry SqlGeometry { get; set; }
        public SqlGeography SqlGeography { get; set; }

        public Unit CoordinateUnits { get; set; }

        public BoundingBox BoundingBox
        {
            get { return BoundingBox.FromCoordinateString(CoordinateString, CoordinateUnits); }
        }

        public GeometryType GeometryType { get; set; }

        public string Error { get; set; }
        public virtual bool Valid { get; set; }
        public double Area { get; set; }
        public LinearUnitTypes AreaUnits { get; set; }

        public abstract string CoordinateString
        {
            get;
        }
        #endregion


        public Geometry()
        {
            Source = "";
            Id = "";
        }


        public abstract string ToWKT();


        public override string ToString()
        {
            return ToString(false);
        }

        public virtual string ToString(bool verbose)
        {
            StringBuilder ret = new StringBuilder();
            if (!verbose)
            {
                ret.AppendFormat("{0}, {1}", GeometryType, CoordinateString);
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

        public SqlGeometry ToSqlGeometry(int srid)
        {
            if (SqlGeometry == null)
            {
                string wkt = ToWKT();
                try
                {
                    SqlGeometry = SqlGeometry.STGeomFromText(new SqlChars(wkt), srid);
                }
                catch (Exception e)
                {
                    throw new Exception("Exception in ToSqlGeometry: " + e.Message, e);
                }
            }

            return SqlGeometry;
        }


        public SqlGeography ToSqlGeography(int srid)
        {

            if (SqlGeography == null)
            {

                string geogWKT = ToWKT();

                try
                {
                    if (!String.IsNullOrEmpty(geogWKT))
                    {
                        SqlGeography = Geometry.WKT2SqlGeography(srid, geogWKT);
                    }

                }
                catch (Exception e)
                {
                    throw new Exception("Exception in ToSqlGeography: " + e.Message, e);
                }
            }

            return SqlGeography;
        }

        public static SqlGeography WKT2SqlGeography(int srid, string geogWKT)
        {
            SqlGeography ret = null;
            try
            {
                ret = SqlGeography.STGeomFromText(new SqlChars(geogWKT), srid);
            }
            catch
            {

                // if the geography was invalid try the fixes from this page: http://www.beginningspatial.com/fixing_invalid_geography_data
                try
                {
                    SqlGeometry sqlGeometry = SqlGeometry.STGeomFromText(new SqlChars(geogWKT), srid);

                    // first try the makevalid fix
                    try
                    {
                        ret = SqlGeography.STGeomFromWKB(sqlGeometry.MakeValid().STAsBinary(), sqlGeometry.STSrid.Value);
                    }
                    catch
                    {
                    }

                    // then try the ring orientation fix
                    if (ret == null)
                    {
                        try
                        {
                            ret = SqlGeography.STGeomFromWKB(sqlGeometry.STUnion(sqlGeometry.STStartPoint()).STAsBinary(), sqlGeometry.STSrid.Value);
                        }
                        catch
                        {
                        }
                    }

                    // then try the buffering unbuffering fix
                    if (ret == null)
                    {
                        try
                        {
                            SqlGeometry temp = sqlGeometry.STBuffer(0.00001).STBuffer(-0.00001);
                            temp = temp.MakeValid();
                            ret = SqlGeography.STGeomFromWKB(temp.STAsBinary(), temp.STSrid.Value);
                        }
                        catch
                        {
                        }
                    }


                    // then try the buffering unbuffering fix at the next level
                    if (ret == null)
                    {
                        try
                        {
                            SqlGeometry temp = sqlGeometry.STBuffer(0.0001).STBuffer(-0.0001);
                            temp = temp.MakeValid();
                            ret = SqlGeography.STGeomFromWKB(temp.STAsBinary(), temp.STSrid.Value);
                        }
                        catch
                        {
                        }
                    }

                    // then try the buffering unbuffering fix at the next level
                    if (ret == null)
                    {
                        try
                        {
                            SqlGeometry temp = sqlGeometry.STBuffer(0.001).STBuffer(-0.001);
                            temp = temp.MakeValid();
                            ret = SqlGeography.STGeomFromWKB(temp.STAsBinary(), temp.STSrid.Value);
                        }
                        catch
                        {
                        }
                    }

                    // then try the reduce fix at one level
                    if (ret == null)
                    {
                        try
                        {
                            ret = SqlGeography.STGeomFromWKB(sqlGeometry.Reduce(0.000001).STAsBinary(), sqlGeometry.STSrid.Value);
                        }
                        catch
                        {
                        }
                    }

                    // then try the reduce fix at the next level
                    if (ret == null)
                    {
                        try
                        {
                            ret = SqlGeography.STGeomFromWKB(sqlGeometry.Reduce(0.00001).STAsBinary(), sqlGeometry.STSrid.Value);
                        }
                        catch
                        {
                        }
                    }

                    // then try the reduce fix at the next level
                    if (ret == null)
                    {
                        try
                        {
                            ret = SqlGeography.STGeomFromWKB(sqlGeometry.Reduce(0.0001).STAsBinary(), sqlGeometry.STSrid.Value);
                        }
                        catch
                        {
                        }
                    }

                    if (ret == null)
                    {
                        string here = "here";
                    }
                }
                catch
                {
                }
            }
            return ret;
        }

        public static SqlGeography BuildSqlGeographyPolygon(Point[] points)
        {
            return BuildSqlGeographyPolygon(points, 4269);
        }

        public static SqlGeography BuildSqlGeographyPolygon(Point[] points, int srid)
        {
            SqlGeography ret = null;

            try
            {
                SqlGeographyBuilder builder = new SqlGeographyBuilder();
                builder.SetSrid(srid);
                builder.BeginGeography(OpenGisGeographyType.Polygon);

                Point firstPoint = points[0];
                builder.BeginFigure(firstPoint.Y, firstPoint.X);

                for (int i = 1; i < points.Length; i++)
                {
                    Point currentPoint = points[i];
                    builder.AddLine(currentPoint.Y, currentPoint.X);
                }

                builder.AddLine(firstPoint.Y, firstPoint.X);
                builder.EndFigure();
                builder.EndGeography();

                ret = builder.ConstructedGeography;

            }
            catch (Exception e)
            {
                throw new Exception("Exception in BuildSqlGeographyPolygon: " + e.Message, e);
            }
            return ret;
        }

        public static SqlGeography BuildSqlGeographyMultiPoint(Point[] points, int srid)
        {
            SqlGeography ret = null;

            try
            {
                SqlGeographyBuilder builder = new SqlGeographyBuilder();
                builder.SetSrid(srid);
                builder.BeginGeography(OpenGisGeographyType.MultiPoint);

                for (int i = 0; i < points.Length; i++)
                {
                    Point currentPoint = points[i];
                    builder.BeginGeography(OpenGisGeographyType.Point);
                    builder.BeginFigure(currentPoint.Y, currentPoint.X);
                    builder.EndFigure();
                    builder.EndGeography();

                }

                builder.EndGeography();

                ret = builder.ConstructedGeography;

            }
            catch (Exception e)
            {
                throw new Exception("Exception in BuildSqlGeographyPolygon: " + e.Message, e);
            }
            return ret;
        }

        public static SqlGeography BuildSqlGeographyMultiPoint(SqlGeography[] geographies, int srid)
        {

            SqlGeography ret = null;

            try
            {
                SqlGeographyBuilder builder = new SqlGeographyBuilder();
                builder.SetSrid(srid);
                builder.BeginGeography(OpenGisGeographyType.MultiPoint);

                for (int i = 0; i < geographies.Length; i++)
                {
                    SqlGeography currentGeography = geographies[i];
                    int numberOfGeographies = currentGeography.STNumGeometries().Value;
                    for (int j = 1; j <= currentGeography.STNumGeometries().Value; j++)
                    {
                        SqlGeography innerGeography = currentGeography.STGeometryN(j);
                        int numberOfPoints = innerGeography.STNumPoints().Value;
                        for (int k = 1; k <= numberOfPoints; k++)
                        {
                            SqlGeography innerPoint = innerGeography.STPointN(k);

                            builder.BeginGeography(OpenGisGeographyType.Point);
                            builder.BeginFigure(innerPoint.Lat.Value, innerPoint.Long.Value);
                            builder.EndFigure();
                            builder.EndGeography();
                        }
                    }
                }

                builder.EndGeography();

                ret = builder.ConstructedGeography;
            }
            catch (Exception e)
            {
                throw new Exception("Exception in BuildSqlGeographyPolygon: " + e.Message, e);
            }

            return ret;
        }

        public static SqlGeometry BuildSqlGeometryMultiPoint(SqlGeometry[] geometries, int srid)
        {

            SqlGeometry ret = null;

            try
            {
                SqlGeometryBuilder builder = new SqlGeometryBuilder();
                builder.SetSrid(srid);
                builder.BeginGeometry(OpenGisGeometryType.MultiPoint);

                for (int i = 0; i < geometries.Length; i++)
                {
                    SqlGeometry currentGeography = geometries[i];
                    int numberOfGeographies = currentGeography.STNumGeometries().Value;
                    for (int j = 1; j <= currentGeography.STNumGeometries().Value; j++)
                    {
                        SqlGeometry innerGeography = currentGeography.STGeometryN(j);
                        int numberOfPoints = innerGeography.STNumPoints().Value;
                        for (int k = 1; k <= numberOfPoints; k++)
                        {
                            SqlGeometry innerPoint = innerGeography.STPointN(k);

                            builder.BeginGeometry(OpenGisGeometryType.Point);
                            builder.BeginFigure(innerPoint.STX.Value, innerPoint.STY.Value);
                            builder.EndFigure();
                            builder.EndGeometry();
                        }
                    }
                }

                builder.EndGeometry();

                ret = builder.ConstructedGeometry;
            }
            catch (Exception e)
            {
                throw new Exception("Exception in BuildSqlGeometryMultiPoint: " + e.Message, e);
            }

            return ret;
        }

        public static SqlGeography BuildSqlGeographyLine(List<SqlGeography> geographies, int srid)
        {
            return BuildSqlGeographyLine(geographies.ToArray(), srid);
        }

        public static SqlGeography BuildSqlGeographyLine(SqlGeography[] geographies, int srid)
        {

            SqlGeography ret = null;

            try
            {
                SqlGeographyBuilder builder = new SqlGeographyBuilder();
                builder.SetSrid(srid);
                builder.BeginGeography(OpenGisGeographyType.LineString);


                for (int i = 0; i < geographies.Length; i++)
                {
                    SqlGeography currentGeography = geographies[i];
                    int numberOfGeographies = currentGeography.STNumGeometries().Value;
                    for (int j = 1; j <= currentGeography.STNumGeometries().Value; j++)
                    {
                        SqlGeography innerGeography = currentGeography.STGeometryN(j);
                        int numberOfPoints = innerGeography.STNumPoints().Value;
                        for (int k = 1; k <= numberOfPoints; k++)
                        {
                            SqlGeography innerPoint = innerGeography.STPointN(k);

                            if (i == 0 && j == 1 && k == 1) // call begin figure on first loop
                            {
                                builder.BeginFigure(innerPoint.Lat.Value, innerPoint.Long.Value);
                            }
                            else // after that just keep adding points
                            {
                                builder.AddLine(innerPoint.Lat.Value, innerPoint.Long.Value);
                            }
                        }
                    }
                }

                builder.EndFigure();
                builder.EndGeography();

                ret = builder.ConstructedGeography;
            }
            catch (Exception e)
            {
                throw new Exception("Exception in BuildSqlGeographyLine: " + e.Message, e);
            }

            return ret;
        }

        public static SqlGeography MidPointOfLine(SqlGeography a)
        {

            SqlGeography ret = null;

            try
            {
                ret = a.EnvelopeCenter();
            }
            catch (Exception e)
            {
                throw new Exception("Exception in MidPointOfLine: " + e.Message, e);
            }

            return ret;
        }

        public static SqlGeography GetPointOnAClosestsToB(SqlGeography a, SqlGeography b)
        {

            SqlGeography ret = null;

            try
            {

                double closestDistance = double.MaxValue;

                int numberOfGeographies = a.STNumGeometries().Value;
                for (int j = 1; j <= a.STNumGeometries().Value; j++)
                {
                    SqlGeography innerGeography = a.STGeometryN(j);
                    int numberOfPoints = innerGeography.STNumPoints().Value;
                    for (int k = 1; k <= numberOfPoints; k++)
                    {
                        SqlGeography innerPoint = innerGeography.STPointN(k);

                        double distance = innerPoint.STDistance(b).Value;
                        if (distance < closestDistance)
                        {
                            ret = innerPoint;
                            closestDistance = distance;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Exception in GetPointOnAClosestsToB: " + e.Message, e);
            }

            return ret;
        }

        public static double CalculateBearing(SqlGeography from, SqlGeography to)
        {
            return Bearing.CalculateBearing(from, to);
        }
    }
}