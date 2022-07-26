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

namespace Cern.Colt.Tests
{
    /// <summary>
    /// AutoParallelTest Description
    /// </summary>
    public class AutoParallelTest
    {
        [Test]
        public void Test()
        {
            // This test cannot go through in Debug mode due to parallel process at the Parallel.For() and AutoParallel.AutoParallelFor with ParallelMode.ForceParallel parameter.
            // If we ran in Debug mode, the process will return incorrect data due to thread handling.

            int fromIndex = 0;
            int toIndex = 5;
            List<int> result = new List<int>();
            int[] resultArrray;

            Action<int> action = ((x) =>{
                result.Add(x);
            });


            Parallel.For(fromIndex, toIndex, action);
            resultArrray = result.ToArray();

            int[] expected = new int[] { 0, 1, 2, 3, 4 };

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], resultArrray[i]);
            }

            result.Clear();

            AutoParallel.AutoParallelFor(fromIndex, toIndex, action);
            resultArrray = result.ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], resultArrray[i]);
            }

            fromIndex = 7;
            toIndex = 2;

            expected = new int[] { 6, 5, 4, 3, 2 };
            result.Clear();
            
            AutoParallel.AutoParallelFor(fromIndex, toIndex, action);
            resultArrray = result.ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], resultArrray[i]);
            }

            AutoParallel.AutoParallelFor(fromIndex, toIndex, action, ParallelMode.ForceParallel);
            resultArrray = result.ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], resultArrray[i]);
            }

            expected = new int[] { 7, 6, 5, 4, 3, 2 };
            result.Clear();

            AutoParallel.AutoParallelFor(fromIndex, toIndex, action, true);
            resultArrray = result.ToArray();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], resultArrray[i]);
            }
        }
    }
}
