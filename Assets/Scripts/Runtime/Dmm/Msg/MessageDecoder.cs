using System.IO;
using com.morln.game.gd.command;
using Dmm.Session;

namespace Dmm.Msg
{
    public class MessageDecoder
    {
        private static readonly GameMsgSerializer Serializer = new GameMsgSerializer();

        /// <summary>
        /// 解析Packet消息。
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public static ProtoMessage Decode(Packet packet, Server server)
        {
            if (packet == null) return null;

            ProtoMessage msg = null;

            switch (server)
            {
                case Server.PServer:
                    msg = CmdType.CreatePServerEmptyMsg(packet.type);
                    break;

                case Server.HServer:
                case Server.GServer:
                    msg = CmdType.CreateHServerAndGServerEmptyMsg(packet.type);
                    break;
            }

            if (msg == null)
                return null;

            if (packet.content != null)
            {
                using (var mem = new MemoryStream())
                {
                    mem.Write(packet.content, 0, packet.content.Length);
                    mem.Seek(0, SeekOrigin.Begin);
                    msg.Content = Serializer.Deserialize(mem, null, msg.Model);
                    mem.Close();
                }
            }

            return msg;
        }
    }
}