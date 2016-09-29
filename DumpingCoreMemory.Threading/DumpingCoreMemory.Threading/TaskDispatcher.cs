using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DumpingCoreMemory.Threading
{
    /// <summary>
    /// The implementation of the <see cref="ITaskDispatcher"/> interface
    /// </summary>
    public class TaskDispatcher : ITaskDispatcher
    {
        private readonly CancellationTokenSource _tokenSource;
        private readonly List<Task> _executingTasks;
        private readonly Semaphore _tasks;

        /// <summary>
        /// The <see cref="TaskDispatcher"/> constructor
        /// </summary>
        /// <param name="maxTasks">The maximum number of tasks that can be dispatched before the call to <seealso cref="Dispatch"/> blocks</param>
        public TaskDispatcher(Int32 maxTasks)
        {
            _executingTasks = new List<Task>();
            _tasks = new Semaphore(maxTasks, maxTasks);
            _tokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Dispatches an <see cref="Action"/> for execution. 
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to dispatch</param>
        /// <param name="completionCallback">The callback action to invoke when the task completes</param>
        /// <param name="dispatchTimeout">The <see cref="TimeSpan"/> to wait for a task to be dispatched</param>
        /// <exception cref="TimeoutException">An exception thrown when the <see cref="action"/> cannot be dispatched within the <paramref name="dispatchTimeout"/> </exception>
        public void Dispatch(Action<CancellationToken> action, Action<Task,Exception> completionCallback, TimeSpan dispatchTimeout)
        {
            if (_tasks.WaitOne(dispatchTimeout))
            {
                lock (_executingTasks)
                {
                    var task = new Task(() => action(_tokenSource.Token));
                    task.ContinueWith(t => Completion(t, completionCallback));
                    task.Start();

                    _executingTasks.Add(task);
                }
            }
            else
            {
                throw new TimeoutException(String.Format("Unable to dispatch action within timeout {0}", dispatchTimeout));
            }
        }

        /// <summary>
        /// Cancel all executing <see cref="Task"/>
        /// </summary>
        public void CancelAll()
        {
            _tokenSource.Cancel();

            Task.WaitAll(_executingTasks.ToArray());
        }

        /// <summary>
        /// The number of <see cref="Task"/> instances currently executing
        /// </summary>
        public int ExecutingTaskCount
        {
            get { return _executingTasks.Count; }
        }

        /// <summary>
        /// The completion callback thats called when a dispatch task was completed
        /// </summary>
        /// <param name="completedTask">The <see cref="Task"/> that completed</param>
        /// <param name="completionCallback">The callback that is invoked when the dispatched <see cref="Task"/> executes to completion</param>
        private void Completion(Task completedTask, Action<Task,Exception> completionCallback)
        {
            _tasks.Release();

            lock (_executingTasks)
            {
                _executingTasks.Remove(completedTask);
            }

            completionCallback(completedTask, completedTask.Exception);
        }
    }
}
