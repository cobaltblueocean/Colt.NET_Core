// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedSparseIDoubleMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Selection view on sparse 1-d matrices holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System.Collections.Generic;

    /// <summary>
    /// Selection view on sparse 1-d matrices holding <tt>double</tt> elements.
    /// </summary>
    public class SelectedSparseDoubleMatrix1D : DoubleMatrix1D
    {
        /// <summary>
        /// Gets the offsets of visible indexes of this matrix.
        /// </summary>
        private readonly int[] Offsets;

        /// <summary>
        /// Gets the offset.
        /// </summary>
        private int Offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedSparseDoubleMatrix1D"/> class.
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="elements">
        /// The cells.
        /// </param>
        /// <param name="offsets">
        /// The indexes of the cells that shall be visible.
        /// </param>
        internal SelectedSparseDoubleMatrix1D(IDictionary<int, double> elements, int[] offsets)
        {
            Setup(offsets.Length, 0, 1);

            this.Elements = elements;
            this.Offsets = offsets;
            this.Offset = 0;
            this.IsView = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedSparseDoubleMatrix1D"/> class.
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <param name="elements">
        /// The cells.
        /// </param>
        /// <param name="zero">
        /// The index of the first element.
        /// </param>
        /// <param name="stride">
        /// The number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
        /// </param>
        /// <param name="offsets">
        /// The offsets of the cells that shall be visible.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        internal SelectedSparseDoubleMatrix1D(int size, IDictionary<int, double> elements, int zero, int stride, int[] offsets, int offset)
        {
            Setup(size, zero, stride);

            this.Elements = elements;
            this.Offsets = offsets;
            this.Offset = offset;
            this.IsView = true;
        }

        /// <summary>
        /// Gets the elements of the matrix.
        /// </summary>
        protected internal IDictionary<int, double> Elements { get; private set; }

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <tt>index</tt>.
        /// </summary>
        /// <param name="index">
        /// The index of the cell.
        /// </param>
        public override double this[int index]
        {
            get
            {
                return Elements[Offset + Offsets[Zero + (index * Stride)]];
            }

            set
            {
                int i = Offset + Offsets[Zero + (index * Stride)];
                if (value == 0)
                    this.Elements.Remove(i);
                else
                    this.Elements.Add(i, value);
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
        public override IDoubleMatrix1D Like(int n)
        {
            return new SparseDoubleMatrix1D(n);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirely independent of the receiver.
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
        public override IDoubleMatrix2D Like2D(int rows, int columns)
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
        /// Tthe position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// </returns>
        public override int Index(int rank)
        {
            // manually inlined:
            return Offset + Offsets[Zero + (rank * Stride)];
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
        public override bool HaveSharedCellsRaw(IDoubleMatrix1D other)
        {
            if (other is SelectedSparseDoubleMatrix1D)
            {
                var otherMatrix = (SelectedSparseDoubleMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }

            if (other is SparseDoubleMatrix1D)
            {
                var otherMatrix = (SparseDoubleMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }

            return false;
        }

        /// <summary>
        /// Sets up a matrix with a given number of cells.
        /// </summary>
        /// <param name="n">
        /// The number of cells the matrix shall have.
        /// </param>
        protected override void Setup(int n)
        {
            base.Setup(n);
            this.Stride = 1;
            this.Offset = 0;
        }

        /// <summary>
        /// onstruct and returns a new selection view.
        /// </summary>
        /// <param name="off">
        /// The offsets of the visible elements.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        public override IDoubleMatrix1D ViewSelectionLike(int[] off)
        {
            return new SelectedSparseDoubleMatrix1D(this.Elements, off);
        }

        public override string ToString(int index)
        {
            return this[index].ToString();
        }
    }
}
