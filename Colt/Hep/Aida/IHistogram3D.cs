using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Hep.Aida
{
    /// <summary>
    /// A C# interface corresponding to the AIDA 3D Histogram.
    /// <p> 
    /// <b>Note</b> All methods that accept a bin number as an argument will
    /// also accept the constants OVERFLOW or UNDERFLOW as the argument, and
    /// as a result give the contents of the resulting OVERFLOW or UNDERFLOW
    /// bin.
    /// </summary>
    public interface IHistogram3D : IHistogram
    {
        /// <summary>
        /// The number of entries (ie the number of times fill was called for this bin).
        /// </summary>
        /// <param name="indexX">the x bin number (0...Nx-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexY">the y bin number (0...Ny-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexZ">the z bin number (0...Nz-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns></returns>
        int BinEntries(int indexX, int indexY, int indexZ);

        /// <summary>
        /// The error on this bin.
        /// </summary>
        /// <param name="indexX">the x bin number (0...Nx-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexY">the y bin number (0...Ny-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexZ">the z bin number (0...Nz-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns></returns>
        double BinError(int indexX, int indexY, int indexZ);
        
        /// <summary>
        /// Total height of the corresponding bin (ie the sum of the weights in this bin).
        /// </summary>
        /// <param name="indexX">the x bin number (0...Nx-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexY">the y bin number (0...Ny-1) or OVERFLOW or UNDERFLOW.</param>
        /// <param name="indexZ">the z bin number (0...Nz-1) or OVERFLOW or UNDERFLOW.</param>
        /// <returns></returns>
        double BinHeight(int indexX, int indexY, int indexZ);
        
        /// <summary>
        /// Fill the histogram with weight 1; equivalent to <tt>fill(x,y,z,1)</tt>..
        /// </summary>
        void Fill(double x, double y, double z);
        
        /// <summary>
        /// Fill the histogram with specified weight.
        /// </summary>
        void Fill(double x, double y, double z, double weight);
        
        /// <summary>
        ///  Returns the mean of the histogram, as calculated on filling-time projected on the X axis.
        /// </summary> 
        double MeanX { get; }

        /// <summary>
        ///  Returns the mean of the histogram, as calculated on filling-time projected on the Y axis.
        /// </summary> 
        double MeanY { get; }

        /// <summary>
        ///  Returns the mean of the histogram, as calculated on filling-time projected on the Z axis.
        /// </summary> 
        double MeanZ { get; }

        /// <summary> 
        /// Indexes of the in-range bins containing the smallest and largest binHeight(), respectively.
        /// </summary> 
        /// <returns><tt>{minBinX,minBinY,minBinZ, maxBinX,maxBinY,maxBinZ}</tt>.</returns>
        int[] MinMaxBins { get; }

        /// <summary>
        /// Create a projection parallel to the XY plane.
        /// Equivalent to <tt>sliceXY(UNDERFLOW,OVERFLOW)</tt>.
        /// </summary>
        IHistogram2D ProjectionXY { get; }

        /// <summary>
        /// Create a projection parallel to the XZ plane.
        /// Equivalent to <tt>sliceXZ(UNDERFLOW,OVERFLOW)</tt>.
        /// </summary>
        IHistogram2D ProjectionXZ { get; }

        /// <summary>
        /// Create a projection parallel to the YZ plane.
        /// Equivalent to <tt>sliceYZ(UNDERFLOW,OVERFLOW)</tt>.
        /// </summary>
        IHistogram2D ProjectionYZ { get; }

        /// <summary>
        /// Returns the rms of the histogram as calculated on filling-time projected on the X axis.
        /// </summary>
        double RmsX { get; }

        /// <summary>
        /// Returns the rms of the histogram as calculated on filling-time projected on the Y axis.
        /// </summary>
        double RmsY { get; }

        /// <summary>
        /// Returns the rms of the histogram as calculated on filling-time projected on the Z axis.
        /// </summary>
        double RmsZ { get; }

        /// <summary>
        /// Create a slice parallel to the XY plane at bin indexZ and one bin wide.
        /// Equivalent to <tt>sliceXY(indexZ,indexZ)</tt>.
        /// </summary>
        IHistogram2D SliceXY(int indexZ);
        
        /// <summary>
        /// Create a slice parallel to the XY plane, between "indexZ1" and "indexZ2" (inclusive).
        /// The returned IHistogram2D represents an instantaneous snapshot of the
        /// histogram at the time the slice was created.
        /// </summary> 
        IHistogram2D SliceXY(int indexZ1, int indexZ2);
        
        /// <summary>
        /// Create a slice parallel to the XZ plane at bin indexY and one bin wide.
        /// Equivalent to <tt>sliceXZ(indexY,indexY)</tt>.
        /// </summary>
        IHistogram2D SliceXZ(int indexY);
        
        /// <summary>
        /// Create a slice parallel to the XZ plane, between "indexY1" and "indexY2" (inclusive).
        /// The returned IHistogram2D represents an instantaneous snapshot of the
        /// histogram at the time the slice was created.
        /// </summary> 
        IHistogram2D SliceXZ(int indexY1, int indexY2);
        
        /// <summary>
        /// Create a slice parallel to the YZ plane at bin indexX and one bin wide.
        /// Equivalent to <tt>sliceYZ(indexX,indexX)</tt>.
        /// </summary>
        IHistogram2D SliceYZ(int indexX);
        
        /// <summary>
        /// Create a slice parallel to the YZ plane, between "indexX1" and "indexX2" (inclusive).
        /// The returned IHistogram2D represents an instantaneous snapshot of the
        /// histogram at the time the slice was created.
        /// </summary> 
        IHistogram2D SliceYZ(int indexX1, int indexX2);
        
        /// <summary>
        /// Return the X axis.
        /// </summary>
        IAxis XAxis { get; }

        /// <summary>
        /// Return the Y axis.
        /// </summary>
        IAxis YAxis { get; }

        /// <summary>
        /// Return the Z axis.
        /// </summary>
        IAxis ZAxis { get; }

    }
}
