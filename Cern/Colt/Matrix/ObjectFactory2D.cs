using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix.Implementation;

namespace Cern.Colt.Matrix
{
    public class ObjectFactory2D: PersistentObject
    {

        #region Local Variables
        private static ObjectFactory2D _dense = new ObjectFactory2D();
        private static ObjectFactory2D _sparse = new ObjectFactory2D();
        #endregion

        #region Property
        public static ObjectFactory2D Dense
        {
            get { return _dense; }
        }

        public static ObjectFactory2D Sparse
        {
            get { return _sparse; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        protected ObjectFactory2D() { }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        public ObjectMatrix2D AppendColumns(ObjectMatrix2D A, ObjectMatrix2D B)
        {
            // force both to have maximal shared number of rows.
            if (B.Rows > A.Rows) B = B.ViewPart(0, 0, A.Rows, B.Columns);
            else if (B.Rows < A.Rows) A = A.ViewPart(0, 0, B.Rows, A.Columns);

            // concatenate
            int ac = A.Columns;
            int bc = B.Columns;
            int r = A.Rows;
            ObjectMatrix2D matrix = Make(r, ac + bc);
            matrix.ViewPart(0, 0, r, ac).Assign(A);
            matrix.ViewPart(0, ac, r, bc).Assign(B);
            return matrix;
        }

        public ObjectMatrix2D AppendRows(ObjectMatrix2D A, ObjectMatrix2D B)
        {
            // force both to have maximal shared number of columns.
            if (B.Columns > A.Columns) B = B.ViewPart(0, 0, B.Rows, A.Columns);
            else if (B.Columns < A.Columns) A = A.ViewPart(0, 0, A.Rows, B.Columns);

            // concatenate
            int ar = A.Rows;
            int br = B.Rows;
            int c = A.Columns;
            ObjectMatrix2D matrix = Make(ar + br, c);
            matrix.ViewPart(0, 0, ar, c).Assign(A);
            matrix.ViewPart(ar, 0, br, c).Assign(B);
            return matrix;
        }

        protected static void CheckRectangularShape(ObjectMatrix2D[][] array)
        {
            int columns = -1;
            for (int row = array.Length; --row >= 0;)
            {
                if (array[row] != null)
                {
                    if (columns == -1) columns = array[row].Length;
                    if (array[row].Length != columns) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_AllRowsOfArrayMustHaveSameNumberOfColumns);
                }
            }
        }

        protected static void CheckRectangularShape(Object[][] array)
        {
            int columns = -1;
            for (int row = array.Length; --row >= 0;)
            {
                if (array[row] != null)
                {
                    if (columns == -1) columns = array[row].Length;
                    if (array[row].Length != columns) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_AllRowsOfArrayMustHaveSameNumberOfColumns);
                }
            }
        }

        public ObjectMatrix2D Compose(ObjectMatrix2D[][] parts)
        {
            CheckRectangularShape(parts);
            int rows = parts.Length;
            int columns = 0;
            if (parts.Length > 0) columns = parts.GetLength(1);
            ObjectMatrix2D empty = Make(0, 0);

            if (rows == 0 || columns == 0) return empty;

            // determine maximum column width of each column
            int[] maxWidths = new int[columns];
            for (int column = columns; --column >= 0;)
            {
                int maxWidth = 0;
                for (int row = rows; --row >= 0;)
                {
                    ObjectMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        int width = part.Columns;
                        if (maxWidth > 0 && width > 0 && width != maxWidth) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_DifferentNumberOfColumns);
                        maxWidth = System.Math.Max(maxWidth, width);
                    }
                }
                maxWidths[column] = maxWidth;
            }

