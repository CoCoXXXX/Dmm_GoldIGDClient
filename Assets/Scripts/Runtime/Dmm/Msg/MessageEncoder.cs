using System.IO;
using com.morln.game.gd.command;
using Dmm.Session;

namespace Dmm.Msg
{
    public class MessageEncoder
    {
        private static readonly GameMsgSerializer Serializer = new GameMsgSerializer();

        public static Packet Encode(ProtoMessage msg)
        {
            if (msg == null) return null;

            var packet = new Packet();
            packet.type = msg.Type;

            if (msg.Content != null)
            {
                using (var mem = new MemoryStream())
                {
                    Serializer.Serialize(mem, msg.Content);
                    mem.Seek(0, SeekOrigin.Begin);
                    packet.content = mem.ToArray();
                    mem.Close();
                }
            }

            return packet;
        }
    }
}