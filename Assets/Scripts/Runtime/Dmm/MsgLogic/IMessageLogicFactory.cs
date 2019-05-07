using System.Collections.Generic;

namespace Dmm.MsgLogic
{
    public interface IMessageLogicFactory
    {
        List<IMessageHandler> GetMessageHandlerList();
        List<IMessageFilter> GetMessageFilterList();
    }
}