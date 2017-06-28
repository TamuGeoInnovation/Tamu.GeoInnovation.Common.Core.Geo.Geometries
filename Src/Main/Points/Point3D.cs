using System;
using System.Xml;
using USC.GISResearchLab.Common.Geometries.Vectors;

namespace USC.GISResearchLab.Common.Geometries.Points
{
	/// <summary>
	/// Basic geometry class : easy to replace
	/// Written so as to be generalized
	/// </summary>
	[Serializable]
	public class Point3D
	{
		double[] _Coordinates = new double[3];

		/// <summary>
		/// Point3D constructor.
		/// </summary>
		/// <exception cref="ArgumentNullException">Argument array must not be null.</exception>
		/// <exception cref="ArgumentException">The Coordinates' array must contain exactly 3 elements.</exception>
		/// <param name="Coordinates">An array containing the three coordinates' values.</param>
		public Point3D(double[] Coordinates)
		{
			if ( Coordinates == null ) throw new ArgumentNullException();
			if ( Coordinates.Length!=3 ) throw new ArgumentException("The Coordinates' array must contain exactly 3 elements.");
			X = Coordinates[0]; Y = Coordinates[1]; Z = Coordinates[2];
		}

		/// <summary>
		/// Point3D constructor.
		/// </summary>
		/// <param name="CoordinateX">X coordinate.</param>
		/// <param name="CoordinateY">Y coordinate.</param>
		/// <param name="CoordinateZ">Z coordinate.</param>
		public Point3D(double CoordinateX, double CoordinateY, double CoordinateZ)
		{
			X = CoordinateX; Y = CoordinateY; Z = CoordinateZ;
		}

		/// <summary>
		/// Accede to coordinates by indexes.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">Index must belong to [0;2].</exception>
		public double this[int CoordinateIndex]
		{
			get { return _Coordinates[CoordinateIndex]; }
			set	{ _Coordinates[CoordinateIndex] = value; }
		}

		/// <summary>
		/// Gets/Set X coordinate.
		/// </summary>
		public double X { set { _Coordinates[0] = value; } get { return _Coordinates[0]; } }

		/// <summary>
		/// Gets/Set Y coordinate.
		/// </summary>
		public double Y { set { _Coordinates[1] = value; } get { return _Coordinates[1]; } }

		/// <summary>
		/// Gets/Set Z coordinate.
		/// </summary>
		public double Z { set { _Coordinates[2] = value; } get { return _Coordinates[2]; } }

		

		/// <summary>
		/// Returns the projection of a point on the line defined with two other points.
		/// When the projection is out of the segment, then the closest extremity is returned.
		/// </summary>
		/// <exception cref="ArgumentNullException">None of the arguments can be null.</exception>
		/// <exception cref="ArgumentException">P1 and P2 must be different.</exception>
		/// <param name="Pt">Point to project.</param>
		/// <param name="P1">First point of the line.</param>
		/// <param name="P2">Second point of the line.</param>
		/// <returns>The projected point if it is on the segment / The closest extremity otherwise.</returns>
		public static Point3D ProjectOnLine(Point3D Pt, Point3D P1, Point3D P2)
		{
			if ( Pt==null || P1==null || P2==null ) throw new ArgumentNullException("None of the arguments can be null.");
			if ( P1.Equals(P2) ) throw new ArgumentException("P1 and P2 must be different.");
			Vector3D VLine = new Vector3D(P1, P2);
			Vector3D V1Pt = new Vector3D(P1, Pt);
			Vector3D Translation = VLine*(VLine|V1Pt)/VLine.SquareNorm;
			Point3D Projection = P1+Translation;

			Vector3D V1Pjt = new Vector3D(P1, Projection);
			double D1 = V1Pjt|VLine;
			if ( D1<0 ) return P1;

			Vector3D V2Pjt = new Vector3D(P2, Projection);
			double D2 = V2Pjt|VLine;
			if ( D2>0 ) return P2;

			return Projection;
		}


        public static Point3D FromKMLString(string kml)
        {
            Point3D ret = null;
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
                                    double z = Convert.ToDouble(items[2]);
                                    ret = new Point3D(x, y, z);
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

		/// <summary>
		/// Object.Equals override.
		/// Tells if two points are equal by comparing coordinates.
		/// </summary>
		/// <exception cref="ArgumentException">Cannot compare Point3D with another type.</exception>
		/// <param name="Point">The other 3DPoint to compare with.</param>
		/// <returns>'true' if points are equal.</returns>
		public override bool Equals(object Point)
		{
			Point3D P = (Point3D)Point;
            if (P == null)
            {
                throw new ArgumentException("Object must be of type " + GetType());
            }

			bool Resultat = true;

            for (int i = 0; i < 3; i++)
            {
                Resultat &= P[i].Equals(this[i]);
            }

			return Resultat;
		}

		/// <summary>
		/// Object.GetHashCode override.
		/// </summary>
		/// <returns>HashCode value.</returns>
		public override int GetHashCode()
		{
            return base.GetHashCode();
		}

		/// <summary>
		/// Object.GetHashCode override.
		/// Returns a textual description of the point.
		/// </summary>
		/// <returns>String describing this point.</returns>
		public override string ToString()
		{
			string Deb = "(";
			string Sep = ",";
			string Fin = ")";
			string Resultat = Deb;
			
            //for (int i=0; i<Dimension; i++)
            //    Resultat += _Coordinates[i].ToString() + (i!=Dimension-1 ? Sep : Fin);
            Resultat = Deb + X.ToString() + Sep + Y.ToString() + Sep +Z.ToString() + Fin;
			return Resultat;
		}
	}
}
