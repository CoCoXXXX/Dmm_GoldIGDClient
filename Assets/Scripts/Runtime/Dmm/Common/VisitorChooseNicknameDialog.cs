using System;
using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dmm.Common
{
    public class VisitorChooseNicknameDialog : MyDialog
    {
        public InputField Nickname;

        public void RandomOtherNickname()
        {
            string noun = null;
            string adj = null;

            Random.InitState((int) DateTime.Now.ToFileTime());

            if (_nounList.Count > 0)
            {
                var nounIdx = Random.Range(0, _nounList.Count);
                noun = _nounList[nounIdx];
            }

            if (_adjectiveList.Count > 0)
            {
                var adjIdx = Random.Range(0, _adjectiveList.Count);
                adj = _adjectiveList[adjIdx];
            }

            Nickname.text = string.Format("{0}的{1}", adj, noun);
        }

        public Toggle MaleToggle;

        public Toggle FemaleToggle;

        public GameObject SexGroup;

        public TextAsset Noun;

        public TextAsset Adjective;

        private readonly List<string> _nounList = new List<string>();

        private readonly List<string> _adjectiveList = new List<string>();

        #region Container

        private IDataContainer<ChooseNicknameResult> _chooseNicknameResultContainer;

        private IDataContainer<FeatureSwitch> _featureSwitchContainer;

        private IDataContainer<User> _userContainer;

        #endregion

        private void OnEnable()
        {
            _chooseNicknameResultContainer =
                GetDataRepository().GetContainer<ChooseNicknameResult>(DataKey.ChooseNicknameResult);
            _featureSwitchContainer = GetDataRepository().GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);
            _userContainer = GetDataRepository().GetContainer<User>(DataKey.MyUser);
        }

        public override void BeforeShow()
        {
            // 初始化昵称。
            _nounList.Clear();
            if (!string.IsNullOrEmpty(Noun.text))
            {
                var nouns = Noun.text.Split('\n');
                if (nouns.Length > 0)
                {
                    for (int i = 0; i < nouns.Length; i++)
                    {
                        var noun = nouns[i];
                        _nounList.Add(noun.Trim());
                    }
                }
            }

            _adjectiveList.Clear();
            if (!string.IsNullOrEmpty(Adjective.text))
            {
                var adjs = Adjective.text.Split('\n');
                if (adjs.Length > 0)
                {
                    for (int i = 0; i < adjs.Length; i++)
                    {
                        var adj = adjs[i];
                        _adjectiveList.Add(adj.Trim());
                    }
                }
            }

            RandomOtherNickname();

            var user = _userContainer.Read();
            var featureSwitch = _featureSwitchContainer.Read();

            var isEnablePersonalInfo = false;
            var userSex = 1;
            if (featureSwitch != null)
            {
                isEnablePersonalInfo = featureSwitch.personal_info;
            }

            if (user != null)
            {
                userSex = user.sex;
            }

            // 初始化性别。
            var personalInfo = isEnablePersonalInfo;
            if (personalInfo)
            {
                int sex = userSex;
                MaleToggle.isOn = sex == 1;
                FemaleToggle.isOn = sex == 0;
            }

            if (SexGroup.activeSelf != personalInfo)
            {
                SexGroup.SetActive(personalInfo);
            }
        }

        public override void AfterHide()
        {
            _nounList.Clear();
            _adjectiveList.Clear();

            Destroy(gameObject);
        }

        public void ConfirmChooseNickname()
        {
            var user = _userContainer.Read();
            var featureSwitch = _featureSwitchContainer.Read();
            var isEnablePersonalIfo = false;
            var userSex = 1;
            if (user != null)
            {
                userSex = user.sex;
            }

            if (featureSwitch != null)
            {
                isEnablePersonalIfo = featureSwitch.personal_info;
            }

            string nickname = null;
            var sex = userSex;

            nickname = Nickname.text;
            if (isEnablePersonalIfo)
            {
                // 只有在开启personalInfo的情况下，才读取玩家选择的性别。
                if (MaleToggle.isOn) sex = 1;
                if (FemaleToggle.isOn) sex = 0;
            }

            var dialogManager = GetDialogManager();
            var remoteAPI = GetRemoteAPI();
            var taskManager = GetTaskManager();
            dialogManager.ShowWaitingDialog(true);

            _chooseNicknameResultContainer.ClearAndInvalidate(Time.time);
            remoteAPI.VisitorChooseNickname(nickname, sex);

            taskManager.ExecuteTask(
                CheckVisitorChooseNicknameResult,
                () => dialogManager.ShowWaitingDialog(false));
        }

        private bool CheckVisitorChooseNicknameResult()
        {
            var res = _chooseNicknameResultContainer.Read();
            if (res == null)
            {
                return false;
            }

            var dialogManager = GetDialogManager();
            var remoteAPI = GetRemoteAPI();
            dialogManager.ShowWaitingDialog(false);

            if (res.result == ResultCode.OK)
            {
                remoteAPI.RequestUserInfo();
                dialogManager.ShowToast("成功编辑昵称！", 2);

                Hide();
            }
            else
            {
                dialogManager.ShowToast("编辑昵称失败，请重试！", 2, true);
            }

            return true;
        }
    }
}