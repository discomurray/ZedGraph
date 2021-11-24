using System;

namespace ZedGraph
{
    /// <summary>
    /// Perform Frequency calculation for a IPointList
    /// </summary>
    public class PointPairListHistogram
    {
        private double[] _xHistValues, _yHistValues, _zHistValues;

        private double _xStep, _yStep, _zStep;

        private double _xMin = Double.MaxValue, _yMin = Double.MaxValue, _zMin = Double.MaxValue;

        private double _xMax = Double.MinValue, _yMax = Double.MinValue, _zMax = Double.MinValue;

        private int _xMaxFreq = 0, _yMaxFreq = 0, _zMaxFreq = 0;

        private IPointList _pts;
		// LCastro 11/18/2021
		// added ready flags and bin number for all axes
        private int _nBinsX;

        private int _nBinsY;

        private int _nBinsZ;

        private bool _readyX;

        private bool _readyY;

        private bool _readyZ;

        private Scale _scale;

        public PointPairListHistogram(IPointList pts, Scale scale, int nBins)
        {
            _pts = pts;
            _scale = scale;
            _nBinsX = nBins;
            _nBinsY = nBins;
            _nBinsZ = nBins;
            _readyX = false;
            _readyY = false;
            _readyZ = false;
        }

        public bool IsReadyX
        {
            get { return _readyX; }
            set { _readyX = value; }
        }
        public bool IsReadyY
        {
            get { return _readyY; }
            set { _readyY = value; }
        }
        public bool IsReadyZ
        {
            get { return _readyZ; }
            set { _readyZ = value; }
        }

        public double[] XHistogram
        {
            get
            {
                if (_xHistValues == null)
                {
                    Calculate();
                }

                return _xHistValues;
            }
        }

        public double[] YHistogram
        {
            get
            {
                if (_yHistValues == null)
                {
                    Calculate();
                }

                return _yHistValues;
            }
        }

        public double[] ZHistogram
        {
            get
            {
                if (_zHistValues == null)
                {
                    Calculate();
                }

                return _zHistValues;
            }
        }

        public double XStep { get { return _xStep; } }

        public double YStep { get { return _yStep; } }

        public double ZStep { get { return _zStep; } }

        public double XMin { get { return _xMin; } }

        public double XMax { get { return _xMax; } }

        public double YMin { get { return _yMin; } }

        public double YMax { get { return _yMax; } }

        public double ZMin { get { return _zMin; } }

        public double ZMax { get { return _zMax; } }

        public int XMaxFreq { get { return _xMaxFreq; } }

        public int YMaxFreq { get { return _yMaxFreq; } }

        public int ZMaxFreq { get { return _zMaxFreq; } }

        public int NBinX
        {
            get
            {
                if (_nBinsX <= 0)
                {
                    if (_scale != null)
                    {
                        //bins get from the width of the scale
                        _nBinsX = GetNBins();
                    }
                    else
                    {
                        //bins get from the sqrt of total points
                        _nBinsX = (int)Math.Sqrt(_pts.Count);
                    }
                }
                return _nBinsX;
            }
            set
            {
                _nBinsX = value;
            }
        }
        public int NBinY
        {
            get
            {
                if (_nBinsY <= 0)
                {
                    if (_scale != null)
                    {
                        //bins get from the width of the scale
                        _nBinsY = GetNBins();
                    }
                    else
                    {
                        //bins get from the sqrt of total points
                        _nBinsY = (int)Math.Sqrt(_pts.Count);
                    }
                }
                return _nBinsY;
            }
            set
            {
                _nBinsY = value;
            }
        }
        public int NBinZ
        {
            get
            {
                if (_nBinsZ <= 0)
                {
                    if (_scale != null)
                    {
                        //bins get from the width of the scale
                        _nBinsZ = GetNBins();
                    }
                    else
                    {
                        //bins get from the sqrt of total points
                        _nBinsZ = (int)Math.Sqrt(_pts.Count);
                    }
                }
                return _nBinsZ;
            }
            set
            {
                _nBinsZ = value;
            }
        }

