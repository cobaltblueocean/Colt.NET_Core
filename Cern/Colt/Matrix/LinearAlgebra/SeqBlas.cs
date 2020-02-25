// <copyright file="SeqBlas.Cs" company="CERN">
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
    /// Sequential implementation of the Basic Linear Algebra System.
    /// </summary>
    public class SeqBlas: IBlas
    {
        /// <summary>
        /// Little trick to allow for "aliasing", that is, renaming this class.
        /// Time and again writing code like
        /// <p>
        /// <i>SeqBlas.blas.Dgemm(..d);</i>
        /// <p>
        /// is a bit awkwardd Using the aliasing you can instead write
        /// <p>
        /// <i>Blas B = SeqBlas.blas; <br>
        /// B.dgemm(..d);</i>
        /// </summary>
        public static IBlas seqBlas = new SeqBlas();

        private static Cern.Jet.Math.Functions F = Cern.Jet.Math.Functions.functions;

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected SeqBlas() { }

        public void Assign(DoubleMatrix2D A, Cern.Colt.Function.DoubleFunction function)
        {
            A.Assign(function);
        }

        public void Assign(DoubleMatrix2D A, DoubleMatrix2D B, Cern.Colt.Function.DoubleDoubleFunction function)
        {
            A.Assign(B, function);
        }

        public double Dasum(DoubleMatrix1D x)
        {
            return x.Aggregate(F2.Plus, F1.Abs);
        }

        public void Daxpy(double alpha, DoubleMatrix1D x, DoubleMatrix1D y)
        {
            y.Assign(x, F2.PlusMult(alpha));
        }

        public void Daxpy(double alpha, DoubleMatrix2D A, DoubleMatrix2D B)
        {
            B.Assign(A, F2.PlusMult(alpha));
        }

        public void Dcopy(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            y.Assign(x);
        }

        public void Dcopy(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            B.Assign(A);
        }

        public double Ddot(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            return x.ZDotProduct(y);
        }

        public void Dgemm(Boolean transposeA, Boolean transposeB, double alpha, DoubleMatrix2D A, DoubleMatrix2D B, double beta, DoubleMatrix2D C)
        {
            A.ZMult(B, C, alpha, beta, transposeA, transposeB);
        }

        public void Dgemv(Boolean transposeA, double alpha, DoubleMatrix2D A, DoubleMatrix1D x, double beta, DoubleMatrix1D y)
        {
            A.ZMult(x, y, alpha, beta, transposeA);
        }

        public void Dger(double alpha, DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix2D A)
        {
            Cern.Jet.Math.PlusMult fun = new Cern.Jet.Math.PlusMult(0);
            for (int i = A.Rows; --i >= 0;)
            {
                fun.Multiplicator = alpha * x[i];
                A.ViewRow(i).Assign(y, fun);

            }
        }

        public double Dnrm2(DoubleMatrix1D x)
        {
            return System.Math.Sqrt(Algebra.Norm2(x));
        }

        public void Drot(DoubleMatrix1D x, DoubleMatrix1D y, double c, double s)
        {
            x.CheckSize(y);
            DoubleMatrix1D tmp = x.Copy();

            x.Assign(F1.Mult(c));
            x.Assign(y, F2.PlusMult(s));

            y.Assign(F1.Mult(c));
            y.Assign(tmp, F2.MinusMult(s));
        }

        public void Drotg(double a, double b, double[] rotvec)
        {
            double c, s, roe, scale, r, z, ra, rb;

            roe = b;

            if (System.Math.Abs(a) > System.Math.Abs(b)) roe = a;

            scale = System.Math.Abs(a) + System.Math.Abs(b);

            if (scale != 0.0)
            {

                ra = a / scale;
                rb = b / scale;
                r = scale * System.Math.Sqrt(ra * ra + rb * rb);
                r = sign(1.0, roe) * r;
                c = a / r;
                s = b / r;
                z = 1.0;
                if (System.Math.Abs(a) > System.Math.Abs(b)) z = s;
                if ((System.Math.Abs(b) >= System.Math.Abs(a)) && (c != 0.0)) z = 1.0 / c;

            }
            else
            {

                c = 1.0;
                s = 0.0;
                r = 0.0;
                z = 0.0;

            }

            a = r;
            b = z;

            rotvec[0] = a;
            rotvec[1] = b;
            rotvec[2] = c;
            rotvec[3] = s;

        }

        public void Dscal(double alpha, DoubleMatrix1D x)
        {
            x.Assign(F1.Mult(alpha));
        }

        public void Dscal(double alpha, DoubleMatrix2D A)
        {
            A.Assign(F1.Mult(alpha));
        }

        public void Dswap(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            y.Swap(x);
        }

        public void Dswap(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            //B.Swap(A); not yet implemented
            A.CheckShape(B);
            for (int i = A.Rows; --i >= 0;) A.ViewRow(i).Swap(B.ViewRow(i));
        }

        public void Dsymv(Boolean isUpperTriangular, double alpha, DoubleMatrix2D A, DoubleMatrix1D x, double beta, DoubleMatrix1D y)
        {
            if (isUpperTriangular) A = A.ViewDice();
            Property.DEFAULT.CheckSquare(A);
            int size = A.Rows;
            if (size != x.Size || size != y.Size)
            {
                throw new ArgumentException(A.ToStringShort() + ", " + x.ToStringShort() + ", " + y.ToStringShort());
            }
            DoubleMatrix1D tmp = x.Like();
            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                for (int j = 0; j <= i; j++)
                {
                    sum += A[i, j] * x[j];
                }
                for (int j = i + 1; j < size; j++)
                {
                    sum += A[j, i] * x[j];
                }
                tmp[i] = alpha * sum + beta * y[i];
            }
            y.Assign(tmp);
        }

        public void Dtrmv(Boolean isUpperTriangular, Boolean transposeA, Boolean isUnitTriangular, DoubleMatrix2D A, DoubleMatrix1D x)
        {
            if (transposeA)
            {
                A = A.ViewDice();
                isUpperTriangular = !isUpperTriangular;
            }

            Property.DEFAULT.CheckSquare(A);
            int size = A.Rows;
            if (size != x.Size)
            {
                throw new ArgumentException(A.ToStringShort() + ", " + x.ToStringShort());
            }

            DoubleMatrix1D b = x.Like();
            DoubleMatrix1D y = x.Like();
            if (isUnitTriangular)
            {
                y.Assign(1);
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    y[i] = A[i, i];
                }
            }

            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                if (!isUpperTriangular)
                {
                    for (int j = 0; j < i; j++)
                    {
                        sum += A[i, j] * x[j];
                    }
                    sum += y[i] * x[i];
                }
                else
                {
                    sum += y[i] * x[i];
                    for (int j = i + 1; j < size; j++)
                    {
                        sum += A[i, j] * x[j];
                    }
                }
                b[i] = sum;
            }
            x.Assign(b);
        }

        public int Idamax(DoubleMatrix1D x)
        {
            int maxIndex = -1;
            double maxValue = Double.MinValue;
            for (int i = x.Size; --i >= 0;)
            {
                double v = System.Math.Abs(x[i]);
                if (v > maxValue)
                {
                    maxValue = v;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        /// <summary>
        /// Implements the FORTRAN sign (not sin) function.
        /// See the code for details.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double sign(double a, double b)
        {
            if (b < 0.0)
            {
                return -System.Math.Abs(a);
            }
            else
            {
                return System.Math.Abs(a);
            }
        }
    }
}
