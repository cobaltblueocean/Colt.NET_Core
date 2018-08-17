// <copyright file="QRDecomposition.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
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

namespace Cern.Colt.Matrix.LinearAlgebra
{
    using F1 = Cern.Colt.Function.BinaryFunctions;
    using F2 = Cern.Colt.Function.UnaryFunctions;

    [Serializable]
    public class QRDecomposition
    {
        /// <summary>
        /// Array for internal storage of decomposition.
        /// @serial internal array storage.
        /// </summary>
        private DoubleMatrix2D QR;
        //private double[][] QR;

        /// <summary>
        /// Row and column dimensions.
        /// @serial column dimension.
        /// @serial row dimension.
        /// </summary>
        private int m, n;

        /// <summary>
        /// Array for internal storage of diagonal of R.
        /// @serial diagonal of R.
        /// </summary>
        private DoubleMatrix1D Rdiag;

        /// <summary>
        /// Constructs and returns a new QR decomposition object;  computed by Householder reflections;
        /// The decomposed matrices can be retrieved via instance methods of the returned decomposition object.
        /// Return a decomposition object to access <i>R</i> and the Householder vectors <i>H</i>, and to compute <i>Q</i>.
        /// </summary>
        /// <param name="A">A rectangular matrix.</param>
        /// <exception cref="ArgumentException">if <i>A.Rows &lt; A.Columns</i>.</exception>
        public QRDecomposition(DoubleMatrix2D A)
        {
            Property.DEFAULT.CheckRectangular(A);

            Functions F = Functions.functions;
            // Initialize.
            QR = A.Copy();
            m = A.Rows;
            n = A.Columns;
            Rdiag = A.Like1D(n);
            //Rdiag = new double[n];
            DoubleDoubleFunction hypot = Algebra.HypotFunction();

            // precompute and cache some views to avoid regenerating them time and again
            DoubleMatrix1D[] QRcolumns = new DoubleMatrix1D[n];
            DoubleMatrix1D[] QRcolumnsPart = new DoubleMatrix1D[n];
            for (int k = 0; k < n; k++)
            {
                QRcolumns[k] = QR.ViewColumn(k);
                QRcolumnsPart[k] = QR.ViewColumn(k).ViewPart(k, m - k);
            }

            // Main loop.
            for (int k = 0; k < n; k++)
            {
                //DoubleMatrix1D QRcolk = QR.ViewColumn(k).ViewPart(k,m-k);
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                //if (k<m) nrm = QRcolumnsPart[k].aggregate(hypot,F.identity);

                for (int i = k; i < m; i++)
                { // fixes bug reported by hong.44@osu.edu
                    nrm = Algebra.Hypot(nrm, QR[i, k]);
                }


                if (nrm != 0.0)
                {
                    // Form k-th Householder vector.
                    if (QR[k, k] < 0) nrm = -nrm;
                    QRcolumnsPart[k].Assign(F2.Div(nrm));
                    /*
                    for (int i = k; i < m; i++) {
                       QR[i][k] /= nrm;
                    }
                    */

                    QR[k, k] = QR[k, k] + 1;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < n; j++)
                    {
                        DoubleMatrix1D QRcolj = QR.ViewColumn(j).ViewPart(k, m - k);
                        double s = QRcolumnsPart[k].ZDotProduct(QRcolj);
                        /*
                        // fixes bug reported by John Chambers
                        DoubleMatrix1D QRcolj = QR.ViewColumn(j).ViewPart(k,m-k);
                        double s = QRcolumnsPart[k].ZDotProduct(QRcolumns[j]);
                        double s = 0.0; 
                        for (int i = k; i < m; i++) {
                          s += QR[i][k]*QR[i][j];
                        }
                        */
                        s = -s / QR[k, k];
                        //QRcolumnsPart[j].Assign(QRcolumns[k], F.PlusMult(s));

                        for (int i = k; i < m; i++)
                        {
                            QR[i, j] = QR[i, j] + s * QR[i, k];
                        }

                    }
                }
                Rdiag[k] = -nrm;
            }
        }

