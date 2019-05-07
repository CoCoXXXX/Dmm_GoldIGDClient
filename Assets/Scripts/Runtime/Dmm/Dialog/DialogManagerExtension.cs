using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Checkin;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Task;
using Dmm.UserTask;

namespace Dmm.Dialog
{
    public static class DialogManagerExtension
    {
        public static void ShowUserTaskDialog(this IDialogManager dialogManager, IAppContext context)
        {
            var dataRepository = context.GetDataRepository();
            var userTaskListResult = dataRepository.GetContainer<UserTaskListResult>(DataKey.UserTaskListResult);
            dialogManager.RequestDialog<UserTaskDialog>(DialogName.UserTaskDialog,
                () =>
                {
                    var remoteApi = context.GetRemoteAPI();
                    userTaskListResult.ClearNotInvalidate();
                    remoteApi.RequestUserTaskList();
                },
                () =>
                {
                    var data = userTaskListResult.Read();
                    if (data == null)
                    {
                        return null;
                    }

                    var res = data.res;
                    if (res == null)
                    {
                        return null;
                    }

                    if (res.code == ResultCode.OK)
                    {
                        var taskList = data.user_task_state;
                        if (taskList == null || taskList.Count <= 0)
                        {
                            return TaskResult.Fail(ResultCode.FAILED, "没有任务数据");
                        }
                        return TaskResult.Success();
                    }
                    else
                    {
                        return TaskResult.Fail(res.code, res.msg);
                    }
                },
                () =>
                {
                    dialogManager.ShowDialog<UserTaskDialog>(DialogName.UserTaskDialog, false, false,
                        (dialog) =>
                        {
                            var data = userTaskListResult.Read();
                            if (data != null)
                            {
                                dialogManager.AddDialogToCacheDialog(DialogName.UserTaskDialog, dialog);
                                dialog.Show();
                            }
                        });
                },
                (errCode, errMsg) =>
                {
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        dialogManager.ShowToast(errMsg, 3, true);
                    }
                    else
                    {
                        dialogManager.ShowToast("获取任务信息失败", 3, true);
                    }
                });
        }

        public static void RequestShowCheckinDialog(this IDialogManager dialogManager, IAppContext context)
        {
            var dataRepository = context.GetDataRepository();
            var checkinConfigResult = dataRepository.GetContainer<CheckinConfigResult>(DataKey.CheckinConfigResult);
            var remoteAPI = context.GetRemoteAPI();

            dialogManager.RequestDialog<CheckinDialog>(
                DialogName.CheckinDialog,
                () =>
                {
                    checkinConfigResult.ClearNotInvalidate();
                    remoteAPI.RequestCheckinConfig();
                },
                () =>
                {
                    var data = checkinConfigResult.Read();
                    if (data == null)
                    {
                        return null;
                    }

                    if (data.res.code == ResultCode.OK)
                    {
                        return TaskResult.Success();
                    }
                    else
                    {
                        return TaskResult.Fail(data.res.code, data.res.msg);
                    }
                },
                () =>
                {
                    dialogManager.ShowDialog<CheckinDialog>(DialogName.CheckinDialog, false, false,
                        (dialog) =>
                        {
                            dialogManager.AddDialogToCacheDialog(DialogName.CheckinDialog, dialog);
                            dialog.Show();
                        });
                },
                (errCode, errMsg) =>
                {
                    // fail
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        dialogManager.ShowToast(errMsg, 3, true);
                    }
                    else
                    {
                        dialogManager.ShowToast("请求签到数据失败", 3, true);
                    }
                }
            );
        }
    }
}