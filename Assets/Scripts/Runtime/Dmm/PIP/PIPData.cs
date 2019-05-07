using Dmm.Res;

namespace Dmm.PIP
{
    public class PIPData
    {
        /// <summary>
        /// PServer地址。
        /// </summary>
        public string Host;

        /// <summary>
        /// PServer端口。
        /// </summary>
        public int Port;

        /// <summary>
        /// 是否开启IpV6。
        /// </summary>
        public bool IpV6 = false;

        /// <summary>
        /// 最新版本版本号。
        /// </summary>
        public int NewVersion;

        /// <summary>
        /// ios 更新地址。
        /// </summary>
        public string IosUrl;

        /// <summary>
        /// android 更新地址。
        /// </summary>
        public string AndroidUrl;

        /// <summary>
        /// 是否强制更新。
        /// </summary>
        public bool ForceUpdate;

        /// <summary>
        /// 更新描述
        /// </summary>
        public string Description;

        #region WebService

        /// <summary>
        /// 使用替代的WebService地址。
        /// </summary>
        public bool ReplaceWS;

        public string IssueSubmitUrl;

        public string IssueHistoryUrl;

        public string ReportUrl;

        public string RaceDescriptionUrl;

        public string RaceHistoryRankUrl;

        public string RealNameUrl;

        #endregion

        #region AssetBundle

        /// <summary>
        /// 资源更新列表。
        /// </summary>
        public AssetVersion[] Assets;

        #endregion
    }
}