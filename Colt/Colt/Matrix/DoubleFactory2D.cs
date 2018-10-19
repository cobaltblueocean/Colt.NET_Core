// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleFactory2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Factory for convenient construction of 2-d matrices holding <tt>double</tt> cells. Also provides convenient methods to compose (concatenate) and decompose (split) matrices from/to constituent blocks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix
{
    using System;

    using Function;

    using Implementation;

    /// <summary>
    /// Factory for convenient construction of 2-d matrices holding <tt>double</tt> cells. Also provides convenient methods to compose (concatenate) and decompose (split) matrices from/to constituent blocks.
    /// </summary>
    public class DoubleFactory2D : PersistentObject
    {
        /// <summary>
        /// A factory producing dense matrices.
        /// </summary>
        public static readonly DoubleFactory2D Dense = new DoubleFactory2D();

        /// <summary>
        /// A factory producing sparse hash matrices.
        /// </summary>
        public static readonly DoubleFactory2D Sparse = new DoubleFactory2D();

        /// <summary>
        /// A factory producing sparse hash matrices.
        /// </summary>
        public static readonly DoubleFactory2D RowCompressed = new DoubleFactory2D();

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleFactory2D"/> class.
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected DoubleFactory2D()
        {
        }

        /// <summary>
        /// C = A||B; Constructs a new matrix which is the column-wise concatenation of two other matrices.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="b">
        /// The matrix B.
        /// </param>
        /// <returns>
        /// The column-wise concatenation of A and B.
        /// </returns>
        public DoubleMatrix2D AppendColumns(DoubleMatrix2D a, DoubleMatrix2D b)
        {
            // force both to have maximal shared number of rows.
            if (b.Rows > a.Rows) b = b.ViewPart(0, 0, a.Rows, b.Columns);
            else if (b.Rows < a.Rows) a = a.ViewPart(0, 0, b.Rows, a.Columns);

            // concatenate
            int ac = a.Columns;
            int bc = b.Columns;
            int r = a.Rows;
            DoubleMatrix2D matrix = Make(r, ac + bc);
            matrix.ViewPart(0, 0, r, ac).Assign(a);
            matrix.ViewPart(0, ac, r, bc).Assign(b);
            return matrix;
        }

        /// <summary>
        /// C = A||B; Constructs a new matrix which is the row-wise concatenation of two other matrices.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="b">
        /// The matrix B.
        /// </param>
        /// <returns>
        /// A new matrix which is the row-wise concatenation of A and B.
        /// </returns>
        public DoubleMatrix2D AppendRows(DoubleMatrix2D a, DoubleMatrix2D b)
        {
            // force both to have maximal shared number of columns.
            if (b.Columns > a.Columns) b = b.ViewPart(0, 0, b.Rows, a.Columns);
            else if (b.Columns < a.Columns) a = a.ViewPart(0, 0, a.Rows, b.Columns);

            // concatenate
            int ar = a.Rows;
            int br = b.Rows;
            int c = a.Columns;
            DoubleMatrix2D matrix = Make(ar + br, c);
            matrix.ViewPart(0, 0, ar, c).Assign(a);
            matrix.ViewPart(ar, 0, br, c).Assign(b);
            return matrix;
        }

        /// <summary>
        /// Constructs a matrix with cells having ascending values.
        /// For debugging purposes.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A matrix with cells having ascending values.
        /// </returns>
        public DoubleMatrix2D Ascending(int rows, int columns)
        {
            return Descending(rows, columns).Assign(UnaryFunctions.Chain(a => -a, UnaryFunctions.Minus(columns * rows)));
        }

        /// <summary>
        /// Constructs a block matrix made from the given parts.
        /// <para>
        /// All matrices of a given column within <tt>parts</tt> must have the same number of columns.
        /// All matrices of a given row within <tt>parts</tt> must have the same number of rows.
        /// Otherwise an <tt>IllegalArgumentException</tt> is thrown. 
        /// <tt>null</tt>s within <tt>parts[row,col]</tt> are an exception to this rule: they are ignored.
        /// Cells are copied.
        /// </para>
        /// </summary>
        /// <param name="parts">
        /// The parts.
        /// </param>
        /// <returns>
        /// A block matrix.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the parts are not subject to the conditions outlined above.
        /// </exception>
        public DoubleMatrix2D Compose(DoubleMatrix2D[][] parts)
        {
            checkRectangularShape(parts);
            int rows = parts.Length;
            int columns = 0;
            if (parts.Length > 0) columns = parts[0].Length;
            DoubleMatrix2D empty = Make(0, 0);

            if (rows == 0 || columns == 0) return empty;

            // determine maximum column width of each column
            var maxWidths = new int[columns];
            for (int column = columns; --column >= 0; )
            {
                int maxWidth = 0;
                for (int row = rows; --row >= 0; )
                {
                    DoubleMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        int width = part.Columns;
                        if (maxWidth > 0 && width > 0 && width != maxWidth) throw new ArgumentOutOfRangeException("parts", "Different number of columns.");
                        maxWidth = Math.Max(maxWidth, width);
                    }
                }

                maxWidths[column] = maxWidth;
            }

            // determine row height of each row
            var maxHeights = new int[rows];
            for (int row = rows; --row >= 0; )
            {
                int maxHeight = 0;
                for (int column = columns; --column >= 0; )
                {
                    DoubleMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        int height = part.Rows;
                        if (maxHeight > 0 && height > 0 && height != maxHeight) throw new ArgumentOutOfRangeException("parts", "Different number of rows.");
                        maxHeight = Math.Max(maxHeight, height);
                    }
                }

                maxHeights[row] = maxHeight;
            }

            // shape of result 
            int resultRows = 0;
            for (int row = rows; --row >= 0; ) resultRows += maxHeights[row];
            int resultCols = 0;
            for (int column = columns; --column >= 0; ) resultCols += maxWidths[column];

            DoubleMatrix2D matrix = Make(resultRows, resultCols);

            // copy
            int r = 0;
            for (int row = 0; row < rows; row++)
            {
                int c = 0;
                for (int column = 0; column < columns; column++)
                {
                    DoubleMatrix2D part = parts[row][column];
                    if (part != null)
                        matrix.ViewPart(r, c, part.Rows, part.Columns).Assign(part);
                    c += maxWidths[column];
                }

                r += maxHeights[row];
            }

            return matrix;
        }

        /// <summary>
        /// Constructs a diagonal block matrix from the given parts (the <i>direct sum</i> of two matrices).
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="b">
        /// The matrix B.
        /// </param>
        /// <returns>
        /// A diagonal block matrix.
        /// </returns>
        public DoubleMatrix2D ComposeDiagonal(DoubleMatrix2D a, DoubleMatrix2D b)
        {
            int ar = a.Rows;
            int ac = a.Columns;
            int br = b.Rows;
            int bc = b.Columns;
            DoubleMatrix2D sum = Make(ar + br, ac + bc);
            sum.ViewPart(0, 0, ar, ac).Assign(a);
            sum.ViewPart(ar, ac, br, bc).Assign(b);
            return sum;
        }

        /// <summary>
        /// Constructs a diagonal block matrix from the given parts.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="b">
        /// The matrix B.
        /// </param>
        /// <param name="c">
        /// The matrix C.
        /// </param>
        /// <returns>
        /// A diagonal block matrix.
        /// </returns>
        public DoubleMatrix2D ComposeDiagonal(DoubleMatrix2D a, DoubleMatrix2D b, DoubleMatrix2D c)
        {
            DoubleMatrix2D diag = Make(a.Rows + b.Rows + c.Rows, a.Columns + b.Columns + c.Columns);
            diag.ViewPart(0, 0, a.Rows, a.Columns).Assign(a);
            diag.ViewPart(a.Rows, a.Columns, b.Rows, b.Columns).Assign(b);
            diag.ViewPart(a.Rows + b.Rows, a.Columns + b.Columns, c.Rows, c.Columns).Assign(c);
            return diag;
        }

        /// <summary>
        /// Splits a block matrix into its constituent blocks; Copies blocks of a matrix into the given parts.
        /// <para>
        /// All matrices of a given column within <tt>parts</tt> must have the same number of columns.
        /// All matrices of a given row within <tt>parts</tt> must have the same number of rows.
        /// Otherwise an <tt>IllegalArgumentException</tt> is thrown. 
        /// <tt>null</tt>s within <tt>parts[row,col]</tt> are an exception to this rule: they are ignored.
        /// Cells are copied.
        /// </para>
        /// </summary>
        /// <param name="parts">
        /// The parts.
        /// </param>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <exception cref="ArgumentException">
        /// subject to the conditions outlined above.
        /// </exception>
        public void Decompose(DoubleMatrix2D[][] parts, DoubleMatrix2D matrix)
        {
            checkRectangularShape(parts);
            int rows = parts.Length;
            int columns = 0;
            if (parts.Length > 0) columns = parts[0].Length;
            if (rows == 0 || columns == 0) return;

            // determine maximum column width of each column
            var maxWidths = new int[columns];
            for (int column = columns; --column >= 0; )
            {
                int maxWidth = 0;
                for (int row = rows; --row >= 0; )
                {
                    DoubleMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        int width = part.Columns;
                        if (maxWidth > 0 && width > 0 && width != maxWidth) throw new ArgumentException("Different number of columns.");
                        maxWidth = Math.Max(maxWidth, width);
                    }
                }

                maxWidths[column] = maxWidth;
            }

            // determine row height of each row
            var maxHeights = new int[rows];
            for (int row = rows; --row >= 0; )
            {
                int maxHeight = 0;
                for (int column = columns; --column >= 0; )
                {
                    DoubleMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        int height = part.Rows;
                        if (maxHeight > 0 && height > 0 && height != maxHeight) throw new ArgumentException("Different number of rows.");
                        maxHeight = Math.Max(maxHeight, height);
                    }
                }

                maxHeights[row] = maxHeight;
            }

            // shape of result parts
            int resultRows = 0;
            for (int row = rows; --row >= 0; ) resultRows += maxHeights[row];
            int resultCols = 0;
            for (int column = columns; --column >= 0; ) resultCols += maxWidths[column];

            if (matrix.Rows < resultRows || matrix.Columns < resultCols) throw new ArgumentException("Parts larger than matrix.");

            // copy
            int r = 0;
            for (int row = 0; row < rows; row++)
            {
                int c = 0;
                for (int column = 0; column < columns; column++)
                {
                    DoubleMatrix2D part = parts[row][column];
                    if (part != null)
                        part.Assign(matrix.ViewPart(r, c, part.Rows, part.Columns));
                    c += maxWidths[column];
                }

                r += maxHeights[row];
            }
        }

        /// <summary>
        /// Constructs a matrix with cells having descending values.
        /// For debugging purposes.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A matrix with cells having descending values.
        /// </returns>
        public DoubleMatrix2D Descending(int rows, int columns)
        {
            DoubleMatrix2D matrix = Make(rows, columns);
            int v = 0;
            for (int row = rows; --row >= 0; )
                for (int column = columns; --column >= 0; )
                    matrix[row, column] = v++;
            return matrix;
        }

        /// <summary>
        /// Constructs a new diagonal matrix whose diagonal elements are the elements of <tt>vector</tt>.
        /// Cells values are copied. The new matrix is not a view.
        /// </summary>
        /// <param name="vector">
        /// The vector.
        /// </param>
        /// <returns>
        /// A new matrix.
        /// </returns>
        public DoubleMatrix2D Diagonal(DoubleMatrix1D vector)
        {
            int size = vector.Size;
            DoubleMatrix2D diag = Make(size, size);
            for (int i = size; --i >= 0; )
                diag[i, i] = vector[i];
            return diag;
        }

        /// <summary>
        /// Constructs a new vector consisting of the diagonal elements of <tt>A</tt>.
        /// Cells values are copied. The new vector is not a view.
        /// </summary>
        /// <param name="a">
        /// The amatrix, need not be square.
        /// </param>
        /// <returns>
        /// A new vector.
        /// </returns>
        public DoubleMatrix1D Diagonal(DoubleMatrix2D a)
        {
            int min = Math.Min(a.Rows, a.Columns);
            DoubleMatrix1D diag = make1D(min);
            for (int i = min; --i >= 0; )
                diag[i] = a[i, i];
            return diag;
        }

        /// <summary>
        /// Constructs an identity matrix (having ones on the diagonal and zeros elsewhere).
        /// </summary>
        /// <param name="rowsAndColumns">
        /// The rows and columns.
        /// </param>
        /// <returns>
        /// An identity matrix.
        /// </returns>
        public DoubleMatrix2D Identity(int rowsAndColumns)
        {
            DoubleMatrix2D matrix = Make(rowsAndColumns, rowsAndColumns);
            for (int i = rowsAndColumns; --i >= 0; )
                matrix[i, i] = 1;
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>for any 1 &lt;= row &lt; values.length: values[row].length != values[row-1].length</tt>.
        /// </exception>
        public DoubleMatrix2D Make(double[][] values)
        {
            if (this == Sparse) return new SparseDoubleMatrix2D(values);
            return new DenseDoubleMatrix2D(values);
        }

        /// <summary>
        /// Construct a matrix from a one-dimensional column-major packed array, ala Fortran.
        /// </summary>
        /// <param name="values">
        /// One-dimensional array of doubles, packed by columns (ala Fortran).
        /// </param>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <tt>values.length</tt> must be a multiple of <tt>rows</tt>.
        /// </exception>
        public DoubleMatrix2D Make(double[] values, int rows)
        {
            int columns = rows != 0 ? values.Length / rows : 0;
            if (rows * columns != values.Length)
                throw new ArgumentException("Array length must be a multiple of m.");

            DoubleMatrix2D matrix = Make(rows, columns);
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    matrix[row, column] = values[row + (column * rows)];
            
            return matrix;
        }

        /// <summary>
        /// Constructs a matrix with the given shape, each cell initialized with zero.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix2D Make(int rows, int columns)
        {
            if (this == Sparse) return new SparseDoubleMatrix2D(rows, columns);
            if (this == RowCompressed) return new RCDoubleMatrix2D(rows, columns);
            return new DenseDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Constructs a matrix with the given shape, each cell initialized with the given value.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <param name="initialValue">
        /// The initial value.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix2D Make(int rows, int columns, double initialValue)
        {
            if (initialValue == 0) return Make(rows, columns);
            return Make(rows, columns).Assign(initialValue);
        }

        /// <summary>
        /// Constructs a matrix with uniformly distributed values in <tt>(0,1)</tt> (exclusive).
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix2D Random(int rows, int columns)
        {
            var r = new Random();
            return Make(rows, columns).Assign(a => r.NextDouble());
        }

        /// <summary>
        /// Constructs a new matrix which is duplicated both along the row and column dimension.
        /// </summary>
        /// <param name="a">
        /// The matrix to duplicate.
        /// </param>
        /// <param name="rowRepeat">
        /// The number of row repetitions.
        /// </param>
        /// <param name="columnRepeat">
        /// The number of column repetitions.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        public DoubleMatrix2D Repeat(DoubleMatrix2D a, int rowRepeat, int columnRepeat)
        {
            int r = a.Rows;
            int c = a.Columns;
            DoubleMatrix2D matrix = Make(r * rowRepeat, c * columnRepeat);
            for (int i = rowRepeat; --i >= 0; )
                for (int j = columnRepeat; --j >= 0; )
                    matrix.ViewPart(r * i, c * j, r, c).Assign(a);
            
            return matrix;
        }

        /// <summary>
        /// Checks whether the given array is rectangular, that is, whether all rows have the same number of columns.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the array is not rectangular.
        /// </exception>
        protected static void checkRectangularShape(double[][] array)
        {
            int columns = -1;
            for (int row = array.Length; --row >= 0; )
            {
                if (array[row] != null)
                {
                    if (columns == -1) columns = array[row].Length;
                    if (array[row].Length != columns) throw new ArgumentOutOfRangeException("array", "All rows of array must have same number of columns.");
                }
            }
        }

        /// <summary>
        /// Checks whether the given array is rectangular, that is, whether all rows have the same number of columns.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the array is not rectangular.
        /// </exception>
        protected static void checkRectangularShape(DoubleMatrix2D[][] array)
        {
            int columns = -1;
            for (int row = array.Length; --row >= 0; )
            {
                if (array[row] != null)
                {
                    if (columns == -1) columns = array[row].Length;
                    if (array[row].Length != columns) throw new ArgumentOutOfRangeException("array", "All rows of array must have same number of columns.");
                }
            }
        }

        /// <summary>
        /// Constructs a 1d matrix of the right dynamic type.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// A matrix.
        /// </returns>
        protected DoubleMatrix1D make1D(int size)
        {
            return Make(0, 0).Like1D(size);
        }
    }
}
