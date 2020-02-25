// <copyright file="AbstractHistogram1D.cs" company="CERN">
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
    public abstract class AbstractHistogram1D : Histogram, IHistogram1D
    {
        private IAxis xAxis;

        /// <summary>
        /// Constant specifying the overflow bin (can be passed to any method expecting a bin number).
        /// </summary>
        private static int OVERFLOW = -1;

        /// <summary>
        /// Constant specifying the underflow bin (can be passed to any method expecting a bin number).
        /// </summary>
        private static int UNDERFLOW = -2;

        public AbstractHistogram1D(String title) : base(title)
        {

        }

        #region Abstract Methods
        public abstract double Mean {get;}

        public abstract int BinEntries(int index);

        public abstract double BinError(int index);

        public abstract double BinHeight(int index);

        public abstract override double EquivalentBinEntries { get; }

        public abstract void Fill(double x, double weight);

        public abstract void Fill(double x);

        public abstract override void Reset();

        public abstract double Rms { get; }
        #endregion

        public override int AllEntries
        {
            get { return Entries + ExtraEntries; }
        }

        public override int Dimensions
        {
            get { return 1; }
        }

        public override int Entries
        {
            get
            {
            int entries = 0;
            for (int i = xAxis.Bins; --i >= 0;) entries += BinEntries(i);
            return entries;
            }
        }

        public override int ExtraEntries
        {
            get
            {
                //return entries[xAxis.under] + entries[xAxis.over];
                return BinEntries(UNDERFLOW) + BinEntries(OVERFLOW);
            }
        }

        protected int Map(int index)
        {
            int bins = xAxis.Bins + 2;
            if (index >= bins) throw new ArgumentException("bin=" + index);
            if (index >= 0) return index + 1;
            if (index == UNDERFLOW) return 0;
            if (index == OVERFLOW) return bins - 1;
            throw new ArgumentException("bin=" + index);
        }

        public int[] MinMaxBins
        {
            get
            {
                double minValue = Double.MaxValue;
                double maxValue = Double.MinValue;
                int minBinX = -1;
                int maxBinX = -1;
                for (int i = xAxis.Bins; --i >= 0;)
                {
                    double value = BinHeight(i);
                    if (value < minValue)
                    {
                        minValue = value;
                        minBinX = i;
                    }
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxBinX = i;
                    }
                }
                int[] result = { minBinX, maxBinX };
                return result;
            }
        }

        public override double SumAllBinHeights
        {
            get
            {
                return SumBinHeights + SumExtraBinHeights;
            }
        }

        public override double SumBinHeights
        {
            get
            {
                double sum = 0;
                for (int i = xAxis.Bins; --i >= 0;) sum += BinHeight(i);
                return sum;
            }
        }

        public override double SumExtraBinHeights
        {
            get
            {
                return BinHeight(UNDERFLOW) + BinHeight(OVERFLOW);
                //return heights[xAxis.under] + heights[xAxis.over];
            }
        }

        public IAxis XAxis
        {
            get{ return xAxis; }
            set { xAxis = value; }
        }
    }
}
