// <copyright file="Histogram2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
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
    public class Histogram2D: AbstractHistogram2D, IHistogram2D
    {
        private double[][] heights;
        private double[][] errors;
        private int[][] entries;
        private int nEntry; // total number of times fill called
        private double sumWeight; // Sum of all weights
        private double sumWeightSquared; // Sum of the squares of the weights
        private double meanX, rmsX;
        private double meanY, rmsY;

        public override double MeanX
        {
            get
            {
                return meanX / sumWeight;
            }
        }

        public override double MeanY
        {
            get
            {
                return meanY / sumWeight;
            }
        }

        public override double RmsX
        {
            get
            {
                return System.Math.Sqrt(rmsX / sumWeight - meanX * meanX / sumWeight / sumWeight);
            }
        }

        public override double RmsY
        {
            get
            {
                return System.Math.Sqrt(rmsY / sumWeight - meanY * meanY / sumWeight / sumWeight);
            }
        }

        public override double EquivalentBinEntries
        {
            get
            {
                return sumWeight * sumWeight / sumWeightSquared;
            }
        }

        public new double SumAllBinHeights
        {
            get
            {
                return sumWeight;
            }
        }

        /// <summary>
        /// Creates a variable-width histogram.
        /// </summary>
        /// <param name="title">The histogram title.</param>
        /// <param name="xEdges">the bin boundaries the x-axis shall have; must be sorted ascending and must not contain multiple identical elements.</param>
        /// <param name="yEdges">the bin boundaries the y-axis shall have; must be sorted ascending and must not contain multiple identical elements.</param>
        /// <exception cref="ArgumentException">if <i>xEdges.Length &lt; 1 || yEdges.Length &lt; 1</i>.</exception>
        /// <example>
        /// <i>xEdges = (0.2, 1.0, 5.0, 6.0), yEdges = (-5, 0, 7)</i> yields 3*2 in-range bins.
        /// </example>
        public Histogram2D(String title, double[] xEdges, double[] yEdges): this(title, new VariableAxis(xEdges), new VariableAxis(yEdges))
        {
            
        }

        /// <summary>
        /// Creates a fixed-width histogram.
        /// </summary>
        /// <param name="title">The histogram title.</param>
        /// <param name="xBins">The number of bins on the X axis.</param>
        /// <param name="xMin">The minimum value on the X axis.</param>
        /// <param name="xMax">The maximum value on the X axis.</param>
        /// <param name="yBins">The number of bins on the Y axis.</param>
        /// <param name="yMin">The minimum value on the Y axis.</param>
        /// <param name="yMax">The maximum value on the Y axis.</param>
        public Histogram2D(String title, int xBins, double xMin, double xMax,
                                         int yBins, double yMin, double yMax): this(title, new FixedAxis(xBins, xMin, xMax), new FixedAxis(yBins, yMin, yMax))
        {
            
        }

        /// <summary>
        /// Creates a histogram with the given axis binning.
        /// </summary>
        /// <param name="title">The histogram title.</param>
        /// <param name="xAxis">The x-axis description to be used for binning.</param>
        /// <param name="yAxis">The y-axis description to be used for binning.</param>
        public Histogram2D(String title, IAxis xAxis, IAxis yAxis): base(title)
        {
            
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            int xBins = xAxis.Bins;
            int yBins = yAxis.Bins;

            entries = new int[xBins + 2, yBins + 2].ToJagged();
            heights = new double[xBins + 2,yBins + 2].ToJagged();
            errors = new double[xBins + 2, yBins + 2].ToJagged();
        }

        protected override IHistogram1D InternalSliceX(string title, int indexY1, int indexY2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexY2 < indexY1) throw new ArgumentException("Invalid bin range");

            int sliceBins = xAxis.Bins + 2;
            int[] sliceEntries = new int[sliceBins];
            double[] sliceHeights = new double[sliceBins];
            double[] sliceErrors = new double[sliceBins];

            //for (int i=xAxis.under; i<=xAxis.over; i++)
            for (int i = 0; i < sliceBins; i++)
            {
                for (int j = indexY1; j <= indexY2; j++)
                {
                    sliceEntries[i] += entries[i][j];
                    sliceHeights[i] += heights[i][j];
                    sliceErrors[i] += errors[i][j];
                }
            }
            Histogram1D result = new Histogram1D(title, xAxis);
            result.SetContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
        }

        protected override IHistogram1D InternalSliceY(string title, int indexX1, int indexX2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexX2 < indexX1) throw new ArgumentException("Invalid bin range");

            int sliceBins = yAxis.Bins + 2;
            int[] sliceEntries = new int[sliceBins];
            double[] sliceHeights = new double[sliceBins];
            double[] sliceErrors = new double[sliceBins];

            for (int i = indexX1; i <= indexX2; i++)
            {
                //for (int j=yAxis.under; j<=yAxis.over; j++)
                for (int j = 0; j < sliceBins; j++)
                {
                    sliceEntries[j] += entries[i][j];
                    sliceHeights[j] += heights[i][j];
                    sliceErrors[j] += errors[i][j];
                }
            }
            Histogram1D result = new Histogram1D(title, yAxis);
            result.SetContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
        }

        public override int AllEntries // perhaps to be deleted (default impld in baseclass sufficient)
        {
            get { return nEntry; }
        }

        public override int BinEntries(int indexX, int indexY)
        {
            //return entries[xAxis.map(indexX)][yAxis.map(indexY)];
            return entries[MapX(indexX)][MapY(indexY)];
        }

        public override double BinError(int indexX, int indexY)
        {
            //return System.Math.Sqrt(errors[xAxis.map(indexX)][yAxis.map(indexY)]);
            return System.Math.Sqrt(errors[MapX(indexX)][MapY(indexY)]);
        }

        public override double BinHeight(int indexX, int indexY)
        {
            //return heights[xAxis.map(indexX)][yAxis.map(indexY)];
            return heights[MapX(indexX)][MapY(indexY)];
        }

        public new void Fill(double x, double y)
        {
            //int xBin = xAxis.getBin(x);
            //int yBin = xAxis.getBin(y);
            int xBin = MapX(xAxis.CoordToIndex(x));
            int yBin = MapY(yAxis.CoordToIndex(y));
            entries[xBin][yBin]++;
            heights[xBin][yBin]++;
            errors[xBin][yBin]++;
            nEntry++;
            sumWeight++;
            sumWeightSquared++;
            meanX += x;
            rmsX += x;
            meanY += y;
            rmsY += y;
        }

        public override void Fill(double x, double y, double weight)
        {
            //int xBin = xAxis.getBin(x);
            //int yBin = xAxis.getBin(y);
            int xBin = MapX(xAxis.CoordToIndex(x));
            int yBin = MapY(yAxis.CoordToIndex(y));
            entries[xBin][yBin]++;
            heights[xBin][yBin] += weight;
            errors[xBin][yBin] += weight * weight;
            nEntry++;
            sumWeight += weight;
            sumWeightSquared += weight * weight;
            meanX += x * weight;
            rmsX += x * weight * weight;
            meanY += y * weight;
            rmsY += y * weight * weight;
        }

        public override void Reset()
        {
            for (int i = 0; i < entries.Length; i++)
                for (int j = 0; j < entries.GetLength(1); j++)
                {
                    entries[i][j] = 0;
                    heights[i][j] = 0;
                    errors[i][j] = 0;
                }
            nEntry = 0;
            sumWeight = 0;
            sumWeightSquared = 0;
            meanX = 0;
            rmsX = 0;
            meanY = 0;
            rmsY = 0;
        }


        /// <summary>
        /// Used internally for creating slices and projections
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="heights"></param>
        /// <param name="errors"></param>
        internal void setContents(int[][] entries, double[][] heights, double[][] errors)
        {
            this.entries = entries;
            this.heights = heights;
            this.errors = errors;

            for (int i = 0; i < entries.Length; i++)
                for (int j = 0; j < entries.GetLength(1); j++)
                {
                    nEntry += entries[i][j];
                    sumWeight += heights[i][j];
                }
            // TODO: Can we do anything sensible/useful with the other statistics?
            sumWeightSquared = Double.NaN;
            meanX = Double.NaN;
            rmsX = Double.NaN;
            meanY = Double.NaN;
            rmsY = Double.NaN;
        }
    }
}
