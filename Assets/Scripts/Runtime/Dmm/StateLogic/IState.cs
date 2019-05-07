namespace Dmm.StateLogic
{
    public interface IState<TData>
    {
        int GetStateCode();
        string GetStateName();

        void Reset();
        void Start();
        bool IsStarted();

        void Initialize(TData data, float time);
        bool Process(TData data, float time);
        StateResult Finish(TData data, float time);

        void OnPause(TData data, bool pause, float time);

        void Abort();
        bool IsAbort();
    }
}