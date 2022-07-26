// Copyright (c) 2017 - presented by Kei Nakai
// Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
// is hereby granted without fee, provided that the above copyright notice appear in all copies and 
// that both that copyright notice and this permission notice appear in supporting documentation. 
// CERN makes no representations about the suitability of this software for any purpose. 
// It is provided "as is" without expressed or implied warranty.
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

namespace System.Threading.Tasks
{
    /// <summary>
    /// Handling Parallel loop method to automate manually/automatically using traditional loop or parallel method
    /// </summary>
    public static class AutoParallel
    {
        private const int _threshold = 100000;


        #region ForEach
        #region IEnumerable
        public static void AutoParallelForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            AutoParallelForEach(source, action, ParallelMode.Auto);
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, Action<T> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.ForEach(source, action);
            }
            else
            {
                foreach (var s in source)
                {
                    action(s);
                }
            }
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, Action<T, ParallelLoopState> action)
        {
            AutoParallelForEach(source, action, ParallelMode.Auto);
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, Action<T, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.ForEach(source, action);
            }
            else
            {
                foreach (var s in source)
                {
                    action(s, null);
                }
            }
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, ParallelOptions options, Action<T, ParallelLoopState> action)
        {
            AutoParallelForEach(source, new AutoParallelOptions(options, _threshold), action, ParallelMode.Auto);
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, AutoParallelOptions options, Action<T, ParallelLoopState> action)
        {
            AutoParallelForEach(source, options, action, ParallelMode.Auto);
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, AutoParallelOptions options, Action<T, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > options.Threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.ForEach(source, options, action);
            }
            else
            {
                foreach (var s in source)
                {
                    action(s, null);
                }
            }
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, ParallelOptions options, Action<T, ParallelLoopState, Int64> action)
        {
            AutoParallelForEach(source, new AutoParallelOptions(options, _threshold), action, ParallelMode.Auto);
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, AutoParallelOptions options, Action<T, ParallelLoopState, Int64> action)
        {
            AutoParallelForEach(source, options, action, ParallelMode.Auto);
        }

        public static void AutoParallelForEach<T>(this IEnumerable<T> source, AutoParallelOptions options, Action<T, ParallelLoopState, Int64> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > options.Threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.ForEach(source, options, action);
            }
            else
            {
                Int64 index = 0;
                foreach (var s in source)
                {
                    action(s, null, index);
                    index++;
                }
            }
        }
        #endregion
        #endregion

        #region For
        #region IEnumerable

        public static void AutoParallelFor(int fromInclusice, int toExclusive, Action<int> action)
        {
            AutoParallelFor(fromInclusice, toExclusive, action, false);
        }

        public static void AutoParallelFor(int fromInclusice, int toExclusive, Action<int> action, Boolean includeToExclusive)
        {
            AutoParallelFor(fromInclusice, toExclusive, action, includeToExclusive, ParallelMode.Auto);
        }

        public static void AutoParallelFor(int fromInclusice, int toExclusive, Action<int> action, ParallelMode parallelMode)
        {
            AutoParallelFor(fromInclusice, toExclusive, action, false, parallelMode);
        }

        public static void AutoParallelFor(int fromInclusice, int toExclusive, Action<int> action, Boolean includeToExclusive, ParallelMode parallelMode)
        {
            Boolean doParallel = false;
            Boolean reverseProcess = false;

            if ((toExclusive - fromInclusice) > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (includeToExclusive)
                toExclusive++;

            if (fromInclusice > toExclusive)
            {
                reverseProcess = true;
                int temp = toExclusive;
                toExclusive = fromInclusice + 1;
                fromInclusice = temp;

                if (includeToExclusive)
                    fromInclusice--;
            }

            if (doParallel)
            {
                if (!reverseProcess)
                {
                    Parallel.For(fromInclusice, toExclusive, action);
                }
                else
                {
                    Action<int> revAction = ((x) =>
                    {

                        action(x);
                    });

                    Parallel.For(fromInclusice, toExclusive, (x) =>
                    {
                        int y = toExclusive - x;
                        action(y);
                    });
                }
            }
            else
            {
                if (!reverseProcess)
                {
                    for (int i = fromInclusice; i < toExclusive; i++)
                    {
                        action(i);
                    }
                }
                else
                {
                    for (int i = fromInclusice; i < toExclusive; i++)
                    {
                        int y = toExclusive - i;

                        if (includeToExclusive)
                            y++;

                            action(y);
                    }
                }
            }
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, Action<int> action)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, action, false);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, Action<int> action, Boolean includeToExclusive)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, action, includeToExclusive, ParallelMode.Auto);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, Action<int> action, ParallelMode parallelMode)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, action, false, parallelMode);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, Action<int> action, Boolean includeToExclusive, ParallelMode parallelMode)
        {
            Boolean doParallel = false;
            Boolean reverseProcess = false;

            if (source.Count() > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (includeToExclusive)
                toExclusive++;

            if (fromInclusice > toExclusive)
            {
                reverseProcess = true;
                int temp = toExclusive;
                toExclusive = fromInclusice + 1;
                fromInclusice = temp;

                if (includeToExclusive)
                    fromInclusice--;
            }

            if (doParallel)
            {
                if(!reverseProcess)
                {
                    Parallel.For(fromInclusice, toExclusive, action);
                }
                else
                {
                    Action<int> revAction = ((x) =>
                    {

                        action(x);
                    });

                    Parallel.For(fromInclusice, toExclusive, (x) =>
                    {
                        int y = toExclusive - x;
                        action(y);
                    });
                }
            }
            else
            {
                if (!reverseProcess)
                { 
                    for (int i = fromInclusice; i < toExclusive; i++)
                    {
                        action(i);
                    }
                }
                else
                {
                    for (int i = fromInclusice; i < toExclusive; i++)
                    {
                        int y = toExclusive - i;
                        action(y);
                    }
                }
            }
        }



        public static void AutoParallelFor(int fromInclusice, int toExclusive, Action<int, ParallelLoopState> action)
        {
            AutoParallelFor(fromInclusice, toExclusive, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor(int fromInclusice, int toExclusive, Action<int, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if ((toExclusive - fromInclusice) > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, action);
            }
            else
            {
                for (int i = fromInclusice; i < toExclusive; i++)
                {
                    action(i, null);
                }
            }
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, Action<int, ParallelLoopState> action)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, Action<int, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, action);
            }
            else
            {
                for (int i = fromInclusice; i < toExclusive; i++)
                {
                    action(i, null);
                }
            }
        }


        public static void AutoParallelFor(int fromInclusice, int toExclusive, ParallelOptions options, Action<int, ParallelLoopState> action)
        {
            AutoParallelFor(fromInclusice, toExclusive, new AutoParallelOptions(options, _threshold), action, ParallelMode.Auto);
        }

        public static void AutoParallelFor(int fromInclusice, int toExclusive, AutoParallelOptions options, Action<int, ParallelLoopState> action)
        {
            AutoParallelFor(fromInclusice, toExclusive, options, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor(int fromInclusice, int toExclusive, AutoParallelOptions options, Action<int, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if ((toExclusive - fromInclusice) > options.Threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, options, action);
            }
            else
            {
                for (int i = fromInclusice; i < toExclusive; i++)
                {
                    action(i, null);
                }
            }
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, ParallelOptions options, Action<int, ParallelLoopState> action)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, new AutoParallelOptions(options, _threshold), action, ParallelMode.Auto);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, AutoParallelOptions options, Action<int, ParallelLoopState> action)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, options, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, int fromInclusice, int toExclusive, AutoParallelOptions options, Action<int, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > options.Threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, options, action);
            }
            else
            {
                for (int i = fromInclusice; i < toExclusive; i++)
                {
                    action(i, null);
                }
            }
        }

        public static void AutoParallelFor(Int64 fromInclusice, Int64 toExclusive, Action<Int64> action)
        {
            AutoParallelFor(fromInclusice, toExclusive, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor(Int64 fromInclusice, Int64 toExclusive, Action<Int64> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if ((toExclusive - fromInclusice) > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, action);
            }
            else
            {
                for (Int64 i = fromInclusice; i < toExclusive; i++)
                {
                    action(i);
                }
            }
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, Int64 fromInclusice, Int64 toExclusive, Action<Int64> action)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, Int64 fromInclusice, Int64 toExclusive, Action<Int64> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, action);
            }
            else
            {
                for (Int64 i = fromInclusice; i < toExclusive; i++)
                {
                    action(i);
                }
            }
        }

        public static void AutoParallelFor(Int64 fromInclusice, Int64 toExclusive, Action<Int64, ParallelLoopState> action)
        {
            AutoParallelFor(fromInclusice, toExclusive, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor(Int64 fromInclusice, Int64 toExclusive, Action<Int64, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if ((toExclusive - fromInclusice) > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, action);
            }
            else
            {
                for (Int64 i = fromInclusice; i < toExclusive; i++)
                {
                    action(i, null);
                }
            }
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, Int64 fromInclusice, Int64 toExclusive, Action<Int64, ParallelLoopState> action)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, Int64 fromInclusice, Int64 toExclusive, Action<Int64, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > _threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, action);
            }
            else
            {
                for (Int64 i = fromInclusice; i < toExclusive; i++)
                {
                    action(i, null);
                }
            }
        }

        public static void AutoParallelFor(Int64 fromInclusice, Int64 toExclusive, ParallelOptions options, Action<Int64, ParallelLoopState> action)
        {
            AutoParallelFor(fromInclusice, toExclusive, new AutoParallelOptions(options, _threshold), action, ParallelMode.Auto);
        }

        public static void AutoParallelFor(Int64 fromInclusice, Int64 toExclusive, AutoParallelOptions options, Action<Int64, ParallelLoopState> action)
        {
            AutoParallelFor(fromInclusice, toExclusive, options, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor(Int64 fromInclusice, Int64 toExclusive, AutoParallelOptions options, Action<Int64, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if ((toExclusive - fromInclusice) > options.Threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, options, action);
            }
            else
            {
                for (Int64 i = fromInclusice; i < toExclusive; i++)
                {
                    action(i, null);
                }
            }
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, Int64 fromInclusice, Int64 toExclusive, ParallelOptions options, Action<Int64, ParallelLoopState> action)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, new AutoParallelOptions(options, _threshold), action, ParallelMode.Auto);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, Int64 fromInclusice, Int64 toExclusive, AutoParallelOptions options, Action<Int64, ParallelLoopState> action)
        {
            AutoParallelFor(source, fromInclusice, toExclusive, options, action, ParallelMode.Auto);
        }

        public static void AutoParallelFor<T>(IEnumerable<T> source, Int64 fromInclusice, Int64 toExclusive, AutoParallelOptions options, Action<Int64, ParallelLoopState> action, ParallelMode parallelMode)
        {
            Boolean doParallel = false;

            if (source.Count() > options.Threshold)
                doParallel = true;

            if (parallelMode == ParallelMode.ForceParallel)
                doParallel = true;
            else if (parallelMode == ParallelMode.NonParallel)
                doParallel = false;

            if (doParallel)
            {
                Parallel.For(fromInclusice, toExclusive, options, action);
            }
            else
            {
                for (Int64 i = fromInclusice; i < toExclusive; i++)
                {
                    action(i, null);
                }
            }
        }
        #endregion
        #endregion
    }
}
