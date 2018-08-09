// <copyright file="Histogram1D.cs" company="CERN">
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
    public class Histogram1D : AbstractHistogram1D , IHistogram1D
    {
        private double[] errors;
        private double[] heights;
        private int[] entries;
        private int nEntry; // total number of times fill called
        private double sumWeight; // Sum of all weights
        private double sumWeightSquared; // Sum of the squares of the weights
        private double mean, rms;

        public override double Mean
        {
            get
            {
                return mean / sumWeight;
            }
        }

        public override double EquivalentBinEntries
        {
            get
            {
                return sumWeight * sumWeight / sumWeightSquared;
            }
        }

        public override double Rms
        {
            get
            {
                return System.Math.Sqrt(rms / sumWeight - mean * mean / sumWeight / sumWeight);
            }
        }

        /// <summary>
        /// Creates a variable-width histogram.
        /// </summary>
        /// <param name="title">The histogram title.</param>
        /// <param name="edges">the bin boundaries the axis shall have; must be sorted ascending and must not contain multiple identical elements.</param>
        /// <exception cref="ArgumentException">if <i>edges.Length &lt; 1</i>.</exception>
        /// <example>
        /// <i>edges = (0.2, 1.0, 5.0)</i> yields an axis with 2 in-range bins <i>[0.2,1.0), [1.0,5.0)</i> and 2 extra bins <i>[-inf,0.2), [5.0,inf]</i>.
        /// </example>
        public Histogram1D(String title, double[] edges): this(title, new VariableAxis(edges))
        {
            
        }

        /// <summary>
        /// Creates a fixed-width histogram.
        /// </summary>
        /// <param name="title">The histogram title.</param>
        /// <param name="bins">The number of bins.</param>
        /// <param name="min">The minimum value on the X axis.</param>
        /// <param name="max">The maximum value on the X axis.</param>
        public Histogram1D(String title, int bins, double min, double max): this(title, new FixedAxis(bins, min, max))
        {
            
        }

        /// <summary>
        /// Creates a histogram with the given axis binning.
        /// </summary>
        /// <param name="title">The histogram title.</param>
        /// <param name="axis">The axis description to be used for binning.</param>
        public Histogram1D(String title, IAxis axis): base(title)
        {
            
            XAxis = axis;
            int bins = axis.Bins;
            entries = new int[bins + 2];
            heights = new double[bins + 2];
            errors = new double[bins + 2];
        }

        public override int AllEntries // perhaps to be deleted (default impld in baseclass sufficient)
        {
            get { return nEntry; }
        }

        public override int BinEntries(int index)
        {
            //return entries[xAxis.map(index)];
            return entries[Map(index)];
        }

        public override double BinError(int index)
        {
            //return System.Math.Sqrt(errors[xAxis.map(index)]);
            return System.Math.Sqrt(errors[Map(index)]);
        }

        public override double BinHeight(int index)
        {
            //return heights[xAxis.map(index)];
            return heights[Map(index)];
        }

        public override void Fill(double x, double weight)
        {
            //int bin = xAxis.getBin(x);
            int bin = Map(XAxis.CoordToIndex(x));
            entries[bin]++;
            heights[bin] += weight;
            errors[bin] += weight * weight;
            nEntry++;
            sumWeight += weight;
            sumWeightSquared += weight * weight;
            mean += x * weight;
            rms += x * weight * weight;
        }

        public override void Fill(double x)
        {
            //int bin = xAxis.getBin(x);
            int bin = Map(XAxis.CoordToIndex(x));
            entries[bin]++;
            heights[bin]++;
            errors[bin]++;
            nEntry++;
            sumWeight++;
            sumWeightSquared++;
            mean += x;
            rms += x * x;
        }

        public override void Reset()
        {
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i] = 0;
                heights[i] = 0;
                errors[i] = 0;
            }
            nEntry = 0;
            sumWeight = 0;
            sumWeightSquared = 0;
            mean = 0;
            rms = 0;
        }


        /// <summary>
        /// Used internally for creating slices and projections
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="heights"></param>
        /// <param name="errors"></param>
        internal void SetContents(int[] entries, double[] heights, double[] errors)
        {
            this.entries = entries;
            this.heights = heights;
            this.errors = errors;

            for (int i = 0; i < entries.Length; i++)
            {
                nEntry += entries[i];
                sumWeight += heights[i];
            }
            // TODO: Can we do anything sensible/useful with the other statistics?
            sumWeightSquared = Double.NaN;
            mean = Double.NaN;
            rms = Double.NaN;
        }
    }
}
