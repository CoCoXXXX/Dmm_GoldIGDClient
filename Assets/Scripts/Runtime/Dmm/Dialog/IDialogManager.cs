using System;
using Dmm.Common;
using Dmm.Task;
using Dmm.Widget;

namespace Dmm.Dialog
{
    public interface IDialogManager
    {
        UIWindow GetCachedDialog(string dialogName);

        void HideDialog(string dialogName);

        void AddDialogToCacheDialog(string dialogName, UIWindow dialog);

        /// <summary>
        /// 从Assetbundle中加载对话框，加载完成添加到dictionary，回传dialog给success
        /// </summary>
        /// <param name="dialogName"></param>
        /// <param name="success"></param>
        /// <typeparam name="T"></typeparam>
        void ShowDialog<T>(string dialogName, bool autoShow = true,
            bool cache = false, Action<T> success = null) where T : UIWindow;

        void RequestDialog<T>(
            string dialogName,
            Action action,
            Func<TaskResult> checker,
            Action success,
            Action<int, string> failHandler,
            Action timeoutHandler = null,
            int timeout = 10,
            bool showWaiting = true) where T : UIWindow;

        AlertBox ShowMessageBox(string content);

        AlertBox ShowConfirmBox(string content);

        AlertBox ShowConfirmBox(
            string content,
            bool enableOkBtn, string okBtnContent, AlertBox.OnClickDelegate onClickOk,
            bool enableCancelBtn, string cancelBtnContent, AlertBox.OnClickDelegate onClickCancel,
            bool enableBg, bool closeOnBg, bool enableCloseBtn);

        void ShowWaitingDialog(bool show, float timeout = 30);

        void ShowToast(string content, float time, bool error = false);

        void InitAutoShowDialogDataQueue();

        void UpdateAutoShowDialog();
    }
}