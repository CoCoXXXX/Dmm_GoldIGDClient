using Dmm.Msg;

namespace Dmm.MsgLogic
{
    public interface IMessageHandler
    {
        Server Server { get; }
        int CmdType { get; }
        void Handle(ProtoMessage msg);
    }
}