using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using Dmm.UserTask;
using UnityEngine;

namespace Dmm.MsgLogic.HU
{
    public class UserTaskTipHandler : MessageHandlerAdapter<UserTaskTip>
    {
        private readonly IDataContainer<UserTaskTip> _userTaskTip;

        private readonly IDialogManager _dialogManager;

        private readonly RemoteAPI _remoteApi;

        public UserTaskTipHandler(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteApi) :
            base(Server.HServer, Msg.CmdType.HU.USER_TASK_TIP)
        {
            _userTaskTip = dataRepository.GetContainer<UserTaskTip>(DataKey.UserTaskTip);
            _dialogManager = dialogManager;
            _remoteApi = remoteApi;
        }

        protected override void DoHandle(UserTaskTip msg)
        {
            _userTaskTip.Write(msg, Time.time);
            _remoteApi.RequestUserTaskList();
            _dialogManager.ShowDialog<UserTaskTipDialog>(DialogName.UserTaskTipDialog, false, true,
                (dialog) =>
                {
                    dialog.ApplyData(msg);
                    dialog.Show();
                });
        }
    }
}