using System.Collections.Generic;
using UnityEngine;

namespace Dmm.Data
{
    ///<summary>
    /// 产品相关的属性配置。
    ///</summary>
    public class ProductConfig : ScriptableObject
    {
        /// <summary>
        /// 产品的名称。
        /// </summary>
        [Header("产品名")] public string ProductName;

        /// <summary>
        /// 产品的显示名称。
        /// </summary>
        [Header("显示名称")] public string DisplayName;

        /// <summary>
        /// AppStore中的AppId。
        /// </summary>
        [Header("AppStore中的AppId")] public string AppId;

        [Header("微信的AppId")] public string WxAppId;

        [Header("Android平台友盟的AppKey")] public string AndroidUmAppKey;

        [Header("IOS平台友盟的AppKey")] public string IOSUmAppKey;

        [Header("Entitlement文件")] public string EntitlementFile;

        [Header("KeyChain的AccessGroup")] public string KeyChainGroup;

        [Header("Universal Links")] public List<string> UniversalLinks;
        
        [Header("AnySDK AppKey")] public string AnySDKAppKey;
        
        [Header("AnySDK AppSecret")] public string AnySDKAppSecret;
        
        [Header("AnySDK PrivateKey")] public string AnySDKPrivateKey;
        
        [Header("AnySDK OauthLoginServer")] public string OauthLoginServer;

        #region WebService
        
        [Header("实名认证的Url")] public string RealNameUrl;

        [Header("提交反馈的Url")] public string IssueSubmitUrl;

        [Header("查看反馈记录的Url")] public string IssueHistoryUrl;

        [Header("举报玩家的Url")] public string ReportUrl;

        [Header("比赛详情的Url")] public string RaceDescriptionUrl;

        [Header("比赛历史排行的Url")] public string RaceHistoryRankUrl;
        
        [Header("Test实名认证的Url")] public string TestRealNameUrl;
        
        [Header("Test提交反馈的Url")] public string TestIssueSubmitUrl;

        [Header("Test查看反馈记录的Url")] public string TestIssueHistoryUrl;

        [Header("Test举报玩家的Url")] public string TestReportUrl;

        [Header("Test比赛详情的Url")] public string TestRaceDescriptionUrl;

        [Header("Test比赛历史排行的Url")] public string TestRaceHistoryRankUrl;
        
        #endregion
    }
}