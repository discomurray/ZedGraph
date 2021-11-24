using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
    /// <summary>
    /// Histogram mini chart attached to a scale object
    /// </summary>
    [Serializable]
    public class ScaleHistogram : ICloneable, ISerializable
    {
        private bool _isVisible;

        private float _width;

        private int _nBin;

        private bool _autoNBin;

        private RectangleF _rect;

        /// <summary>
        /// private field that stores the owner Scale that contains this ScaleHistogram instance.
        /// </summary>
        public Scale _ownerScale;

        private bool _isHiFreqVisible;

        #region Constructor

        public ScaleHistogram(Scale scale)
        {
            _ownerScale = scale;

            _isVisible = Defaults.IsVisible;
            _width = Defaults.Width;
            _nBin = Defaults.NBin;
            _isHiFreqVisible = Defaults.IsHiFreqVisible;
            _autoNBin = Defaults.AutoNBin;
        }

        public ScaleHistogram(ScaleHistogram rhs, Scale scale)
        {
            _ownerScale = scale;

            _isVisible = rhs._isVisible;
            _width = rhs._width;
            _nBin = rhs.NBin;
            _isHiFreqVisible = rhs._isHiFreqVisible;
            _autoNBin = rhs._autoNBin;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public ScaleHistogram Clone()
        {
            return new ScaleHistogram(this, this._ownerScale);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value that determines if the histogram mini chart will be visible
        /// </summary>
        /// <value>true to show the histogram, false otherwise</value>
        /// <seealso cref="Default.IsVisible">Default.IsVisible</seealso>.
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int NBin
        {
            get
            {
                return _nBin;
            }
            set
            {
                if (_nBin != value)
                {
                    _nBin = value;
                }
            }
        }

        public RectangleF Rect
        {
            get { return _rect; }
        }

        public bool IsHiFreqVisible
        {
            get { return _isHiFreqVisible; }
            set { _isHiFreqVisible = value; }
        }

        public bool IsAutoNBin
        {
            get { return _autoNBin; }
            set { _autoNBin = value; }
        }

        #endregion

        #region Defaults
        public struct Defaults
        {
            public static bool IsVisible = false;
            public static float Width = 22.0F;
            public static int NBin = -1;
            public static bool IsHiFreqVisible = false;
            public static bool AutoNBin = true;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Current schema value that defines the version of the serialized file
        /// </summary>
        public const int schema = 10;

        /// <summary>
        /// Constructor for deserializing objects
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
        /// </param>
        /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
        /// </param>
        protected ScaleHistogram(SerializationInfo info, StreamingContext context)
        {
            // The schema value is just a file version parameter.  You can use it to make future versions
            // backwards compatible as new member variables are added to classes
            int sch = info.GetInt32("schema");
            _isVisible = info.GetBoolean("isVisible");
            _width = info.GetSingle("width");
            _nBin = info.GetInt32("nBin");
            _isHiFreqVisible = info.GetBoolean("isHiFreqVisible");
            _autoNBin = info.GetBoolean("IsAutoNBin");
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
        /// </summary>
        /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
        /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("schema", schema);
            info.AddValue("isVisible", _isVisible);
            info.AddValue("Width", _width);
            info.AddValue("nBin", _nBin);
            info.AddValue("isHiFreqVisible", _isHiFreqVisible);
            info.AddValue("IsAutoNBin", _autoNBin);
        }

        #endregion

        public float CalcSpace(float scaleFactor)
        {
            if (_isVisible)
            {
                if (Math.Abs(_ownerScale.Min - _ownerScale.Max) > 0)
                {
                    return _width * scaleFactor;
                }
            }
            return 0F;
        }

        public string GetFrequencyValues(GraphPane pane, Scale scale, Point mousePt)
        {
            StringBuilder strValues = new StringBuilder();
            RectangleF rect = _rect;
            float px = 0F;

            foreach (CurveItem curve in pane.CurveList)
            {
                if (curve._histogram == null || curve.NPts == 0)
                    continue;

                //For all curves related to the given scale
                bool isX = true;
                
                //Rotate rect depending on scale oriestation and get the pixel value within the rect.
                if (curve.GetYAxis(pane).Scale == scale)
                {
                    bool isY2 = scale._ownerAxis is Y2Axis;
                    if(isY2)
                    {
                        rect = new RectangleF(scale.Rect.X, scale.Rect.Y, _rect.Height, _rect.Width);
                        px = scale.Rect.Y + _rect.Width - mousePt.Y;
                    }
                    else
                    {
                        rect = new RectangleF(scale.Rect.X + scale.Rect.Width - _rect.Height, scale.Rect.Y, _rect.Height, _rect.Width);
                        px = scale.Rect.Y + _rect.Width - mousePt.Y;
                    }
                    
                    isX = false;
                }
                else if (curve.GetXAxis(pane).Scale == scale)
                {
                    rect = new RectangleF(scale.Rect.X + _rect.X, scale.Rect.Y + _rect.Y, _rect.Width, _rect.Height);
                    px = mousePt.X - rect.X;
                }

                //Get the histogram values depending on the given scale
                double[] histValues = isX ? curve._histogram.XHistogram : curve._histogram.YHistogram;
                double min = isX ? curve._histogram.XMin : curve._histogram.YMin;
                double max = isX ? curve._histogram.XMax : curve._histogram.YMax;
                double step = isX ? curve._histogram.XStep : curve._histogram.YStep;

                if (histValues != null && rect.Contains(mousePt))
                {
                    float maxPx = isX ? rect.Width : rect.Height;

                    if (px > 0 && px < maxPx)
                    {
                        //Translate pixel into a bin index
                        double pctPx = px / maxPx;
                        double xValue = scale.Min + (pctPx * Math.Abs(scale.Min - scale.Max));
                        int idx = (int)((xValue - min) / step);

                        if (idx >= 0 && idx < histValues.Length)
                            strValues.AppendLine(String.Format("{0}: {1:0.0#}%", curve.Label.Text, (histValues[idx] / curve.NPts) * 100));
                    }
                }
            }

            return strValues.ToString();
        }

        internal void Draw(Graphics g, GraphPane pane, Pen pen, Scale scale, float topPix, float rightPix, float width, float scaleFactor)
        {
            // draw the minor grid
            if (_isVisible)
            {
                float scaledeWidth = CalcSpace(scaleFactor);

                if (scaledeWidth > 0F)
                {
                    _rect = new RectangleF(0.0F, topPix, rightPix, /*topPix + */scaledeWidth);

                    g.DrawRectangle(pen, 0.0F, topPix, rightPix, /*topPix + */scaledeWidth);

                    bool isX = scale._ownerAxis is XAxis || scale._ownerAxis is X2Axis;
                    bool isY2 = scale._ownerAxis is Y2Axis;
                    bool isY = scale._ownerAxis is YAxis;
                    bool isZ = !isX && !isY && !isY2;

                    //Select scale curves
                    System.Collections.Generic.List<CurveItem> curves = pane.CurveList;

                    if(!isZ)
                    {
                        Predicate<CurveItem> matches = delegate(CurveItem item) { return isX ? item.GetXAxis(pane)._scaleHistogram == this : item.GetYAxis(pane).ScaleHistogram == this; };

                        curves = pane.CurveList.FindAll(matches);
                    }
                    
                    if (curves.Count > 0)
                    {
                        Matrix saveMatrix = g.Transform;

                        // Move the origin to the BottomLeft of the ChartRect, which is the left
                        //side of the X axis (facing from the label side)
                        g.TranslateTransform(0, _rect.Bottom);
                        g.ScaleTransform(1, -1);
                        if (!isX)
                        {
                            if(isY2 || isZ)
                            {
                                g.ScaleTransform(-1, 1);
                                g.ScaleTransform(-1, 1);
                            }
                            else
                            {
                                g.TranslateTransform(_rect.Right, 0);
                                g.ScaleTransform(-1, 1);
                            }
                        }

                        foreach (CurveItem curve in curves)
                        {
                            if(curve._histogram == null)
                            {
                                curve._histogram = new PointPairListHistogram(curve.Points, scale, this.IsAutoNBin ? -1 : this.NBin);
                            }

                            PointPairListHistogram hist = curve._histogram;
							// LCastro 11/18/2021
							// Separated X and Y axis histograms
                            if(!hist.IsReadyY && (isY || isY2))
                            {
                                hist.NBinY = this.NBin;
                                hist.CalculateY();
                            }

                            if (!hist.IsReadyX && isX)
                            {
                                hist.NBinX = this.NBin;
                                hist.CalculateX();
                            }

                            if(hist.IsReadyX && hist.IsReadyY)
                            {
                                //Draw lines
                                float yPx = 0F, xPx = 0F;
                                float lastX = Single.NaN,
                                    lastY = Single.NaN;

                                double yValue = 0d, xValue = 0d;
                                double min = 0d, max = 0d, step = 1d;

                                min = isX ? hist.XMin : isZ ? hist.ZMin : hist.YMin;
                                max = isX ? hist.XMax : isZ ? hist.ZMax : hist.YMax;
                                step = isX ? hist.XStep : isZ ? hist.ZStep : hist.YStep;

                                int nBin = isX ? hist.NBinX : hist.NBinY;

                                for (int n = 0; n < nBin; n++)
                                {
                                    xValue = min + (n * step);
                                    yValue = isX ? hist.XHistogram[n] / hist.XMaxFreq : 
                                        isZ ? hist.ZHistogram[n] / hist.ZMaxFreq : 
                                        hist.YHistogram[n] / hist.YMaxFreq;

                                    if (xValue >= scale.Min && xValue <= scale.Max)
                                    {
                                        double pct = (xValue - scale.Min) / Math.Abs(scale.Min - scale.Max);
                                        xPx = (float)pct * _rect.Width;
                                        yPx = ((float)yValue * scaledeWidth);

                                        using (Pen tPen = new Pen(curve.Color, 2f))
                                        {
                                            if(n == 0)
                                            {
                                                g.DrawLine(tPen, xPx, 0, xPx, yPx);
                                            }
                                            else 
                                            {
                                                g.DrawLine(tPen, lastX, lastY, xPx, lastY);
                                                g.DrawLine(tPen, xPx, lastY, xPx, yPx);
                                            }
                                        }

                                        lastX = xPx;
                                        lastY = yPx;
                                    }
                                }
                            }
                        }

                        g.Transform = saveMatrix;
                    }
                }
            }
        }

        internal void DrawHiFreqArea(Graphics g, GraphPane pane, Scale scale, RectangleF rect)
        {
            // draw the minor grid
            if (_isVisible && _isHiFreqVisible)
            {
                bool isX = scale._ownerAxis is XAxis || scale._ownerAxis is X2Axis;
                bool isY2 = scale._ownerAxis is Y2Axis;
                bool isY = scale._ownerAxis is YAxis;
                bool isZ = !isX && !isY && !isY2;

                //Select scale curves
                System.Collections.Generic.List<CurveItem> curves = pane.CurveList;

                if(!isZ)
                {
                    Predicate<CurveItem> matches = delegate(CurveItem item) { return isX ? item.GetXAxis(pane)._scaleHistogram == this : item.GetYAxis(pane).ScaleHistogram == this; };

                    curves = pane.CurveList.FindAll(matches);
                }

                if (curves.Count > 0)
                {
                    Matrix saveMatrix = g.Transform;

                    g.TranslateTransform(rect.X, rect.Height + rect.Y);
                    g.ScaleTransform(1, -1);
                    if (!isX)
                    {
                        if (isY2 || isZ)
                        {
                            g.TranslateTransform(rect.Right - rect.X, 0);
                            g.ScaleTransform(-1, 1);
                        }
                    }

                    foreach (CurveItem curve in curves)
                    {
                        if (curve._histogram == null)
                        {
                            curve._histogram = new PointPairListHistogram(curve.Points, scale, this.NBin);
                        }

                        PointPairListHistogram hist = curve._histogram;

                        if (!hist.IsReadyY && (isY || isY2))
                        {
                            hist.NBinY = this.NBin;
                            hist.CalculateY();
                        }

                        if (!hist.IsReadyX && isX)
                        {
                            hist.NBinX = this.NBin;
                            hist.CalculateX();
                        }

                        if (hist.IsReadyX && hist.IsReadyY)
                        {
                            //Draw lines
                            float x1Px = 0F, x2Px = 0F;
                            double x1Value = 0d, x2Value = 0d;
                            double min = isX ? hist.XMin : isZ ? hist.ZMin : hist.YMin;
                            double max = isX ? hist.XMax : isZ ? hist.ZMax : hist.YMax;
                            double step = isX ? hist.XStep : isZ ? hist.ZStep : hist.YStep;
                            double maxFreq = isX ? hist.XMaxFreq : isZ ? hist.ZMaxFreq : hist.YMaxFreq;
                            float h = isX ? rect.Height : rect.Width;
                            double freq = 0d, pct = 0d;
                            int nBin = isX ? hist.NBinX : hist.NBinY;
                            using (Brush brush = new SolidBrush(Color.FromArgb(40, curve.Color)))
                            {
                                for (int n = 0; n < nBin; n++)
                                {
                                    freq = isX ? hist.XHistogram[n] : isZ ? hist.ZHistogram[n]: hist.YHistogram[n];

                                    if (freq == maxFreq)
                                    {
                                        x1Value = min + (n * step);
                                        x2Value = x1Value + step;

                                        //all area is visible
                                        if (x1Value >= scale.Min && x2Value <= scale.Max)
                                        {
                                            pct = (x1Value - scale.Min) / Math.Abs(scale.Min - scale.Max);
                                            x1Px = (float)pct * _rect.Width;
                                            pct = (x2Value - scale.Min) / Math.Abs(scale.Min - scale.Max);
                                            x2Px = (float)pct * _rect.Width;

                                            g.FillRectangle(brush, isX ? x1Px : 0, isX ? 0 : x1Px, isX ? x2Px - x1Px : h, isX ? h : x2Px - x1Px);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    g.Transform = saveMatrix;
                }
            }
        }
    }
}