            // determine row height of each row
            int[] maxHeights = new int[rows];
            for (int row = rows; --row >= 0;)
            {
                int maxHeight = 0;
                for (int column = columns; --column >= 0;)
                {
                    ObjectMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        int height = part.Rows;
                        if (maxHeight > 0 && height > 0 && height != maxHeight) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_DifferentNumberOfRows);
                        maxHeight = System.Math.Max(maxHeight, height);
                    }
                }
                maxHeights[row] = maxHeight;
            }


            // shape of result 
            int resultRows = 0;
            for (int row = rows; --row >= 0;) resultRows += maxHeights[row];
            int resultCols = 0;
            for (int column = columns; --column >= 0;) resultCols += maxWidths[column];

            ObjectMatrix2D matrix = Make(resultRows, resultCols);

            // copy
            int r = 0;
            for (int row = 0; row < rows; row++)
            {
                int c = 0;
                for (int column = 0; column < columns; column++)
                {
                    ObjectMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        matrix.ViewPart(r, c, part.Rows, part.Columns).Assign(part);
                    }
                    c += maxWidths[column];
                }
                r += maxHeights[row];
            }

            return matrix;
        }

        public ObjectMatrix2D ComposeDiagonal(ObjectMatrix2D A, ObjectMatrix2D B)
        {
            int ar = A.Rows; int ac = A.Columns;
            int br = B.Rows; int bc = B.Columns;
            ObjectMatrix2D sum = Make(ar + br, ac + bc);
            sum.ViewPart(0, 0, ar, ac).Assign(A);
            sum.ViewPart(ar, ac, br, bc).Assign(B);
            return sum;
        }

        public ObjectMatrix2D ComposeDiagonal(ObjectMatrix2D A, ObjectMatrix2D B, ObjectMatrix2D C)
        {
            ObjectMatrix2D diag = Make(A.Rows + B.Rows + C.Rows, A.Columns + B.Columns + C.Columns);
            diag.ViewPart(0, 0, A.Rows, A.Columns).Assign(A);
            diag.ViewPart(A.Rows, A.Columns, B.Rows, B.Columns).Assign(B);
            diag.ViewPart(A.Rows + B.Rows, A.Columns + B.Columns, C.Rows, C.Columns).Assign(C);
            return diag;
        }

        public void Decompose(ObjectMatrix2D[][] parts, ObjectMatrix2D matrix)
        {
            CheckRectangularShape(parts);
            int rows = parts.Length;
            int columns = 0;
            if (parts.Length > 0) columns = parts.GetLength(1);
            if (rows == 0 || columns == 0) return;

            // determine maximum column width of each column
            int[] maxWidths = new int[columns];
            for (int column = columns; --column >= 0;)
            {
                int maxWidth = 0;
                for (int row = rows; --row >= 0;)
                {
                    ObjectMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        int width = part.Columns;
                        if (maxWidth > 0 && width > 0 && width != maxWidth) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_DifferentNumberOfColumns);
                        maxWidth = System.Math.Max(maxWidth, width);
                    }
                }
                maxWidths[column] = maxWidth;
            }

            // determine row height of each row
            int[] maxHeights = new int[rows];
            for (int row = rows; --row >= 0;)
            {
                int maxHeight = 0;
                for (int column = columns; --column >= 0;)
                {
                    ObjectMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        int height = part.Rows;
                        if (maxHeight > 0 && height > 0 && height != maxHeight) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_DifferentNumberOfRows);
                        maxHeight = System.Math.Max(maxHeight, height);
                    }
                }
                maxHeights[row] = maxHeight;
            }


            // shape of result parts
            int resultRows = 0;
            for (int row = rows; --row >= 0;) resultRows += maxHeights[row];
            int resultCols = 0;
            for (int column = columns; --column >= 0;) resultCols += maxWidths[column];

            if (matrix.Rows < resultRows || matrix.Columns < resultCols) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_PartsLargerThanMatrix);

            // copy
            int r = 0;
            for (int row = 0; row < rows; row++)
            {
                int c = 0;
                for (int column = 0; column < columns; column++)
                {
                    ObjectMatrix2D part = parts[row][column];
                    if (part != null)
                    {
                        part.Assign(matrix.ViewPart(r, c, part.Rows, part.Columns));
                    }
                    c += maxWidths[column];
                }
                r += maxHeights[row];
            }

        }

        public ObjectMatrix2D Diagonal(ObjectMatrix1D vector)
        {
            int size = vector.Size;
            ObjectMatrix2D diag = Make(size, size);
            for (int i = size; --i >= 0;)
            {
                diag[i, i] = vector[i];
            }
            return diag;
        }

        public ObjectMatrix1D Diagonal(ObjectMatrix2D A)
        {
            int min = System.Math.Min(A.Rows, A.Columns);
            ObjectMatrix1D diag = Make1D(min);
            for (int i = min; --i >= 0;)
            {
                diag[i] = A[i, i];
            }
            return diag;
        }

        public ObjectMatrix2D Make(Object[,] values)
        {
            return Make(values.ToJagged());
        }

        public ObjectMatrix2D Make(Object[][] values)
        {
            if (this == Sparse) return new SparseObjectMatrix2D(values);
            else return new DenseObjectMatrix2D(values);
        }

        public ObjectMatrix2D Make(Object[] values, int rows)
        {
            int columns = (rows != 0 ? values.Length / rows : 0);
            if (rows * columns != values.Length)
                throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_ArrayLengthMustBeAMultipleOfM);

            ObjectMatrix2D matrix = Make(rows, columns);
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    matrix[row, column] = values[row + column * rows];
                }
            }
            return matrix;
        }


        public ObjectMatrix2D Make(int rows, int columns)
        {
            if (this == Sparse) return new SparseObjectMatrix2D(rows, columns);
            else return new DenseObjectMatrix2D(rows, columns);
        }
        /**
         * Constructs a matrix with the given shape, each cell initialized with the given value.
         */
        public ObjectMatrix2D Make(int rows, int columns, Object initialValue)
        {
            if (initialValue == null) return Make(rows, columns);
            return Make(rows, columns).Assign(initialValue);
        }
        /**
         * Constructs a 1d matrix of the right dynamic type.
         */
        protected ObjectMatrix1D Make1D(int size)
        {
            return Make(0, 0).Like1D(size);
        }
        /**
C = A||A||..||A; Constructs a new matrix which is duplicated both along the row and column dimension.
Example:
<pre>
0 1
2 3
repeat(2,3) -->
0 1 0 1 0 1
2 3 2 3 2 3
0 1 0 1 0 1
2 3 2 3 2 3
</pre>
*/
        public ObjectMatrix2D Repeat(ObjectMatrix2D A, int rowRepeat, int columnRepeat)
        {
            int r = A.Rows;
            int c = A.Columns;
            ObjectMatrix2D matrix = Make(r * rowRepeat, c * columnRepeat);
            for (int i = rowRepeat; --i >= 0;)
            {
                for (int j = columnRepeat; --j >= 0;)
                {
                    matrix.ViewPart(r * i, c * j, r, c).Assign(A);
                }
            }
            return matrix;
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
