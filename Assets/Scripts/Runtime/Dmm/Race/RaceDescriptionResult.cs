using System;

namespace Dmm.Race
{
    [Serializable]
    public class RaceDescriptionResult
    {
        public const int Ok = 0;

        public const int Error = -100;

        public int result;

        public string error;

        public RaceDescription data;

        public RaceDescriptionResult()
        {
        }

        public RaceDescriptionResult(int result, string errMsg, RaceDescription raceDescription)
        {
            this.result = result;
            this.error = errMsg;
            this.data = raceDescription;
        }
    }

    [Serializable]
    public class RaceDescription
    {
        public RaceData[] historyList;

        public RaceData current;

        public Race race;
    }

    [Serializable]
    public class RaceData
    {
        public bool IsCurrent = false;

        public string subRaceId;

        public long rank;

        public long score;

        public string signUpTime;

        public RankingList[] rankingList;
    }

    [Serializable]
    public class RankingList
    {
        public string nickname;

        public long rank;

        public long score;

        public string username;
    }

    [Serializable]
    public class Race
    {
        /// <summary>
        /// 详细的奖励描述用于在比赛详情中显示
        /// </summary>
        public string fullAwardDescription;

        /// <summary>
        /// 详细的比赛描述用于在比赛详情中显示
        /// </summary>
        public string fullDescription;
    }
}