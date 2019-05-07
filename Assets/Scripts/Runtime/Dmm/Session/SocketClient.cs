using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using com.morln.game.gd.command;
using Dmm.Log;
using Dmm.Msg;
using ProtoBuf;

namespace Dmm.Session
{
    /// <summary>
    /// Socket客户端。
    /// </summary>
    public class SocketClient : ISocketClient
    {
        public const string Tag = "SocketClient";

        #region 构造函数

        private readonly MessageQueue<ProtoMessage> _msgQueue;

        public SocketClient(MessageQueue<ProtoMessage> msgQueue)
        {
            Server = Server.Null;
            _msgQueue = msgQueue;
        }

        #endregion

        #region 状态

        /// <summary>
        /// 当前客户端的状态。
        /// </summary>
        public SocketStatus Status { get; private set; }

        public SocketStatus GetStatus()
        {
            return Status;
        }

        #endregion

        #region 连接、关闭socket

        /// <summary>
        /// Client连接的服务器。
        /// </summary>
        public Server Server { get; private set; }

        public Server GetServer()
        {
            return Server;
        }

        /// <summary>
        /// 当前使用的TcpClient。
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// 连接远程服务器。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="server"></param>
        /// <returns>true: 成功开始连接，false：开始连接失败</returns>
        public bool Connect(string host, int port, Server server,
            AddressFamily networkType = AddressFamily.InterNetwork)
        {
            try
            {
                MyLog.InfoAsync(Tag, string.Format("connect {0}:{1}({2})", host, port, networkType));

                // 先关闭之前的连接。
                Close();

                // 服务器。
                Server = server;

                var client = new TcpClient(networkType);
                client.NoDelay = true;
                client.LingerState = new LingerOption(false, 0);
                client.SendTimeout = 5000;

                // 开始新的连接。
                Status = SocketStatus.Connecting;
                client.BeginConnect(host, port, FinishConnect, client);
                return true;
            }
            catch (Exception e)
            {
                MyLog.ErrorAsync(Tag, string.Format("connect error: {0}\n{1}", e.Message, e.StackTrace));
                Status = SocketStatus.Disconnected;
                return false;
            }
        }

        private void FinishConnect(IAsyncResult result)
        {
            try
            {
                var beginClient = (TcpClient) result.AsyncState;
                if (beginClient == null)
                {
                    Close();
                    return;
                }
                beginClient.EndConnect(result);
                if (beginClient.Connected)
                {
                    // 启动读取和写入线程。
                    _readThread = new ReadThread(this, beginClient, _msgQueue);
                    _readThread.Start();

                    _writeThread = new WriteThread(this, beginClient, _msgQueue);
                    _writeThread.Start();

                    // 已经连接上。
                    Status = SocketStatus.Connected;
                    _client = beginClient;
                }
                else
                {
                    // 未连接成功，关闭当前连接。
                    Close();
                }
            }
            catch (Exception e)
            {
                MyLog.ErrorAsync(Tag, string.Format("end connect error: {0}\n{1}", e.Message, e.StackTrace));
                Close();
            }
        }

        /// <summary>
        /// 关闭Client。
        /// </summary>
        public void Close()
        {
            Status = SocketStatus.Disconnected;

            if (_readThread != null)
            {
                _readThread.Stop();
                _readThread = null;
            }

            if (_writeThread != null)
            {
                _writeThread.Stop();
                _writeThread = null;
            }

            if (_client != null)
            {
                try
                {
                    if (_client.Connected)
                        _client.GetStream().Close();
                }
                catch (Exception e)
                {
                }
                try
                {
                    _client.Close();
                }
                catch (Exception e)
                {
                }

                _client = null;
            }
        }

        #endregion

        #region 读取写入线程

        /// <summary>
        /// 检查数据的时间间隔。
        /// </summary>
        public const int DataCheckInterval = 10;

        /// <summary>
        /// 从socket中读取数据的线程。
        /// </summary>
        private ReadThread _readThread;

