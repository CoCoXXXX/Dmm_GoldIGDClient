using Dmm.Msg;

namespace Dmm.MsgLogic
{
    public abstract class MessageHandlerAdapter<T> : IMessageHandler where T : class
    {
        public Server Server { get; private set; }

        public int CmdType { get; private set; }

        protected MessageHandlerAdapter(Server server, int cmdType)
        {
            Server = server;
            CmdType = cmdType;
        }

        public void Handle(ProtoMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            var content = msg.Content as T;
            if (content == null)
            {
                return;
            }

            DoHandle(content);
        }

        protected abstract void DoHandle(T msg);
    }
}