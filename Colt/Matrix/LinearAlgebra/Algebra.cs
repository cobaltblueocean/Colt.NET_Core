namespace Colt.Matrix.LinearAlgebra
{
    using System;
    using Colt.Function;

    /// <summary>
    /// Linear algebraic matrix operations operating on {@link DoubleMatrix2D}; concentrates most functionality of this package.
    /// </summary>
    public static class Algebra
    {
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
        /// <tt>z</tt>; a new vector with <tt>z.size()==A.rows()</tt>.
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
            if (x.Size() == 0) return 0;
            return x.Aggregate(BinaryFunctions.Plus, Math.Abs);
        }

        /// <summary>
        /// Returns the two-norm of matrix <tt>A</tt>, which is the maximum singular value; obtained from SVD.
        /// </summary>
        /// <param name="a">The matrix A.</param>
        /// <returns>The two-norm of A.</returns>
        public static double Norm2(DoubleMatrix2D a)
        {
            return svd(a).Norm2();
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
        internal static double hypot(double a, double b)
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
        /// Constructs and returns the SingularValue-decomposition of the given matrix.
        /// </summary>
        /// <param name="matrix">
        /// The matrix A
        /// </param>
        /// <returns>
        /// The SVD of A.
        /// </returns>
        private static SingularValueDecomposition svd(DoubleMatrix2D matrix)
        {
            return new SingularValueDecomposition(matrix);
        }
    }
}
