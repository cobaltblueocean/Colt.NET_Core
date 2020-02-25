﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedDenseDoubleMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Selection view on dense 1-d matrices holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System;

    /// <summary>
    /// Selection view on dense 1-d matrices holding <tt>double</tt> elements.
    /// </summary>
    public class SelectedDenseDoubleMatrix1D : DoubleMatrix1D
    {
        /// <summary>
        /// Gets the offsets of visible indexes of this matrix.
        /// </summary>
        private readonly int[] Offsets;

        /// <summary>
        /// Gets the offset.
        /// </summary>
        protected int Offset { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedDenseDoubleMatrix1D"/> class.
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="elements">
        /// The cells.
        /// </param>
        /// <param name="offsets">
        /// The indexes of the cells that shall be visible.
        /// </param>
        internal SelectedDenseDoubleMatrix1D(double[] elements, int[] offsets)
        {
            Setup(offsets.Length, 0, 1);

            this.Elements = elements;
            this.Offsets = offsets;
            this.Offset = 0;
            IsView = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedDenseDoubleMatrix1D"/> class.
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
        internal SelectedDenseDoubleMatrix1D(int size, double[] elements, int zero, int stride, int[] offsets, int offset)
        {
            Setup(size, zero, stride);

            this.Elements = elements;
            this.Offsets = offsets;
            this.Offset = offset;
            IsView = true;
        }

        /// <summary>
        /// Gets the elements of this matrix.
        /// </summary>
        internal double[] Elements { get; private set; }

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
                Elements[Offset + Offsets[Zero + (index * Stride)]] = value;
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
            return new DenseDoubleMatrix1D(n);
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
            return new DenseDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// You may want to override this method for performance.
        /// </summary>
        /// <param name="rank">
        /// The rank of the element.
        /// </param>
        /// <returns>
        /// Returns the position of the element with the given relative rank.
        /// </returns>
        protected internal override int Index(int rank)
        {
            ////return this.offset + super.index(rank);
            // manually inlined:
            return Offset + Offsets[Zero + (rank * Stride)];
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// Default implementation. Override, if necessary.
        /// </summary>
        /// <param name="absRank">
        /// The absolute rank of the element.
        /// </param>
        /// <returns>
        /// The position
        /// </returns>
        protected override int GetOffset(int absRank)
        {
            return Offsets[absRank];
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
            if (other is SelectedDenseDoubleMatrix1D)
            {
                var otherMatrix = (SelectedDenseDoubleMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }

            if (other is DenseDoubleMatrix1D)
            {
                var otherMatrix = (DenseDoubleMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }

            return false;
        }

        /// <summary>
        /// Sets up a matrix with a given number of cells.
        /// </summary>
        /// <param name="n">
        /// The the number of cells the matrix shall have.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size &lt; 0</tt>.
        /// </exception>
        protected override void Setup(int n)
        {
            base.Setup(n);
            this.Stride = 1;
            this.Offset = 0;
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offs">
        /// The offsets of the visible elements.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        protected override DoubleMatrix1D ViewSelectionLike(int[] offs)
        {
            return new SelectedDenseDoubleMatrix1D(this.Elements, offs);
        }

        public override string ToString(int index)
        {
            return this[index].ToString();
        }
    }
}
