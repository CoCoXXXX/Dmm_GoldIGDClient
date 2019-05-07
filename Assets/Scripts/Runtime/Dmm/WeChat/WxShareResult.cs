using System;

namespace Dmm.WeChat
{
    public enum ShareResultType
    {
        TaskCode = 1,

        AwardCode = 2
    }

    public class WxShareResult
    {
        public const int Success = 0;

        public const int ErrCommon = -1;

        public const int UserCancel = -2;

        public const int SendFail = -3;

        public int Res;

        public string ErrMsg;

        /// <summary>
        /// jeson 类型，包括code 和 type，awardcode 和 taskcode
        /// type = 1 taskcode
        /// type = 2 awardcode
        /// </summary> 
        public string Content;
    }

    [Serializable]
    public class ShareContent
    {
        public ShareContent(ShareResultType type, string content)
        {
            Type = (int) type;

            Content = content;
        }

        public int Type;

        public string Content;
    }
}