// <copyright file="RCDoubleMatrix2D.cs" company="CERN">
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
using Cern.Colt.List;

namespace Cern.Colt.Matrix.Implementation
{
    using F1 = Cern.Jet.Math.Functions.DoubleFunctions;
    using F2 = Cern.Jet.Math.Functions.DoubleDoubleFunctions;

    public class RCDoubleMatrix2D : WrapperDoubleMatrix2D
    {

        /// <summary>
        /// The elements of the matrix.
        /// </summary>
        
        public IntArrayList Indexes { get; protected set; }
        public DoubleArrayList Values { get; protected set; }
        public int[] Starts { get; protected set; }

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
                int k = Indexes.BinarySearchFromTo(column, Starts[row], Starts[row + 1] - 1);
                double v = 0;
                if (k >= 0) v = Values[k];
                return v;
            }
            set {
                int k = Indexes.BinarySearchFromTo(column, Starts[row], Starts[row + 1] - 1);
                if (k >= 0)
                { // found
                    if (value == 0)
                        Remove(row, k);
                    else
                        Values[k] = value;
                    return;
                }

                if (value != 0)
                {
                    k = -k - 1;
                    Insert(row, column, k, value);
                }
            }
        }

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
        public RCDoubleMatrix2D(double[][] values) : this(values.Length, values.Length == 0 ? 0 : values.GetLength(1))
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
        public RCDoubleMatrix2D(int rows, int columns) : base(null)
        {

            try
            {
                Setup(rows, columns);
            }
            catch (ArgumentException exc)
            { // we can hold rows*columns>int.MaxValue cells !
                if (!"matrix too large".Equals(exc.Message)) throw exc;
            }
            Indexes = new IntArrayList();
            Values = new DoubleArrayList();
            Starts = new int[rows + 1];
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>value</i>.
        /// </summary>
        /// <param name="value">the value to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        public override DoubleMatrix2D Assign(double value)
        {
            // overriden for performance only
            if (value == 0)
            {
                Indexes.Clear();
                Values.Clear();
                for (int i = Starts.Length; --i >= 0;) Starts[i] = 0;
            }
            else base.Assign(value);
            return this;
        }

        public DoubleMatrix2D Assign(Cern.Colt.Function.DoubleFunction function, Double multialpha = 1)
        {
            if (Cern.Jet.Math.Functions.EvaluateDoubleFunctionEquality(function, F1.Mult(multialpha)))
            {
                //if (function == F1.Mult()) { // x[i] = mult*x[i]
                //var mult = new Cern.Jet.Math.Mult();
                double alpha = multialpha; //((Cern.Jet.Math.Mult)function).multiplicator;
                if (alpha == 1) return this;
                if (alpha == 0) return Assign(0);
                if (double.IsNaN(alpha)) return Assign(alpha); // the funny definition of IsNaN()d This should better not happen.

                double[] vals = Values.ToArray();
                for (int j = Values.Count; --j >= 0;)
                {
                    vals[j] *= alpha;
                }

                /*
                forEachNonZero(
                    new Cern.Colt.Function.IntIntDoubleFunction() {
                        public double apply(int i, int j, double value) {
                            return function(value);
                        }
                    }
                );
                */
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
        public override DoubleMatrix2D Assign(DoubleMatrix2D source)
        {
            if (source == this) return this; // nothing to do
            CheckShape(source);
            // overriden for performance only
            if (!(source is RCDoubleMatrix2D))
            {
                //return base.Assign(source);

                Assign(0);
                source.ForEachNonZero(
                    new Cern.Colt.Function.IntIntDoubleFunction((i, j, value) =>
                    {

                        this[i, j] = value;
                        return value;
                    }));

                return this;
            }

            // even quicker
            RCDoubleMatrix2D other = (RCDoubleMatrix2D)source;

            Array.Copy(other.Starts, 0, this.Starts, 0, this.Starts.Length);
            int s = other.Indexes.Count;
            this.Indexes.SetSize(s);
            this.Values.SetSize(s);
            this.Indexes.ReplaceFromToWithFrom(0, s - 1, other.Indexes, 0);
            this.Values.ReplaceFromToWithFrom(0, s - 1, other.Values, 0);

            return this;
        }

        public DoubleMatrix2D Assign(DoubleMatrix2D matrixY, Cern.Colt.Function.DoubleDoubleFunction function, Double x = 0, Double y = 0)
        {
            CheckShape(matrixY);

            if (Cern.Jet.Math.Functions.EvaluateDoubleDoubleFunctionEquality(function, F2.PlusMult(x)))
            {
                double alpha = x;

                //if (function is Cern.Jet.Math.PlusMult) { // x[i] = x[i] + alpha*y[i]
                //double alpha = ((Cern.Jet.Math.PlusMult)function).multiplicator;
                if (alpha == 0) return this; // nothing to do
                matrixY.ForEachNonZero(
                    new Cern.Colt.Function.IntIntDoubleFunction((i, j, value) =>
                    {
                        this[i, j] = this[i, j] + alpha * value;
                        return value;
                    }
            ));
                return this;
            }

            //if (function==Cern.Jet.Math.Functions.mult) { // x[i] = x[i] * y[i]
            var mult = F2.Mult(x, y);
            if (Cern.Jet.Math.Functions.EvaluateFunctionEquality(function.Method, F2.Mult.Method))
            {
                int[] idx = Indexes.ToArray();
                double[] vals = Values.ToArray();

                for (int i = Starts.Length - 1; --i >= 0;)
                {
                    int low = Starts[i];
                    for (int k = Starts[i + 1]; --k >= low;)
                    {
                        int j = idx[k];
                        vals[k] *= matrixY[i, j];
                        if (vals[k] == 0) Remove(i, j);
                    }
                }
                return this;

            }

            if (Cern.Jet.Math.Functions.EvaluateFunctionEquality(function.Method, F2.Div.Method))
            {
                //	if (function==Cern.Jet.Math.Functions.Div) { // x[i] = x[i] / y[i]
                int[] idx = Indexes.ToArray();
                double[] vals = Values.ToArray();

                for (int i = Starts.Length - 1; --i >= 0;)
                {
                    int low = Starts[i];
                    for (int k = Starts[i + 1]; --k >= low;)
                    {
                        int j = idx[k];
                        vals[k] /= matrixY[i, j];
                        if (vals[k] == 0) Remove(i, j);
                    }
                }
                return this;

            }

            return base.Assign(matrixY, function);
        }

        public override DoubleMatrix2D ForEachNonZero(Cern.Colt.Function.IntIntDoubleFunction function)
        {
            int[] idx = Indexes.ToArray();
            double[] vals = Values.ToArray();

            for (int i = Starts.Length - 1; --i >= 0;)
            {
                int low = Starts[i];
                for (int k = Starts[i + 1]; --k >= low;)
                {
                    int j = idx[k];
                    double value = vals[k];
                    double r = function(i, j, value);
                    if (r != value) vals[k] = r;
                }
            }
            return this;
        }

        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        /// <returns></returns>
        protected new DoubleMatrix2D GetContent()
        {
            return this;
        }

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
        [Obsolete("GetQuick(int row, int column) is deprecated, please use indexer instead.")]
        public override Double GetQuick(int row, int column)
        {
            return this[row, column];
        }

        protected void Insert(int row, int column, int index, double value)
        {
            Indexes.Insert(index, column);
            Values.Insert(index, value);
            for (int i = Starts.Length; --i > row;) Starts[i]++;
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
        public override DoubleMatrix2D Like(int rows, int columns)
        {
            return new RCDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix2D</i> the new matrix must be of type <i>DenseDoubleMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix2D</i> the new matrix must be of type <i>SparseDoubleMatrix1D</i>, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override DoubleMatrix1D Like1D(int size)
        {
            return new SparseDoubleMatrix1D(size);
        }

        protected void Remove(int row, int index)
        {
            Indexes.Remove(index);
            Values.Remove(index);
            for (int i = Starts.Length; --i > row;) Starts[i]--;
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

        public void TrimToSize()
        {
            Indexes.TrimToSize();
            Values.TrimToSize();
        }

        public override DoubleMatrix1D ZMult(DoubleMatrix1D y, DoubleMatrix1D z, double alpha, double beta, Boolean transposeA)
        {
            int m = Rows;
            int n = Columns;
            if (transposeA)
            {
                m = Columns;
                n = Rows;
            }

            Boolean ignore = (z == null || !transposeA);
            if (z == null) z = new DenseDoubleMatrix1D(m);

            if (!(y is DenseDoubleMatrix1D && z is DenseDoubleMatrix1D))
            {
                return base.ZMult(y, z, alpha, beta, transposeA);
            }

            if (n != y.Size || m > z.Size)
                throw new ArgumentException("Incompatible args: " + ((transposeA ? ViewDice() : this).ToStringShort()) + ", " + y.ToStringShort() + ", " + z.ToStringShort());

            DenseDoubleMatrix1D zz = (DenseDoubleMatrix1D)z;
            double[] zElements = zz.Elements;
            int zStride = zz.Stride;
            int zi = z.Index(0);

            DenseDoubleMatrix1D yy = (DenseDoubleMatrix1D)y;
            double[] yElements = yy.Elements;
            int yStride = yy.Stride;
            int yi = y.Index(0);

            if (yElements == null || zElements == null) throw new NullReferenceException();

            /*
            forEachNonZero(
                new Cern.Colt.Function.IntIntDoubleFunction() {
                    public double apply(int i, int j, double value) {
                        zElements[zi + zStride*i] += value * yElements[yi + yStride*j];
                        //z.SetQuick(row,z.getQuick(row) + value * y.getQuick(column));
                        //Console.WriteLine("["+i+","+j+"]-->"+value);
                        return value;
                    }
                }
            );
            */


            int[] idx = Indexes.ToArray();
            double[] vals = Values.ToArray();
            int s = Starts.Length - 1;
            if (!transposeA)
            {
                for (int i = 0; i < s; i++)
                {
                    int high = Starts[i + 1];
                    double sum = 0;
                    for (int k = Starts[i]; k < high; k++)
                    {
                        int j = idx[k];
                        sum += vals[k] * yElements[yi + yStride * j];
                    }
                    zElements[zi] = alpha * sum + beta * zElements[zi];
                    zi += zStride;
                }
            }
            else
            {
                if (!ignore) z.Assign(F1.Mult(beta));
                for (int i = 0; i < s; i++)
                {
                    int high = Starts[i + 1];
                    double yElem = alpha * yElements[yi + yStride * i];
                    for (int k = Starts[i]; k < high; k++)
                    {
                        int j = idx[k];
                        zElements[zi + zStride * j] += vals[k] * yElem;
                    }
                }
            }

            return z;
        }

        public override DoubleMatrix2D ZMult(DoubleMatrix2D B, DoubleMatrix2D C, double alpha, double beta, Boolean transposeA, Boolean transposeB)
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
                throw new ArgumentException("Matrix2D inner dimensions must agree:" + ToStringShort() + ", " + (transposeB ? B.ViewDice() : B).ToStringShort());
            if (C.Rows != m || C.Columns != p)
                throw new ArgumentException("Incompatibel result matrix: " + ToStringShort() + ", " + (transposeB ? B.ViewDice() : B).ToStringShort() + ", " + C.ToStringShort());
            if (this == C || B == C)
                throw new ArgumentException("Matrices must not be identical");

            if (!ignore) C.Assign(F1.Mult(beta));

            // cache views	
            DoubleMatrix1D[] Brows = new DoubleMatrix1D[n];
            for (int i = n; --i >= 0;) Brows[i] = B.ViewRow(i);
            DoubleMatrix1D[] Crows = new DoubleMatrix1D[m];
            for (int i = m; --i >= 0;) Crows[i] = C.ViewRow(i);

            int[] idx = Indexes.ToArray();
            double[] vals = Values.ToArray();
            for (int i = Starts.Length - 1; --i >= 0;)
            {
                int low = Starts[i];
                for (int k = Starts[i + 1]; --k >= low;)
                {
                    int j = idx[k];
                    var fun = F2.PlusMult(vals[k] * alpha);
                    //fun.Multiplicator = vals[k] * alpha;
                    if (!transposeA)
                        Crows[i].Assign(Brows[j], fun);
                    else
                        Crows[j].Assign(Brows[i], fun);
                }
            }

            return C;
        }
    }
}
