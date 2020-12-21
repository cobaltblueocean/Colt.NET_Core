// <copyright file="SmpBlas.cs" company="CERN">
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
using Cern.Colt.Function;

namespace Cern.Colt.Matrix.LinearAlgebra
{
    public class SmpBlas : IBlas
    {
        /// <summary>
        /// The public global parallel blas; initialized via <see cref="AllocateBlas(int, IBlas)"/>.
        /// Do not modify this variable via other means (it is public).
        /// </summary>
        public static IBlas smpBlas = SeqBlas.seqBlas;

        protected IBlas seqBlas; // blocks are operated on in parallel; for each block this seq algo is used.

        protected Smp smp;

        protected int maxThreads;

        protected static int NN_THRESHOLD = 30000;

        /// <summary>
        /// Constructs a blas using a maximum of <i>maxThreads<i> threads; each executing the given sequential algos.
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="seqBlas"></param>
        public SmpBlas(int maxThreads, IBlas seqBlas)
        {
            this.seqBlas = seqBlas;
            this.maxThreads = maxThreads;
            this.smp = new Smp(maxThreads);
            //Smp.smp = new Smp(maxThreads);
        }

        /// <summary>
        /// Sets the public global variable <i>SmpBlas.smpBlas</i> to a blas using a maximum of <i>maxThreads</i> threads, each executing the given sequential algorithm; <i>maxThreads</i> is normally the number of CPUs.
        /// Call this method at the very beginning of your programd 
        /// Normally there is no need to call this method more than once.
        /// </summary>
        /// <param name="maxThreads">the maximum number of threads (= CPUs) to be used</param>
        /// <param name="seqBlas">the sequential blas algorithms to be used on concurrently processed matrix blocks.</param>
        public static void AllocateBlas(int maxThreads, IBlas seqBlas)
        {
            if (smpBlas is SmpBlas)
            { // no need to change anything?
                SmpBlas s = (SmpBlas)smpBlas;
                if (s.maxThreads == maxThreads && s.seqBlas == seqBlas) return;
            }

            if (maxThreads <= 1)
                smpBlas = seqBlas;
            else
            {
                smpBlas = new SmpBlas(maxThreads, seqBlas);
            }
        }

        #region Implemented Methods

        public void Assign(DoubleMatrix2D A, DoubleFunction function)
        {
            run(A, false, new Matrix2DMatrix2DFunction((AA, BB) =>
                                    {

                                        seqBlas.Assign(AA, function);
                                        return 0;
                                    }));
        }

        public void Assign(DoubleMatrix2D x, DoubleMatrix2D y, DoubleDoubleFunction function)
        {
            run(x, y, false, new Matrix2DMatrix2DFunction((AA, BB) =>
                                    {

                                        seqBlas.Assign(AA, BB, function);
                                        return 0;
                                    }));
        }

        public double Dasum(DoubleMatrix1D x)
        {
            return seqBlas.Dasum(x);
        }

        public void Daxpy(double alpha, DoubleMatrix1D x, DoubleMatrix1D y)
        {
            seqBlas.Daxpy(alpha, x, y);
        }

        public void Daxpy(double alpha, DoubleMatrix2D A, DoubleMatrix2D B)
        {
            seqBlas.Daxpy(alpha, A, B);
        }

        public void Dcopy(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            seqBlas.Dcopy(x, y);
        }

        public void Dcopy(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            seqBlas.Dcopy(A, B);
        }

        public double Ddot(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            return seqBlas.Ddot(x, y);
        }

