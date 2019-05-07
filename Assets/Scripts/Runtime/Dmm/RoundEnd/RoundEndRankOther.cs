using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Report;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.RoundEnd
{
    public class RoundEndRankOther : MonoBehaviour
    {
        #region Inject

        private IDialogManager _dialogManager;

        private RemoteAPI _remoteAPI;

        private SpriteHolder _spriteHolder;

        private IDataContainer<TableUserData> _tableUser;

        private IDataContainer<AppState> _appState;

        private IAppController _appController;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            IAppController appController,
            RemoteAPI remoteAPI,
            SpriteHolder spriteHolder)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _spriteHolder = spriteHolder;
            _appController = appController;

            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);

            _appState = dataRepository.GetContainer<AppState>(DataKey.AppState);
        }

        public class Factory : Factory<RoundEndRankOther>
        {
        }

        #endregion

        private string _username;

        private string _nickName;

        public Image WinIcon;

        public NicknameGroup Nickname;

        public GameObject BtnGroup;

        public GameObject LeaveTag;

        private void Update()
        {
            var tableUser = _tableUser.Read();
            var inTable = tableUser.InTable(_username);
            if (LeaveTag.activeSelf == inTable)
            {
                LeaveTag.SetActive(!inTable);
            }
        }

        public void Reset()
        {
            _username = null;

            if (WinIcon.gameObject.activeSelf)
            {
                WinIcon.gameObject.SetActive(false);
            }

            Nickname.Clear();

            if (BtnGroup.activeSelf)
            {
                BtnGroup.SetActive(false);
            }

            if (LeaveTag.activeSelf)
            {
                LeaveTag.SetActive(false);
            }
        }

        public void ApplyData(bool win, string username, string nickname, int vip)
        {
            if (!WinIcon.gameObject.activeSelf)
            {
                WinIcon.gameObject.SetActive(true);
            }

            WinIcon.sprite = win ? _spriteHolder.WinIcon : _spriteHolder.LoseIcon;

            if (!Nickname.gameObject.activeSelf)
            {
                Nickname.gameObject.SetActive(true);
            }

            _username = username;
            _nickName = nickname;
            Nickname.SetData(nickname, vip);

            EnableBtnGroup(false);
        }

        private void EnableBtnGroup(bool show)
        {
            if (BtnGroup.gameObject.activeSelf != show)
            {
                BtnGroup.gameObject.SetActive(show);
            }
        }

        public void ShowBtnGroup()
        {
            var singleGame = _appController.IsSingleGameMode();
            if (singleGame)
            {
                return;
            }

            EnableBtnGroup(true);
        }

        public void Kickout()
        {
            if (string.IsNullOrEmpty(_username))
            {
                EnableBtnGroup(false);
                return;
            }

            var tableUser = _tableUser.Read();
            var seat = tableUser.GetSeatOfUser(_username);
            if (seat == -1)
            {
                EnableBtnGroup(false);
                _dialogManager.ShowToast("对方已经离开座位", 2);
                return;
            }

            if (_appState.Read() == AppState.Playing)
            {
                EnableBtnGroup(false);
                _dialogManager.ShowToast("牌局中不能踢人哦", 2);
                return;
            }

            _remoteAPI.KickOut(seat);
            EnableBtnGroup(false);
        }

        public void Report()
        {
            var badUser = new User();
            badUser.nickname = _nickName;
            badUser.username = _username;

            _dialogManager.ShowDialog<ReportDialog>(DialogName.ReportDialog, false, true,
                (dialog) =>
                {
                    dialog.ApplyData(badUser);
                    dialog.Show();
                });
        }
    }
}