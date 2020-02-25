// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by Cern.Colt.Matrix.LinearAlgebra Inc.
//
// Copyright (C) 2012 - present by Cern.Colt.Matrix.LinearAlgebra Incd and the Cern.Colt.Matrix.LinearAlgebra group of companies
//
// Please see distribution for license.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//     
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.LinearAlgebra
{
    /// <summary>
    /// LUDecomposition Description
    /// </summary>
    [Serializable]
    public class LUDecomposition
    {
        protected LUDecompositionQuick quick;

        /// <summary>
        /// Constructs and returns a new LU Decomposition object; 
        /// The decomposed matrices can be retrieved via instance methods of the returned decomposition object.
        /// Return structure to access L, U and piv.
        /// </summary>
        /// <param name="A">Rectangular matrix</param>
        public LUDecomposition(DoubleMatrix2D A)
        {
            quick = new LUDecompositionQuick(0); // zero tolerance for compatibility with Jama
            quick.Decompose(A.Copy());
        }

        /// <summary>
        /// Returns the determinant, <i>det(A)</i>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Matrix must be square</exception>
        public double Det()
        {
            return quick.Det();
        }

        /// <summary>
        /// Returns pivot permutation vector as a one-dimensional double array
        /// </summary>
        private double[] DoublePivot
        {
            get
            {
                return quick.DoublePivot;
            }
        }

        /// <summary>
        /// Returns the lower triangular factor, <i>L</i>.
        /// </summary>
        public DoubleMatrix2D L
        {
            get
            {
                return quick.L;
            }
        }

        /// <summary>
        /// Returns a copy of the pivot permutation vector.
        /// </summary>
        public int[] Pivot
        {
            get
            {
                return (int[])quick.Pivot.Clone();
            }
        }

        /// <summary>
        /// Returns the upper triangular factor, <i>U</i>.
        /// </summary>
        public DoubleMatrix2D U
        {
            get
            {
                return quick.U;
            }
        }

        /// <summary>
        /// Returns whether the matrix is nonsingular (has an inverse).
        /// true if <i>U</i>, and hence <i>A</i>, is nonsingular; false otherwise.
        /// </summary>
        public Boolean IsNonsingular
        {
            get
            {
                return quick.IsNonsingular;
            }
        }

        /// <summary>
        /// Solves <i>A*X = B</i>.
        /// </summary>
        /// <param name="B">A matrix with as many rows as <i>A</i> and any number of columns.</param>
        /// <returns><i>X</i> so that <i>L*U*X = B(piv,:)</i>.</returns>
        /// <exception cref="ArgumentException">if B.rows() != A.rows().</exception>
        /// <exception cref="ArgumentException">if A is singular, that is, if !this.isNonsingular().</exception>
        /// <exception cref="ArgumentException">if A.rows() &lt; A.columns().</exception>
        public DoubleMatrix2D Solve(DoubleMatrix2D B)
        {
            DoubleMatrix2D X = B.Copy();
            quick.Solve(X);
            return X;
        }

        /// <summary>
        /// Returns a String with (propertyName, propertyValue) pairs.
        /// Useful for debugging or to quickly get the rough picture.
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// rank          : 3
        /// trace         : 0
        /// </example>
        public override String ToString()
        {
            return quick.ToString();
        }
    }
}
