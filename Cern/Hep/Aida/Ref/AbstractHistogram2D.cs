// <copyright file="AbstractHistogram2D.cs" company="CERN">
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
    public abstract class AbstractHistogram2D : Histogram, IHistogram2D
    {
        protected IAxis xAxis, yAxis;

        public AbstractHistogram2D(String title) : base(title)
        {
        }

        #region Abstract Methods, Properties
        public abstract double MeanX { get; }

        public abstract double MeanY { get; }

        public abstract double RmsX { get; }

        public abstract double RmsY { get; }

        public override abstract double EquivalentBinEntries { get; }

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
        /// <param name="title"></param>
        /// <param name="indexY1"></param>
        /// <param name="indexY2"></param>
        /// <returns></returns>
        protected abstract IHistogram1D InternalSliceX(String title, int indexY1, int indexY2);

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
        protected abstract IHistogram1D InternalSliceY(String title, int indexX1, int indexX2);

        public abstract int BinEntries(int indexX, int indexY);

        public abstract double BinError(int indexX, int indexY);

        public abstract double BinHeight(int indexX, int indexY);

        public abstract void Fill(double x, double y, double weight);

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
                int maxBinX = -1;
                int maxBinY = -1;
                for (int i = xAxis.Bins; --i >= 0;)
                {
                    for (int j = yAxis.Bins; --j >= 0;)
                    {
                        double value = BinHeight(i, j);
                        if (value < minValue)
                        {
                            minValue = value;
                            minBinX = i;
                            minBinY = j;
                        }
                        if (value > maxValue)
                        {
                            maxValue = value;
                            maxBinX = i;
                            maxBinY = j;
                        }
                    }
                }
                int[] result = { minBinX, minBinY, maxBinX, maxBinY };
                return result;
            }
        }

        public virtual IHistogram1D ProjectionX
        {
            get
            {
                String newTitle = Title + " (projectionX)";
                //return internalSliceX(newTitle,yAxis.under,yAxis.over);
                return InternalSliceX(newTitle, MapY(HistogramType.UNDERFLOW.ToInt()), MapY(HistogramType.OVERFLOW.ToInt()));
            }
        }

        public virtual IHistogram1D ProjectionY
        {
            get
            {
                String newTitle = Title + " (projectionY)";
                //return internalSliceY(newTitle,xAxis.under,xAxis.over);
                return InternalSliceY(newTitle, MapX(HistogramType.UNDERFLOW.ToInt()), MapX(HistogramType.OVERFLOW.ToInt()));
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

        public override int AllEntries
        {
            get
            {
                int n = 0;
                for (int i = xAxis.Bins; --i >= -2;)
                    for (int j = yAxis.Bins; --j >= -2;)
                    {
                        n += BinEntries(i, j);
                    }
                return n;
            }
        }

        public override int Dimensions
        {
            get
            {
                return 2;
            }
        }

        public override int Entries
        {
            get
            {
                int n = 0;
                for (int i = 0; i < xAxis.Bins; i++)
                    for (int j = 0; j < yAxis.Bins; j++)
                    {
                        n += BinEntries(i, j);
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
                    {
                        n += BinHeight(i, j);
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
                    {
                        n += BinHeight(i, j);
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

        public virtual int BinEntriesX(int indexX)
        {
            return ProjectionX.BinEntries(indexX);
        }

        public virtual int BinEntriesY(int indexY)
        {
            return ProjectionY.BinEntries(indexY);
        }

        public virtual double BinHeightX(int indexX)
        {
            return ProjectionX.BinHeight(indexX);
        }

        public double BinHeightY(int indexY)
        {
            return ProjectionY.BinHeight(indexY);
        }

        public virtual void Fill(double x, double y)
        {
            Fill(x, y, 1);
        }

        /// <summary>
        /// Package private method to map from the external representation of bin
        /// number to our internal representation of bin number
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual int MapX(int index)
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
        public virtual int MapY(int index)
        {
            int bins = yAxis.Bins + 2;
            if (index >= bins) throw new ArgumentException("bin=" + index);
            if (index >= 0) return index + 1;
            if (index == HistogramType.UNDERFLOW.ToInt()) return 0;
            if (index == HistogramType.OVERFLOW.ToInt()) return bins - 1;
            throw new ArgumentException("bin=" + index);
        }

        public virtual IHistogram1D SliceX(int indexY)
        {
            //int start = yAxis.map(indexY);
            int start = MapY(indexY);
            String newTitle = Title + " (sliceX [" + indexY + "])";
            return InternalSliceX(newTitle, start, start);
        }

        public virtual IHistogram1D SliceX(int indexY1, int indexY2)
        {
            //int start = yAxis.map(indexY1);
            //int stop = yAxis.map(indexY2);
            int start = MapY(indexY1);
            int stop = MapY(indexY2);
            String newTitle = Title + " (sliceX [" + indexY1 + ":" + indexY2 + "])";
            return InternalSliceX(newTitle, start, stop);
        }

        public virtual IHistogram1D SliceY(int indexX)
        {
            //int start = xAxis.map(indexX);
            int start = MapX(indexX);
            String newTitle = Title + " (sliceY [" + indexX + "])";
            return InternalSliceY(newTitle, start, start);
        }

        public virtual IHistogram1D SliceY(int indexX1, int indexX2)
        {
            //int start = xAxis.map(indexX1);
            //int stop = xAxis.map(indexX2);
            int start = MapX(indexX1);
            int stop = MapX(indexX2);
            String newTitle = Title + " (slicey [" + indexX1 + ":" + indexX2 + "])";
            return InternalSliceY(newTitle, start, stop);
        }
    }
}
