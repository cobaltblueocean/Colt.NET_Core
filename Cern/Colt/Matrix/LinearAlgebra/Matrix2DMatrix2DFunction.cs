// <copyright file="Matrix2DMatrix2DFunction.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.LinearAlgebra
{
    /// <summary>
    /// a delegate that a function to two arguments.
    /// </summary>
    /// <param name="x">the first argument passed to the function.</param>
    /// <param name="y">the second argument passed to the function.</param>
    /// <returns>the result of the function.</returns>
    public delegate double Matrix2DMatrix2DFunction(DoubleMatrix2D x, DoubleMatrix2D y);
}
