using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Hep.Aida
{
    /// <summary>
    /// A C# interface corresponding to the AIDA 2D Histogram.
    /// <p> 
    /// <b>Note</b> All methods that accept a bin number as an argument will
    /// also accept the constants OVERFLOW or UNDERFLOW as the argument, and
    /// as a result give the contents of the resulting OVERFLOW or UNDERFLOW
    /// bin.
    /// </summary>
    public interface IHistogram2D : IHistogram
    {
        /// <summary>
        /// The number of entries(ie the number of times fill was called for this bin).
        /// </summary>
        /// <param name="indexX">the x bin number(0...Nx-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexY">the y bin number (0...Ny-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns></returns>
        int BinEntries(int indexX, int indexY);

        /// <summary>
        /// Equivalent to <tt>projectionX().binEntries(indexX)</tt>.
        /// </summary>
        int BinEntriesX(int indexX);

        /// <summary>
        /// Equivalent to <tt>projectionY().binEntries(indexY)</tt>.
        /// </summary>
        int BinEntriesY(int indexY);

        /// <summary>
        /// The error on this bin.
        /// </summary>
        /// <param name="indexX">the x bin number(0...Nx-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexY">the y bin number (0...Ny-1) or OVERFLOW or UNDERFLOW.</param>
        double BinError(int indexX, int indexY);

        /// <summary>
        /// Total height of the corresponding bin (ie the sum of the weights in this bin).
        /// </summary>
        /// <param name="indexX">the x bin number(0...Nx-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexY">the y bin number (0...Ny-1) or OVERFLOW or UNDERFLOW.</param>
        double BinHeight(int indexX, int indexY);

        /// <summary>
        /// Equivalent to <tt>projectionX().binHeight(indexX)</tt>.
        /// </summary>
        /// <param name="indexX">the x bin number(0...Nx-1) or OVERFLOW or UNDERFLOW.</param>
        double BinHeightX(int indexX);

        /// <summary>
        /// Equivalent to <tt>projectionY().binHeight(indexY)</tt>.
        /// </summary>
        /// <param name="indexY">the y bin number (0...Ny-1) or OVERFLOW or UNDERFLOW.</param>
        double BinHeightY(int indexY);

        /// <summary>
        /// Fill the histogram with weight 1.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void Fill(double x, double y);

        /// <summary>
        /// Fill the histogram with specified weight.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void Fill(double x, double y, double weight);

        /// <summary>
        ///  Returns the mean of the histogram, as calculated on filling-time projected on the X axis.
        /// </summary>
        double MeanX();

        /// <summary>
        ///  Returns the mean of the histogram, as calculated on filling-time projected on the Y axis.
        /// </summary>
        double MeanY();

        /// <summary> 
        /// Indexes of the in-range bins containing the smallest and largest binHeight(), respectively.
        /// </summary>
        /// <returns><tt>{minBinX,minBinY, maxBinX,maxBinY}</tt>.</returns>
        int[] MinMaxBins();

        /// <summary>
        /// Create a projection parallel to the X axis.
        /// Equivalent to <tt>sliceX(UNDERFLOW,OVERFLOW)</tt>.
        /// </summary>
        IHistogram1D ProjectionX();

        /// <summary>
        /// Create a projection parallel to the Y axis.
        /// Equivalent to <tt>sliceY(UNDERFLOW,OVERFLOW)</tt>.
        /// </summary>
        IHistogram1D ProjectionY();

        /// <summary>
        /// Returns the rms of the histogram as calculated on filling-time projected on the X axis.
        /// </summary>
        double RmsX();

        /// <summary>
        /// Returns the rms of the histogram as calculated on filling-time projected on the Y axis.
        /// </summary>
        double RmsY();

        /// <summary>
        /// Slice parallel to the Y axis at bin indexY and one bin wide.
        /// Equivalent to <tt>sliceX(indexY,indexY)</tt>.
        /// </summary>
        /// <param name="indexY"></param>
        IHistogram1D SliceX(int indexY);

        /// <summary>
        /// Create a slice parallel to the axis X axis, between "indexY1" and "indexY2" (inclusive).
        /// The returned IHistogram1D represents an instantaneous snapshot of the
        /// histogram at the time the slice was created.
        /// </summary>
        /// <param name="indexY1"></param>
        /// <param name="indexY2"></param>
        IHistogram1D SliceX(int indexY1, int indexY2);

        /// <summary>
        /// Slice parallel to the X axis at bin indexX and one bin wide.
        /// Equivalent to <tt>sliceY(indexX,indexX)</tt>.
        /// </summary>
        /// <param name="indexX"></param>
        IHistogram1D SliceY(int indexX);

        /// <summary>
        /// Create a slice parallel to the axis Y axis, between "indexX1" and "indexX2" (inclusive)
        /// The returned IHistogram1D represents an instantaneous snapshot of the
        /// histogram at the time the slice was created.
        /// </summary>
        /// <param name="indexX1"></param>
        /// <param name="indexX2"></param>
        IHistogram1D SliceY(int indexX1, int indexX2);

        /// <summary>
        /// Return the X axis.
        /// </summary>
        IAxis XAxis();

        /// <summary>
        /// Return the Y axis.
        /// </summary>
        IAxis YAxis();
    }
}
