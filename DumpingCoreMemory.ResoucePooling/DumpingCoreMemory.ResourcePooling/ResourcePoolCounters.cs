/**
* MIT License
*
* Copyright (c) 2016 Derek Goslin < http://corememorydump.blogspot.ie/ >
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System.Collections.Generic;
using System.Diagnostics;

namespace DumpingCoreMemory.ResourcePooling
{
    public class ResourcePoolCounters
    {
        public static void Register()
        {
            var counters = new List<CounterCreationData>
                               {
                                   new CounterCreationData(CounterMetadata.CreationCounterName,
                                                           CounterMetadata.CreationCounterHelp,
                                                           PerformanceCounterType.RateOfCountsPerSecond32),
                                   new CounterCreationData(CounterMetadata.AcquiredCounterName,
                                                           CounterMetadata.AcquiredCounterHelp,
                                                           PerformanceCounterType.RateOfCountsPerSecond32),
                                   new CounterCreationData(CounterMetadata.ReleasedCounterName,
                                                           CounterMetadata.ReleasedCounterHelp,
                                                           PerformanceCounterType.RateOfCountsPerSecond32),
                                   new CounterCreationData(CounterMetadata.TotalCreatedCounterName,
                                                           CounterMetadata.TotalCreatedCounterHelp,
                                                           PerformanceCounterType.NumberOfItems32)
                               };

            var ccds = new CounterCreationDataCollection(counters.ToArray());

            if (!PerformanceCounterCategory.Exists(CounterMetadata.CategoryName))
            {
                PerformanceCounterCategory.Create(CounterMetadata.CategoryName, "",
                                                  PerformanceCounterCategoryType.MultiInstance, ccds);
            }
        }

        public static void UnRegister()
        {
            if (PerformanceCounterCategory.Exists(CounterMetadata.CategoryName))
            {
                PerformanceCounterCategory.Delete(CounterMetadata.CategoryName);
            }
        }
    }
}