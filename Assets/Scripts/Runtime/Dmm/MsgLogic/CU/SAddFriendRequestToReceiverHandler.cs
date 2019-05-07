using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Left;
using Dmm.Msg;
using UnityEngine;

namespace Dmm.MsgLogic.CU
{
    public class SAddFriendRequestToReceiverHandler : MessageHandlerAdapter<SAddFriendRequestToReceiver>
    {
        private readonly IDataContainer<List<string>> _friendIgnoreList;

        private readonly IDialogManager _dialogManager;

        public SAddFriendRequestToReceiverHandler(
            IDataRepository dataRepository, IDialogManager dialogManager)
            : base(Server.GServer, Msg.CmdType.CU.ADD_FRIEND_REQUEST_TO_RECEIVER_V6)
        {
            _dialogManager = dialogManager;
            _friendIgnoreList = dataRepository.GetContainer<List<string>>(DataKey.FriendRequesterList);
        }

        protected override void DoHandle(SAddFriendRequestToReceiver msg)
        {
            var friendIgnoreList = _friendIgnoreList.Read();

            if (friendIgnoreList == null)
            {
                friendIgnoreList = new List<string>();
                _friendIgnoreList.Write(friendIgnoreList, Time.time);
            }

            if (friendIgnoreList.Contains(msg.sender_username))
            {
                return;
            }

            _dialogManager.ShowDialog<FriendRequestDialog>(DialogName.FriendRequestDialog, false, false,
                (dialog) =>
                {
                    dialog.ApplyData(msg.sender_username, msg.sender_nickname, msg.sender_sex);
                    dialog.Show();
                });
        }
    }
}