using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class AlertBox : MonoBehaviour
    {
        public Image Bg;

        private Tweener _bgTweener;

        public RectTransform Frame;

        private Tweener _frameTweener;

        public Button BgBtn;

        public Button CloseBtn;

        public Text ContentTxt;

        public GameObject BtnContainer;

        public Button OkBtn;

        public Text OkBtnTxt;

        public Button CancelBtn;

        public Text CancelBtnTxt;

        public float ShowAnimationTime = 0.2f;

        public float HideAnimationTime = 0.1f;

        public delegate void OnClickDelegate();

        private OnClickDelegate _onOkListener;

        private OnClickDelegate _onCancelListener;

        #region 显示和隐藏

        public void Show(
            string content,
            bool enableOkBtn = true,
            string okBtnContent = "确定",
            OnClickDelegate onOkListener = null,
            bool enableCancelbtn = false,
            string cancelBtnContent = null,
            OnClickDelegate onCancelListener = null,
            bool enableBg = true,
            bool closeOnBg = false,
            bool enableCloseBtn = true
        )
        {
            // 设置内容。
            if (ContentTxt)
            {
                if (!ContentTxt.gameObject.activeSelf)
                {
                    ContentTxt.gameObject.SetActive(true);
                }

                ContentTxt.text = content;
            }

            if (Bg)
            {
                if (Bg.gameObject.activeSelf != enableBg)
                {
                    Bg.gameObject.SetActive(enableBg);
                }
            }

            // 背景按钮。
            if (BgBtn)
            {
                BgBtn.interactable = closeOnBg;
            }

            // 关闭按钮。
            if (CloseBtn && CloseBtn.gameObject.activeSelf != enableCloseBtn)
            {
                CloseBtn.gameObject.SetActive(enableCloseBtn);
            }

            // 确认按钮。
            if (OkBtn)
            {
                if (OkBtn.gameObject.activeSelf != enableOkBtn)
                {
                    OkBtn.gameObject.SetActive(enableOkBtn);
                }
            }

            if (enableOkBtn && OkBtnTxt)
            {
                OkBtnTxt.text = okBtnContent;
            }

            // 取消按钮。
            if (CancelBtn)
            {
                if (CancelBtn.gameObject.activeSelf != enableCancelbtn)
                {
                    CancelBtn.gameObject.SetActive(enableCancelbtn);
                }
            }

            if (enableCancelbtn && CancelBtnTxt)
            {
                CancelBtnTxt.text = cancelBtnContent;
            }

            // 确认按钮和取消按钮的容器。
            if (BtnContainer)
            {
                var enableContainer = enableOkBtn || enableCancelbtn;
                if (BtnContainer.activeSelf != enableContainer)
                {
                    BtnContainer.SetActive(enableContainer);
                }
            }

            _onOkListener = onOkListener;
            _onCancelListener = onCancelListener;

            // 显示对话框。
            ResetAllTweeners();
            if (Frame)
            {
                // 显示对话框的时候，是应该必须显示内容的。
                if (!Frame.gameObject.activeSelf)
                {
                    Frame.gameObject.SetActive(true);
                }

                Frame.localScale = new Vector3(0, 0, 1);
                _frameTweener = Frame
                    .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                    .SetEase(Ease.OutBack, 1.1f);
            }

            if (Bg && Bg.gameObject.activeSelf)
            {
                _bgTweener = Bg.DOFade(150f / 255f, ShowAnimationTime);
            }
        }

        public void Close()
        {
            ResetAllTweeners();

            if (Frame && Frame.gameObject.activeSelf)
            {
                _frameTweener = Frame
                    .DOScale(new Vector3(0, 0, 1), HideAnimationTime)
                    .OnComplete(() => Destroy(gameObject));
            }

            if (Bg && Bg.gameObject.activeSelf)
            {
                _bgTweener = Bg
                    .DOFade(0, HideAnimationTime)
                    .OnComplete(() => Bg.gameObject.SetActive(false));
            }
        }

        private void ResetAllTweeners()
        {
            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            if (_frameTweener != null)
            {
                _frameTweener.Kill();
                _frameTweener = null;
            }
        }

        #endregion

        #region 按钮事件

        public void Ok()
        {
            if (_onOkListener != null)
            {
                _onOkListener();
            }

            Close();
        }

        public void Cancel()
        {
            if (_onCancelListener != null)
            {
                _onCancelListener();
            }

            Close();
        }

        public void OnClickOk()
        {
            Ok();
        }

        public void OnClickCancel()
        {
            Cancel();
        }

        public void OnClickBg()
        {
            Cancel();
        }

        public void OnClickClose()
        {
            Cancel();
        }

        #endregion
    }
}