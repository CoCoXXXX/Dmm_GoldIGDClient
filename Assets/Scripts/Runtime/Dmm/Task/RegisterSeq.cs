using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Network;

namespace Dmm.Task
{
    public class RegisterSeq : ActionSequence
    {
        public const string Tag = "Register";

        private readonly string _registerUsername;
        private readonly string _registerNickname;
        private readonly string _registerPassword;
        private readonly int _registerSex;
        private bool _registerFinished = false;

        private readonly INetworkManager _network;
        private readonly IAppController _appController;
        private readonly IDialogManager _dialogManager;

        private readonly RemoteAPI _remoteAPI;

        private readonly IDataContainer<HLoginResult> _hLoginResult;

        private readonly IDataContainer<HRegisterResult> _hRegisterResult;

        public RegisterSeq(string username, string nickname, string password, int sex, IAppController appController,
            IMessageRouter messageRouter, INetworkManager network, IDialogManager dialogManager,
            RemoteAPI remoteAPI, IDataRepository dataRepository)
        {
            _registerUsername = username;
            _registerNickname = nickname;
            _registerPassword = password;
            _registerSex = sex;

            StartListener = BeforeStart;

            _appController = appController;
            _network = network;
            _remoteAPI = remoteAPI;
            _dialogManager = dialogManager;
            _hLoginResult = dataRepository.GetContainer<HLoginResult>(DataKey.HLoginResult);
            _hRegisterResult = dataRepository.GetContainer<HRegisterResult>(DataKey.HRegisterResult);
        }

        public void BeforeStart()
        {
            _registerFinished = false;

            if (IsLoginOk())
            {
                // 已经登陆成功的情况下，向HServer发送注册命令。
                Append(ClearLoginData, () => true);
                Append(SendRegisterMsg, CheckHRegisterResult, () => _dialogManager.ShowWaitingDialog(false));
            }
            else
            {
                // 未登陆成功的情况下，则连接PServer，向PServer发送注册命令。
                Cancel();
            }
        }

        private bool IsLoginOk()
        {
            var data = _hLoginResult.Read();
            if (data == null)
            {
                return false;
            }

            return data.result == ResultCode.OK;
        }

        #region HServer注册流程

        private void ClearLoginData()
        {
            _dialogManager.ShowWaitingDialog(true);
            _hRegisterResult.ClearNotInvalidate();
        }

        private void SendRegisterMsg()
        {
            _remoteAPI.HRegister(_registerUsername, _registerPassword, _registerNickname, _registerSex);
            MyLog.InfoWithFrame("Register", "register to hserver.");
        }

        #endregion

        #region 注册流程

        private bool CheckHRegisterResult()
        {
            var res = _hRegisterResult.Read();
            if (res == null)
                return false;

            _dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                RegisterOk();
                MyLog.InfoWithFrame(Tag, "hregister ok.");
            }
            else
            {
                // 注册失败已经有提示了，不需要做任何动作。
                RegisterFail(null);
                MyLog.InfoWithFrame(Tag, "hregister fail.");
            }

            return true;
        }

        /// <summary>
        /// 注册成功
        /// </summary>
        private void RegisterOk()
        {
            _registerFinished = true;

            _dialogManager.ShowWaitingDialog(false);

            _dialogManager.HideDialog(DialogName.RegisterDialog);
            _dialogManager.ShowConfirmBox(
                "恭喜您注册成功！",
                true, "马上登陆", () =>
                {
                    LoginRecord.LastUsername = _registerUsername;
                    LoginRecord.LastPassword = _registerPassword;
                    LoginRecord.LastLoginType = LoginRecord.NormalUser;
                    LoginRecord.SaveAll();

                    _appController.ClearAppStateData();
                    _network.InitLogin();
                },
                false, null, null,
                true, false, true);
            _remoteAPI.RequestUserInfo();
        }

        /// <summary>
        /// 注册失败
        /// </summary>
        private void RegisterFail(string errMsg)
        {
            _registerFinished = true;

            _dialogManager.ShowWaitingDialog(false);

            Cancel();

            if (!string.IsNullOrEmpty(errMsg))
                _dialogManager.ShowToast(errMsg, 2, true);
        }

        #endregion
    }
}