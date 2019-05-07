using System;
using System.Collections;
using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Task;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Race
{
    public class RaceItem : Item<RaceConfig>
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private ITaskManager _task;

        private ConfigHolder _config;

        private IDataContainer<User> _myUser;

        private IDataContainer<ApplyRaceResult> _applyRaceResult;

        private IDataContainer<RaceDescriptionResult> _raceDescriptionResult;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI,
            ITaskManager task,
            ConfigHolder configHolder)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _task = task;
            _config = configHolder;
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
            _applyRaceResult = dataRepository.GetContainer<ApplyRaceResult>(DataKey.ApplyRaceResult);
            _raceDescriptionResult =
                dataRepository.GetContainer<RaceDescriptionResult>(DataKey.RaceDescriptionResult);
        }

        public class Factory : Factory<RaceItem>
        {
        }

        #endregion

        #region 组件

        public AsyncImage RaceIcon;

        public Text DisPlayNameTxt;

        public Text RaceDateTxt;

        public Text RaceDescriptionTxt;

        public Text RaceStartTimeTxt;

        public CurrencyValue Tickets;

        public Button SignUpBtn;

        public Button MatchBtn;

        public Button SignUpOverBtn;

        /// <summary>
        /// 按钮。
        /// </summary>
        public Button Button;

        /// <summary>
        /// 房间ID。
        /// </summary>
        public long RaceId
        {
            get
            {
                if (_data == null)
                    return -1;

                return _data.race_id;
            }
        }

        /// <summary>
        /// 比赛数据。
        /// </summary>
        private RaceConfig _data;

        #endregion

        public override RaceConfig GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, RaceConfig data)
        {
            _data = data;
            if (data == null)
            {
                return;
            }

            // 载入比赛图片。
            if (string.IsNullOrEmpty(data.pic))
            {
                RaceIcon.Reset();
            }
            else
            {
                RaceIcon.SetTargetPic(data.pic, null, data.pic_url);
            }

            DisPlayNameTxt.text = data.display_name;
            RaceDateTxt.text = data.race_description;
            RaceDescriptionTxt.text = data.award_description;
            RaceStartTimeTxt.text = data.open_time;
            if (data.fee != null)
            {
                Tickets.SetCurrency(data.fee.count, data.fee.type);
            }

            if (!data.race_over)
            {
                SignUpOverBtn.gameObject.SetActive(false);
                SignUpBtn.gameObject.SetActive(!data.signed_up);
                MatchBtn.gameObject.SetActive(data.signed_up);
            }
            else
            {
                SignUpOverBtn.gameObject.SetActive(true);
            }
        }

        public override void Reset(int currentIndex)
        {
            RaceIcon.Reset();
            DisPlayNameTxt.text = "";
            RaceDateTxt.text = "";
            RaceDescriptionTxt.text = "";
            RaceStartTimeTxt.text = "";
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return Button;
        }

        public void OnSignUpBtnClicked()
        {
            if (_data == null)
                return;

            var fee = _data.fee;
            if (fee != null)
            {
                _dialogManager.ShowConfirmBox(string.Format("是否花费<color=green>{0}{1}</color>报名比赛？",
                        fee.count, CurrencyType.LabelOf(fee.type)), true, "确定", () =>
                    {
                        _dialogManager.ShowWaitingDialog(true);
                        _applyRaceResult.ClearNotInvalidate();
                        _task.ExecuteTask(CheckApplyRaceResult, GetApplyRaceResultComplete,
                            () => { _dialogManager.ShowWaitingDialog(false); }, 5);

                        _remoteAPI.ApplyRace(_data.race_id);
                    }, true, "取消",
                    null, true, false, true);
            }
        }

        public void OnMatchBtnClicked()
        {
            if (_data == null)
            {
                _dialogManager.ShowToast("参加比赛失败,如有疑问请与客服联系", 2, true);
                return;
            }

            _remoteAPI.ChooseRoom((int) _data.room_id);
        }

        public void OnRuleBtnClicked()
        {
            _dialogManager.RequestDialog<RaceIntroduceDialog>(
                DialogName.RaceIntroduceDialog,
                () =>
                {
                    _raceDescriptionResult.ClearNotInvalidate();
                    GetRaceDescriptionResult();
                },
                () =>
                {
                    var data = _raceDescriptionResult.Read();
                    if (data == null)
                    {
                        return null;
                    }

                    if (data.result == RaceDescriptionResult.Ok)
                    {
                        return TaskResult.Success();
                    }
                    else
                    {
                        return TaskResult.Fail(data.result, data.error);
                    }
                },
                () =>
                {
                    _dialogManager.ShowDialog<RaceIntroduceDialog>(DialogName.RaceIntroduceDialog, false, false,
                        (dialog) =>
                        {
                            dialog.Apply(_data, _raceDescriptionResult.Read());
                            dialog.Show();
                        });
                },
                (errCode, errMsg) =>
                {
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        _dialogManager.ShowToast(errMsg, 3, true);
                    }
                    else
                    {
                        _dialogManager.ShowToast("获取比赛信息失败", 3, true);
                    }
                }
            );
        }

        //格式 http://localhost/race/getInfo?username=huang&subRaceId=f6419a7536e64ca18a5f4d1833c5807b
        private void GetRaceDescriptionResult()
        {
            if (_data == null)
            {
                _dialogManager.ShowToast("数据发生错误", 2, true);
                return;
            }

            var raceId = _data.race_id;
            var myUser = _myUser.Read();
            var userName = myUser.Username();
            var address = _config.RaceDescriptionUrl;
            var data = string.Format("username={0}&raceId={1}",
                userName, raceId);
            var url = address + data;

            StartCoroutine(GetRaceDescriptionResult(url));
        }

        private IEnumerator GetRaceDescriptionResult(string url)
        {
            RaceDescriptionResult res = null;
            var www = new WWW(url);
            yield return www;

            var errorMsg = "获取数据失败";
            if (www.error != null)
            {
                var errLog = www.error;
                MyLog.ErrorWithFrame("huData", "GetRaceDescriptionResult fail :" + errLog);

                res = new RaceDescriptionResult(RaceDescriptionResult.Error, errorMsg, null);
                _raceDescriptionResult.Write(res, Time.time);

                www.Dispose();
                yield break;
            }

            var data = www.text;
            MyLog.InfoWithFrame("huData", "RaceDescriptionResult data is :" + data);

            try
            {
                res = JsonUtility.FromJson<RaceDescriptionResult>(data);
            }
            catch (Exception e)
            {
                res = new RaceDescriptionResult(RaceDescriptionResult.Error, errorMsg, null);
            }

            www.Dispose();
            _raceDescriptionResult.Write(res, Time.time);
        }

        private bool CheckApplyRaceResult()
        {
            var applyRaceResult = _applyRaceResult.Read();
            return applyRaceResult != null;
        }

        private void GetApplyRaceResultComplete()
        {
            _dialogManager.ShowWaitingDialog(false);
            var applyRaceResult = _applyRaceResult.Read();
            if (applyRaceResult == null)
            {
                _dialogManager.ShowToast("报名失败,如有疑问请与客服联系", 2, true);
                return;
            }
            var res = applyRaceResult.res;
            if (res == null)
            {
                _dialogManager.ShowToast("报名失败,如有疑问请与客服联系", 2, true);
                return;
            }

            if (res.code != ResultCode.OK)
            {
                var errMsg = res.msg;
                if (string.IsNullOrEmpty(errMsg))
                {
                    _dialogManager.ShowToast("报名失败,如有疑问请与客服联系", 2, true);
                }
                else
                {
                    _dialogManager.ShowToast(errMsg, 2, true);
                }
                return;
            }

            _remoteAPI.RequestRaceConfigList();
        }
    }
}