using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Sound;
using UnityEngine;
using Zenject;

namespace Dmm.Game
{
    public class ChuPaiButtonGroup : MonoBehaviour
    {
        #region Inject

        private ISoundController _soundController;

        private IDataContainer<TableUserData> _tableUser;

        private IDataContainer<PlayingData> _playingData;

        [Inject]
        public void Initialize(IDataRepository dataRepository, ISoundController soundController)
        {
            _soundController = soundController;
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        #endregion

        public RectTransform ChuPaiGroupTransform;

        public RectTransform BuChuGroupTransform;

        /// <summary>
        /// 出牌按钮的刷新时间。
        /// </summary>
        private float _refreshTime;

        public void RefreshContent()
        {
            var time = _playingData.Timestamp;
            if (_refreshTime >= time)
            {
                return;
            }

            _refreshTime = time;

            var tableUser = _tableUser.Read();
            var playingData = _playingData.Read();

            if (tableUser == null || playingData == null)
            {
                ShowChuPaiGroup(false);
                return;
            }

            var mySeat = tableUser.MySeat;
            var currentChuPaiSeat = playingData.chupai_key_owner_seat;
            var isMustChuPai = playingData.must_chupai == 1;
            var show = currentChuPaiSeat == mySeat;

            ShowChuPaiGroup(show, !isMustChuPai);
        }

        public float ChuPaiGroupShowAnimationTime = 0.2f;

        public float ChuPaiGroupHideAnimationTime = 0.1f;

        private Tweener _chuPaiGroupTweener;

        private Tweener _buChuGroupTweener;

        public void ShowChuPaiGroup(bool show, bool withBuChu = true)
        {
            if (_chuPaiGroupTweener != null)
            {
                _chuPaiGroupTweener.Kill();
                _chuPaiGroupTweener = null;
            }

            if (_buChuGroupTweener != null)
            {
                _buChuGroupTweener.Kill();
                _buChuGroupTweener = null;
            }

            if (ChuPaiGroupTransform)
            {
                if (show)
                {
                    _soundController.PlayPleaseChuPaiSound();

                    if (!ChuPaiGroupTransform.gameObject.activeSelf)
                    {
                        ChuPaiGroupTransform.gameObject.SetActive(true);
                    }

                    ChuPaiGroupTransform.localScale = new Vector3(0, 0, 1);

                    _chuPaiGroupTweener = ChuPaiGroupTransform
                        .DOScale(new Vector3(1, 1, 1), ChuPaiGroupShowAnimationTime)
                        .SetEase(Ease.OutBack, 1.1f);
                }
                else
                {
                    if (ChuPaiGroupTransform.gameObject.activeSelf)
                    {
                        _chuPaiGroupTweener = ChuPaiGroupTransform
                            .DOScale(new Vector3(0, 0, 1), ChuPaiGroupHideAnimationTime)
                            .SetEase(Ease.Linear)
                            .OnComplete(() => ChuPaiGroupTransform.gameObject.SetActive(false));
                    }
                }
            }

            if (!show)
            {
                withBuChu = false;
            }

            if (BuChuGroupTransform)
            {
                if (withBuChu)
                {
                    if (!BuChuGroupTransform.gameObject.activeSelf)
                        BuChuGroupTransform.gameObject.SetActive(true);

                    BuChuGroupTransform.localScale = new Vector3(0, 0, 1);
                    _buChuGroupTweener = BuChuGroupTransform
                        .DOScale(new Vector3(1, 1, 1), ChuPaiGroupShowAnimationTime)
                        .SetEase(Ease.OutBack, 1.1f);
                }
                else
                {
                    if (BuChuGroupTransform.gameObject.activeSelf)
                    {
                        _buChuGroupTweener = BuChuGroupTransform
                            .DOScale(new Vector3(0, 0, 1), ChuPaiGroupHideAnimationTime)
                            .SetEase(Ease.Linear)
                            .OnComplete(() => BuChuGroupTransform.gameObject.SetActive(false));
                    }
                }
            }
        }
    }
}