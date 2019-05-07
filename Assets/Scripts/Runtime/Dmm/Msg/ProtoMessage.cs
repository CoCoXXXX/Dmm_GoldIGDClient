using System;
using Dmm.Session;

namespace Dmm.Msg
{
    /// <summary>
    /// 解析之后的消息。
    /// </summary>
    public class ProtoMessage
    {
        /// <summary>
        /// 消息类型码。
        /// </summary>
        public int Type;

        /// <summary>
        /// 消息解析过的内容。
        /// </summary>
        public object Content;

        /// <summary>
        /// 消息的实际类型。
        /// </summary>
        public Type Model;

        /// <summary>
        /// 消息归属于哪个Server。
        /// </summary>
        public Server Server;
    }
}