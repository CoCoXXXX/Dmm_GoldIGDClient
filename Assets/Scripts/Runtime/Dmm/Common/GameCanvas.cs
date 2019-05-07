using Dmm.Widget;
using UnityEngine;

namespace Dmm.Common
{
    public class GameCanvas : MonoBehaviour, IGameCanvas
    {
        public AsyncImage Background;

        public AsyncImage GetBackground()
        {
            return Background;
        }

        public CanvasGroup WindowSwitchCover;

        public CanvasGroup GetWindowSwitchCover()
        {
            return WindowSwitchCover;
        }

        public WaitingDialog WaitingDialog;

        public WaitingDialog GetWaitingDialog()
        {
            return WaitingDialog;
        }

        #region UI Container

        public RectTransform WindowParent;

        public RectTransform GetWindowParent()
        {
            return WindowParent;
        }

        public RectTransform DialogParent;

        public RectTransform GetDialogContainer()
        {
            return DialogParent;
        }

        public RectTransform SystemMsgParent;

        public RectTransform GetSystemMsgParent()
        {
            return SystemMsgParent;
        }

        public RectTransform SystemMsgContainer;

        public RectTransform GetSystemMsgContainer()
        {
            return SystemMsgContainer;
        }

        public RectTransform ToastParent;

        public RectTransform GetToastParent()
        {
            return ToastParent;
        }

        /// <summary>
        /// 放在GameCanvas中的LoginModeWindow 不要AssetBundle加载
        /// </summary>
        public RectTransform LoginModeWindow;

        public RectTransform GetLoginModeWindow()
        {
            return LoginModeWindow;
        }
        
        #endregion

        #region Canvas

        public RectTransform Canvas;

        public float GetCanvasWidth()
        {
            if (!Canvas)
            {
                return 0;
            }

            return Canvas.rect.width;
        }

        public float GetCanvasHeight()
        {
            if (!Canvas)
            {
                return 0;
            }

            return Canvas.rect.height;
        }

        #endregion
    }
}