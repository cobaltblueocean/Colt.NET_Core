// <copyright file="Utility.cs" company="CERN">
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
    /// <summary>
    /// Convenient histogram utilitiesd 
    /// </summary>
    public class Utility
    {
        public Utility() { }

        public int MaxBin(IHistogram1D h)
        {
            int maxBin = -1;
            double maxValue = Double.MinValue;
            for (int i = h.XAxis.Bins; --i >= 0;)
            {
                double value = h.BinHeight(i);
                if (value > maxValue)
                {
                    maxValue = value;
                    maxBin = i;
                }
            }
            return maxBin;
        }

        /// <summary>
        /// Returns the indexX of the in-range bin containing the MaxBinHeight.
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public int MaxBinX(IHistogram2D h)
        {
            double maxValue = Double.MinValue;
            int maxBinX = -1;
            int maxBinY = -1;
            for (int i = h.XAxis.Bins; --i >= 0;)
            {
                for (int j = h.YAxis.Bins; --j >= 0;)
                {
                    double value = h.BinHeight(i, j);
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxBinX = i;
                        maxBinY = j;
                    }
                }
            }
            return maxBinX;
        }

        /// <summary>
        /// Returns the indexY of the in-range bin containing the MaxBinHeight.
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public int MaxBinY(IHistogram2D h)
        {
            double maxValue = Double.MinValue;
            int maxBinX = -1;
            int maxBinY = -1;
            for (int i = h.XAxis.Bins; --i >= 0;)
            {
                for (int j = h.YAxis.Bins; --j >= 0;)
                {
                    double value = h.BinHeight(i, j);
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxBinX = i;
                        maxBinY = j;
                    }
                }
            }
            return maxBinY;
        }

        /// <summary>
        /// Returns the index of the in-range bin containing the MinBinHeight.
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public int MinBin(IHistogram1D h)
        {
            int minBin = -1;
            double minValue = Double.MaxValue;
            for (int i = h.XAxis.Bins; --i >= 0;)
            {
                double value = h.BinHeight(i);
                if (value < minValue)
                {
                    minValue = value;
                    minBin = i;
                }
            }
            return minBin;
        }

        /// <summary>
        /// Returns the indexX of the in-range bin containing the MinBinHeight.
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public int MinBinX(IHistogram2D h)
        {
            double minValue = Double.MaxValue;
            int minBinX = -1;
            int minBinY = -1;
            for (int i = h.XAxis.Bins; --i >= 0;)
            {
                for (int j = h.YAxis.Bins; --j >= 0;)
                {
                    double value = h.BinHeight(i, j);
                    if (value < minValue)
                    {
                        minValue = value;
                        minBinX = i;
                        minBinY = j;
                    }
                }
            }
            return minBinX;
        }

        /// <summary>
        /// Returns the indexY of the in-range bin containing the MinBinHeight.
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public int MinBinY(IHistogram2D h)
        {
            double minValue = Double.MaxValue;
            int minBinX = -1;
            int minBinY = -1;
            for (int i = h.XAxis.Bins; --i >= 0;)
            {
                for (int j = h.YAxis.Bins; --j >= 0;)
                {
                    double value = h.BinHeight(i, j);
                    if (value < minValue)
                    {
                        minValue = value;
                        minBinX = i;
                        minBinY = j;
                    }
                }
            }
            return minBinY;
        }
    }
}
