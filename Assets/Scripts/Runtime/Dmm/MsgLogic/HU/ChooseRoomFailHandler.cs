using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class ChooseRoomFailHandler : MessageHandlerAdapter<ChooseRoomFail>
    {
        private readonly IDataContainer<ChooseRoomFail> _chooseRoomFail;

        private readonly IDataContainer<GLoginResult> _gLoginResult;

        private readonly IDialogManager _dialogManager;

        public ChooseRoomFailHandler(IDataRepository dataRepository, IDialogManager dialogManager) :
            base(Server.HServer, Msg.CmdType.HU.CHOOSE_ROOM_FAIL)
        {
            _chooseRoomFail = dataRepository.GetContainer<ChooseRoomFail>(DataKey.ChooseRoomFail);
            _gLoginResult = dataRepository.GetContainer<GLoginResult>(DataKey.GLoginResult);
            _dialogManager = dialogManager;
        }

        protected override void DoHandle(ChooseRoomFail msg)
        {
            _chooseRoomFail.Write(msg, Time.time);

            if (msg != null)
            {
                // 选房失败的情况。
                _gLoginResult.ClearAndInvalidate(Time.time);
            }

            _dialogManager.ShowDialog<ChooseRoomFailDialog>(DialogName.ChooseRoomFailDialog, false, false,
                (dialog) =>
                {
                    dialog.ApplyData(msg);
                    dialog.Show();
                });
        }
    }
}