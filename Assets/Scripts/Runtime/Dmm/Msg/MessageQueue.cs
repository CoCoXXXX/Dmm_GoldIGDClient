using System.Collections.Generic;

namespace Dmm.Msg
{
    /// <summary>
    /// 保存写入和读出的消息。
    /// </summary>
    public class MessageQueue<T>
    {
        private readonly Queue<T> _readQueue = new Queue<T>();

        #region 读取队列

        public int ReadMessageCount()
        {
            lock (_readQueue)
            {
                return _readQueue.Count;
            }
        }

        public T DequeueReadMessage()
        {
            lock (_readQueue)
            {
                if (_readQueue.Count > 0)
                {
                    return _readQueue.Dequeue();
                }
                else
                {
                    return default(T);
                }
            }
        }

        public void EnqueueReadMessage(T msg)
        {
            lock (_readQueue)
            {
                if (!_readQueue.Contains(msg))
                {
                    _readQueue.Enqueue(msg);
                }
            }
        }

        public void ClearReadMessage()
        {
            lock (_readQueue)
            {
                _readQueue.Clear();
            }
        }

        #endregion

        #region 写入队列

        private readonly Queue<T> _writeQueue = new Queue<T>();

        public int WriteMessageCount()
        {
            lock (_writeQueue)
            {
                return _writeQueue.Count;
            }
        }

        public T DequeueWriteMessage()
        {
            lock (_writeQueue)
            {
                if (_writeQueue.Count > 0)
                {
                    return _writeQueue.Dequeue();
                }
                else
                {
                    return default(T);
                }
            }
        }

        public void EnqueueWriteMessage(T msg)
        {
            lock (_writeQueue)
            {
                if (!_writeQueue.Contains(msg))
                {
                    _writeQueue.Enqueue(msg);
                }
            }
        }

        public void ClearWriteMessage()
        {
            lock (_writeQueue)
            {
                _writeQueue.Clear();
            }
        }

        #endregion
    }
}