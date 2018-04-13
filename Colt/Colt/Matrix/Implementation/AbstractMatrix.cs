// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractMatrix.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Abstract base class for arbitrary-dimensional matrices holding objects or primitive data types such as <code>int</code>, <code>float</code>, etc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    /// Abstract base class for arbitrary-dimensional matrices holding objects or primitive data types such as <code>int</code>, <code>float</code>, etc.
    /// </summary>
    public abstract class AbstractMatrix : PersistentObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the receiver is a view or not.
        /// </summary>
        protected bool isView { get; set; }

        /// <summary>
        /// Returns the number of cells.
        /// </summary>
        /// <returns>
        /// The number of cells.
        /// </returns>
        public abstract int Size();
    }
}
