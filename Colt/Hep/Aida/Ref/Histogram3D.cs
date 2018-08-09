// <copyright file="Histogram3D.cs" company="CERN">
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
    public class Histogram3D: AbstractHistogram3D, IHistogram3D
    {
        private double[][][] heights;
        private double[][][] errors;
        private int[][][] entries;
        private int nEntry; // total number of times fill called
        private double sumWeight; // Sum of all weights
        private double sumWeightSquared; // Sum of the squares of the weights
        private double meanX, rmsX;
        private double meanY, rmsY;
        private double meanZ, rmsZ;

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

        public override double MeanZ
        {
            get
            {
                return meanZ / sumWeight;
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

        public override double RmsZ
        {
            get
            {
                return System.Math.Sqrt(rmsZ / sumWeight - meanZ * meanZ / sumWeight / sumWeight);
            }
        }

        public override double EquivalentBinEntries
        {
            get
            {
                return sumWeight * sumWeight / sumWeightSquared;
            }
        }

        public override double SumAllBinHeights
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
        /// <param name="zEdges">the bin boundaries the z-axis shall have; must be sorted ascending and must not contain multiple identical elements.</param>
        /// <exception cref="ArgumentException">if <i>xEdges.Length &lt; 1 || yEdges.Length &lt; 1|| zEdges.Length &lt; 1</i>.</exception>
        /// <example>
        /// <i>xEdges = (0.2, 1.0, 5.0, 6.0), yEdges = (-5, 0, 7), zEdges = (-5, 0, 7)</i> yields 3*2*2 in-range bins.
        /// </example>
        public Histogram3D(String title, double[] xEdges, double[] yEdges, double[] zEdges):this(title, new VariableAxis(xEdges), new VariableAxis(yEdges), new VariableAxis(zEdges))
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
        /// <param name="zBins">The number of bins on the Z axis.</param>
        /// <param name="zMin">The minimum value on the Z axis.</param>
        /// <param name="zMax">The maximum value on the Z axis.</param>
        public Histogram3D(String title, int xBins, double xMin, double xMax,
                                         int yBins, double yMin, double yMax,
                                         int zBins, double zMin, double zMax):this(title, new FixedAxis(xBins, xMin, xMax), new FixedAxis(yBins, yMin, yMax), new FixedAxis(zBins, zMin, zMax))
        {
            
        }

        /// <summary>
        /// Creates a histogram with the given axis binning.
        /// </summary>
        /// <param name="title">The histogram title.</param>
        /// <param name="xAxis">The x-axis description to be used for binning.</param>
        /// <param name="yAxis">The y-axis description to be used for binning.</param>
        /// <param name="zAxis">The z-axis description to be used for binning.</param>
        public Histogram3D(String title, IAxis xAxis, IAxis yAxis, IAxis zAxis):base(title)
        {
            
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            this.zAxis = zAxis;
            int xBins = xAxis.Bins;
            int yBins = yAxis.Bins;
            int zBins = zAxis.Bins;

            entries = new int[xBins + 2,yBins + 2, zBins + 2].ToJagged();
            heights = new double[xBins + 2, yBins + 2, zBins + 2].ToJagged();
            errors = new double[xBins + 2, yBins + 2, zBins + 2].ToJagged();

        }

        protected override IHistogram2D InternalSliceXY(string title, int indexZ1, int indexZ2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexZ2 < indexZ1) throw new ArgumentException("Invalid bin range");

            int xBins = xAxis.Bins + 2;
            int yBins = yAxis.Bins + 2;
            int[][] sliceEntries = new int[xBins, yBins].ToJagged();
            double[][] sliceHeights = new double[xBins, yBins].ToJagged();
            double[][] sliceErrors = new double[xBins, yBins].ToJagged();

            for (int i = 0; i < xBins; i++)
            {
                for (int j = 0; j < yBins; j++)
                {
                    for (int k = indexZ1; k <= indexZ2; k++)
                    {
                        sliceEntries[i][j] += entries[i][j][k];
                        sliceHeights[i][j] += heights[i][j][k];
                        sliceErrors[i][j] += errors[i][j][k];
                    }
                }
            }
            Histogram2D result = new Histogram2D(title, xAxis, yAxis);
            result.setContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
        }

        protected override IHistogram2D InternalSliceXZ(string title, int indexY1, int indexY2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexY2 < indexY1) throw new ArgumentException("Invalid bin range");

            int xBins = xAxis.Bins + 2;
            int zBins = zAxis.Bins + 2;
            int[][] sliceEntries = new int[xBins, zBins].ToJagged();
            double[][] sliceHeights = new double[xBins, zBins].ToJagged();
            double[][] sliceErrors = new double[xBins,zBins].ToJagged();

            for (int i = 0; i < xBins; i++)
            {
                for (int j = indexY1; j <= indexY2; j++)
                {
                    for (int k = 0; i < zBins; k++)
                    {
                        sliceEntries[i][k] += entries[i][j][k];
                        sliceHeights[i][k] += heights[i][j][k];
                        sliceErrors[i][k] += errors[i][j][k];
                    }
                }
            }
            Histogram2D result = new Histogram2D(title, xAxis, zAxis);
            result.setContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
        }

        protected override IHistogram2D InternalSliceYZ(string title, int indexX1, int indexX2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexX2 < indexX1) throw new ArgumentException("Invalid bin range");

            int yBins = yAxis.Bins + 2;
            int zBins = zAxis.Bins + 2;
            int[][] sliceEntries = new int[yBins, zBins].ToJagged();
            double[][] sliceHeights = new double[yBins,zBins].ToJagged();
            double[][] sliceErrors = new double[yBins, zBins].ToJagged();

            for (int i = indexX1; i <= indexX2; i++)
            {
                for (int j = 0; j < yBins; j++)
                {
                    for (int k = 0; k < zBins; k++)
                    {
                        sliceEntries[j][k] += entries[i][j][k];
                        sliceHeights[j][k] += heights[i][j][k];
                        sliceErrors[j][k] += errors[i][j][k];
                    }
                }
            }
            Histogram2D result = new Histogram2D(title, yAxis, zAxis);
            result.setContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
        }

        public override int AllEntries // perhaps to be deleted (default impld in baseclass sufficient)
        {
            get { return nEntry; }
        }

        public override int BinEntries(int indexX, int indexY, int indexZ)
        {
            return entries[MapX(indexX)][MapY(indexY)][MapZ(indexZ)];
        }

        public override double BinError(int indexX, int indexY, int indexZ)
        {
            return System.Math.Sqrt(errors[MapX(indexX)][MapY(indexY)][MapZ(indexZ)]);
        }

        public override double BinHeight(int indexX, int indexY, int indexZ)
        {
            return heights[MapX(indexX)][MapY(indexY)][MapZ(indexZ)];
        }

        public new void Fill(double x, double y, double z)
        {
            int xBin = MapX(xAxis.CoordToIndex(x));
            int yBin = MapY(yAxis.CoordToIndex(y));
            int zBin = MapZ(zAxis.CoordToIndex(z));
            entries[xBin][yBin][zBin]++;
            heights[xBin][yBin][zBin]++;
            errors[xBin][yBin][zBin]++;
            nEntry++;
            sumWeight++;
            sumWeightSquared++;
            meanX += x;
            rmsX += x;
            meanY += y;
            rmsY += y;
            meanZ += z;
            rmsZ += z;
        }

        public override void Fill(double x, double y, double z, double weight)
        {
            int xBin = MapX(xAxis.CoordToIndex(x));
            int yBin = MapY(yAxis.CoordToIndex(y));
            int zBin = MapZ(zAxis.CoordToIndex(z));
            entries[xBin][yBin][zBin]++;
            heights[xBin][yBin][zBin] += weight;
            errors[xBin][yBin][zBin] += weight * weight;
            nEntry++;
            sumWeight += weight;
            sumWeightSquared += weight * weight;
            meanX += x * weight;
            rmsX += x * weight * weight;
            meanY += y * weight;
            rmsY += y * weight * weight;
            meanZ += z * weight;
            rmsZ += z * weight * weight;
        }

        public override void Reset()
        {
            for (int i = 0; i < entries.Length; i++)
                for (int j = 0; j < entries.GetLength(1); j++)
                    for (int k = 0; j < entries[0].GetLength(1); k++)
                    {
                        entries[i][j][k] = 0;
                        heights[i][j][k] = 0;
                        errors[i][j][k] = 0;
                    }
            nEntry = 0;
            sumWeight = 0;
            sumWeightSquared = 0;
            meanX = 0;
            rmsX = 0;
            meanY = 0;
            rmsY = 0;
            meanZ = 0;
            rmsZ = 0;
        }
    }
}
