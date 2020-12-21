namespace Cern.Colt.Matrix.LinearAlgebra
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using Implementation;
    using F1 = Cern.Jet.Math.Functions.DoubleFunctions;
    using F2 = Cern.Jet.Math.Functions.DoubleDoubleFunctions;


    /// <summary>
    /// Tests matrices for linear algebraic properties (equality, tridiagonality, symmetry, singularity, etc).
    /// </summary>
    public class Property : PersistentObject
    {
        /// <summary>
        /// The default Property object; currently has <i>Tolerance==1.0E-9</i>.
        /// </summary>
        public static readonly Property DEFAULT = new Property(1.0E-9);

        /// <summary>
        /// A Property object with <i>tolerance()==0.0</i>.
        /// </summary>
        public static Property ZERO = new Property(0.0);

        /// <summary>
        /// A Property object with <i>tolerance()==1.0E-12</i>.
        /// </summary>
        public static Property TWELVE = new Property(1.0E-12);

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class with a tolerance of <i>System.Math.Abs(newTolerance)</i>.
        /// </summary>
        /// <param name="newTolerance">
        /// The new tolerance.
        /// </param>
        public Property(double newTolerance)
        {
            Tolerance = System.Math.Abs(newTolerance);
        }

        /// <summary>
        /// Gets or sets the current tolerance.
        /// </summary>
        public double Tolerance { get; protected set; }

        /// <summary>
        /// Returns a String with <i>Length</i> blanks.
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        internal static String Blanks(int Length)
        {
            if (Length < 0) Length = 0;
            StringBuilder buf = new StringBuilder(Length);
            for (int k = 0; k < Length; k++)
            {
                buf.Append(' ');
            }
            return buf.ToString();
        }

        /// <summary>
        /// Checks whether the given matrix <i>A</i> is <i>rectangular</i>.
        /// </summary>
        /// <param name="a">
        /// The matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <i>A.Rows &lt; A.Columns</i>.
        /// </exception>
        public void CheckRectangular(DoubleMatrix2D a)
        {
            if (a.Rows < a.Columns)
            {
                throw new ArgumentOutOfRangeException(String.Format(Cern.LocalizedResources.Instance().Exception_MatrixMustBeRectangular, AbstractFormatter.Shape(a)));
            }
        }

        /// <summary>
        /// Checks whether the given matrix <i>A</i> is <i>square</i>.
        /// </summary>
        /// <param name="A"></param>
        /// <exception cref="ArgumentException">if <i>A.Rows != A.Columns</i>.</exception>
        public void CheckSquare(DoubleMatrix2D A)
        {
            if (A.Rows != A.Columns) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_MatrixMustBeSquare);
        }

        /// <summary>
        /// Returns the matrix's fraction of non-zero cells; <i>A.cardinality() / A.Count</i>.
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static double Density(DoubleMatrix2D A)
        {
            return A.Cardinality() / (double)A.Size;
        }

        /// <summary>
        /// Returns whether all cells of the given matrix <i>A</i> are equal to the given value.
        /// The result is <i>true</i> if and only if <i>A != null</i> and
        /// <i>! (System.Math.Abs(value - A[i]) > Tolerance)</i> holds for all coordinates.
        /// </summary>
        /// <param name="a">
        /// The first matrix to compare.
        /// </param>
        /// <param name="value">
        /// The value to compare against.
        /// </param>
        /// <returns>
        /// <i>true</i> if the matrix is equal to the value;
        /// <i>false</i> otherwise.
        /// </returns>
        public bool Equals(DoubleMatrix1D a, double value)
        {
            if (a == null) return false;
            double epsilon = Tolerance;
            for (int i = a.Size; --i >= 0;)
            {
                double x = a[i];
                double diff = System.Math.Abs(value - x);
                if ((Double.IsNaN(diff)) && ((Double.IsNaN(value) && Double.IsNaN(x)) || value == x)) diff = 0;
                if (!(diff <= epsilon)) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether both given matrices <i>A</i> and <i>B</i> are equal.
        /// The result is <i>true</i> if <i>A==B</i>d 
        /// Otherwise, the result is <i>true</i> if and only if both arguments are <i>!= null</i>, 
        /// have the same size and
        /// <i>! (System.Math.Abs(A[i] - B[i]) > Tolerance)</i> holds for all indexes.
        /// </summary>
        /// <param name="a">
        /// The first matrix to compare.
        /// </param>
        /// <param name="b">
        /// The second matrix to compare.
        /// </param>
        /// <returns>
        /// <i>true</i> if both matrices are equal;
        /// <i>false</i> otherwise.
        /// </returns>
        public bool Equals(DoubleMatrix1D a, DoubleMatrix1D b)
        {
            if (a == b) return true;
            if (!(a != null && b != null)) return false;
            int size = a.Size;
            if (size != b.Size) return false;

            double epsilon = Tolerance;
            for (int i = size; --i >= 0;)
            {
                double x = a[i];
                double value = b[i];
                double diff = System.Math.Abs(value - x);
                if ((Double.IsNaN(diff)) && ((Double.IsNaN(value) && Double.IsNaN(x)) || value == x)) diff = 0;
                if (!(diff <= epsilon)) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether all cells of the given matrix <i>A</i> are equal to the given value.
        /// </summary>
        /// <param name="a">
        /// The first matrix to compare.
        /// </param>
        /// <param name="value">
        /// The value to compare against.
        /// </param>
        /// <returns>
        /// <i>true</i> if the matrix is equal to the value;
        /// <i>false</i> otherwise.
        /// </returns>
        public bool Equals(DoubleMatrix2D a, double value)
        {
            if (a == null) return false;
            int rows = a.Rows;
            int columns = a.Columns;

            double epsilon = Tolerance;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    double x = a[row, column];
                    double diff = System.Math.Abs(value - x);
                    if ((Double.IsNaN(diff)) && ((Double.IsNaN(value) && Double.IsNaN(x)) || value == x)) diff = 0;
                    if (!(diff <= epsilon)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether both given matrices <i>A</i> and <i>B</i> are equal.
        /// </summary>
        /// <param name="a">
        /// The first matrix to compare.
        /// </param>
        /// <param name="b">
        /// The second matrix to compare.
        /// </param>
        /// <returns>
        /// <i>true</i> if both matrices are equal;
        /// <i>false</i> otherwise.
        /// </returns>
        public bool Equals(DoubleMatrix2D a, DoubleMatrix2D b)
        {
            if (a == b) return true;
            if (!(a != null && b != null)) return false;
            int rows = a.Rows;
            int columns = a.Columns;
            if (columns != b.Columns || rows != b.Rows) return false;

            double epsilon = Tolerance;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    double x = a[row, column];
                    double value = b[row, column];
                    double diff = System.Math.Abs(value - x);
                    if ((Double.IsNaN(diff)) && ((Double.IsNaN(value) && Double.IsNaN(x)) || value == x)) diff = 0;
                    if (!(diff <= epsilon))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether all cells of the given matrix <i>A</i> are equal to the given value.
        /// The result is <i>true</i> if and only if <i>A != null</i> and
        /// <i>! (System.Math.Abs(value - A[slice,row,col]) > Tolerance)</i> holds for all coordinates.
        /// @param   A   the first matrix to compare.
        /// @param   value   the value to compare against.
        /// @return  <i>true</i> if the matrix is equal to the value;
        ///          <i>false</i> otherwise.
        /// </summary>
        public Boolean Equals(DoubleMatrix3D A, double value)
        {
            if (A == null) return false;
            int rows = A.Rows;
            int columns = A.Columns;

            double epsilon = Tolerance;
            for (int slice = A.Slices; --slice >= 0;)
            {
                for (int row = rows; --row >= 0;)
                {
                    for (int column = columns; --column >= 0;)
                    {
                        //if (!(A.getQuick(slice,row,column) == value)) return false;
                        //if (System.Math.Abs(value - A.getQuick(slice,row,column)) > epsilon) return false;
                        double x = A[slice, row, column];
                        double diff = System.Math.Abs(value - x);
                        if ((Double.IsNaN(diff)) && ((Double.IsNaN(value) && Double.IsNaN(x)) || value == x)) diff = 0;
                        if (!(diff <= epsilon)) return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Returns whether both given matrices <i>A</i> and <i>B</i> are equal.
        /// The result is <i>true</i> if <i>A==B</i>d 
        /// Otherwise, the result is <i>true</i> if and only if both arguments are <i>!= null</i>, 
        /// have the same number of columns, rows and slices, and
        /// <i>! (System.Math.Abs(A[slice,row,col] - B[slice,row,col]) > Tolerance)</i> holds for all coordinates.
        /// @param   A   the first matrix to compare.
        /// @param   B   the second matrix to compare.
        /// @return  <i>true</i> if both matrices are equal;
        ///          <i>false</i> otherwise.
        /// </summary>
        public Boolean Equals(DoubleMatrix3D A, DoubleMatrix3D B)
        {
            if (A == B) return true;
            if (!(A != null && B != null)) return false;
            int slices = A.Slices;
            int rows = A.Rows;
            int columns = A.Columns;
            if (columns != B.Columns || rows != B.Rows || slices != B.Slices) return false;

            double epsilon = Tolerance;
            for (int slice = slices; --slice >= 0;)
            {
                for (int row = rows; --row >= 0;)
                {
                    for (int column = columns; --column >= 0;)
                    {
                        //if (!(A.getQuick(slice,row,column) == B.getQuick(slice,row,column))) return false;
                        //if (System.Math.Abs(A.getQuick(slice,row,column) - B.getQuick(slice,row,column)) > epsilon) return false;
                        double x = A[slice, row, column];
                        double value = B[slice, row, column];
                        double diff = System.Math.Abs(value - x);
                        if ((Double.IsNaN(diff)) && ((Double.IsNaN(value) && Double.IsNaN(x)) || value == x)) diff = 0;
                        if (!(diff <= epsilon)) return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Modifies the given matrix square matrix<i>A</i> such that it is diagonally dominant by row and column, hence non-singular, hence invertible.
        /// For testing purposes only.
        /// <summary>
        ///<param name = "A" > the square matrix to modify.</param>
        ///<exception cref = "ArgumentException" >if <i>!isSquare(A)</i>. </exception>
        public void GenerateNonSingular(DoubleMatrix2D A)
        {
            CheckSquare(A);
            var F = Cern.Jet.Math.Functions.functions;
            int min = System.Math.Min(A.Rows, A.Columns);
            for (int i = min; --i >= 0;)
            {
                A[i, i] = 0;
            }
            for (int i = min; --i >= 0;)
            {
                double rowSum = A.ViewRow(i).Aggregate(F2.Plus, F1.Abs);
                double colSum = A.ViewColumn(i).Aggregate(F2.Plus, F1.Abs);
                A[i, i] = System.Math.Max(rowSum, colSum) + i + 1;
            }
        }
        /// <summary>
        /// <summary>
        protected static String Get(List<Object> list, int index)
        {
            return ((String)list[index]);
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>diagonal</i> if <i>A[i,j] == 0</i> whenever <i>i != j</i>.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsDiagonal(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    //if (row!=column && A.getQuick(row,column) != 0) return false;
                    if (row != column && !(System.Math.Abs(A[row, column]) <= epsilon)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>diagonally dominant by column</i> if the absolute value of each diagonal element is larger than the sum of the absolute values of theoff-diagonal elements in the corresponding column.
        /// <i>returns true if for all i: abs(A[i,i]) &gt; Sum(abs(A[j,i])); j != i.</i>
        /// Matrix may but need not be square.
        /// <p>
        /// Note: Ignores tolerance.
        /// <summary>
        public Boolean IsDiagonallyDominantByColumn(DoubleMatrix2D A)
        {
            Cern.Jet.Math.Functions F = Cern.Jet.Math.Functions.functions;
            double epsilon = Tolerance;
            int min = System.Math.Min(A.Rows, A.Columns);
            for (int i = min; --i >= 0;)
            {
                double diag = System.Math.Abs(A[i, i]);
                diag += diag;
                if (diag <= A.ViewColumn(i).Aggregate(F2.Plus, F1.Abs)) return false;
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>diagonally dominant by row</i> if the absolute value of each diagonal element is larger than the sum of the absolute values of the off-diagonal elements in the corresponding row.
        /// <i>returns true if for all i: abs(A[i,i]) &gt; Sum(abs(A[i,j])); j != i.</i>
        /// Matrix may but need not be square.
        /// <p>
        /// Note: Ignores tolerance.
        /// <summary>
        public Boolean IsDiagonallyDominantByRow(DoubleMatrix2D A)
        {
            Cern.Jet.Math.Functions F = Cern.Jet.Math.Functions.functions;
            double epsilon = Tolerance;
            int min = System.Math.Min(A.Rows, A.Columns);
            for (int i = min; --i >= 0;)
            {
                double diag = System.Math.Abs(A[i, i]);
                diag += diag;
                if (diag <= A.ViewRow(i).Aggregate(F2.Plus, F1.Abs)) return false;
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is an <i>identity</i> matrix if <i>A[i,i] == 1</i> and all other cells are zero.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsIdentity(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    double v = A[row, column];
                    if (row == column)
                    {
                        if (!(System.Math.Abs(1 - v) < epsilon)) return false;
                    }
                    else if (!(System.Math.Abs(v) <= epsilon)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>lower bidiagonal</i> if <i>A[i,j]==0</i> unless <i>i==j || i==j+1</i>.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsLowerBidiagonal(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (!(row == column || row == column + 1))
                    {
                        //if (A.getQuick(row,column) != 0) return false;
                        if (!(System.Math.Abs(A[row, column]) <= epsilon)) return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>lower triangular</i> if <i>A[i,j]==0</i> whenever <i>i &lt; j</i>.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsLowerTriangular(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int column = columns; --column >= 0;)
            {
                for (int row = System.Math.Min(column, rows); --row >= 0;)
                {
                    //if (A.getQuick(row,column) != 0) return false;
                    if (!(System.Math.Abs(A[row, column]) <= epsilon)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>non-negative</i> if <i>A[i,j] &gt;= 0</i> holds for all cells.
        /// <p>
        /// Note: Ignores tolerance.
        /// <summary>
        public Boolean IsNonNegative(DoubleMatrix2D A)
        {
            int rows = A.Rows;
            int columns = A.Columns;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (!(A[row, column] >= 0)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A square matrix <i>A</i> is <i>orthogonal</i> if <i>A*transpose(A) = I</i>.
        /// <summary>
        /// <exception cref="ArgumentException">if <i>!isSquare(A)</i>. </exception>
        public Boolean IsOrthogonal(DoubleMatrix2D A)
        {
            CheckSquare(A);
            return Equals(A.ZMult(A, null, 1, 0, false, true), Cern.Colt.Matrix.DoubleFactory2D.Dense.Identity(A.Rows));
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>positive</i> if <i>A[i,j] &gt; 0</i> holds for all cells.
        /// <p>
        /// Note: Ignores tolerance.
        /// <summary>
        public Boolean IsPositive(DoubleMatrix2D A)
        {
            int rows = A.Rows;
            int columns = A.Columns;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (!(A[row, column] > 0)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>singular</i> if it has no inverse, that is, iff <i>det(A)==0</i>.
        /// <summary>
        public Boolean IsSingular(DoubleMatrix2D A)
        {
            var lu = new LUDecomposition(A);

            return !(System.Math.Abs(lu.Det()) >= Tolerance);
        }
        /// <summary>
        /// A square matrix <i>A</i> is <i>skew-symmetric</i> if <i>A = -transpose(A)</i>, that is <i>A[i,j] == -A[j,i]</i>.
        /// <summary>
        /// <exception cref="ArgumentException">if <i>!isSquare(A)</i>. </exception>
        public Boolean IsSkewSymmetric(DoubleMatrix2D A)
        {
            CheckSquare(A);
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int row = rows; --row >= 0;)
            {
                for (int column = rows; --column >= 0;)
                {
                    //if (A.getQuick(row,column) != -A.getQuick(column,row)) return false;
                    if (!(System.Math.Abs(A[row, column] + A[column, row]) <= epsilon)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>square</i> if it has the same number of rows and columns.
        /// <summary>
        public Boolean IsSquare(DoubleMatrix2D A)
        {
            return A.Rows == A.Columns;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>strictly lower triangular</i> if <i>A[i,j]==0</i> whenever <i>i &lt;= j</i>.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsStrictlyLowerTriangular(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int column = columns; --column >= 0;)
            {
                for (int row = System.Math.Min(rows, column + 1); --row >= 0;)
                {
                    //if (A.getQuick(row,column) != 0) return false;
                    if (!(System.Math.Abs(A[row, column]) <= epsilon)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>strictly triangular</i> if it is triangular and its diagonal elements all equal 0.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsStrictlyTriangular(DoubleMatrix2D A)
        {
            if (!IsTriangular(A)) return false;

            double epsilon = Tolerance;
            for (int i = System.Math.Min(A.Rows, A.Columns); --i >= 0;)
            {
                //if (A.getQuick(i,i) != 0) return false;
                if (!(System.Math.Abs(A[i, i]) <= epsilon)) return false;
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>strictly upper triangular</i> if <i>A[i,j]==0</i> whenever <i>i &gt;= j</i>.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsStrictlyUpperTriangular(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int column = columns; --column >= 0;)
            {
                for (int row = rows; --row >= column;)
                {
                    //if (A.getQuick(row,column) != 0) return false;
                    if (!(System.Math.Abs(A[row, column]) <= epsilon)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// A matrix <i>A</i> is <i>symmetric</i> if <i>A = tranpose(A)</i>, that is <i>A[i,j] == A[j,i]</i>.
        /// <summary>
        /// <exception cref="ArgumentException">if <i>!isSquare(A)</i>. </exception>
        public Boolean IsSymmetric(DoubleMatrix2D A)
        {
            CheckSquare(A);
            return Equals(A, A.ViewDice());
        }

        /// <summary>
        /// A matrix <i>A</i> is <i>triangular</i> iff it is either upper or lower triangular.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsTriangular(DoubleMatrix2D A)
        {
            return IsLowerTriangular(A) || IsUpperTriangular(A);
        }

        /// <summary>
        /// A matrix <i>A</i> is <i>tridiagonal</i> if <i>A[i,j]==0</i> whenever <i>System.Math.Abs(i-j) > 1</i>.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsTridiagonal(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (System.Math.Abs(row - column) > 1)
                    {
                        //if (A.getQuick(row,column) != 0) return false;
                        if (!(System.Math.Abs(A[row, column]) <= epsilon)) return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>unit triangular</i> if it is triangular and its diagonal elements all equal 1.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsUnitTriangular(DoubleMatrix2D A)
        {
            if (!IsTriangular(A)) return false;

            double epsilon = Tolerance;
            for (int i = System.Math.Min(A.Rows, A.Columns); --i >= 0;)
            {
                //if (A.getQuick(i,i) != 1) return false;
                if (!(System.Math.Abs(1 - A[i, i]) <= epsilon)) return false;
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>upper bidiagonal</i> if <i>A[i,j]==0</i> unless <i>i==j || i==j-1</i>.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsUpperBidiagonal(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int row = rows; --row >= 0;)
            {
                for (int column = columns; --column >= 0;)
                {
                    if (!(row == column || row == column - 1))
                    {
                        //if (A.getQuick(row,column) != 0) return false;
                        if (!(System.Math.Abs(A[row, column]) <= epsilon)) return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>upper triangular</i> if <i>A[i,j]==0</i> whenever <i>i &gt; j</i>.
        /// Matrix may but need not be square.
        /// <summary>
        public Boolean IsUpperTriangular(DoubleMatrix2D A)
        {
            double epsilon = Tolerance;
            int rows = A.Rows;
            int columns = A.Columns;
            for (int column = columns; --column >= 0;)
            {
                for (int row = rows; --row > column;)
                {
                    //if (A.getQuick(row,column) != 0) return false;
                    if (!(System.Math.Abs(A[row, column]) <= epsilon)) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A matrix <i>A</i> is <i>zero</i> if all its cells are zero.
        /// <summary>
        public Boolean IsZero(DoubleMatrix2D A)
        {
            return Equals(A, 0);
        }
        /// <summary>
        /// The<i> lower bandwidth</i> of a square matrix<i> A</i> is the maximum <i>i-j</i> for which<i> A[i, j]</i> is nonzero and <i>i &gt; j</i>.
        ///  A<i> banded</i> matrix has a "band" about the diagonal.
        ///  Diagonal, tridiagonal and triangular matrices are special cases.
/// <summary>
/// </summary>
///<param name = "A" > the square matrix to analyze.</param>
///<returns>the lower bandwith.</returns>
///<exception cref = "ArgumentException" >if <i>!isSquare(A)</i>. </exception>
///<see cref = "#semiBandwidth(DoubleMatrix2D)" ></ see>
///<see cref= "#upperBandwidth(DoubleMatrix2D)" ></ see>
        public int LowerBandwidth(DoubleMatrix2D A)
        {
            CheckSquare(A);
            double epsilon = Tolerance;
            int rows = A.Rows;

            for (int k = rows; --k >= 0;)
            {
                for (int i = rows - k; --i >= 0;)
                {
                    int j = i + k;
                    //if (A.getQuick(j,i) != 0) return k;
                    if (!(System.Math.Abs(A[j, i]) <= epsilon)) return k;
                }
            }
            return 0;
        }
        /// <summary>
        ///         Returns the<i>semi-bandwidth</i> of the given square matrix <i>A</i>.
        ///         A<i> banded</i> matrix has a "band" about the diagonal.
        ///         It is a matrix with all cells equal to zero,
        ///         with the possible exception of the cells along the diagonal line,
        ///         the<i> k</i> diagonal lines above the diagonal, and the <i>k</i> diagonal lines below the diagonal.
        /// The<i> semi-bandwith l</i> is the number <i>k+1</i>.
        /// The<i> bandwidth p</i> is the number <i>2*k + 1</i>.
        /// For example, a tridiagonal matrix corresponds to<i> k = 1, l= 2, p= 3 </ i >,
        ///         a diagonal or zero matrix corresponds to<i> k = 0, l= 1, p= 1 </ i >,
        ///         <p>
        /// The<i> upper bandwidth</i> is the maximum <i>j-i</i> for which<i> A[i, j]</i> is nonzero and <i>j &gt; i</i>.
        ///  The<i> lower bandwidth</i> is the maximum<i>i-j</i> for which<i> A[i, j]</i> is nonzero and <i>i &gt; j</i> d
        /// Diagonal, tridiagonal and triangular matrices are special cases.
        /// <p>
        /// Examples:
        /// <table border = "1" cellspacing="0">
        ///           <tr align = "left" valign="top"> 
        ///             <td valign = "middle" align="left"><i>matrix</i></td>
        ///             <td> <i>4&nbsp;x&nbsp;4&nbsp;<br>
        ///               0&nbsp;0&nbsp;0&nbsp;0<br>
        ///               0&nbsp;0&nbsp;0&nbsp;0<br>
        ///               0&nbsp;0&nbsp;0&nbsp;0<br>
        ///               0&nbsp;0&nbsp;0&nbsp;0 </i></td>
        ///             <td><i>4&nbsp;x&nbsp;4<br>
        ///               1&nbsp;0&nbsp;0&nbsp;0<br>
        ///               0&nbsp;0&nbsp;0&nbsp;0<br>
        ///               0&nbsp;0&nbsp;0&nbsp;0<br>
        ///               0&nbsp;0&nbsp;0&nbsp;1 </i></td>
        ///             <td><i>4&nbsp;x&nbsp;4<br>
        ///               1&nbsp;1&nbsp;0&nbsp;0<br>
        ///               1&nbsp;1&nbsp;1&nbsp;0<br>
        ///               0&nbsp;1&nbsp;1&nbsp;1<br>
        ///               0&nbsp;0&nbsp;1&nbsp;1 </i></td>
        ///             <td><i> 4&nbsp;x&nbsp;4<br>
        ///               0&nbsp;1&nbsp;1&nbsp;1<br>
        ///               0&nbsp;1&nbsp;1&nbsp;1<br>
        ///               0&nbsp;0&nbsp;0&nbsp;1<br>
        ///               0&nbsp;0&nbsp;0&nbsp;1 </i></td>
        ///             <td><i> 4&nbsp;x&nbsp;4<br>
        ///               0&nbsp;0&nbsp;0&nbsp;0<br>
        ///               1&nbsp;1&nbsp;0&nbsp;0<br>
        ///               1&nbsp;1&nbsp;0&nbsp;0<br>
        ///               1&nbsp;1&nbsp;1&nbsp;1 </i></td>
        ///             <td><i>4&nbsp;x&nbsp;4<br>
        ///               1&nbsp;1&nbsp;0&nbsp;0<br>
        ///               0&nbsp;1&nbsp;1&nbsp;0<br>
        ///               0&nbsp;1&nbsp;0&nbsp;1<br>
        ///               1&nbsp;0&nbsp;1&nbsp;1 </i><i> </i> </td>
        ///             <td><i>4&nbsp;x&nbsp;4<br>
        ///               1&nbsp;1&nbsp;1&nbsp;0<br>
        ///               0&nbsp;1&nbsp;0&nbsp;0<br>
        ///               1&nbsp;1&nbsp;0&nbsp;1<br>
        ///               0&nbsp;0&nbsp;1&nbsp;1 </i> </td>
        ///           </tr>
        ///           <tr align = "center" valign="middle"> 
        ///             <td><i>upperBandwidth</i></td>
        ///             <td> 
        ///               <div align = "center" >< i > 0 </ i ></ div >
        ///             </ td >
        ///             < td >
        ///               < div align="center"><i>0</i></div>
        ///             </td>
        ///             <td> 
        ///               <div align = "center" >< i > 1 </ i ></ div >
        ///             </ td >
        ///             < td >< i > 3 </ i ></ td >
        ///             < td align="center" valign="middle"><i>0</i></td>
        ///             <td align = "center" valign="middle"> 
        ///               <div align = "center" >< i > 1 </ i ></ div >
        ///             </ td >
        ///             < td align="center" valign="middle"> 
        ///               <div align = "center" >< i > 2 </ i ></ div >
        ///             </ td >
        ///           </ tr >
        ///           < tr align="center" valign="middle"> 
        ///             <td><i>lowerBandwidth</i></td>
        ///             <td> 
        ///               <div align = "center" >< i > 0 </ i ></ div >
        ///             </ td >
        ///             < td >
        ///               < div align="center"><i>0</i></div>
        ///             </td>
        ///             <td> 
        ///               <div align = "center" >< i > 1 </ i ></ div >
        ///             </ td >
        ///             < td >< i > 0 </ i ></ td >
        ///             < td align="center" valign="middle"><i>3</i></td>
        ///             <td align = "center" valign="middle"> 
        ///               <div align = "center" >< i > 3 </ i ></ div >
        ///             </ td >
        ///             < td align="center" valign="middle"> 
        ///               <div align = "center" >< i > 2 </ i ></ div >
        ///             </ td >
        ///           </ tr >
        ///           < tr align="center" valign="middle"> 
        ///             <td><i>semiBandwidth</i></td>
        ///             <td> 
        ///               <div align = "center" >< i > 1 </ i ></ div >
        ///             </ td >
        ///             < td >
        ///               < div align="center"><i>1</i></div>
        ///             </td>
        ///             <td> 
        ///               <div align = "center" >< i > 2 </ i ></ div >
        ///             </ td >
        ///             < td >< i > 4 </ i ></ td >
        ///             < td align="center" valign="middle"><i>4</i></td>
        ///             <td align = "center" valign="middle"> 
        ///               <div align = "center" >< i > 4 </ i ></ div >
        ///             </ td >
        ///             < td align="center" valign="middle"> 
        ///               <div align = "center" >< i > 3 </ i ></ div >
        ///             </ td >
        ///           </ tr >
        ///           < tr align="center" valign="middle"> 
        ///             <td><i>description</i></td>
        ///             <td> 
        ///               <div align = "center" >< i > zero </ i ></ div >
        ///             </ td >
        ///             < td >
        ///               < div align="center"><i>diagonal</i></div>
        ///             </td>
        ///             <td> 
        ///               <div align = "center" >< i > tridiagonal </ i ></ div >
        ///             </ td >
        ///             < td >< i > upper triangular</i></td>
        ///             <td align = "center" valign="middle"><i>lower triangular</i></td>
        ///             <td align = "center" valign= "middle" >
        ///                 < div align= "center" >< i > unstructured </ i ></ div >
        ///               </ td >
        ///               < td align= "center" valign= "middle" >
        ///                 < div align= "center" >< i > unstructured </ i ></ div >
        ///               </ td >
        ///             </ tr >
        ///   </ table >
        /// <summary>
        ///<param name= "A" > the square matrix to analyze.</param>
        ///<returns>the semi-bandwith<i> l</i>.</returns>
        ///<exception cref = "ArgumentException" >if <i>!isSquare(A)</i>. </exception>
        ///<see cref = "#lowerBandwidth(DoubleMatrix2D)" ></see>
        ///<see cref= "#upperBandwidth(DoubleMatrix2D)" ></see>
        public int SemiBandwidth(DoubleMatrix2D A)
        {
            CheckSquare(A);
            double epsilon = Tolerance;
            int rows = A.Rows;

            for (int k = rows; --k >= 0;)
            {
                for (int i = rows - k; --i >= 0;)
                {
                    int j = i + k;
                    //if (A.getQuick(j,i) != 0) return k+1;
                    //if (A.getQuick(i,j) != 0) return k+1;
                    if (!(System.Math.Abs(A[j, i]) <= epsilon)) return k + 1;
                    if (!(System.Math.Abs(A[i, j]) <= epsilon)) return k + 1;
                }
            }
            return 1;
        }
        /// <summary>
        /// Sets the tolerance to <i>System.Math.Abs(newTolerance)</i>.
        /// <summary>
        /// <exception cref="NotSupportedException">if <i>this==DEFAULT || this==ZERO || this==TWELVE</i>. </exception>
        public void SetTolerance(double newTolerance)
        {
            if (this == DEFAULT || this == ZERO || this == TWELVE)
            {
                throw new ArgumentException("Attempted to modify immutable object.");
                //throw new NotSupportedException("Attempted to modify object."); // since JDK1.2
            }
            Tolerance = System.Math.Abs(newTolerance);
        }

        /// <summary>
        /// Returns summary information about the given matrix<i> A</i>.
        /// That is a String with (propertyName, propertyValue) pairs.
        /// Useful for debugging or to quickly get the rough picture of a matrix.
        ///         For example,
        ///         <pre>
        /// density                      : 0.9
        /// isDiagonal                   : false
        /// isDiagonallyDominantByRow    : false
        /// isDiagonallyDominantByColumn : false
        /// isIdentity                   : false
        /// isLowerBidiagonal            : false
        /// isLowerTriangular            : false
        /// isNonNegative                : true
        /// isOrthogonal                 : Illegal operation or error: Matrix must be square.
        /// isPositive                   : true
        /// isSingular                   : Illegal operation or error: Matrix must be square.
        /// isSkewSymmetric              : Illegal operation or error: Matrix must be square.
        /// isSquare                     : false
        /// isStrictlyLowerTriangular    : false
        /// isStrictlyTriangular         : false
        /// isStrictlyUpperTriangular    : false
        /// isSymmetric                  : Illegal operation or error: Matrix must be square.
        /// isTriangular                 : false
        /// isTridiagonal                : false
        /// isUnitTriangular             : false
        /// isUpperBidiagonal            : false
        /// isUpperTriangular            : false
        /// isZero                       : false
        /// lowerBandwidth               : Illegal operation or error: Matrix must be square.
        /// semiBandwidth                : Illegal operation or error: Matrix must be square.
        /// upperBandwidth               : Illegal operation or error: Matrix must be square.
        /// </pre>
        /// <summary>
        public String ToString(DoubleMatrix2D A)
        {
            var names = new List<Object>();
            var values = new List<Object>();
            String unknown = "Illegal operation or error: ";

            // determine properties
            names.Add("density");
            try { values.Add(Density(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            // determine properties
            names.Add("isDiagonal");
            try { values.Add(IsDiagonal(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            // determine properties
            names.Add("isDiagonallyDominantByRow");
            try { values.Add(IsDiagonallyDominantByRow(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            // determine properties
            names.Add("isDiagonallyDominantByColumn");
            try { values.Add(IsDiagonallyDominantByColumn(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isIdentity");
            try { values.Add(IsIdentity(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isLowerBidiagonal");
            try { values.Add(IsLowerBidiagonal(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isLowerTriangular");
            try { values.Add(IsLowerTriangular(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isNonNegative");
            try { values.Add(IsNonNegative(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isOrthogonal");
            try { values.Add(IsOrthogonal(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isPositive");
            try { values.Add(IsPositive(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isSingular");
            try { values.Add(IsSingular(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isSkewSymmetric");
            try { values.Add(IsSkewSymmetric(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isSquare");
            try { values.Add(IsSquare(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isStrictlyLowerTriangular");
            try { values.Add(IsStrictlyLowerTriangular(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isStrictlyTriangular");
            try { values.Add(IsStrictlyTriangular(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isStrictlyUpperTriangular");
            try { values.Add(IsStrictlyUpperTriangular(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isSymmetric");
            try { values.Add(IsSymmetric(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isTriangular");
            try { values.Add(IsTriangular(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isTridiagonal");
            try { values.Add(IsTridiagonal(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isUnitTriangular");
            try { values.Add(IsUnitTriangular(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isUpperBidiagonal");
            try { values.Add(IsUpperBidiagonal(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isUpperTriangular");
            try { values.Add(IsUpperTriangular(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("isZero");
            try { values.Add(IsZero(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("lowerBandwidth");
            try { values.Add(LowerBandwidth(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("semiBandwidth");
            try { values.Add(SemiBandwidth(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("upperBandwidth");
            try { values.Add(UpperBandwidth(A).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }


            // sort ascending by property name
            Cern.Colt.Function.IntComparator comp = new Cern.Colt.Function.IntComparator((a, b) =>
            {
                return names[a].ToString().CompareTo(names[b].ToString());
            }
                );

            Cern.Colt.Swapper swapper = new Cern.Colt.Swapper((a, b) =>
        {
            Object tmp;
            tmp = names[a]; names[a] = names[b]; names[b] = tmp;
            tmp = values[a]; values[a] = values[b]; values[b] = tmp;
        });

            Cern.Colt.GenericSorting.QuickSort(0, names.Count, comp, swapper);

            // determine padding for nice formatting
            int maxLength = 0;
            for (int i = 0; i < names.Count; i++) {
                int Length = ((String)names[i]).Length;
                maxLength = System.Math.Max(Length, maxLength);
            }

            // finally, format properties
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < names.Count; i++) {
                String name = ((String)names[i]);
                buf.Append(name);
                buf.Append(Blanks(maxLength - name.Length));
                buf.Append(" : ");
                buf.Append(values[i]);
                if (i < names.Count - 1)
                    buf.Append('\n');
            }

            return buf.ToString();
        }
        /// <summary>
        /// The<i> upper bandwidth</i> of a square matrix<i> A</i> is the
        /// maximum <i>j-i</i> for which<i> A[i, j]</i> is nonzero and <i>j &gt; i</i>.
        ///  A<i> banded</i> matrix has a "band" about the diagonald
        ///  Diagonal, tridiagonal and triangular matrices are special cases.
        /// <summary>
        /// <param name = "A" > the square matrix to analyze.</param>
        /// <returns>the upper bandwith.</returns>
        /// <exception cref = "ArgumentException" >if <i>!isSquare(A)</i>. </exception>
        /// <see cref = "#semiBandwidth(DoubleMatrix2D)" ></see>
        /// <see cref= "#lowerBandwidth(DoubleMatrix2D)" ></see>
        public int UpperBandwidth(DoubleMatrix2D A)
        {
            CheckSquare(A);
            double epsilon = Tolerance;
            int rows = A.Rows;

            for (int k = rows; --k >= 0;)
            {
                for (int i = rows - k; --i >= 0;)
                {
                    int j = i + k;
                    //if (A.getQuick(i,j) != 0) return k;
                    if (!(System.Math.Abs(A[i, j]) <= epsilon)) return k;
                }
            }
            return 0;
        }
    }
}
