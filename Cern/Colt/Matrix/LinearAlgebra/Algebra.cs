namespace Cern.Colt.Matrix.LinearAlgebra
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cern.Colt.Function;

    /// <summary>
    /// Linear algebraic matrix operations operating on {@link DoubleMatrix2D}; concentrates most functionality of this package.
    /// </summary>
    public static class Algebra
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Internal Methods

        #endregion

        #region Local Public Methods

        /// <summary>
        /// Returns sqrt(a^2 + b^2) without under/overflow.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// Sqrt(a^2 + b^2) without under/overflow.
        /// </returns>
        public static double Hypot(double a, double b)
        {
            double r;
            if (System.Math.Abs(a) > System.Math.Abs(b))
            {
                r = b / a;
                r = System.Math.Abs(a) * System.Math.Sqrt(1 + (r * r));
            }
            else if (b != 0)
            {
                r = a / b;
                r = System.Math.Abs(b) * System.Math.Sqrt(1 + (r * r));
            }
            else
            {
                r = 0.0;
            }

            return r;
        }

        /// <summary>
        /// Returns the condition of matrix <i>A</i>, which is the ratio of largest to smallest singular value.
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static double Cond(DoubleMatrix2D A)
        {
            return Svd(A).Cond();
        }

        /// <summary>
        /// Returns the determinant of matrix <i>A</i>.
        /// </summary>
        /// <param name="A"></param>
        /// <returns>a determinant</returns>
        public static double Det(DoubleMatrix2D A)
        {
            return Lu(A).Det();
        }

        /// <summary>
        /// Constructs and returns the Eigenvalue-decomposition of the given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static EigenvalueDecomposition Eig(DoubleMatrix2D matrix)
        {
            return new EigenvalueDecomposition(matrix);
        }

        public static DoubleMatrix2D Inverse(DoubleMatrix2D A)
        {
            if (A.IsSquare && A.IsDiagonal)
            {
                DoubleMatrix2D inv = A.Copy();
                Boolean IsNonSingular = Diagonal.Inverse(inv);
                if (!IsNonSingular) throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_MatrixIsSingular);
                return inv;
            }
            return Solve(A, DoubleFactory2D.Dense.Identity(A.Rows));
        }

        /// <summary>
        /// Inner product of two vectors; <i>Sum(x[i] * y[i])</i>.
        /// </summary>
        /// <param name="x">The first source vector.</param>
        /// <param name="y">The second source vector.</param>
        /// <returns>The inner product.</returns>
        public static double Mult(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            return x.ZDotProduct(y);
        }

        /// <summary>
        /// Linear algebraic matrix-vector multiplication; <i>z = A * y</i>.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// <i>z</i>; a new vector with <i>z.Size==A.Rows</i>.
        /// </returns>
        public static DoubleMatrix1D Mult(DoubleMatrix2D a, DoubleMatrix1D y)
        {
            return a.ZMult(y, null);
        }

        /// <summary>
        /// Linear algebraic matrix-matrix multiplication; <i>C = A x B</i>.
        /// </summary>
        /// <param name="a">
        /// The first source matrix.
        /// </param>
        /// <param name="b">
        /// The second source matrix.
        /// </param>
        /// <returns>
        /// <i>C</i>; a new matrix holding the results, with <i>C.Rows=A.Rows, C.Columns==B.Columns</i>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <i>B.Rows != A.Columns</i>.
        /// </exception>
        public static DoubleMatrix2D Mult(DoubleMatrix2D a, DoubleMatrix2D b)
        {
            return a.ZMult(b, null);
        }

        /// <summary>
        /// Outer product of two vectors; Sets <i>A[i,j] = x[i] * y[j]</i>.
        /// </summary>
        /// <param name="x">the first source vector.</param>
        /// <param name="y">the second source vector.</param>
        /// <param name="A">the matrix to hold the resultsd Set this parameter to <i>null</i> to indicate that a new result matrix shall be constructed.</param>
        /// <returns>A (for convenience only).</returns>
        public static DoubleMatrix2D MultOuter(DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix2D A)
        {
            int rows = x.Size;
            int columns = y.Size;
            if (A == null) A = x.Like2D(rows, columns);
            if (A.Rows != rows || A.Columns != columns) throw new ArgumentException();

            for (int row = rows; --row >= 0;) A.ViewRow(row).Assign(y);

            for (int column = columns; --column >= 0;) A.ViewColumn(column).Assign(x, BinaryFunctions.Mult);
            return A;
        }

        /// <summary>
        /// Returns the one-norm of vector <i>x</i>, which is <i>Sum(abs(x[i]))</i>.
        /// </summary>
        /// <param name="x">
        /// The vector x.
        /// </param>
        /// <returns>
        /// The one-norm of x.
        /// </returns>
        public static double Norm1(DoubleMatrix1D x)
        {
            if (x.Size == 0) return 0;
            return x.Aggregate(BinaryFunctions.Plus, Math.Abs);
        }

        /// <summary>
        /// Returns the one-norm of matrix <i>A</i>, which is the maximum absolute column sum.
        /// </summary>
        /// <param name="A">The matrix A</param>
        /// <returns>The one-norm of A.</returns>
        public static double Norm1(DoubleMatrix2D A)
        {
            double max = 0;
            for (int column = A.Columns; --column >= 0;)
            {
                max = System.Math.Max(max, Norm1(A.ViewColumn(column)));
            }
            return max;
        }

        /// <summary>
        /// Returns the two-norm (aka <i>euclidean norm</i>) of vector <i>x</i>; equivalent to <i>mult(x,x)</i>.
        /// </summary>
        /// <param name="x">The vector x.</param>
        /// <returns>The two-norm of A.</returns>
        public static double Norm2(DoubleMatrix1D x)
        {
            return Mult(x, x);
        }

        /// <summary>
        /// Returns the two-norm of matrix <i>A</i>, which is the maximum singular value; obtained from SVD.
        /// </summary>
        /// <param name="a">The matrix A.</param>
        /// <returns>The two-norm of A.</returns>
        public static double Norm2(DoubleMatrix2D a)
        {
            return Svd(a).Norm2;
        }

        /// <summary>
        /// Returns the Frobenius norm of matrix <i>A</i>, which is <i>Sqrt(Sum(A[i,j]<sup>2</sup>))</i>.
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static double NormF(DoubleMatrix2D A)
        {
            if (A.Size == 0) return 0;
            return A.Aggregate(HypotFunction(), Cern.Jet.Math.Functions.DoubleFunctions.Identity);
        }

        /// <summary>
        /// Returns the infinity norm of vector <i>x</i>, which is <i>Max(abs(x[i]))</i>.
        /// </summary>
        /// <param name="x">The vector x.</param>
        /// <returns>The infinity norm of matrix <i>A</i></returns>
        public static double NormInfinity(DoubleMatrix1D x)
        {
            // fix for bug reported by T.J.Hunt@open.ac.uk
            if (x.Size == 0) return 0;
            return x.Aggregate(Math.Max, Math.Abs);
            //	if (x.Size==0) return 0;
            //	return x.aggregate(cern.Cern.Jet.math.Functions.plus,cern.Cern.Jet.math.Functions.abs);
            //	double max = 0;
            //	for (int i = x.Size; --i >= 0; ) {
            //		max = System.Math.Max(max, x.getQuick(i));
            //	}
            //	return max;
        }

        /// <summary>
        /// Returns the infinity norm of matrix <i>A</i>, which is the maximum absolute row sum.
        /// </summary>
        /// <param name="A">The matrix A.</param>
        /// <returns>The infinity norm of matrix <i>A</i></returns>
        public static double NormInfinity(DoubleMatrix2D A)
        {
            double max = 0;
            for (int row = A.Rows; --row >= 0;)
            {
                //max = System.Math.Max(max, normInfinity(A.ViewRow(row)));
                max = System.Math.Max(max, Norm1(A.ViewRow(row)));
            }
            return max;
        }

        /// <summary>
        /// Returns sqrt(a^2 + b^2) without under/overflow.
        /// </summary>
        /// <returns></returns>
        public static Cern.Colt.Function.DoubleDoubleFunction HypotFunction()
        {
            return new Cern.Colt.Function.DoubleDoubleFunction((a, b) =>
            {
                return Hypot(a, b);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="indexes"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static DoubleMatrix1D Permute(DoubleMatrix1D A, int[] indexes, double[] work)
        {
            // check validity
            int size = A.Size;
            if (indexes.Length != size) throw new IndexOutOfRangeException(Cern.LocalizedResources.Instance().Exception_InvalidPermutation);

            /*
            int i=size;
            int a;
            while (--i >= 0 && (a=indexes[i])==i) if (a < 0 || a >= size) throw new IndexOutOfRangeException(Cern.LocalizedResources.Instance().Exception_InvalidPermutation);
            if (i<0) return; // nothing to permute
            */

            if (work == null || size > work.Length)
            {
                work = A.ToArray();
            }
            else
            {
                A.ToArray(ref work);
            }
            for (int i = size; --i >= 0;) A[i] = work[indexes[i]];
            return A;
        }

        public static DoubleMatrix2D Permute(DoubleMatrix2D A, int[] rowIndexes, int[] columnIndexes)
        {
            return A.ViewSelection(rowIndexes, columnIndexes);
        }

        public static DoubleMatrix2D PermuteColumns(DoubleMatrix2D A, int[] indexes, int[] work)
        {
            return PermuteRows(A.ViewDice(), indexes, work);
        }

        public static DoubleMatrix2D PermuteRows(DoubleMatrix2D A, int[] indexes, int[] work)
        {
            // check validity
            int size = A.Rows;
            if (indexes.Length != size) throw new IndexOutOfRangeException(Cern.LocalizedResources.Instance().Exception_InvalidPermutation);

            /*
            int i=size;
            int a;
            while (--i >= 0 && (a=indexes[i])==i) if (a < 0 || a >= size) throw new IndexOutOfRangeException(Cern.LocalizedResources.Instance().Exception_InvalidPermutation);
            if (i<0) return; // nothing to permute
            */

            int columns = A.Columns;
            if (columns < size / 10)
            { // quicker
                double[] doubleWork = new double[size];
                for (int j = A.Columns; --j >= 0;) Permute(A.ViewColumn(j), indexes, doubleWork);
                return A;
            }

            var swapper = new Cern.Colt.Swapper((a, b) =>
        {
            A.ViewRow(a).Swap(A.ViewRow(b));
        }
    );

            Cern.Colt.GenericPermuting.Permute(indexes, swapper, work, null);
            return A;
        }

        /// <summary>
        /// Linear algebraic matrix power; <i>B = A<sup>k</sup> &lt;==> B = A*A*...*A</i>.
        /// <ul>
        /// <li><i>p &gt;= 1: B = A*A*...*A</i>.</li>
        /// <li><i>p == 0: B = identity matrix</i>.</li>
        /// <li><i>p &lt;  0: B = pow(inverse(A),-p)</i>.</li>
        /// </ul>
        /// Implementation: Based on logarithms of 2, memory usage minimized.
        /// </summary>
        /// <param name="A">the source matrix; must be square; stays unaffected by this operation.</param>
        /// <param name="p">the exponent, can be any number.</param>
        /// <returns><i>B</i>, a newly constructed result matrix; storage-independent of <i>A</i>.</returns>
        ///<exception cref="ArgumentException">if <i>!property().isSquare(A)</i>.</exception>
        public static DoubleMatrix2D Pow(DoubleMatrix2D A, int p)
        {
            // matrix multiplication based on log2 method: A*A*....*A is slow, ((A * A)^2)^2 * ..D is faster
            // allocates two auxiliary matrices as work space

            IBlas blas = SmpBlas.smpBlas; // for parallel matrix mult; if not initialized defaults to sequential blas
            Property.DEFAULT.CheckSquare(A);
            if (p < 0)
            {
                A = Inverse(A);
                p = -p;
            }
            if (p == 0) return DoubleFactory2D.Dense.Identity(A.Rows);
            DoubleMatrix2D T = A.Like(); // temporary
            if (p == 1) return T.Assign(A);  // safes one auxiliary matrix allocation
            if (p == 2)
            {
                blas.Dgemm(false, false, 1, A, A, 0, T); // mult(A,A); // safes one auxiliary matrix allocation
                return T;
            }

            int k = Cern.Colt.Bitvector.QuickBitVector.MostSignificantBit(p); // index of highest bit in state "true"

            /*
            this is the naive version:
            DoubleMatrix2D B = A.Copy();
            for (int i=0; i<p-1; i++) {
                B = mult(B,A);
            }
            return B;
            */

            // here comes the optimized version:
            //cern.colt.Timer timer = new cern.colt.Timer().start();

            int i = 0;
            while (i <= k && (p & (1 << i)) == 0)
            { // while (bit i of p == false)
              // A = mult(A,A); would allocate a lot of temporary memory
                blas.Dgemm(false, false, 1, A, A, 0, T); // A.zMult(A,T);
                DoubleMatrix2D swap = A; A = T; T = swap; // swap A with T
                i++;
            }

            DoubleMatrix2D B = A.Copy();
            i++;
            for (; i <= k; i++)
            {
                // A = mult(A,A); would allocate a lot of temporary memory
                blas.Dgemm(false, false, 1, A, A, 0, T); // A.zMult(A,T);	
                DoubleMatrix2D swap = A; A = T; T = swap; // swap A with T

                if ((p & (1 << i)) != 0)
                { // if (bit i of p == true)
                  // B = mult(B,A); would allocate a lot of temporary memory
                    blas.Dgemm(false, false, 1, B, A, 0, T); // B.zMult(A,T);		
                    swap = B; B = T; T = swap; // swap B with T
                }
            }
            //timer.stop().Display();
            return B;
        }

        /// <summary>
        /// Returns the effective numerical rank of matrix <i>A</i>, obtained from Singular Value Decomposition.
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static int Rank(DoubleMatrix2D A)
        {
            return Svd(A).Rank;
        }

        public static DoubleMatrix2D Solve(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            LUDecomposition lu = new LUDecomposition(A);
            QRDecomposition qr = new QRDecomposition(B);

            return (A.Rows == A.Columns ? (lu.Solve(B)) : (qr.Solve(B)));
        }

        /// <summary>
        /// Solves X*A = B, which is also A'*X' = B'.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>X; a new independent matrix; solution if A is square, least squares solution otherwise.</returns>
        public static DoubleMatrix2D SolveTranspose(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return Solve(Transpose(A), Transpose(B));
        }

        /// <summary>
        /// Copies the columns of the indicated rows into a new sub matrix.
        /// <i>sub[0..rowIndexes.Length-1,0..columnTo-columnFrom] = A[rowIndexes(:),columnFrom..columnTo]</i>;
        /// The returned matrix is <i>not backed</i> by this matrix, so changes in the returned matrix are <i>not reflected</i> in this matrix, and vice-versa.
        /// </summary>
        /// <param name="A">the source matrix to copy from.</param>
        /// <param name="rowIndexes">the indexes of the rows to copyd May be unsorted.</param>
        /// <param name="columnFrom">the index of the first column to copy (inclusive).</param>
        /// <param name="columnTo">the index of the last column to copy (inclusive).</param>
        /// <returns>a new sub matrix; with <i>sub.Rows==rowIndexes.Length; sub.Columns==columnTo-columnFrom+1</i>.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>columnFrom &lt; 0 || columnTo-columnFrom+1 &lt; 0 || columnTo+1>matrix.Columns || for any row=rowIndexes[i]: row  &lt;  0 || row >= matrix.Rows</i>.</exception>
        private static DoubleMatrix2D SubMatrix(DoubleMatrix2D A, int[] rowIndexes, int columnFrom, int columnTo)
        {
            int width = columnTo - columnFrom + 1;
            int rows = A.Rows;
            A = A.ViewPart(0, columnFrom, rows, width);
            DoubleMatrix2D sub = A.Like(rowIndexes.Length, width);

            for (int r = rowIndexes.Length; --r >= 0;)
            {
                int row = rowIndexes[r];
                if (row < 0 || row >= rows)
                    throw new IndexOutOfRangeException(Cern.LocalizedResources.Instance().Exception_IllegalIndex);
                sub.ViewRow(r).Assign(A.ViewRow(row));
            }
            return sub;
        }

        /// <summary>
        /// Copies the rows of the indicated columns into a new sub matrix.
        /// <i>sub[0..rowTo-rowFrom,0..columnIndexes.Length-1] = A[rowFrom..rowTo,columnIndexes(:)]</i>;
        /// The returned matrix is <i>not backed</i> by this matrix, so changes in the returned matrix are <i>not reflected</i> in this matrix, and vice-versa.
        /// </summary>
        /// <param name="A">the source matrix to copy from.</param>
        /// <param name="rowFrom">the index of the first row to copy (inclusive).</param>
        /// <param name="rowTo">the index of the last row to copy (inclusive).</param>
        /// <param name="columnIndexes">the indexes of the columns to copyd May be unsorted.</param>
        /// <returns>a new sub matrix; with <i>sub.Rows==rowTo-rowFrom+1; sub.Columns==columnIndexes.Length</i>.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>rowFrom &lt; 0 || rowTo-rowFrom + 1 &lt; 0 || rowTo + 1 > matrix.Rows || for any col = columnIndexes[i]: col &lt; 0 || col >= matrix.Columns</i>.</exception>
        private static DoubleMatrix2D SubMatrix(DoubleMatrix2D A, int rowFrom, int rowTo, int[] columnIndexes)
        {
            if (rowTo - rowFrom >= A.Rows) throw new IndexOutOfRangeException(Cern.LocalizedResources.Instance().Exception_TooManyRows);
            int height = rowTo - rowFrom + 1;
            int columns = A.Columns;
            A = A.ViewPart(rowFrom, 0, height, columns);
            DoubleMatrix2D sub = A.Like(height, columnIndexes.Length);

            for (int c = columnIndexes.Length; --c >= 0;)
            {
                int column = columnIndexes[c];
                if (column < 0 || column >= columns)
                    throw new IndexOutOfRangeException(Cern.LocalizedResources.Instance().Exception_IllegalIndex);
                sub.ViewColumn(c).Assign(A.ViewColumn(column));
            }
            return sub;
        }

        /// <summary>
        /// Constructs and returns a new <i>sub-range view</i> which is the sub matrix <i>A[fromRow..toRow,fromColumn..toColumn]</i>.
        /// The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa.
        /// Use idioms like <i>result = subMatrix(..D).Copy()</i> to generate an independent sub matrix.
        /// </summary>
        /// <param name="A">the source matrix.</param>
        /// <param name="fromRow">The index of the first row (inclusive).</param>
        /// <param name="toRow">The index of the last row (inclusive).</param>
        /// <param name="fromColumn">The index of the first column (inclusive).</param>
        /// <param name="toColumn">The index of the last column (inclusive).</param>
        /// <returns>a new sub-range view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>fromColumn &lt; 0 || toColumn - fromColumn + 1 &lt; 0 || toColumn >= A.Columns || fromRow &lt; 0 || toRow - fromRow + 1 &lt; 0 || toRow >= A.Rows</i></exception>
        public static DoubleMatrix2D SubMatrix(DoubleMatrix2D A, int fromRow, int toRow, int fromColumn, int toColumn)
        {
            return A.ViewPart(fromRow, fromColumn, toRow - fromRow + 1, toColumn - fromColumn + 1);
        }

        /// <summary>
        /// Returns a String with (propertyName, propertyValue) pairs.
        /// Useful for debugging or to quickly get the rough picture.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        /// <example>
        /// Cond          : 14.073264490042144
        /// Det           : Illegal operation or error: Matrix must be square.
        /// Norm1         : 0.9620244354009628
        /// Norm2         : 3.0
        /// NormF         : 1.304841791648992
        /// NormInfinity  : 1.5406551198102534
        /// Rank          : 3
        /// Trace         : 0
        /// </example>
        public static String ToString(DoubleMatrix2D matrix)
        {
            List<Object> names = new List<Object>();
            List<Object> values = new List<Object>();
            String unknown = "Illegal operation or error: ";

            // determine properties
            names.Add("cond");
            try { values.Add(Cond(matrix).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("det");
            try { values.Add(Det(matrix).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("norm1");
            try { values.Add(Norm1(matrix).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("norm2");
            try { values.Add(Norm2(matrix).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("normF");
            try { values.Add(NormF(matrix).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("normInfinity");
            try { values.Add(NormInfinity(matrix).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("rank");
            try { values.Add(Rank(matrix).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }

            names.Add("trace");
            try { values.Add(Trace(matrix).ToString()); }
            catch (ArgumentException exc) { values.Add(unknown + exc.Message); }


            // sort ascending by property name
            Cern.Colt.Function.IntComparator comp = new Cern.Colt.Function.IntComparator((a, b) =>
            {
                return names[a].ToString().CompareTo(names[b]);
            }
                );
            {

                Cern.Colt.Swapper swapper = new Cern.Colt.Swapper((a, b) =>
                {
                    Object tmp;
                    tmp = names[a]; names[a] = names[b]; names[b] = tmp;
                    tmp = values[a]; values[a] = values[b]; values[b] = tmp;
                }
                );
                Cern.Colt.GenericSorting.QuickSort(0, names.Count, comp, swapper);

                // determine padding for nice formatting
                int maxLength = 0;
                for (int i = 0; i < names.Count; i++)
                {
                    int Length = ((String)names[i]).Length;
                    maxLength = System.Math.Max(Length, maxLength);
                }

                // finally, format properties
                StringBuilder buf = new StringBuilder();
                for (int i = 0; i < names.Count; i++)
                {
                    String name = ((String)names[i]);
                    buf.Append(name);
                    buf.Append(Property.Blanks(maxLength - name.Length));
                    buf.Append(" : ");
                    buf.Append(values[i]);
                    if (i < names.Count - 1)
                        buf.Append('\n');
                }

                return buf.ToString();
            }
        }

        /// <summary>
        /// Returns the results of <i>ToString(A)</i> and additionally the results of all sorts of decompositions applied to the given matrix.
        /// Useful for debugging or to quickly get the rough picture.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        /// <example>
        /// <pre>
        /// A = 3 x 3 matrix
        /// 249  66  68
        /// 104 214 108
        /// 144 146 293
        ///
        /// cond         : 3.931600417472078
        /// det          : 9638870.0
        /// norm1        : 497.0
        /// norm2        : 473.34508217011404
        /// normF        : 516.873292016525
        /// normInfinity : 583.0
        /// rank         : 3
        /// trace        : 756.0
        ///
        /// density                      : 1.0
        /// isDiagonal                   : false
        /// isDiagonallyDominantByColumn : true
        /// isDiagonallyDominantByRow    : true
        /// isIdentity                   : false
        /// isLowerBidiagonal            : false
        /// isLowerTriangular            : false
        /// isNonNegative                : true
        /// isOrthogonal                 : false
        /// isPositive                   : true
        /// isSingular                   : false
        /// isSkewSymmetric              : false
        /// isSquare                     : true
        /// isStrictlyLowerTriangular    : false
        /// isStrictlyTriangular         : false
        /// isStrictlyUpperTriangular    : false
        /// isSymmetric                  : false
        /// isTriangular                 : false
        /// isTridiagonal                : false
        /// isUnitTriangular             : false
        /// isUpperBidiagonal            : false
        /// isUpperTriangular            : false
        /// isZero                       : false
        /// lowerBandwidth               : 2
        /// semiBandwidth                : 3
        /// upperBandwidth               : 2
        ///
        /// -----------------------------------------------------------------------------
        /// LUDecompositionQuick(A) --> isNonSingular(A), det(A), pivot, L, U, inverse(A)
        /// -----------------------------------------------------------------------------
        /// isNonSingular = true
        /// det = 9638870.0
        /// pivot = [0, 1, 2]
        ///
        /// L = 3 x 3 matrix
        /// 1        0       0
        /// 0.417671 1       0
        /// 0.578313 0.57839 1
        ///
        /// U = 3 x 3 matrix
        /// 249  66         68       
        ///   0 186.433735  79.598394
        ///   0   0        207.635819
        ///
        /// inverse(A) = 3 x 3 matrix
        ///  0.004869 -0.000976 -0.00077 
        /// -0.001548  0.006553 -0.002056
        /// -0.001622 -0.002786  0.004816
        ///
        /// -----------------------------------------------------------------
        /// QRDecomposition(A) --> hasFullRank(A), H, Q, R, pseudo inverse(A)
        /// -----------------------------------------------------------------
        /// hasFullRank = true
        ///
        /// H = 3 x 3 matrix
        /// 1.814086 0        0
        /// 0.34002  1.903675 0
        /// 0.470797 0.428218 2
        ///
        /// Q = 3 x 3 matrix
        /// -0.814086  0.508871  0.279845
        /// -0.34002  -0.808296  0.48067 
        /// -0.470797 -0.296154 -0.831049
        ///
        /// R = 3 x 3 matrix
        /// -305.864349 -195.230337 -230.023539
        ///    0        -182.628353  467.703164
        ///    0           0        -309.13388 
        ///
        /// pseudo inverse(A) = 3 x 3 matrix
        ///  0.006601  0.001998 -0.005912
        /// -0.005105  0.000444  0.008506
        /// -0.000905 -0.001555  0.002688
        ///
        /// --------------------------------------------------------------------------
        /// CholeskyDecomposition(A) --> isSymmetricPositiveDefinite(A), L, inverse(A)
        /// --------------------------------------------------------------------------
        /// isSymmetricPositiveDefinite = false
        ///
        /// L = 3 x 3 matrix
        /// 15.779734  0         0       
        ///  6.590732 13.059948  0       
        ///  9.125629  6.573948 12.903724
        ///
        /// inverse(A) = Illegal operation or error: Matrix is not symmetric positive definite.
        ///
        /// ---------------------------------------------------------------------
        /// EigenvalueDecomposition(A) --> D, V, realEigenvalues, imagEigenvalues
        /// ---------------------------------------------------------------------
        /// realEigenvalues = 1 x 3 matrix
        /// 462.796507 172.382058 120.821435
        /// imagEigenvalues = 1 x 3 matrix
        /// 0 0 0
        ///
        /// D = 3 x 3 matrix
        /// 462.796507   0          0       
        ///   0        172.382058   0       
        ///   0          0        120.821435
        ///
        /// V = 3 x 3 matrix
        /// -0.398877 -0.778282  0.094294
        /// -0.500327  0.217793 -0.806319
        /// -0.768485  0.66553   0.604862
        ///
        /// ---------------------------------------------------------------------
        /// SingularValueDecomposition(A) --> cond(A), rank(A), norm2(A), U, S, V
        /// ---------------------------------------------------------------------
        /// cond = 3.931600417472078
        /// rank = 3
        /// norm2 = 473.34508217011404
        ///
        /// U = 3 x 3 matrix
        /// 0.46657  -0.877519  0.110777
        /// 0.50486   0.161382 -0.847982
        /// 0.726243  0.45157   0.51832 
        ///
        /// S = 3 x 3 matrix
        /// 473.345082   0          0       
        ///   0        169.137441   0       
        ///   0          0        120.395013
        ///
        /// V = 3 x 3 matrix
        /// 0.577296 -0.808174  0.116546
        /// 0.517308  0.251562 -0.817991
        /// 0.631761  0.532513  0.563301
        /// </pre>
        ///</example>
        public static String ToVerboseString(DoubleMatrix2D matrix)
        {
            /*
                StringBuilder buf = new StringBuilder();
                String unknown = "Illegal operation or error: ";
                String constructionException = "Illegal operation or error upon construction: ";

                buf.Append("------------------------------------------------------------------\n");
                buf.Append("LUDecomposition(A) --> isNonSingular, det, pivot, L, U, inverse(A)\n");
                buf.Append("------------------------------------------------------------------\n");
            */

            String constructionException = Cern.LocalizedResources.Instance().Exception_IllegalOperationOrErrorUponConstructionOf;
            StringBuilder buf = new StringBuilder();

            buf.Append("A = ");
            buf.Append(matrix);

            buf.Append("\n\n" + ToString(matrix));
            buf.Append("\n\n" + Property.DEFAULT.ToString(matrix));

            LUDecomposition lu = null;
            try { lu = new LUDecomposition(matrix); }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " LUDecomposition: " + exc.Message);
            }
            if (lu != null) buf.Append("\n\n" + lu.ToString());

            QRDecomposition qr = null;
            try { qr = new QRDecomposition(matrix); }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " QRDecomposition: " + exc.Message);
            }
            if (qr != null) buf.Append("\n\n" + qr.ToString());

            CholeskyDecomposition chol = null;
            try { chol = new CholeskyDecomposition(matrix); }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " CholeskyDecomposition: " + exc.Message);
            }
            if (chol != null) buf.Append("\n\n" + chol.ToString());

            EigenvalueDecomposition eig = null;
            try { eig = new EigenvalueDecomposition(matrix); }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " EigenvalueDecomposition: " + exc.Message);
            }
            if (eig != null) buf.Append("\n\n" + eig.ToString());

            SingularValueDecomposition svd = null;
            try { svd = new SingularValueDecomposition(matrix); }
            catch (ArgumentException exc)
            {
                buf.Append("\n\n" + constructionException + " SingularValueDecomposition: " + exc.Message);
            }
            if (svd != null) buf.Append("\n\n" + svd.ToString());

            return buf.ToString();
        }

        /// <summary>
        /// Returns the sum of the diagonal elements of matrix <i>A</i>; <i>Sum(A[i,i])</i>.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <returns>
        /// The sum of the diagonal elements of matrix <i>A</i>.
        /// </returns>
        public static double Trace(DoubleMatrix2D a)
        {
            double sum = 0;
            for (int i = System.Math.Min(a.Rows, a.Columns); --i >= 0;)
                sum += a[i, i];
            return sum;
        }

        /// <summary>
        /// Constructs and returns a new view which is the transposition of the given matrix <i>A</i>.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <returns>
        /// The transpose of A.
        /// </returns>
        public static DoubleMatrix2D Transpose(DoubleMatrix2D a)
        {
            return a.ViewDice();
        }

        /// <summary>
        /// Modifies the matrix to be a lower trapezoidal matrix.
        /// </summary>
        /// <param name="A"></param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D TrapezoidalLower(DoubleMatrix2D A)
        {
            int rows = A.Rows;
            int columns = A.Columns;
            for (int r = rows; --r >= 0;)
            {
                for (int c = columns; --c >= 0;)
                {
                    if (r < c) A[r, c] = 0;
                }
            }
            return A;
        }
        #endregion

        #region Local Private Methods
        /// <summary>
        /// Constructs and returns the cholesky-decomposition of the given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static CholeskyDecomposition Chol(DoubleMatrix2D matrix)
        {
            return new CholeskyDecomposition(matrix);
        }

        /// <summary>
        /// Constructs and returns the SingularValue-decomposition of the given matrix.
        /// </summary>
        /// <param name="matrix">
        /// The matrix A
        /// </param>
        /// <returns>
        /// The SVD of A.
        /// </returns>
        private static SingularValueDecomposition Svd(DoubleMatrix2D matrix)
        {
            return new SingularValueDecomposition(matrix);
        }

        /// <summary>
        /// Constructs and returns the LU-decomposition of the given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static LUDecomposition Lu(DoubleMatrix2D matrix)
        {
            return new LUDecomposition(matrix);
        }

        /// <summary>
        /// Constructs and returns the QR-decomposition of the given matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static QRDecomposition Qr(DoubleMatrix2D matrix)
        {
            return new QRDecomposition(matrix);
        }

        /// <summary>
        /// Outer product of two vectors; Returns a matrix with <i>A[i,j] = x[i] * y[j]</i>.
        /// </summary>
        /// <param name="x">the first source vector.</param>
        /// <param name="y">the second source vector.</param>
        /// <returns>the outer product </i>A</i>.</returns>
        private static DoubleMatrix2D XMultOuter(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            DoubleMatrix2D A = x.Like2D(x.Size, y.Size);
            MultOuter(x, y, A);
            return A;
        }

        /// <summary>
        /// Linear algebraic matrix power; <i>B = A<sup>k</sup> &lt;==> B = A*A*...*A</i>.
        /// </summary>
        /// <param name="A">the source matrix; must be square.</param>
        /// <param name="k">the exponent, can be any number.</param>
        /// <returns>a new result matrix.</returns>
        /// <exception cref="ArgumentException">if <i>!Testing.isSquare(A)</i>.</exception>
        private static DoubleMatrix2D XPowSlow(DoubleMatrix2D A, int k)
        {
            //cern.colt.Timer timer = new cern.colt.Timer().start();
            DoubleMatrix2D result = A.Copy();
            for (int i = 0; i < k - 1; i++)
            {
                result = Mult(result, A);
            }
            //timer.stop().Display();
            return result;
        }
        #endregion
    }
}
