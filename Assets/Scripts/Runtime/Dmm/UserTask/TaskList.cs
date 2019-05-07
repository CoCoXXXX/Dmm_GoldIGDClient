using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Dialog;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.UserTask
{
    public class TaskList : ItemList<UserTaskState>
    {
        #region Inject

        private IDialogManager _dialogManager;

        private TaskItem.Factory _taskItemFactory;

        [Inject]
        public void Initialize(IDialogManager dialogManager, TaskItem.Factory taskItemFactory)
        {
            _dialogManager = dialogManager;
            _taskItemFactory = taskItemFactory;
        }

        #endregion

        private readonly List<UserTaskState> _userTaskList = new List<UserTaskState>();

        public UserTaskDialog UserTaskDialog;

        public void RefreshTaskList(List<UserTaskState> userTaskList)
        {
            if (userTaskList == null || userTaskList.Count <= 0)
            {
                _dialogManager.ShowToast("获取任务数据失败！", 2, true);
            }

            _userTaskList.Clear();
            _userTaskList.AddRange(userTaskList);
            RefreshContent();
        }

        public override int SlotCount()
        {
            return _userTaskList.Count;
        }

        public override Item<UserTaskState> CreateItem()
        {
            var item = _taskItemFactory.Create();
            if (item)
            {
                item.UserTaskDialog = UserTaskDialog;
            }
            return item;
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
            return 0;
        }

        public override int DataCount()
        {
            return _userTaskList.Count;
        }

        public override UserTaskState GetData(int index)
        {
            if (index < 0 || index >= DataCount())
            {
                return null;
            }

            return _userTaskList[index];
        }

        public override void OnItemSelected(Item<UserTaskState> item)
        {
        }
    }
}