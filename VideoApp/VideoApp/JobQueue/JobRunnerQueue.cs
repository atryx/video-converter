using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        //private BlockingCollection<object> _jobs = new BlockingCollection<object>();
        private readonly IFFmpegWraperService _ffmpegWraper;

        public JobRunnerQueue(IFFmpegWraperService ffmpegWraper)
        {
            _ffmpegWraper = ffmpegWraper;
        }

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
                    await _ffmpegWraper.ConvertToOtherFormat(ffmpegArguments.InputFile, ffmpegArguments.OutputFile, ffmpegArguments.OutputFormat);
                    //var result = _commandExecuter.ExecuteCommand(processStartParameters);
                    var myEvents = new CustomEventArgs(ffmpegArguments.ParentVideoId, ffmpegArguments.OutputFile, true);
                    OnJobFinished(myEvents);
                }
                catch(Exception ex)
                {
                    ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(ProcessQueuedItems), null);
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
        public string OutputFile { get; private set; }

        public bool ConversionResult { get; set; }

        public CustomEventArgs(int parentId, string fullPath, bool conversionResult) : base()
        {
            this.ParentVideoFileId = parentId;
            this.OutputFile = fullPath;
            this.ConversionResult = conversionResult;
        }
    }
}
