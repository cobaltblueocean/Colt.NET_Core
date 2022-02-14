// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by OpenGamma Inc.
//
// Copyright (C) 2012 - present by OpenGamma Inc. and the OpenGamma group of companies
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
using NUnit.Framework;
using Cern.Jet.Stat;
using Cern.Jet.Random;
using Cern.Jet.Random.Engine;
using System.Runtime.CompilerServices;

namespace Cern.Colt.Tests
{
    /// <summary>
    /// RandomNormalTest Description
    /// </summary>
    public class RandomNormalTest
    {
        private Normal _normal;
        private double _mean;
        private double _standardDeviation;

        [Test]
        public void TestMain()
        {
            RandomEngine RANDOM = new MersenneTwister(MersenneTwister.DefaultSeed);
            _mean = 0;
            _standardDeviation = 1;
            _normal = new Normal(_mean, _standardDeviation, RANDOM);

            double random = _normal.NextDouble();

            Assert.Pass("Get random value: " + random);
        }
    }
}
