using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic
{
    public class SimpleMessageHandler<T> : MessageHandlerAdapter<T> where T : class
    {
        private readonly IDataContainer<T> _container;

        public SimpleMessageHandler(Server server, int cmdType, IDataContainer<T> container) :
            base(server, cmdType)
        {
            _container = container;
        }

        protected override void DoHandle(T msg)
        {
            _container.Write(msg, Time.time);
        }
    }
}