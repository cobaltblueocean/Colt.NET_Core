// <copyright file="CholeskyDecomposition.cs" company="CERN">
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

namespace Cern.Colt.Matrix.LinearAlgebra
{
    using F1 = Cern.Jet.Math.Functions.DoubleFunctions;
    using F2 = Cern.Jet.Math.Functions.DoubleDoubleFunctions;


    /// <summary>
    /// For a symmetric, positive definite matrix <i>A</i>, the Cholesky decomposition
    /// is a lower triangular matrix<i>L</i> so that<i> A = L * L'</i>;
    /// If the matrix is not symmetric or positive definite, the constructor
    /// returns a partial decomposition and sets an internal flag that may
    /// be queried by the<i> isSymmetricPositiveDefinite()</i> method.
    /// </summary>
    [Serializable]
    public class CholeskyDecomposition
    {
        /// <summary>
        /// Array for internal storage of decomposition.
        /// internal array storage.
        /// </summary>
        private DoubleMatrix2D mL;

        /// <summary>
        /// Row and column dimension (square matrix).
        /// @serial matrix dimension.
        /// </summary>
        private int n;

        /// <summary>
        /// Symmetric and positive definite flag.
        /// @serial is symmetric and positive definite flag.
        /// </summary>
        private Boolean isSymmetricPositiveDefinite;

        /// <summary>
        /// Constructs and returns a new Cholesky decomposition object for a symmetric and positive definite matrix; 
        /// The decomposed matrices can be retrieved via instance methods of the returned decomposition object.
        /// 
        /// Return a structure to access <i>L</i> and <i>isSymmetricPositiveDefinite</i> flag.
        /// </summary>
        /// <param name="A">Square, symmetric matrix.</param>
        /// <exception cref="ArgumentException">if <i>A</i> is not square.</exception>
        public CholeskyDecomposition(DoubleMatrix2D A)
        {
            Property.DEFAULT.CheckSquare(A);
            // Initialize.
            //double[][] A = Arg.getArray();

            n = A.Rows;
            //L = new double[n][n];
            mL = A.Like(n, n);
            isSymmetricPositiveDefinite = (A.Columns == n);

            //precompute and cache some views to avoid regenerating them time and again
            DoubleMatrix1D[] Lrows = new DoubleMatrix1D[n];
            for (int j = 0; j < n; j++) Lrows[j] = mL.ViewRow(j);

            // Main loop.
            for (int j = 0; j < n; j++)
            {
                //double[] Lrowj = L[j];
                //DoubleMatrix1D Lrowj = L.ViewRow(j);
                double d = 0.0;
                for (int k = 0; k < j; k++)
                {
                    //double[] Lrowk = L[k];
                    double s = Lrows[k].ZDotProduct(Lrows[j], 0, k);
                    /*
                    DoubleMatrix1D Lrowk = L.ViewRow(k);
                    double s = 0.0;
                    for (int i = 0; i < k; i++) {
                       s += Lrowk.getQuick(i)*Lrowj.getQuick(i);
                    }
                    */
                    s = (A[j, k] - s) / mL[k, k];
                    Lrows[j][k] = s;
                    d = d + s * s;
                    isSymmetricPositiveDefinite = isSymmetricPositiveDefinite && (A[k, j] == A[j, k]);
                }
                d = A[j, j] - d;
                isSymmetricPositiveDefinite = isSymmetricPositiveDefinite && (d > 0.0);
                mL[j, j] = System.Math.Sqrt(System.Math.Max(d, 0.0));

                for (int k = j + 1; k < n; k++)
                {
                    mL[j, k] = 0.0;
                }
            }
        }

        /// <summary>
        /// Returns the triangular factor, <i>L</i>.
        /// </summary>
        public DoubleMatrix2D L
        {
            get
            {
                return mL;
            }
        }

        /// <summary>
        /// Returns whether the matrix <i>A</i> is symmetric and positive definite.
        /// </summary>
        /// <returns>true if <i>A</i> is symmetric and positive definite; false otherwise</returns>
        public Boolean IsSymmetricPositiveDefinite
        {
            get
            {
                return isSymmetricPositiveDefinite;
            }
        }

