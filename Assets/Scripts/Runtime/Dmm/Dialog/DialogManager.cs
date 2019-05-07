using System;
using System.Collections.Generic;
using com.morln.game.gd.command;
using DG.Tweening;
using Dmm.App;
using Dmm.Common;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Res;
using Dmm.Task;
using Dmm.Widget;
using UnityEngine;
using Zenject;
using Toast = Dmm.Common.Toast;

namespace Dmm.Dialog
{
    public class DialogManager : MonoBehaviour, IDialogManager
    {
        public class DialogFactory : PrefabFactory<UIWindow>
        {
        }

        #region Inject

        private IAppContext _context;

        private IGameCanvas _gameCanvas;

        private DialogFactory _dialogFactory;

        private ITaskManager _task;

        private IResourceManager _resource;

        [Inject]
        public void Inject(
            IAppContext context,
            IGameCanvas gameCanvas,
            ITaskManager taskManager,
            IResourceManager resourceManager,
            DialogFactory dialogFactory)
        {
            _context = context;
            _gameCanvas = gameCanvas;
            _task = taskManager;
            _dialogFactory = dialogFactory;
            _resource = resourceManager;

            _autoShowDialogNameAndFunc.Add(DialogName.CheckinDialog, AutoShowCheckinDialog);
            _autoShowDialogNameAndFunc.Add(DialogName.UserTaskDialog, AutoShowUserTaskDialog);
        }

        #endregion

        #region Dialog

        private const string DialogPath = "Dialog/";

        private readonly Dictionary<string, UIWindow> _dialogCache = new Dictionary<string, UIWindow>();

        public void ShowDialog<T>(string dialogName, bool autoShow = true,
            bool cache = false, Action<T> success = null) where T : UIWindow
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                ShowToast("加载对话框失败", 2, true);
                return;
            }

            UIWindow dialog = null;

            if (cache && _dialogCache.ContainsKey(dialogName))
            {
                dialog = _dialogCache[dialogName];
            }

            if (dialog)
            {
                AsyncLoadDialog<T>(
                    dialogName,
                    autoShow,
                    cache,
                    null,
                    () => { return TaskResult.Success(); },
                    success,
                    null
                );

                return;
            }

