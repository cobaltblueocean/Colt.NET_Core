using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Hep.Aida
{
    /// <summary>
    /// An IAxis represents a binned histogram axis. A 1D Histogram would have
    /// one Axis representing the X axis, while a 2D Histogram would have two
    /// axes representing the X and Y Axis.
    /// </summary>
    public interface IAxis
    {
        /// <summary>
        /// Centre of the bin specified.
        /// </summary>
        /// <param name="index">Bin number (0...bins()-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns></returns>
        double BinCentre(int index);

        /// <summary>
        /// Lower edge of the specified bin.
        /// </summary>
        /// <param name="index">Bin number (0...bins()-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns>the lower edge of the bin; for the underflow bin this is <tt>Double.NEGATIVE_INFINITY</tt>.</returns>
        double BinLowerEdge(int index);

        /// <summary>
        /// The number of bins (excluding underflow and overflow) on the axis.
        /// </summary>
        /// <returns></returns>
        int Bins();

        /// <summary>
        /// Upper edge of the specified bin.
        /// </summary>
        /// <param name="index">Bin number (0...bins()-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns>the upper edge of the bin; for the overflow bin this is <tt>Double.POSITIVE_INFINITY</tt>.</returns>
        double BinUpperEdge(int index);

        /// <summary>
        /// Width of the bin specified.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        double BinWidth(int index);

        /// <summary>
        /// Converts a coordinate on the axis to a bin number. If the coordinate is &lt; lowerEdge returns UNDERFLOW, and if the coordinate is &gt;= upperEdge returns OVERFLOW.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        int CoordToIndex(double coord);

        /// <summary>
        /// Lower axis edge.
        /// </summary>
        /// <returns></returns>
        double LowerEdge();

        /// <summary>
        /// Upper axis edge.
        /// </summary>
        /// <returns></returns>
        double UpperEdge();
    }
}
