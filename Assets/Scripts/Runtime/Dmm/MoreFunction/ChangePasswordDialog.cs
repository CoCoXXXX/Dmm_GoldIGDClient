using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.MoreFunction
{
    public class ChangePasswordDialog : MyDialog
    {
        public InputField OldPasswordEdt;

        public InputField NewPasswordEdt;

        #region Container

        private IDataContainer<EditPasswordResult> _changePasswordResult;

        #endregion

        private void OnEnable()
        {
            var dataRepository = GetDataRepository();
            _changePasswordResult =
                dataRepository.GetContainer<EditPasswordResult>(DataKey.EditPasswordResult);
        }

        public void ConfirmChange()
        {
            string oldPassword = null;
            string newPassword = null;

            if (OldPasswordEdt)
            {
                oldPassword = OldPasswordEdt.text;
            }
            if (NewPasswordEdt)
            {
                newPassword = NewPasswordEdt.text;
            }

            var dialogManager = GetDialogManager();
            if (string.IsNullOrEmpty(oldPassword))
            {
                dialogManager.ShowToast("请输入旧密码！", 3, true);
                return;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                dialogManager.ShowToast("请输入新密码！", 3, true);
                return;
            }

            if (StringUtil.AreEqual(oldPassword, newPassword))
            {
                dialogManager.ShowToast("新密码与旧密码相同，无需修改！", 3);
                return;
            }

            dialogManager.ShowWaitingDialog(true);

            _changePasswordResult.ClearAndInvalidate(Time.time);
            GetRemoteAPI().ChangePassword(oldPassword, newPassword);

            GetTaskManager().ExecuteTask(
                CheckChangePasswordResult,
                () => dialogManager.ShowWaitingDialog(false));
        }

        private bool CheckChangePasswordResult()
        {
            var res = _changePasswordResult.Read();
            if (res == null)
            {
                return false;
            }

            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                dialogManager.ShowToast("修改密码成功！", 2);
                Hide();
            }
            else
            {
                switch (res.result)
                {
                    case ResultCode.H_EDIT_PASSWORD_ILLEGAL:
                        dialogManager.ShowMessageBox("密码格式错误！\n密码只可以使用大小写字母与下划线");
                        break;

                    case ResultCode.H_EDIT_PASSWORD_NOT_AUTH:
                        dialogManager.ShowToast("原密码错误，请重新输入！", 3, true);
                        break;

                    default:
                        dialogManager.ShowToast("修改密码失败，请重试！", 2, true);
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