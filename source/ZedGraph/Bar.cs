//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright (C) 2004  John Champion
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
using System.Drawing.Drawing2D;

namespace ZedGraph
{
	/// <summary>
	/// A class representing all the characteristics of the <see cref="Bar"/>
	/// segments that make up a curve on the graph.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 2.2 $ $Date: 2004-09-13 06:51:42 $ </version>
	public class Bar
	{
	#region Fields
		/// <summary>
		/// Private field that stores the color of the frame around this
		/// <see cref="Bar"/>.  Use the public
		/// property <see cref="FrameColor"/> to access this value.  This property is
		/// only applicable if the <see cref="IsFramed"/> property is true.
		/// </summary>
		private Color	frameColor;
		/// <summary>
		/// Private field that stores the <see cref="ZedGraph.Fill"/> data for this
		/// <see cref="Bar"/>.  Use the public property <see cref="Fill"/> to
		/// access this value.
		/// </summary>
		private Fill	fill;
		/// <summary>
		/// Private field that determines if a frame will be drawn around this
		/// <see cref="Bar"/>.  Use the public
		/// property <see cref="IsFramed"/> to access this value.  The pen width
		/// for the frame is determined by the property <see cref="FrameWidth"/>.
		/// </summary>
		private bool	isFramed;
		/// <summary>
		/// Private field that determines the pen width for the frame around this
		/// <see cref="Bar"/>, in pixel units.  Use the public
		/// property <see cref="FrameWidth"/> to access this value.
		/// </summary>
		private float	frameWidth;
	#endregion

	#region Defaults
		/// <summary>
		/// A simple struct that defines the
		/// default property values for the <see cref="Bar"/> class.
		/// </summary>
		public struct Default
		{
			// Default Bar properties
			/// <summary>
			/// The default pen width to be used for drawing the frame around the bars
			/// (<see cref="Bar.FrameWidth"/> property).  Units are points.
			/// </summary>
			public static float FrameWidth = 1.0F;
			/// <summary>
			/// The default fill mode for bars (<see cref="ZedGraph.Fill.Type"/> property).
			/// </summary>
			public static FillType FillType = FillType.Brush;
			/// <summary>
			/// The default frame mode for bars (<see cref="Bar.IsFramed"/> property).
			/// true to display frames around bars, false otherwise
			/// </summary>
			public static bool IsFramed = true;
			/// <summary>
			/// The default color for drawing frames around bars
			/// (<see cref="Bar.FrameColor"/> property).
			/// </summary>
			public static Color FrameColor = Color.Black;
			/// <summary>
			/// The default color for filling in the bars
			/// (<see cref="ZedGraph.Fill.Color"/> property).
			/// </summary>
			public static Color FillColor = Color.Red;
			/// <summary>
			/// The default custom brush for filling in the bars
			/// (<see cref="ZedGraph.Fill.Brush"/> property).
			/// </summary>
			public static Brush FillBrush = null; //new LinearGradientBrush( new Rectangle(0,0,100,100),
				// Color.White, Color.Red, 0F );
		}
	#endregion

	#region Constructors
		/// <summary>
		/// Default constructor that sets all <see cref="Bar"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		public Bar() : this( Color.Empty )
		{
		}