        /// <summary>
        /// Calculate the number of bins depending on the graphic size of the scale.
        /// Each bin will  not be bigger than 10 pixels
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        private int GetNBins()
        {
            if (_pts.Count > 1)
            {
                bool isX = _scale._ownerAxis is XAxis || _scale._ownerAxis is X2Axis;
                bool isY = _scale._ownerAxis is YAxis || _scale._ownerAxis is Y2Axis;
                bool isZ = !isX && !isY;

                double value = Double.NaN, min = Double.MaxValue, max = Double.MinValue;

                int i = 0;

                for (; i < _pts.Count; i++)
                {
                    value = isX ? _pts[i].X : isZ ? _pts[i].Z : _pts[i].Y;
                    min = Math.Min(value, min);
                    max = Math.Max(value, max);
                }

                float unitPixels = (isX ? _scale.Rect.Width : _scale.Rect.Height) / Convert.ToSingle(Math.Abs(_scale.Min - _scale.Max));

                float width = Convert.ToSingle(Math.Abs(min - max)) * unitPixels;

                return Convert.ToInt32(width / 10);
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nBins"></param>
        /// <returns></returns>
        public bool Calculate()
        {
        	//LCastro 11/18/2021
        	//Separated calculation by axis
            CalculateX();
            CalculateY();
            CalculateZ();
            return (_readyX && _readyY && _readyZ);
        }

        public bool CalculateX()
        {
            _xMin = Double.MaxValue; _xMax = Double.MinValue;

            int nX = this.NBinX;

            _xStep = 0d;

            if (nX > 1)
            {
                int i = 0;

                for (; i < _pts.Count; i++)
                {
                    _xMin = Math.Min(_pts[i].X, _xMin);
                    _xMax = Math.Max(_pts[i].X, _xMax);
                }

                _xHistValues = new double[nX];

                _xStep = Math.Abs(_xMin - _xMax) / nX;

                int idx = -1;
                for (i = 0; i < _pts.Count; i++)
                {
                    if (_xStep > 0d)
                    {
                        idx = (int)((_pts[i].X - _xMin) / _xStep);
                        if (idx < 0)
                            _xHistValues[0]++;
                        else if (idx >= nX)
                            _xHistValues[nX - 1]++;
                        else
                            _xHistValues[idx]++;
                    }
                }
                for (i = 0; i < nX; i++)
                {
                    _xMaxFreq = Math.Max((int)_xHistValues[i], _xMaxFreq);
                }
                _readyX = true;
            }
            return _readyX;
        }
        public bool CalculateY()
        {
            _yMin = Double.MaxValue; _yMax = Double.MinValue;
            int nY = this.NBinY;
            _yStep = 0d;
            if (nY > 1)
            {
                int i = 0;

                for (; i < _pts.Count; i++)
                {
                    _yMin = Math.Min(_pts[i].Y, _yMin);
                    _yMax = Math.Max(_pts[i].Y, _yMax);
                }

                _yHistValues = new double[nY];
                _yStep = Math.Abs(_yMin - _yMax) / nY;

                int idx = -1;
                for (i = 0; i < _pts.Count; i++)
                {
                    if (_yStep > 0d)
                    {
                        idx = (int)((_pts[i].Y - _yMin) / _yStep);
                        if (idx < 0)
                            _yHistValues[0]++;
                        else if (idx >= nY)
                            _yHistValues[nY - 1]++;
                        else
                            _yHistValues[idx]++;
                    }
                }
                for (i = 0; i < nY; i++)
                {
                    _yMaxFreq = Math.Max((int)_yHistValues[i], _yMaxFreq);
                }
                _readyY = true;
            }
            return _readyY;
        }
        public bool CalculateZ()
        {
            _zMin = Double.MaxValue; _zMax = Double.MinValue;
            int nZ = this.NBinZ;
            _zStep = 0d;

            if (nZ > 1)
            {
                int i = 0;

                for (; i < _pts.Count; i++)
                {
                    _zMin = Math.Min(_pts[i].Z, _zMin);
                    _zMax = Math.Max(_pts[i].Z, _zMax);
                }
                _zHistValues = new double[nZ];

                _zStep = Math.Abs(_zMin - _zMax) / nZ;

                int idx = -1;
                for (i = 0; i < _pts.Count; i++)
                {
                    if (_zStep > 0d)
                    {
                        idx = (int)((_pts[i].Z - _zMin) / _zStep);
                        if (idx < 0)
                            _zHistValues[0]++;
                        else if (idx >= nZ)
                            _zHistValues[nZ - 1]++;
                        else
                            _zHistValues[idx]++;
                    }

                }
                for (i = 0; i < nZ; i++)
                {
                    _zMaxFreq = Math.Max((int)_zHistValues[i], _zMaxFreq);
                }
                _readyZ = true;
            }
            return _readyZ;
        }

    }
}
