using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.Common;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Res;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Left
{
    public class FriendItem : Item<FriendInfo>
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        [Inject]
        public void Initialize(
            IDialogManager dialogManager,
            RemoteAPI remoteAPI)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
        }

        public class Factory : Factory<FriendItem>
        {
        }

        #endregion

        public LayoutElement Layout;

        public Vector2 NormalSize;

        public Vector2 ExpandSize;

        public float ExpandTime = 0.2f;

        private Tweener _expandTweener;

        public Button Button;

        public AsyncImage HeadIcon;

        public Text Nickname;

        public Sprite OnlineSprite;

        public Sprite OfflineSprite;

        public Image OnlineState;

        public RectTransform FunctionGroup;

        private FriendInfo _data;

        private bool _selected;

        public override FriendInfo GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, FriendInfo data)
        {
            _data = data;

            if (data == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(data.friend_head_icon))
            {
                HeadIcon.gameObject.SetActive(false);
            }
            else
            {
                HeadIcon.gameObject.SetActive(true);
                HeadIcon.SetTargetPic(GetHeadIconName(data.friend_head_icon), null,
                    data.friend_head_icon);
            }

            if (Nickname)
            {
                if (!Nickname.gameObject.activeSelf)
                {
                    Nickname.gameObject.SetActive(true);
                }

                Nickname.text = data.nickname;
            }

            if (OnlineState)
            {
                if (!OnlineState.gameObject.activeSelf)
                {
                    OnlineState.gameObject.SetActive(true);
                }

                switch (data.online_state)
                {
                    case com.morln.game.gd.command.OnlineState.Offline:
                        OnlineState.sprite = OfflineSprite;
                        break;

                    case com.morln.game.gd.command.OnlineState.Online:
                    case com.morln.game.gd.command.OnlineState.Playing:
                        OnlineState.sprite = OnlineSprite;
                        break;
                }
            }

            if (FunctionGroup && FunctionGroup.gameObject.activeSelf)
            {
                FunctionGroup.gameObject.SetActive(false);
            }

            _selected = false;

            if (Layout)
            {
                Layout.preferredWidth = NormalSize.x;
                Layout.preferredHeight = NormalSize.y;
            }
        }

        private string GetHeadIconName(string headIconUrl)
        {
            if (string.IsNullOrEmpty(headIconUrl))
            {
                return "default";
            }
            return string.Format("headicon-{0}", WWW.EscapeURL(headIconUrl));
        }

        public override void Reset(int currentIndex)
        {
            HeadIcon.Reset();

            if (Nickname && Nickname.gameObject.activeSelf)
            {
                Nickname.gameObject.SetActive(false);
            }

            if (OnlineState && OnlineState.gameObject.activeSelf)
            {
                OnlineState.gameObject.SetActive(false);
            }

            if (FunctionGroup && FunctionGroup.gameObject.activeSelf)
            {
                FunctionGroup.gameObject.SetActive(false);
            }

            if (Layout)
            {
                Layout.preferredWidth = NormalSize.x;
                Layout.preferredHeight = NormalSize.y;
            }

            if (_expandTweener != null)
            {
                _expandTweener.Kill();
                _expandTweener = null;
            }

            _selected = false;
        }

        public override void Select(bool selected)
        {
            if (selected != _selected)
            {
                _selected = selected;

                if (_expandTweener != null)
                {
                    _expandTweener.Kill();
                    _expandTweener = null;
                }

                if (Layout)
                {
                    if (selected)
                    {
                        _expandTweener = Layout
                            .DOPreferredSize(ExpandSize, ExpandTime)
                            .OnComplete(() =>
                            {
                                if (FunctionGroup && !FunctionGroup.gameObject.activeSelf)
                                    FunctionGroup.gameObject.SetActive(true);
                            });
                    }
                    else
                    {
                        if (FunctionGroup && FunctionGroup.gameObject.activeSelf)
                            FunctionGroup.gameObject.SetActive(false);

                        _expandTweener = Layout
                            .DOPreferredSize(NormalSize, ExpandTime);
                    }
                }
            }
        }

        public override Button GetClickButton()
        {
            return Button;
        }

        public void OnClickPlayerInfoBtn()
        {
            if (_data != null)
            {
                _remoteAPI.RequestFriendDetail(_data.username);
            }
        }

        public void OnClickDeleteBtn()
        {
            if (_data != null)
            {
                _dialogManager.ShowConfirmBox(
                    string.Format("确认删除好友<color=green>{0}</color>？", _data.nickname),
                    true, "删除", () => { _remoteAPI.RemoveFriend(_data.username); },
                    true, "取消", null, false, false, true
                );
            }
        }

        public void OnClickTraceBtn()
        {
            _remoteAPI.TraceUser(_data.username, false);
        }
    }
}