using Dmm.Widget;
using UnityEngine;

namespace Dmm.Common
{
    /// <summary>
    /// 游戏的Canvas。
    /// </summary>
    public interface IGameCanvas
    {
        AsyncImage GetBackground();
        CanvasGroup GetWindowSwitchCover();
        WaitingDialog GetWaitingDialog();

        RectTransform GetWindowParent();
        RectTransform GetDialogContainer();

        RectTransform GetSystemMsgParent();
        RectTransform GetSystemMsgContainer();

        RectTransform GetToastParent();

        RectTransform GetLoginModeWindow();

        #region Canvas

        float GetCanvasWidth();
        float GetCanvasHeight();

        #endregion
    }
}