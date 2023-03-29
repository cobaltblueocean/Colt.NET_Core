// <copyright file="IBlas.cs" company="CERN">
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

namespace Cern.Colt.Matrix.LinearAlgebra
{
    public interface IBlas
    {
        /// <summary>
        /// Assigns the result of a function to each cell; <i>x[row,col] = function(x[row,col])</i>.
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="function">a function object taking as argument the current cell's value.</param>
        /// <see cref="Cern.Jet.Math.Functions"/>
        void Assign(IDoubleMatrix2D A, IDoubleFunction function);

        /// <summary>
        /// Assigns the result of a function to each cell; <i>x[row,col] = function(x[row,col],y[row,col])</i>.
        /// </summary>
        /// <param name="x">the matrix to modify.</param>
        /// <param name="y">the secondary matrix to operate on.</param>
        /// <param name="function">a function object taking as first argument the current cell's value of <i>this</i>, and as second argument the current cell's value of <i>y</i>, <i>this</i> (for convenience only).</param>
        /// <exception cref="ArgumentException">if <i>x.Columns != y.Columns || x.Rows != y.Rows</i></exception>
        /// <see cref="Cern.Jet.Math.Functions"/>
        void Assign(IDoubleMatrix2D x, IDoubleMatrix2D y, IDoubleDoubleFunction function);

        /// <summary>
        /// Returns the sum of absolute values; <i>|x[0]| + |x[1]| + ..d </i>.
        /// In fact equivalent to <i>x.aggregate(Cern.Jet.Math.Functions.plus, Cern.Jet.Math.Functions.abs)</i>.
        /// </summary>
        /// <param name="x">the first vector.</param>
        /// <returns></returns>
        double Dasum(IDoubleMatrix1D x);

        /// <summary>
        /// Combined vector scaling; <i>y = y + alpha*x</i>.
        /// In fact equivalent to <i>y.assign(x,Cern.Jet.Math.Functions.plusMult(alpha))</i>.
        /// </summary>
        /// <param name="alpha">a scale factor.</param>
        /// <param name="x">the first source vector.</param>
        /// <param name="y">the second source vector, this is also the vector where results are stored.</param>
        /// <exception cref="ArgumentException"><i>x.Count != y.Count</i>..</exception>
        void Daxpy(double alpha, IDoubleMatrix1D x, IDoubleMatrix1D y);

        /// <summary>
        /// Combined matrix scaling; <i>B = B + alpha*A</i>.
        /// In fact equivalent to <i>B.assign(A,Cern.Jet.Math.Functions.plusMult(alpha))</i>.
        /// </summary>
        /// <param name="alpha">a scale factor.</param>
        /// <param name="A">the first source matrix.</param>
        /// <param name="B">the second source matrix, this is also the matrix where results are stored.</param>
        /// <exception cref="ArgumentException">if <i>A.Columns != B.Columns || A.Rows != B.Rows</i>.</exception>
        void Daxpy(double alpha, IDoubleMatrix2D A, IDoubleMatrix2D B);

        /// <summary>
        /// Vector assignment (copying); <i>y = x</i>.
        /// In fact equivalent to <i>y.assign(x)</i>.
        /// </summary>
        /// <param name="x">the source vector.</param>
        /// <param name="y">the destination vector.</param>
        /// <exception cref="ArgumentException"><i>x.Count != y.Count</i>.</exception>
        void Dcopy(IDoubleMatrix1D x, IDoubleMatrix1D y);

        /// <summary>
        /// Matrix assignment (copying); <i>B = A</i>.
        /// In fact equivalent to <i>B.assign(A)</i>.
        /// </summary>
        /// <param name="A">the source matrix.</param>
        /// <param name="B">the destination matrix.</param>
        /// <exception cref="ArgumentException">if <i>A.Columns != B.Columns || A.Rows != B.Rows</i>.</exception>
        void Dcopy(IDoubleMatrix2D A, IDoubleMatrix2D B);

        /// <summary>
        /// Returns the dot product of two vectors x and y, which is <i>Sum(x[i]*y[i])</i>.
        /// In fact equivalent to <i>x.ZDotProduct(y)</i>.
        /// </summary>
        /// <param name="x">the first vector.</param>
        /// <param name="y">the second vector.</param>
        /// <returns>the sum of products.</returns>
        /// <exception cref="ArgumentException">if <i>x.Count != y.Count</i>.</exception>
        double Ddot(IDoubleMatrix1D x, IDoubleMatrix1D y);

