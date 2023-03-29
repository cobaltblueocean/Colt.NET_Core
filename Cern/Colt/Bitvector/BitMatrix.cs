// <copyright file="BitMatrix.cs" company="CERN">
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
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Bitvector
{
    public class BitMatrix
    {
        protected int _columns;
        protected int _rows;

        /// <summary>
        /// The bits of this matrix.
        /// bits are stored in row major, i.e.
        /// bitIndex==row*columns + column
        /// columnOf(bitIndex)==bitIndex%columns
        /// rowOf(bitIndex)==bitIndex/columns
        /// </summary>
        private long[] _bits;

        /// <summary>
        /// Returns the number of columns of the receiver.
        /// </summary>
        public int Columns
        {
            get { return _columns; }
            protected set { _columns = value; }
        }

        public long[] Elements
        {
            get { return _bits; }
            protected set { _bits = value; }
        }

        /// <summary>
        /// Returns the number of rows of the receiver.
        /// </summary>
        public int Rows
        {
            get { return _rows; }
            protected set { _rows = value; }
        }

        /// <summary>
        /// Returns the size of the receiver which is <i>columns()*rows()</i>.
        /// </summary>
        public int Size
        {
            get { return _columns * _rows; }
        }

        public Boolean this[int column, int row]
        {
            get
            {
                return QuickBitVector.Get(_bits, row * _columns + column);
            }
            set
            {
                QuickBitVector.Put(_bits, row * _columns + column, value);
            }
        }

        /// <summary>
        /// Constructs a bit matrix with a given number of columns and rowsd All bits are initially <i>false</i>.
        /// </summary>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <exception cref="ArgumentException">if <i>columns &lt; 0 || rows &lt; 0</i>.</exception>
        public BitMatrix(int columns, int rows)
        {
            GetElements(QuickBitVector.MakeBitVector(columns * rows, 1), columns, rows);
        }

        /// <summary>
        /// Performs a logical <b>AND</b> of the receiver with another bit matrix.
        /// The receiver is modified so that a bit in it has the
        /// value <code>true</code> if and only if it already had the 
        /// value <code>true</code> and the corresponding bit in the other bit matrix
        /// argument has the value <code>true</code>.
        /// </summary>
        /// <param name="other">a bit matrix.</param>
        /// <exception cref="ArgumentException">if <i>columns() != other.Columns || rows() != other.Rows</i>.</exception>
        public void And(BitMatrix other)
        {
            CheckDimensionCompatibility(other);
            ToBitVector().And(other.ToBitVector());
        }

        /// <summary>
        /// Clears all of the bits in receiver whose corresponding
        /// bit is set in the other bit matrix.
        /// In other words, determines the difference (A\B) between two bit matrices.
        /// </summary>
        /// <param name="other">a bit matrix with which to mask the receiver.</param>
        /// <exception cref="ArgumentException">if <i>columns() != other.Columns || rows() != other.Rows</i>.</exception>
        public void AndNot(BitMatrix other)
        {
            CheckDimensionCompatibility(other);
            ToBitVector().AndNot(other.ToBitVector());
        }

        /// <summary>
        /// Returns the number of bits currently in the <i>true</i> state.
        /// Optimized for speedd Particularly quick if the receiver is either sparse or dense.
        /// </summary>
        /// <returns></returns>
        public int Cardinality()
        {
            return ToBitVector().Cardinality();
        }

        /// <summary>
        /// Sanity check for operations requiring matrices with the same number of columns and rows.
        /// </summary>
        /// <param name="other"></param>
        protected void CheckDimensionCompatibility(BitMatrix other)
        {
            if (Columns != other.Columns || Rows != other.Rows) throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_IncompatibleDimensions, _columns, _rows, +other.Columns, other.Rows));
        }

        /// <summary>
        /// Clears all bits of the receiver.
        /// </summary>
        public void Clear()
        {
            ToBitVector().Clear();
        }

        /// <summary>
        /// Cloning this <code>BitMatrix</code> produces a new <code>BitMatrix</code> 
        /// that is equal to it.
        /// The clone of the bit matrix is another bit matrix that has exactly the 
        /// same bits set to <code>true</code> as this bit matrix and the same 
        /// number of columns and rowsd 
        /// </summary>
        /// <returns>a clone of this bit matrix.</returns>
        public Object Clone()
        {
            BitMatrix clone = (BitMatrix)base.MemberwiseClone();
            if (this._bits != null) clone._bits = (long[])this._bits.Clone();
            return clone;
        }

        /// <summary>
        /// Checks whether the receiver contains the given box.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected void ContainsBox(int column, int row, int width, int height)
        {
            if (column < 0 || column + width > _columns || row < 0 || row + height > _rows) throw new IndexOutOfRangeException("column:" + column + ", row:" + row + " ,width:" + width + ", height:" + height);
        }

        /// <summary>
        /// Returns a shallow clone of the receiver; calls <code>clone()</code> and casts the result.
        /// </summary>
        /// <returns>a shallow clone of the receiver.</returns>
        public BitMatrix Copy()
        {
            return (BitMatrix)Clone();
        }

        /// <summary>
        /// You normally need not use this methodd Use this method only if performance is criticald 
        /// Sets the bit matrix's backing bits, columns and rows.
        /// <b>WARNING:</b> For efficiency reasons and to keep memory usage low, <b>the array is not copied</b>.
        /// So if subsequently you modify the specified array directly via the [] operator, be sure you know what you're doing.
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <exception cref="ArgumentException">if <i>columns &lt; 0 || rows &lt; 0 || columns*rows &gt; bits.Length*64</i></exception>
        protected void GetElements(long[] bits, int columns, int rows)
        {
            if (columns < 0 || columns < 0 || columns * rows > bits.Length * QuickBitVector.BITS_PER_UNIT) throw new ArgumentException();
            this._bits = bits;
            this._columns = columns;
            this._rows = rows;
        }

        /// <summary>
        /// Compares this object against the specified object.
        /// The result is <code>true</code> if and only if the argument is 
        /// not <code>null</code> and is a <code>BitMatrix</code> object
        /// that has the same number of columns and rows as the receiver and 
        /// that has exactly the same bits set to <code>true</code> as the receiver.
        /// </summary>
        /// <param name="obj">the object to compare with.</param>
        /// <returns>
        /// <code>true</code> if the objects are the same;
        /// <code>false</code> otherwise.
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            if (obj == null || !(obj is BitMatrix))
                return false;
            if (this == obj)
                return true;

            BitMatrix other = (BitMatrix)obj;
            if (_columns != other.Columns || _rows != other.Rows) return false;

            return ToBitVector().Equals(other.ToBitVector());
        }

        /// <summary>
        /// Applies a procedure to each coordinate that holds a bit in the given state.
        /// Iterates rowwise downwards from [columns()-1,rows()-1] to [0,0].
        /// Useful, for example, if you want to copy bits into an image or somewhere else.
        /// Optimized for speedd Particularly quick if one of the following conditions holds
        /// <ul>
        /// <li><i>state==true</i> and the receiver is sparse (<i>cardinality()</i> is small compared to <i>Size</i>).
        /// <li><i>state==false</i> and the receiver is dense (<i>cardinality()</i> is large compared to <i>Size</i>).
        /// </ul>
        /// </summary>
        /// <param name="state">element to search for.</param>
        /// <param name="procedure">a procedure object taking as first argument the current column and as second argument the current rowd Stops iteration if the procedure returns <i>false</i>, otherwise continuesd </param>
        /// <returns><i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised </returns>
        public Boolean ForEachCoordinateInState(Boolean state, Cern.Colt.Function.IntIntProcedureDelegate procedure)
        {
            /*
            this is equivalent to the low level version below, apart from that it iterates in the reverse oder and is slower.
            if (Size==0) return true;
            BitVector vector = toBitVector();
            return vector.forEachIndexFromToInState(0,Size-1,state,
                new cern.colt.function.IntFunction() {
                    public Boolean apply(int index) {
                        return function.apply(index%columns, index/columns);
                    }
                }
            );
            */

            //low level implementation for speed.
            if (Size == 0) return true;
            BitVector vector = new BitVector(_bits, Size);

            long[] theBits = _bits;

            int column = _columns - 1;
            int row = _rows - 1;

            // for each coordinate of bits of partial unit
            long val = theBits[_bits.Length - 1];
            for (int j = vector.NumberOfBitsInPartialUnit(); --j >= 0;)
            {
                long mask = val & (1L << j);
                if ((state && (mask != 0L)) || ((!state) && (mask == 0L)))
                {
                    if (!procedure(column, row)) return false;
                }
                if (--column < 0)
                {
                    column = _columns - 1;
                    --row;
                }
            }


            // for each coordinate of bits of full units
            int bitsPerUnit = QuickBitVector.BITS_PER_UNIT;
            long comparator;
            if (state) comparator = 0L;
            else comparator = ~0L; // all 64 bits set

            for (int i = vector.NumberOfFullUnits(); --i >= 0;)
            {
                val = theBits[i];
                if (val != comparator)
                {
                    // at least one element within current unit matches.
                    // iterate over all bits within current unit.
                    if (state)
                    {
                        for (int j = bitsPerUnit; --j >= 0;)
                        {
                            if (((val & (1L << j))) != 0L)
                            {
                                if (!procedure(column, row)) return false;
                            }
                            if (--column < 0)
                            {
                                column = _columns - 1;
                                --row;
                            }
                        }
                    }
                    else
                    { // unrolled comparison for speed.
                        for (int j = bitsPerUnit; --j >= 0;)
                        {
                            if (((val & (1L << j))) == 0L)
                            {
                                if (!procedure(column, row)) return false;
                            }
                            if (--column < 0)
                            {
                                column = _columns - 1;
                                --row;
                            }
                        }
                    }

                }
                else
                { // no element within current unit matches --> skip unit
                    column -= bitsPerUnit;
                    if (column < 0)
                    {
                        // avoid implementation with *, /, %
                        column += bitsPerUnit;
                        for (int j = bitsPerUnit; --j >= 0;)
                        {
                            if (--column < 0)
                            {
                                column = _columns - 1;
                                --row;
                            }
                        }
                    }
                }

            }

            return true;

        }

        /// <summary>
        /// Returns from the receiver the value of the bit at the specified coordinate.
        /// The value is <i>true</i> if this bit is currently set; otherwise, returns <i>false</i>.
        /// </summary>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <returns>the value of the bit at the specified coordinate.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>column&lt;0 || column&gt;=columns() || row&lt;0 || row&gt;=rows()</i></exception>
        public Boolean Get(int column, int row)
        {
            if (column < 0 || column >= _columns || row < 0 || row >= _rows) throw new IndexOutOfRangeException("column:" + column + ", row:" + row);
            return this[column, row];
        }

        /// <summary>
        /// Returns from the receiver the value of the bit at the specified coordinate; <b>WARNING:</b> Does not check preconditions.
        /// The value is <i>true</i> if this bit is currently set; otherwise, returns <i>false</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid values without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>column&gt;=0 && column&lt;columns() && row&gt;=0 && row&lt;rows()</i>.
        /// </summary>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <returns>the value of the bit at the specified coordinate.</returns>
        public Boolean GetQuick(int column, int row)
        {
            return this[column, row];
        }

        /// <summary>
        /// Returns a hash code value for the receiverd
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToBitVector().GetHashCode();
        }

        /// <summary>
        /// Performs a logical <b>NOT</b> on the bits of the receiver.
        /// </summary>
        public void Not()
        {
            ToBitVector().Not();
        }

        /// <summary>
        /// Performs a logical <b>OR</b> of the receiver with another bit matrix.
        /// The receiver is modified so that a bit in it has the
        /// value <code>true</code> if and only if it either already had the 
        /// value <code>true</code> or the corresponding bit in the other bit matrix
        /// argument has the value <code>true</code>.
        /// </summary>
        /// <param name="other">a bit matrix.</param>
        /// <exception cref="ArgumentException">if <i>columns() != other.Columns || rows() != other.Rows</i>.</exception>
        public void Or(BitMatrix other)
        {
            CheckDimensionCompatibility(other);
            ToBitVector().Or(other.ToBitVector());
        }

        /// <summary>
        /// Constructs and returns a new matrix with <i>width</i> columns and <i>height</i> rows which is a copy of the contents of the given box.
        /// The box ranges from <i>[column,row]</i> to <i>[column+width-1,row+height-1]</i>, all inclusive.
        /// </summary>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="width">the width of the box.</param>
        /// <param name="height">the height of the box.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">if <i>column&lt;0 || column+width&gt;columns() || row&lt;0 || row+height&gt;rows()</i></exception>
        public BitMatrix Part(int column, int row, int width, int height)
        {
            if (column < 0 || column + width > _columns || row < 0 || row + height > _rows) throw new IndexOutOfRangeException("column:" + column + ", row:" + row + " ,width:" + width + ", height:" + height);
            if (width <= 0 || height <= 0) return new BitMatrix(0, 0);

            BitMatrix subMatrix = new BitMatrix(width, height);
            subMatrix.ReplaceBoxWith(0, 0, width, height, this, column, row);
            return subMatrix;
        }

        /// <summary>
        /// Sets the bit at the specified coordinate to the state specified by <i>value</i>.
        /// </summary>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="value">the value of the bit to be copied into the specified coordinate.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>column&lt;0 || column&gt;=columns() || row&lt;0 || row&gt;=rows()</i></exception>
        public void Put(int column, int row, Boolean value)
        {
            if (column < 0 || column >= _columns || row < 0 || row >= _rows) throw new IndexOutOfRangeException("column:" + column + ", row:" + row);
            QuickBitVector.Put(_bits, row * _columns + column, value);
        }

        /// <summary>
        /// Sets the bit at the specified coordinate to the state specified by <i>value</i>; <b>WARNING:</b> Does not check preconditions.
        ///
        /// <p>Provided with invalid parameters this method may return invalid values without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>column&gt;=0 && column&lt;columns() && row&gt;=0 && row&lt;rows()</i>.
        /// </summary>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="value">the value of the bit to be copied into the specified coordinate.</param>
        public void PutQuick(int column, int row, Boolean value)
        {
            this[column, row] = value;
        }

        /// <summary>
        /// Replaces a box of the receiver with the contents of another matrix's box.
        /// The source box ranges from <i>[sourceColumn,sourceRow]</i> to <i>[sourceColumn+width-1,sourceRow+height-1]</i>, all inclusive.
        /// The destination box ranges from <i>[column,row]</i> to <i>[column+width-1,row+height-1]</i>, all inclusive.
        /// Does nothing if <i>width &lt;= 0 || height &lt;= 0</i>.
        /// If <i>source==this</i> and the source and destination box intersect in an ambiguous way, then replaces as if using an intermediate auxiliary copy of the receiver.
        /// </summary>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="width">the width of the box.</param>
        /// <param name="height">the height of the box.</param>
        /// <param name="source">the source matrix to copy from(may be identical to the receiver).</param>
        /// <param name="sourceColumn">the index of the source column-coordinate.</param>
        /// <param name="sourceRow">the index of the source row-coordinate.</param>
        /// <exception cref="">if <i>column&lt;0 || column+width&gt;columns() || row&lt;0 || row+height&gt;rows()</i></exception>
        /// <exception cref="">if <i>sourceColumn&lt;0 || sourceColumn+width&gt;source.Columns || sourceRow&lt;0 || sourceRow+height&gt;source.Rows</i></exception>
        public void ReplaceBoxWith(int column, int row, int width, int height, BitMatrix source, int sourceColumn, int sourceRow)
        {
            this.ContainsBox(column, row, width, height);
            source.ContainsBox(sourceColumn, sourceRow, width, height);
            if (width <= 0 || height <= 0) return;

            if (source == this)
            {
                Rectangle destRect = new Rectangle(column, row, width, height);
                Rectangle sourceRect = new Rectangle(sourceColumn, sourceRow, width, height);
                if (destRect.IntersectsWith(sourceRect))
                { // dangerous intersection
                    source = source.Copy();
                }
            }

            BitVector sourceVector = source.ToBitVector();
            BitVector destVector = this.ToBitVector();
            int sourceColumns = source.Columns;
            for (; --height >= 0; row++, sourceRow++)
            {
                int offset = row * _columns + column;
                int sourceOffset = sourceRow * sourceColumns + sourceColumn;
                destVector.ReplaceFromToWith(offset, offset + width - 1, sourceVector, sourceOffset);
            }
        }

        /// <summary>
        /// Sets the bits in the given box to the state specified by <i>value</i>.
        /// The box ranges from <i>[column,row]</i> to <i>[column+width-1,row+height-1]</i>, all inclusive.
        /// (Does nothing if <i>width &lt;= 0 || height &lt;= 0</i>).
        /// </summary>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="width">the width of the box.</param>
        /// <param name="height">the height of the box.</param>
        /// <param name="value">the value of the bit to be copied into the bits of the specified box.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>column&lt;0 || column+width&gt;columns() || row&lt;0 || row+height&gt;rows()</i></exception>
        public void ReplaceBoxWith(int column, int row, int width, int height, Boolean value)
        {
            ContainsBox(column, row, width, height);
            if (width <= 0 || height <= 0) return;

            BitVector destVector = this.ToBitVector();
            for (; --height >= 0; row++)
            {
                int offset = row * _columns + column;
                destVector.ReplaceFromToWith(offset, offset + width - 1, value);
            }
        }

        /// <summary>
        /// Converts the receiver to a bitvectord 
        /// In many cases this method only makes sense on one-dimensional matrices.
        /// <b>WARNING:</b> The returned bitvector and the receiver share the <b>same</b> backing bits.
        /// Modifying either of them will affect the other.
        /// If this behaviour is not what you want, you should first use <i>copy()</i> to make sure both objects use separate internal storage.
        /// </summary>
        /// <returns></returns>
        public BitVector ToBitVector()
        {
            return new BitVector(_bits, Size);
        }

        /// <summary>
        /// Returns a (very crude) string representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return ToBitVector().ToString();
        }

        /// <summary>
        /// Performs a logical <b>XOR</b> of the receiver with another bit matrix.
        /// The receiver is modified so that a bit in it has the
        /// value <code>true</code> if and only if one of the following statements holds:
        /// <ul>
        /// <li>The bit initially has the value <code>true</code>, and the 
        ///     corresponding bit in the argument has the value <code>false</code>.
        /// <li>The bit initially has the value <code>false</code>, and the 
        ///     corresponding bit in the argument has the value <code>true</code>d 
        /// </ul>
        /// </summary>
        /// <param name="other">a bit matrix.</param>
        /// <exception cref="ArgumentException">if <i>columns() != other.Columns || rows() != other.Rows</i>.</exception>
        public void Xor(BitMatrix other)
        {
            CheckDimensionCompatibility(other);
            ToBitVector().Xor(other.ToBitVector());
        }
    }
}
