using System.Collections;
using DG.Tweening;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Login
{
    public class LoginPanel : UIWindow
    {
        #region Inject

        private IDialogManager _dialogManager;

        private IDataContainer<string> _serviceQQ;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
            _serviceQQ = dataRepository.GetContainer<string>(DataKey.ServiceQQ);
        }

        #endregion

        public Image Bg;

        public RectTransform NormalLoginContent;

        public InputField Username;

        public InputField Password;

        public Button LoginBtn;

        public Button ForgetPasswordBtn;

        public float ShowAnimationTime = 0.2f;

        private Tweener _bgTweener;

        private Tweener _normalLoginContentTweener;

        private Tweener _loginTypeContentTweener;

        public override void Show()
        {
            StartCoroutine(ShowCoroutine());
        }

        private IEnumerator ShowCoroutine()
        {
            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            if (_normalLoginContentTweener != null)
            {
                _normalLoginContentTweener.Kill();
                _normalLoginContentTweener = null;
            }

            if (_loginTypeContentTweener != null)
            {
                _loginTypeContentTweener.Kill();
                _loginTypeContentTweener = null;
            }

            if (Bg)
            {
                if (!Bg.gameObject.activeSelf)
                    Bg.gameObject.SetActive(true);

                Bg.color = new Color(0, 0, 0, 0);
            }

            // 正常账号登陆对话框，不显示。
            if (NormalLoginContent)
            {
                if (!NormalLoginContent.gameObject.activeSelf)
                    NormalLoginContent.gameObject.SetActive(true);

                NormalLoginContent.transform.localScale = new Vector3(0, 0, 1);
            }

            yield return null;

            if (Bg)
            {
                _bgTweener = Bg
                    .DOColor(new Color(0, 0, 0, 150f / 255f), ShowAnimationTime)
                    .SetEase(Ease.Linear);
            }

            if (NormalLoginContent)
            {
                _normalLoginContentTweener = NormalLoginContent
                    .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                    .SetEase(Ease.OutBack);
            }

            Username.text = LoginRecord.LastUsername;
            Password.text = LoginRecord.LastPassword;
        }

        public float HideAnimationTime = 0.1f;

        public override void Hide()
        {
            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            // 关闭背景图片。
            if (Bg)
            {
                _bgTweener = Bg
                    .DOColor(new Color(0, 0, 0, 0), HideAnimationTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        if (Bg.gameObject.activeSelf)
                            Bg.gameObject.SetActive(false);

                        Destroy(gameObject);
                    });
            }

            // 关闭正常登陆对话框。
            if (_normalLoginContentTweener != null)
            {
                _normalLoginContentTweener.Kill();
                _normalLoginContentTweener = null;
            }

            if (NormalLoginContent)
            {
                _normalLoginContentTweener = NormalLoginContent
                    .DOScale(new Vector3(0, 0, 0), HideAnimationTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        if (NormalLoginContent.gameObject.activeSelf)
                            NormalLoginContent.gameObject.SetActive(false);
                    });
            }

            // 关闭登陆类型选择框。
            if (_loginTypeContentTweener != null)
            {
                _loginTypeContentTweener.Kill();
                _loginTypeContentTweener = null;
            }
        }

        public void Login()
        {
            string u = null;
            string p = null;

            if (Username)
            {
                u = Username.text;
            }
            if (Password)
            {
                p = Password.text;
            }

            if (string.IsNullOrEmpty(u))
            {
                Toast("用户名不能为空");
                return;
            }

            if (string.IsNullOrEmpty(p))
            {
                Toast("密码不能为空");
                return;
            }

            LoginRecord.CurrentLoginType = LoginRecord.NormalUser;
            LoginRecord.LastUsername = u;
            LoginRecord.LastPassword = p;
            LoginRecord.SaveAll();
        }

        public void ForgetPassword()
        {
            var serviceQQ = _serviceQQ.Read();
            _dialogManager.ShowConfirmBox(string.Format("请联系官方客服：{0}", serviceQQ));
        }

        public void Register()
        {
            _dialogManager.ShowDialog<RegisterDialog>(DialogName.RegisterDialog, true, true);

            // 打开注册对话框的同时关闭登陆对话框。
            Hide();
        }

        private void Toast(string content)
        {
            _dialogManager.ShowToast(content, 3);
        }
    }
}