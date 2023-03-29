// <copyright file="AbstractMatrix3D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
namespace Cern.Colt.Matrix.Implementation
{
    public interface IMatrix3D<T>
    {
        int Columns { get; }
        int Rows { get; }
        int Size { get; }
        int Slices { get; }
        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <tt>[row,column]</tt>.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        T this[int slice, int row, int column] { get; set; }
        void CheckShape(IMatrix3D<T> B);
        void CheckShape(IMatrix3D<T> B, IMatrix3D<T> C);
        string ToString(int slice, int row, int column);
        string ToStringShort();
        IMatrix3D<T> VColumnFlip();
        IMatrix3D<T> VDice(int axis0, int axis1, int axis2);
        IMatrix3D<T> VPart(int slice, int row, int column, int depth, int height, int width);
        IMatrix3D<T> VRowFlip();
        IMatrix3D<T> VSliceFlip();
        IMatrix3D<T> VStrides(int Slicestride, int Rowstride, int Columnstride);
    }
}