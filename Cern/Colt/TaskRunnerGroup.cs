// <copyright file="TaskRunningGroup.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cern.Colt
{
    /// <summary>
    /// Implement light weight thread pool.
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/435668/code-for-a-simple-thread-pool-in-c-sharp"/>
    public sealed class TaskRunnerGroup : IDisposable
    {
        public TaskRunnerGroup(int size)
        {
            this._workers = new LinkedList<Thread>();
            AutoParallel.AutoParallelFor(0, size, (i) =>
             {
                 var worker = new Thread(this.Worker) { Name = string.Concat("Worker ", i) };
                 worker.Start();
                 this._workers.AddLast(worker);
             });
        }

        public void Dispose()
        {
            var waitForThreads = false;
            lock (this._tasks)
            {
                if (!this._disposed)
                {
                    GC.SuppressFinalize(this);

                    this._disallowAdd = true; // wait for all tasks to finish processing while not allowing any more new tasks
                    while (this._tasks.Count > 0)
                    {
                        Monitor.Wait(this._tasks);
                    }

                    this._disposed = true;
                    Monitor.PulseAll(this._tasks); // wake all workers (none of them will be active at this point; disposed flag will cause then to finish so that we can join them)
                    waitForThreads = true;
                }
            }
            if (waitForThreads)
            {
                AutoParallel.AutoParallelForEach(this._workers, (worker) =>
               {
                   worker.Join();
               });
            }
        }

        public void QueueTask(Action task)
        {
            lock (this._tasks)
            {
                if (this._disallowAdd) { throw new InvalidOperationException(Cern.LocalizedResources.Instance().Exception_ThisPoolInstanceIsInTheProcess); }
                if (this._disposed) { throw new ObjectDisposedException(Cern.LocalizedResources.Instance().Exception_ThisPoolProcessHasAlreadyBeenDisposed); }
                this._tasks.AddLast(task);
                Monitor.PulseAll(this._tasks); // pulse because tasks count changed
            }
        }

        private void Worker()
        {
            Action task = null;
            while (true) // loop until threadpool is disposed
            {
                lock (this._tasks) // finding a task needs to be atomic
                {
                    while (true) // wait for our turn in _workers queue and an available task
                    {
                        if (this._disposed)
                        {
                            return;
                        }
                        if (null != this._workers.First && object.ReferenceEquals(Thread.CurrentThread, this._workers.First.Value) && this._tasks.Count > 0) // we can only claim a task if its our turn (this worker thread is the first entry in _worker queue) and there is a task available
                        {
                            task = this._tasks.First.Value;
                            this._tasks.RemoveFirst();
                            this._workers.RemoveFirst();
                            Monitor.PulseAll(this._tasks); // pulse because current (First) worker changed (so that next available sleeping worker will pick up its task)
                            break; // we found a task to process, break out from the above 'while (true)' loop
                        }
                        Monitor.Wait(this._tasks); // go to sleep, either not our turn or no task to process
                    }
                }

                task(); // process the found task
                lock (this._tasks)
                {
                    this._workers.AddLast(Thread.CurrentThread);
                }
                task = null;
            }
        }

        public void Statistics()
        {
            foreach (var t in this._tasks)
            {
                Console.WriteLine(t.ToString());
            }
        }

        private readonly LinkedList<Thread> _workers; // queue of worker threads ready to process actions
        private readonly LinkedList<Action> _tasks = new LinkedList<Action>(); // actions to be processed by worker threads
        private bool _disallowAdd; // set to true when disposing queue but there are still tasks pending
        private bool _disposed; // set to true when disposing queue and no more tasks are pending
    }
}
