using System.Collections.Generic;
using Dmm.MsgLogic;
using UnityEngine;
using Zenject;

namespace Dmm.Msg
{
    /// <summary>
    /// 消息路由。
    /// </summary>
    public class MessageRouter : MonoBehaviour, IMessageRouter
    {
        #region Inject

        private IMsgRepo _msgRepo;

        private IMessageLogicFactory _messageLogicFactory;

        [Inject]
        public void Initialize(
            IMsgRepo msgRepo,
            IMessageLogicFactory messageLogicFactory)
        {
            _msgRepo = msgRepo;
            _messageLogicFactory = messageLogicFactory;
        }

        #endregion

        private void Start()
        {
            // 注册命令过滤器。
            InitMessageFilters();
            // 注册命令处理逻辑。
            InitMessageHandlers();
        }

        private readonly List<IMessageFilter> _filters = new List<IMessageFilter>();

        private void InitMessageFilters()
        {
            var filters = _messageLogicFactory.GetMessageFilterList();
            if (filters == null || filters.Count <= 0)
            {
                return;
            }

            foreach (var f in filters)
            {
                if (f == null)
                {
                    continue;
                }

                if (!_filters.Contains(f))
                {
                    _filters.Add(f);
                }
            }
        }

        private readonly Dictionary<Server, Dictionary<int, IMessageHandler>> _msgHandlers =
            new Dictionary<Server, Dictionary<int, IMessageHandler>>();

        /// <summary>
        /// 初始化命令逻辑模块。
        /// </summary>
        private void InitMessageHandlers()
        {
            var handlers = _messageLogicFactory.GetMessageHandlerList();
            if (handlers == null || handlers.Count <= 0)
            {
                return;
            }

            foreach (var handler in handlers)
            {
                if (handler == null)
                {
                    continue;
                }

                var server = handler.Server;
                var type = handler.CmdType;

                Dictionary<int, IMessageHandler> serverHandlers;
                _msgHandlers.TryGetValue(server, out serverHandlers);

                if (serverHandlers == null)
                {
                    serverHandlers = new Dictionary<int, IMessageHandler>();
                    _msgHandlers.Add(server, serverHandlers);
                }

                if (serverHandlers.ContainsKey(type))
                {
                    serverHandlers[type] = handler;
                }
                else
                {
                    serverHandlers.Add(type, handler);
                }
            }
        }

        /// <summary>
        /// 每一帧处理的命令数量。
        /// </summary>
        public int FetchCountPerFrame = 1;

        /// <summary>
        /// 获取消息的过程在LateUpdate中执行。  
        /// </summary>
        public void LateUpdate()
        {
            // 不管有没有断线，都应该始终检查是否收到消息，并处理完成。
            CheckReceivedMsg();
        }

        #region 发送消息

        private void CheckReceivedMsg()
        {
            if (_msgRepo.ReadMessageCount() <= 0)
            {
                return;
            }

            for (int i = 0; i < FetchCountPerFrame; i++)
            {
                var msg = _msgRepo.DequeueReadMessage();
                if (msg == null)
                {
                    continue;
                }

                LastResponseTime = Time.time;

                var filtered = false;
                for (int f = 0; f < _filters.Count; f++)
                {
                    var filter = _filters[f];
                    if (filter.Filter(msg))
                    {
                        filtered = true;
                        break;
                    }
                }

                if (!filtered)
                {
                    Dispatch(msg);
                }
            }
        }

        private void Dispatch(ProtoMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            var type = msg.Type;
            var server = msg.Server;

            Dictionary<int, IMessageHandler> serverHandlers;
            _msgHandlers.TryGetValue(server, out serverHandlers);

            if (serverHandlers == null)
            {
                return;
            }

            IMessageHandler handler;
            serverHandlers.TryGetValue(type, out handler);
            if (handler == null)
            {
                return;
            }

            handler.Handle(msg);
        }

        /// <summary>
        /// 上一次消息的时间。
        /// </summary>
        public float LastResponseTime { get; private set; }

        public float GetLastResponseTime()
        {
            return LastResponseTime;
        }

        public void ResetLastResponseTime()
        {
            LastResponseTime = 0;
        }

        #endregion

        public IMsgRepo GetMsgRepo()
        {
            return _msgRepo;
        }
    }
}