using System.Net.Sockets;
using Dmm.Msg;

namespace Dmm.Session
{
    public interface ISocketClient
    {
        void Close();
        bool Connect(string host, int port, Server server, AddressFamily networkAddressFamily);
        SocketStatus GetStatus();
        Server GetServer();
    }

    /// <summary>
    /// Socket状态。
    /// </summary>
    public enum SocketStatus
    {
        /// <summary>
        /// 断线。
        /// </summary>
        Disconnected,

        /// <summary>
        /// 正在连接当中。
        /// </summary>
        Connecting,

        /// <summary>
        /// 已经连接服务器。
        /// </summary>
        Connected
    }
}