// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseDoubleMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Sparse hashed 1-d matrix (aka <i>vector</i>) holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System;
    using System.Collections.Generic;

    using Function;

    /// <summary>
    /// Sparse hashed 1-d matrix (aka <i>vector</i>) holding <tt>double</tt> elements.
    /// </summary>
    public class SparseDoubleMatrix1D : DoubleMatrix1D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SparseDoubleMatrix1D"/> class with a copy of the given values.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the new matrix.
        /// </param>
        public SparseDoubleMatrix1D(double[] values)
        {
            Setup(values.Length);
            Assign(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseDoubleMatrix1D"/> class with a given number of cells.
        /// All entries are initially <tt>0</tt>.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size &lt; 0</tt>.
        /// </exception>
        public SparseDoubleMatrix1D(int size)
        {
            Setup(size);
            this.elements = new Dictionary<int, double>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseDoubleMatrix1D"/> class as a view with a given number of parameters.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <param name="elements">
        /// The elements the cells.
        /// </param>
        /// <param name="offset">
        /// The index of the first element.
        /// </param>
        /// <param name="stride">
        /// The number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size &lt; 0</tt>.
        /// </exception>
        internal SparseDoubleMatrix1D(int size, IDictionary<int, double> elements, int offset, int stride)
        {
            Setup(size, offset, stride);
            this.elements = elements;
            IsView = true;
        }

        /// <summary>
        /// Gets the elements of the matrix.
        /// </summary>
        internal IDictionary<int, double> elements { get; private set; }

        /// <summary>
        /// Get or sets the matrix cell value at coordinate <tt>index</tt>.
        /// </summary>
        /// <param name="index">
        /// The index of the cell.
        /// </param>
        public override double this[int index]
        {
            get
            {
                int i = Zero + (index * Stride);
                return elements.ContainsKey(i) ? elements[i] : 0;
            }

            set
            {
                int i = Zero + (index * Stride);
                if (value == 0)
                    this.elements.Remove(i);
                else
                    this.elements[i] = value;
            }
        }

        /// <summary>
        /// Applies a function to each cell and aggregates the results.
        /// </summary>
        /// <param name="aggr">
        /// An aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.
        /// </param>
        /// <param name="f">
        /// A function transforming the current cell value.
        /// </param>
        /// <returns>
        /// The aggregated measure.
        /// </returns>
        public override double Aggregate(DoubleDoubleFunction aggr, DoubleFunction f)
        {
            double result = double.NaN;
            bool first = true;
            foreach (var e in elements.Values)
            {
                if (first)
                {
                    first = false;
                    result = f(e);
                }
                else
                {
                    result = aggr(result, f(e));
                }
            }

            return result;
        }

        /// <summary>
        /// Sets all cells to the state specified by <tt>value</tt>.
        /// </summary>
        /// <param name="value">
        /// The value to be filled into the cells.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public override DoubleMatrix1D Assign(double value)
        {
            // overriden for performance only
            if (!IsView && value == 0) this.elements.Clear();
            else base.Assign(value);
            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[i] = function(x[i])</tt>.
        /// (Iterates downwards from <tt>[size()-1]</tt> to <tt>[0]</tt>).
        /// </summary>
        /// <param name="function">
        /// A  function taking as argument the current cell's value.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public override DoubleMatrix1D Assign(DoubleFunction function)
        {
            var indices = new int[elements.Count];
            elements.Keys.CopyTo(indices, 0);
            foreach (var i in indices)
                elements[i] = function(elements[i]);
            return this;
        }

        /// <summary>
        /// Returns the number of cells having non-zero values.
        /// </summary>
        /// <returns>
        /// The number of cells having non-zero values.
        /// </returns>
        public override int Cardinality()
        {
            return IsView ? base.Cardinality() : elements.Count;
        }

        /// <summary>
        /// Fills the coordinates and values of cells having non-zero values into the specified lists.
        /// Fills into the lists, starting at index 0.
        /// After this call returns the specified lists all have a new size, the number of non-zero values.
        /// </summary>
        /// <param name="indexList">
        /// The list to be filled with indexes, can have any size.
        /// </param>
        /// <param name="valueList">
        /// The list to be filled with values, can have any size.
        /// </param>
        public override void GetNonZeros(List<int> indexList, List<double> valueList)
        {
            bool fillIndexList = indexList != null;
            bool fillValueList = valueList != null;
            if (fillIndexList) indexList.Clear();
            if (fillValueList) valueList.Clear();
            foreach (var e in elements)
            {
                if (fillIndexList) indexList.Add(e.Key);
                if (fillValueList) valueList.Add(e.Value);
            }
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// </summary>
        /// <param name="n">
        /// The number of cell the matrix shall have.
        /// </param>
        /// <returns>
        /// A new empty matrix of the same dynamic type.
        /// </returns>
        public override DoubleMatrix1D Like(int n)
        {
            return new SparseDoubleMatrix1D(n);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        public override DoubleMatrix2D Like2D(int rows, int columns)
        {
            return new SparseDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// You may want to override this method for performance.
        /// </summary>
        /// <param name="rank">
        /// The rank of the element.
        /// </param>
        /// <returns>
        /// The position of the element with the given relative rank.
        /// </returns>
        protected internal override int Index(int rank)
        {
            // overriden for manual inlining only
            return Zero + (rank * Stride);
        }

        /// <summary>
        /// Returns <tt>true</tt> if both matrices share at least one identical cell.
        /// </summary>
        /// <param name="other">
        /// The other matrix.
        /// </param>
        /// <returns>
        /// <tt>true</tt> if both matrices share at least one identical cell.
        /// </returns>
        protected override bool HaveSharedCellsRaw(DoubleMatrix1D other)
        {
            if (other is SelectedSparseDoubleMatrix1D)
            {
                var otherMatrix = (SelectedSparseDoubleMatrix1D)other;
                return this.elements == otherMatrix.Elements;
            }

            if (other is SparseDoubleMatrix1D)
            {
                var otherMatrix = (SparseDoubleMatrix1D)other;
                return this.elements == otherMatrix.elements;
            }

            return false;
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offsets">
        /// The offsets of the visible elements.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        protected override DoubleMatrix1D ViewSelectionLike(int[] offsets)
        {
            return new SelectedSparseDoubleMatrix1D(this.elements, offsets);
        }
    }
}
