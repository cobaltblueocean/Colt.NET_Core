using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Cern.Colt.Matrix.LinearAlgebra;
using Cern.Colt.Matrix;

namespace Cern.Colt.Tests
{
    public class DecompositionTest
    {
        private const Double delta = 1e-10;

        [Test]
        public void DecompositionTest1()
        {
            var matrix = DoubleFactory2D.Dense.Make(new double[,] { { 1000, 12.41257184938517 }, { 12.41257184938517, 83.92041889242871 } });
            var expect = DoubleFactory2D.Dense.Make(new double[,] { { 0.001001839305944707, -1.4818089007059005E-4 }, { -1.4818089007059005E-4, 0.011937968365349673 } });

            matrix = Algebra.Inverse(matrix);


            for(int i=0; i< expect.Rows; i++)
            {
                for(int j =0; j<expect.Columns; j++)
                {
                    Assert.AreEqual(expect[i, j], matrix[i, j], delta);
                }
            }
        }
    }
}
