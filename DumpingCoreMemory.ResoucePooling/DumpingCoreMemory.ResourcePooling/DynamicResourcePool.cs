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
using System.Collections.Concurrent;
using System.Diagnostics;

namespace DumpingCoreMemory.ResourcePooling
{
    public class DynamicResourcePool<T> : IDynamicResourcePool<T> where T : class
    {
        private readonly ConcurrentQueue<WeakReference> _resources;

        private readonly PerformanceCounter _rateOfCreation;
        private readonly PerformanceCounter _rateOfRelease;
        private readonly PerformanceCounter _rateOfAquire;
        private readonly PerformanceCounter _totalResources;
        private readonly Func<T> _factoryFunc;

        /// <summary>
        /// Construct an instance of a <see cref="DynamicResourcePool{T}"/>
        /// </summary>
        /// <param name="resourceFactoryFunc">The factory used to create resources</param>
        public DynamicResourcePool(Func<T> resourceFactoryFunc)
        {
            _resources = new ConcurrentQueue<WeakReference>();
            _factoryFunc = resourceFactoryFunc;
            
            if(PerformanceCounterCategory.Exists(CounterMetadata.CategoryName))
            {
                String instanceName = GetType().GetGenericArguments()[0].Name;

                _rateOfCreation = new PerformanceCounter(CounterMetadata.CategoryName, CounterMetadata.CreationCounterName, instanceName, false);
                _rateOfRelease = new PerformanceCounter(CounterMetadata.CategoryName, CounterMetadata.ReleasedCounterName, instanceName, false);
                _rateOfAquire = new PerformanceCounter(CounterMetadata.CategoryName, CounterMetadata.AcquiredCounterName, instanceName, false);
                _totalResources = new PerformanceCounter(CounterMetadata.CategoryName, CounterMetadata.TotalCreatedCounterName, instanceName, false);
            }
        }
        
        /// <summary>
        /// Acquires a resource from the pool.
        /// </summary>
        /// <returns>An instance of a resource from the pool</returns>
        public T AcquireResource()
        {
            IncrementCounter(_rateOfAquire);
            WeakReference weakReference;
            T resource = default(T);

            if(_resources.TryDequeue(out weakReference))
            {
		        resource = (T)(weakReference.Target);

                if(resource == null)
                {
                    DecrementCounter(_totalResources);
                }
	        }
	
	        if(resource == null)
	        {
                if(_factoryFunc != null)
                {
                    IncrementCounter(_totalResources);
                    IncrementCounter(_rateOfCreation);
                    resource = _factoryFunc();
                }
	        }

	        return resource;
        }

        /// <summary>
        /// Release a resource back to the pool
        /// </summary>
        /// <param name="resource">The resource to release</param>
        /// <exception cref="ArgumentNullException">Thrown when the argument is resource is null</exception>
        public void ReleaseResource(T resource)
        {
            IncrementCounter(_rateOfRelease);

            if(resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            _resources.Enqueue(new WeakReference(resource));
        }

        private static void DecrementCounter(PerformanceCounter counter)
        {
            if (counter != null)
            {
                counter.Decrement();
            }
        }

        private static void IncrementCounter(PerformanceCounter counter)
        {
            if (counter != null)
            {
                counter.Increment();
            }
        }
    }
}
