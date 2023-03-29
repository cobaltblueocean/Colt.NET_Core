// <copyright file="TridiagonalDoubleMatrix2D.cs" company="CERN">
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
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Math;
using Cern.Colt.Function;

namespace Cern.Colt.Matrix.Implementation
{
    using F1 = Cern.Jet.Math.Functions.DoubleFunctions;
    using F2 = Cern.Jet.Math.Functions.DoubleDoubleFunctions;

    /// <summary>
    /// Tridiagonal 2-d matrix holding <i>double</i> elements.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <p>
    /// <b>Implementation:</b>
    /// TODO.
    /// 
    /// @author wolfgang.hoschek@cern.ch
    /// @version 0.9, 04/14/2000
    /// </summary>
    public class TridiagonalDoubleMatrix2D : WrapperDoubleMatrix2D
    {

        /// <summary>
        /// The non zero elements of the matrix: {lower, diagonal, upper}.
        /// </summary>
        private double[] values;

        public double[] Values
        {
            get { return values; }
            protected set { values = value; }
        }

        /// <summary>
        /// The startIndexes and number of non zeros: {lowerStart, diagonalStart, upperStart, values.Length, lowerNonZeros, diagonalNonZeros, upperNonZeros}.
        /// lowerStart = 0
        /// diagonalStart = lowerStart + lower.Length
        /// upperStart = diagonalStart + diagonal.Length
        /// </summary>
        protected int[] dims;

        public int[] Dims
        {
            get { return dims; }
            protected set { dims = value; }
        }

