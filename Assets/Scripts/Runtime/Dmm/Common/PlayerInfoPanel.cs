using System;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class PlayerInfoPanel : MyDialog
    {
        public Text Username;

        public Text Nickname;

        public Slider ExpSlider;

        public Text CurLevel;

        public Text ExpCeilFloor;

        public Text Sex;

        public GameObject SexGroup;

        #region Container

        private IDataContainer<User> _user;

        private IDataContainer<EditUserInfoResult> _editUserInfoResult;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        #endregion

        public void CityChanged()
        {
            _cityChanged = true;
        }

        private bool _cityChanged = false;

        public InputField Email;

        public void EmailChanged()
        {
            _emailChanged = true;
        }

        private bool _emailChanged = false;

        public InputField Description;

        public void DescriptionChanged()
        {
            _descriptionChanged = true;
        }

        private bool _descriptionChanged = false;

        public Button SaveButton;

        private void OnEnable()
        {
            var dataRepository = GetDataRepository();
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _editUserInfoResult = dataRepository.GetContainer<EditUserInfoResult>(DataKey.EditUserInfoResult);
            _featureSwitch = dataRepository.GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
        }

        public void Update()
        {
            RefreshContent();
        }

        private float _refreshTime;

        private void RefreshContent()
        {
            if (_refreshTime >= _user.Timestamp)
            {
                return;
            }

            _refreshTime = _user.Timestamp;

            var user = _user.Read();
            if (user == null)
            {
                Clear();
                return;
            }

            Username.text = user.username;
            Nickname.text = user.nickname;

            ExpSlider.minValue = user.exp_floor;
            ExpSlider.maxValue = user.exp_ceil;
            ExpSlider.value = user.exp;

            ExpCeilFloor.text = string.Format("{0}/{1}", user.exp - user.exp_floor, user.exp_ceil - user.exp_floor);
            CurLevel.text = string.Format("{0}:{1}", user.level, LevelTitle.TitleOf(user.level));
            Sex.text = user.sex == 1 ? "男" : "女";

            var featureSwitch = _featureSwitch.Read();
            var isEnablePersonalInfo = featureSwitch != null && featureSwitch.personal_info;
            if (SexGroup.activeSelf != isEnablePersonalInfo)
            {
                SexGroup.SetActive(isEnablePersonalInfo);
            }

            Email.text = user.email;

            ResetChangeFlag();
        }

        public void Clear()
        {
            Username.text = "";
            Nickname.text = "";
            ExpSlider.value = 0;
            ExpCeilFloor.text = "";
            CurLevel.text = "";
            Sex.text = "";
            Email.text = "";
        }

        private void ResetChangeFlag()
        {
            _cityChanged = false;
            _emailChanged = false;
            _descriptionChanged = false;
        }

        private bool HasChange()
        {
            return _cityChanged || _emailChanged || _descriptionChanged;
        }

        public void CopyUserName()
        {
            var clipboradManager = GetClipboardManager();
            var user = _user.Read();
            var dialogManager = GetDialogManager();
            if (user == null)
            {
                dialogManager.ShowToast("复制账号失败", 2, true);
                return;
            }
            var userName = user.username;

            if (string.IsNullOrEmpty(userName))
            {
                dialogManager.ShowToast("复制账号失败", 2, true);
                return;
            }
            try
            {
                clipboradManager.CopyToClipboard(userName);
            }
            catch (Exception e)
            {
                dialogManager.ShowToast("复制账号失败", 2, true);
                return;
            }

            dialogManager.ShowToast("已复制账号到粘贴板", 2);
        }

        public void SavePlayerInfo()
        {
            var dialogManager = GetDialogManager();
            if (!HasChange())
            {
                dialogManager.ShowToast("没有任何更改，不需要保存！", 2);
                return;
            }

            // 个性签名。
            string description = null;
            // 城市。
            string city = null;

            // 邮箱。
            string email = null;
            if (_emailChanged && Email)
            {
                email = Email.text;

                if (!string.IsNullOrEmpty(email))
                {
                    email = email.Trim();
                    if (!DataUtil.ValidateEmail(email))
                    {
                        dialogManager.ShowToast("请填写正确的Email，格式为xxx@xxx.xxx", 3);
                        return;
                    }
                }
            }

            dialogManager.ShowWaitingDialog(true);

            var remoteAPI = GetRemoteAPI();
            var taskManager = GetTaskManager();
            _editUserInfoResult.ClearAndInvalidate(Time.time);
            remoteAPI.EditUserInfo(
                // 现在不能编辑昵称了。
                null,
                // 也不支持修改性别了。
                -1,
                email,
                description,
                city
            );

            taskManager.ExecuteTask(
                CheckEditUserInfoResult,
                () => dialogManager.ShowWaitingDialog(false));

            // 不需要关闭保存按钮，如果编辑出错的话，之后还能够继续编辑。
        }

        private bool CheckEditUserInfoResult()
        {
            var res = _editUserInfoResult.Read();
            if (res == null)
            {
                return false;
            }

            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                dialogManager.ShowToast("成功编辑用户信息^_^", 2);

                var remoteAPI = GetRemoteAPI();
                remoteAPI.RequestUserInfo();
                Hide();
            }
            else
            {
                switch (res.result)
                {
                    case ResultCode.H_EDIT_SEX_ILLEGAL:
                        dialogManager.ShowToast("编辑信息失败，性别出错！", 2, true);
                        break;

                    case ResultCode.DATA_FAILED:
                        dialogManager.ShowToast("数据出错，请稍后再试！", 2, true);
                        break;
                }
            }

            _editUserInfoResult.ClearAndInvalidate(Time.time);
            return true;
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}