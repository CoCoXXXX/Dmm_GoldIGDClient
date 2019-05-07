using System;

namespace Dmm.Task
{
    public interface ITaskManager
    {
        void ExecuteTask(
            Func<bool> checker,
            Action timeoutHandler,
            float timeout = 10);

        void ExecuteTask(
            Func<bool> checker,
            Action completeHandler,
            Action timeoutHandler,
            float timeout = 10);

        void ExecuteTask(
            Func<TaskResult> checker,
            Action completeHandler,
            Action successHandler,
            Action<int, string> failHandler,
            Action timeoutHandler,
            float timeout = 10);

        void ExecuteSeq(IActionSequence seq, bool reset = true, bool autoStart = true);

        void CancelSeq(IActionSequence seq, bool autoCancel = true);
    }
}