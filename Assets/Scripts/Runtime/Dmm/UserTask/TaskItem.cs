using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Task;
using Dmm.Util;
using Dmm.WeChat;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.UserTask
{
    public class TaskItem : Item<UserTaskState>
    {
        #region Inject

        private IDialogManager _dialogManager;

        private RemoteAPI _remoteApi;

        private IWeChatManager _weChatManager;

        private ITaskManager _taskManager;

        private IDataContainer<GetUserTaskAwardResult> _getUserTaskAwardResult;

        private IDataContainer<User> _user;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteApi,
            ITaskManager taskManager,
            IWeChatManager weChatManager)
        {
            _dialogManager = dialogManager;
            _remoteApi = remoteApi;
            _weChatManager = weChatManager;
            _taskManager = taskManager;
            _getUserTaskAwardResult =
                dataRepository.GetContainer<GetUserTaskAwardResult>(DataKey.GetUserTaskAwardResult);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        public class Factory : Factory<TaskItem>
        {
        }

        #endregion

        private UserTaskState _data;

        public AsyncImage AwardIcon;

        public Text AwardCounText;

        public Text DescriptionText;

        public Slider TaskProcess;

        public Text ProgressTxt;

        public Button GoToBtn;

        public Button GetAwardBtn;

        public GameObject HasGet;

        public UserTaskDialog UserTaskDialog;

        public override UserTaskState GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, UserTaskState data)
        {
            _data = data;
            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            AwardIcon.SetTargetPic(data.pic, null, data.pic_url);

            AwardCounText.text = data.display_currency_amount + "";

            DescriptionText.text = _data.description;

            if (data.current_progress < 0 || data.total_progress <= 0)
            {
                Reset(currentIndex);
                return;
            }

            float progress = (float) data.current_progress / data.total_progress;
            TaskProcess.value = progress;

            ProgressTxt.text = data.current_progress >= data.total_progress
                ? "已完成"
                : data.current_progress + "/" + data.total_progress;

            GoToBtn.gameObject.SetActive(false);
            GetAwardBtn.gameObject.SetActive(false);
            HasGet.SetActive(false);

            if (progress >= 0 && progress < 1)
            {
                GoToBtn.gameObject.SetActive(true);
            }
            else
            {
                if (data.award_posted)
                {
                    HasGet.SetActive(true);
                }
                else
                {
                    GetAwardBtn.gameObject.SetActive(true);
                }
            }
        }

        public override void Reset(int currentIndex)
        {
            AwardIcon.Reset();
            AwardCounText.text = "";
            DescriptionText.text = "";
            TaskProcess.value = 0;
            GoToBtn.gameObject.SetActive(false);
            GetAwardBtn.gameObject.SetActive(false);
            HasGet.SetActive(false);
            ProgressTxt.text = "";
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return null;
        }

        /// <summary>
        /// 去完成
        /// </summary>
        public void GoToTask()
        {
            if (_data == null)
            {
                return;
            }

            var action = _data.extra_action;
            var taskCode = _data.task_code;
            if (action == null)
            {
                return;
            }

            UserTaskDialog.Hide();

            switch (action.type)
            {
                case ExtraActionType.SHOW_DIALOG:
                    _dialogManager.ShowDialog<UIWindow>(action.dialog_name);
                    break;
                case ExtraActionType.AWARD:
                    _remoteApi.RequestAward(action.award_type, action.award_code);
                    break;
                case ExtraActionType.GOTO_ROOM:
                    _remoteApi.ChooseRoom((int) action.room_id);
                    break;
                case ExtraActionType.WE_CHAT_SHARE:
                    var shareContent = new ShareContent(ShareResultType.TaskCode, taskCode);
                    var content = JsonUtility.ToJson(shareContent);
                    if (action.wx_share_type == WxShareType.WxShare)
                    {
                        _weChatManager.WxShare(action.wx_url, action.wx_img_url, null, action.wx_title,
                            action.wx_content,
                            action.wx_thumb_url, content);
                    }
                    else if (action.wx_share_type == WxShareType.WxCircle)
                    {
                        _weChatManager.WxCircle(action.wx_url, action.wx_img_url, null, action.wx_title,
                            action.wx_content,
                            action.wx_thumb_url, content);
                    }

                    break;
                default: break;
            }
        }

        /// <summary>
        /// 领取奖励
        /// </summary>
        public void GetAward()
        {
            if (_data == null)
            {
                _dialogManager.ShowToast("数据发生错误无法领奖", 2, true);
                return;
            }

            _dialogManager.ShowWaitingDialog(true);
            _getUserTaskAwardResult.ClearNotInvalidate();
            _remoteApi.GetUserTaskAward(_data.task_code);
            _taskManager.ExecuteTask(CheckGetUserTaskAwardResult, () => _dialogManager.ShowWaitingDialog(false), 5f);
        }

        private bool CheckGetUserTaskAwardResult()
        {
            var data = _getUserTaskAwardResult.Read(true);
            if (data == null)
            {
                return false;
            }

            _dialogManager.ShowWaitingDialog(false);

            var res = data.res;
            if (data.res == null)
            {
                return true;
            }

            if (res.code == ResultCode.OK)
            {
                var myUser = _user.Read();
                var msgUser = data.user;
                // 将桌子中的玩家数据更新到我的数据中。
                if (DataUtil.UpdateUserPublic(msgUser, myUser))
                {
                    _user.Invalidate(Time.time);
                }

                _dialogManager.ShowDialog<GetAwardDialog>(DialogName.GetAwardDialog, false, true,
                    (dialog) =>
                    {
                        dialog.ApplyData(data.description, AwardType.UserTask, data.currency);
                        dialog.Show();
                    });

                _remoteApi.RequestUserTaskList();
            }
            else
            {
                if (string.IsNullOrEmpty(res.msg))
                {
                    _dialogManager.ShowToast(data.res.msg, 3, true);
                }
                else
                {
                    _dialogManager.ShowToast("领取奖励失败", 3, true);
                }
            }

            return true;
        }

        public void OnAwardItemClick()
        {
            _dialogManager.ShowDialog<ItemDetailDialog>(DialogName.ItemDetailDialog, false, true,
                (dialog) =>
                {
                    dialog.ApplyData(_data, this);
                    dialog.Show();
                });
        }
    }
}