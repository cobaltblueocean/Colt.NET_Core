namespace Cern.Colt.Matrix.LinearAlgebra
{
    using System;

    using Implementation;

    /// <summary>
    /// Tests matrices for linear algebraic properties (equality, tridiagonality, symmetry, singularity, etc).
    /// </summary>
    public class Property : PersistentObject
    {
        /// <summary>
        /// The default Property object; currently has <tt>Tolerance==1.0E-9</tt>.
        /// </summary>
        public static readonly Property DEFAULT = new Property(1.0E-9);

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class with a tolerance of <tt>Math.Abs(newTolerance)</tt>.
        /// </summary>
        /// <param name="newTolerance">
        /// The new tolerance.
        /// </param>
        public Property(double newTolerance)
        {
            Tolerance = Math.Abs(newTolerance);
        }

        /// <summary>
        /// Gets or sets the current tolerance.
        /// </summary>
        public double Tolerance { get; protected set; }

        /// <summary>
        /// Checks whether the given matrix <tt>A</tt> is <i>rectangular</i>.
        /// </summary>
        /// <param name="a">
        /// The matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>A.rows() &lt; A.columns()</tt>.
        /// </exception>
        public static void CheckRectangular(DoubleMatrix2D a)
        {
            if (a.Rows < a.Columns)
            {
                throw new ArgumentOutOfRangeException("Matrix must be rectangular: " + AbstractFormatter.Shape(a));
            }
        }

        /// <summary>
        /// Returns whether all cells of the given matrix <tt>A</tt> are equal to the given value.
        /// The result is <tt>true</tt> if and only if <tt>A != null</tt> and
        /// <tt>! (Math.abs(value - A[i]) > Tolerance)</tt> holds for all coordinates.
        /// </summary>
        /// <param name="a">
        /// The first matrix to compare.
        /// </param>
        /// <param name="value">
        /// The value to compare against.
        /// </param>
        /// <returns>
        /// <tt>true</tt> if the matrix is equal to the value;
        /// <tt>false</tt> otherwise.
        /// </returns>
        public bool Equals(DoubleMatrix1D a, double value)
        {
            if (a == null) return false;
            double epsilon = Tolerance;
            for (int i = a.Size(); --i >= 0;)
            {
                double x = a[i];
                double diff = Math.Abs(value - x);
                if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
                if (!(diff <= epsilon)) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether both given matrices <tt>A</tt> and <tt>B</tt> are equal.
        /// The result is <tt>true</tt> if <tt>A==B</tt>. 
        /// Otherwise, the result is <tt>true</tt> if and only if both arguments are <tt>!= null</tt>, 
        /// have the same size and
        /// <tt>! (Math.abs(A[i] - B[i]) > Tolerance)</tt> holds for all indexes.
        /// </summary>
        /// <param name="a">
        /// The first matrix to compare.
        /// </param>
        /// <param name="b">
        /// The second matrix to compare.
        /// </param>
        /// <returns>
        /// <tt>true</tt> if both matrices are equal;
        /// <tt>false</tt> otherwise.
        /// </returns>
        public bool Equals(DoubleMatrix1D a, DoubleMatrix1D b)
        {
            if (a == b) return true;
            if (!(a != null && b != null)) return false;
            int size = a.Size();
            if (size != b.Size()) return false;

            double epsilon = Tolerance;
            for (int i = size; --i >= 0;)
            {
                double x = a[i];
                double value = b[i];
                double diff = Math.Abs(value - x);
                if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
                if (!(diff <= epsilon)) return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether all cells of the given matrix <tt>A</tt> are equal to the given value.
        /// </summary>
        /// <param name="a">
        /// The first matrix to compare.
        /// </param>
        /// <param name="value">
        /// The value to compare against.
        /// </param>
        /// <returns>
        /// <tt>true</tt> if the matrix is equal to the value;
        /// <tt>false</tt> otherwise.
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
                    double diff = Math.Abs(value - x);
                    if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
                    if (!(diff <= epsilon)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether both given matrices <tt>A</tt> and <tt>B</tt> are equal.
        /// </summary>
        /// <param name="a">
        /// The first matrix to compare.
        /// </param>
        /// <param name="b">
        /// The second matrix to compare.
        /// </param>
        /// <returns>
        /// <tt>true</tt> if both matrices are equal;
        /// <tt>false</tt> otherwise.
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
                    double diff = Math.Abs(value - x);
                    if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
                    if (!(diff <= epsilon))
                        return false;
                }
            }

            return true;
        }
    }
}
