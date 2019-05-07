using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.UserTask
{
    public class UserTaskList : ItemList<UserTaskData>
    {
        #region Inject

        private IDataContainer<UserTaskListResult> _userTaskResult;

        private IDialogManager _dialogManager;

        private UserTaskItem.Factory _leftBtnFactory;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            UserTaskItem.Factory leftBtnFactory)
        {
            _userTaskResult = dataRepository.GetContainer<UserTaskListResult>(DataKey.UserTaskListResult);
            _dialogManager = dialogManager;
            _leftBtnFactory = leftBtnFactory;
        }

        #endregion

        /// <summary>
        /// 左边的按钮列表
        /// </summary>
        private readonly List<UserTaskData> _activityTaskLeftBtns = new List<UserTaskData>();

        private readonly List<UserTaskState> _dailyTaskList = new List<UserTaskState>();

        private readonly List<UserTaskState> _fuLiTaskList = new List<UserTaskState>();

        private readonly List<UserTaskState> _freeEggTaskList = new List<UserTaskState>();

        private readonly List<UserTaskState> _newPlayerTaskList = new List<UserTaskState>();

        public Sprite DailyTaskSprite;

        public Sprite FuLiTaskSprite;

        public Sprite FreeEggTaskSprite;

        public Sprite NewPlayerTaskSprite;

        public Sprite DailyTaskSelectedSprite;

        public Sprite FuLiTaskSelectedSprite;

        public Sprite FreeEggTaskSelectedSprite;

        public Sprite NewPlayerTaskSelectedSprite;

        public UserTaskDialog UserTaskDialog;

        public override void BeforeRefresh()
        {
            InitData();
            InitUserTaskLeftBtn();
        }

        private void InitData()
        {
            _dailyTaskList.Clear();
            _fuLiTaskList.Clear();
            _freeEggTaskList.Clear();
            _newPlayerTaskList.Clear();

            var taskResult = _userTaskResult.Read();
            if (taskResult == null)
            {
                _dialogManager.ShowToast("没有任务", 2, true);
                return;
            }

            var taskList = taskResult.user_task_state;
            if (taskList == null || taskList.Count <= 0)
            {
                _dialogManager.ShowToast("没有任务", 2, true);
                return;
            }

            for (int i = 0; i < taskList.Count; i++)
            {
                var taskItem = taskList[i];
                if (taskItem == null)
                {
                    continue;
                }

                var type = taskItem.type;
                switch (type)
                {
                    case UserTaskType.DAILY_TASK:
                        _dailyTaskList.Add(taskItem);
                        break;
                    case UserTaskType.FULI_TASK:
                        _fuLiTaskList.Add(taskItem);
                        break;
                    case UserTaskType.FREE_EGG_TASK:
                        _freeEggTaskList.Add(taskItem);
                        break;
                    case UserTaskType.NEW_PLAYER_TASK:
                        _newPlayerTaskList.Add(taskItem);
                        break;
                }
            }
        }

        private void InitUserTaskLeftBtn()
        {
            _activityTaskLeftBtns.Clear();

            if (_dailyTaskList.Count > 0)
            {
                var dailyTaskBtn = new UserTaskData
                {
                    BtnSprite = DailyTaskSprite,
                    BtnSelectedSprite = DailyTaskSelectedSprite,
                    TaskType = UserTaskType.DAILY_TASK,
                    TaskList = _dailyTaskList,
                    CanGetAward = CanGetAward(_dailyTaskList)
                };
                _activityTaskLeftBtns.Add(dailyTaskBtn);
            }

            if (_fuLiTaskList.Count > 0)
            {
                var fuliTaskBtn = new UserTaskData
                {
                    BtnSprite = FuLiTaskSprite,
                    BtnSelectedSprite = FuLiTaskSelectedSprite,
                    TaskType = UserTaskType.FULI_TASK,
                    TaskList = _fuLiTaskList,
                    CanGetAward = CanGetAward(_fuLiTaskList)
                };
                _activityTaskLeftBtns.Add(fuliTaskBtn);
            }

            if (_freeEggTaskList.Count > 0)
            {
                var freeEggTaskBtn = new UserTaskData
                {
                    BtnSprite = FreeEggTaskSprite,
                    BtnSelectedSprite = FreeEggTaskSelectedSprite,
                    TaskType = UserTaskType.FREE_EGG_TASK,
                    TaskList = _freeEggTaskList,
                    CanGetAward = CanGetAward(_freeEggTaskList)
                };
                _activityTaskLeftBtns.Add(freeEggTaskBtn);
            }

            if (_newPlayerTaskList.Count > 0)
            {
                var newPlayerTaskBtn = new UserTaskData
                {
                    BtnSprite = NewPlayerTaskSprite,
                    BtnSelectedSprite = NewPlayerTaskSelectedSprite,
                    TaskType = UserTaskType.NEW_PLAYER_TASK,
                    TaskList = _newPlayerTaskList,
                    CanGetAward = CanGetAward(_newPlayerTaskList)
                };
                _activityTaskLeftBtns.Add(newPlayerTaskBtn);
            }
        }

        private bool CanGetAward(List<UserTaskState> taskList)
        {
            if (taskList == null || taskList.Count <= 0)
            {
                return false;
            }

            for (var i = 0; i < taskList.Count; i++)
            {
                var taskItem = taskList[i];
                if ((taskItem.total_progress <= taskItem.current_progress) && !taskItem.award_posted)
                {
                    return true;
                }
            }

            return false;
        }

        public override int SlotCount()
        {
            return _activityTaskLeftBtns.Count;
        }

        public override Item<UserTaskData> CreateItem()
        {
            return _leftBtnFactory.Create();
        }

        public override bool HasDivider()
        {
            return false;
        }

        public override GameObject CreateDivider()
        {
            return null;
        }

        public override float DataUpdateTime()
        {
            return _userTaskResult.Timestamp;
        }

        public override int DataCount()
        {
            return _activityTaskLeftBtns.Count;
        }

        public override UserTaskData GetData(int index)
        {
            if (index < 0 || index >= _activityTaskLeftBtns.Count)
            {
                return null;
            }

            return _activityTaskLeftBtns[index];
        }

        public override void OnItemSelected(Item<UserTaskData> item)
        {
            if (item == null)
            {
                return;
            }

            for (var i = 0; i < GetItemCount(); i++)
            {
                var btnItem = GetItem(i) as UserTaskItem;
                if (btnItem == null)
                {
                    continue;
                }

                var itemData = btnItem.GetData();
                if (itemData == null)
                {
                    continue;
                }

                btnItem.Tip.SetActive(itemData.CanGetAward);
                btnItem.Selected(false);
            }

            var thisItem = item as UserTaskItem;
            if (thisItem)
            {
                thisItem.Selected(true);
                thisItem.Tip.SetActive(false);
            }

            var taskData = item.GetData();
            if (taskData == null)
            {
                return;
            }
            //刷新右侧任务任务列表
            UserTaskDialog.RefreshTaskContent(taskData.TaskList);
        }
    }
}