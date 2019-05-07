using DG.Tweening;
using Dmm.Analytic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Dmm.Game
{
    public class ShowDesktopBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Inject

        private IAnalyticManager _analyticManager;

        [Inject]
        public void Initialize(IAnalyticManager analyticManager)
        {
            _analyticManager = analyticManager;
        }

        #endregion

        public float AnimationTime = 0.2f;

        public float CardLayoutAlpha = 0.2f;

        public CanvasGroup CardLayoutContent;

        private Tweener _currentTweener;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_currentTweener != null)
            {
                _currentTweener.Kill();
                _currentTweener = null;
            }

            if (CardLayoutContent)
            {
                _currentTweener = CardLayoutContent.DOFade(CardLayoutAlpha, AnimationTime);
            }

            _analyticManager.Event("game_show_desktop");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_currentTweener != null)
            {
                _currentTweener.Kill();
                _currentTweener = null;
            }

            if (CardLayoutContent)
            {
                _currentTweener = CardLayoutContent.DOFade(1, AnimationTime);
            }
        }
    }
}