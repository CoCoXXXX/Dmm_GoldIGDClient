using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Checkin
{
    /// <summary>
    /// 签到对话框。
    /// </summary>
    public class CheckinDialog : MyDialog
    {
        public Text DateInfo;

        public Text DetailInfo;

        public Text CheckinDays;

        public Button CheckinBtn;

        public Button RecheckinBtn;

        public Calendar Calendar;

        public CheckinConditionList CheckinConditionList;

        #region DataContainer

        private IDataContainer<User> _myUser;

        private IDataContainer<CheckinConfigResult> _checkinConfigResult;

        private IDataContainer<CheckinConfig> _checkinConfig;

        private IDataContainer<ReCheckinResult> _reCheckinResult;

        private IDataContainer<CheckinResult> _checkinResult;

        #endregion

        public void Update()
        {
            if (Status == StatusShow)
            {
                RefreshContent();
            }
        }

        private void OnEnable()
        {
            var dataRepository = GetDataRepository();

            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
            _checkinConfigResult =
                dataRepository.GetContainer<CheckinConfigResult>(DataKey.CheckinConfigResult);
            _checkinConfig =
                dataRepository.GetContainer<CheckinConfig>(DataKey.CheckinConfig);
            _reCheckinResult = dataRepository.GetContainer<ReCheckinResult>(DataKey.ReCheckinResult);
            _checkinResult = dataRepository.GetContainer<CheckinResult>(DataKey.CheckinResult);
        }

        public override void BeforeShow()
        {
            RefreshContent();
        }

        private float ContentRefreshTime { get; set; }

        private void RefreshContent()
        {
            var max = Mathf.Max(_checkinConfigResult.Timestamp, _myUser.Timestamp);
            if (ContentRefreshTime >= max)
            {
                return;
            }

            ContentRefreshTime = max;

            var config = _checkinConfig.Read();
            if (config == null)
            {
                Reset();
                return;
            }

            if (DateInfo)
            {
                DateInfo.text = string.Format(
                    "{0}年{1}月{2}日",
                    config.current_year,
                    config.current_month,
                    config.current_day);
            }

            if (DetailInfo)
            {
                var user = _myUser.Read();
                DetailInfo.text = string.Format(
                    "共有补签卡<color=red>{0}</color>张，当月需补签<color=red>{1}</color>天",
                    user != null ? user.recheckin_card : 0,
                    config.need_recheckin_days);
            }

            if (CheckinDays)
            {
                CheckinDays.text = string.Format(
                    "您已连续签到<color=red>{0}</color>天",
                    config.continue_checkin_days);
            }

            // 判断今天的签到状态。
            // 如果今天未签到，则显示签到按钮。
            // 如果今天已经签到：
            // 1、存在需要补签的天数，则显示补签按钮。
            // 2、不存在需要补签的天数，则显示disabled的签到按钮。

            var checkinItems = config.checkin_item;
            var todayChecked = false;
            if (checkinItems != null)
            {
                for (int i = 0; i < checkinItems.Count; i++)
                {
                    var c = checkinItems[i];
                    if (config.current_day == c.day)
                    {
                        todayChecked = c.status == CheckinStatus.Checked;
                        break;
                    }
                }
            }

            if (!todayChecked)
            {
                if (!CheckinBtn.gameObject.activeSelf)
                {
                    CheckinBtn.gameObject.SetActive(true);
                }

                if (!CheckinBtn.interactable)
                {
                    CheckinBtn.interactable = true;
                }

                if (RecheckinBtn.gameObject.activeSelf)
                {
                    RecheckinBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                var recheckin = config.need_recheckin_days > 0;

                if (RecheckinBtn.gameObject.activeSelf != recheckin)
                {
                    RecheckinBtn.gameObject.SetActive(recheckin);
                }

                if (CheckinBtn.gameObject.activeSelf == recheckin)
                {
                    CheckinBtn.gameObject.SetActive(!recheckin);
                }

                if (CheckinBtn.interactable != recheckin)
                {
                    CheckinBtn.interactable = recheckin;
                }
            }
        }

        private void Reset()
        {
            DateInfo.text = "";
            DetailInfo.text = "";
            CheckinDays.text = "";
        }

        public void DoCheckin()
        {
            var remoteAPI = GetRemoteAPI();
            var taskManager = GetTaskManager();
            var dialogManager = GetDialogManager();

            dialogManager.ShowWaitingDialog(true);
            _checkinResult.ClearNotInvalidate();
            remoteAPI.Checkin();
            taskManager.ExecuteTask(CheckCheckinResult, () => dialogManager.ShowWaitingDialog(false));
        }

        private bool CheckCheckinResult()
        {
            var data = _checkinResult.Read();
            if (data == null)
            {
                return false;
            }

            var res = data.res;
            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);

            if (res.code != ResultCode.OK)
            {
                if (!string.IsNullOrEmpty(data.res.msg))
                {
                    dialogManager.ShowToast(data.res.msg, 3, true);
                }
            }

            return true;
        }

        public void DoRecheckin()
        {
            var dialogManager = GetDialogManager();
            var remoteAPI = GetRemoteAPI();
            var taskManager = GetTaskManager();

            dialogManager.ShowWaitingDialog(true);

            _reCheckinResult.ClearAndInvalidate(Time.time);
            remoteAPI.ReCheckin();

            taskManager.ExecuteTask(CheckReCheckinResult, () => dialogManager.ShowWaitingDialog(false));
        }

        private bool CheckReCheckinResult()
        {
            var data = _reCheckinResult.Read();
            if (data == null)
            {
                return false;
            }

            var dialogManager = GetDialogManager();
            dialogManager.ShowWaitingDialog(false);

            var res = data.res;
            if (res.code != ResultCode.OK)
            {
                if (!string.IsNullOrEmpty(res.msg))
                {
                    dialogManager.ShowToast(res.msg, 3, true);
                }
            }

            return true;
        }

        public string HelpInfo;

        public void ShowHelpInfo()
        {
            // 显示一个签到帮助信息的对话框。
            var dialogManager = GetDialogManager();
            dialogManager.ShowConfirmBox(
                HelpInfo,
                false, null, null,
                false, null, null,
                false, false,
                true
            );
        }

        public override void AfterHide()
        {
            Destroy(gameObject);
        }
    }
}