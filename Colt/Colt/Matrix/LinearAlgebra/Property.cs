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
        /// The default Property object; currently has <i>Tolerance==1.0E-9</i>.
        /// </summary>
        public static readonly Property DEFAULT = new Property(1.0E-9);

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
        /// Checks whether the given matrix <i>A</i> is <i>rectangular</i>.
        /// </summary>
        /// <param name="a">
        /// The matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <i>A.Rows &lt; A.Columns</i>.
        /// </exception>
        public static void CheckRectangular(DoubleMatrix2D a)
        {
            if (a.Rows < a.Columns)
            {
                throw new ArgumentOutOfRangeException("Matrix must be rectangular: " + AbstractFormatter.Shape(a));
            }
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
            for (int i = a.size; --i >= 0;)
            {
                double x = a[i];
                double diff = System.Math.Abs(value - x);
                if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
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
            int size = a.size;
            if (size != b.size) return false;

            double epsilon = Tolerance;
            for (int i = size; --i >= 0;)
            {
                double x = a[i];
                double value = b[i];
                double diff = System.Math.Abs(value - x);
                if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
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
                    if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
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
                    if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
                    if (!(diff <= epsilon))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether all cells of the given matrix <i>A</i> are equal to the given value.
        /// The result is <i>true</i> if and only if <i>A != null</i> and
        /// <i>! (System.Math.Abs(value - A[slice,row,col]) > tolerance())</i> holds for all coordinates.
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
                        double x = A.getQuick(slice, row, column);
                        double diff = System.Math.Abs(value - x);
                        if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
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
        /// <i>! (System.Math.Abs(A[slice,row,col] - B[slice,row,col]) > tolerance())</i> holds for all coordinates.
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
                        double x = A.getQuick(slice, row, column);
                        double value = B.getQuick(slice, row, column);
                        double diff = System.Math.Abs(value - x);
                        if ((diff != diff) && ((value != value && x != x) || value == x)) diff = 0;
                        if (!(diff <= epsilon)) return false;
                    }
                }
            }
            return true;
        }
    }
}
