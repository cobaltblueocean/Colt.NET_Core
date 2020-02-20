// <copyright file="AbstractHistogram3D.cs" company="CERN">
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
    public abstract class AbstractHistogram3D: Histogram, IHistogram3D
    {
        protected IAxis xAxis, yAxis, zAxis;

        public AbstractHistogram3D(String title): base(title)
        {
    }

        #region Abstract Methods, Properties
        /// <summary>
        /// The precise meaning of the arguments to the public slice
        /// methods is somewhat ambiguous, so we define this internal
        /// slice method and clearly specify its arguments.
        /// <p>
        /// <b>Note 0</b>indexX1 and indexX2 use our INTERNAL bin numbering scheme
        /// <b>Note 1</b>The slice is done between indexX1 and indexX2 INCLUSIVE
        /// <b>Note 2</b>indexX1 and indexX2 may include the use of under and over flow bins
        /// <b>Note 3</b>There is no note 3 (yet)
        /// </summary>
        protected abstract IHistogram2D InternalSliceXY(String title, int indexZ1, int indexZ2);

        /// <summary>
        /// The precise meaning of the arguments to the public slice
        /// methods is somewhat ambiguous, so we define this internal
        /// slice method and clearly specify its arguments.
        /// <p>
        /// <b>Note 0</b>indexY1 and indexY2 use our INTERNAL bin numbering scheme
        /// <b>Note 1</b>The slice is done between indexY1 and indexY2 INCLUSIVE
        /// <b>Note 2</b>indexY1 and indexY2 may include the use of under and over flow bins
        /// <b>Note 3</b>There is no note 3 (yet)
        /// </summary>
        protected abstract IHistogram2D InternalSliceXZ(String title, int indexY1, int indexY2);

        /// <summary>
        /// The precise meaning of the arguments to the public slice
        /// methods is somewhat ambiguous, so we define this internal
        /// slice method and clearly specify its arguments.
        /// <p>
        /// <b>Note 0</b>indexX1 and indexX2 use our INTERNAL bin numbering scheme
        /// <b>Note 1</b>The slice is done between indexX1 and indexX2 INCLUSIVE
        /// <b>Note 2</b>indexX1 and indexX2 may include the use of under and over flow bins
        /// <b>Note 3</b>There is no note 3 (yet)
        /// </summary>
        protected abstract IHistogram2D InternalSliceYZ(String title, int indexX1, int indexX2);

        public abstract double MeanX { get; }

        public abstract double MeanY { get; }

        public abstract double MeanZ { get; }

        public abstract double RmsX { get; }

        public abstract double RmsY { get; }

        public abstract double RmsZ { get; }

        public abstract override double EquivalentBinEntries { get; }

        public abstract int BinEntries(int indexX, int indexY, int indexZ);

        public abstract double BinError(int indexX, int indexY, int indexZ);

        public abstract double BinHeight(int indexX, int indexY, int indexZ);

        public abstract void Fill(double x, double y, double z, double weight);

        public abstract override void Reset();

        #endregion

        public virtual int[] MinMaxBins
        {
            get
            {
                double minValue = Double.MaxValue;
                double maxValue = Double.MinValue;
                int minBinX = -1;
                int minBinY = -1;
                int minBinZ = -1;
                int maxBinX = -1;
                int maxBinY = -1;
                int maxBinZ = -1;
                for (int i = xAxis.Bins; --i >= 0;)
                {
                    for (int j = yAxis.Bins; --j >= 0;)
                    {
                        for (int k = zAxis.Bins; --k >= 0;)
                        {
                            double value = BinHeight(i, j, k);
                            if (value < minValue)
                            {
                                minValue = value;
                                minBinX = i;
                                minBinY = j;
                                minBinZ = k;
                            }
                            if (value > maxValue)
                            {
                                maxValue = value;
                                maxBinX = i;
                                maxBinY = j;
                                maxBinZ = k;
                            }
                        }
                    }
                }
                int[] result = { minBinX, minBinY, minBinZ, maxBinX, maxBinY, maxBinZ };
                return result;
            }
        }

        public virtual IHistogram2D ProjectionXY
        {
            get
            {
                String newTitle = Title + " (projectionXY)";
                return InternalSliceXY(newTitle, MapZ(HistogramType.UNDERFLOW.ToInt()), MapZ(HistogramType.OVERFLOW.ToInt()));
            }
        }

        public virtual IHistogram2D ProjectionXZ
        {
            get
            {
                String newTitle = Title + " (projectionXZ)";
                return InternalSliceXZ(newTitle, MapY(HistogramType.UNDERFLOW.ToInt()), MapY(HistogramType.OVERFLOW.ToInt()));
            }
        }

        public virtual IHistogram2D ProjectionYZ
        {
            get
            {
                String newTitle = Title + " (projectionYZ)";
                return InternalSliceYZ(newTitle, MapX(HistogramType.UNDERFLOW.ToInt()), MapX(HistogramType.OVERFLOW.ToInt()));
            }
        }

        public virtual IAxis XAxis
        {
            get
            {
                return xAxis;
            }
            set
            {
                xAxis = value;
            }
        }

        public virtual IAxis YAxis
        {
            get
            {
                return yAxis;
            }
            set
            {
                yAxis = value;
            }
        }

        public virtual IAxis ZAxis
        {
            get
            {
                return zAxis;
            }
            set
            {
                zAxis = value;
            }
        }

        public override int AllEntries
        {
            get
            {
                int n = 0;
                for (int i = xAxis.Bins; --i >= -2;)
                    for (int j = yAxis.Bins; --j >= -2;)
                        for (int k = zAxis.Bins; --k >= -2;)
                        {
                            n += BinEntries(i, j, k);
                        }
                return n;
            }
        }

        public override int Dimensions
        {
            get { return 3; }
        }

        public override int Entries
        {
            get
            {
                int n = 0;
                for (int i = 0; i < xAxis.Bins; i++)
                    for (int j = 0; j < yAxis.Bins; j++)
                        for (int k = 0; k < zAxis.Bins; k++)
                        {
                            n += BinEntries(i, j, k);
                        }
                return n;
            }
        }

        public override int ExtraEntries
        {
            get
            {
                return AllEntries - Entries;
            }
        }

        public override double SumAllBinHeights
        {
            get
            {
                double n = 0;
                for (int i = xAxis.Bins; --i >= -2;)
                    for (int j = yAxis.Bins; --j >= -2;)
                        for (int k = zAxis.Bins; --k >= -2;)
                        {
                            n += BinHeight(i, j, k);
                        }
                return n;
            }
        }

        public override double SumBinHeights
        {
            get
            {
                double n = 0;
                for (int i = 0; i < xAxis.Bins; i++)
                    for (int j = 0; j < yAxis.Bins; j++)
                        for (int k = 0; k < zAxis.Bins; k++)
                        {
                            n += BinHeight(i, j, k);
                        }
                return n;
            }
        }

        public override double SumExtraBinHeights
        {
            get
            {
                return SumAllBinHeights - SumBinHeights;
            }
        }

        public virtual void Fill(double x, double y, double z)
        {
            Fill(x, y, z, 1);
        }

        /// <summary>
        /// Package private method to map from the external representation of bin
        /// number to our internal representation of bin number
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual int MapX(int index)
        {
            int bins = xAxis.Bins + 2;
            if (index >= bins) throw new ArgumentException("bin=" + index);
            if (index >= 0) return index + 1;
            if (index == HistogramType.UNDERFLOW.ToInt()) return 0;
            if (index == HistogramType.OVERFLOW.ToInt()) return bins - 1;
            throw new ArgumentException("bin=" + index);
        }

        /// <summary>
        /// Package private method to map from the external representation of bin
        /// number to our internal representation of bin number
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual int MapY(int index)
        {
            int bins = yAxis.Bins + 2;
            if (index >= bins) throw new ArgumentException("bin=" + index);
            if (index >= 0) return index + 1;
            if (index == HistogramType.UNDERFLOW.ToInt()) return 0;
            if (index == HistogramType.OVERFLOW.ToInt()) return bins - 1;
            throw new ArgumentException("bin=" + index);
        }

        /// <summary>
        /// Package private method to map from the external representation of bin
        /// number to our internal representation of bin number
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual int MapZ(int index)
        {
            int bins = zAxis.Bins + 2;
            if (index >= bins) throw new ArgumentException("bin=" + index);
            if (index >= 0) return index + 1;
            if (index == HistogramType.UNDERFLOW.ToInt()) return 0;
            if (index == HistogramType.OVERFLOW.ToInt()) return bins - 1;
            throw new ArgumentException("bin=" + index);
        }

        public virtual IHistogram2D SliceXY(int indexZ)
        {
            return SliceXY(indexZ, indexZ);
        }

        public virtual IHistogram2D SliceXY(int indexZ1, int indexZ2)
        {
            int start = MapZ(indexZ1);
            int stop = MapZ(indexZ2);
            String newTitle = Title + " (sliceXY [" + indexZ1 + ":" + indexZ2 + "])";
            return InternalSliceXY(newTitle, start, stop);
        }

        public virtual IHistogram2D SliceXZ(int indexY)
        {
            return SliceXZ(indexY, indexY);
        }

        public virtual IHistogram2D SliceXZ(int indexY1, int indexY2)
        {
            int start = MapY(indexY1);
            int stop = MapY(indexY2);
            String newTitle = Title + " (sliceXZ [" + indexY1 + ":" + indexY2 + "])";
            return InternalSliceXY(newTitle, start, stop);
        }

        public virtual IHistogram2D SliceYZ(int indexX)
        {
            return SliceYZ(indexX, indexX);
        }

        public virtual IHistogram2D SliceYZ(int indexX1, int indexX2)
        {
            int start = MapX(indexX1);
            int stop = MapX(indexX2);
            String newTitle = Title + " (sliceYZ [" + indexX1 + ":" + indexX2 + "])";
            return InternalSliceYZ(newTitle, start, stop);
        }
    }
}
