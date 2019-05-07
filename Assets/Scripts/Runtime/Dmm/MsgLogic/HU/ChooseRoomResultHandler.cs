using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.DataRelation;
using Dmm.Msg;
using Dmm.Network;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class ChooseRoomResultHandler : MessageHandlerAdapter<ChooseRoomResult>
    {
        private readonly IDataContainer<ChooseRoomResult> _chooseRoomResult;

        private readonly IDataContainer<ServerAddress> _gServerAddress;

        private readonly INetworkManager _network;

        public ChooseRoomResultHandler(IDataRepository dataRepository,
            INetworkManager network) :
            base(Server.HServer, Msg.CmdType.HU.CHOOSE_ROOM_RESULT)
        {
            _chooseRoomResult = dataRepository.GetContainer<ChooseRoomResult>(DataKey.ChooseRoomResult);
            _gServerAddress = dataRepository.GetContainer<ServerAddress>(DataKey.GameServerAddress);
            _network = network;
        }

        protected override void DoHandle(ChooseRoomResult msg)
        {
            _chooseRoomResult.Write(msg, Time.time);

//            if (string.IsNullOrEmpty(msg.game_server_addr))
//            {
//                return;
//            }
//
//            var gserverAddress = _gServerAddress.Read();
//            if (gserverAddress == null)
//            {
//                return;
//            }
//            var gserverIp = gserverAddress.Ip;
//            var gServerPort = gserverAddress.Port;
//
//            if (string.IsNullOrEmpty(gserverIp) || gServerPort == 0)
//            {
//                return;
//            }

            _network.StartConnectGServer();
        }
    }
}