using System;

namespace Dmm.Race
{
    [Serializable]
    public class HistoryRaceRankResult
    {
        public const int Ok = 0;

        public const int Error = -100;

        public int result;

        public string error;

        public HistoryRaceRank data;

        public HistoryRaceRankResult()
        {
        }

        public HistoryRaceRankResult(int result, string errMsg, HistoryRaceRank historyRaceRank)
        {
            this.result = result;
            this.error = errMsg;
            this.data = historyRaceRank;
        }
    }

    [Serializable]
    public class HistoryRaceRank
    {
        public RaceData current;
    }
}