        /// <summary>
        /// Generalized linear algebraic matrix-matrix multiply; <i>C = alpha*A*B + beta*C</i>.
        /// In fact equivalent to <i>A.zMult(B,C,alpha,beta,transposeA,transposeB)</i>.
        /// Note: Matrix shape conformance is checked <i>after</i> potential transpositions.
        /// </summary>
        /// <param name="transposeA">set this flag to indicate that the multiplication shall be performed on A'.</param>
        /// <param name="transposeB">set this flag to indicate that the multiplication shall be performed on B'.</param>
        /// <param name="alpha">a scale factor.</param>
        /// <param name="A">the first source matrix.</param>
        /// <param name="B">the second source matrix.</param>
        /// <param name="beta">a scale factor.</param>
        /// <param name="C">the third source matrix, this is also the matrix where results are stored.</param>
        /// <exception cref="ArgumentException">if <i>B.Rows != A.Columns</i>.</exception>
        /// <exception cref="ArgumentException">if <i>C.Rows != A.Rows || C.Columns != B.Columns</i>.</exception>
        /// <exception cref="ArgumentException">if <i>A == C || B == C</i>.</exception>
        void Dgemm(Boolean transposeA, Boolean transposeB, double alpha, IDoubleMatrix2D A, IDoubleMatrix2D B, double beta, IDoubleMatrix2D C);

        /// <summary>
        /// Generalized linear algebraic matrix-vector multiply; <i>y = alpha*A*x + beta*y</i>.
        /// In fact equivalent to <i>A.zMult(x,y,alpha,beta,transposeA)</i>.
        /// Note: Matrix shape conformance is checked <i>after</i> potential transpositions.
        /// </summary>
        /// <param name="transposeA">set this flag to indicate that the multiplication shall be performed on A'.</param>
        /// <param name="alpha">a scale factor.</param>
        /// <param name="A">the source matrix.</param>
        /// <param name="x">the first source vector.</param>
        /// <param name="beta">a scale factor.</param>
        /// <param name="y">the second source vector, this is also the vector where results are stored.</param>
        /// <exception cref="ArgumentException"><i>A.Columns != x.Count || A.Rows != y.Count)</i>..</exception>
        void Dgemv(Boolean transposeA, double alpha, IDoubleMatrix2D A, IDoubleMatrix1D x, double beta, IDoubleMatrix1D y);

        /// <summary>
        /// Performs a rank 1 update; <i>A = A + alpha*x*y'</i>.
        /// </summary>
        /// <param name="alpha">a scalar.</param>
        /// <param name="x">an m element vector.</param>
        /// <param name="y">an n element vector.</param>
        /// <param name="A">an m by n matrix.</param>
        /// <example>
        /// A = { {6,5}, {7,6} }, x = {1,2}, y = {3,4}, alpha = 1 -->
        /// A = { {9,9}, {13,14} }
        /// </example>
        void Dger(double alpha, IDoubleMatrix1D x, IDoubleMatrix1D y, IDoubleMatrix2D A);

        /// <summary>
        /// Return the 2-norm; <i>sqrt(x[0]^2 + x[1]^2 + ..d)</i>.
        /// In fact equivalent to <i>System.Math.Sqrt(Algebra.DEFAULT.norm2(x))</i>.
        /// </summary>
        /// <param name="x">the vector.</param>
        /// <returns></returns>
        double Dnrm2(IDoubleMatrix1D x);

        /// <summary>
        /// Applies a givens plane rotation to (x,y); <i>x = c*x + s*y; y = c*y - s*x</i>.
        /// </summary>
        /// <param name="x">the first vector.</param>
        /// <param name="y">the second vector.</param>
        /// <param name="c">the cosine of the angle of rotation.</param>
        /// <param name="s"><the sine of the angle of rotation./param>
        void Drot(IDoubleMatrix1D x, IDoubleMatrix1D y, double c, double s);

