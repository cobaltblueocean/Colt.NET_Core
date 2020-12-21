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
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    public class AutoParallelOptions: ParallelOptions
    {
        public long Threshold { get; set; }

        public AutoParallelOptions(ParallelOptions options)
        {
            Threshold = 100000;
        }

        public AutoParallelOptions(ParallelOptions options, long threshold)
        {
            if (threshold <= 0)
                throw new ArgumentOutOfRangeException("threshold", Cern.LocalizedResources.Instance().AutoParallel_ThresholdValueNegative);

            Threshold = threshold;
        }
    }
}
