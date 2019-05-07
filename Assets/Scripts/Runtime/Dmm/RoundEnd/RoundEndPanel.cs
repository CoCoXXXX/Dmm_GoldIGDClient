using com.morln.game.gd.command;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.RoundEnd
{
    public class RoundEndPanel : MonoBehaviour
    {
        public void Awake()
        {
            // 不能在Start中Reset。
            // 否则会出现第一次enable RoundEndPanel的时候，
            // 由于在下一帧调用了Start中的Reset，导致DOTween动画被重置的问题。
            Reset();
        }

        public float ShowAnimationTime = 0.3f;

        public float HideAnimationTime = 0.1f;

        public Image BgCover;

        public TotalRoundEnd TotalRoundEnd;

        public MidRoundEnd MidRoundEnd;

        private Tweener _bgTweener;

        private Tweener _totalTweener;

        private Tweener _midTweener;

        public void Reset()
        {
            ResetTweeners();

            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            BgCover.color = new Color(0, 0, 0, 0);
            if (BgCover.gameObject.activeSelf)
                BgCover.gameObject.SetActive(false);

            TotalRoundEnd.Reset();
            if (TotalRoundEnd.gameObject.activeSelf)
                TotalRoundEnd.gameObject.SetActive(false);

            MidRoundEnd.ResetContent();
            if (MidRoundEnd.gameObject.activeSelf)
                MidRoundEnd.gameObject.SetActive(false);
        }

        public void ShowTotalRoundEnd(BRoundEnd data)
        {
            ResetTweeners();

            if (!TotalRoundEnd)
                return;

            ShowBgCover(true);

            if (MidRoundEnd.gameObject.activeSelf)
                MidRoundEnd.gameObject.SetActive(false);

            if (!TotalRoundEnd.gameObject.activeSelf)
                TotalRoundEnd.gameObject.SetActive(true);

            TotalRoundEnd.Reset();
            TotalRoundEnd.transform.localScale = new Vector3(0, 0, 1);

            _totalTweener = TotalRoundEnd.transform
                .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                .SetEase(Ease.OutBack, 1.1f)
                .OnComplete(() => TotalRoundEnd.RefreshContent(data));
        }

        /// <summary>
        /// 比赛房结算
        /// </summary>
        /// <param name="data"></param>
        public void ShowTotalRoundEnd(com.morln.game.gd.command.RoundEnd data)
        {
            ResetTweeners();

            if (!TotalRoundEnd)
                return;

            ShowBgCover(true);

            if (MidRoundEnd.gameObject.activeSelf)
                MidRoundEnd.gameObject.SetActive(false);

            if (!TotalRoundEnd.gameObject.activeSelf)
                TotalRoundEnd.gameObject.SetActive(true);

            TotalRoundEnd.Reset();
            TotalRoundEnd.transform.localScale = new Vector3(0, 0, 1);

            _totalTweener = TotalRoundEnd.transform
                .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                .SetEase(Ease.OutBack, 1.1f)
                .OnComplete(() => TotalRoundEnd.RaceRefreshContent(data));
        }

        private void HideTotalRoundEnd()
        {
            if (!TotalRoundEnd.gameObject.activeSelf)
                return;

            _totalTweener = TotalRoundEnd.transform
                .DOScale(new Vector3(0, 0, 1), HideAnimationTime)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                    {
                        TotalRoundEnd.Reset();
                        TotalRoundEnd.gameObject.SetActive(false);
                    }
                );
        }

        public void ShowMidRoundEnd(MidRoundEnd msg)
        {
            ResetTweeners();

            if (TotalRoundEnd.gameObject.activeSelf)
                TotalRoundEnd.gameObject.SetActive(false);

            if (!MidRoundEnd.gameObject.activeSelf)
                MidRoundEnd.gameObject.SetActive(true);

            MidRoundEnd.transform.localScale = new Vector3(0, 0, 1);
            _midTweener = MidRoundEnd.transform
                .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                .SetEase(Ease.OutBack, 1.1f);
        }

        private void HideMidRoundEnd()
        {
            if (!MidRoundEnd.gameObject.activeSelf)
                return;

            _midTweener = MidRoundEnd.transform
                .DOScale(new Vector3(0, 0, 1), HideAnimationTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => MidRoundEnd.gameObject.SetActive(false));
        }

        public void Hide()
        {
            ResetTweeners();

            ShowBgCover(false);
            HideTotalRoundEnd();
            HideMidRoundEnd();
        }

        private void ShowBgCover(bool show)
        {
            if (_bgTweener != null)
            {
                _bgTweener.Kill();
                _bgTweener = null;
            }

            if (show)
            {
                if (!BgCover.gameObject.activeSelf)
                    BgCover.gameObject.SetActive(true);

                BgCover.color = new Color(0, 0, 0, 0);
                _bgTweener = BgCover
                    .DOFade(150f / 255f, ShowAnimationTime)
                    .SetEase(Ease.Linear);
            }
            else
            {
                if (BgCover.gameObject.activeSelf)
                {
                    _bgTweener = BgCover
                        .DOFade(0, HideAnimationTime)
                        .SetEase(Ease.Linear)
                        .OnComplete(() => BgCover.gameObject.SetActive(false));
                }
            }
        }

        private void ResetTweeners()
        {
            if (_totalTweener != null)
            {
                _totalTweener.Kill();
                _totalTweener = null;
            }

            if (_midTweener != null)
            {
                _midTweener.Kill();
                _midTweener = null;
            }
        }
    }
}