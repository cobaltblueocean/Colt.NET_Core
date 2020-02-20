using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Hep.Aida.Bin
{
    /// <summary>
    /// Applies a function to one bin argument.
    /// </summary>
    /// <param name="x">the argument passed to the function.</param>
    /// <returns>the result of the function.</returns>
    public delegate double BinFunction1D(DynamicBin1D x);

    /// <summary>
    /// Applies a function to two bin arguments.
    /// </summary>
    /// <param name="x">the first argument passed to the function.</param>
    /// <param name="y">the second argument passed to the function.</param>
    /// <returns>the result of the function.</returns>
    public delegate double BinBinFunction1D(DynamicBin1D x, DynamicBin1D y);

    public static class BinFunctions1D
    {
        /// <summary>
        /// Function that returns <tt>x.Max()</tt>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D Max
        {
            get { return x => x.Max; }
        }

        /// <summary>
        /// Function that returns <i>x => x.Mean()</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D Mean
        {
            get { return x => x.Mean(); }
        }

        /// <summary>
        /// Function that returns <i>x => x.Median()</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D Median
        {
            get { return x => x.Median(); }
        }

        /// <summary>
        /// Function that returns <i>x => x.min()</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D Min
        {
            get { return x => x.Min; }
        }

        /// <summary>
        /// Function that returns <i>x => x.rms()</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D Rms
        {
            get { return x => x.Rms(); }
        }

        /// <summary>
        /// Function that returns <i>x => x.Count</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D Size
        {
            get { return x => x.Size; }
        }

        /// <summary>
        /// Function that returns <i>x => x.standardDeviation()</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D StandardDeviation
        {
            get { return x => x.StandardDeviation(); }
        }

        /// <summary>
        /// Function that returns <i>x => x.sum()</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D Sum
        {
            get { return x => x.Sum; }
        }

        /// <summary>
        /// Function that returns <i>x => x.sumOfLogarithms()</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D SumOfLogarithms
        {
            get { return x => x.SumOfLogarithms; }
        }

        /// <summary>
        /// Function that returns <i>x => x.geometricMean()</i>.
        /// </summary>
        /// <returns></returns>
        public static BinFunction1D GeometricMean()
        {
            return x => x.GeometricMean();
        }
    }
}
