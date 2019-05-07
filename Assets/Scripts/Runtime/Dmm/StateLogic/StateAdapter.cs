namespace Dmm.StateLogic
{
    public abstract class StateAdapter<TData> : IState<TData>
    {
        private bool _started = false;
        private bool _abort = false;

        public void Start()
        {
            _started = true;
            _abort = false;
        }

        public bool IsStarted()
        {
            return _started;
        }

        public void Reset()
        {
            _started = false;
        }

        public virtual void OnPause(TData data, bool pause, float time)
        {
        }

        public void Abort()
        {
            _abort = true;
        }

        public bool IsAbort()
        {
            return _abort;
        }

        public abstract int GetStateCode();

        public abstract string GetStateName();

        public abstract void Initialize(TData data, float time);

        public abstract bool Process(TData data, float time);

        public abstract StateResult Finish(TData data, float time);
    }
}