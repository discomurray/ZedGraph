//============================================================================
//PointPairList Class
//Copyright (C) 2004  Jerry Vos
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Drawing;
using System.Collections;

namespace ZedGraph
{
	/// <summary>
	/// A collection class containing a list of <see cref="PointPair"/> objects
	/// that define the set of points to be displayed on the curve.
	/// </summary>
	/// 
	/// <author> Jerry Vos based on code by John Champion
	/// modified by John Champion</author>
	/// <version> $Revision: 3.0 $ $Date: 2004-09-22 02:18:08 $ </version>
	public class PointPairList : CollectionBase, ICloneable
	{
	#region Fields
		/// <summary>Private field to maintain the sort status of this
		/// <see cref="PointPairList"/>.  Use the public property
		/// <see cref="Sorted"/> to access this value.
		/// </summary>
		private bool sorted = true;
	#endregion

	#region Properties
		/// <summary>
		/// true if the list is currently sorted.
		/// </summary>
		/// <seealso cref="Sort"/>
		public bool Sorted
		{
			get { return sorted; }
		}
	#endregion

	#region Constructors
		/// <summary>
		/// Default constructor for the collection class
		/// </summary>
		public PointPairList()
		{
		}

		/// <summary>
		/// Constructor to initialize the PointPairList from two arrays of
		/// type double.
		/// </summary>
		public PointPairList( double[] x, double[] y )
		{
			Add( x, y );
			
			sorted = false;
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The PointPairList from which to copy</param>
		public PointPairList( PointPairList rhs )
		{
			Add( rhs );

			sorted = false;
		}
	#endregion

	#region Methods
		/// <summary>
		/// Deep-copy clone routine
		/// </summary>
		/// <returns>A new, independent copy of the CurveList</returns>
		public object Clone()
		{ 
			return new PointPairList( this ); 
		}
		
		/// <summary>
		/// Indexer to access the specified <see cref="PointPair"/> object by
		/// its ordinal position in the list.
		/// </summary>
		/// <param name="index">The ordinal position (zero-based) of the
		/// <see cref="PointPair"/> object to be accessed.</param>
		/// <value>A <see cref="PointPair"/> object reference.</value>
		public PointPair this[ int index ]  
		{
			get { return( (PointPair) List[index] ); }
			set { List[index] = value; }
		}

		/// <summary>
		/// Add a <see cref="PointPair"/> object to the collection at the end of the list.
		/// </summary>
		/// <param name="point">A reference to the <see cref="PointPair"/> object to
		/// be added</param>
		public void Add( PointPair point )
		{
			List.Add( point );
			sorted = false;
		}

		/// <summary>
		/// Add a <see cref="PointPairList"/> object to the collection at the end of the list.
		/// </summary>
		/// <param name="pointList">A reference to the <see cref="PointPairList"/> object to
		/// be added</param>
		public void Add( PointPairList pointList )
		{
			foreach ( PointPair point in pointList )
				this.Add( point );
				
			sorted = false;
		}

		/// <summary>
		/// Add a set of points to the PointPairList from two arrays of type double.
		/// If either array is null, then a set of ordinal values is automatically
		/// generated in its place (see <see cref="AxisType.Ordinal"/>.
		/// If the arrays are of different size, then the larger array prevails and the
		/// smaller array is padded with <see cref="PointPair.Missing"/> values.
		/// </summary>
		/// <param name="x">A double[] array of X values</param>
		/// <param name="y">A double[] array of Y values</param>
		public void Add( double[] x, double[] y )
		{
			PointPair	point;
			int 		len = 0;
			
			if ( x != null )
				len = x.Length;
			if ( y != null && y.Length > len )
				len = y.Length;
			
			for ( int i=0; i<len; i++ )
			{
				if ( x == null )
					point.X = (double) i + 1.0;
				else if ( i < x.Length )
					point.X = x[i];
				else
					point.X = PointPair.Missing;
					
				if ( y == null )
					point.Y = (double) i + 1.0;
				else if ( i < y.Length )
					point.Y = y[i];
				else
					point.Y = PointPair.Missing;
					
				List.Add( point );
			}
			
			sorted = false;
		}

		/// <summary>
		/// Add a single point to the PointPairList from values of type double.
		/// </summary>
		/// <param name="x">The X value</param>
		/// <param name="y">The Y value</param>
		public void Add( double x, double y )
		{
			PointPair	point = new PointPair( x, y );
			List.Add( point );
			
			sorted = false;
		}

		/// <summary>
		/// Remove a <see cref="PointPair"/> object from the collection at the
		/// specified ordinal location.
		/// </summary>
		/// <param name="index">
		/// An ordinal position in the list at which the object to be removed 
		/// is located.
		/// </param>
		public void Remove( int index )
		{
			List.RemoveAt( index );
		}

		/// <summary>
		/// Sorts the list according to the point x values. Will not sort the 
		/// list if the list is already sorted.
		/// </summary>
		/// <returns>If the list was sorted before sort was called</returns>
		public bool Sort()
		{
			// if it is already sorted we don't have to sort again
			if ( sorted )
				return true;

			InnerList.Sort( new PointPair.PointPairComparer() );
			return false;
		}
		
		/// <summary>
		/// Go through the list of <see cref="PointPair"/> data values
		/// and determine the minimum and maximum values in the data.
		/// </summary>
		/// <param name="xMin">The minimum X value in the range of data</param>
		/// <param name="xMax">The maximum X value in the range of data</param>
		/// <param name="yMin">The minimum Y value in the range of data</param>
		/// <param name="yMax">The maximum Y value in the range of data</param>
		/// <param name="ignoreInitial">ignoreInitial is a boolean value that
		/// affects the data range that is considered for the automatic scale
		/// ranging (see <see cref="GraphPane.IsIgnoreInitial"/>).  If true, then initial
		/// data points where the Y value is zero are not included when
		/// automatically determining the scale <see cref="Axis.Min"/>,
		/// <see cref="Axis.Max"/>, and <see cref="Axis.Step"/> size.  All data after
		/// the first non-zero Y value are included.
		/// </param>
		public void GetRange(	ref double xMin, ref double xMax,
								ref double yMin, ref double yMax,
								bool ignoreInitial )
		{
			// initialize the values to outrageous ones to start
			xMin = yMin = Double.MaxValue;
			xMax = yMax = Double.MinValue;
			
			// Loop over each point in the arrays
			foreach ( PointPair point in this )
			{
				double curX = point.X;
				double curY = point.Y;
				
				// ignoreInitial becomes false at the first non-zero
				// Y value
				if (	ignoreInitial && curY != 0 &&
						curY != PointPair.Missing )
					ignoreInitial = false;
				
				if ( 	!ignoreInitial &&
						curX != PointPair.Missing &&
						curY != PointPair.Missing )
				{
					if ( curX < xMin )
						xMin = curX;
					if ( curX > xMax )
						xMax = curX;
					if ( curY < yMin )
						yMin = curY;
					if ( curY > yMax )
						yMax = curY;
				}
			}	
		}
	#endregion
	}
}


