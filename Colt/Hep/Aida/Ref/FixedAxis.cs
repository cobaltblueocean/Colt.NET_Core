// <copyright file="FixedAxis.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Hep.Aida.Ref
{
    public class FixedAxis : IAxis
    {
        private int bins;
        private double min;
        private double binWidth;
        
        // Package private for ease of use in Histogram1D and Histogram2D
        private int xunder, xover;

        public int Bins
        {
            get
            {
                return bins;
            }
        }

        public double LowerEdge
        {
            get
            {
                return min;
            }
        }

        public double UpperEdge
        {
            get
            {
                return min + binWidth * bins;
            }
        }

        /// <summary>
        /// Create an Axis
        /// </summary>
        /// <param name="bins">Number of bins</param>
        /// <param name="min">Minimum for axis</param>
        /// <param name="max">Maximum for axis</param>
        public FixedAxis(int bins, double min, double max)
        {
            if (bins < 1) throw new ArgumentException("bins=" + bins);
            if (max <= min) throw new ArgumentException("max <= min");

            // Note, for internal consistency we save only min and binWidth
            // and always use these quantities for all calculationsd Due to 
            // rounding errors the return value from upperEdge is not necessarily
            // exactly equal to max

            this.bins = bins;
            this.min = min;
            this.binWidth = (max - min) / bins;

            // our internal definition of overflow/underflow differs from
            // that of the outside world
            //this.under = 0;
            //this.over = bins+1;
        }

        public double BinCentre(int index)
        {
            return min + binWidth * index + binWidth / 2;
        }

        public double BinLowerEdge(int index)
        {
            if (index == HistogramType.UNDERFLOW.ToInt()) return Double.NegativeInfinity;
            if (index == HistogramType.OVERFLOW.ToInt()) return UpperEdge;
            return min + binWidth * index;
        }

        public double BinUpperEdge(int index)
        {
            if (index == HistogramType.UNDERFLOW.ToInt()) return min;
            if (index == HistogramType.OVERFLOW.ToInt()) return Double.PositiveInfinity;
            return min + binWidth * (index + 1);
        }

        public double BinWidth(int index)
        {
            return binWidth;
        }

        public int CoordToIndex(double coord)
        {
            if (coord < min) return HistogramType.UNDERFLOW.ToInt();
            int index = (int)System.Math.Floor((coord - min) / binWidth);
            if (index >= bins) return HistogramType.OVERFLOW.ToInt();

            return index;
        }

        /// <summary>
        /// This package private method is similar to coordToIndex except
        /// that it returns our internal definition for overflow/underflow
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private int XGetBin(double coord)
        {
            if (coord < min) return xunder;
            int index = (int)System.Math.Floor((coord - min) / binWidth);
            if (index > bins) return xover;
            return index + 1;
        }

        /// <summary>
        /// Package private method to map from the external representation of bin
        /// number to our internal representation of bin number
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int XMap(int index)
        {
            if (index >= bins) throw new ArgumentException("bin=" + index);
            if (index >= 0) return index + 1;
            if (index == HistogramType.UNDERFLOW.ToInt()) return xunder;
            if (index == HistogramType.OVERFLOW.ToInt()) return xover;
            throw new ArgumentException("bin=" + index);
        }
    }
}
