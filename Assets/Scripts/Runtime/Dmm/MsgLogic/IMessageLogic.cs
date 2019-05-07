using System;
using Dmm.Msg;

namespace Dmm.MsgLogic
{
    public interface IMessageLogic
    {
        int[] SupportedTypes();
        Action<ProtoMessage> GetHandler(int type);
        Server Server();
    }
}