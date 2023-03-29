// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleSortingTest.cs" company="Università Politecnica delle Marche">
//   Mauro Mazzieri, 2010.
//   Adapted from Java source code of Colt, copyright © 1999 CERN - European Organization for Nuclear Research.
// </copyright>
// <summary>
//   Unit test of <see cref="Colt.Matrix.DoubleAlgorithms.Sorting" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Tests
{
    using System;

    using Cern.Colt.Matrix;
    using Cern.Colt.Matrix.Implementation;
    using Cern.Jet.Math;
    using NUnit.Framework;

    /// <summary>
    /// Unit test of <see cref="Colt.Matrix.DoubleAlgorithms.Sorting"/>
    /// </summary>
    [TestFixture]
    public class DoubleSortingTest
    {
        /// <summary>
        /// Tests sort by sum of row.
        /// </summary>
        [Test]
        public void Zdemo1()
        {
            var sort = Cern.Colt.Matrix.DoubleAlgorithms.Sorting.QuickSort;
            IDoubleMatrix2D matrix = DoubleFactory2D.Dense.Descending(4, 3);
            var sorted = sort.Sort(
                matrix,
                (a, b) =>
                {
                    double aSum = a.ZSum(); double bSum = b.ZSum();
                    return aSum < bSum ? -1 : aSum == bSum ? 0 : 1;
                });
            Assert.AreEqual(2, sorted[0, 0]);
            Assert.AreEqual(0, sorted[0, 2]);
            Assert.AreEqual(11, sorted[3, 0]);
            Assert.AreEqual(9, sorted[3, 2]);
        }

        /// <summary>
        /// Tests sort by sinus of cell values.
        /// </summary>
        [Test]
        public void Zdemo3()
        {
            var sort = Cern.Colt.Matrix.DoubleAlgorithms.Sorting.QuickSort;
            double[] values = { 0.5, 1.5, 2.5, 3.5 };
            IDoubleMatrix1D matrix = new DenseDoubleMatrix1D(values);
            IDoubleMatrix1D sorted = sort.Sort(
                matrix,
                (a, b) =>
                {
                    double sina = Math.Sin(a);
                    double sinb = Math.Sin(b);
                    return sina < sinb ? -1 : sina == sinb ? 0 : 1;
                });
            Assert.AreEqual(3.5, sorted[0]);
            Assert.AreEqual(0.5, sorted[1]);
            Assert.AreEqual(2.5, sorted[2]);
            Assert.AreEqual(1.5, sorted[3]);
        }

        /// <summary>
        /// Demonstrates applying functions.
        /// </summary>
        [Test]
        public void Zdemo4()
        {
            double[] values1 = { 0, 1, 2, 3 };
            double[] values2 = { 0, 2, 4, 6 };
            IDoubleMatrix1D matrix1 = new DenseDoubleMatrix1D(values1);
            IDoubleMatrix1D matrix2 = new DenseDoubleMatrix1D(values2);
            matrix1.Assign(matrix2, new DoubleDoubleFunction() { Eval = (a, b) => Math.Pow(a, b) });
            Assert.AreEqual(1, matrix1[0]);
            Assert.AreEqual(1, matrix1[1]);
            Assert.AreEqual(Math.Pow(2, 4), matrix1[2]);
            Assert.AreEqual(Math.Pow(3, 6), matrix1[3]);
        }
    }
}
