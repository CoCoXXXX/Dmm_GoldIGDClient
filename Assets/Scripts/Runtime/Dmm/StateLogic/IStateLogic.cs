namespace Dmm.StateLogic
{
    public interface IStateLogic<TData>
    {
        void Start();
        void Stop();

        void AddState(IState<TData> state);
        StateResult Process(float time);
        void OnPause(bool pause, float time);

        int GetCurrentStateCode();
        void SwitchTo(int state);
        void Reset();
    }
}