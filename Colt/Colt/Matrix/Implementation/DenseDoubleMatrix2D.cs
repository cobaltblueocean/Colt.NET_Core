// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DenseDoubleMatrix2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Dense 2-d matrix holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System;

    using Function;

    /// <summary>
    /// Dense 2-d matrix holding <tt>double</tt> elements.
    /// </summary>
    public sealed class DenseDoubleMatrix2D : DoubleMatrix2D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDoubleMatrix2D"/> class with a copy of the given values.
        /// <tt>values</tt> is required to have the form <tt>values[row][column]</tt>
        /// and have exactly the same number of columns in every row.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the new matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>for any 1 &lt;= row &lt; values.length: values[row].length != values[row-1].length</tt>.
        /// </exception>
        public DenseDoubleMatrix2D(double[][] values)
        {
            int rows = values.Length;
            int columns = values.Length == 0 ? 0 : values[0].Length;
            setUp(rows, columns);
            elements = new double[rows * columns];
            Assign(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDoubleMatrix2D"/> class with a given number of rows and columns.
        /// All entries are initially <tt>0</tt>.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>rows &lt; 0 || columns &lt; 0 || (double)columns*rows &gt; Integer.MAX_VALUE</tt>.
        /// </exception>
        public DenseDoubleMatrix2D(int rows, int columns)
        {
            setUp(rows, columns);
            elements = new double[rows * columns];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDoubleMatrix2D"/> class.
        /// Constructs a view with the given parameters.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <param name="elements">
        /// The cells.
        /// </param>
        /// <param name="rowZero">
        /// The row of the first element.
        /// </param>
        /// <param name="columnZero">
        /// The column of the first element.
        /// </param>
        /// <param name="rowStride">
        /// The number of elements between two rows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
        /// </param>
        /// <param name="columnStride">
        /// The number of elements between two columns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
        /// </param>
        internal DenseDoubleMatrix2D(int rows, int columns, double[] elements, int rowZero, int columnZero, int rowStride, int columnStride)
        {
            setUp(rows, columns, rowZero, columnZero, rowStride, columnStride);
            this.elements = elements;
            isView = true;
        }

        /// <summary>
        /// Gets the elements of this matrix.
        /// Elements are stored in row major, i.e.
        /// index==row*columns + column
        /// columnOf(index)==index%columns
        /// rowOf(index)==index/columns
        /// i.e. {row0 column0..m}, {row1 column0..m}, ..., {rown column0..m}
        /// </summary>
        internal double[] elements { get; private set; }

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <tt>[row,column]</tt>.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        public override double this[int row, int column]
        {
            get
            {
                // manually inlined:
                return elements[rowZero + (row * rowStride) + columnZero + (column * columnStride)];
            }

            set
            {
                // manually inlined:
                elements[rowZero + (row * rowStride) + columnZero + (column * columnStride)] = value;
            }
        }

        /// <summary>
        /// Sets all cells to the state specified by <tt>values</tt>.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the cells.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>values.length != rows() || for any 0 &lt;= row &lt; rows(): values[row].length != columns()</tt>.
        /// </exception>
        public override DoubleMatrix2D Assign(double[][] values)
        {
            if (isView) base.Assign(values);
            else
            {
                if (values.Length != Rows) throw new ArgumentOutOfRangeException("values", "Must have same number of rows: rows=" + values.Length + "rows()=" + Rows);
                int i = Columns * (Rows - 1);
                for (int row = Rows; --row >= 0;)
                {
                    double[] currentRow = values[row];
                    if (currentRow.Length != Columns) throw new ArgumentOutOfRangeException("values", "Must have same number of columns in every row: columns=" + currentRow.Length + "columns()=" + Columns);
                    Array.Copy(currentRow, 0, elements, i, Columns);
                    i -= Columns;
                }
            }

            return this;
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
        public override DoubleMatrix2D Assign(double value)
        {
            double[] elems = elements;
            int index = this.index(0, 0);
            int cs = columnStride;
            int rs = rowStride;
            for (int row = Rows; --row >= 0;)
            {
                for (int i = index, column = Columns; --column >= 0;)
                {
                    elems[i] = value;
                    i += cs;
                }

                index += rs;
            }

            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[row,col] = function(x[row,col])</tt>.
        /// </summary>
        /// <param name="function">
        /// A function taking as argument the current cell's value.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public override DoubleMatrix2D Assign(DoubleFunction function)
        {
            double[] elems = elements;
            if (elems == null) throw new ApplicationException();
            int ind = index(0, 0);
            int cs = columnStride;
            int rs = rowStride;

            // the general case x[i] = f(x[i])
            for (int row = Rows; --row >= 0;)
            {
                for (int i = ind, column = Columns; --column >= 0;)
                {
                    elems[i] = function(elems[i]);
                    i += cs;
                }

                ind += rs;
            }

            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// </summary>
        /// <param name="source">
        /// The source matrix to copy from (may be identical to the receiver).
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>columns() != source.columns() || rows() != source.rows()</tt>
        /// </exception>
        public override DoubleMatrix2D Assign(DoubleMatrix2D source)
        {
            // overriden for performance only
            if (!(source is DenseDoubleMatrix2D))
                return base.Assign(source);

            var other = (DenseDoubleMatrix2D)source;
            if (other == this) return this; // nothing to do
            checkShape(other);

            if (!isView && !other.isView)
            {
                // quickest
                Array.Copy(other.elements, 0, elements, 0, elements.Length);
                return this;
            }

            if (haveSharedCells(other))
            {
                DoubleMatrix2D c = other.Copy();
                if (!(c is DenseDoubleMatrix2D))
                {
                    // should not happen
                    return base.Assign(other);
                }

                other = (DenseDoubleMatrix2D)c;
            }

            double[] elems = elements;
            double[] otherElems = other.elements;
            if (elems == null || otherElems == null) throw new ApplicationException();
            int cs = columnStride;
            int ocs = other.columnStride;
            int rs = rowStride;
            int ors = other.rowStride;

            int otherIndex = other.index(0, 0);
            int ind = index(0, 0);
            for (int row = Rows; --row >= 0;)
            {
                for (int i = ind, j = otherIndex, column = Columns; --column >= 0;)
                {
                    elems[i] = otherElems[j];
                    i += cs;
                    j += ocs;
                }

                ind += rs;
                otherIndex += ors;
            }

            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[row,col] = function(x[row,col],y[row,col])</tt>.
        /// </summary>
        /// <param name="y">
        /// The secondary matrix to operate on.
        /// </param>
        /// <param name="function">
        /// A function taking as first argument the current cell's value of <tt>this</tt>, and as second argument the current cell's value of <tt>y</tt>.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>columns() != other.columns() || rows() != other.rows()</tt>
        /// </exception>
        public override DoubleMatrix2D Assign(DoubleMatrix2D y, DoubleDoubleFunction function)
        {
            // overriden for performance only
            if (!(y is DenseDoubleMatrix2D))
                return base.Assign(y, function);

            var other = (DenseDoubleMatrix2D)y;
            checkShape(y);

            double[] elems = elements;
            double[] otherElems = other.elements;
            if (elems == null || otherElems == null) throw new ApplicationException();
            int cs = columnStride;
            int ocs = columnStride;
            int rs = rowStride;
            int ors = other.rowStride;

            int otherIndex = other.index(0, 0);
            int index = this.index(0, 0);

            // specialized for speed
            for (int row = Rows; --row >= 0;)
            {
                for (int i = index, j = otherIndex, column = Columns; --column >= 0;)
                {
                    elems[i] = function(elems[i], otherElems[j]);
                    i += cs;
                    j += ocs;
                }

                index += rs;
                otherIndex += ors;
            }

            return this;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <returns>
        /// A new empty matrix of the same dynamic type.
        /// </returns>
        public override DoubleMatrix2D Like(int rows, int columns)
        {
            return new DenseDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        public override DoubleMatrix1D Like1D(int size)
        {
            return new DenseDoubleMatrix1D(size);
        }

        /// <summary>
        /// 8 neighbor stencil transformation. For efficient finite difference operations.
        /// Applies a function to a moving <tt>3 x 3</tt> window.
        /// Does nothing if <tt>rows() &lt; 3 || columns() &lt; 3</tt>.
        /// </summary>
        /// <param name="b">
        /// The matrix to hold the results.
        /// </param>
        /// <param name="function">
        /// The unction to be applied to the 9 cells.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>rows() != B.rows() || columns() != B.columns()</tt>.
        /// </exception>
        public override void ZAssign8Neighbors(DoubleMatrix2D b, Double9Function function)
        {
            // 1. using only 4-5 out of the 9 cells in "function" is *not* the limiting factor for performance.
            // 2. if the "function" would be hardwired into the innermost loop, a speedup of 1.5-2.0 would be seen
            // but then the multi-purpose interface is gone...
            if (!(b is DenseDoubleMatrix2D))
            {
                base.ZAssign8Neighbors(b, function);
                return;
            }

            checkShape(b);
            int r = Rows - 1;
            int c = Columns - 1;
            if (Rows < 3 || Columns < 3) return; // nothing to do

            var bb = (DenseDoubleMatrix2D)b;
            int a_rs = rowStride;
            int b_rs = bb.rowStride;
            int a_cs = columnStride;
            int b_cs = bb.columnStride;
            double[] elems = elements;
            double[] b_elems = bb.elements;
            if (elems == null || b_elems == null) throw new ApplicationException();

            int a_index = index(1, 1);
            int b_index = bb.index(1, 1);
            for (int i = 1; i < r; i++)
            {
                int b11 = b_index;

                int a02 = a_index - a_rs - a_cs;
                int a12 = a02 + a_rs;
                int a22 = a12 + a_rs;

                // in each step six cells can be remembered in registers - they don't need to be reread from slow memory
                double a00 = elems[a02];
                a02 += a_cs;
                double a01 = elems[a02];
                double a10 = elems[a12];
                a12 += a_cs;
                double a11 = elems[a12];
                double a20 = elems[a22];
                a22 += a_cs;
                double a21 = elems[a22];

                for (int j = 1; j < c; j++)
                {
                    // in each step 3 instead of 9 cells need to be read from memory.
                    double _a02 = elems[a02 += a_cs];
                    double _a12 = elems[a12 += a_cs];
                    double _a22 = elems[a22 += a_cs];

                    b_elems[b11] = function(a00, a01, _a02, a10, a11, _a12, a20, a21, _a22);
                    b11 += b_cs;

                    // move remembered cells
                    a00 = a01;
                    a01 = _a02;
                    a10 = a11;
                    a11 = _a12;
                    a20 = a21;
                    a21 = _a22;
                }

                a_index += a_rs;
                b_index += b_rs;
            }
        }

        /// <summary>
        /// Linear algebraic matrix-vector multiplication; <tt>z = alpha * A * y + beta*z</tt>.
        /// </summary>
        /// <param name="y">
        /// The ource vector.
        /// </param>
        /// <param name="z">
        /// The vector where results are to be stored. Set this parameter to <tt>null</tt> to indicate that a new result vector shall be constructed.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="beta">
        /// The beta.
        /// </param>
        /// <param name="transposeA">
        /// Whether A must be transposed.
        /// </param>
        /// <returns>
        /// z (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>A.columns() != y.size() || A.rows() &gt; z.size())</tt>.
        /// </exception>
        public override DoubleMatrix1D ZMult(DoubleMatrix1D y, DoubleMatrix1D z, double alpha, double beta, bool transposeA)
        {
            if (transposeA) return ViewDice().ZMult(y, z, alpha, beta, false);
            if (z == null) z = new DenseDoubleMatrix1D(Rows);
            if (!(y is DenseDoubleMatrix1D) && z is DenseDoubleMatrix1D) return base.ZMult(y, z, alpha, beta, false);

            if (Columns != y.size || Rows > z.size)
                throw new ArgumentException("Incompatible args: " + this + ", " + y + ", " + z);

            var yy = (DenseDoubleMatrix1D)y;
            var zz = (DenseDoubleMatrix1D)z;
            double[] aElems = elements;
            double[] yElems = yy.elements;
            double[] zElems = zz.elements;
            if (aElems == null || yElems == null || zElems == null) throw new ApplicationException();
            int _as = columnStride;
            int ys = yy.stride;
            int zs = zz.stride;

            int indexA = index(0, 0);
            int indexY = yy.index(0);
            int indexZ = zz.index(0);

            int cols = Columns;
            for (int row = Rows; --row >= 0;)
            {
                double sum = 0;

                /*
                // not loop unrolled
                for (int i=indexA, j=indexY, column=columns; --column >= 0;) {
                    sum += AElems[i] * yElems[j];
                    i += As;
                    j += ys;
                }
                */

                // loop unrolled
                int i = indexA - _as;
                int j = indexY - ys;
                for (int k = cols % 4; --k >= 0;)
                    sum += aElems[i += _as] * yElems[j += ys];
                for (int k = cols / 4; --k >= 0;)
                {
                    sum += (aElems[i += _as] * yElems[j += ys]) +
                        (aElems[i += _as] * yElems[j += ys]) +
                        (aElems[i += _as] * yElems[j += ys]) +
                        (aElems[i += _as] * yElems[j += ys]);
                }

                zElems[indexZ] = (alpha * sum) + (beta * zElems[indexZ]);
                indexA += rowStride;
                indexZ += zs;
            }

            return z;
        }

        /// <summary>
        /// Linear algebraic matrix-matrix multiplication; <tt>C = alpha * A x B + beta*C</tt>.
        /// </summary>
        /// <param name="b">
        /// The econd source matrix.
        /// </param>
        /// <param name="c">
        /// The matrix where results are to be stored. Set this parameter to <tt>null</tt> to indicate that a new result matrix shall be constructed.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="beta">
        /// The beta.
        /// </param>
        /// <param name="transposeA">
        /// Whether A must be transposed.
        /// </param>
        /// <param name="transposeB">
        /// Whether B must be transposed.
        /// </param>
        /// <returns>
        /// C (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>B.rows() != A.columns()</tt>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <tt>C.rows() != A.rows() || C.columns() != B.columns()</tt>.
        /// </exception>
        /// <exception cref="ArithmeticException">
        /// If <tt>A == C || B == C</tt>.
        /// </exception>
        public override DoubleMatrix2D ZMult(DoubleMatrix2D b, DoubleMatrix2D c, double alpha, double beta, bool transposeA, bool transposeB)
        {
            // overriden for performance only
            if (transposeA) return ViewDice().ZMult(b, c, alpha, beta, false, transposeB);
            if (b is SparseDoubleMatrix2D)
            {
                // exploit quick sparse mult
                // A*B = (B' * A')'
                if (c == null)
                    return b.ZMult(this, null, alpha, beta, !transposeB, true).ViewDice();

                b.ZMult(this, c.ViewDice(), alpha, beta, !transposeB, true);
                return c;
            }

            if (transposeB) return ZMult(b.ViewDice(), c, alpha, beta, false, false);

            int m = Rows;
            int n = Columns;
            int p = b.Columns;
            if (c == null) c = new DenseDoubleMatrix2D(m, p);
            if (!(c is DenseDoubleMatrix2D)) return base.ZMult(b, c, alpha, beta, false, false);
            if (b.Rows != n)
                throw new ArgumentOutOfRangeException("b", "Matrix2D inner dimensions must agree:" + this + ", " + b);
            if (c.Rows != m || c.Columns != p)
                throw new ArgumentException("Incompatible result matrix: " + this + ", " + b + ", " + c);
            if (this == c || b == c)
                throw new ArgumentException("Matrices must not be identical");

            var bb = (DenseDoubleMatrix2D)b;
            var cc = (DenseDoubleMatrix2D)c;
            double[] aElems = elements;
            double[] bElems = bb.elements;
            double[] cElems = cc.elements;
            if (aElems == null || bElems == null || cElems == null) throw new ApplicationException();

            int cA = columnStride;
            int cB = bb.columnStride;
            int cC = cc.columnStride;

            int rA = rowStride;
            int rB = bb.rowStride;
            int rC = cc.rowStride;

            /*
            A is blocked to hide memory latency
                    xxxxxxx B
                    xxxxxxx
                    xxxxxxx
            A
            xxx     xxxxxxx C
            xxx     xxxxxxx
            ---     -------
            xxx     xxxxxxx
            xxx     xxxxxxx
            ---     -------
            xxx     xxxxxxx
            */
            const int BLOCK_SIZE = 30000;
            int m_optimal = (BLOCK_SIZE - n) / (n + 1);
            if (m_optimal <= 0) m_optimal = 1;
            int blocks = m / m_optimal;
            int rr = 0;
            if (m % m_optimal != 0) blocks++;
            for (; --blocks >= 0;)
            {
                int jB = bb.index(0, 0);
                int indexA = index(rr, 0);
                int jC = cc.index(rr, 0);
                rr += m_optimal;
                if (blocks == 0) m_optimal += m - rr;

                for (int j = p; --j >= 0;)
                {
                    int iA = indexA;
                    int iC = jC;
                    for (int i = m_optimal; --i >= 0;)
                    {
                        int kA = iA;
                        int kB = jB;
                        double s = 0;

                        /*
                        // not unrolled:
                        for (int k = n; --k >= 0;) {
                            //s += getQuick(i,k) * B.getQuick(k,j);
                            s += AElems[kA] * BElems[kB];
                            kB += rB;
                            kA += cA;
                        }
                        */

                        // loop unrolled
                        kA -= cA;
                        kB -= rB;

                        for (int k = n % 4; --k >= 0;)
                            s += aElems[kA += cA] * bElems[kB += rB];
                        for (int k = n / 4; --k >= 0;)
                        {
                            s += (aElems[kA += cA] * bElems[kB += rB]) +
                                (aElems[kA += cA] * bElems[kB += rB]) +
                                (aElems[kA += cA] * bElems[kB += rB]) +
                                (aElems[kA += cA] * bElems[kB += rB]);
                        }

                        cElems[iC] = (alpha * s) + (beta * cElems[iC]);
                        iA += rA;
                        iC += rC;
                    }

                    jB += cB;
                    jC += cC;
                }
            }

            return c;
        }

        /// <summary>
        /// Returns the sum of all cells; <tt>Sum( x[i,j] )</tt>.
        /// </summary>
        /// <returns>
        /// The sum of all cells.
        /// </returns>
        public override double ZSum()
        {
            double sum = 0;
            double[] elems = elements;
            if (elems == null) throw new ApplicationException();
            int index = this.index(0, 0);
            int cs = columnStride;
            int rs = rowStride;
            for (int row = Rows; --row >= 0;)
            {
                for (int i = index, column = Columns; --column >= 0;)
                {
                    sum += elems[i];
                    i += cs;
                }

                index += rs;
            }

            return sum;
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <param name="zero">
        /// The index of the first element.
        /// </param>
        /// <param name="stride">
        /// The number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        protected internal override DoubleMatrix1D like1D(int size, int zero, int stride)
        {
            return new DenseDoubleMatrix1D(size, elements, zero, stride);
        }

        /// <summary>
        /// Returns <tt>true</tt> if both matrices share common cells.
        /// More formally, returns <tt>true</tt> if <tt>other != null</tt> and at least one of the following conditions is met
        /// <ul>
        /// <li>the receiver is a view of the other matrix</li>
        /// <li>the other matrix is a view of the receiver</li>
        /// <li><tt>this == other</tt></li>
        /// </ul>
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// <tt>true</tt> if both matrices share common cells.
        /// </returns>
        protected override bool haveSharedCellsRaw(DoubleMatrix2D other)
        {
            if (other is SelectedDenseDoubleMatrix2D)
            {
                var otherMatrix = (SelectedDenseDoubleMatrix2D)other;
                return elements == otherMatrix.elements;
            }

            if (other is DenseDoubleMatrix2D)
            {
                var otherMatrix = (DenseDoubleMatrix2D)other;
                return elements == otherMatrix.elements;
            }

            return false;
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        /// <returns>
        /// The position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
        /// </returns>
        protected override int index(int row, int column)
        {
            // return super.index(row,column);
            // manually inlined for speed:
            return rowZero + (row * rowStride) + columnZero + (column * columnStride);
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="rowOffsets">
        /// The row offsets of the visible elements.
        /// </param>
        /// <param name="cOffsets">
        /// The column offsets of the visible elements.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        protected override DoubleMatrix2D viewSelectionLike(int[] rowOffsets, int[] cOffsets)
        {
            return new SelectedDenseDoubleMatrix2D(elements, rowOffsets, cOffsets, 0);
        }
    }
}