        /// <summary>
        /// Solves <i>A*X = B</i>; returns <i>X</i>.
        /// </summary>
        /// <param name="B">A Matrix with as many rows as <i>A</i> and any number of columns.</param>
        /// <returns><i>X</i> so that <i>L*L'*X = B</i>.</returns>
        /// <exception cref="ArgumentException">if <i>B.Rows != A.Rows</i>.</exception>
        /// <exception cref="ArgumentException">if <i>!isSymmetricPositiveDefinite()</i>.</exception>
        public DoubleMatrix2D Solve(DoubleMatrix2D B)
        {
            // Copy right hand side.
            DoubleMatrix2D X = B.Copy();
            int nx = B.Columns;

            // fix by MG Ferreira <mgf@webmail.co.Za>
            // old code is in method xxxSolveBuggy()
            for (int c = 0; c < nx; c++)
            {
                // Solve L*Y = B;
                for (int i = 0; i < n; i++)
                {
                    double sum = B[i, c];
                    for (int k = i - 1; k >= 0; k--)
                    {
                        sum -= mL[i, k] * X[k, c];
                    }
                    X[i, c] = sum / mL[i, i];
                }

                // Solve L'*X = Y;
                for (int i = n - 1; i >= 0; i--)
                {
                    double sum = X[i, c];
                    for (int k = i + 1; k < n; k++)
                    {
                        sum -= mL[k, i] * X[k, c];
                    }
                    X[i, c] = sum / mL[i, i];
                }
            }

            return X;
        }

        /// <summary>
        /// Solves <i>A*X = B</i>; returns <i>X</i>.
        /// </summary>
        /// <param name="B">A Matrix with as many rows as <i>A</i> and any number of columns.</param>
        /// <returns><i>X</i> so that <i>L*L'*X = B</i>.</returns>
        /// <exception cref="ArgumentException">if <i>B.Rows != A.Rows</i>.</exception>
        /// <exception cref="ArgumentException">if <i>!isSymmetricPositiveDefinite()</i>.</exception>
        private DoubleMatrix2D XXXsolveBuggy(DoubleMatrix2D B)
        {
            var F = Cern.Jet.Math.Functions.functions;

            if (B.Rows != n)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!isSymmetricPositiveDefinite)
            {
                throw new ArgumentException("Matrix is not symmetric positive definite.");
            }

            // Copy right hand side.
            DoubleMatrix2D X = B.Copy();
            int nx = B.Columns;

            // precompute and cache some views to avoid regenerating them time and again
            DoubleMatrix1D[] Xrows = new DoubleMatrix1D[n];
            for (int k = 0; k < n; k++) Xrows[k] = X.ViewRow(k);

            // Solve L*Y = B;
            for (int k = 0; k < n; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    // X[i,j] -= X[k,j]*L[i,k]
                    Xrows[i].Assign(Xrows[k], F2.MinusMult(mL[i, k]));
                }
                Xrows[k].Assign(F1.Div(mL[k, k]));
            }

            // Solve L'*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                Xrows[k].Assign(F1.Div(mL[k, k]));
                for (int i = 0; i < k; i++)
                {
                    // X[i,j] -= X[k,j]*L[k,i]
                    Xrows[i].Assign(Xrows[k], F2.MinusMult(mL[k, i]));
                }
            }
            return X;
        }

        /// <summary>
        /// Returns a String with (propertyName, propertyValue) pairs.
        /// Useful for debugging or to quickly get the rough picture.
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// rank          : 3
        /// trace         : 0
        /// </example>
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            String unknown = "Illegal operation or error: ";

            buf.Append("--------------------------------------------------------------------------\n");
            buf.Append("CholeskyDecomposition(A) --> isSymmetricPositiveDefinite(A), L, inverse(A)\n");
            buf.Append("--------------------------------------------------------------------------\n");

            buf.Append("isSymmetricPositiveDefinite = ");
            try { buf.Append(this.IsSymmetricPositiveDefinite.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\nL = ");
            try { buf.Append(this.L.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\ninverse(A) = ");
            try { buf.Append(this.Solve(Cern.Colt.Matrix.DoubleFactory2D.Dense.Identity(mL.Rows)).ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            return buf.ToString();
        }
    }
}
