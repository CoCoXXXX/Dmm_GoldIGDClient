using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class HLoginResultHandler : MessageHandlerAdapter<HLoginResult>
    {
        private readonly IDataContainer<Queue<BRoomInOut>> _roomInOut;

        private readonly IDataContainer<HLoginResult> _hLoginResult;

        public HLoginResultHandler(IDataRepository dataRepository) :
            base(Server.HServer, Msg.CmdType.HU.LOGIN_RESULT)
        {
            _roomInOut = dataRepository.GetContainer<Queue<BRoomInOut>>(DataKey.BRoomInOut);
            _hLoginResult = dataRepository.GetContainer<HLoginResult>(DataKey.HLoginResult);
        }

        protected override void DoHandle(HLoginResult msg)
        {
            _hLoginResult.Write(msg, Time.time);
            var roomInOut = _roomInOut.Read();
            if (roomInOut != null)
            {
                roomInOut.Clear();
            }
        }
    }
}