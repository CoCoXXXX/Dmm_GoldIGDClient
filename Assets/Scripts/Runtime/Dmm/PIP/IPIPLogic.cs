namespace Dmm.PIP
{
    public interface IPIPLogic
    {
        void StartDownloadPIP();
        int GetCurrentStatus();

        string GetHost();
        int GetPort();

        bool EnableIpV6();

        int GetNewVersion();
        string GetIosUrl();
        string GetAndroidUrl();
        bool GetForceUpdate();
        string GetDescription();

        bool ReplaceWS();
        string GetRealNameUrl();
        string GetIssueSubmitUrl();
        string GetIssueHistoryUrl();
        string GetReportUrl();
        string GetRaceDescriptionUrl();
        string GetRaceHistoryRankUrl();

        PIPData GetPIPData();
        bool IsTest();
    }

    /// <summary>
    /// PIP的状态。
    /// </summary>
    public class PIPStatus
    {
        /// <summary>
        /// 没有PIP列表。
        /// </summary>
        public const int NoPIP = 0;

        /// <summary>
        /// 正在下载中。
        /// </summary>
        public const int Downloading = 1;

        /// <summary>
        /// 获取PIP成功。
        /// </summary>
        public const int Success = 2;

        /// <summary>
        /// 获取PIP失败。
        /// </summary>
        public const int Fail = 3;
    }
}