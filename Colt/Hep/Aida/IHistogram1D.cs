using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Hep.Aida
{
    /// <summary>
    /// A C# interface corresponding to the AIDA 1D Histogram.
/// <p> 
/// <b>Note</b> All methods that accept a bin number as an argument will
/// also accept the constants OVERFLOW or UNDERFLOW as the argument, and
/// as a result give the contents of the resulting OVERFLOW or UNDERFLOW
/// bin.
    /// </summary>
    public interface IHistogram1D: IHistogram
    {
        /// <summary>
        /// Number of entries in the corresponding bin (ie the number of times fill was called for this bin).
        /// </summary>
        /// <param name="index">the bin number (0...N-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns></returns>
        int BinEntries(int index);

        /// <summary>
        /// The error on this bin.
        /// </summary>
        /// <param name="index">the bin number (0...N-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns></returns>
        double BinError(int index);

        /// <summary>
        /// Total height of the corresponding bin (ie the sum of the weights in this bin).
        /// </summary>
        /// <param name="index">the bin number (0...N-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns></returns>
        double BinHeight(int index);

        /// <summary>
        /// Fill histogram with weight 1.
        /// </summary>
        /// <param name="x"></param>
        void Fill(double x);

        /// <summary>
        /// Fill histogram with specified weight.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="weight"></param>
        void Fill(double x, double weight);

        /// <summary>
        /// Returns the mean of the whole histogram as calculated on filling-time.
        /// </summary>
        /// <returns></returns>
        double Mean { get; }

        /// <summary>
        /// Indexes of the in-range bins containing the smallest and largest binHeight(), respectively.
        /// </summary>
        /// <returns><i>{minBin,maxBin}</i>.</returns>
        int[] MinMaxBins { get; }

        /// <summary>
        /// Returns the rms of the whole histogram as calculated on filling-time.
        /// </summary>
        /// <returns></returns>
        double Rms { get; }

        /// <summary>
        /// Returns the X Axis.
        /// </summary>
        /// <returns></returns>
        IAxis XAxis { get; }
    }
}
