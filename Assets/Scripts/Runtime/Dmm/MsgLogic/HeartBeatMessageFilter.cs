using Dmm.Msg;
using Dmm.Network;

namespace Dmm.MsgLogic
{
    public class HeartBeatMessageFilter : IMessageFilter
    {
        private readonly RemoteAPI _remoteAPI;

        public HeartBeatMessageFilter(RemoteAPI remoteAPI)
        {
            _remoteAPI = remoteAPI;
        }

        public bool Filter(ProtoMessage msg)
        {
            if (msg.Type == CmdType.HU.HB_REQ)
            {
                _remoteAPI.HBRes(0, Server.HServer);
                return true;
            }

            if (msg.Type == CmdType.HU.HB_RES)
                return true;

            return false;
        }
    }
}