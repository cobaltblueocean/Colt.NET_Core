// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by Cern.Colt.Matrix.LinearAlgebra Inc.
//
// Copyright (C) 2012 - present by Cern.Colt.Matrix.LinearAlgebra Incd and the Cern.Colt.Matrix.LinearAlgebra group of companies
//
// Please see distribution for license.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//     
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.LinearAlgebra
{
    /// <summary>
    /// LUDecompositionQuick Description
    /// </summary>
    public class LUDecompositionQuick
    {

        #region Local Variables
        protected DoubleMatrix2D _lu;
        protected int pivsign;
        protected int[] piv;

        protected Boolean isNonSingular;

        //protected Algebra algebra;

        protected double[] workDouble;
        protected int[] work1;
        protected int[] work2;
        #endregion

        #region Property
        public DoubleMatrix2D L
        {
            get { return lowerTriangular(_lu.Copy()); }
        }

        public DoubleMatrix2D LU
        {
            get { return _lu.Copy();}
            set { _lu = value; }
        }

        public int[] Pivot
        {
            get { return piv; }
        }

        public DoubleMatrix2D U
        {
            get { return upperTriangular(_lu.Copy()); }
        }

        public Boolean IsNonsingular
        {
            get { return isNonSingular; }
        }

        protected int M
        {
            get { return LU.Rows; }
        }

        protected int N
        {
            get { return LU.Columns; }
        }
        #endregion

        #region Constructor
        public LUDecompositionQuick() : this(Property.DEFAULT.Tolerance)
        {

        }

        public LUDecompositionQuick(double tolerance)
        {
            //this.algebra = new Algebra(tolerance);
        }
        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        protected static Boolean isNonsingular(DoubleMatrix2D matrix)
        {
            int m = matrix.Rows;
            int n = matrix.Columns;
            double epsilon = Double.Epsilon; // consider numerical instability
            for (int j = Math.Min(n, m); --j >= 0;)
            {
                //if (matrix[j,j) == 0) return false;
                if (Math.Abs(matrix[j, j]) <= epsilon) return false;
            }
            return true;
        }

        public void decompose(DoubleMatrix2D A)
        {
            int CUT_OFF = 10;
            // setup
            LU = A;
            int m = A.Rows;
            int n = A.Columns;

            // setup pivot vector
            if (this.piv == null || this.piv.Length != m) this.piv = new int[m];
            for (int i = m; --i >= 0;) piv[i] = i;
            pivsign = 1;

            if (m * n == 0)
            {
                LU = LU;
                return; // nothing to do
            }

            //precompute and cache some views to avoid regenerating them time and again
            DoubleMatrix1D[] LUrows = new DoubleMatrix1D[m];
            for (int i = 0; i < m; i++) LUrows[i] = LU.ViewRow(i);

            List<int> nonZeroIndexes = new List<int>(); // sparsity
            DoubleMatrix1D LUcolj = LU.ViewColumn(0).Like();  // blocked column j
            Cern.Jet.Math.Mult multFunction = Cern.Jet.Math.Mult.CreateInstance(0);

            // Outer loop.
            for (int j = 0; j < n; j++)
            {
                // blocking (make copy of j-th column to localize references)
                LUcolj.Assign(LU.ViewColumn(j));

                // sparsity detection
                int maxCardinality = m / CUT_OFF; // == heuristic depending on speedup
                LUcolj.GetNonZeros(nonZeroIndexes, null, maxCardinality);
                int cardinality = nonZeroIndexes.Count;
                Boolean sparse = (cardinality < maxCardinality);

                // Apply previous transformations.
                for (int i = 0; i < m; i++)
                {
                    int kmax = System.Math.Min(i, j);
                    double s;
                    if (sparse)
                    {
                        s = LUrows[i].ZDotProduct(LUcolj, 0, kmax, nonZeroIndexes);
                    }
                    else
                    {
                        s = LUrows[i].ZDotProduct(LUcolj, 0, kmax);
                    }
                    double before = LUcolj[i];
                    double after = before - s;
                    LUcolj[i] = after; // LUcolj is a copy
                    LU[i, j]= after;   // this is the original
                    if (sparse)
                    {
                        if (before == 0 && after != 0)
                        { // nasty bug fixed!
                            int pos = nonZeroIndexes.BinarySearch(i);
                            pos = -pos - 1;
                            nonZeroIndexes.Insert(pos, i);
                        }
                        if (before != 0 && after == 0)
                        {
                            nonZeroIndexes.Remove(nonZeroIndexes.BinarySearch(i));
                        }
                    }
                }

                // Find pivot and exchange if necessary.
                int p = j;
                if (p < m)
                {
                    double max = System.Math.Abs(LUcolj[p]);
                    for (int i = j + 1; i < m; i++)
                    {
                        double v = System.Math.Abs(LUcolj[i]);
                        if (v > max)
                        {
                            p = i;
                            max = v;
                        }
                    }
                }
                if (p != j)
                {
                    LUrows[p].Swap(LUrows[j]);
                    int k = piv[p]; piv[p] = piv[j]; piv[j] = k;
                    pivsign = -pivsign;
                }

                // Compute multipliers.
                double jj;
                if (j < m && (jj = LU[j, j]) != 0.0)
                {
                    multFunction.Multiplicator = 1 / jj;
                    LU.ViewColumn(j).ViewPart(j + 1, m - (j + 1)).Assign(multFunction);
                }

            }
            LU = LU;
        }

        public void decompose(DoubleMatrix2D A, int semiBandwidth)
        {
            if (!A.IsSquare || semiBandwidth < 0 || semiBandwidth > 2)
            {
                decompose(A);
                return;
            }
            // setup
            LU = A;
            int m = A.Rows;
            int n = A.Columns;

            // setup pivot vector
            if (this.piv == null || this.piv.Length != m) this.piv = new int[m];
            for (int i = m; --i >= 0;) piv[i] = i;
            pivsign = 1;

            if (m * n == 0)
            {
                LU = A;
                return; // nothing to do
            }

            //if (semiBandwidth == 1) { // A is diagonal; nothing to do
            if (semiBandwidth == 2)
            { // A is tridiagonal
              // currently no pivoting !
                if (n > 1) A[1, 0] =  A[1, 0] / A[0, 0];

                for (int i = 1; i < n; i++)
                {
                    double ei = A[i, i] - A[i, i - 1] * A[i - 1, i];
                    A[i, i] = ei;
                    if (i < n - 1) A[i + 1, i] = A[i + 1, i] / ei;
                }
            }
            LU = A;
        }

        public double det()
        {
            int m = M;
            int n = N;
            if (m != n) throw new ArgumentException("Matrix must be square.");

            if (!IsNonsingular) return 0; // avoid rounding errors

            double det = (double)pivsign;
            for (int j = 0; j < n; j++)
            {
                det *= LU[j, j];
            }
            return det;
        }

        public void solve(DoubleMatrix1D B)
        {
            //algebra.property().checkRectangular(LU);
           
            int m = M;
            int n = N;
            if (B.Count() != m) throw new ArgumentException("Matrix dimensions must agree.");
            if (!this.IsNonsingular) throw new ArgumentException("Matrix is singular.");


            // right hand side with pivoting
            // Matrix Xmat = B.getMatrix(piv,0,nx-1);
            if (this.workDouble == null || this.workDouble.Length < m) this.workDouble = new double[m];
            Algebra.Permute(B, this.piv, this.workDouble);

            if (m * n == 0) return; // nothing to do

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < n; k++)
            {
                double f = B[k];
                if (f != 0)
                {
                    for (int i = k + 1; i < n; i++)
                    {
                        // B[i] -= B[k]*LU[i][k];
                        double v = LU[i, k];
                        if (v != 0) B[i] = B[i] - f * v;
                    }
                }
            }

            // Solve U*B = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                // B[k] /= LU[k,k] 
                B[k] = B[k] / LU[k, k];
                double f = B[k];
                if (f != 0)
                {
                    for (int i = 0; i < k; i++)
                    {
                        // B[i] -= B[k]*LU[i][k];
                        double v = LU[i, k];
                        if (v != 0) B[i]  = B[i] - f * v;
                    }
                }
            }
        }

        public void solve(DoubleMatrix2D B)
        {
            int CUT_OFF = 10;
            //algebra.property().checkRectangular(LU);
            int m = M;
            int n = N;
            if (B.Rows != m) throw new ArgumentException("Matrix row dimensions must agree.");
            if (!this.IsNonsingular) throw new ArgumentException("Matrix is singular.");


            // right hand side with pivoting
            // Matrix Xmat = B.getMatrix(piv,0,nx-1);
            if (this.work1 == null || this.work1.Length < m) this.work1 = new int[m];
            //if (this.work2 == null || this.work2.Length < m) this.work2 = new int[m];
            Algebra.PermuteRows(B, this.piv, this.work1);

            if (m * n == 0) return; // nothing to do
            int nx = B.Columns;

            //precompute and cache some views to avoid regenerating them time and again
            DoubleMatrix1D[] Brows = new DoubleMatrix1D[n];
            for (int k = 0; k < n; k++) Brows[k] = B.ViewRow(k);

            // transformations
           Cern.Jet.Math.Mult div =Cern.Jet.Math.Mult.Div(0);
           Cern.Jet.Math.PlusMult minusMult =Cern.Jet.Math.PlusMult.MinusMult(0);

            List<int> nonZeroIndexes = new List<int>(); // sparsity
            DoubleMatrix1D Browk = Cern.Colt.Matrix.DoubleFactory1D.Dense.Make(nx); // blocked row k

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < n; k++)
            {
                // blocking (make copy of k-th row to localize references)		
                Browk.Assign(Brows[k]);

                // sparsity detection
                int maxCardinality = nx / CUT_OFF; // == heuristic depending on speedup
                Browk.GetNonZeros(nonZeroIndexes, null, maxCardinality);
                int cardinality = nonZeroIndexes.Count;
                Boolean sparse = (cardinality < maxCardinality);

                for (int i = k + 1; i < n; i++)
                {
                    //for (int j = 0; j < nx; j++) B[i][j] -= B[k][j]*LU[i][k];
                    //for (int j = 0; j < nx; j++) B.set(i,j, B.Get(i,j) - B.Get(k,j)*LU.Get(i,k));

                    minusMult.Multiplicator = -LU[i, k];
                    if (minusMult.Multiplicator != 0)
                    {
                        if (sparse)
                        {
                            Brows[i].Assign(Browk, minusMult, nonZeroIndexes);
                        }
                        else
                        {
                            Brows[i].Assign(Browk, minusMult);
                        }
                    }
                }
            }

            // Solve U*B = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                // for (int j = 0; j < nx; j++) B[k][j] /= LU[k][k];
                // for (int j = 0; j < nx; j++) B.set(k,j, B.Get(k,j) / LU.Get(k,k));
                div.Multiplicator = 1 / LU[k, k];
                Brows[k].Assign(div);

                // blocking
                if (Browk == null) Browk = Cern.Colt.Matrix.DoubleFactory1D.Dense.Make(B.Columns);
                Browk.Assign(Brows[k]);

                // sparsity detection
                int maxCardinality = nx / CUT_OFF; // == heuristic depending on speedup
                Browk.GetNonZeros(nonZeroIndexes, null, maxCardinality);
                int cardinality = nonZeroIndexes.Count;
                Boolean sparse = (cardinality < maxCardinality);

                //Browk.GetNonZeros(nonZeroIndexes,null);
                //Boolean sparse = nonZeroIndexes.Count < nx/10;

                for (int i = 0; i < k; i++)
                {
                    // for (int j = 0; j < nx; j++) B[i][j] -= B[k][j]*LU[i][k];
                    // for (int j = 0; j < nx; j++) B.set(i,j, B.Get(i,j) - B.Get(k,j)*LU.Get(i,k));

                    minusMult.Multiplicator = -LU[i, k];
                    if (minusMult.Multiplicator != 0)
                    {
                        if (sparse)
                        {
                            Brows[i].Assign(Browk, minusMult, nonZeroIndexes);
                        }
                        else
                        {
                            Brows[i].Assign(Browk, minusMult);
                        }
                    }
                }
            }
        }

        public String ToString()
        {
            StringBuilder buf = new StringBuilder();
            String unknown = "Illegal operation or error: ";

            buf.Append("-----------------------------------------------------------------------------\n");
            buf.Append("LUDecompositionQuick(A) --> isNonSingular(A), det(A), pivot, L, U, inverse(A)\n");
            buf.Append("-----------------------------------------------------------------------------\n");

            buf.Append("isNonSingular = ");
            try { buf.Append(this.IsNonsingular.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\ndet = ");
            try { buf.Append(this.det().ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\npivot = ");
            try { buf.Append((new List<int>(this.Pivot)).ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\nL = ");
            try { buf.Append(this.L.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\nU = ");
            try { buf.Append(this.U.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            buf.Append("\n\ninverse(A) = ");
            DoubleMatrix2D identity = Cern.Colt.Matrix.DoubleFactory2D.Dense.Identity(LU.Rows);
            try { this.solve(identity); buf.Append(identity.ToString()); }
            catch (ArgumentException exc) { buf.Append(unknown + exc.Message); }

            return buf.ToString();
        }

        #endregion

        #region Local Protected Methods

        protected double[] getDoublePivot()
        {
            int m = M;
            double[] vals = new double[m];
            for (int i = 0; i < m; i++)
            {
                vals[i] = (double)piv[i];
            }
            return vals;
        }

        protected DoubleMatrix2D lowerTriangular(DoubleMatrix2D A)
        {
            int rows = A.Rows;
            int columns = A.Columns;
            int min = System.Math.Min(rows, columns);
            for (int r = min; --r >= 0;)
            {
                for (int c = min; --c >= 0;)
                {
                    if (r < c) A[r, c] = 0;
                    else if (r == c) A[r, c] = 1;
                }
            }
            if (columns > rows) A.ViewPart(0, min, rows, columns - min).Assign(0);

            return A;
        }

        protected DoubleMatrix2D upperTriangular(DoubleMatrix2D A)
        {
            int rows = A.Rows;
            int columns = A.Columns;
            int min = System.Math.Min(rows, columns);
            for (int r = min; --r >= 0;)
            {
                for (int c = min; --c >= 0;)
                {
                    if (r > c) A[r, c] = 0;
                }
            }
            if (columns < rows) A.ViewPart(min, 0, rows - min, columns).Assign(0);

            return A;
        }

        #endregion

        #region Local Private Methods
        private double[] xgetDoublePivot()
        {
            int m = M;
            double[] vals = new double[m];
            for (int i = 0; i < m; i++)
            {
                vals[i] = (double)piv[i];
            }
            return vals;
        }
        #endregion

    }
}
