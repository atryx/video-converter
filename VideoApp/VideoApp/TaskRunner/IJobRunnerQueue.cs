using System;

namespace VideoApp.Web.TaskRunner
{
    public interface IJobRunnerQueue
    {
       void Enqueue(object job);
    }
}