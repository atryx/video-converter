using System;
using System.Collections.Generic;
using System.Threading;
using VideoApp.Web.JobQueue;
using VideoApp.Web.Models;
using VideoApp.Web.Utilities;

namespace VideoApp.Web.TaskRunner
{
    public class JobRunnerQueue : IJobRunnerQueue
    {
        public event EventHandler<CustomEventArgs> JobFinished;

        private Queue<object> _jobs = new Queue<object>();
        private bool _delegateQueuedOrRunning = false;
        private readonly IFFmpegWraperService _ffmpegWraper;

        public JobRunnerQueue(IFFmpegWraperService ffmpegWraper)
        {
            _ffmpegWraper = ffmpegWraper;
        }

        public void Enqueue(object job)
        {
            lock (_jobs)
            {
                _jobs.Enqueue(job);
                if (!_delegateQueuedOrRunning)
                {
                    _delegateQueuedOrRunning = true;
                    ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ProcessQueuedItems), null);
                }
            }
        }
        private async void ProcessQueuedItems(object ignored)
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
                    var ffmpegArguments = (FFmpegArguments)item;
                    if (ffmpegArguments.Operation.Equals(OperationType.Conversion))
                    {
                        var fileLocation = await _ffmpegWraper.ConvertToOtherFormat(ffmpegArguments.InputFile, ffmpegArguments.OutputFile, ffmpegArguments.OutputFormat);
                        var myEvents = new CustomEventArgs(ffmpegArguments.ParentVideoId, fileLocation, ffmpegArguments.Operation);
                        OnJobFinished(myEvents);
                    }
                    else
                    {
                        var output = await _ffmpegWraper.GenerateHLS(ffmpegArguments.InputFile, ffmpegArguments.OutputFormat);
                        var myEvents = new CustomEventArgs(ffmpegArguments.ParentVideoId, output, ffmpegArguments.Operation);
                        OnJobFinished(myEvents);
                    }
                }
                catch(Exception ex)
                {
                    ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ProcessQueuedItems), null);
                    throw;
                }
            }
        }

        protected virtual void OnJobFinished(CustomEventArgs e)
        {
            JobFinished?.Invoke(this, e);
        }
    }

    public class CustomEventArgs : EventArgs
    {
        public int ParentVideoFileId { get; private set; }
        public string OutputFile { get; private set; }

        public OperationType Operation { get; set; }

        public CustomEventArgs(int parentId, string fullPath, OperationType operation) : base()
        {
            this.ParentVideoFileId = parentId;
            this.OutputFile = fullPath;
            this.Operation = operation;
        }
    }
}
