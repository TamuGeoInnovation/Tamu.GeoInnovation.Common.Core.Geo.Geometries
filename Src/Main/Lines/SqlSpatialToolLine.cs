using Microsoft.SqlServer.Types;
using SQLSpatialTools;
using USC.GISResearchLab.Common.Geometries.Points;

namespace USC.GISResearchLab.Common.Geometries.Lines
{
    /// <summary>
    /// Summary description for Segment.
    /// </summary>
    public class SqlSpatialToolLine : Line
    {


        public SqlSpatialToolLine() : base()
        {

        }

        public SqlSpatialToolLine(double fromX, double fromY, double toX, double toY)
            : base(fromX, fromY, toX, toY)
        {

        }

        public SqlSpatialToolLine(Point start, Point end)
            : base(start, end)
        {

        }



        public new Point Interpolate(double percentage)
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
    }
}