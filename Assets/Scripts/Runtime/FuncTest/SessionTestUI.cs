using com.morln.game.gd.command;
using Dmm.Msg;
using Dmm.Session;
using UnityEngine;
using UnityEngine.UI;

namespace FuncTest
{
    public class SessionTestUI : MonoBehaviour
    {
        public InputField Host;

        public InputField Port;

        public Text ConnectionState;

        public InputField MsgType;

        public InputField MsgContent;

        public Text ReceiveMsg;

        private SocketClient _socketClient;

        public void Awake()
        {
//            _socketClient = new SocketClient(MessageQueue.Current);
        }

        public void Update()
        {
            switch (_socketClient.Status)
            {
                case SocketStatus.Connected:
                    ConnectionState.text = "已连接";
                    break;

                case SocketStatus.Disconnected:
                    ConnectionState.text = "断开";
                    break;

                case SocketStatus.Connecting:
                    ConnectionState.text = "连接中";
                    break;
            }

            /*var msg = MessageQueue.Current.DequeueReadMessage();
            if (msg != null)
            {
                ReceiveMsg.text = "收到消息：" + msg.type + "\n" + System.Text.Encoding.UTF8.GetString(msg.content);
            }*/
        }

        public void Connect()
        {
            var host = Host.text;
            var port = int.Parse(Port.text);

            _socketClient.Connect(host, port, Server.PServer);
        }

        public void SendMsg()
        {
            var type = int.Parse(MsgType.text);
            var content = MsgContent.text;

            var packet = new Packet();
            packet.type = type;
            packet.content = System.Text.Encoding.UTF8.GetBytes(content);

//            MessageQueue.Current.EnqueueWriteMessage(packet);
        }

        public void OnApplicationQuit()
        {
            _socketClient.Close();
        }
    }
}