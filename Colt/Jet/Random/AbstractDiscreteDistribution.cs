using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Random
{
    /// <summary>
    /// Abstract base class for all discrete distributions.
    /// </summary>
    public abstract class AbstractDiscreteDistribution: AbstractDistribution
    {
        /// <summary>
        /// Returns a random number from the distribution; returns <see cref="NextInt()"/>.
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            return (double)NextInt();
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public new abstract int NextInt();
    }
}
