using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dmm.Task
{
    public class TaskManager : MonoBehaviour, ITaskManager
    {
        #region Unity方法

        public void Update()
        {
            UpdateTask();
            UpdateActionSequence();
        }

        #endregion

        #region Task接口

        private readonly List<TaskWrapper> _taskList = new List<TaskWrapper>();

        private class BoolTaskChecker : ITaskChecker
        {
            private readonly Func<bool> _checker;

            public BoolTaskChecker(Func<bool> checker)
            {
                _checker = checker;
            }

            public TaskResult Check()
            {
                if (_checker == null)
                {
                    return TaskResult.Success();
                }

                var res = _checker();
                if (res)
                {
                    return TaskResult.Success();
                }
                else
                {
                    return null;
                }
            }
        }

        private class TaskWrapper
        {
            public float StartTime;

            public float Timeout;

            public ITaskChecker CheckTaskState;

            public Action CompleteHandler;

            public Action SuccessHandler;

            public Action<int, string> FailHandler;

            public Action TimeoutHandler;
        }

        public void ExecuteTask(
            Func<bool> checker,
            Action timeoutHandler,
            float timeout = 10)
        {
            var task = new TaskWrapper();
            task.StartTime = Time.time;
            task.CheckTaskState = new BoolTaskChecker(checker);
            task.Timeout = timeout;
            task.TimeoutHandler = timeoutHandler;

            lock (_taskList)
            {
                _taskList.Add(task);
            }
        }

        public void ExecuteTask(
            Func<bool> checker,
            Action completeHandler,
            Action timeoutHandler,
            float timeout = 10)
        {
            var task = new TaskWrapper();
            task.StartTime = Time.time;
            task.CheckTaskState = new BoolTaskChecker(checker);
            task.CompleteHandler = completeHandler;
            task.Timeout = timeout;
            task.TimeoutHandler = timeoutHandler;

            lock (_taskList)
            {
                _taskList.Add(task);
            }
        }

        private class TaskResultChecker : ITaskChecker
        {
            private readonly Func<TaskResult> _checker;

            public TaskResultChecker(Func<TaskResult> checker)
            {
                _checker = checker;
            }

            public TaskResult Check()
            {
                if (_checker == null)
                {
                    return TaskResult.Success();
                }

                return _checker();
            }
        }

        public void ExecuteTask(
            Func<TaskResult> checker,
            Action completeHandler,
            Action successHandler,
            Action<int, string> failHandler,
            Action timeoutHandler,
            float timeout = 10)
        {
            var task = new TaskWrapper();
            task.StartTime = Time.time;
            task.CheckTaskState = new TaskResultChecker(checker);
            task.CompleteHandler = completeHandler;
            task.SuccessHandler = successHandler;
            task.FailHandler = failHandler;
            task.TimeoutHandler = timeoutHandler;
            task.Timeout = timeout;

            lock (_taskList)
            {
                _taskList.Add(task);
            }
        }

        private void UpdateTask()
        {
            lock (_taskList)
            {
                for (int i = 0; i < _taskList.Count; i++)
                {
                    var task = _taskList[i];
                    if (task == null)
                    {
                        _taskList.RemoveAt(i);
                        i--;
                        continue;
                    }

                    var checker = task.CheckTaskState;
                    var complete = task.CompleteHandler;
                    var success = task.SuccessHandler;
                    var fail = task.FailHandler;
                    var timeout = task.TimeoutHandler;

                    if (checker == null)
                    {
                        if (complete != null)
                        {
                            complete();
                        }

                        if (success != null)
                        {
                            success();
                        }

                        _taskList.RemoveAt(i);
                        i--;
                        continue;
                    }

                    var res = checker.Check();
                    if (res != null)
                    {
                        if (complete != null)
                        {
                            complete();
                        }

                        if (res.IsSuccess)
                        {
                            if (success != null)
                            {
                                success();
                            }
                        }
                        else
                        {
                            if (fail != null)
                            {
                                fail(res.ErrCode, res.ErrMsg);
                            }
                        }

                        _taskList.RemoveAt(i);
                        i--;
                        continue;
                    }

                    if (task.StartTime + task.Timeout <= Time.time)
                    {
                        if (timeout != null)
                        {
                            timeout();
                        }

                        _taskList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        #endregion

        #region ActionSequence接口

        private readonly List<IActionSequence> _seqList = new List<IActionSequence>();

        public void ExecuteSeq(IActionSequence seq, bool reset = true, bool autoStart = true)
        {
            if (seq == null) return;

            lock (_seqList)
            {
                if (!_seqList.Contains(seq))
                    _seqList.Add(seq);
            }

            if (reset) seq.Reset();
            if (autoStart) seq.Start();
        }

        public void CancelSeq(IActionSequence seq, bool autoCancel = true)
        {
            if (seq == null) return;

            lock (_seqList)
            {
                _seqList.Remove(seq);
                if (autoCancel) seq.Cancel();
            }
        }

        private void UpdateActionSequence()
        {
            lock (_seqList)
            {
                for (int i = 0; i < _seqList.Count; i++)
                {
                    var seq = _seqList[i];
                    var status = seq.GetStatus();
                    switch (status)
                    {
                        case ActionSequenceStatus.Running:
                            seq.Process();
                            break;

                        case ActionSequenceStatus.Idle:
                            // 等待任务开始。
                            continue;

                        case ActionSequenceStatus.Finished:
                        case ActionSequenceStatus.Canceled:
                            _seqList.RemoveAt(i);
                            i--;
                            continue;
                    }
                }
            }
        }

        #endregion
    }
}