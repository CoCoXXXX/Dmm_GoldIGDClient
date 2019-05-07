using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Left
{
    public class FriendRequestDialog : MyDialog
    {
        public Text RequestContent;

        public Toggle IgnoreToggle;

        private string _username;

        private string _nickname;

        public string RequestUsername
        {
            get { return _username; }
        }

        private IDataContainer<List<string>> _friendRequesterList;

        private IDataContainer<List<string>> _friendIgnoreList;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        private void OnEnable()
        {
            var dataRepository = GetDataRepository();

            _friendRequesterList = dataRepository.GetContainer<List<string>>(DataKey.FriendRequesterList);
            _friendIgnoreList = dataRepository.GetContainer<List<string>>(DataKey.FriendIgnoreList);
            _featureSwitch = dataRepository.GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
        }

        public void ApplyData(string username, string nickname, int sex)
        {
            _username = username;
            _nickname = nickname;

            var friendRequesterList = _friendRequesterList.Read();
            var friendIgnoreList = _friendIgnoreList.Read();

            if (friendRequesterList == null)
            {
                friendRequesterList = new List<string>();
            }

            if (friendIgnoreList == null)
            {
                friendIgnoreList = new List<string>();
            }

            var featureSwitch = _featureSwitch.Read();

            if (!friendRequesterList.Contains(username))
            {
                friendRequesterList.Add(username);
            }

            _friendRequesterList.Write(friendRequesterList, Time.time);

            if (IgnoreToggle)
            {
                IgnoreToggle.isOn =
                    string.IsNullOrEmpty(username) ||
                    friendIgnoreList.Contains(username);
            }

            if (RequestContent)
            {
                var sexStr = "";
                var isEnablePersonalInfo = featureSwitch != null && featureSwitch.personal_info;
                if (isEnablePersonalInfo)
                {
                    sexStr = sex == 1 ? "(男)" : "(女)";
                }

                RequestContent.text = string.Format("{0}{1}\n想添加您为好友", _nickname, sexStr);
            }
        }

        public void Accept()
        {
            var remoteAPI = GetRemoteAPI();
            remoteAPI.AddFriendResponse(true, _username);
            Hide();
        }

        public void Decline()
        {
            var remoteAPI = GetRemoteAPI();
            remoteAPI.AddFriendResponse(false, _username);
            Hide();
        }

        public void OnIngnoreChanged()
        {
            var ignored = IgnoreToggle && IgnoreToggle.isOn;
            var friendIgnoreList = _friendIgnoreList.Read();
            if (friendIgnoreList == null)
            {
                friendIgnoreList = new List<string>();
            }

            if (ignored)
            {
                if (!string.IsNullOrEmpty(_username) &&
                    !friendIgnoreList.Contains(_username))
                {
                    friendIgnoreList.Add(_username);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_username) && friendIgnoreList.Contains(_username))
                {
                    friendIgnoreList.Remove(_username);
                }
            }

            _friendIgnoreList.Write(friendIgnoreList, Time.time);
        }

        public override void AfterHide()
        {
            var friendRequesterList = _friendRequesterList.Read();

            if (friendRequesterList == null)
            {
                friendRequesterList = new List<string>();
            }

            if (!string.IsNullOrEmpty(_username) && friendRequesterList.Contains(_username))
            {
                friendRequesterList.Remove(_username);
            }

            _friendRequesterList.Write(friendRequesterList, Time.time);

            Destroy(gameObject);
        }
    }
}