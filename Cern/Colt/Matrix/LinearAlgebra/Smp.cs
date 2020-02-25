// <copyright file="Smp.Cs" company="CERN">
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
using System.Threading;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.LinearAlgebra
{
    /// <summary>
    /// To implement light weight thread pool mechanism, refered to this resource.
    /// https://stackoverflow.com/questions/435668/code-for-a-simple-thread-pool-in-c-sharp
    /// </summary>
    public class Smp
    {
        private TaskRunnerGroup taskGroup; // a very efficient and light weight thread pool

        protected int maxThreads;

        public TaskRunnerGroup TaskGroup { get { return taskGroup; } }

        /// <summary>
        /// Constructs a new Smp using a maximum of <i>maxThreads<i> threads.
        /// </summary>
        /// <param name="maxThreads"></param>
        public Smp(int maxThreads)
        {
            maxThreads = System.Math.Max(1, maxThreads);
            this.maxThreads = maxThreads;
            if (maxThreads > 1)
            {
                this.taskGroup = new TaskRunnerGroup(maxThreads);
            }
            else
            { // avoid parallel overhead
                this.taskGroup = null;
            }
        }


        /// <summary>
        /// Clean up deamon threads, if necessary.
        /// </summary>
        ~Smp()
        {
            if (this.taskGroup != null) this.taskGroup.Dispose();
        }

        public void Run(DoubleMatrix2D[] blocksA, DoubleMatrix2D[] blocksB, ref double[] results, Matrix2DMatrix2DFunction function)
        {

            double[] buf = new double[blocksA.Length];

            Action<int> task = (i =>
            {
                double result = function(blocksA[i], blocksB != null ? blocksB[i] : null);
                if (buf != null) buf[i] = result;
                //Console.Write("."); 
            });

            for (int i = 0; i < blocksA.Length; i++)
            {
                int k = i;

                taskGroup.QueueTask(() => task(k));
            }

            results = buf;

            #region Original Java Code
            //            FJTask[] subTasks = new FJTask[blocksA.Length];
            //            for (int i = 0; i < blocksA.Length; i++)
            //            {
            //                int k = i;
            //                subTasks[i] = new FJTask()
            //                {

            //            public void run()
            //                {
            //                    double result = function(blocksA[k], blocksB != null ? blocksB[k] : null);
            //                    if (results != null) results[k] = result;
            //                    //Console.Write("."); 
            //                }
            //            };
            //        }

            //	// run tasks and wait for completion
            //	try { 
            //		this.taskGroup.invoke(

            //            new FJTask()
            //        {
            //            public void run()
            //            {
            //                coInvoke(subTasks);
            //            }
            //        }
            //		);
            //	} catch (InterruptedException exc) {}
            //}
            #endregion
        }

        public DoubleMatrix2D[] SplitBlockedNN(DoubleMatrix2D A, int threshold, long flops)
        {
            /*
            determine how to split and parallelize best into blocks
            if more B.Columns than tasks --> split B.Columns, as follows:

                    xx|xx|xxx B
                    xx|xx|xxx
                    xx|xx|xxx
            A
            xxx     xx|xx|xxx C 
            xxx		xx|xx|xxx
            xxx		xx|xx|xxx
            xxx		xx|xx|xxx
            xxx		xx|xx|xxx

            if less B.Columns than tasks --> split A.rows, as follows:

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
            //long flops = 2L*A.Rows*A.Columns*A.Columns;
            int noOfTasks = (int)System.Math.Min(flops / threshold, this.maxThreads); // each thread should process at least 30000 flops
            Boolean splitHoriz = (A.Columns < noOfTasks);
            //Boolean splitHoriz = (A.Columns >= noOfTasks);
            int p = splitHoriz ? A.Rows : A.Columns;
            noOfTasks = System.Math.Min(p, noOfTasks);

            if (noOfTasks < 2)
            { // parallelization doesn't pay off (too much start up overhead)
                return null;
            }

            // set up concurrent tasks
            int span = p / noOfTasks;
            DoubleMatrix2D[] blocks = new DoubleMatrix2D[noOfTasks];
            for (int i = 0; i < noOfTasks; i++)
            {
                int offset = i * span;
                if (i == noOfTasks - 1) span = p - span * i; // last span may be a bit larger

                DoubleMatrix2D AA, BB, CC;
                if (!splitHoriz)
                {   // split B along columns into blocks
                    blocks[i] = A.ViewPart(0, offset, A.Rows, span);
                }
                else
                { // split A along rows into blocks
                    blocks[i] = A.ViewPart(offset, 0, span, A.Columns);
                }
            }
            return blocks;
        }

        public DoubleMatrix2D[][] SplitBlockedNN(DoubleMatrix2D A, DoubleMatrix2D B, int threshold, long flops)
        {
            DoubleMatrix2D[] blocksA = SplitBlockedNN(A, threshold, flops);
            if (blocksA == null) return null;
            DoubleMatrix2D[] blocksB = SplitBlockedNN(B, threshold, flops);
            if (blocksB == null) return null;
            DoubleMatrix2D[][] blocks = { blocksA, blocksB };
            return blocks;
        }

        public DoubleMatrix2D[] SplitStridedNN(DoubleMatrix2D A, int threshold, long flops)
        {
            /*
            determine how to split and parallelize best into blocks
            if more B.Columns than tasks --> split B.Columns, as follows:

                    xx|xx|xxx B
                    xx|xx|xxx
                    xx|xx|xxx
            A
            xxx     xx|xx|xxx C 
            xxx		xx|xx|xxx
            xxx		xx|xx|xxx
            xxx		xx|xx|xxx
            xxx		xx|xx|xxx

            if less B.Columns than tasks --> split A.rows, as follows:

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
            //long flops = 2L*A.Rows*A.Columns*A.Columns;
            int noOfTasks = (int)System.Math.Min(flops / threshold, this.maxThreads); // each thread should process at least 30000 flops
            Boolean splitHoriz = (A.Columns < noOfTasks);
            //Boolean splitHoriz = (A.Columns >= noOfTasks);
            int p = splitHoriz ? A.Rows : A.Columns;
            noOfTasks = System.Math.Min(p, noOfTasks);

            if (noOfTasks < 2)
            { // parallelization doesn't pay off (too much start up overhead)
                return null;
            }

            // set up concurrent tasks
            int span = p / noOfTasks;
            DoubleMatrix2D[] blocks = new DoubleMatrix2D[noOfTasks];
            for (int i = 0; i < noOfTasks; i++)
            {
                int offset = i * span;
                if (i == noOfTasks - 1) span = p - span * i; // last span may be a bit larger

                DoubleMatrix2D AA, BB, CC;
                if (!splitHoriz)
                {
                    // split B along columns into blocks
                    blocks[i] = A.ViewPart(0, i, A.Rows, A.Columns - i).ViewStrides(1, noOfTasks);
                }
                else
                {
                    // split A along rows into blocks
                    blocks[i] = A.ViewPart(i, 0, A.Rows - i, A.Columns).ViewStrides(noOfTasks, 1);
                }
            }
            return blocks;
        }

        /// <summary>
        /// Prints various snapshot statistics to System.out; Simply delegates to {@link EDU.oswego.Cs.dl.util.Concurrent.FJTaskRunnerGroup#stats}.
        /// </summary>
        public void Statistics()
        {
            if (this.taskGroup != null) this.taskGroup.Statistics();
        }
    }
}
