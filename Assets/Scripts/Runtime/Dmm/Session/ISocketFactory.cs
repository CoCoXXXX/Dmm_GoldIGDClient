namespace Dmm.Session
{
    public interface ISocketFactory
    {
        ISocketClient CreateSocket();
    }
}