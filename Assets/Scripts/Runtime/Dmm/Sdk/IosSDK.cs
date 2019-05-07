using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dmm.Data;
using UnityEngine;
using Zenject;

namespace Dmm.Sdk
{
    public class IosSDK : MonoBehaviour
    {
        #region Inject

        private ConfigHolder _config;

        [Inject]
        public void Initialize(ConfigHolder config)
        {
            _config = config;
        }

        #endregion

#if UNITY_IOS

        #region util

        [DllImport("__Internal")]
        private static extern string getFromKeyChain(string key, string accessGroup);

        [DllImport("__Internal")]
        private static extern bool saveToKeyChain(string key, string content, string accessGroup);

        [DllImport("__Internal")]
        private static extern bool resetKeyChainContent(string key, string accessGroup);

        [DllImport("__Internal")]
        private static extern void openUrl(string url);

        [DllImport("__Internal")]
        private static extern string getIpV6(string host);

        #endregion

        #region pay

        [DllImport("__Internal")]
        private static extern void wxPay(string order);

        [DllImport("__Internal")]
        private static extern void iapPay(string productId, string outTradeNo);

        [DllImport("__Internal")]
        private static extern void getProductInfo(string[] productIds, int productIdCount);

        #endregion

        #region Wechat

        [DllImport("__Internal")]
        private static extern void wxInit(string appId, string appSecret);

        [DllImport("__Internal")]
        private static extern void wxShare(string url, string imgUrl, string imgPath, string title, string content,
            string thumbUrl, string shareContent, bool isCircle);

        [DllImport("__Internal")]
        private static extern bool wxAuth(string deviceId);

        [DllImport("__Internal")]
        private static extern bool isWechatInstalled();
            
        [DllImport ("__Internal")]
        private static extern void copyTextToClipboard(string input);

        #endregion

#endif

        #region Pay

        public void WxPay(string order)
        {
#if UNITY_IOS
            wxPay(order);
#endif
        }

        public void GetProductInfo(List<string> list)
        {
            if (list == null || list.Count <= 0)
            {
                return;
            }

            var ids = list.ToArray();
#if UNITY_IOS
            getProductInfo(ids, ids.Length);
#endif
        }

        public void IapPay(string productId, string outTradeNo)
        {
#if UNITY_IOS
            iapPay(productId, outTradeNo);
#endif
        }

        #endregion

        #region Weixin

        public void WxShareInit(string appId, string appSecret)
        {
#if UNITY_IOS
            wxInit(appId, appSecret);
#endif
        }

        public void WxShare(string url, string imgUrl, string imgPath, string title, string content,
            string thumbUrl, string shareContent = null)
        {
#if UNITY_IOS
            wxShare(url, imgUrl, imgPath, title, content, thumbUrl, shareContent, false);
#endif
        }

        public void WxCircle(string url, string imgUrl, string imgPath, string title, string content,
            string thumbUrl, string shareContent = null)
        {
#if UNITY_IOS
            wxShare(url, imgUrl, imgPath, title, content, thumbUrl, shareContent, true);
#endif
        }

        #endregion

        #region Util

        public void OpenUrl(string url)
        {
#if UNITY_IOS
            openUrl(url);
#endif
        }

        public const string DeviceIdKey = "visitor";

        public string GetDeviceId()
        {
#if UNITY_IOS
            return getFromKeyChain(DeviceIdKey, _config.KeyChainGroup);
#endif
            return null;
        }

        public bool SaveDeviceId(string deviceId)
        {
#if UNITY_IOS
            return saveToKeyChain(DeviceIdKey, deviceId, _config.KeyChainGroup);
#endif
            return false;
        }

        public bool ResetDeviceId()
        {
#if UNITY_IOS
            return resetKeyChainContent(DeviceIdKey, _config.KeyChainGroup);
#endif
            return false;
        }

        public const string UsernameKey = "username";

        public string GetUsername()
        {
#if UNITY_IOS
            return getFromKeyChain(UsernameKey, _config.KeyChainGroup);
#endif
            return null;
        }

        public bool SaveUsername(string username)
        {
#if UNITY_IOS
            return saveToKeyChain(UsernameKey, username, _config.KeyChainGroup);
#endif
            return false;
        }

        public bool ResetUsername()
        {
#if UNITY_IOS
            return resetKeyChainContent(UsernameKey, _config.KeyChainGroup);
#endif
            return false;
        }

        public string GetIpV6(string host)
        {
#if UNITY_IOS
            return getIpV6(host);
#endif
            return null;
        }

        public bool IsWechatInstalled()
        {
#if UNITY_IOS
            return isWechatInstalled();
#endif
            return false;
        }

        public void WxAuth(string deviceId)
        {
#if UNITY_IOS
            wxAuth(deviceId);
#endif
        }

        public void CopyToClipboard(string input)
        {
#if UNITY_IOS
           copyTextToClipboard(input);
#endif
        }
    }

    #endregion
}