using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.StateLogic;
using UnityEngine;

namespace Dmm.Network
{
    public class NetworkLoginHallServerOkState : StateAdapter<IAppContext>
    {
        private const string Tag = "NetworkLoginHallServerOkState";

        private IDataContainer<bool> _autoShowCheckinDialog;

        private IDataContainer<User> _myUser;

        public override int GetStateCode()
        {
            return NetworkState.LoginHallServerOk;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            MyLog.DebugWithFrame(Tag, "Enter login ok state.");
            _heartbeatTime = -1;
            _heartbeat = false;

            var dataRepository = context.GetDataRepository();
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);

            // 登陆成功的时候，尝试关闭断线对话框。
            var dialogManager = context.GetDialogManager();
            dialogManager.HideDialog(DialogName.DisconnectedDialog);

            //初始化需要自动打开的对话框队列
            dialogManager.InitAutoShowDialogDataQueue();
            CheckShowNickNameDialog(context);
        }

        /// <summary>
        /// 检测是否显示编辑昵称
        /// </summary>
        /// <param name="context"></param>
        private void CheckShowNickNameDialog(IAppContext context)
        {
            var dialogManager = context.GetDialogManager();

            var user = _myUser.Read();
            if (user != null)
            {
                if (LoginRecord.LastLoginType == LoginRecord.Visitor)
                {
                    if (string.IsNullOrEmpty(user.nickname))
                    {
                        // 显示选择昵称的对话框。
                        dialogManager.ShowDialog<VisitorChooseNicknameDialog>(DialogName.VisitorChooseNicknameDialog);
                    }
                }
            }
        }

        public override bool Process(IAppContext context, float time)
        {
            if (_pausing)
            {
                // 如果在暂停中，则不检查网络状态。
                return false;
            }

            // 检查网络是否断线。
            var network = context.GetNetworkManager();
            if (!network.IsConnected())
            {
                // 已经断线，需要重新登陆。
                MyLog.InfoWithFrame(Tag, "Socket Disconnected");
                Relogin(context);
                return true;
            }

            // 检查心跳情况。
            var msgRouter = context.GetMessageRouter();
            var lastResponseTime = msgRouter.GetLastResponseTime();

            if (_hasPaused)
            {
                if (!_heartbeat)
                {
                    // 没发送过心跳，则发送一次心跳。
                    SendHeartbeat(context, time);
                    return false;
                }

                if (lastResponseTime >= _heartbeatTime)
                {
                    // 发送过心跳，已经收到新消息了。
                    HeartbeatFinish(time);
                    _hasPaused = false;
                    return false;
                }

                // 发送过心跳，还没收到消息，则检查是否心跳已经超时。

                if (time - _heartbeatTime >= OnResumeHeartbeatTimeout)
                {
                    // 心跳超时了，则执行心跳超时逻辑。
                    _hasPaused = false;
                    HeartbeatTimeoutLogic(context, time);
                    return true;
                }

                return false;
            }

            // 没有发送过心跳消息：
            if (!_heartbeat)
            {
                // 如果已经Idle了，则向服务器发送一次心跳。
                if (time - lastResponseTime >= IdleTime)
                {
                    SendHeartbeat(context, time);
                }

                return false;
            }

            // 已经发送过心跳消息：

            // 收到过新消息，则重置心跳时间。
            if (lastResponseTime >= _heartbeatTime)
            {
                HeartbeatFinish(time);
                return false;
            }

            // 心跳尚未超时，则继续检查。
            if (time - _heartbeatTime < HeartbeatTimeout)
            {
                return false;
            }

            // 尚未收到新消息，而且当前时间与心跳时间之间的时间差，已经超过timeout了。
            HeartbeatTimeoutLogic(context, time);
            return true;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            // 断线之后一般直接重连游戏服务器。
            MyLog.InfoWithFrame(Tag, "Exit login ok state.");
            return null;
        }

        private void SendHeartbeat(IAppContext context, float time)
        {
            MyLog.InfoWithFrame(Tag, string.Format("Send heart beat msg at: {0}", time));

            // 如果已经Idle了，则向服务器发送一次心跳。
            var network = context.GetNetworkManager();
            var server = network.GetServer();
            var hbreq = CmdUtil.Shared.HBReq(0, server);
            var msgRepo = context.GetMsgRepo();
            msgRepo.SendMsg(hbreq);

            _heartbeatTime = time;
            _heartbeat = true;
        }

        private void HeartbeatFinish(float time)
        {
            MyLog.InfoWithFrame(Tag, string.Format("Heartbeat finish at: {0}", time));

            _heartbeat = false;
            _heartbeatTime = -1;
        }

        private void HeartbeatTimeoutLogic(IAppContext context, float time)
        {
            MyLog.InfoWithFrame(Tag, string.Format("Heartbeat timeout at: {0}", time));
            HeartbeatFinish(time);

            Relogin(context);
        }

        #region 心跳参数

        /// <summary>
        /// 是否已经发送过心跳。
        /// </summary>
        private bool _heartbeat = false;

        /// <summary>
        /// 发送心跳的时间。
        /// </summary>
        private float _heartbeatTime;

        /// <summary>
        /// 最后一次收到消息之后，过了IdleTime还没有收到消息的话，则认为进入了Idle状态，此时应当立即发送一个心跳请求。
        /// 如果心跳请求发送之后，HBTimeOut之内没有收到任何新的消息。则视为断线。
        /// </summary>
        public float IdleTime = 5f;

        /// <summary>
        /// 发送心跳消息之后多长时间没有收到任何消息，则视为掉线。
        /// </summary>
        public float HeartbeatTimeout = 10f;

        /// <summary>
        /// 应用出现暂停的时候，发送心跳之后的反应时间。
        /// </summary>
        public float OnResumeHeartbeatTimeout = 5;

        #endregion

        #region Pause

        private bool _hasPaused = false;

        private bool _pausing = false;

        public override void OnPause(IAppContext context, bool pause, float time)
        {
            MyLog.InfoWithFrame(Tag, string.Format("OnPause: {0}, at: {1}", pause, time));
            _pausing = pause;

            if (pause)
            {
                _hasPaused = true;
            }
        }

        #endregion

        private void Relogin(IAppContext context)
        {
            var dialog = context.GetDialogManager();

            dialog.ShowConfirmBox("与服务器断开连接，请检查您的网络连接是否异常", true, "重新连接",
                () =>
                {
                    var network = context.GetNetworkManager();
                    network.InitLogin();
                }, false, "", null, true, false, false);
        }
    }
}