        /// <summary>
        /// 向socket中写入数据的线程。
        /// </summary>
        private WriteThread _writeThread;

        /// <summary>
        /// 读消息线程。
        /// </summary>
        public class ReadThread
        {
            public bool Running { get; private set; }

            private SocketClient _parent;

            private readonly TcpClient _tcpClient;

            private readonly Thread _worker;

            private MessageQueue<ProtoMessage> _msgQueue;

            public ReadThread(SocketClient parent, TcpClient client, MessageQueue<ProtoMessage> msgQueue)
            {
                _parent = parent;
                _msgQueue = msgQueue;
                _tcpClient = client;
                _worker = new Thread(ReadPacket);
            }

            public void Start()
            {
                if (!Running)
                {
                    Running = true;
                    _worker.Start();
                }
            }

            public void Stop()
            {
                Running = false;
                _parent = null;
                _msgQueue = null;
                _worker.Interrupt();
            }

            /// <summary>
            /// 读取Packet。
            /// </summary>
            private void ReadPacket()
            {
                var serializer = new PacketSerializer();
                var type = typeof(Packet);

                while (Running)
                {
                    try
                    {
                        if (_msgQueue == null) return;

                        if (_tcpClient.Available > 0)
                        {
                            var stream = _tcpClient.GetStream();
                            var packet =
                                serializer.DeserializeWithLengthPrefix(stream, null, type, PrefixStyle.Fixed32BigEndian,
                                    0) as Packet;
                            var msg = MessageDecoder.Decode(packet, _parent.Server);

                            if (msg != null && _msgQueue != null) _msgQueue.EnqueueReadMessage(msg);
                        }
                        else
                        {
                            Thread.Sleep(DataCheckInterval);
                        }
                    }
                    catch (ObjectDisposedException e)
                    {
                        // 客户端已经被销毁了，需要断开连接。
                        if (_parent != null) _parent.Close();
                    }
                    catch (IOException e)
                    {
                        // 通信发生错误，需要断开连接。
                        if (_parent != null) _parent.Close();
                    }
                    catch (ThreadInterruptedException e)
                    {
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 写入线程。
        /// </summary>
        public class WriteThread
        {
            public bool Running { get; private set; }

            private SocketClient _parent;

            private readonly TcpClient _tcpClient;

            private readonly Thread _worker;

            private MessageQueue<ProtoMessage> _msgQueue;

            public WriteThread(SocketClient parent, TcpClient client, MessageQueue<ProtoMessage> msgQueue)
            {
                _parent = parent;
                _msgQueue = msgQueue;
                _tcpClient = client;
                _worker = new Thread(WritePacket);
            }

            public void Start()
            {
                if (!Running)
                {
                    Running = true;
                    _worker.Start();
                }
            }

            public void Stop()
            {
                Running = false;
                _parent = null;
                _msgQueue = null;
                _worker.Interrupt();
            }

            private void WritePacket()
            {
                var serializer = new PacketSerializer();
                var type = typeof(Packet);

                while (Running)
                {
                    try
                    {
                        if (_msgQueue == null) return;

                        var msg = _msgQueue.DequeueWriteMessage();
                        if (msg != null)
                        {
                            var packet = MessageEncoder.Encode(msg);
                            if (packet != null)
                            {
                                var stream = _tcpClient.GetStream();
                                serializer.SerializeWithLengthPrefix(stream, packet, type, PrefixStyle.Fixed32BigEndian,
                                    0);
                                stream.Flush();
                            }
                        }
                        else
                        {
                            Thread.Sleep(DataCheckInterval);
                        }
                    }
                    catch (ObjectDisposedException e)
                    {
                        // 客户端已经被销毁了，需要断开连接。
                        if (_parent != null) _parent.Close();
                    }
                    catch (IOException e)
                    {
                        // 通信发生错误，需要断开连接。
                        if (_parent != null) _parent.Close();
                    }
                    catch (Exception e)
                    {
                        // TODO 出错了。
                    }
                }
            }
        }

        #endregion
    }
}