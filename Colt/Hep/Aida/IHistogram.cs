using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Hep.Aida
{
    /// <summary>
    /// A common base interface for IHistogram1D, IHistogram2D and IHistogram3D.
    /// </summary>
    public interface IHistogram
    {
        /// <summary>
        /// Represent specifying the overflow/underflow bin (can be passed to any method expecting a bin number).
        /// </summary>
        HistogramType Status { get; set; }

        /// <summary>
        /// Number of all entries in all (both in-range and under/overflow) bins in the histogram.
        /// </summary>
        /// <returns></returns>
        int AllEntries();

        /// <summary>
        /// Returns 1 for one-dimensional histograms, 2 for two-dimensional histograms, and so on.
        /// </summary>
        /// <returns></returns>
        int Dimensions();

        /// <summary>
        /// Number of in-range entries in the histogram.
        /// </summary>
        /// <returns></returns>
        int Entries();

        /// <summary>
        /// Number of equivalent entries.
        /// </summary>
        /// <returns><i>SUM[ weight ] ^ 2 / SUM[ weight^2 ]</i>.</returns>
        double EquivalentBinEntries();

        /// <summary>
        /// Number of under and overflow entries in the histogram.
        /// </summary>
        /// <returns></returns>
        int ExtraEntries();

        /// <summary>
        /// Reset contents; as if just constructed.
        /// </summary>
        void Reset();

        /// <summary>
        /// Sum of all (both in-range and under/overflow) bin heights in the histogram.
        /// </summary>
        /// <returns></returns>
        double SumAllBinHeights();

        /// <summary>
        /// Sum of in-range bin heights in the histogram.
        /// </summary>
        /// <returns></returns>
        double SumBinHeights();

        /// <summary>
        /// Sum of under/overflow bin heights in the histogram.
        /// </summary>
        /// <returns></returns>
        double SumExtraBinHeights();

        /// <summary>
        /// Title of the histogram (will be set only in the constructor).
        /// </summary>
        /// <returns></returns>
        String Title();
    }

    public enum HistogramType
    {
        OVERFLOW = -1,
        UNDERFLOW = -2
    }
}
