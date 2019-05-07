using System;

namespace Dmm.Help
{
    [Serializable]
    public class HistoryRecordResult
    {
        public const int Ok = 0;

        public const int Error = -100;

        public int result;

        public string error;

        public HistoryRecord[] data;

        public HistoryRecordResult()
        {
        }

        public HistoryRecordResult(int result, string errMsg, HistoryRecord[] historyRecord)
        {
            this.result = result;
            this.error = errMsg;
            this.data = historyRecord;
        }
    }

    [Serializable]
    public class HistoryRecord
    {
        public string contact;

        public string content;

        public long createTime;

        public int issueId;

        public string reply;

        public string replyTime;

        public long resolveTime;

        public bool resolved;

        public int type;

        public string username;
    }
}