using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class VisitorRegularizeDialog : MyDialog
    {
        public InputField UsernameEdt;

        public InputField PasswordEdt;

        public InputField RepasswordEdt;

        public InputField NicknameEdt;

        public Toggle MaleToggle;

        public Toggle FemaleToggle;

        public Text VisitorIDTxt;

        #region Container

        private IDataContainer<VisitorRegularizeResult> _visitorRegularizeResult;

        #endregion

        private void OnEnable()
        {
            _visitorRegularizeResult =
                GetDataRepository().GetContainer<VisitorRegularizeResult>(DataKey.VisitorRegularizeResult);
        }

        public void ApplyData(User data)
        {
            if (data == null)
            {
                return;
            }

            MaleToggle.isOn = data.sex == 1;
            FemaleToggle.isOn = data.sex == 0;

            VisitorIDTxt.text = data.username;
        }

        private static string _regularizeUsername;
        private static string _regularizePassword;

        public void DoRegularize()
        {
            string username = null;
            string password = null;
            string repassword = null;
            string nickname = null;
            int sex = 0;

            username = UsernameEdt.text;
            password = PasswordEdt.text;
            repassword = RepasswordEdt.text;
            nickname = NicknameEdt.text;
            sex = MaleToggle.isOn ? 1 : 0;

            var dialogManager = GetDialogManager();
            if (string.IsNullOrEmpty(username))
            {
                dialogManager.ShowToast("请输入账号！", 2, true);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                dialogManager.ShowToast("请输入密码！", 2, true);
                return;
            }

            if (!password.Equals(repassword))
            {
                dialogManager.ShowToast("两次输入的密码不相同！", 2, true);
                return;
            }

            if (string.IsNullOrEmpty(nickname))
            {
                dialogManager.ShowToast("请输入昵称！", 2, true);
                return;
            }

            if (!DataUtil.ValidateUsername(username))
            {
                dialogManager.ShowMessageBox("账号格式出错！\n账号只能由字母、数字、和下划线组成");
                return;
            }

            if (!DataUtil.ValidatePassword(password))
            {
                dialogManager.ShowMessageBox("密码格式出错！\n密码只能由字母、数字、和下划线组成");
                return;
            }

            if (!DataUtil.ValidateNickname(nickname))
            {
                dialogManager.ShowMessageBox("昵称不能超过16个字，不能包含换行");
                return;
            }

            dialogManager.ShowWaitingDialog(true);

            _regularizeUsername = username;
            _regularizePassword = password;

            _visitorRegularizeResult.ClearAndInvalidate(Time.time);
            GetRemoteAPI().VisitorRegularize(username, nickname, password, sex);

            GetTaskManager().ExecuteTask(
                CheckVisitorRegularizeResult,
                () => dialogManager.ShowWaitingDialog(false));

            GetAnalyticManager().Event("visitor_regularize_apply");
        }

        private bool CheckVisitorRegularizeResult()
        {
            var res = _visitorRegularizeResult.Read();
            if (res == null)
            {
                return false;
            }

            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                // 转正成功。
                GetAppController().ClearAppStateData();
                dialogManager.ShowConfirmBox(
                    "恭喜您转正成功！",
                    true, "马上登陆", () =>
                    {
                        LoginRecord.LastUsername = _regularizeUsername;
                        LoginRecord.LastPassword = _regularizePassword;
                        LoginRecord.LastLoginType = LoginRecord.NormalUser;
                        LoginRecord.LastVisitorUsername = _regularizeUsername;
                        LoginRecord.SaveAll();

                        MyLog.InfoWithFrame("VisitorRegularize",
                            string.Format("save visitorUsername:{0}", _regularizeUsername));
#if UNITY_IOS
                        GetIosSDK().SaveUsername(_regularizeUsername);
#endif
#if UNITY_ANDROID // TODO 将visitorUsername保存到安卓客户端中。
#endif

                        GetNetworkManager().InitLogin();
                    },
                    false, null, null,
                    true, false, false);

                GetRemoteAPI().RequestUserInfo();

                Hide();
            }
            else
            {
                switch (res.result)
                {
                    case ResultCode.P_REGULARIZE_NO_VISITOR:
                        dialogManager.ShowMessageBox("转正失败，当前游客数据不存在！");
                        break;

                    case ResultCode.P_REGISTER_USERNAME_ILLEGAL:
                        dialogManager.ShowMessageBox("账号格式不正确！只能包含字母、数字和下划线");
                        break;

                    case ResultCode.P_REGISTER_PASSWORD_ILLEGAL:
                        dialogManager.ShowMessageBox("密码格式不正确！只能包含字母、数字和下划线");
                        break;

                    case ResultCode.P_REGISTER_NICKNAME_ILLEGAL:
                        dialogManager.ShowMessageBox("昵称不合法！\n不能包含回车字符，长度不能超过16个字");
                        break;

                    case ResultCode.P_REGISTER_USER_EXIST:
                        dialogManager.ShowMessageBox("账号已存在，请换一个重试");
                        break;

                    default:
                        dialogManager.ShowToast("转正失败！", 2, true);
                        break;
                }
            }

            return true;
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}