// <copyright file="DoubleFactory3D.cs" company="CERN">
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
    using F1 = Cern.Jet.Math.Functions.DoubleFunctions;
    using F2 = Cern.Jet.Math.Functions.DoubleDoubleFunctions;

    /// <summary>
    /// Factory for convenient construction of 3-d matrices holding <i>double</i> cellsd 
    /// Use idioms like <i>DoubleFactory3D.dense.make(4,4,4)</i> to construct dense matrices, 
    /// <i>DoubleFactory3D.sparse.make(4,4,4)</i> to construct sparse matrices.
    ///
    /// If the factory is used frequently it might be useful to streamline the notationd 
    /// 
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    /// <example>
    /// DoubleFactory3D F = DoubleFactory3D.Dense;
    /// F.Make(4,4,4);
    /// F.descending(10,20,5);
    /// F.Random(4,4,5);
    /// ...
    /// </example>
    public class DoubleFactory3D
    {
        /// <summary>
        /// A factory producing dense matrices.
        /// </summary>
        private static DoubleFactory3D _dense = new DoubleFactory3D();

        public static DoubleFactory3D Dense
        {
            get { return _dense; }
        }

        /// <summary>
        /// A factory producing sparse matrices.
        /// </summary>
        private static DoubleFactory3D _sparse = new DoubleFactory3D();

        public static DoubleFactory3D Sparse
        {
            get { return _sparse; }
        }

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected DoubleFactory3D() { }

        /// <summary>
        /// Constructs a matrix with cells having ascending values.
        /// For debugging purposes.
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DoubleMatrix3D Ascending(int slices, int rows, int columns)
        {
            //Cern.Jet.Math.Functions F = Cern.Jet.Math.Functions.functions;
            return Descending(slices, rows, columns).Assign(F1.Chain(F1.Neg, F1.Minus(slices * rows * columns)));
        }

        /// <summary>
        /// Constructs a matrix with cells having descending values.
        /// For debugging purposes.
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DoubleMatrix3D Descending(int slices, int rows, int columns)
        {
            DoubleMatrix3D matrix = Make(slices, rows, columns);
            int v = 0;
            for (int slice = slices; --slice >= 0;)
            {
                for (int row = rows; --row >= 0;)
                {
                    for (int column = columns; --column >= 0;)
                    {
                        matrix[slice, row, column] = v++;
                    }
                }
            }
            return matrix;
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
        public DoubleMatrix3D Make(double[,,] values)
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
        public DoubleMatrix3D Make(double[][][] values)
        {
            if (this == _sparse) return new SparseDoubleMatrix3D(values);
            return new DenseDoubleMatrix3D(values);
        }

        /// <summary>
        /// Constructs a matrix with the given shape, each cell initialized with zero.
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DoubleMatrix3D Make(int slices, int rows, int columns)
        {
            if (this == _sparse) return new SparseDoubleMatrix3D(slices, rows, columns);
            return new DenseDoubleMatrix3D(slices, rows, columns);
        }

        /// <summary>
        /// Constructs a matrix with the given shape, each cell initialized with the given value.
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public DoubleMatrix3D Make(int slices, int rows, int columns, double initialValue)
        {
            return Make(slices, rows, columns).Assign(initialValue);
        }

        /// <summary>
        /// Constructs a matrix with uniformly distributed values in <i>(0,1)</i> (exclusive).
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public DoubleMatrix3D Random(int slices, int rows, int columns)
        {
            return Make(slices, rows, columns).Assign(F1.Random());
        }
    }
}
