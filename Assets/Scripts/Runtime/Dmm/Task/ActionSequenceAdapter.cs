using System.Collections.Generic;
using UnityEngine;

namespace Dmm.Task
{
    public abstract class ActionSequenceAdapter : IActionSequence
    {
        private readonly List<IAction> _actionList = new List<IAction>();

        private int _curActionIndex;

        private ActionSequenceStatus _status;

        private float _startTime;

        public ActionSequenceStatus GetStatus()
        {
            return _status;
        }

        public float GetCurrentTime()
        {
            return Time.time;
        }

        public float GetStartTime()
        {
            return _startTime;
        }

        public void Start()
        {
            lock (this)
            {
                if (_status == ActionSequenceStatus.Running)
                {
                    return;
                }

                // 在这里调用，确保状态确实是Before Start的。
                OnSequenceStart();

                _status = ActionSequenceStatus.Running;
                _curActionIndex = 0;
                _startTime = GetCurrentTime();
            }
        }

        public void Cancel()
        {
            lock (this)
            {
                if (_status == ActionSequenceStatus.Idle ||
                    _status == ActionSequenceStatus.Running)
                {
                    _status = ActionSequenceStatus.Canceled;

                    _curActionIndex = -1;

                    OnSequenceCancel();
                }
            }
        }

        public void Reset()
        {
            lock (this)
            {
                _status = ActionSequenceStatus.Idle;
                _curActionIndex = -1;
                _startTime = 0;
            }
        }

        private void Finish()
        {
            lock (this)
            {
                _status = ActionSequenceStatus.Finished;

                _curActionIndex = -1;

                OnSequenceComplete();
            }
        }

        public void Process()
        {
            lock (this)
            {
                // 尚未启动的或者已经结束的任务，不执行。
                if (_status != ActionSequenceStatus.Running)
                {
                    return;
                }

                // 没有任务的，不执行。
                if (_actionList.Count <= 0)
                {
                    return;
                }

                if (_curActionIndex < 0 || _curActionIndex >= _actionList.Count)
                {
                    // 内部参数不合法，结束执行。
                    Cancel();
                    return;
                }

                var ac = _actionList[_curActionIndex];
                if (!ac.IsStarted())
                {
                    ac.Start();
                    ac.SetStartTime(GetCurrentTime());
                    ac.ExecuteAction();
                    return;
                }

                var complete = ac.CheckResult();

                // 检查是否完成，之前调用的ResultChecker有可能在中间调用Stop，导致流程中止。
                // 因为是单线程环境，所以只要检查当前线程中可能会导致流程中止的点就可以了。
                if (_status != ActionSequenceStatus.Running)
                    return;

                var timeout = GetCurrentTime() - ac.GetStartTime() >= ac.GetTimeout();

                if (complete || timeout)
                {
                    if (timeout)
                        // 流程超时的情况下，并没有直接结束流程，而是由TimeoutHandler来解决。
                        ac.Timeout();

                    // 调用TimeoutHandler有可能调用Stop导致流程中止。
                    if (_status != ActionSequenceStatus.Running)
                        return;

                    _curActionIndex++;

                    if (_curActionIndex >= _actionList.Count)
                    {
                        // 整个执行序列完成。
                        Finish();
                    }
                }
            }
        }

        public IActionSequence Append(IAction ac)
        {
            lock (this)
            {
                if (!_actionList.Contains(ac))
                    _actionList.Add(ac);

                return this;
            }
        }

        public IActionSequence AppendInterval(float time)
        {
            lock (this)
            {
                _actionList.Add(new IntervalAction(time));
                return this;
            }
        }

        protected abstract void OnSequenceStart();

        protected abstract void OnSequenceComplete();

        protected abstract void OnSequenceCancel();

        private class IntervalAction : IAction
        {
            private bool _started;
            private float _startTime;
            private readonly float _interval;

            public IntervalAction(float interval)
            {
                _interval = interval;
            }

            public void Start()
            {
                _started = true;
            }

            public bool IsStarted()
            {
                return _started;
            }

            public void SetStartTime(float time)
            {
                _startTime = time;
            }

            public float GetStartTime()
            {
                return _startTime;
            }

            public float GetTimeout()
            {
                return _interval;
            }

            public void ExecuteAction()
            {
            }

            public bool CheckResult()
            {
                return false;
            }

            public void Timeout()
            {
            }
        }
    }
}