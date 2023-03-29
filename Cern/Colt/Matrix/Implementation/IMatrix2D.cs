// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractMatrix2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Abstract base class for 2-d matrices holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    public interface IMatrix2D<T>
    {
        int Columns { get; }
        int Rows { get; }
        int Size { get; }
        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <tt>[row,column]</tt>.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        T this[int row, int column] { get; set; }
        void CheckShape(IMatrix2D<T> b);
        void CheckShape(IMatrix2D<T> b, IMatrix2D<T> c);
        IMatrix2D<T> VDice();
        IMatrix2D<T> VColumnFlip();
        IMatrix2D<T> VPart(int row, int column, int height, int width);
        IMatrix2D<T> VRowFlip();
        IMatrix2D<T> VStrides(int rStride, int cStride);
        string ToString();
        string ToString(int row, int column);
        string ToStringShort();
    }
}