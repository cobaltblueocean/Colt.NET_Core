// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingularValueDecomposition.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   For an <tt>m x n</tt> matrix <tt>A</tt> with <tt>m &gt;= n</tt>, the singular value decomposition is an <tt>m x n</tt> orthogonal matrix <tt>U</tt>, an <tt>n x n</tt> diagonal matrix <tt>S</tt>, and an <tt>n x n</tt> orthogonal matrix <tt>V</tt> so that <tt>A = U*S*V'</tt>.
//   <para> The singular values, <tt>sigma[k] = S[k][k]</tt>, are ordered so that <tt>sigma[0] &gt;= sigma[1] &gt;= ... &gt;= sigma[n-1]</tt>.</para>
//   <para> The singular value decomposition always exists, so the constructor will never fail.  The matrix condition number and the effective numerical rank can be computed from this decomposition.</para>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.LinearAlgebra
{
    using System;
    using System.Text;

    using Cern.Colt.Matrix.Implementation;

    using Function;

    /// <summary>
    /// For an <tt>m x n</tt> matrix <tt>A</tt> with <tt>m >= n</tt>, the singular value decomposition is an <tt>m x n</tt> orthogonal matrix <tt>U</tt>, an <tt>n x n</tt> diagonal matrix <tt>S</tt>, and an <tt>n x n</tt> orthogonal matrix <tt>V</tt> so that <tt>A = U*S*V'</tt>.///
    /// <para> The singular values, <tt>sigma[k] = S[k][k]</tt>, are ordered so that <tt>sigma[0] >= sigma[1] >= ... >= sigma[n-1]</tt>.</para>
    /// <para> The singular value decomposition always exists, so the constructor will never fail.  The matrix condition number and the effective numerical rank can be computed from this decomposition.</para>
    /// </summary>
    public class SingularValueDecomposition
    {
        /// <summary>
        /// Array for internal storage of U
        /// </summary>
        private DoubleMatrix2D _u;

        /// <summary>
        /// Array for internal storage of V
        /// </summary>
        private DoubleMatrix2D _v;

        /// <summary>
        /// Array for internal storage of singular values.
        /// </summary>
        private double[] _s;

        /// <summary>
        /// Row dimension.
        /// </summary>
        private int _m;

        /// <summary>
        /// Column dimension.
        /// </summary>
        private int _n;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularValueDecomposition"/> class with a vector (the first column).
        /// </summary>
        /// <param name="arg">
        /// A vector.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <code>a.Rows &lt; a.Columns</code>.
        /// </exception>
        public SingularValueDecomposition(DoubleMatrix1D arg)
        {
            batchSVD(DoubleFactory2D.Dense.Make(arg.ToArray(), arg.Size()), true, true, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularValueDecomposition"/> class. 
        /// The decomposed matrices can be retrieved via instance methods of the returned decomposition object.
        /// </summary>
        /// <param name="arg">
        /// A rectangular matrix.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <code>a.Rows &lt; a.Columns</code>.
        /// </exception>
        public SingularValueDecomposition(DoubleMatrix2D arg)
        {
            batchSVD(arg, true, true, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularValueDecomposition"/> class from a previous decomposition.
        /// </summary>
        /// <param name="u">
        /// The matrix U.
        /// </param>
        /// <param name="s">
        /// The vector of singular values.
        /// </param>
        /// <param name="v">
        /// The matrix V. Can be <code>null</code>.
        /// </param>
        public SingularValueDecomposition(DoubleMatrix2D u, double[] s, DoubleMatrix2D v)
        {
            _u = u;
            _s = s;
            _v = v;
            _m = u.Rows;
            _n = s.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularValueDecomposition"/> class. 
        /// The decomposed matrices can be retrieved via instance methods of the returned decomposition object.
        /// </summary>
        /// <param name="arg">
        /// A rectangular matrix.
        /// </param>
        /// <param name="wantU">
        /// Whether the matrix U is needed.
        /// </param>
        /// <param name="wantV">
        /// Whether the matrix V is needed.
        /// </param>
        /// <param name="order">
        /// whether the singular values must be ordered.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If 
        /// <code>
        /// a.Rows &lt; a.Columns
        /// </code>
        /// .
        /// </exception>
        public SingularValueDecomposition(DoubleMatrix2D arg, bool wantU, bool wantV, bool order)
        {
            batchSVD(arg, wantU, wantV, order);
        }

        /// <summary>
        /// Returns the two norm condition number, which is <tt>max(S) / min(S)</tt>.
        /// </summary>
        /// <returns>
        /// The two norm condition number, which is <tt>max(S) / min(S)</tt>.
        /// </returns>
        public double Cond()
        {
            return _s[0] / _s[Math.Min(_m, _n) - 1];
        }

        /// <summary>
        /// Encode a vector in the space spanned by columns of U.
        /// </summary>
        /// <param name="d">
        /// The vector d.
        /// </param>
        /// <returns>
        /// U'd
        /// </returns>
        public DoubleMatrix1D Encode(DoubleMatrix1D d)
        {
            return _u.ZMult(d.Size() < _m ? DoubleFactory1D.Sparse.AppendColumns(d, DoubleFactory1D.Sparse.Make(_m - d.Size())) : d, null, 1, 0, true);
        }

        /// <summary>
        /// Returns the diagonal matrix of singular values.
        /// </summary>
        /// <returns>
        /// S
        /// </returns>
        public DoubleMatrix2D GetS()
        {
            return DoubleFactory2D.Sparse.Diagonal(DoubleFactory1D.Dense.Make(_s));
        }

        /// <summary>
        /// Returns the diagonal of <tt>S</tt>, which is a one-dimensional array of singular values.
        /// </summary>
        /// <returns>
        /// Diagonal of <tt>S</tt>.
        /// </returns>
        public double[] GetSingularValues()
        {
            return _s;
        }

        /// <summary>
        /// Returns the left singular vectors <tt>U</tt>.
        /// </summary>
        /// <returns>
        /// <tt>U</tt>
        /// </returns>
        public DoubleMatrix2D GetU()
        {
            return _u.ViewPart(0, 0, _m, Math.Min(_m + 1, _n));
        }

        /// <summary>
        /// Returns the right singular vectors <tt>V</tt>.
        /// </summary>
        /// <returns>
        /// <tt>V</tt>
        /// </returns>
        public DoubleMatrix2D GetV()
        {
            return _v;
        }

        /// <summary>
        /// Returns the two norm, which is <tt>max(S)</tt>.
        /// </summary>
        /// <returns>
        /// The two norm.
        /// </returns>
        public double Norm2()
        {
            return _s[0];
        }

        /// <summary>
        /// Returns the effective numerical matrix rank, which is the number of nonnegligible singular values.
        /// </summary>
        /// <returns>
        /// The effective numerical matrix rank.
        /// </returns>
        public int Rank()
        {
            double eps = Math.Pow(2.0, -52.0);
            double tol = Math.Max(_m, _n) * _s[0] * eps;
            int r = 0;
            for (int i = 0; i < _s.Length; i++)
                if (_s[i] > tol)
                    r++;
            return r;
        }

        /// <summary>
        /// Reduce the SVD according to the numerical rank of A.
        /// Discard negligible singular values and corresponding columns of U and V.
        /// </summary>
        public void Reduce()
        {
            Reduce(Rank());
        }

        /// <summary>
        /// Reduced rank-<code>k</code> decomposition.
        /// Assume that the singular values are ordered.
        /// Discard the lowest singular values and the corresponding columns of U and V.
        /// </summary>
        /// <param name="r">The reduced rank.</param>
        public void Reduce(int r)
        {
            if (r < _n)
            {
                _u = _u.ViewPart(0, 0, _u.Rows, r);
                if (_v != null)
                    _v = _v.ViewPart(0, 0, _v.Rows, r);
                var s = new double[r];
                Array.Copy(_s, s, r);
                _s = s;
                _n = r;
            }
        }

        /// <summary>
        /// Returns a string with (propertyName, propertyValue) pairs.
        /// Useful for debugging or to quickly get the rough picture.
        /// </summary>
        /// <returns>
        /// A string with (propertyName, propertyValue) pairs.
        /// </returns>
        public override string ToString()
        {
            var buf = new StringBuilder();
            const string Unknown = "Illegal operation or error: ";

            buf.Append("---------------------------------------------------------------------\n");
            buf.Append("SingularValueDecomposition(A) --> cond(A), rank(A), norm2(A), U, S, V\n");
            buf.Append("---------------------------------------------------------------------\n");

            buf.Append("cond = ");
            try
            {
                buf.Append(Cond().ToString());
            }
            catch (ArgumentException exc)
            {
                buf.Append(Unknown + exc.Message);
            }

            buf.Append("\nrank = ");
            try
            {
                buf.Append(Rank().ToString());
            }
            catch (ArgumentException exc)
            {
                buf.Append(Unknown + exc.Message);
            }

            buf.Append("\nnorm2 = ");
            try
            {
                buf.Append(Norm2().ToString());
            }
            catch (ArgumentException exc)
            {
                buf.Append(Unknown + exc.Message);
            }

            buf.Append("\n\nU = ");
            try
            {
                buf.Append(GetU().ToString());
            }
            catch (ArgumentException exc)
            {
                buf.Append(Unknown + exc.Message);
            }

            buf.Append("\n\nS = ");
            try
            {
                buf.Append(GetS().ToString());
            }
            catch (ArgumentException exc)
            {
                buf.Append(Unknown + exc.Message);
            }

            buf.Append("\n\nV = ");
            try
            {
                buf.Append(GetV().ToString());
            }
            catch (ArgumentException exc)
            {
                buf.Append(Unknown + exc.Message);
            }

            return buf.ToString();
        }

        /// <summary>
        /// Update the SVD with the addition of a new column.
        /// </summary>
        /// <param name="c">
        /// The new column.
        /// </param>
        public void Update(DoubleMatrix1D c)
        {
            Update(c, true);
        }

        /// <summary>
        /// Update the SVD with the addition of a new column.
        /// </summary>
        /// <param name="c">
        /// The new column.
        /// </param>
        /// <param name="wantV">
        /// Whether the matrix V is needed.
        /// </param>
        public void Update(DoubleMatrix1D c, bool wantV)
        {
            int nRows = c.Size() - _m;
            if (nRows > 0)
            {
                _u = DoubleFactory2D.Dense.AppendRows(_u, new SparseDoubleMatrix2D(nRows, _n));
                _m = c.Size();
            }
            else if (nRows < 0)
            {
                c = DoubleFactory1D.Sparse.AppendColumns(c, DoubleFactory1D.Sparse.Make(-nRows));
            }

            var d = DoubleFactory2D.Dense.Make(c.ToArray(), c.Size());

            // l = U'd is the eigencoding of d
            var l = _u.ViewDice().ZMult(d, null);

            ////var uu = _u.ZMult(_u.ViewDice(), null);

            // Ul = UU'd
            ////var ul = uu.ZMult(d, null);
            var ul = _u.ZMult(l, null);

            // h = d - UU'd = d - Ul is the component of d orthogonal to the subspace spanned by U
            ////var h = d.Copy().Assign(uu.ZMult(d, null), BinaryFunctions.Minus);
            ////var h = d.Copy().Assign(ul, BinaryFunctions.Minus);

            // k is the projection of d onto the subspace othogonal to U
            var k = Math.Sqrt(d.Aggregate(BinaryFunctions.Plus, a => a * a) - (2 * l.Aggregate(BinaryFunctions.Plus, a => a * a)) + ul.Aggregate(BinaryFunctions.Plus, a => a * a));

            // truncation
            if (k == 0 || double.IsNaN(k)) return;

            _n++;

            // j = d - UU'd = d - Ul is an orthogonal basis for the component of d orthogonal to the subspace spanned by U
            ////var j = h.Assign(UnaryFunctions.Div(k));
            var j = d.Assign(ul, BinaryFunctions.Minus).Assign(UnaryFunctions.Div(k));

            // Q = [ S, l; 0, ||h||]
            var q =
                DoubleFactory2D.Sparse.Compose(
                    new[] { new[] { GetS(), l }, new[] { null, DoubleFactory2D.Dense.Make(1, 1, k) } });

            var svdq = new SingularValueDecomposition(q, true, wantV, true);
            _u = DoubleFactory2D.Dense.AppendColumns(_u, j).ZMult(svdq.GetU(), null);
            _s = svdq.GetSingularValues();
            if (wantV)
                _v = DoubleFactory2D.Dense.ComposeDiagonal(_v, DoubleFactory2D.Dense.Identity(1)).ZMult(
                    svdq.GetV(), null);
        }

        /// <summary>
        /// Batch decomposition of the given matrix.
        /// The decomposed matrices can be retrieved via instance methods.
        /// </summary>
        /// <param name="arg">
        /// A rectangular matrix.
        /// </param>
        /// <param name="wantU">
        /// Whether the matrix U is needed.
        /// </param>
        /// <param name="wantV">
        /// Whether the matrix V is needed.
        /// </param>
        /// <param name="order">
        /// whether the singular values must be ordered.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If 
        /// <code>
        /// a.Rows &lt; a.Columns
        /// </code>
        /// .
        /// </exception>
        private void batchSVD(DoubleMatrix2D arg, bool wantU, bool wantV, bool order)
        {
            Property.CheckRectangular(arg);

            // Derived from LINPACK code.
            // Initialize.
            double[][] a = arg.ToArray();
            _m = arg.Rows;
            _n = arg.Columns;
            int nu = Math.Min(_m, _n);
            _s = new double[Math.Min(_m + 1, _n)];
            _u = new DenseDoubleMatrix2D(_m, nu);
            if (wantV)
                _v = new DenseDoubleMatrix2D(_n, _n);

            var e = new double[_n];
            var work = new double[_m];

            // Reduce A to bidiagonal form, storing the diagonal elements
            // in s and the super-diagonal elements in e.
            int nct = Math.Min(_m - 1, _n);
            int nrt = Math.Max(0, Math.Min(_n - 2, _m));
            for (int k = 0; k < Math.Max(nct, nrt); k++)
            {
                if (k < nct)
                {
                    // Compute the transformation for the k-th column and
                    // place the k-th diagonal in s[k].
                    // Compute 2-norm of k-th column without under/overflow.
                    _s[k] = 0;
                    for (int i = k; i < _m; i++)
                        _s[k] = Algebra.hypot(_s[k], a[i][k]);
                    if (_s[k] != 0.0)
                    {
                        if (a[k][k] < 0.0)
                            _s[k] = -_s[k];
                        for (int i = k; i < _m; i++)
                            a[i][k] /= _s[k];
                        a[k][k] += 1.0;
                    }

                    _s[k] = -_s[k];
                }

                for (int j = k + 1; j < _n; j++)
                {
                    if ((k < nct) & (_s[k] != 0.0))
                    {
                        // Apply the transformation.
                        double t = 0;
                        for (int i = k; i < _m; i++) t += a[i][k] * a[i][j];
                        t = -t / a[k][k];
                        for (int i = k; i < _m; i++) a[i][j] += t * a[i][k];
                    }

                    // Place the k-th row of A into e for the
                    // subsequent calculation of the row transformation.
                    e[j] = a[k][j];
                }

                if (wantU & (k < nct))
                {
                    // Place the transformation in U for subsequent back multiplication.
                    for (int i = k; i < _m; i++)
                    {
                        _u[i, k] = a[i][k];
                    }
                }

                if (k < nrt)
                {
                    // Compute the k-th row transformation and place the
                    // k-th super-diagonal in e[k].
                    // Compute 2-norm without under/overflow.
                    e[k] = 0;
                    for (int i = k + 1; i < _n; i++)
                        e[k] = Algebra.hypot(e[k], e[i]);
                    if (e[k] != 0.0)
                    {
                        if (e[k + 1] < 0.0)
                            e[k] = -e[k];
                        for (int i = k + 1; i < _n; i++)
                            e[i] /= e[k];
                        e[k + 1] += 1.0;
                    }

                    e[k] = -e[k];
                    if ((k + 1 < _m) & (e[k] != 0.0))
                    {
                        // Apply the transformation.
                        for (int i = k + 1; i < _m; i++)
                            work[i] = 0.0;
                        for (int j = k + 1; j < _n; j++)
                            for (int i = k + 1; i < _m; i++)
                                work[i] += e[j] * a[i][j];

                        for (int j = k + 1; j < _n; j++)
                        {
                            double t = -e[j] / e[k + 1];
                            for (int i = k + 1; i < _m; i++) a[i][j] += t * work[i];
                        }
                    }

                    if (wantV)
                    {
                        // Place the transformation in V for subsequent back multiplication.
                        for (int i = k + 1; i < _n; i++) _v[i, k] = e[i];
                    }
                }
            }

            // Set up the final bidiagonal matrix or order p.
            int p = Math.Min(_n, _m + 1);
            if (nct < _n)
                _s[nct] = a[nct][nct];
            if (_m < p)
                _s[p - 1] = 0.0;
            if (nrt + 1 < p)
                e[nrt] = a[nrt][p - 1];
            e[p - 1] = 0.0;

            // If required, generate U.
            if (wantU)
            {
                for (int j = nct; j < nu; j++)
                {
                    for (int i = 0; i < _m; i++)
                        _u[i, j] = 0.0;
                    _u[j, j] = 1.0;
                }

                for (int k = nct - 1; k >= 0; k--)
                {
                    if (_s[k] != 0.0)
                    {
                        for (int j = k + 1; j < nu; j++)
                        {
                            double t = 0;
                            for (int i = k; i < _m; i++) t += _u[i, k] * _u[i, j];
                            t = -t / _u[k, k];
                            for (int i = k; i < _m; i++) _u[i, j] += t * _u[i, k];
                        }

                        for (int i = k; i < _m; i++) _u[i, k] = -_u[i, k];
                        _u[k, k] = 1.0 + _u[k, k];
                        for (int i = 0; i < k - 1; i++) _u[i, k] = 0.0;
                    }
                    else
                    {
                        for (int i = 0; i < _m; i++)
                            _u[i, k] = 0.0;
                        _u[k, k] = 1.0;
                    }
                }
            }

            // If required, generate V.
            if (wantV)
            {
                for (int k = _n - 1; k >= 0; k--)
                {
                    if ((k < nrt) & (e[k] != 0.0))
                    {
                        for (int j = k + 1; j < nu; j++)
                        {
                            double t = 0;
                            for (int i = k + 1; i < _n; i++)
                                t += _v[i, k] * _v[i, j];
                            t = -t / _v[k + 1, k];
                            for (int i = k + 1; i < _n; i++)
                            {
                                _v[i, j] += t * _v[i, k];
                            }
                        }
                    }

                    for (int i = 0; i < _n; i++)
                        _v[i, k] = 0.0;
                    _v[k, k] = 1.0;
                }
            }

            // Main iteration loop for the singular values.
            int pp = p - 1;
            int iter = 0;
            double eps = Math.Pow(2.0, -52.0);
            while (p > 0)
            {
                int k, kase;

                // Here is where a test for too many iterations would go.

                // This section of the program inspects for
                // negligible elements in the s and e arrays.  On
                // completion the variables kase and k are set as follows.

                // kase = 1     if s(p) and e[k-1] are negligible and k<p
                // kase = 2     if s(k) is negligible and k<p
                // kase = 3     if e[k-1] is negligible, k<p, and
                //              s(k), ..., s(p) are not negligible (qr step).
                // kase = 4     if e(p-1) is negligible (convergence).
                for (k = p - 2; k >= -1; k--)
                {
                    if (k == -1)
                        break;
                    if (Math.Abs(e[k]) <= eps * (Math.Abs(_s[k]) + Math.Abs(_s[k + 1])))
                    {
                        e[k] = 0.0;
                        break;
                    }
                }

                if (k == p - 2)
                {
                    kase = 4;
                }
                else
                {
                    int ks;
                    for (ks = p - 1; ks >= k; ks--)
                    {
                        if (ks == k)
                            break;
                        double t = (ks != p ? Math.Abs(e[ks]) : 0) +
                                   (ks != k + 1 ? Math.Abs(e[ks - 1]) : 0);
                        if (Math.Abs(_s[ks]) <= eps * t)
                        {
                            _s[ks] = 0.0;
                            break;
                        }
                    }

                    if (ks == k)
                    {
                        kase = 3;
                    }
                    else if (ks == p - 1)
                    {
                        kase = 1;
                    }
                    else
                    {
                        kase = 2;
                        k = ks;
                    }
                }

                k++;

                // Perform the task indicated by kase.
                switch (kase)
                {
                    // Deflate negligible s(p).
                    case 1:
                        {
                            double f = e[p - 2];
                            e[p - 2] = 0.0;
                            for (int j = p - 2; j >= k; j--)
                            {
                                double t = Algebra.hypot(_s[j], f);
                                double cs = _s[j] / t;
                                double sn = f / t;
                                _s[j] = t;
                                if (j != k)
                                {
                                    f = -sn * e[j - 1];
                                    e[j - 1] = cs * e[j - 1];
                                }

                                if (wantV)
                                {
                                    for (int i = 0; i < _n; i++)
                                    {
                                        t = (cs * _v[i, j]) + (sn * _v[i, p - 1]);
                                        _v[i, p - 1] = -(sn * _v[i, j]) + (cs * _v[i, p - 1]);
                                        _v[i, j] = t;
                                    }
                                }
                            }
                        }

                        break;

                    // Split at negligible s(k).
                    case 2:
                        {
                            double f = e[k - 1];
                            e[k - 1] = 0.0;
                            for (int j = k; j < p; j++)
                            {
                                double t = Algebra.hypot(_s[j], f);
                                double cs = _s[j] / t;
                                double sn = f / t;
                                _s[j] = t;
                                f = -sn * e[j];
                                e[j] = cs * e[j];
                                if (wantU)
                                {
                                    for (int i = 0; i < _m; i++)
                                    {
                                        t = (cs * _u[i, j]) + (sn * _u[i, k - 1]);
                                        _u[i, k - 1] = -(sn * _u[i, j]) + (cs * _u[i, k - 1]);
                                        _u[i, j] = t;
                                    }
                                }
                            }
                        }

                        break;

                    // Perform one qr step.
                    case 3:
                        {
                            // Calculate the shift.
                            double scale =
                                Math.Max(
                                    Math.Max(
                                        Math.Max(Math.Max(Math.Abs(_s[p - 1]), Math.Abs(_s[p - 2])), Math.Abs(e[p - 2])),
                                        Math.Abs(_s[k])),
                                    Math.Abs(e[k]));
                            double sp = _s[p - 1] / scale;
                            double spm1 = _s[p - 2] / scale;
                            double epm1 = e[p - 2] / scale;
                            double sk = _s[k] / scale;
                            double ek = e[k] / scale;
                            double b = (((spm1 + sp) * (spm1 - sp)) + (epm1 * epm1)) / 2.0;
                            double c = (sp * epm1) * (sp * epm1);
                            double shift = 0.0;
                            if ((b != 0.0) | (c != 0.0))
                            {
                                shift = Math.Sqrt((b * b) + c);
                                if (b < 0.0)
                                    shift = -shift;
                                shift = c / (b + shift);
                            }

                            double f = ((sk + sp) * (sk - sp)) + shift;
                            double g = sk * ek;

                            // Chase zeros.
                            for (int j = k; j < p - 1; j++)
                            {
                                double t = Algebra.hypot(f, g);
                                double cs = f / t;
                                double sn = g / t;
                                if (j != k)
                                    e[j - 1] = t;
                                f = (cs * _s[j]) + (sn * e[j]);
                                e[j] = (cs * e[j]) - (sn * _s[j]);
                                g = sn * _s[j + 1];
                                _s[j + 1] = cs * _s[j + 1];
                                if (wantV)
                                {
                                    for (int i = 0; i < _n; i++)
                                    {
                                        t = (cs * _v[i, j]) + (sn * _v[i, j + 1]);
                                        _v[i, j + 1] = -(sn * _v[i, j]) + (cs * _v[i, j + 1]);
                                        _v[i, j] = t;
                                    }
                                }

                                t = Algebra.hypot(f, g);
                                cs = f / t;
                                sn = g / t;
                                _s[j] = t;
                                f = (cs * e[j]) + (sn * _s[j + 1]);
                                _s[j + 1] = -(sn * e[j]) + (cs * _s[j + 1]);
                                g = sn * e[j + 1];
                                e[j + 1] = cs * e[j + 1];
                                if (wantU && (j < _m - 1))
                                {
                                    for (int i = 0; i < _m; i++)
                                    {
                                        t = (cs * _u[i, j]) + (sn * _u[i, j + 1]);
                                        _u[i, j + 1] = -(sn * _u[i, j]) + (cs * _u[i, j + 1]);
                                        _u[i, j] = t;
                                    }
                                }
                            }

                            e[p - 2] = f;
                            iter = iter + 1;
                        }

                        break;

                    // Convergence.
                    case 4:
                        {
                            // Make the singular values positive.
                            if (_s[k] <= 0.0)
                            {
                                _s[k] = _s[k] < 0.0 ? -_s[k] : 0.0;
                                if (wantV)
                                {
                                    for (int i = 0; i <= pp; i++)
                                    {
                                        _v[i, k] = -_v[i, k];
                                    }
                                }
                            }

                            // Order the singular values.
                            if (order)
                                while (k < pp)
                                {
                                    if (_s[k] >= _s[k + 1])
                                        break;
                                    double t = _s[k];
                                    _s[k] = _s[k + 1];
                                    _s[k + 1] = t;
                                    if (wantV && (k < _n - 1))
                                    {
                                        for (int i = 0; i < _n; i++)
                                        {
                                            t = _v[i, k + 1];
                                            _v[i, k + 1] = _v[i, k];
                                            _v[i, k] = t;
                                        }
                                    }

                                    if (k < _m - 1)
                                    {
                                        for (int i = 0; i < _m; i++)
                                        {
                                            t = _u[i, k + 1];
                                            _u[i, k + 1] = _u[i, k];
                                            _u[i, k] = t;
                                        }
                                    }

                                    k++;
                                }

                            iter = 0;
                            p--;
                        }

                        break;
                }
            }
        }
    }
}
