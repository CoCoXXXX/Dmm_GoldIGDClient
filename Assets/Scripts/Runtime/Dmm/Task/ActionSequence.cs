using System;

namespace Dmm.Task
{
    public class ActionSequence : ActionSequenceAdapter
    {
        public Action StartListener;

        public Action CompleteListener;

        public Action CancelListener;

        /// <summary>
        /// Seq已经完成执行了。
        /// </summary>
        public bool Finished
        {
            get { return GetStatus() == ActionSequenceStatus.Finished; }
        }

        /// <summary>
        /// Seq当前是否在运行中。
        /// </summary>
        public bool Running
        {
            get { return GetStatus() == ActionSequenceStatus.Running; }
        }

        /// <summary>
        /// 被取消了。
        /// </summary>
        public bool Canceled
        {
            get { return GetStatus() == ActionSequenceStatus.Canceled; }
        }

        public ActionSequence Append(
            Action actionLogic,
            Func<bool> checker = null,
            Action actionTimeoutHandler = null,
            float timeout = 60)
        {
            var ac = new ActionNode(actionLogic, checker, timeout, actionTimeoutHandler);
            Append(ac);
            return this;
        }

        protected override void OnSequenceStart()
        {
            if (StartListener != null)
            {
                StartListener();
            }
        }

        protected override void OnSequenceComplete()
        {
            if (CompleteListener != null)
            {
                CompleteListener();
            }
        }

        protected override void OnSequenceCancel()
        {
            if (CancelListener != null)
            {
                CancelListener();
            }
        }

        private class ActionNode : IAction
        {
            private bool _started;

            private float _startTime;

            private readonly float _timeout;

            private readonly Action _actionLogic;

            private readonly Func<bool> _resultChecker;

            private readonly Action _actionTimeoutHandler;

            public ActionNode(
                Action actionLogic,
                Func<bool> resultChecker,
                float timeout,
                Action actionTimeoutHandler)
            {
                _timeout = timeout;
                _actionLogic = actionLogic;
                _resultChecker = resultChecker;
                _actionTimeoutHandler = actionTimeoutHandler;
            }

            public void SetStartTime(float time)
            {
                _startTime = time;
            }

            public float GetStartTime()
            {
                return _startTime;
            }

            public bool IsStarted()
            {
                return _started;
            }

            public void Start()
            {
                _started = true;
            }

            public float GetTimeout()
            {
                return _timeout;
            }

            public void ExecuteAction()
            {
                if (_actionLogic != null)
                {
                    _actionLogic();
                }
            }

            public bool CheckResult()
            {
                if (_resultChecker == null)
                {
                    return true;
                }

                return _resultChecker();
            }

            public void Timeout()
            {
                if (_actionTimeoutHandler != null)
                {
                    _actionTimeoutHandler();
                }
            }
        }
    }
}