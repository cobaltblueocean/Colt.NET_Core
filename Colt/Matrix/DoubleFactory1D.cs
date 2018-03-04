// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleFactory1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Factory for convenient construction of 1-d matrices holding <tt>double</tt> cells.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Colt.Matrix
{
    using System;
    using System.Collections.Generic;

    using Function;

    using Implementation;

    /// <summary>
    /// Factory for convenient construction of 1-d matrices holding <tt>double</tt> cells.
    /// </summary>
    public class DoubleFactory1D : PersistentObject
    {
        /// <summary>
        /// A factory producing dense matrices.
        /// </summary>
        public static readonly DoubleFactory1D Dense = new DoubleFactory1D();

        /// <summary>
        /// A factory producing sparse hash matrices.
        /// </summary>
        public static readonly DoubleFactory1D Sparse = new DoubleFactory1D();

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleFactory1D"/> class.
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected DoubleFactory1D()
        {
        }

        /// <summary>
        /// C = A||B; Constructs a new matrix which is the concatenation of two other matrices.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="b">
        /// The matrix B.
        /// </param>
        /// <returns>
        /// The concatenation of A and B.
        /// </returns>
        public DoubleMatrix1D AppendColumns(DoubleMatrix1D a, DoubleMatrix1D b)
        {
            // concatenate
            DoubleMatrix1D matrix = Make(a.Size() + b.Size());
            matrix.ViewPart(0, a.Size()).Assign(a);
            matrix.ViewPart(a.Size(), b.Size()).Assign(b);
            return matrix;
        }

        /// <summary>
        /// Constructs a matrix with cells having ascending values.
        /// For debugging purposes.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// A matrix with cells having ascending values.
        /// </returns>
        public DoubleMatrix1D Ascending(int size)
        {
            return Descending(size).Assign(UnaryFunctions.Chain(a => -a, UnaryFunctions.Minus(size)));
        }

        /// <summary>
        /// Constructs a matrix with cells having descending values.
        /// For debugging purposes.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// A matrix with cells having descending values.
        /// </returns>
        public DoubleMatrix1D Descending(int size)
        {
            DoubleMatrix1D matrix = Make(size);
            int v = 0;
            for (int i = size; --i >= 0; )
                matrix[size] = v++;
            return matrix;
        }

        /// <summary>
        /// Constructs a matrix with the given cell values.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the new matrix.
        /// </param>
        /// <returns>
        /// A matrix with the given cell values.
        /// </returns>
        public DoubleMatrix1D Make(double[] values)
        {
            if (this == Sparse) return new SparseDoubleMatrix1D(values);
            return new DenseDoubleMatrix1D(values);
        }

        /// <summary>
        /// Construct a matrix which is the concatenation of all given parts.
        /// Cells are copied.
        /// </summary>
        /// <param name="parts">
        /// The parts.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix1D Make(DoubleMatrix1D[] parts)
        {
            if (parts.Length == 0) return Make(0);

            int size = 0;
            for (int i = 0; i < parts.Length; i++) size += parts[i].Size();

            DoubleMatrix1D vector = Make(size);
            size = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                vector.ViewPart(size, parts[i].Size()).Assign(parts[i]);
                size += parts[i].Size();
            }

            return vector;
        }

        /// <summary>
        /// Constructs a matrix with the given shape, each cell initialized with zero.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix1D Make(int size)
        {
            if (this == Sparse) return new SparseDoubleMatrix1D(size);
            return new DenseDoubleMatrix1D(size);
        }

        /// <summary>
        /// Constructs a matrix with the given shape, each cell initialized with the given value.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="initialValue">
        /// The initial value.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix1D Make(int size, double initialValue)
        {
            if (initialValue == 0) return Make(size);
            return Make(size).Assign(initialValue);
        }

        /// <summary>
        /// Constructs a matrix from the values of the given list.
        /// The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the new matrix.
        /// </param>
        /// <returns>
        /// A new matrix.
        /// </returns>
        public DoubleMatrix1D Make(IList<double> values)
        {
            int size = values.Count;
            DoubleMatrix1D vector = Make(size);
            for (int i = size; --i >= 0; ) vector[i] = values[i];
            return vector;
        }

        /// <summary>
        /// Constructs a matrix with uniformly distributed values in <tt>(0,1)</tt> (exclusive).
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix1D Random(int size)
        {
            return Make(size).Assign(new Random().NextDouble());
        }

        /// <summary>
        /// Constructs a new matrix which is A duplicated <tt>repeat</tt> times.
        /// </summary>
        /// <param name="a">
        /// The matrix to duplicate.
        /// </param>
        /// <param name="repeat">
        /// The number of repetitions.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix1D Repeat(DoubleMatrix1D a, int repeat)
        {
            int size = a.Size();
            DoubleMatrix1D matrix = Make(repeat * size);
            for (int i = repeat; --i >= 0; )
                matrix.ViewPart(size * i, size).Assign(a);

            return matrix;
        }

        /// <summary>
        /// Constructs a list from the given matrix.
        /// The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the list, and vice-versa.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the new list.
        /// </param>
        /// <returns>
        /// A new list.
        /// </returns>
        public List<double> ToList(DoubleMatrix1D values)
        {
            int size = values.Size();
            var list = new List<double>(size);
            for (int i = size; --i >= 0; ) list.Add(values[i]);
            return list;
        }
    }
}
