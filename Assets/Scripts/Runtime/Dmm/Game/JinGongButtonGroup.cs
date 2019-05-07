using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Game
{
    public class JinGongButtonGroup : MonoBehaviour
    {
        #region Inject

        private ISoundController _soundController;

        private IDataContainer<TableUserData> _tableUserData;

        private IDataContainer<PlayingData> _playingData;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            ISoundController soundController)
        {
            _soundController = soundController;
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        #endregion

        public CardLayout CardLayout;

        public RectTransform HuanGongBtnTransform;

        public RectTransform JinGongBtnTransform;

        public float ShowAnimationTime = 0.2f;

        public float HideAnimationTime = 0.1f;

        private float _refreshTime;

        public void RefreshContent()
        {
            var time = Mathf.Max(_playingData.Timestamp);

            if (_refreshTime >= time)
            {
                return;
            }

            _refreshTime = time;

            var playingData = _playingData.Read();

            var tableUser = _tableUserData.Read();

            if (playingData == null)
            {
                return;
            }

            var tablePeriod = playingData.period;
            if (tablePeriod == TablePeriod.JinGong)
            {
                ShowHuanGongBtn(false);

                var mySeat = tableUser.MySeat;

                var show = false;
                if (playingData.jg_seat1 == mySeat)
                {
                    show = playingData.jg_poker1 == -1;
                }
                else if (playingData.jg_seat2 == mySeat)
                {
                    show = playingData.jg_poker2 == -1;
                }

                ShowJinGongBtn(show);
            }
            else if (tablePeriod == TablePeriod.HuanGong)
            {
                ShowJinGongBtn(false);

                var mySeat = tableUser.MySeat;

                var show = false;
                if (playingData.hg_seat1 == mySeat)
                {
                    show = playingData.hg_poker1 == -1;
                }
                else if (playingData.hg_seat2 == mySeat)
                {
                    show = playingData.hg_poker2 == -1;
                }

                ShowHuanGongBtn(show);
            }
        }

        private Tweener _jinGongTweener;

        public void ShowJinGongBtn(bool show)
        {
            if (_jinGongTweener != null)
            {
                _jinGongTweener.Kill();
                _jinGongTweener = null;
            }

            if (show)
            {
                _soundController.PlayPleaseChuPaiSound();
                CardLayout.SelectJinGongPoker();

                if (!JinGongBtnTransform.gameObject.activeSelf)
                {
                    JinGongBtnTransform.gameObject.SetActive(true);
                }

                JinGongBtnTransform.localScale = new Vector3(0, 0, 1);
                _jinGongTweener = JinGongBtnTransform
                    .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                    .SetEase(Ease.OutBack, 1.1f);
            }
            else
            {
                if (JinGongBtnTransform.gameObject.activeSelf)
                {
                    _jinGongTweener = JinGongBtnTransform
                        .DOScale(new Vector3(0, 0, 1), HideAnimationTime)
                        .SetEase(Ease.Linear)
                        .OnComplete(() => JinGongBtnTransform.gameObject.SetActive(false));
                }
            }
        }

        private Tweener _huanGongTweener;

        public void ShowHuanGongBtn(bool show)
        {
            if (_huanGongTweener != null)
            {
                _huanGongTweener.Kill();
                _huanGongTweener = null;
            }

            if (show)
            {
                _soundController.PlayPleaseChuPaiSound();

                if (!HuanGongBtnTransform.gameObject.activeSelf)
                {
                    HuanGongBtnTransform.gameObject.SetActive(true);
                }

                HuanGongBtnTransform.localScale = new Vector3(0, 0, 1);
                _huanGongTweener = HuanGongBtnTransform
                    .DOScale(new Vector3(1, 1, 1), ShowAnimationTime)
                    .SetEase(Ease.OutBack, 1.1f);
            }
            else
            {
                if (HuanGongBtnTransform.gameObject.activeSelf)
                {
                    _huanGongTweener = HuanGongBtnTransform
                        .DOScale(new Vector3(0, 0, 1), HideAnimationTime)
                        .SetEase(Ease.Linear)
                        .OnComplete(() => HuanGongBtnTransform.gameObject.SetActive(false));
                }
            }
        }
    }
}