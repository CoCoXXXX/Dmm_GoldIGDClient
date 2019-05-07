using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Dialog;

namespace Dmm.UserTask
{
    public class UserTaskDialog : MyDialog
    {
        public TaskList TaskList;

        public void RefreshTaskContent(List<UserTaskState> data)
        {
            TaskList.RefreshTaskList(data);
        }
    }
}