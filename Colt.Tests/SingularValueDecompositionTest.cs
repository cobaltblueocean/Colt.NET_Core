// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingularValueDecompositionTest.cs" company="UniversitEPolitecnica delle Marche">
//   Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Unit test for <see cref="SingularValueDecomposition"/>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Colt.Tests
{
    using System;

    using Cern.Colt.Function;
    using Cern.Colt.Matrix;
    using Cern.Colt.Matrix.Implementation;
    using Cern.Colt.Matrix.LinearAlgebra;

    using NUnit.Framework;

    /// <summary>
    /// Unit test for <see cref="SingularValueDecomposition"/>
    /// </summary>
    [TestFixture]
    public class SingularValueDecompositionTest
    {
        /// <summary>
        /// The matrix A (from LSA).
        /// </summary>
        private DoubleMatrix2D _a;

        /// <summary>
        /// The matrix B (random).
        /// </summary>
        private DoubleMatrix2D _b;

        /// <summary>
        /// A has values from (Deerwesteret al, 1990), B has random values.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _a = new DenseDoubleMatrix2D(new[]
                {
                    new[] { 1d, 0d, 0d, 1d, 0d, 0d, 0d, 0d, 0d },
                    new[] { 1d, 0d, 1d, 0d, 0d, 0d, 0d, 0d, 0d },
                    new[] { 1d, 1d, 0d, 0d, 0d, 0d, 0d, 0d, 0d },
                    new[] { 0d, 1d, 1d, 0d, 1d, 0d, 0d, 0d, 0d },
                    new[] { 0d, 1d, 1d, 2d, 0d, 0d, 0d, 0d, 0d },
                    new[] { 0d, 1d, 0d, 0d, 1d, 0d, 0d, 0d, 0d },
                    new[] { 0d, 1d, 0d, 0d, 1d, 0d, 0d, 0d, 0d },
                    new[] { 0d, 0d, 1d, 1d, 0d, 0d, 0d, 0d, 0d },
                    new[] { 0d, 1d, 0d, 0d, 0d, 0d, 0d, 0d, 1d },
                    new[] { 0d, 0d, 0d, 0d, 0d, 1d, 1d, 1d, 0d },
                    new[] { 0d, 0d, 0d, 0d, 0d, 0d, 1d, 1d, 1d },
                    new[] { 0d, 0d, 0d, 0d, 0d, 0d, 0d, 1d, 1d },
                });
            _b = DoubleFactory2D.Dense.Random(12, 4);
        }

        /// <summary>
        /// Check some values from the result of batch SVD.
        /// </summary>
        [Test]
        public void TestLSI()
        {
            var svd = new SingularValueDecomposition(_a, true, true, true);
            ////svd.Reduce(2);
            var scales = svd.SingularValues;
            Assert.LessOrEqual(Math.Abs(scales[0] - 3.34), 0.01);
            Assert.LessOrEqual(Math.Abs(scales[1] - 2.54), 0.01);
            var terms = svd.U;
            Assert.LessOrEqual(Math.Abs(terms[0, 0] - 0.22), 0.01);
            Assert.LessOrEqual(Math.Abs(terms[1, 1] + 0.07), 0.01);
            var docs = svd.V;
            Assert.LessOrEqual(Math.Abs(docs[0, 0] - 0.20), 0.01);
            Assert.LessOrEqual(Math.Abs(docs[1, 1] - 0.17), 0.01);
        }

        /// <summary>
        /// Tests preliminar to an incremental SVD.
        /// </summary>
        [Test]
        public void TestUpdating()
        {
            // first row
            var m = DoubleFactory2D.Dense.Make(_a.ViewColumn(0).ToArray(), _a.Rows);
            var initialSVD = new SingularValueDecomposition(m);
            var u = initialSVD.U;
            var s = initialSVD.S;
            var v = initialSVD.V;

            // second row
            var d = DoubleFactory2D.Dense.Make(_a.ViewColumn(1).ToArray(), _a.Rows);
            var l = u.ViewDice().ZMult(d, null);
            var uu = u.ZMult(u.ViewDice(), null);
            var ul = uu.ZMult(d, null);
            var h = d.Copy().Assign(uu.ZMult(d, null), BinaryFunctions.Minus);
            var k = Math.Sqrt(d.Aggregate(BinaryFunctions.Plus, a => a * a) - (2 * l.Aggregate(BinaryFunctions.Plus, a => a * a)) + ul.Aggregate(BinaryFunctions.Plus, a => a * a));
            var j1 = h.Assign(UnaryFunctions.Div(k));
            Assert.AreEqual(j1.Assign(UnaryFunctions.Mult(k)), h);

            var q =
                DoubleFactory2D.Dense.Compose(
                    new[] { new[] { s, l }, new[] { null, DoubleFactory2D.Dense.Make(1, 1, k) } });
            var svdq = new SingularValueDecomposition(q);
            var u2 = DoubleFactory2D.Dense.AppendColumns(u, j1).ZMult(svdq.U, null);
            var s2 = svdq.S;
            var v2 = DoubleFactory2D.Dense.ComposeDiagonal(v, DoubleFactory2D.Dense.Identity(1)).ZMult(
                svdq.V, null);

            var svd = new SingularValueDecomposition(_a.ViewPart(0, 0, _a.Rows, 2));
            Assert.AreEqual(svd.S, s2);
        }

        /// <summary>
        /// Test the incremental decomposition.
        /// </summary>
        [Test]
        public void IncrementalSVD()
        {
            const double Tolerance = 1.0E-6;

            foreach (var m in new[] { _a, _b })
            {
                // incremental SVD
                var d = m.ViewColumn(0);
                var incSvd = new SingularValueDecomposition(d);
                var svd = new SingularValueDecomposition(m.ViewPart(0, 0, m.Rows, 1));
                var s1 = svd.SingularValues;
                var s2 = incSvd.SingularValues;
                for (int j = 0; j < s1.Length; j++) Assert.LessOrEqual(Math.Abs(s1[j] - s2[j]), Tolerance);
                for (int i = 1; i < m.Columns; i++)
                {
                    svd = new SingularValueDecomposition(m.ViewPart(0, 0, m.Rows, 1 + i));
                    d = m.ViewColumn(i);
                    incSvd.Update(d, false);
                    s1 = svd.SingularValues;
                    s2 = incSvd.SingularValues;
                    for (int j = 0; j < s1.Length; j++) Assert.LessOrEqual(Math.Abs(s1[j] - s2[j]), Tolerance);
                }
            }
        }
    }
}
