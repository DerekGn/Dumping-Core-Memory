using System;
using System.Threading;
using System.Threading.Tasks;
using DumpingCoreMemory.Threading;

namespace TestClient
{
    class Program
    {
        private static readonly ManualResetEvent WaitHandle = new ManualResetEvent(false);
        private static Int32 _dispatchCount;

        static void Main(string[] args)
        {
            DispatchTasks(10, 10, BlockingDispatchedTask);

            DispatchTasks(10, 10, ExceptionOnDispatchedTask);

            try
            {
                DispatchTasks(1, 2, BlockingDispatchedTask);
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine("Unable to dispatch within timeout");
            }

            var taskDispatcher = new TaskDispatcher(10);

            taskDispatcher.Dispatch(CancellableTask, 
                (completedTask, exception) => Console.WriteLine("Task: {0} State: {1}, Exception: {2}", completedTask.Id, completedTask.Status, exception == null ? "None" : exception.Message), 
                new TimeSpan(0, 0, 1));

            taskDispatcher.CancelAll();
        }

        private static void DispatchTasks(Int32 maxTasks, Int32 tasksToDispatch, Action<CancellationToken> actionToDispatch)
        {
            _dispatchCount = 0;
            WaitHandle.Reset();

            var taskDisptcher = new TaskDispatcher(maxTasks);

            for (int i = 0; i < tasksToDispatch; i++)
            {
                taskDisptcher.Dispatch(actionToDispatch, CompletionCallback, new TimeSpan(0, 0, 1)); 
            }

            WaitHandle.WaitOne();
        }

        private static void CompletionCallback(Task completedTask, Exception exception)
        {
            Console.WriteLine("Task: {0} State: {1}, Exception: {2}", completedTask.Id, completedTask.Status, exception == null ? "None" : exception.Message);

            if (++_dispatchCount == 5)
            {
                WaitHandle.Set();
            }
        }

        static void BlockingDispatchedTask(CancellationToken cancellationToken)
        {
            Console.WriteLine("Task: {0} Dispatched", Task.CurrentId);

            Thread.Sleep(1000);
        }

        static void ExceptionOnDispatchedTask(CancellationToken cancellationToken)
        {
            Console.WriteLine("Task: {0} Dispatched throwing exception", Task.CurrentId);

            throw new Exception("Error");
        }

        static void CancellableTask(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100000; i++)
            {
                // Simulating work.
                Thread.SpinWait(5000000);

                if (cancellationToken.IsCancellationRequested)
                {
                    // Perform cleanup if necessary.
                    //...
                    // Terminate the operation.
                    break;
                }
            }
        }
    }
}
