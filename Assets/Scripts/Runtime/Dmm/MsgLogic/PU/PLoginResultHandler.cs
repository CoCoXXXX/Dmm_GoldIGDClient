using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.PU
{
    public class PLoginResultHandler : MessageHandlerAdapter<PLoginResult>
    {
        private readonly IDataContainer<PLoginResult> _pLoginResult;

        public PLoginResultHandler(IDataRepository dataRepository) :
            base(Server.PServer, Msg.CmdType.PU.LOGIN_RESULT)
        {
            _pLoginResult = dataRepository.GetContainer<PLoginResult>(DataKey.PLoginResult);
        }

        protected override void DoHandle(PLoginResult loginResult)
        {
            _pLoginResult.Write(loginResult, Time.time);

            if (loginResult != null && loginResult.result == ResultCode.OK)
            {
                LoginRecord.Token = loginResult.token;
                LoginRecord.SaveAll();
            }
        }
    }
}