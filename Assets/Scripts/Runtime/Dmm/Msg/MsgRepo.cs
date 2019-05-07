namespace Dmm.Msg
{
    /// <summary>
    /// 通信消息仓库，通过这个单例持有的消息队列来获取服务器前后来的消息。
    /// </summary>
    public class MsgRepo : IMsgRepo
    {
        #region MessageQueue

        /// <summary>
        /// 消息队列。
        /// </summary>
        private readonly MessageQueue<ProtoMessage> _queue = new MessageQueue<ProtoMessage>();

        public MessageQueue<ProtoMessage> GetMessageQueue()
        {
            return _queue;
        }

        #endregion

        #region 发送、接受消息

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg(ProtoMessage msg)
        {
            if (msg == null) return;
            _queue.EnqueueWriteMessage(msg);
        }

        /// <summary>
        /// 收到消息。
        /// </summary>
        /// <param name="msg"></param>
        public void ReceiveMsg(ProtoMessage msg)
        {
            if (msg == null) return;
            _queue.EnqueueReadMessage(msg);
        }

        #endregion

        #region WriteMessage

        public int WriteMessageCount()
        {
            return _queue.WriteMessageCount();
        }

        public ProtoMessage DequeueWriteMessage()
        {
            if (_queue.WriteMessageCount() <= 0)
                return null;

            return _queue.DequeueWriteMessage();
        }

        public void ClearWriteMessage()
        {
            _queue.ClearWriteMessage();
        }

        #endregion

        #region ReadMessage

        public int ReadMessageCount()
        {
            return _queue.ReadMessageCount();
        }

        public ProtoMessage DequeueReadMessage()
        {
            if (_queue.ReadMessageCount() <= 0)
                return null;

            return _queue.DequeueReadMessage();
        }

        public void ClearReadMessage()
        {
            _queue.ClearReadMessage();
        }

        #endregion
    }
}