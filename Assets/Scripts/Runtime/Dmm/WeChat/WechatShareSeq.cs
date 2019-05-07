using System;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Task;

namespace Dmm.WeChat
{
    public class WechatShareSeq : ActionSequence
    {
        private readonly NotifyDoShare _currentNotifyDoShare;

        private readonly RemoteAPI _remoteAPI;

        private readonly IDataContainer<NotifyDoShareResult> _notifyDoShareResult;

        private readonly IDialogManager _dialogManager;

        private readonly Action _finishCallBack;

        public WechatShareSeq(NotifyDoShare currentNotifyDoShare,
            IDataContainer<NotifyDoShareResult> notifyDoShareResult, RemoteAPI remoteApi, IDialogManager dialogManager,
            Action callBack)
        {
            _finishCallBack = callBack;
            _dialogManager = dialogManager;
            _remoteAPI = remoteApi;
            StartListener = BeforeStart;
            _notifyDoShareResult = notifyDoShareResult;
            _currentNotifyDoShare = currentNotifyDoShare;
        }

        private void BeforeStart()
        {
            Append(NotifyDoShare, CheckNotifyDoShareResult, TimeOut, 5f);
            Append(NotifyDoShare, CheckNotifyDoShareResult, TimeOut, 10f);
            Append(NotifyDoShare, CheckNotifyDoShareResult, TimeOut, 15f);
        }

        private void NotifyDoShare()
        {
            if (_currentNotifyDoShare == null)
            {
                if (_finishCallBack != null)
                {
                    _finishCallBack();
                }
                return;
            }

            MyLog.InfoWithFrame("WechatShareSeq",
                "FirstNotifyDoShare _currentNotifyDoShare.type == " + _currentNotifyDoShare.type +
                "user_task_code == " + _currentNotifyDoShare.user_task_code + "_currentNotifyDoShare.id =" +
                _currentNotifyDoShare.id);
            _notifyDoShareResult.ClearNotInvalidate();
            _remoteAPI.NotifyDoShare(_currentNotifyDoShare.id, _currentNotifyDoShare.type,
                _currentNotifyDoShare.user_task_code,
                _currentNotifyDoShare.award_code, _currentNotifyDoShare.share_source);
        }

        private bool CheckNotifyDoShareResult()
        {
            var notifyDoShareResult = _notifyDoShareResult.Read();
            if (notifyDoShareResult == null)
            {
                return false;
            }

            if (_finishCallBack != null)
            {
                MyLog.InfoWithFrame("WechatShareSeq", "_finishCallBack");
                _finishCallBack();
            }

            MyLog.InfoWithFrame("WechatShareSeq", "_currentNotifyDoShare id ==" + _currentNotifyDoShare.id);
            MyLog.InfoWithFrame("WechatShareSeq", "notifyDoShareResult id ==" + notifyDoShareResult.id);

            return true;
        }

        private void TimeOut()
        {
            MyLog.InfoWithFrame("WechatShareSeq", "share res time out");
        }
    }
}