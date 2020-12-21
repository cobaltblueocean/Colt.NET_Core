// <copyright file="VariableAxis.cs" company="CERN">
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
    public class VariableAxis : IAxis
    {
        private double min;
        private int bins;
        private double[] edges;

        public double Min
        {
            get { return min; }
            set { min = value; }
        }

        //public int Bins
        //{
        //    get { return bins; }
        //    set { bins = value; }
        //}

        public double[] Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        /// <summary>
        /// Constructs and returns an axis with the given bin edges.
        /// 
        /// </summary>
        /// <param name="edges">the bin boundaries the partition shall have; must be sorted ascending and must not contain multiple identical elements.</param>
        /// <example>
        /// <i>edges = (0.2, 1.0, 5.0)</i> yields an axis with 2 in-range bins <i>[0.2,1.0), [1.0,5.0)</i> and 2 extra bins <i>[-inf,0.2), [5.0,inf]</i>.
        /// </example>
        /// <exception cref="ArgumentException">if <i>edges.Length &lt; 1</i>.</exception>
        public VariableAxis(double[] edges)
        {
            if (edges.Length < 1) throw new ArgumentException();

            // check if really sorted and has no multiple identical elements
            for (int i = 0; i < edges.Length - 1; i++)
            {
                if (edges[i + 1] <= edges[i])
                {
                    throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_EdgesMustBeSorted);
                }
            }

            this.min = edges[0];
            this.bins = edges.Length - 1;
            this.edges = (double[])edges.Clone();
        }

        public double BinCentre(int index)
        {
            return (BinLowerEdge(index) + BinUpperEdge(index)) / 2;
        }

        public double BinLowerEdge(int index)
        {
            if (HistogramType.UNDERFLOW.Equals(index)) return Double.NegativeInfinity;
            if (HistogramType.OVERFLOW.Equals(index)) return UpperEdge;
            return edges[index];
        }

        public int Bins
        {
            get
            {
                return bins;
            }
        }

        public double BinUpperEdge(int index)
        {
            if (HistogramType.UNDERFLOW.Equals(index)) return LowerEdge;
            if (HistogramType.OVERFLOW.Equals(index)) return Double.PositiveInfinity;
            return edges[index + 1];
        }

        public double BinWidth(int index)
        {
            return BinUpperEdge(index) - BinLowerEdge(index);
        }

        public int CoordToIndex(double coord)
        {
            if (coord < min) return (int)HistogramType.UNDERFLOW.ToInt();

            int index = Array.BinarySearch(this.edges, coord);
            //int index = new List<Double>(this.edges).BinarySearch(coord); // just for debugging
            if (index < 0) index = -index - 1 - 1; // not found
                                                   //else index++; // found

            if (index >= bins) return HistogramType.OVERFLOW.ToInt();

            return index;
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
                return edges[edges.Length - 1];
            }
        }

        /**
	 * Returns a string representation of the specified arrayd  The string
	 * representation consists of a list of the arrays's elements, enclosed in square brackets
	 * (<i>"[]"</i>)d  Adjacent elements are separated by the characters
	 * <i>", "</i> (comma and space).
	 * @return a string representation of the specified array.
	 */
        protected static String ToString(double[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                    buf.Append(", ");
            }
            buf.Append("]");
            return buf.ToString();
        }
    }
}