        public void Dgemm(bool transposeA, bool transposeB, double alpha, DoubleMatrix2D A, DoubleMatrix2D B, double beta, DoubleMatrix2D C)
        {
            /*
	        determine how to split and parallelize best into blocks
	        if more B.columns than tasks --> split B.columns, as follows:
	
			        xx|xx|xxx B
			        xx|xx|xxx
			        xx|xx|xxx
	        A
	        xxx     xx|xx|xxx C 
	        xxx		xx|xx|xxx
	        xxx		xx|xx|xxx
	        xxx		xx|xx|xxx
	        xxx		xx|xx|xxx

	        if less B.columns than tasks --> split A.rows, as follows:
	
			        xxxxxxx B
			        xxxxxxx
			        xxxxxxx
	        A
	        xxx     xxxxxxx C
	        xxx     xxxxxxx
	        ---     -------
	        xxx     xxxxxxx
	        xxx     xxxxxxx
	        ---     -------
	        xxx     xxxxxxx

	        */
            if (transposeA)
            {
                Dgemm(false, transposeB, alpha, A.ViewDice(), B, beta, C);
                return;
            }
            if (transposeB)
            {
                Dgemm(transposeA, false, alpha, A, B.ViewDice(), beta, C);
                return;
            }
            int m = A.Rows;
            int n = A.Columns;
            int p = B.Columns;

            if (B.Rows != n)
                throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_Matrix2DInnerDimensionMustAgree, A.ToStringShort() , B.ToStringShort()));
            if (C.Rows != m || C.Columns != p)
                throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_IncompatibleResultMatrix,  A.ToStringShort() ,B.ToStringShort(), C.ToStringShort()));
            if (A == C || B == C)
                throw new ArgumentException(Cern.LocalizedResources.Instance().Exception_MatricesMustNotBeIdentical);

            long flops = 2L * m * n * p;
            int noOfTasks = (int)System.Math.Min(flops / 30000, this.maxThreads); // each thread should process at least 30000 flops
            Boolean splitB = (p >= noOfTasks);
            int width = splitB ? p : m;
            noOfTasks = System.Math.Min(width, noOfTasks);

            if (noOfTasks < 2)
            { // parallelization doesn't pay off (too much start up overhead)
                seqBlas.Dgemm(transposeA, transposeB, alpha, A, B, beta, C);
                return;
            }

            // set up concurrent tasks
            int span = width / noOfTasks;
            //FJTask[] subTasks = new FJTask[noOfTasks];


            for (int i = 0; i < noOfTasks; i++)
            {
                int offset = i * span;
                if (i == noOfTasks - 1) span = width - span * i; // last span may be a bit larger

                DoubleMatrix2D AA, BB, CC;
                if (splitB)
                {
                    // split B along columns into blocks
                    AA = A;
                    BB = B.ViewPart(0, offset, n, span);
                    CC = C.ViewPart(0, offset, m, span);
                }
                else
                {
                    // split A along rows into blocks
                    AA = A.ViewPart(offset, 0, span, n);
                    BB = B;
                    CC = C.ViewPart(offset, 0, span, p);
                }

                Action task = (() =>
                {
                    seqBlas.Dgemm(transposeA, transposeB, alpha, AA, BB, beta, CC);
                });

                // run tasks and wait for completion
                try
                {
                    this.smp.TaskGroup.QueueTask(() => task());
                }
                catch (TaskCanceledException exc)
                {
                    Console.Write(exc.Message);
                }
            }

        }

        public void Dgemv(bool transposeA, double alpha, DoubleMatrix2D A, DoubleMatrix1D x, double beta, DoubleMatrix1D y)
        {
            /*
            split A, as follows:

                    x x
                    x
                    x
            A
            xxx     x y
            xxx     x
            ---     -
            xxx     x
            xxx     x
            ---     -
            xxx     x

            */
            if (transposeA)
            {
                Dgemv(false, alpha, A.ViewDice(), x, beta, y);
                return;
            }
            int m = A.Rows;
            int n = A.Columns;
            long flops = 2L * m * n;
            int noOfTasks = (int)System.Math.Min(flops / 30000, this.maxThreads); // each thread should process at least 30000 flops
            int width = A.Rows;
            noOfTasks = System.Math.Min(width, noOfTasks);

            if (noOfTasks < 2)
            { // parallelization doesn't pay off (too much start up overhead)
                seqBlas.Dgemv(transposeA, alpha, A, x, beta, y);
                return;
            }

            // set up concurrent tasks
            int span = width / noOfTasks;
            //FJTask[] subTasks = new FJTask[noOfTasks];
            for (int i = 0; i < noOfTasks; i++)
            {
                int offset = i * span;
                if (i == noOfTasks - 1) span = width - span * i; // last span may be a bit larger

                // split A along rows into blocks
                DoubleMatrix2D AA = A.ViewPart(offset, 0, span, n);
                DoubleMatrix1D yy = y.ViewPart(offset, span);

                //    subTasks[i] = new FJTask()
                //    {

                //        public void run()
                //    {
                //        seqBlas.Dgemv(transposeA, alpha, AA, x, beta, yy);
                //        //Console.WriteLine("Hello "+offset); 
                //    }
                //};

                Action task = (() =>
                {
                    seqBlas.Dgemv(transposeA, alpha, AA, x, beta, yy);
                });

                // run tasks and wait for completion
                try
                {
                    this.smp.TaskGroup.QueueTask(() => task());
                }
                catch (TaskCanceledException exc)
                {
                    Console.Write(exc.Message);
                }

            }
        }

        public void Dger(double alpha, DoubleMatrix1D x, DoubleMatrix1D y, DoubleMatrix2D A)
        {
            seqBlas.Dger(alpha, x, y, A);
        }

        public double Dnrm2(DoubleMatrix1D x)
        {
            return seqBlas.Dnrm2(x);
        }

        public void Drot(DoubleMatrix1D x, DoubleMatrix1D y, double c, double s)
        {
            seqBlas.Drot(x, y, c, s);
        }

        public void Drotg(double a, double b, double[] rotvec)
        {
            seqBlas.Drotg(a, b, rotvec);
        }

        public void Dscal(double alpha, DoubleMatrix1D x)
        {
            seqBlas.Dscal(alpha, x);
        }

        public void Dscal(double alpha, DoubleMatrix2D A)
        {
            seqBlas.Dscal(alpha, A);
        }

        public void Dswap(DoubleMatrix1D x, DoubleMatrix1D y)
        {
            seqBlas.Dswap(x, y);
        }

        public void Dswap(DoubleMatrix2D x, DoubleMatrix2D y)
        {
            seqBlas.Dswap(x, y);
        }

        public void Dsymv(bool isUpperTriangular, double alpha, DoubleMatrix2D A, DoubleMatrix1D x, double beta, DoubleMatrix1D y)
        {
            seqBlas.Dsymv(isUpperTriangular, alpha, A, x, beta, y);
        }

        public void Dtrmv(bool isUpperTriangular, bool transposeA, bool isUnitTriangular, DoubleMatrix2D A, DoubleMatrix1D x)
        {
            seqBlas.Dtrmv(isUpperTriangular, transposeA, isUnitTriangular, A, x);
        }

        public int Idamax(DoubleMatrix1D x)
        {
            return seqBlas.Idamax(x);
        }
        #endregion

        protected double[] run(DoubleMatrix2D A, DoubleMatrix2D B, Boolean collectResults, Matrix2DMatrix2DFunction fun)
        {
            DoubleMatrix2D[][] blocks;
            blocks = this.smp.SplitBlockedNN(A, B, NN_THRESHOLD, A.Rows * A.Columns);
            //blocks = this.smp.splitStridedNN(A, B, NN_THRESHOLD, A.Rows*A.Columns);
            int b = blocks != null ? blocks.GetLength(1) : 1;
            double[] results = collectResults ? new double[b] : null;

            if (blocks == null)
            {  // too small --> sequential
                double result = fun(A, B);
                if (collectResults) results[0] = result;
                return results;
            }
            else
            {  // parallel
                this.smp.Run(blocks[0], blocks[1], ref results, fun);
            }
            return results;
        }

        protected double[] run(DoubleMatrix2D A, Boolean collectResults, Matrix2DMatrix2DFunction fun)
        {
            DoubleMatrix2D[] blocks;
            blocks = this.smp.SplitBlockedNN(A, NN_THRESHOLD, A.Rows * A.Columns);
            //blocks = this.smp.splitStridedNN(A, NN_THRESHOLD, A.Rows*A.Columns);
            int b = blocks != null ? blocks.Length : 1;
            double[] results = collectResults ? new double[b] : null;

            if (blocks == null)
            { // too small -> sequential
                double result = fun(A, null);
                if (collectResults) results[0] = result;
                return results;
            }
            else
            { // parallel
                this.smp.Run(blocks, null, ref results, fun);
            }
            return results;
        }


        /// <summary>
        /// Prints various snapshot statistics to System.out; Simply delegates to {@link EDU.oswego.cs.Dl.util.concurrent.FJTaskRunnerGroup#stats}.
        /// </summary>
        public void stats()
        {
            if (this.smp != null) this.smp.Statistics();
        }

        private double XSum(DoubleMatrix2D A)
        {
            double[] sums = run(A, true,
                new Matrix2DMatrix2DFunction((AA, BB) =>
                {
                    return AA.ZSum();
                }));

            double sum = 0;
            for (int i = sums.Length; --i >= 0;) sum += sums[i];
            return sum;
        }
    }
}
