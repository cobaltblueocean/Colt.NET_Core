namespace Cern.Colt.Matrix.LinearAlgebra
{
    using System;
    using Cern.Colt.Function;

    /// <summary>
    /// Linear algebraic matrix operations operating on {@link DoubleMatrix2D}; concentrates most functionality of this package.
    /// </summary>
    public static class Algebra
    {
        public static DoubleMatrix2D Inverse(DoubleMatrix2D A)
        {
            if (A.IsSquare && A.IsDiagonal)
            {
                DoubleMatrix2D inv = A.Copy();
                Boolean isNonSingular = Diagonal.Inverse(inv);
                if (!isNonSingular) throw new ArgumentException("A is singular.");
                return inv;
            }
            return Solve(A, DoubleFactory2D.Dense.Identity(A.Rows));
        }

        /// <summary>
        /// Inner product of two vectors; <tt>Sum(x[i] * y[i])</tt>.
        /// </summary>
        /// <param name="x">The first source vector.</param>
        /// <param name="y">The second source vector.</param>
        /// <returns>The inner product.</returns>
        public static double Mult(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            return x.ZDotProduct(y);
        }

        /// <summary>
        /// Linear algebraic matrix-vector multiplication; <tt>z = A * y</tt>.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// </param>
        /// <returns>
        /// <tt>z</tt>; a new vector with <tt>z.Size==A.rows()</tt>.
        /// </returns>
        public static DoubleMatrix1D Mult(DoubleMatrix2D a, DoubleMatrix1D y)
        {
            return a.ZMult(y, null);
        }

        /// <summary>
        /// Linear algebraic matrix-matrix multiplication; <tt>C = A x B</tt>.
        /// </summary>
        /// <param name="a">
        /// The first source matrix.
        /// </param>
        /// <param name="b">
        /// The second source matrix.
        /// </param>
        /// <returns>
        /// <tt>C</tt>; a new matrix holding the results, with <tt>C.rows()=A.rows(), C.columns()==B.columns()</tt>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <tt>B.rows() != A.columns()</tt>.
        /// </exception>
        public static DoubleMatrix2D Mult(DoubleMatrix2D a, DoubleMatrix2D b)
        {
            return a.ZMult(b, null);
        }

        /// <summary>
        /// Outer product of two vectors; Sets <tt>A[i,j] = x[i] * y[j]</tt>.
        /// </summary>
        /// <param name="x">the first source vector.</param>
        /// <param name="y">the second source vector.</param>
        /// <param name="A">the matrix to hold the results. Set this parameter to <tt>null</tt> to indicate that a new result matrix shall be constructed.</param>
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
        /// Returns the one-norm of vector <tt>x</tt>, which is <tt>Sum(abs(x[i]))</tt>.
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
        /// Returns the one-norm of matrix <tt>A</tt>, which is the maximum absolute column sum.
        /// </summary>
        /// <param name="A">The matrix A</param>
        /// <returns>The one-norm of A.</returns>
        public static double Norm1(DoubleMatrix2D A)
        {
            double max = 0;
            for (int column = A.Columns; --column >= 0;)
            {
                max = Math.Max(max, Norm1(A.ViewColumn(column)));
            }
            return max;
        }

        /// <summary>
        /// Returns the two-norm (aka <i>euclidean norm</i>) of vector <tt>x</tt>; equivalent to <tt>mult(x,x)</tt>.
        /// </summary>
        /// <param name="x">The vector x.</param>
        /// <returns>The two-norm of A.</returns>
        public static double Norm2(DoubleMatrix1D x)
        {
            return Mult(x, x);
        }

        /// <summary>
        /// Returns the two-norm of matrix <tt>A</tt>, which is the maximum singular value; obtained from SVD.
        /// </summary>
        /// <param name="a">The matrix A.</param>
        /// <returns>The two-norm of A.</returns>
        public static double Norm2(DoubleMatrix2D a)
        {
            return Svd(a).Norm2();
        }

        /// <summary>
        /// Returns the infinity norm of vector <tt>x</tt>, which is <tt>Max(abs(x[i]))</tt>.
        /// </summary>
        /// <param name="x">The vector x.</param>
        /// <returns>The infinity norm of matrix <tt>A</tt></returns>
        public static double NormInfinity(DoubleMatrix1D x)
        {
            // fix for bug reported by T.J.Hunt@open.ac.uk
            if (x.Size == 0) return 0;
            return x.Aggregate(Math.Max, Math.Abs);
            //	if (x.Size==0) return 0;
            //	return x.aggregate(cern.Cern.Jet.math.Functions.plus,cern.Cern.Jet.math.Functions.abs);
            //	double max = 0;
            //	for (int i = x.Size; --i >= 0; ) {
            //		max = Math.max(max, x.getQuick(i));
            //	}
            //	return max;
        }

        /// <summary>
        /// Returns the infinity norm of matrix <tt>A</tt>, which is the maximum absolute row sum.
        /// </summary>
        /// <param name="A">The matrix A.</param>
        /// <returns>The infinity norm of matrix <tt>A</tt></returns>
        public static double NormInfinity(DoubleMatrix2D A)
        {
            double max = 0;
            for (int row = A.Rows; --row >= 0;)
            {
                //max = Math.max(max, normInfinity(A.viewRow(row)));
                max = Math.Max(max, Norm1(A.ViewRow(row)));
            }
            return max;
        }

        /// <summary>
        /// Returns the sum of the diagonal elements of matrix <tt>A</tt>; <tt>Sum(A[i,i])</tt>.
        /// </summary>
        /// <param name="a">
        /// The matrix A.
        /// </param>
        /// <returns>
        /// The sum of the diagonal elements of matrix <tt>A</tt>.
        /// </returns>
        public static double Trace(DoubleMatrix2D a)
        {
            double sum = 0;
            for (int i = Math.Min(a.Rows, a.Columns); --i >= 0;)
                sum += a[i, i];
            return sum;
        }

        /// <summary>
        /// Constructs and returns a new view which is the transposition of the given matrix <tt>A</tt>.
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
        /// <returns><tt>A</tt> (for convenience only).</returns>
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
        internal static double Hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b / a;
                r = Math.Abs(a) * Math.Sqrt(1 + (r * r));
            }
            else if (b != 0)
            {
                r = a / b;
                r = Math.Abs(b) * Math.Sqrt(1 + (r * r));
            }
            else
            {
                r = 0.0;
            }

            return r;
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
            if (indexes.Length != size) throw new IndexOutOfRangeException("invalid permutation");

            /*
            int i=size;
            int a;
            while (--i >= 0 && (a=indexes[i])==i) if (a < 0 || a >= size) throw new IndexOutOfBoundsException("invalid permutation");
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
            if (indexes.Length != size) throw new IndexOutOfRangeException("invalid permutation");

            /*
            int i=size;
            int a;
            while (--i >= 0 && (a=indexes[i])==i) if (a < 0 || a >= size) throw new IndexOutOfBoundsException("invalid permutation");
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

            Cern.Colt.GenericPermuting.permute(indexes, swapper, work, null);
            return A;
        }

        public static DoubleMatrix2D Solve(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            LUDecomposition lu = new LUDecomposition(A);
            QRDecomposition qr = new QRDecomposition(B);

            return (A.Rows == A.Columns ? (lu.Solve(B)) : (qr.Solve(B)));
        }

    }
}
