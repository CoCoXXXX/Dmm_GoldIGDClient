using Dmm.Dialog;
using Dmm.Util;
using UnityEngine.UI;

namespace Dmm.Login
{
    public class RegisterDialog : MyDialog
    {
        public InputField UsernameEdt;

        public InputField PasswordEdt;

        public InputField RePasswordEdt;

        public InputField NicknameEdt;

        public Toggle MaleToggle;

        public Toggle FemaleToggle;

        public void DoRegister()
        {
            string username = null;
            string password = null;
            string repassword = null;
            string nickname = null;
            // 默认是女性用户角色吧。
            int sex = 0;

            username = UsernameEdt.text;
            password = PasswordEdt.text;
            repassword = RePasswordEdt.text;
            nickname = NicknameEdt.text;
            if (MaleToggle)
            {
                sex = MaleToggle.isOn ? 1 : 0;
            }

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

            GetRemoteAPI().Register(username, nickname, password, sex);
            GetAnalyticManager().Event("register_apply");
        }
    }
}