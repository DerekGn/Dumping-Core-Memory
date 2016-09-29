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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DumpingCoreMemory.ResourcePooling;

namespace TestClient
{
    class Program
    {
        private const int BufferSize = 10485760;

        static void Main(string[] args)
        {
            ResourcePoolCounters.Register();

            var dynamicResourcePool = new DynamicResourcePool<ByteBuffer>(() => new ByteBuffer(BufferSize));
            var pool = dynamicResourcePool;

            Console.WriteLine("Memory used before Allocation:\t\t{0:N0}", GC.GetTotalMemory(false));
            Console.WriteLine("Memory to be Allocated:\t\t\t{0:N0}", BufferSize * 20);

            var tokenSource = new CancellationTokenSource();
            List<Task> tasks = CreateTasks(20, 100, pool, tokenSource);
            
            Thread.Sleep(new TimeSpan(0,0,1,30));

            tokenSource.Cancel();

            WaitForTasks(tasks);

            Console.WriteLine("Memory used after Allocation:\t\t{0:N0}", GC.GetTotalMemory(false));

            GC.Collect();

            Console.WriteLine("Memory used after GC:\t\t\t{0:N0}", GC.GetTotalMemory(false));

            ResourcePoolCounters.UnRegister();
        }

        private static void WaitForTasks(List<Task> tasks)
        {
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                Console.WriteLine("\nAggregateException thrown with the following inner exceptions:");
                
                foreach (var v in e.InnerExceptions)
                {
                    if (v is TaskCanceledException)
                        Console.WriteLine("   TaskCanceledException: Task {0}",((TaskCanceledException)v).Task.Id);
                    else
                        Console.WriteLine("   Exception: {0}", v.GetType().Name);
                }
            } 
        }
        
        private static List<Task> CreateTasks(Int32 taskCount, Int32 delay, DynamicResourcePool<ByteBuffer> pool, CancellationTokenSource tokenSource)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < taskCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(() => ResourceAllocation(pool, delay, tokenSource.Token), tokenSource.Token));
            }

            return tasks;
        }

        public static void ResourceAllocation(IDynamicResourcePool<ByteBuffer> dynamicResourcePool, Int32 delay, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                ByteBuffer buffer = dynamicResourcePool.AcquireResource();

                Thread.Sleep(delay);

                dynamicResourcePool.ReleaseResource(buffer);    
            }
        }
    }
}