		/// <summary>
		/// Default constructor that sets the 
		/// <see cref="Color"/> as specified, and the remaining
		/// <see cref="Bar"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// The specified color is only applied to the
		/// <see cref="ZedGraph.Fill.Color"/>, and the <see cref="FrameColor"/>
		/// will be defaulted.
		/// </summary>
		/// <param name="color">A <see cref="Color"/> value indicating
		/// the <see cref="ZedGraph.Fill.Color"/>
		/// of the Bar.
		/// </param>
		public Bar( Color color )
		{
			this.frameColor = Default.FrameColor;
			this.isFramed = Default.IsFramed;
			this.frameWidth = Default.FrameWidth;
			this.fill = new Fill( color.IsEmpty ? Default.FillColor : color,
						Default.FillBrush, Default.FillType );
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The Bar object from which to copy</param>
		public Bar( Bar rhs )
		{
			this.frameColor = rhs.FrameColor;
			this.isFramed = rhs.IsFramed;
			this.frameWidth = rhs.FrameWidth;
			this.fill = rhs.Fill;
		}
	#endregion

	#region Methods
		/// <summary>
		/// Deep-copy clone routine
		/// </summary>
		/// <returns>A new, independent copy of the Bar</returns>
		public object Clone()
		{ 
			return new Bar( this ); 
		}
	#endregion

	#region Properties
		/// <summary>
		/// The color of the frame around the <see cref="Bar"/>.
		/// </summary>
		/// <seealso cref="IsFramed"/>
		/// <seealso cref="FrameWidth"/>
		/// <seealso cref="Default.FrameColor"/>
		public Color FrameColor
		{
			get { return frameColor; }
			set { frameColor = value; }
		}
		/// <summary>
		/// Gets or sets the <see cref="ZedGraph.Fill"/> data for this
		/// <see cref="Bar"/>.
		/// </summary>
		public Fill	Fill
		{
			get { return fill; }
			set { fill = value; }
		}

		/// <summary>
		/// Determines if the <see cref="Bar"/> has a frame around it.
		/// </summary>
		/// <seealso cref="FrameColor"/>
		/// <seealso cref="FrameWidth"/>
		/// <seealso cref="Default.IsFramed"/>
		public bool IsFramed
		{
			get { return isFramed; }
			set { isFramed = value; }
		}
		/// <summary>
		/// Determines the pen width for the frame around the <see cref="Bar"/>,
		/// in pixel units.
		/// </summary>
		/// <seealso cref="IsFramed"/>
		/// <seealso cref="FrameColor"/>
		/// <seealso cref="Default.FrameWidth"/>
		public float FrameWidth
		{
			get { return frameWidth; }
			set { frameWidth = value; }
		}
	#endregion

	#region Rendering Methods
		/// <summary>
		/// Draw the <see cref="Bar"/> to the specified <see cref="Graphics"/> device
		/// at the specified location.  This routine draws a single bar.
		/// </summary>
		/// <param name="g">
		/// A graphic device object to be drawn into.  This is normally e.Graphics from the
		/// PaintEventArgs argument to the Paint() method.
		/// </param>
		/// <param name="left">The x position of the left side of the bar in
		/// screen pixel units</param>
		/// <param name="right">The x position of the right side of the bar in
		/// screen pixel units</param>
		/// <param name="top">The y position of the top of the bar in
		/// screen pixel units</param>
		/// <param name="bottom">The y position of the bottom of the bar in
		/// screen pixel units</param>
		/// <param name="scaleFactor">
		/// The scaling factor for the features of the graph based on the <see cref="GraphPane.BaseDimension"/>.  This
		/// scaling factor is calculated by the <see cref="GraphPane.CalcScaleFactor"/> method.  The scale factor
		/// represents a linear multiple to be applied to font sizes, symbol sizes, etc.
		/// </param>
		/// <param name="fullFrame">true to draw the bottom portion of the frame around the
		/// bar (this is for legend entries)</param>	
		public void Draw( Graphics g, float left, float right, float top,
						float bottom, double scaleFactor, bool fullFrame )
		{
			if ( top > bottom )
			{
				float junk = top;
				top = bottom;
				bottom = junk;
			}

			// Fill the Bar
			if ( this.fill.IsFilled && ( !this.fill.Color.IsEmpty || this.fill.Brush != null ) )
			{
				Brush	brush;
				if ( this.fill.Type == FillType.Brush )
				{
					// If they didn't provide a brush, make one using the fillcolor gradient to white
					if ( this.fill.Brush == null )
						brush = new LinearGradientBrush( new Rectangle( 0, 0, 100, 100 ),
							Color.White, this.fill.Color, 0F );
					else
						brush = this.fill.Brush;
				}
				else
					brush = (Brush) new SolidBrush( this.fill.Color );

				RectangleF rect = new RectangleF( left, top, right - left, bottom - top );
				if ( brush is LinearGradientBrush || brush is TextureBrush )
				{
					LinearGradientBrush linBrush = (LinearGradientBrush) brush.Clone();
					linBrush.ScaleTransform( rect.Width / linBrush.Rectangle.Width,
						rect.Height / linBrush.Rectangle.Height, MatrixOrder.Append );
					linBrush.TranslateTransform( rect.Left - linBrush.Rectangle.Left,
						rect.Top - linBrush.Rectangle.Top, MatrixOrder.Append );
					brush = (Brush) linBrush;
				}
				g.FillRectangle( brush, rect );
			}

			if ( this.isFramed && !this.frameColor.IsEmpty  )
			{
				Pen pen = new Pen( this.frameColor, this.frameWidth );

				g.DrawLine( pen, left, bottom, left, top );
				g.DrawLine( pen, left, top, right, top );
				g.DrawLine( pen, right, top, right, bottom );
				if ( fullFrame )
					g.DrawLine( pen, right, bottom, left, bottom );
			}
		}
	#endregion
	}
}