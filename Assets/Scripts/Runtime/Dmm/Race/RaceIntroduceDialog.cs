using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Task;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Race
{
    public class RaceIntroduceDialog : MyDialog
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private ITaskManager _task;

        private IDataContainer<ApplyRaceResult> _applyRaceResult;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI,
            ITaskManager task)
        {
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
            _task = task;
            _applyRaceResult = dataRepository.GetContainer<ApplyRaceResult>(DataKey.ApplyRaceResult);
        }

        #endregion

        #region 组件

        public GameObject IntroduceImage;

        public GameObject AwardsImage;

        public GameObject RankImage;

        public GameObject IntroduceContent;

        public GameObject AwardsContent;

        public GameObject RankContent;

        public GameObject ButtonGroup;

        public Text RaceIntroduceTxt;

        public Text RaceAwardsTxt;

        public Text MyRankTxt;

        public Text MyScoreTxt;

        public Text HistoryRankNotExist;

        public Text RankNotExistTxt;

        #endregion

        private RaceConfig _raceConfig;

        private RaceDescriptionResult _raceData;

        public void Apply(RaceConfig raceConfig, RaceDescriptionResult raceData)
        {
            _raceConfig = raceConfig;
            _raceData = raceData;
            RefreshContent();
        }

        public DisplayRankList DisplayRankList;

        public override void BeforeShow()
        {
            OnIntroduceBtnClick();
        }

        public override void AfterShow()
        {
        }

        private void RefreshContent()
        {
            if (_raceData == null || _raceData.data == null)
            {
                return;
            }

            var current = _raceData.data.current;
            if (current == null)
            {
                MyRankTxt.text = "暂无排行";
                MyScoreTxt.text = "暂无积分";
            }
            else
            {
                if (current.rank == -1)
                {
                    MyRankTxt.text = "暂无排行";
                }
                else
                {
                    MyRankTxt.text = current.rank < 0 ? "暂无排行" : current.rank.ToString();
                }
                MyScoreTxt.text = current.score.ToString();
            }

            var raceData = _raceData.data.race;
            if (raceData == null)
            {
                _dialogManager.ShowConfirmBox("获取数据失败");
                return;
            }
            RaceIntroduceTxt.text = raceData.fullDescription;
            RaceAwardsTxt.text = raceData.fullAwardDescription;
            ButtonGroup.SetActive(true);
        }

        public void OnIntroduceBtnClick()
        {
            if (!IntroduceImage.activeSelf)
            {
                IntroduceImage.SetActive(true);
            }
            if (AwardsImage.activeSelf)
            {
                AwardsImage.SetActive(false);
            }
            if (RankImage.activeSelf)
            {
                RankImage.SetActive(false);
            }

            if (!IntroduceContent.activeSelf)
            {
                IntroduceContent.SetActive(true);
            }
            if (AwardsContent.activeSelf)
            {
                AwardsContent.SetActive(false);
            }
            if (RankContent.activeSelf)
            {
                RankContent.SetActive(false);
            }
        }

        public void OnAwardsBtnClick()
        {
            if (IntroduceImage.activeSelf)
            {
                IntroduceImage.SetActive(false);
            }
            if (!AwardsImage.activeSelf)
            {
                AwardsImage.SetActive(true);
            }
            if (RankImage.activeSelf)
            {
                RankImage.SetActive(false);
            }

            if (IntroduceContent.activeSelf)
            {
                IntroduceContent.SetActive(false);
            }
            if (!AwardsContent.activeSelf)
            {
                AwardsContent.SetActive(true);
            }
            if (RankContent.activeSelf)
            {
                RankContent.SetActive(false);
            }
        }

        public void OnRankBtnClick()
        {
            if (IntroduceImage.activeSelf)
            {
                IntroduceImage.SetActive(false);
            }
            if (AwardsImage.activeSelf)
            {
                AwardsImage.SetActive(false);
            }
            if (!RankImage.activeSelf)
            {
                RankImage.SetActive(true);
            }

            if (IntroduceContent.activeSelf)
            {
                IntroduceContent.SetActive(false);
            }
            if (AwardsContent.activeSelf)
            {
                AwardsContent.SetActive(false);
            }
            if (!RankContent.activeSelf)
            {
                RankContent.SetActive(true);
            }
        }

        public void OnSignupBtnClick()
        {
            if (_raceConfig == null)
            {
                _dialogManager.ShowToast("报名失败", 2, true);
                return;
            }
            var fee = _raceConfig.fee;
            if (fee != null)
            {
                _dialogManager.ShowConfirmBox(string.Format("是否花费<color=green>{0}{1}</color>报名比赛？",
                        fee.count, CurrencyType.LabelOf(fee.type)), true, "确定", () =>
                    {
                        _dialogManager.ShowWaitingDialog(true);
                        _applyRaceResult.ClearAndInvalidate(0);
                        _task.ExecuteTask(CheckApplyRaceResult, GetApplyRaceResultComplete,
                            () => { _dialogManager.ShowWaitingDialog(false); }, 5);

                        _remoteAPI.ApplyRace(_raceConfig.race_id);
                    }, true, "取消",
                    null, true, false, true);
            }
        }

        private bool CheckApplyRaceResult()
        {
            var applyRaceResult = _applyRaceResult.Read();
            if (applyRaceResult == null)
            {
                return false;
            }
            return true;
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
            _dialogManager.ShowToast("恭喜您报名成功了！", 2);
            _remoteAPI.RequestRaceConfigList();
            Hide();
        }

        public void SetHistoryRankNotExist(bool isShow)
        {
            HistoryRankNotExist.gameObject.SetActive(isShow);
        }

        public void SetRankNotExist(bool isShow)
        {
            RankNotExistTxt.gameObject.SetActive(isShow);
        }

        public void RefreshDisplayRankList(string subRaceId, bool isCurrent)
        {
            DisplayRankList.GetHistoryRankList(subRaceId, isCurrent);
        }
    }
}