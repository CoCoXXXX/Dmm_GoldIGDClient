namespace Dmm.Task
{
    public interface IActionSequence
    {
        ActionSequenceStatus GetStatus();
        void Process();

        IActionSequence Append(IAction ac);
        IActionSequence AppendInterval(float time);

        void Start();
        void Cancel();
        void Reset();

        float GetCurrentTime();
        float GetStartTime();
    }
}