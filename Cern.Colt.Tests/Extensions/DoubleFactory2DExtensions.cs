using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix;

namespace Cern.Colt.Matrix
{
    public static class DoubleFactory2DExtensions
    {
        /// <summary>
        /// Constructs a randomly sampled matrix with the given shape.
        /// Randomly picks exactly<tt>Math.round(rows* columns* nonZeroFraction)</tt> cells and initializes them to<tt> value</tt>, all the rest will be initialized to zero.
        /// Note that this is not the same as setting each cell with probability<tt> nonZeroFraction</tt> to<tt> value</tt>.
        /// Note: The random seed is a constant.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="value"></param>
        /// <param name="nonZeroFraction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if nonZeroFraction &lt; 0 || nonZeroFraction > 1.</exception>
        /// <see cref="Cern.Jet.Random.Sampling.RandomSamplingAssistant"/>
        public static IDoubleMatrix2D Sample(this DoubleFactory2D factory, int rows, int columns, double value, double nonZeroFraction)
        {
            IDoubleMatrix2D matrix = factory.Make(rows, columns);
            return Sample(factory, matrix, value, nonZeroFraction);
        }

        /// <summary>
        /// Modifies the given matrix to be a randomly sampled matrix.
        /// Randomly picks exactly <i>System.Math.Round(rows*columns*nonZeroFraction)</i> cells and initializes them to <i>value</i>, all the rest will be initialized to zero.
        /// Note that this is not the same as setting each cell with probability <i>nonZeroFraction</i> to <i>value</i>.
        /// Note: The random seed is a constant.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="matrix"></param>
        /// <param name="value"></param>
        /// <param name="nonZeroFraction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if nonZeroFraction &lt; 0 || nonZeroFraction > 1.</exception>
        /// <see cref="Cern.Jet.Random.Sampling.RandomSamplingAssistant"/>
        public static IDoubleMatrix2D Sample(this DoubleFactory2D factory, IDoubleMatrix2D matrix, double value, double nonZeroFraction)
        {
            int rows = matrix.Rows;
            int columns = matrix.Columns;
            double epsilon = 1e-09;
            if (nonZeroFraction < 0 - epsilon || nonZeroFraction > 1 + epsilon) throw new ArgumentException();
            if (nonZeroFraction < 0) nonZeroFraction = 0;
            if (nonZeroFraction > 1) nonZeroFraction = 1;

            matrix.Assign(0);

            int size = rows * columns;
            int n = (int)System.Math.Round(size * nonZeroFraction);
            if (n == 0) return matrix;

            var sampler = new Cern.Jet.Random.Sampling.RandomSamplingAssistant(n, size, new Cern.Jet.Random.Engine.MersenneTwister());
            for (int i = 0; i < size; i++)
            {
                if (sampler.SampleNextElement())
                {
                    int row = (int)(i / columns);
                    int column = (int)(i % columns);
                    matrix[row, column] = value;
                }
            }

            return matrix;
        }
    }
}
