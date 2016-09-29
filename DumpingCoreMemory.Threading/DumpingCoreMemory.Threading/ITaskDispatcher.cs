using System;
using System.Threading;
using System.Threading.Tasks;

namespace DumpingCoreMemory.Threading
{
    public interface ITaskDispatcher
    {
        /// <summary>
        /// Dispatches an <see cref="Action"/> for execution. 
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to dispatch</param>
        /// <param name="completionCallback">The callback action to invoke when the task completes</param>
        /// <param name="dispatchTimeout">The <see cref="TimeSpan"/> to wait for a task to be dispatched</param>
        void Dispatch(Action<CancellationToken> action, Action<Task,Exception> completionCallback, TimeSpan dispatchTimeout);

        /// <summary>
        /// Cancel all executing <see cref="Task"/>
        /// </summary>
        void CancelAll();
        
        /// <summary>
        /// The number of <see cref="Task"/> instances currently executing
        /// </summary>
        Int32 ExecutingTaskCount { get; }
    }
}
