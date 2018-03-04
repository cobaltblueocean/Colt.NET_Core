// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Functions.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Function delegates operatind on doubles matrices.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Colt.Matrix.DoubleAlgorithms
{
    /// <summary>
    /// A comparison function which imposes a <i>total ordering</i> on some collection of elements. 
    /// </summary>
    /// <param name="o1">
    /// The first matrix.
    /// </param>
    /// <param name="o2">
    /// The second matrix.
    /// </param>
    /// <returns>
    /// A negative integer, zero, or a positive integer as the first argument is less than, equal
    /// to, or greater than the second.
    /// </returns>
    public delegate int DoubleMatrix1DComparator(DoubleMatrix1D o1, DoubleMatrix1D o2);
}
