// <copyright file="ObjectFactory3D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix.Implementation;

namespace Cern.Colt.Matrix
{
    /**
Factory for convenient construction of 3-d matrices holding <i>Object</i> cellsd 
Use idioms like <i>ObjectFactory3D.dense.make(4,4,4)</i> to construct dense matrices, 
<i>ObjectFactory3D.sparse.make(4,4,4)</i> to construct sparse matrices.

If the factory is used frequently it might be useful to streamline the notationd 
For example by aliasing:
<table>
<td class="PRE"> 
<pre>
ObjectFactory3D F = ObjectFactory3D.dense;
F.make(4,4,4);
...
</pre>
</td>
</table>

@author wolfgang.hoschek@cern.ch
@version 1.0, 09/24/99
*/
    /// <summary>
    /// 
    /// </summary>
    public class ObjectFactory3D
    {
        /// <summary>
        /// A factory producing dense matrices.
        /// </summary>
        private static ObjectFactory3D _dense = new ObjectFactory3D();

        public static ObjectFactory3D Dense
        {
            get { return _dense; }
        }

        /// <summary>
        /// A factory producing sparse matrices.
        /// </summary>
        private static ObjectFactory3D _sparse = new ObjectFactory3D();

        public static ObjectFactory3D Sparse
        {
            get { return _sparse; }
        }

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected ObjectFactory3D() { }


        /// <summary>
        /// Constructs a matrix with the given cell values.
        /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
        /// and have exactly the same number of slices, rows and columns as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">the values to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>values.Length != slices() || for any 0 &lt;= slice &lt; slices(): values[slice].Length != rows()</i>.</exception>
        /// <exception cref="ArgumentException">if <i>for any 0 &lt;= column &lt; columns(): values[slice][row].Length != columns()</i>.</exception>
        public ObjectMatrix3D Make(Object[,,] values)
        {
            return Make(values.ToJagged());
        }

        /// <summary>
        /// Constructs a matrix with the given cell values.
        /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
        /// and have exactly the same number of slices, rows and columns as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">the values to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>values.Length != slices() || for any 0 &lt;= slice &lt; slices(): values[slice].Length != rows()</i>.</exception>
        /// <exception cref="ArgumentException">if <i>for any 0 &lt;= column &lt; columns(): values[slice][row].Length != columns()</i>.</exception>
        public ObjectMatrix3D Make(Object[][][] values)
        {
            if (this == _sparse) return new SparseObjectMatrix3D(values);
            return new DenseObjectMatrix3D(values);
        }

        /// <summary>
        /// Constructs a matrix with the given shape, each cell initialized with zero.
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public ObjectMatrix3D Make(int slices, int rows, int columns)
        {
            if (this == _sparse) return new SparseObjectMatrix3D(slices, rows, columns);
            return new DenseObjectMatrix3D(slices, rows, columns);
        }

        /// <summary>
        /// Constructs a matrix with the given shape, each cell initialized with the given value.
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public ObjectMatrix3D Make(int slices, int rows, int columns, Object initialValue)
        {
            return Make(slices, rows, columns).Assign(initialValue);
        }
    }
}
