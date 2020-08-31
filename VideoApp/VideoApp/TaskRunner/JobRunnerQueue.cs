using FFmpegUtilities;
using System;
using System.Collections.Generic;
using System.Threading;
using VideoApp.FFmpegUtilities.Models;

namespace VideoApp.Web.TaskRunner
{
    public class JobRunnerQueue : IJobRunnerQueue
    {
        public event EventHandler<CustomEventArgs> JobFinished;

        private Queue<object> _jobs = new Queue<object>();
        private bool _delegateQueuedOrRunning = false;
        //private BlockingCollection<object> _jobs = new BlockingCollection<object>();
        private CommandExecuter _commandExecuter = new CommandExecuter();

        //public JobRunnerQueue()
        //{
        //    var thread = new Thread(new ThreadStart(OnStart));
        //    thread.IsBackground = true;
        //    thread.Start();
        //}

        public void Enqueue(object job)
        {
            lock (_jobs)
            {
                _jobs.Enqueue(job);
                if (!_delegateQueuedOrRunning)
                {
                    _delegateQueuedOrRunning = true;
                    ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems, null);
                }
            }
        }
        private void ProcessQueuedItems(object ignored)
        {
            while (true)
            {
                object item;
                lock (_jobs)
                {
                    if (_jobs.Count == 0)
                    {
                        _delegateQueuedOrRunning = false;
                        break;
                    }

                    item = _jobs.Dequeue();
                }

                try
                {
                    var processStartParameters = (ProcessStartParameters)item;
                    var result = _commandExecuter.ExecuteCommand(processStartParameters);
                    var myEvents = new CustomEventArgs(processStartParameters.ParentVideoFileId, processStartParameters.ConvertedVideoFullPath, result);
                    OnJobFinished(myEvents);
                }
                catch(Exception ex)
                {
                    ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems, null);
                    throw;
                }
            }
        }

        //private void OnStart()
        //{
        //    foreach (var job in _jobs.GetConsumingEnumerable(CancellationToken.None))
        //    {
        //        var item = (ProcessStartParameters)job;
        //        var result = _commandExecuter.ExecuteCommand(item);
        //        var myEvents = new CustomEventArgs(item.ParentVideoFileId, item.ConvertedVideoFullPath, result);
        //        OnJobFinished(myEvents);
        //    }
        //}

        protected virtual void OnJobFinished(CustomEventArgs e)
        {
            JobFinished?.Invoke(this, e);
        }
    }

    public class CustomEventArgs : EventArgs
    {
        public int ParentVideoFileId { get; private set; }
        public string ConvertedFileFullpath { get; private set; }

        public bool ConversionResult { get; set; }

        public CustomEventArgs(int parentId, string fullPath, bool conversionResult) : base()
        {
            this.ParentVideoFileId = parentId;
            this.ConvertedFileFullpath = fullPath;
            this.ConversionResult = conversionResult;
        }
    }
}