        /// <summary>
        /// Returns the Householder vectors <i>H</i> that a lower trapezoidal matrix whose columns define the householder reflections.
        /// </summary>
        public DoubleMatrix2D H
        {
            get
            {
                return Algebra.TrapezoidalLower(QR.Copy());
            }
        }

        /// <summary>
        /// Generates and returns the (economy-sized) orthogonal factor <i>Q</i>.
        /// </summary>
        public DoubleMatrix2D Q
        {
            get
            {            //Functions F = Functions.functions;
                DoubleMatrix2D Q = QR.Like();
                //double[][] Q = X.getArray();
                for (int k = n - 1; k >= 0; k--)
                {
                    DoubleMatrix1D QRcolk = QR.ViewColumn(k).ViewPart(k, m - k);
                    Q[k, k] = 1;
                    for (int j = k; j < n; j++)
                    {
                        if (QR[k, k] != 0)
                        {
                            DoubleMatrix1D Qcolj = Q.ViewColumn(j).ViewPart(k, m - k);
                            double s = QRcolk.ZDotProduct(Qcolj);
                            s = -s / QR[k, k];
                            Qcolj.Assign(QRcolk, Functions.DoubleDoubleFunctions.PlusMult(s));
                        }
                    }
                }
                return Q;
            }
        }

        /// <summary>
        /// Returns the upper triangular factor, <i>R</i>.
        /// </summary>
        public DoubleMatrix2D R
        {
            get
            {
                DoubleMatrix2D R = QR.Like(n, n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i < j)
                            R[i, j] = QR[i, j];
                        else if (i == j)
                            R[i, j] = Rdiag[i];
                        else
                            R[i, j] = 0;
                    }
                }
                return R;
            }
        }

        /// <summary>
        /// Returns whether the matrix <i>A</i> has full rank.
        /// true if <i>R</i>, and hence <i>A</i>, has full rank.
        /// </summary>
        public Boolean HasFullRank
        {
            get
            {
                for (int j = 0; j < n; j++)
                {
                    if (Rdiag[j] == 0) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Least squares solution of <i>A*X = B</i>; <i>returns X</i>.
        /// </summary>
        /// <param name="B">A matrix with as many rows as <i>A</i> and any number of columns.</param>
        /// <returns><i>X</i> that minimizes the two norm of <i>Q*R*X - B</i>.</returns>
        /// <exception cref="ArgumentException">if <i>B.Rows != A.Rows</i>.</exception>
        /// <exception cref="ArgumentException">if <i>!this.hasFullRank()</i> (<i>A</i> is rank deficient).</exception>
        public DoubleMatrix2D Solve(DoubleMatrix2D B)
        {
            Functions F = Functions.functions;
            if (B.Rows != m)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!this.HasFullRank)
            {
                throw new ArgumentException("Matrix is rank deficient.");
            }

            // Copy right hand side
            int nx = B.Columns;
            DoubleMatrix2D X = B.Copy();

            // Compute Y = transpose(Q)*B
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < nx; j++)
                {
                    double s = 0.0;
                    for (int i = k; i < m; i++)
                    {
                        s += QR[i, k] * X[i, j];
                    }
                    s = -s / QR[k, k];
                    for (int i = k; i < m; i++)
                    {
                        X[i, j] = X[i, j] + s * QR[i, k];
                    }
                }
            }
            // Solve R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                {
                    X[k, j] = X[k, j] / Rdiag[k];
                }
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[i, j] = X[i, j] - X[k, j] * QR[i, k];
                    }
                }
            }
            return X.ViewPart(0, 0, n, nx);
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

            buf.Append("-----------------------------------------------------------------\n");
            buf.Append("QRDecomposition(A) --> hasFullRank(A), H, Q, R, pseudo inverse(A)\n");
            buf.Append("-----------------------------------------------------------------\n");

            buf.Append("hasFullRank = ");
            try { buf.Append(this.HasFullRank.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\nH = ");
            try { buf.Append(this.H.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\nQ = ");
            try { buf.Append(this.Q.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\nR = ");
            try { buf.Append(this.R.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\npseudo inverse(A) = ");
            try { buf.Append(this.Solve(Cern.Colt.Matrix.DoubleFactory2D.Dense.Identity(QR.Rows)).ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            return buf.ToString();
        }
    }
}