        protected static int NONZERO = 4;


        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <i>[row,column]</i>.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        public override double this[int row, int column]
        {
            get
            {
                int i = row;
                int j = column;

                int k = j - i + 1;
                int q = i;
                if (k == 0) q = j; // lower diagonal

                if (k >= 0 && k <= 2)
                {
                    return values[dims[k] + q];
                }
                return 0;
            }
            set
            {
                int i = row;
                int j = column;

                Boolean isZero = (value == 0);

                int k = j - i + 1;
                int q = i;
                if (k == 0) q = j; // lower diagonal

                if (k >= 0 && k <= 2)
                {
                    int index = dims[k] + q;
                    if (values[index] != 0)
                    {
                        if (isZero) dims[k + NONZERO]--; // one nonZero less
                    }
                    else
                    {
                        if (!isZero) dims[k + NONZERO]++; // one nonZero more
                    }
                    values[index] = value;
                    return;
                }

                if (!isZero) throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_CannotStoreNonZeroValueToNonTridiagonalCoordinate, row, column, value));
            }
        }


        //protected double diagonal[];
        //protected double lower[];
        //protected double upper[];

        //protected int diagonalNonZeros;
        //protected int lowerNonZeros;
        //protected int upperNonZeros;
        //protected int N;

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// <i>values</i> is required to have the form <i>values[row][column]</i>
        /// and have exactly the same number of columns in every row.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">The values to be filled into the new matrix.</param>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= row &lt; values.Length: values[row].Length != values[row-1].Length</i>.</exception>
        public TridiagonalDoubleMatrix2D(double[][] values) : this(values.Length, values.Length == 0 ? 0 : values.GetLength(1))
        {
            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of rows and columns.
        /// All entries are initially <i>0</i>.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <exception cref="ArgumentException">if <i>rows &lt; 0 || columns &lt; 0 || (double)columns * rows > int.MaxValue</i>.</exception>
        public TridiagonalDoubleMatrix2D(int rows, int columns) : base(null)
        {
            Setup(rows, columns);

            int d = System.Math.Min(rows, columns);
            int u = d - 1;
            int l = d - 1;
            if (rows > columns) l++;
            if (rows < columns) u++;

            values = new double[l + d + u]; // {lower, diagonal, upper}
            int[] dimensions = { 0, l, l + d, l + d + u, 0, 0, 0 }; // {lowerStart, diagonalStart, upperStart, values.Length, lowerNonZeros, diagonalNonZeros, upperNonZeros}
            dims = dimensions;

            //diagonal = new double[d];
            //lower = new double[l];
            //upper = new double[u];

            //diagonalNonZeros = 0;
            //lowerNonZeros = 0;
            //upperNonZeros = 0;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>value</i>.
        /// </summary>
        /// <param name="value">the value to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        public override IDoubleMatrix2D Assign(double value)
        {
            // overriden for performance only
            if (value == 0)
            {
                for (int i = values.Length; --i >= 0;) values[i] = 0;
                for (int i = dims.Length; --i >= NONZERO;) dims[i] = 0;

                //for (int i=diagonal.Length; --i >= 0; ) diagonal[i]=0;
                //for (int i=upper.Length; --i >= 0; ) upper[i]=0;
                //for (int i=lower.Length; --i >= 0; ) lower[i]=0;

                //diagonalNonZeros = 0;
                //lowerNonZeros = 0;
                //upperNonZeros = 0;
            }
            else base.Assign(value);
            return this;
        }

        public IDoubleMatrix2D Assign(IDoubleFunction function, Double multialpha = 1)
        {
            if (Cern.Jet.Math.Functions.EvaluateFunctionEquality(function.Eval.Method, F2.Mult.Eval.Method))
            { // x[i] = mult*x[i]
                double alpha = multialpha; //((Cern.Jet.Math.Mult)function).multiplicator;
                if (alpha == 1) return this;
                if (alpha == 0) return Assign(0);
                if (double.IsNaN(alpha)) return Assign(alpha); // the funny definition of IsNaN()d This should better not happen.

                /*
                double[] vals = values.ToArray();
                for (int j=values.Count; --j >= 0; ) {
                    vals[j] *= alpha;
                }
                */

                ForEachNonZero(
                    new Cern.Colt.Function.IntIntDoubleFunctionDelegate((i, j, value) =>
                    {

                        return function.Apply(value);
                    }
        ));
            }
            else
            {
                base.Assign(function);
            }
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same number of rows and columns.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
        /// </summary>
        /// <param name="source">the source matrix to copy from (may be identical to the receiver).</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>columns() != source.columns() || rows() != source.rows()</i></exception>
        public override IDoubleMatrix2D Assign(IDoubleMatrix2D source)
        {
            // overriden for performance only
            if (source == this) return this; // nothing to do
            CheckShape(source);

            if (source is TridiagonalDoubleMatrix2D)
            {
                // quickest
                TridiagonalDoubleMatrix2D other = (TridiagonalDoubleMatrix2D)source;

                Array.Copy(other.Values, 0, this.Values, 0, this.Values.Length);
                Array.Copy(other.dims, 0, this.dims, 0, this.dims.Length);
                return this;
            }

            if (source is RCDoubleMatrix2D || source is SparseDoubleMatrix2D)
            {
                Assign(0);
                source.ForEachNonZero(
                    new Cern.Colt.Function.IntIntDoubleFunctionDelegate((i, j, value) =>
                    {
                        this[i, j] = value;
                        return value;
                    }
            ));
                return this;
            }

            return base.Assign(source);
        }

        public IDoubleMatrix2D Assign(IDoubleMatrix2D matrixY, Cern.Colt.Function.IDoubleDoubleFunction function, Double x = 0, Double y = 0)
        {
            CheckShape(matrixY);

            if (Cern.Jet.Math.Functions.EvaluateDoubleDoubleFunctionEquality(function, F2.PlusMult(x)))
            { // x[i] = x[i] + alpha*y[i]
                double alpha = x; // ((Cern.Jet.Math.PlusMult)function).multiplicator;
                if (alpha == 0) return this; // nothing to do
                matrixY.ForEachNonZero(
                    new Cern.Colt.Function.IntIntDoubleFunctionDelegate((i, j, value) =>
                    {

                        this[i, j] = this[i, j] + alpha * value;
                        return value;
                    }
                ));
                return this;
            }

            if (Cern.Jet.Math.Functions.EvaluateFunctionEquality(function.Eval.Method, F2.Mult.Eval.Method))
            { // x[i] = x[i] * y[i]
                ForEachNonZero(

                    new Cern.Colt.Function.IntIntDoubleFunctionDelegate((i, j, value) =>
                    {
                        this[i, j] = this[i, j] * matrixY[i, j];
                        return value;
                    }
                ));
                return this;

            }

            if (Cern.Jet.Math.Functions.EvaluateFunctionEquality(function.Eval.Method, F2.Div.Eval.Method))
            { // x[i] = x[i] / y[i]
                ForEachNonZero(

                    new Cern.Colt.Function.IntIntDoubleFunctionDelegate((i, j, value) =>
                    {
                        this[i, j] = this[i, j] / matrixY[i, j];
                        return value;
                    }
                ));
                return this;

            }

            return base.Assign(matrixY, function);
        }

        public override IDoubleMatrix2D ForEachNonZero(Cern.Colt.Function.IntIntDoubleFunctionDelegate function)
        {
            for (int kind = 0; kind <= 2; kind++)
            {
                int i = 0, j = 0;
                switch (kind)
                {
                    case 0:
                        i = 1;  // lower 
                                // case 1: {   } // diagonal
                        break;
                    case 2:
                        j = 1;  // upper
                        break;
                }
                int low = dims[kind];
                int high = dims[kind + 1];

                for (int k = low; k < high; k++, i++, j++)
                {
                    double value = values[k];
                    if (value != 0)
                    {
                        double r = function(i, j, value);
                        if (r != value)
                        {
                            if (r == 0) dims[kind + NONZERO]++; // one non zero more
                            values[k] = r;
                        }
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        /// <returns></returns>
        protected new IDoubleMatrix2D GetContent()
        {
            return this;
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>[row,column]</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        [Obsolete("GetQuick(int row, int column) is deprecated, please use indexer instead.")]
        public override double GetQuick(int row, int column)
        {
            return this[row, column];
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix2D</i> the new matrix must also be of type <i>DenseDoubleMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix2D</i> the new matrix must also be of type <i>SparseDoubleMatrix2D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override IDoubleMatrix2D Like(int rows, int columns)
        {
            return new TridiagonalDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix2D</i> the new matrix must be of type <i>DenseIDoubleMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix2D</i> the new matrix must be of type <i>SparseIDoubleMatrix1D</i>, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override IDoubleMatrix1D Like1D(int size)
        {
            return new SparseDoubleMatrix1D(size);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[row,column]</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        [Obsolete("SetQuick(int row, int column, double value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int row, int column, double value)
        {
            this[row, column] = value;
        }

        public override IDoubleMatrix1D ZMult(IDoubleMatrix1D y, IDoubleMatrix1D z, double alpha, double beta, Boolean transposeA)
        {
            int m = Rows;
            int n = Columns;
            if (transposeA)
            {
                m = Columns;
                n = Rows;
            }

            Boolean ignore = (z == null);
            if (z == null) z = new DenseDoubleMatrix1D(m);

            if (!(!this.IsView && y is DenseDoubleMatrix1D && z is DenseDoubleMatrix1D))
            {
                return base.ZMult(y, z, alpha, beta, transposeA);
            }

            if (n != y.Size || m > z.Size)
                throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_IncompatibleArgs, ((transposeA ? ViewDice() : this).ToStringShort()) , y.ToStringShort() ,z.ToStringShort()));

            if (!ignore) z.Assign(F1.Mult(beta / alpha));

            DenseDoubleMatrix1D zz = (DenseDoubleMatrix1D)z;
            double[] zElements = zz.Elements;
            int zStride = zz.Stride;
            int zi = z.Index(0);

            DenseDoubleMatrix1D yy = (DenseDoubleMatrix1D)y;
            double[] yElements = yy.Elements;
            int yStride = yy.Stride;
            int yi = y.Index(0);

            if (yElements == null || zElements == null) throw new NullReferenceException();

            ForEachNonZero(
                new Cern.Colt.Function.IntIntDoubleFunctionDelegate((i, j, value) =>
                {

                    if (transposeA) { int tmp = i; i = j; j = tmp; }
                    zElements[zi + zStride * i] += value * yElements[yi + yStride * j];
            //z.setQuick(row,z.getQuick(row) + value * y.getQuick(column));
            //Console.WriteLine("["+i+","+j+"]-->"+value);
            return value;
                }
            ));

            if (alpha != 1) z.Assign(F1.Mult(alpha));
            return z;
        }

        public override IDoubleMatrix2D ZMult(IDoubleMatrix2D B, IDoubleMatrix2D C, double alpha, double beta, Boolean transposeA, Boolean transposeB)
        {
            if (transposeB) B = B.ViewDice();
            int m = Rows;
            int n = Columns;
            if (transposeA)
            {
                m = Columns;
                n = Rows;
            }
            int p = B.Columns;
            Boolean ignore = (C == null);
            if (C == null) C = new DenseDoubleMatrix2D(m, p);

            if (B.Rows != n)
                throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_Matrix2DInnerDimensionMustAgree ,ToStringShort() , (transposeB ? B.ViewDice() : B).ToStringShort()));
            if (C.Rows != m || C.Columns != p)
                throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_IncompatibleResultMatrix, ToStringShort() , (transposeB ? B.ViewDice() : B).ToStringShort(), C.ToStringShort()));
            if (this == C || B == C)
                throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_MatricesMustNotBeIdentical);

            if (!ignore) C.Assign(F1.Mult(beta));

            // cache views	
            IDoubleMatrix1D[] Brows = new IDoubleMatrix1D[n];
            for (int i = n; --i >= 0;) Brows[i] = B.ViewRow(i);
            IDoubleMatrix1D[] Crows = new IDoubleMatrix1D[m];
            for (int i = m; --i >= 0;) Crows[i] = C.ViewRow(i);


            ForEachNonZero(
                new Cern.Colt.Function.IntIntDoubleFunctionDelegate((i, j, value) =>
                {

                    var fun = F2.PlusMult(value * alpha);
            //fun.multiplicator = value * alpha;
            if (!transposeA)
                        Crows[i].Assign(Brows[j], fun);
                    else
                        Crows[j].Assign(Brows[i], fun);
                    return value;
                }
            ));

            return C;
        }
    }
}
