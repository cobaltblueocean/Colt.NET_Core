// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleMatrix2DTest.cs" company="Università Politecnica delle Marche">
//   Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Test for 2-d matrix of doubles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Tests
{
    using Cern.Colt.Matrix;
    using Cern.Colt.Matrix.Implementation;

    using Cern.Colt.Matrix.LinearAlgebra;

    using NUnit.Framework;
    using Cern.Colt.Function;
    using _algebra = Cern.Colt.Matrix.LinearAlgebra.Algebra;

    /// <summary>
    /// Test for 2-d matrix of doubles.
    /// </summary>
    [TestFixture]
    public class DoubleMatrix2DTest
    {
        /// <summary>
        /// Test column views.
        /// </summary>
        [Test]
        public void TestViewColumn()
        {
            var a = DoubleFactory2D.Dense.Ascending(5, 4);
            DoubleMatrix1D d = a.ViewColumn(0);
            Assert.AreEqual(5, d.Size);
            Assert.AreEqual(1, d[0]);
            Assert.AreEqual(5, d[1]);
            Assert.AreEqual(9, d[2]);
            Assert.AreEqual(13, d[3]);
            Assert.AreEqual(17, d[4]);

            var b = new DenseDoubleMatrix2D(new[]
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
            d = b.ViewColumn(0);
            Assert.AreEqual(1, d[0]);
            Assert.AreEqual(1, d[1]);
            Assert.AreEqual(1, d[2]);
            Assert.AreEqual(0, d[3]);
        }

        /// <summary>
        /// Test addition.
        /// </summary>
        [Test]
        public void TestOperations()
        {
            var a = new DenseDoubleMatrix2D(new[] { new[] { 3d, 6d }, new[] { 5d, 8d }, new[] { -2d, 9d } });
            var b = new DenseDoubleMatrix2D(new[] { new[] { -6d, 1d }, new[] { 0d, 9d }, new[] { 8d, 3d } });
            Assert.AreEqual(new DenseDoubleMatrix2D(new[] { new[] { -3d, 7d }, new[] { 5d, 17d }, new[] { 6d, 12d } }), a.Copy().Assign(b, BinaryFunctions.Plus));
            Assert.AreEqual(new DenseDoubleMatrix2D(new[] { new[] { 9d, 5d }, new[] { 5d, -1d }, new[] { -10d, 6d } }), a.Copy().Assign(b, BinaryFunctions.Minus));
            Assert.AreEqual(new DenseDoubleMatrix2D(new[] { new[] { 6d, 12d }, new[] { 10d, 16d }, new[] { -4d, 18d } }), a.Copy().Assign(UnaryFunctions.Mult(2d)));
            Assert.AreEqual(new DenseDoubleMatrix2D(new[] { new[] { 1.5d, 3d }, new[] { 2.5d, 4d }, new[] { -1d, 4.5d } }), a.Copy().Assign(UnaryFunctions.Div(2d)));
            var c =
                new DenseDoubleMatrix2D(
                    new[] { new[] { 4d, 1d, 9d }, new[] { 6d, 2d, 8d }, new[] { 7d, 3d, 5d }, new[] { 11d, 10d, 12d } });
            var d =
                new DenseDoubleMatrix2D(new[] { new[] { 2d, 9d }, new[] { 5d, 12d }, new[] { 8d, 10d } });
            Assert.AreEqual(
                new DenseDoubleMatrix2D(new[] { new[] { 85d, 138d }, new[] { 86d, 158d }, new[] { 69d, 149d }, new[] { 168d, 339d } }), Algebra.Mult(c, d));
        }


        [Test]
        public void TestInverse()
        {
            double[][] indep = new double[2][]{ new double[2]{ 1000d, 4.534295808307033d }, new double[2]{ 4.534295808307033d, 83.9744901410694d } };
            DoubleMatrix2D matrix = DoubleFactory2D.Dense.Make(indep);

            Assert.IsTrue(Diagonal.Inverse(matrix));

            //DoubleMatrix2D invExpected = DoubleFactory2D.Dense.Make(new double[2][] { new double[2] { -380765.17865137121d, 1727501.9535081913d }, new double[2] { 83.9744901410694d, -380.76517865137123d } });
            DoubleMatrix2D invExpected = DoubleFactory2D.Dense.Make(new double[2][] { new double[2] { -5.792061672297568E-4, 0.22054153974121438 }, new double[2] { 0.22054153974121438, -4.863854257968181E-5 } });

            DoubleMatrix2D invResult = _algebra.Inverse(matrix);

            Assert.AreEqual(invResult.ToArray(), invExpected.ToArray());
        }
    }
}
