using Dmm.Msg;

namespace Dmm.MsgLogic
{
    public interface IMessageFilter
    {
        bool Filter(ProtoMessage msg);
    }
}