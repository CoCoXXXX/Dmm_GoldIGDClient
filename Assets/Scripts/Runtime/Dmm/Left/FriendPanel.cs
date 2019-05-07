using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Left
{
    public class FriendPanel : MonoBehaviour
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private IAnalyticManager _analyticManager;

        private IDataContainer<SFriendListResult> _sFriendListResult;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            IAnalyticManager analyticManager,
            RemoteAPI remoteAPI)
        {
            _dialogManager = dialogManager;
            _analyticManager = analyticManager;
            _remoteAPI = remoteAPI;
            _sFriendListResult = dataRepository.GetContainer<SFriendListResult>(DataKey.SFriendListResult);
        }

        #endregion

        public float FriendListRefreshInterval = 60;

        public void OnEnable()
        {
            InitFriendList();
        }

        public void InitFriendList()
        {
            if (_sFriendListResult.Read() == null ||
                Time.time - _sFriendListResult.Timestamp >= FriendListRefreshInterval)
                // 目前没有好友列表数据，或者刷新间隔不足60秒的时候，就开始刷新。
            {
                _remoteAPI.RefreshFriendList();
            }
        }

        public float RotateSpeed = 360;

        public Image WaitingImg;

        public Text TipText;

        private float _refreshTime;

        public void Update()
        {
            if (_sFriendListResult.Timestamp <= 0 &&
                _sFriendListResult.Read() == null)
            {
                if (TipText.gameObject.activeSelf)
                {
                    TipText.gameObject.SetActive(false);
                }

                if (!WaitingImg.gameObject.activeSelf)
                {
                    WaitingImg.gameObject.SetActive(true);
                }

                var r = WaitingImg.rectTransform.rotation.eulerAngles;
                WaitingImg.rectTransform.rotation = Quaternion.Euler(0, 0, r.z - RotateSpeed * Time.deltaTime);
            }
            else
            {
                if (WaitingImg.gameObject.activeSelf)
                {
                    WaitingImg.gameObject.SetActive(false);
                }
            }

            if (_refreshTime >= _sFriendListResult.Timestamp)
            {
                return;
            }

            _refreshTime = _sFriendListResult.Timestamp;

            var res = _sFriendListResult.Read();
            if (res == null)
            {
                if (!TipText.gameObject.activeSelf)
                {
                    TipText.gameObject.SetActive(true);
                }

                TipText.text = "没有好友数据";
                return;
            }

            if (res.result.code == ResultCode.OK)
            {
                if (FriendCount() <= 0)
                {
                    if (!TipText.gameObject.activeSelf)
                    {
                        TipText.gameObject.SetActive(true);
                    }

                    TipText.text = "没有好友数据";
                }
                else
                {
                    if (TipText.gameObject.activeSelf)
                    {
                        TipText.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (!TipText.gameObject.activeSelf)
                {
                    TipText.gameObject.SetActive(true);
                }

                TipText.text = "刷新失败";
            }
        }

        public void RefreshFriendList()
        {
            _sFriendListResult.ClearAndInvalidate(Time.time);
            _remoteAPI.RefreshFriendList();

            _analyticManager.Event("refresh_friend_list_manually");
        }

        public void FindFriend()
        {
            _dialogManager.ShowDialog<FindFriendDialog>(DialogName.FindFriendDialog);
        }

        private List<FriendInfo> GetFriendList()
        {
            var friendListResult = _sFriendListResult.Read();
            if (friendListResult == null || friendListResult.result.code != ResultCode.OK)
            {
                return null;
            }

            return friendListResult.info;
        }

        private int FriendCount()
        {
            var list = GetFriendList();
            if (list == null)
            {
                return 0;
            }

            return list.Count;
        }
    }
}