using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix;
using Cern.Colt.Matrix.Implementation;
using Cern.Colt.Matrix.LinearAlgebra;
using NUnit.Framework;

namespace Colt.Tests
{
    public class NormInfinityTest
    {
        public static void main(String[] args)
        {
            DoubleMatrix1D x1 = DoubleFactory1D.Dense
                    .Make(new double[] { 1.0, 2.0 });
            DoubleMatrix1D x2 = DoubleFactory1D.Dense
                    .Make(new double[] { 1.0, -2.0 });
            DoubleMatrix1D x3 = DoubleFactory1D.Dense.Make(new double[] { -1.0, -2.0 });

            Assert.Inconclusive(Algebra.NormInfinity(x1).ToString());
            Assert.Inconclusive(Algebra.NormInfinity(x2).ToString());
            Assert.Inconclusive(Algebra.NormInfinity(x3).ToString());
        }
    }
}
