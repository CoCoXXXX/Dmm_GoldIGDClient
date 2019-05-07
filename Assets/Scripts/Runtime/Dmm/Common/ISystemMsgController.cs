namespace Dmm.Common
{
    public interface ISystemMsgController
    {
        void Show(string content, bool error = false);
    }
}