        /// <summary>
        /// Constructs a Givens plane rotation for <i>(a,b)</i>.
        /// Taken from the LINPACK translation from FORTRAN to Java, interface slightly modified.
        /// In the LINPACK listing DROTG is attributed to Jack Dongarra
        /// </summary>
        /// <param name="a">rotational elimination parameter a.</param>
        /// <param name="b">rotational elimination parameter b.</param>
        /// <param name="rotvec">Must be at least of Length 4d On output contains the values <i>{a,b,c,s}</i>.</param>
        void Drotg(double a, double b, double[] rotvec);

        /// <summary>
        /// Vector scaling; <i>x = alpha*x</i>.
        /// In fact equivalent to <i>x.assign(Cern.Jet.Math.Functions.mult(alpha))</i>.
        /// </summary>
        /// <param name="alpha">a scale factor.</param>
        /// <param name="x">the first vector.</param>
        void Dscal(double alpha, IDoubleMatrix1D x);

        /// <summary>
        /// Matrix scaling; <i>A = alpha*A</i>.
        /// In fact equivalent to <i>A.assign(Cern.Jet.Math.Functions.mult(alpha))</i>.
        /// </summary>
        /// <param name="alpha">a scale factor.</param>
        /// <param name="A">the matrix.</param>
        void Dscal(double alpha, IDoubleMatrix2D A);

        /// <summary>
        /// Swaps the elements of two vectors; <i>y <==> x</i>.
        /// In fact equivalent to <i>y.swap(x)</i>.
        /// </summary>
        /// <param name="x">the first vector.</param>
        /// <param name="y">the second vector.</param>
        /// <exception cref="ArgumentException"><i>x.Count != y.Count</i>.</exception>
        void Dswap(IDoubleMatrix1D x, IDoubleMatrix1D y);

        /// <summary>
        /// Swaps the elements of two matrices; <i>B <==> A</i>.
        /// </summary>
        /// <param name="x">the first matrix.</param>
        /// <param name="y">the second matrix.</param>
        /// <exception cref="ArgumentException">if <i>A.Columns != B.Columns || A.Rows != B.Rows</i>.</exception>
        void Dswap(IDoubleMatrix2D x, IDoubleMatrix2D y);

        /// <summary>
        /// Symmetric matrix-vector multiplication; <i>y = alpha*A*x + beta*y</i>.
        /// Where alpha and beta are scalars, x and y are n element vectors and
        /// A is an n by n symmetric matrix.
        /// A can be in upper or lower triangular format.
        /// </summary>
        /// <param name="isUpperTriangular">is A upper triangular or lower triangular part to be used?</param>
        /// <param name="alpha">scaling factor.</param>
        /// <param name="A">the source matrix.</param>
        /// <param name="x">the first source vector.</param>
        /// <param name="beta">scaling factor.</param>
        /// <param name="y">the second vector holding source and destination.</param>
        void Dsymv(Boolean IsUpperTriangular, double alpha, IDoubleMatrix2D A, IDoubleMatrix1D x, double beta, IDoubleMatrix1D y);

        /// <summary>
        /// Triangular matrix-vector multiplication; <i>x = A*x</i> or <i>x = A'*x</i>.
        /// Where x is an n element vector and A is an n by n unit, or non-unit,
        /// upper or lower triangular matrix.
        /// </summary>
        /// <param name="isUpperTriangular">is A upper triangular or lower triangular?</param>
        /// <param name="transposeA">set this flag to indicate that the multiplication shall be performed on A'.</param>
        /// <param name="isUnitTriangular">true --> A is assumed to be unit triangular; false --> A is not assumed to be unit triangular</param>
        /// <param name="A">the source matrix.</param>
        /// <param name="x">the vector holding source and destination.</param>
        void Dtrmv(Boolean IsUpperTriangular, Boolean transposeA, Boolean IsUnitTriangular, IDoubleMatrix2D A, IDoubleMatrix1D x);

        /// <summary>
        /// Returns the index of largest absolute value; <i>i such that |x[i]| == max(|x[0]|,|x[1]|,..d).</i>.
        /// </summary>
        /// <param name="x">the vector to search through.</param>
        /// <returns>the index of largest absolute value (-1 if x is empty).</returns>
        int Idamax(IDoubleMatrix1D x);
    }
}
