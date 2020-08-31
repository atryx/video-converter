using System;
using VideoApp.Web.TaskRunner;

namespace VideoApp.Web.JobQueue
{
    public interface IJobRunnerQueue
    {
        event EventHandler<CustomEventArgs> JobFinished;
        void Enqueue(object job);
    }
}
