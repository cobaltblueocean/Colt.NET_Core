// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by Cern.Colt.Matrix.LinearAlgebra Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.LinearAlgebra
{
    /// <summary>
    /// For diagonal matrices we can often do better.
    /// </summary>
    public static class Diagonal
    {
        public static Boolean Inverse(DoubleMatrix2D A)
        {
            //Property.DEFAULT.checkSquare(A);
            //A.IsSquare;

            Boolean isNonSingular = true;
            for (int i = A.Rows; --i >= 0;)
            {
                double v = A[i, i];
                isNonSingular &= (v != 0);
                A[i, i] = ( 1 / v);
            }
            return isNonSingular;
        }
    }
}
