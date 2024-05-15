using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Cern.Jet.Random.Engine;

namespace Cern.Colt.Tests
{
    [Parallelizable(ParallelScope.ContextMask)]
    public class RandomTest
    {
        [Test]
        public void Test1()
        {
            var RANDOM = new MersenneTwister(MersenneTwister.DefaultSeed);
            RANDOM.NextDouble();
            ClassicAssert.Pass();
        }
    }
}
