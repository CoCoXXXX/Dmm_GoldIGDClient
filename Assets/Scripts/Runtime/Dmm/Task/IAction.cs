namespace Dmm.Task
{
    public interface IAction
    {
        void SetStartTime(float time);
        float GetStartTime();
        bool IsStarted();
        void Start();

        float GetTimeout();
        void ExecuteAction();
        bool CheckResult();
        void Timeout();
    }
}