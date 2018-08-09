// <copyright file="SelectedDenseObjectMatrix1D.cs" company="CERN">
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

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    /// Selection view on dense 1-d matrices holding <i>Object</i> elements.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Objects of this class are typically constructed via <i>viewIndexes</i> methods on some source matrix.
    /// The interface introduced in abstract base classes defines everything a user can do.
    /// From a user point of view there is nothing special about this class; it presents the same functionality with the same signatures and semantics as its abstract baseclass(es) while introducing no additional functionality.
    /// Thus, this class need not be visible to users.
    /// By the way, the same principle applies to concrete DenseXXX, SparseXXX classes: they presents the same functionality with the same signatures and semantics as abstract baseclass(es) while introducing no additional functionality.
    /// Thus, they need not be visible to users, eitherd 
    /// Factory methods could hide all these concrete types.
    /// <p>
    /// This class uses no delegationd 
    /// Its instances point directly to the datad 
    /// Cell addressing overhead is 1 additional array index access per get/set.
    /// <p>
    /// Note that this implementation is not synchronized.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// <i>memory [bytes] = 4*indexes.Length</i>.
    /// Thus, an index view with 1000 indexes additionally uses 4 KB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// Depends on the parent view holding cells.
    /// <p>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class SelectedDenseObjectMatrix1D : ObjectMatrix1D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        internal Object[] Elements { get; private set; }

        /// <summary>
        /// The offsets of visible indexes of this matrix.
        /// </summary>
        protected int[] Offsets;

        /// <summary>
        /// The offset.
        /// </summary>
        protected int Offset;

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <i>index</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        public override object this[int index]
        {
            get
            {
                //if (debug) if (index<0 || index>=size) checkIndex(index);
                //return elements[index(index)];
                //manually inlined:
                //return Elements[Offset + Offsets[Zero + index * Stride]];
                return Elements[Index(index)];
            }
            set
            {
                //if (debug) if (index<0 || index>=size) checkIndex(index);
                //elements[index(index)] = value;
                //manually inlined:
                //Elements[Offset + Offsets[Zero + index * Stride]] = value;
                Elements[Index(index)] = value;
            }
        }

        /// <summary>
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="elements">the cells.</param>
        /// <param name="indexes">The indexes of the cells that shall be visible.</param>
        public SelectedDenseObjectMatrix1D(Object[] elements, int[] offsets) : this(offsets.Length, elements, 0, 1, offsets, 0)
        {

        }

        /// <summary>
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <param name="elements">the cells.</param>
        /// <param name="zero">the index of the first element.</param>
        /// <param name="stride">the number of indexes between any two elements, i.ed <i>index(i+1)-index(i)</i>.</param>
        /// <param name="offsets">the offsets of the cells that shall be visible.</param>
        /// <param name="offset"></param>
        public SelectedDenseObjectMatrix1D(int size, Object[] elements, int zero, int stride, int[] offsets, int offset)
        {
            Setup(size, zero, stride);

            this.Elements = elements;
            this.Offsets = offsets;
            this.Offset = offset;
            this.IsView = true;
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// </summary>
        /// <param name="absRank">the absolute rank of the element.</param>
        /// <returns>the position.</returns>
        protected override int GetOffset(int absRank)
        {
            return Offsets[absRank];
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        [Obsolete("GetQuick(int index) is deprecated, please use indexer instead.")]
        public Object GetQuick(int index)
        {
            return this[index];
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected new Boolean HaveSharedCellsRaw(ObjectMatrix1D other)
        {
            if (other is SelectedDenseObjectMatrix1D)
            {
                SelectedDenseObjectMatrix1D otherMatrix = (SelectedDenseObjectMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is DenseObjectMatrix1D)
            {
                DenseObjectMatrix1D otherMatrix = (DenseObjectMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// You may want to override this method for performance.
        /// </summary>
        /// <param name="rank">the rank of the element.</param>
        /// <returns>the position of the element</returns>
        protected new int Index(int rank)
        {
            //return this.offset + base.index(rank);
            // manually inlined:
            return Offset + Offsets[Zero + rank * Stride];
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must also be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must also be of type <i>SparseObjectMatrix1D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="size">the number of cell the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override ObjectMatrix1D Like(int size)
        {
            return new DenseObjectMatrix1D(size);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must be of type <i>SparseObjectMatrix2D</i>, etc.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override ObjectMatrix2D Like2D(int rows, int columns)
        {
            return new DenseObjectMatrix2D(rows, columns);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>index</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        [Obsolete("SetQuick(int index, Object value) is deprecated, please use indexer instead.")]
        public void SetQuick(int index, Object value)
        {
            this[index] = value;
        }

        /// <summary>
        /// Sets up a matrix with a given number of cells.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        protected override void Setup(int size)
        {
            base.Setup(size);
            this.Stride = 1;
            this.Offset = 0;
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override ObjectMatrix1D ViewSelectionLike(int[] offsets)
        {
            return new SelectedDenseObjectMatrix1D(this.Elements, offsets);
        }

        public override string ToString(int index)
        {
            return this[index].ToString();
        }
    }
}
