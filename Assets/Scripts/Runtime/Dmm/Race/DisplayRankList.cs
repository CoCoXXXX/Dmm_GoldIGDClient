using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Race
{
    public class DisplayRankList : ItemList<RankingList>
    {
        #region Inject

        private IAppContext _context;

        private DisplayRankItem.Factory _displayRankFactory;

        private IDataContainer<User> _myUser;

        private IDataContainer<RaceDescriptionResult> _raceDescriptionResult;

        private IDataContainer<List<RaceData>> _raceDataList;

        private IDataContainer<HistoryRaceRankResult> _historyRaceRankResult;

        [Inject]
        public void Initialize(
            IAppContext context,
            IDataRepository dataRepository,
            DisplayRankItem.Factory displayRankFactory)
        {
            _context = context;
            _displayRankFactory = displayRankFactory;
            _myUser = dataRepository.GetContainer<User>(DataKey.MyUser);
            _raceDescriptionResult =
                dataRepository.GetContainer<RaceDescriptionResult>(DataKey.RaceDescriptionResult);
            _raceDataList = dataRepository.GetContainer<List<RaceData>>(DataKey.RaceDataList);
            _historyRaceRankResult =
                dataRepository.GetContainer<HistoryRaceRankResult>(DataKey.HistoryRaceRankResult);
        }

        #endregion

        public RaceIntroduceDialog Dialog;

        private List<RankingList> _historyRankingList;

        private string _subRaceId;

        public void GetHistoryRankList(string subRaceId, bool isCurrent)
        {
            _subRaceId = subRaceId;
            if (isCurrent)
            {
                var currentRaceData = GetCurrentRaceData();
                if (currentRaceData != null)
                {
                    _historyRankingList = currentRaceData.rankingList.ToList();
                    _historyRankingList.Sort((x, y) => x.rank.CompareTo(y.rank));
                    Refresh();
                }
            }
            else
            {
                GetHistoryRankingListBySubRaceId(subRaceId);
            }
        }

        public RaceData GetCurrentRaceData()
        {
            var raceDescriptionResult = _raceDescriptionResult.Read();

            if (raceDescriptionResult == null)
            {
                return null;
            }

            var data = raceDescriptionResult.data;
            if (data == null)
            {
                return null;
            }

            var current = data.current;
            return current;
        }


        private void Refresh()
        {
            SelectEmpty();
            RefreshContent();
            SetRefreshTime(Time.time);
        }

        private void GetHistoryRankingListBySubRaceId(string subRaceId)
        {
            var dialogManager = _context.GetDialogManager();
            var task = _context.GetTaskManager();
            var historyRaceData = GetHistoryRaceData(subRaceId);

            if (historyRaceData != null)
            {
                var rankingList = historyRaceData.rankingList;
                if (rankingList != null)
                {
                    _historyRankingList = rankingList.ToList();
                    _historyRankingList.Sort((x, y) => x.rank.CompareTo(y.rank));
                    Refresh();
                    return;
                }
            }

            dialogManager.ShowWaitingDialog(true);
            _historyRaceRankResult.ClearNotInvalidate();
            task.ExecuteTask(CheckRankingListResult, () => dialogManager.ShowWaitingDialog(false));
            GetRankingListResult();
        }

        //格式 http://114.55.30.148:18080/race-service/getHistoryInfo?username=huang&subRaceId=df6a8d85-fca2-4061-b3a4-1aea97d78b44
        private void GetRankingListResult()
        {
            var configHolder = _context.GetConfigHolder();
            var myUser = _myUser.Read();
            var userName = myUser.Username();
            var address = configHolder.RaceHistoryRankUrl;
            var data = string.Format("username={0}&subRaceId={1}",
                userName, _subRaceId);
            var url = address + data;

            StartCoroutine(GetRankingListResult(url));
        }

        private IEnumerator GetRankingListResult(string url)
        {
            HistoryRaceRankResult res = null;
            var www = new WWW(url);
            yield return www;

            var errorMsg = "获取排行榜数据失败";
            if (www.error != null)
            {
                var errLog = www.error;
                MyLog.ErrorWithFrame("huData", "HistoryRaceRankResult fail :" + errLog);

                res = new HistoryRaceRankResult(HistoryRaceRankResult.Error, errorMsg, null);
                _historyRaceRankResult.Write(res, Time.time);

                www.Dispose();
                yield break;
            }

            var data = www.text.ToString();
            MyLog.InfoWithFrame("huData", "HistoryRaceRankResult data is :" + data);

            try
            {
                res = JsonUtility.FromJson<HistoryRaceRankResult>(data);
            }
            catch (Exception e)
            {
                res = new HistoryRaceRankResult(HistoryRaceRankResult.Error, errorMsg, null);
            }

            www.Dispose();
            _historyRaceRankResult.Write(res, Time.time);
        }

        private bool CheckRankingListResult()
        {
            var dialogManager = _context.GetDialogManager();
            var data = _historyRaceRankResult.Read();
            if (data == null)
            {
                return false;
            }

            dialogManager.ShowWaitingDialog(false);
            if (data.result == RaceDescriptionResult.Ok)
            {
                if (data.data != null)
                {
                    AddHistoryRaceData(data.data.current);
                    var current = data.data.current;
                    _historyRankingList = current.rankingList.ToList();
                    _historyRankingList.Sort((x, y) => x.rank.CompareTo(y.rank));
                    Refresh();
                }
            }
            else
            {
                var msg = data.error;
                if (!string.IsNullOrEmpty(msg))
                {
                    dialogManager.ShowToast(msg, 2, true);
                }
                else
                {
                    dialogManager.ShowToast("获取排行榜数据失败", 2, true);
                }
            }

            return true;
        }

        public void AddHistoryRaceData(RaceData historyRaceData)
        {
            var raceDataList = _raceDataList.Read();
            if (raceDataList == null)
            {
                raceDataList = new List<RaceData>();
            }

            if (historyRaceData != null)
            {
                for (var i = 0; i < raceDataList.Count; i++)
                {
                    if (raceDataList[i].subRaceId == historyRaceData.subRaceId)
                    {
                        raceDataList[i] = historyRaceData;
                        return;
                    }
                }

                raceDataList.Add(historyRaceData);
            }

            _raceDataList.Write(raceDataList, Time.time);
        }

        public RaceData GetHistoryRaceData(string subRaceId)
        {
            var raceDataList = _raceDataList.Read();

            if (raceDataList == null)
            {
                raceDataList = new List<RaceData>();
            }

            for (var i = 0; i < raceDataList.Count; i++)
            {
                if (subRaceId == raceDataList[i].subRaceId)
                {
                    return raceDataList[i];
                }
            }

            _raceDataList.Write(raceDataList, Time.time);
            return null;
        }

        public override int SlotCount()
        {
            var count = _historyRankingList.Count;
            Dialog.SetRankNotExist(count <= 0);
            return count;
        }

        public override Item<RankingList> CreateItem()
        {
            return _displayRankFactory.Create();
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
            var count = _historyRankingList.Count;
            Dialog.SetRankNotExist(count <= 0);
            return count;
        }

        public override RankingList GetData(int index)
        {
            var count = _historyRankingList.Count;
            if ((index < 0) || (index >= count))
            {
                return null;
            }

            return _historyRankingList[index];
        }

        public override void OnItemSelected(Item<RankingList> item)
        {
        }
    }
}