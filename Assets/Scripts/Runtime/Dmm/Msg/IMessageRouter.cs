namespace Dmm.Msg
{
    public interface IMessageRouter
    {
        float GetLastResponseTime();
        void ResetLastResponseTime();
    }
}