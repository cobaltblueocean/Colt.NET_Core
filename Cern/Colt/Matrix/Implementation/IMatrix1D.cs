// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Abstract base class for 1-d matrices (aka <i>vectors</i>) holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    public interface IMatrix1D<T>
    {
        int Size { get; set; }
        int Stride { get; set; }
        int Zero { get; set; }
        T this[int index] { get; set; }
        void CheckSize(IMatrix1D<T> b);
        int Index(int rank);
        public IMatrix1D<T> VFlip();
        public IMatrix1D<T> VPart(int index, int width);
        public IMatrix1D<T> VStrides(int str);
        string ToString();
        string ToString(int index);
        string ToStringShort();
    }
}