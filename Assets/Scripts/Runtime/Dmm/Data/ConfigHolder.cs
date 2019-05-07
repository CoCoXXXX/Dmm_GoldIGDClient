using System.Collections.Generic;
using Dmm.Pay;
using Dmm.PIP;
using UnityEngine;
using Zenject;

namespace Dmm.Data
{
    public class ConfigHolder : MonoBehaviour
    {
        #region Inject

        private IPIPLogic _pip;

        [Inject]
        public void Inject(IPIPLogic pipLogic)
        {
            _pip = pipLogic;
        }

        #endregion

        #region ClientConfig

        public ClientConfig ClientConfig;

        public ProductConfig ProductConfig
        {
            get
            {
                if (ClientConfig == null)
                {
                    return null;
                }

                return ClientConfig.Product;
            }
        }

        public string Product
        {
            get
            {
                var productConfig = ProductConfig;
                if (productConfig == null)
                {
                    return null;
                }

                return productConfig.ProductName;
            }
        }

        public string ProductDisplayName
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return "NULL";
                }

                return product.DisplayName;
            }
        }

        public string AppId
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                return product.AppId;
            }
        }

        public string UmAppKey
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }
#if UNITY_EDITOR
                return "";
#elif UNITY_IPHONE
                return product.IOSUmAppKey;
#elif UNITY_ANDROID
                return product.AndroidUmAppKey;
#else
                return "";
#endif
            }
        }

        public string WxAppId
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                return product.WxAppId;
            }
        }

        public string KeyChainGroup
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                return product.KeyChainGroup;
            }
        }

        public int ClientVersion
        {
            get
            {
                if (ClientConfig == null)
                {
                    return 0;
                }

                return ClientConfig.ClientVersion;
            }
        }

        public int ResourceVersion
        {
            get
            {
                var version = ClientVersion;
                if (version <= 0)
                {
                    return int.MaxValue;
                }

                return ClientVersion * 10000;
            }
        }

        public string VersionTxt
        {
            get
            {
                if (ClientConfig == null)
                {
                    return null;
                }

                return ClientConfig.VersionTxt;
            }
        }

        public string SaleChannel
        {
            get
            {
                if (ClientConfig == null)
                    return "NULL";

                return ClientConfig.SaleChannel;
            }
        }

        public int Platform
        {
            get
            {
                if (ClientConfig == null)
                    return Constant.Platform.Default;

                return ClientConfig.Platform;
            }
        }

        public bool XiaoMiMode
        {
            get
            {
                if (ClientConfig == null)
                    return false;

                // 只有在安卓平台上小米模式才开启。
                return ClientConfig.XiaoMiMode && Platform == Constant.Platform.Android;
            }
        }

        public string AnySDKAppKey
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                return product.AnySDKAppKey;
            }
        }

        public string AnySDKAppSecret
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                return product.AnySDKAppSecret;
            }
        }

        public string AnySDKPrivateKey
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                return product.AnySDKPrivateKey;
            }
        }

        /// <summary>
        /// 而 oauthLoginServer 参数是游戏服务提供的用来做登陆验证转发的接口地址，在此处配置的接口地址仅用于 
        /// SIMSDK 测试模式下(即直接运行母包时)做登录时框架请求的地址，而在正式打出渠道包的时候会被替换成相应渠道在打包工具中配置的地址参数。
        /// </summary>
        public string OauthLoginServer
        {
            get
            {
                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                return product.OauthLoginServer;
            }
        }

        #endregion

        #region IapConfig

        public IapConfig IapConfigData;

        public List<string> GetIapProductIdList()
        {
            if (IapConfigData == null)
            {
                return null;
            }

            return IapConfigData.ProductIdList;
        }

        #endregion

        #region WebService
        
        public string RealNameUrl
        {
            get
            {
                if (_pip.ReplaceWS())
                {
                    return _pip.GetRealNameUrl();
                }

                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                if (_pip.IsTest())
                {
                    return product.TestRealNameUrl;
                }
         
                return product.RealNameUrl;
            }
        }
        
        public string IssueSubmitUrl
        {
            get
            {
                if (_pip.ReplaceWS())
                {
                    return _pip.GetIssueSubmitUrl();
                }

                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                if (_pip.IsTest())
                {
                    return product.TestIssueSubmitUrl;
                }

                return product.IssueSubmitUrl;
            }
        }

        public string IssueHistoryUrl
        {
            get
            {
                if (_pip.ReplaceWS())
                {
                    return _pip.GetIssueHistoryUrl();
                }

                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                if (_pip.IsTest())
                {
                    return product.TestIssueHistoryUrl;
                }

                return product.IssueHistoryUrl;
            }
        }

        public string ReportUrl
        {
            get
            {
                if (_pip.ReplaceWS())
                {
                    return _pip.GetReportUrl();
                }

                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                if (_pip.IsTest())
                {
                    return product.TestReportUrl;
                }

                return product.ReportUrl;
            }
        }

        public string RaceDescriptionUrl
        {
            get
            {
                if (_pip.ReplaceWS())
                {
                    return _pip.GetRaceDescriptionUrl();
                }

                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                if (_pip.IsTest())
                {
                    return product.TestRaceDescriptionUrl;
                }

                return product.RaceDescriptionUrl;
            }
        }

        public string RaceHistoryRankUrl
        {
            get
            {
                if (_pip.ReplaceWS())
                {
                    return _pip.GetRaceHistoryRankUrl();
                }

                var product = ProductConfig;
                if (product == null)
                {
                    return null;
                }

                if (_pip.IsTest())
                {
                    return product.TestRaceHistoryRankUrl;
                }

                return product.RaceHistoryRankUrl;
            }
        }

        #endregion
    }
}