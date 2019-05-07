using System;

namespace Dmm.Task
{
    public class ActionSequenceGeneric<TData> : ActionSequenceAdapter
    {
        public Action<TData> StartListener;

        public Action<TData> CompleteListener;

        public Action<TData> CancelListener;

        private readonly TData _data;

        public ActionSequenceGeneric(TData data)
        {
            _data = data;
        }

        public ActionSequenceGeneric<TData> Append(
            Action<TData> actionLogic,
            Func<TData, bool> checker = null,
            Action<TData> actionTimeoutHandler = null,
            float timeout = 60)
        {
            var ac = new ActionGeneric(actionLogic, checker, timeout, actionTimeoutHandler, _data);
            Append(ac);

            return this;
        }

        protected override void OnSequenceStart()
        {
            if (StartListener != null)
            {
                StartListener(_data);
            }
        }

        protected override void OnSequenceComplete()
        {
            if (CompleteListener != null)
            {
                CompleteListener(_data);
            }
        }

        protected override void OnSequenceCancel()
        {
            if (CancelListener != null)
            {
                CancelListener(_data);
            }
        }

        private class ActionGeneric : IAction
        {
            private bool _started;

            private float _startTime;

            private readonly float _timeout;

            private readonly TData _data;

            private readonly Action<TData> _actionLogic;

            private readonly Func<TData, bool> _resultChecker;

            private readonly Action<TData> _actionTimeoutHandler;

            public ActionGeneric(
                Action<TData> actionLogic,
                Func<TData, bool> resultChecker,
                float timeout,
                Action<TData> actionTimeoutHandler,
                TData data)
            {
                _timeout = timeout;
                _actionLogic = actionLogic;
                _resultChecker = resultChecker;
                _actionTimeoutHandler = actionTimeoutHandler;
                _data = data;
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
                return _timeout;
            }

            public void ExecuteAction()
            {
                if (_actionLogic != null)
                {
                    _actionLogic(_data);
                }
            }

            public bool CheckResult()
            {
                if (_resultChecker == null)
                {
                    return true;
                }

                return _resultChecker(_data);
            }

            public void Timeout()
            {
                if (_actionTimeoutHandler != null)
                {
                    _actionTimeoutHandler(_data);
                }
            }
        }
    }
}