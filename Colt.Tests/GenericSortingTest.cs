// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSortingTest.cs" company="Università Politecnica delle Marche">
//   Mauro Mazzieri, 2010.
//   Adapted from Java source code of Colt, copyright © 1999 CERN - European Organization for Nuclear Research.
// </copyright>
// <summary>
//   Unit test for <see cref="GenericSorting"/>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Colt.Tests
{
    using System;

    using Cern.Colt;
    using Cern.Colt.Matrix;
    using Cern.Colt.Matrix.Implementation;
    using Cern.Colt.Matrix.DoubleAlgorithms;

    using NUnit.Framework;

    /// <summary>
    /// Unit test for <see cref="GenericSorting"/>
    /// </summary>
    [TestFixture]
    public class GenericSortingTest
    {
        /// <summary>
        /// Just a demo
        /// </summary>
        [Test]
        public void Demo1()
        {
            var x = new[] { 3, 2, 1 };
            var y = new[] { 3.0, 2.0, 1.0 };
            var z = new[] { 6.0, 7.0, 8.0 };

            const int From = 0;
            int to = x.Length;
            GenericSorting.QuickSort(
                From,
                to,
                (a, b) => x[a] == x[b] ? 0 : (x[a] < x[b] ? -1 : 1),
                (a, b) =>
                {
                    int t1 = x[a]; x[a] = x[b]; x[b] = t1;
                    double t2 = y[a]; y[a] = y[b]; y[b] = t2;
                    double t3 = z[a]; z[a] = z[b]; z[b] = t3;
                });

            Assert.AreEqual(1, x[0]);
            Assert.AreEqual(2, x[1]);
            Assert.AreEqual(3, x[2]);
            Assert.AreEqual(1.0, y[0]);
            Assert.AreEqual(2.0, y[1]);
            Assert.AreEqual(3.0, y[2]);
            Assert.AreEqual(8.0, z[0]);
            Assert.AreEqual(7.0, z[1]);
            Assert.AreEqual(6.0, z[2]);
        }

        /// <summary>
        /// Just another demo.
        /// </summary>
        [Test]
        public void Demo2()
        {
            var x = new[] { 6, 7, 8, 9 };
            var y = new[] { 3.0, 2.0, 1.0, 3.0 };
            var z = new[] { 5.0, 4.0, 4.0, 1.0 };

            const int From = 0;
            int to = x.Length;
            GenericSorting.QuickSort(
                       From,
                       to,
                       (a, b) =>
                       {
                           if (y[a] == y[b]) return z[a] == z[b] ? 0 : (z[a] < z[b] ? -1 : 1);
                           return y[a] < y[b] ? -1 : 1;
                       },
                       (a, b) =>
                       {
                           int t1 = x[a]; x[a] = x[b]; x[b] = t1;
                           double t2 = y[a]; y[a] = y[b]; y[b] = t2;
                           double t3 = z[a]; z[a] = z[b]; z[b] = t3;
                       });

            Assert.AreEqual(8, x[0]);
            Assert.AreEqual(7, x[1]);
            Assert.AreEqual(9, x[2]);
            Assert.AreEqual(6, x[3]);
            Assert.AreEqual(1.0, y[0]);
            Assert.AreEqual(2.0, y[1]);
            Assert.AreEqual(3.0, y[2]);
            Assert.AreEqual(3.0, y[3]);
            Assert.AreEqual(4.0, z[0]);
            Assert.AreEqual(4.0, z[1]);
            Assert.AreEqual(1.0, z[2]);
            Assert.AreEqual(5.0, z[3]);
        }

        /// <summary>
        /// Checks the correctness of the partition method by generating random input parameters and checking whether results are correct.
        /// </summary>
        [Test]
        public void TestRandomly()
        {
            const int Runs = 100;
            var gen = new Random();
            for (int run = 0; run < Runs; run++)
            {
                const int MaxSize = 50;

                int size = gen.Next(1, MaxSize);
                int from, to;
                if (size == 0)
                {
                    from = 0;
                    to = -1;
                }
                else
                {
                    from = gen.Next(0, size - 1);
                    to = gen.Next(Math.Min(from, size - 1), size - 1);
                }

                var a1 = new DenseDoubleMatrix2D(size, size);
                var p1 = a1.ViewPart(from, from, size - to, size - to);

                int intervalFrom = gen.Next(size / 2, 2 * size);
                int intervalTo = gen.Next(intervalFrom, 2 * size);

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        a1[i, j] = gen.Next(intervalFrom, intervalTo);
                    }
                }

                var a2 = a1.Copy();
                var p2 = a2.ViewPart(from, from, size - to, size - to);

                const int Column = 0;
                var s1 = Cern.Colt.Matrix.DoubleAlgorithms.Sorting.QuickSort.Sort(p1, Column);
                var s2 = Cern.Colt.Matrix.DoubleAlgorithms.Sorting.MergeSort.Sort(p2, Column);

                var v1 = s1.ViewColumn(Column);
                var sv1 = v1.ToString();
                var v2 = s2.ViewColumn(Column);
                var sv2 = v2.ToString();

                Assert.IsTrue(v1.Equals(v2));
            }
        }
    }
}
