using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class WaitingDialog : MonoBehaviour
    {
        public Image Bg;

        public Image Image;

        public delegate void TimeoutHandler();

        public float RotateSpeed = 360;

        public float ShowAnimationTime = 0.5f;

        public float HideAnimationTime = 0.05f;

        private Tweener _bgTweener;

        private Tweener _imgTweener;

        private bool _show;

        private float _showTime;

        private float _timeout;

        public void Show(float timeout)
        {
            _showTime = Time.time;
            _timeout = timeout;

            if (!_show)
            {
                ResetAllTweeners();

                if (Bg)
                {
                    if (!Bg.gameObject.activeSelf)
                        Bg.gameObject.SetActive(true);

                    _bgTweener = Bg.DOFade(100f / 255f, ShowAnimationTime);
                }

                if (Image)
                {
                    if (!Image.gameObject.activeSelf)
                        Image.gameObject.SetActive(true);

                    _imgTweener = Image.DOFade(1, ShowAnimationTime);
                }

                _show = true;
            }
        }

        public void Hide()
        {
            _show = false;

            ResetAllTweeners();

            if (Bg && Bg.gameObject.activeSelf)
            {
                _bgTweener = Bg
                    .DOFade(0, HideAnimationTime)
                    .OnComplete(() => Bg.gameObject.SetActive(false));
            }

            if (Image && Image.gameObject.activeSelf)
            {
                _imgTweener = Image
                    .DOFade(0, HideAnimationTime)
                    .OnComplete(() => Image.gameObject.SetActive(false));
            }
        }

        private void ResetAllTweeners()
        {
            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            if (_imgTweener != null)
            {
                _imgTweener.Kill();
                _imgTweener = null;
            }
        }

        public void Update()
        {
            if (!_show) return;

            if (Image)
            {
                var r = Image.rectTransform.rotation.eulerAngles;
                Image.rectTransform.rotation = Quaternion.Euler(0, 0, r.z - RotateSpeed * Time.deltaTime);
            }

            if (Time.time >= _showTime + _timeout)
                Hide();
        }
    }
}