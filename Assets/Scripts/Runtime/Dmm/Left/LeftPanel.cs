using UnityEngine;

namespace Dmm.Left
{
    public class LeftPanel : MonoBehaviour
    {
        public InfoPanel InfoPanel;

        public RectTransform FriendList;

        public GameObject InfoTag;

        public GameObject FriendTag;

        public void OnEnable()
        {
            SwitchTo(LeftPanelType.Info);
        }

        public void SwitchToInfo()
        {
            SwitchTo(LeftPanelType.Info);
        }

        public void SwitchToFriend()
        {
            SwitchTo(LeftPanelType.Friend);
        }

        public void SwitchTo(LeftPanelType type)
        {
            var info = type == LeftPanelType.Info;
            if (InfoPanel.gameObject.activeSelf != info)
            {
                InfoPanel.gameObject.SetActive(info);
            }
            if (InfoTag.activeSelf != info)
            {
                InfoTag.SetActive(info);
            }

            var friend = type == LeftPanelType.Friend;
            if (FriendList.gameObject.activeSelf != friend)
            {
                FriendList.gameObject.SetActive(friend);
            }
            if (FriendTag.activeSelf != friend)
            {
                FriendTag.SetActive(friend);
            }
        }
    }

    public enum LeftPanelType
    {
        /// <summary>
        /// 玩家信息。
        /// </summary>
        Info,

        /// <summary>
        /// 好友面板。
        /// </summary>
        Friend,

        Function
    }
}