            AsyncLoadDialog<T>(
                dialogName,
                autoShow,
                cache,
                () =>
                {
                    var assetBundleName = DialogAssetBundleMap.GetAssetBundleName(dialogName);
                    _resource.StartLoadResource(assetBundleName, dialogName);
                },
                () =>
                {
                    var assetBundleName = DialogAssetBundleMap.GetAssetBundleName(dialogName);
                    if (string.IsNullOrEmpty(assetBundleName))
                    {
                        return TaskResult.Fail(ResultCode.FAILED, "加载对话框失败");
                    }

                    _resource.HasCached(assetBundleName);

                    if (!_resource.HasCached(assetBundleName))
                    {
                        return null;
                    }

                    return TaskResult.Success();
                },
                success,
                (errCode, errMsg) =>
                {
                    // fail
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        ShowToast(errMsg, 2, true);
                    }
                    else
                    {
                        ShowToast("加载对话框失败", 2, true);
                    }
                },
                () => { ShowToast("加载对话框超时", 2, true); }
            );
        }

        private void AsyncLoadDialog<T>(
            string dialogName,
            bool autoShow,
            bool cache,
            Action action,
            Func<TaskResult> checker,
            Action<T> success,
            Action<int, string> failHandler,
            Action timeoutHandler = null,
            int timeout = 10,
            bool showWaiting = true) where T : UIWindow
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                return;
            }

            if (showWaiting)
            {
                ShowWaitingDialog(true, timeout);
            }

            if (action != null)
            {
                action();
            }

            _task.ExecuteTask(
                checker,
                () =>
                {
                    if (showWaiting)
                    {
                        ShowWaitingDialog(false);
                    }
                },
                // success
                () =>
                {
                    var dialog = default(T);
                    if (cache && _dialogCache.ContainsKey(dialogName))
                    {
                        dialog = _dialogCache[dialogName] as T;
                    }

                    if (!dialog)
                    {
                        dialog = CreateDialog(dialogName) as T;
                    }

                    if (dialog)
                    {
                        dialog.transform.SetParent(_gameCanvas.GetDialogContainer(), false);
                        dialog.transform.SetAsLastSibling();
                        if (autoShow)
                        {
                            dialog.Show();
                        }

                        if (cache)
                        {
                            if (_dialogCache.ContainsKey(dialogName))
                            {
                                _dialogCache[dialogName] = dialog;
                            }
                            else
                            {
                                _dialogCache.Add(dialogName, dialog);
                            }
                        }

                        if (success != null)
                        {
                            success(dialog);
                        }
                    }
                    else
                    {
                        MyLog.ErrorWithFrame(name,"创建对话框失败 ：" + dialogName + " 检查是否是类型不匹配");
                    }
                },
                (errCode, errMsg) =>
                {
                    if (failHandler != null)
                    {
                        failHandler(errCode, errMsg);
                    }
                },
                () =>
                {
                    if (showWaiting)
                    {
                        ShowWaitingDialog(false);
                    }

                    if (timeoutHandler != null)
                    {
                        timeoutHandler();
                    }
                },
                timeout
            );
        }

        public UIWindow GetCachedDialog(string dialogName)
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                return null;
            }

            if (!_dialogCache.ContainsKey(dialogName))
            {
                return null;
            }

            return _dialogCache[dialogName];
        }

        public void HideDialog(string dialogName)
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                return;
            }

            if (!_dialogCache.ContainsKey(dialogName))
            {
                return;
            }

            var dialog = _dialogCache[dialogName];
            dialog.Hide();
            _dialogCache.Remove(dialogName);
        }

        public void AddDialogToCacheDialog(string dialogName, UIWindow dialog)
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                return;
            }

            if (_dialogCache.ContainsKey(dialogName))
            {
                return;
            }

            _dialogCache.Add(dialogName, dialog);
        }

        /// <summary>
        /// 必须确保dialog的assetbundle已经缓存在字典后调用
        /// </summary>
        /// <param name="dialogName"></param>
        /// <returns></returns>
        private UIWindow CreateDialog(string dialogName)
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                return null;
            }

            var assetBundleName = DialogAssetBundleMap.GetAssetBundleName(dialogName);
            var prefab = _resource.GetResource<GameObject>(assetBundleName, dialogName);

            if (!prefab)
            {
                return null;
            }

            return _dialogFactory.Create(prefab);
        }

        public void RequestDialog<T>(
            string dialogName,
            Action action,
            Func<TaskResult> checker,
            Action success,
            Action<int, string> failHandler,
            Action timeoutHandler = null,
            int timeout = 5,
            bool showWaiting = true) where T : UIWindow
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                return;
            }

            if (showWaiting)
            {
                ShowWaitingDialog(true, timeout);
            }

            if (action != null)
            {
                action();
            }

            if (checker == null)
            {
                if (showWaiting)
                {
                    ShowWaitingDialog(false);
                }

                if (success != null)
                {
                    success();
                }
                return;
            }

            _task.ExecuteTask(
                checker,
                () =>
                {
                    if (showWaiting)
                    {
                        ShowWaitingDialog(false);
                    }
                },
                () =>
                {
                    if (success != null)
                    {
                        success();
                    }
                },
                (errCode, errMsg) =>
                {
                    if (failHandler != null)
                    {
                        failHandler(errCode, errMsg);
                    }
                },
                () =>
                {
                    if (showWaiting)
                    {
                        ShowWaitingDialog(false);
                    }

                    if (timeoutHandler != null)
                    {
                        timeoutHandler();
                    }
                },
                timeout
            );
        }

        #endregion

        #region Toast

        public Toast ToastPrefab;

        public float ToastShowAnimationTime = 0.2f;

        public float ToastHideAnimationTime = 0.1f;

        public void ShowToast(string content, float time, bool error = false)
        {
            var toastContainer = _gameCanvas.GetToastParent();
            if (!ToastPrefab || !toastContainer)
                return;

            var toast = Instantiate(ToastPrefab) as Toast;
            if (!toast) return;

            toast.Content = content;
            toast.Error = error;
            toast.transform.SetParent(toastContainer, false);

            if (toast.CanvasGroup)
            {
                toast.CanvasGroup.alpha = 0;
                var sequence = DOTween.Sequence();
                sequence
                    .Append(toast.CanvasGroup.DOFade(1, ToastShowAnimationTime))
                    .AppendInterval(time)
                    .Append(toast.CanvasGroup.DOFade(0, ToastHideAnimationTime))
                    .AppendCallback(() => Destroy(toast.gameObject));

                sequence.Play();
            }
            else
            {
                Destroy(toast.gameObject);
            }
        }

        #endregion

        #region AlertBox

        public AlertBox AlertBoxPrefab;

        public AlertBox ShowMessageBox(string content)
        {
            if (!AlertBoxPrefab)
                return null;

            var alertBox = Instantiate(AlertBoxPrefab) as AlertBox;
            if (!alertBox)
                return null;

            alertBox.transform.SetParent(_gameCanvas.GetDialogContainer(), false);
            alertBox.transform.SetAsLastSibling();

            alertBox.Show(
                content,
                false, null, null,
                false, null, null,
                true, true
            );

            return alertBox;
        }

        public AlertBox ShowConfirmBox(string content)
        {
            if (!AlertBoxPrefab)
                return null;

            var alertBox = Instantiate(AlertBoxPrefab) as AlertBox;
            if (!alertBox)
                return null;

            alertBox.transform.SetParent(_gameCanvas.GetDialogContainer(), false);
            alertBox.transform.SetAsLastSibling();

            alertBox.Show(content);
            return alertBox;
        }

        public AlertBox ShowConfirmBox(
            string content,
            bool enableOkBtn, string okBtnContent, AlertBox.OnClickDelegate onClickOk,
            bool enableCancelBtn, string cancelBtnContent, AlertBox.OnClickDelegate onClickCancel,
            bool enableBg, bool closeOnBg, bool enableCloseBtn)
        {
            if (!AlertBoxPrefab)
                return null;

            var alertBox = Instantiate(AlertBoxPrefab) as AlertBox;
            if (!alertBox)
                return null;

            alertBox.transform.SetParent(_gameCanvas.GetDialogContainer(), false);
            alertBox.transform.SetAsLastSibling();

            alertBox.Show(
                content,
                enableOkBtn, okBtnContent, onClickOk,
                enableCancelBtn, cancelBtnContent, onClickCancel,
                enableBg, closeOnBg, enableCloseBtn);

            return alertBox;
        }

        #endregion

        #region WaitingDialog

        // 因为WaitingDialog是需要经常显示的一个dialog。
        // 而且需要的反应比较快，所以就不使用Prefab的方式了。

        /// <summary>
        /// 显示等待对话框。
        /// </summary>
        /// <param name="show"></param>
        /// <param name="timeout"></param>
        public void ShowWaitingDialog(bool show, float timeout = 10)
        {
            var waitingDialog = _gameCanvas.GetWaitingDialog();
            if (waitingDialog)
            {
                if (show)
                {
                    waitingDialog.Show(timeout);
                }
                else
                {
                    waitingDialog.Hide();
                }
            }
        }

        #endregion

        #region AutoShowDialog Queue 自动显示的对话框队列

        private readonly Queue<AutoShowDialogData> _autoShowDialogDatas = new Queue<AutoShowDialogData>();

        private readonly string _fristDialogName = DialogName.CheckinDialog;

        private readonly Dictionary<string, Action<AutoShowDialogData>> _autoShowDialogNameAndFunc =
            new Dictionary<string, Action<AutoShowDialogData>>();

        public void InitAutoShowDialogDataQueue()
        {
            _autoShowDialogDatas.Clear();

            foreach (var k in _autoShowDialogNameAndFunc)
            {
                var data = new AutoShowDialogData();
                data.DialogName = k.Key;
                data.DialogShow = (int) DialogDataState.Null;
                data.Trigger = k.Key == _fristDialogName;
                data.Action = k.Value;
                data.CurrentShowDialog = null;
                _autoShowDialogDatas.Enqueue(data);
            }
        }

        public void UpdateAutoShowDialog()
        {
            if (_autoShowDialogDatas == null || GetAutoShowDialogCount() <= 0)
            {
                return;
            }

            var data = PeekAutoShowDialogData();
            if (!data.Trigger)
            {
                return;
            }

            switch (data.DialogShow)
            {
                case (int) DialogDataState.Null:
                    data.Action(data);
                    data.SetDialogDataState(DialogDataState.Wait);
                    break;
                case (int) DialogDataState.Wait:
                    break;
                case (int) DialogDataState.Ok: //对话框已经打开
                    if (data.CurrentShowDialog != null)
                    {
                        break;
                    }

                    DequeueAutoShowDialogData();
                    OpenCurrentDialogTrigger();
                    break;
                case (int) DialogDataState.Failed: //队列首个对话框没打开
                    DequeueAutoShowDialogData();
                    OpenCurrentDialogTrigger();
                    break;
            }
        }

        public int GetAutoShowDialogCount()
        {
            return _autoShowDialogDatas.Count;
        }

        public AutoShowDialogData DequeueAutoShowDialogData()
        {
            if (GetAutoShowDialogCount() <= 0)
            {
                return null;
            }

            return _autoShowDialogDatas.Dequeue();
        }

        public bool ContainAutoShowDialogData(AutoShowDialogData data)
        {
            return _autoShowDialogDatas.Contains(data);
        }

        public bool ContainAutoShowDialogData(string dialogName)
        {
            foreach (var k in _autoShowDialogDatas)
            {
                if (k.DialogName == dialogName)
                {
                    return true;
                }
            }
            return false;
        }

        public void OpenCurrentDialogTrigger()
        {
            if (GetAutoShowDialogCount() <= 0)
            {
                return;
            }

            var data = PeekAutoShowDialogData();
            if (data == null)
            {
                return;
            }
            data.Trigger = true;
        }

        public AutoShowDialogData PeekAutoShowDialogData()
        {
            return _autoShowDialogDatas.Peek();
        }

        public void AutoShowCheckinDialog(AutoShowDialogData dialogData)
        {
            var remoteAPI = _context.GetRemoteAPI();
            var task = _context.GetTaskManager();
            var dataRepository = _context.GetDataRepository();
            var checkinConfigResultContainer =
                dataRepository.GetContainer<CheckinConfigResult>(DataKey.CheckinConfigResult);

            checkinConfigResultContainer.ClearNotInvalidate();
            remoteAPI.RequestCheckinConfig();
            task.ExecuteTask(
                () =>
                {
                    var data = checkinConfigResultContainer.Read();
                    dialogData.SetDialogDataState(DialogDataState.Wait);
                    return data != null;
                },
                () =>
                {
                    var data = checkinConfigResultContainer.Read();
                    if (data == null)
                    {
                        dialogData.SetDialogDataState(DialogDataState.Failed);
                        return;
                    }
                    var config = data.checkin_config;
                    if (config == null)
                    {
                        dialogData.SetDialogDataState(DialogDataState.Failed);
                        return;
                    }
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

                    if (todayChecked)
                    {
                        dialogData.SetDialogDataState(DialogDataState.Failed);
                        return;
                    }

                    ShowDialog<UIWindow>(DialogName.CheckinDialog, true, true,
                        (dialog) =>
                        {
                            dialogData.SetDialogDataState(DialogDataState.Ok);
                            dialogData.SetCurrentShowDialog(dialog);
                        });
                }, null);
        }

        public void AutoShowUserTaskDialog(AutoShowDialogData dialogData)
        {
            var dataRepository = _context.GetDataRepository();
            var userTaskListResult = dataRepository.GetContainer<UserTaskListResult>(DataKey.UserTaskListResult);
            var task = _context.GetTaskManager();

            var remoteApi = _context.GetRemoteAPI();
            userTaskListResult.ClearNotInvalidate();
            remoteApi.RequestUserTaskList();

            task.ExecuteTask(() =>
                {
                    var data = userTaskListResult.Read();
                    dialogData.SetDialogDataState(DialogDataState.Wait);
                    return data != null;
                },
                () =>
                {
                    var data = userTaskListResult.Read();
                    if (data == null)
                    {
                        dialogData.SetDialogDataState(DialogDataState.Failed);
                        return;
                    }

                    var res = data.res;
                    if (res == null)
                    {
                        dialogData.SetDialogDataState(DialogDataState.Failed);
                        return;
                    }

                    if (res.code == ResultCode.OK)
                    {
                        var taskList = data.user_task_state;
                        if (taskList == null || taskList.Count <= 0)
                        {
                            dialogData.SetDialogDataState(DialogDataState.Failed);
                            return;
                        }

                        ShowDialog<UIWindow>(DialogName.UserTaskDialog, true, true,
                            (dialog) =>
                            {
                                dialogData.SetDialogDataState(DialogDataState.Ok);
                                dialogData.SetCurrentShowDialog(dialog);
                            });
                    }
                    else
                    {
                        dialogData.SetDialogDataState(DialogDataState.Failed);
                    }
                }, null
            );
        }

        #endregion
    }
}