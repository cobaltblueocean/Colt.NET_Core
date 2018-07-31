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
    /**
////Selection view on dense 1-d matrices holding <i>Object</i> elements.
////First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
////<p>
////<b>Implementation:</b>
////<p>
////Objects of this class are typically constructed via <i>viewIndexes</i> methods on some source matrix.
////The interface introduced in abstract base classes defines everything a user can do.
////From a user point of view there is nothing special about this class; it presents the same functionality with the same signatures and semantics as its abstract baseclass(es) while introducing no additional functionality.
////Thus, this class need not be visible to users.
////By the way, the same principle applies to concrete DenseXXX, SparseXXX classes: they presents the same functionality with the same signatures and semantics as abstract baseclass(es) while introducing no additional functionality.
////Thus, they need not be visible to users, eitherd 
////Factory methods could hide all these concrete types.
////<p>
////This class uses no delegationd 
////Its instances point directly to the datad 
////Cell addressing overhead is 1 additional array index access per get/set.
////<p>
////Note that this implementation is not synchronized.
////<p>
////<b>Memory requirements:</b>
////<p>
////<i>memory [bytes] = 4*indexes.Length</i>.
////Thus, an index view with 1000 indexes additionally uses 4 KB.
////<p>
////<b>Time complexity:</b>
////<p>
////Depends on the parent view holding cells.
////<p>
////@author wolfgang.hoschek@cern.ch
////@version 1.0, 09/24/99
*/
    public class SelectedDenseObjectMatrix1D : ObjectMatrix1D
    {
        /**
	  * The elements of this matrix.
	  */
        internal Object[] Elements { get; private set; }

        /**
          * The offsets of visible indexes of this matrix.
          */
        protected int[] offsets;

        /**
          * The offset.
          */
        protected int offset;

        public override object this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /**
 * Constructs a matrix view with the given parameters.
 * @param elements the cells.
 * @param  indexes   The indexes of the cells that shall be visible.
 */
        protected SelectedDenseObjectMatrix1D(Object[] elements, int[] offsets):this(offsets.Length, elements, 0, 1, offsets, 0)
        {
            
        }
        /**
         * Constructs a matrix view with the given parameters.
         * @param size the number of cells the matrix shall have.
         * @param elements the cells.
         * @param zero the index of the first element.
         * @param stride the number of indexes between any two elements, i.ed <i>index(i+1)-index(i)</i>.
         * @param offsets   the offsets of the cells that shall be visible.
         * @param offset   
         */
        protected SelectedDenseObjectMatrix1D(int size, Object[] elements, int zero, int stride, int[] offsets, int offset)
        {
            Setup(size, zero, stride);

            this.Elements = elements;
            this.offsets = offsets;
            this.offset = offset;
            this.IsView = true;
        }
        /**
         * Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
         * Default implementationd Override, if necessary.
         *
         * @param  rank   the absolute rank of the element.
         * @return the position.
         */
        protected int Offset(int absRank)
        {
            return offsets[absRank];
        }
        /**
         * Returns the matrix cell value at coordinate <i>index</i>.
         *
         * <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
         * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
         * Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
         *
         * @param     index   the index of the cell.
         * @return    the value of the specified cell.
         */
        public Object GetQuick(int index)
        {
            //if (debug) if (index<0 || index>=size) checkIndex(index);
            //return elements[index(index)];
            //manually inlined:
            return Elements[offset + offsets[Zero + index * Stride]];
        }
        /**
         * Returns <i>true</i> if both matrices share at least one identical cell.
         */
        protected new Boolean HaveSharedCellsRaw(ObjectMatrix1D other)
        {
            if (other is SelectedDenseObjectMatrix1D) {
                SelectedDenseObjectMatrix1D otherMatrix = (SelectedDenseObjectMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }
	else if (other is DenseObjectMatrix1D) {
                DenseObjectMatrix1D otherMatrix = (DenseObjectMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }
        /**
         * Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
         * You may want to override this method for performance.
         *
         * @param     rank   the rank of the element.
         */
        protected int index(int rank)
        {
            //return this.offset + base.index(rank);
            // manually inlined:
            return offset + offsets[Zero + rank * Stride];
        }
        /**
         * Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
         * For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must also be of type <i>DenseObjectMatrix1D</i>,
         * if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must also be of type <i>SparseObjectMatrix1D</i>, etc.
         * In general, the new matrix should have internal parametrization as similar as possible.
         *
         * @param size the number of cell the matrix shall have.
         * @return  a new empty matrix of the same dynamic type.
         */
        public override ObjectMatrix1D Like(int size)
        {
            return new DenseObjectMatrix1D(size);
        }

        /**
 * Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
 * For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must be of type <i>DenseObjectMatrix2D</i>,
 * if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must be of type <i>SparseObjectMatrix2D</i>, etc.
 *
 * @param rows the number of rows the matrix shall have.
 * @param columns the number of columns the matrix shall have.
 * @return  a new matrix of the corresponding dynamic type.
 */
        public override ObjectMatrix2D Like2D(int rows, int columns)
        {
            return new DenseObjectMatrix2D(rows, columns);
        }

        /**
 * Sets the matrix cell at coordinate <i>index</i> to the specified value.
 *
 * <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
 * <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
 * Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
 *
 * @param     index   the index of the cell.
 * @param    value the value to be filled into the specified cell.
 */
        public void setQuick(int index, Object value)
        {
            //if (debug) if (index<0 || index>=size) checkIndex(index);
            //elements[index(index)] = value;
            //manually inlined:
            Elements[offset + offsets[Zero + index * Stride]] = value;
        }
        /**
         * Sets up a matrix with a given number of cells.
         * @param size the number of cells the matrix shall have.
         */
        protected void setUp(int size)
        {
            base.Setup(size);
            this.Stride = 1;
            this.offset = 0;
        }
        /**
         * Construct and returns a new selection view.
         *
         * @param offsets the offsets of the visible elements.
         * @return  a new view.
         */
        protected override ObjectMatrix1D ViewSelectionLike(int[] offsets)
        {
            return new SelectedDenseObjectMatrix1D(this.Elements, offsets);
        }
    }
}
