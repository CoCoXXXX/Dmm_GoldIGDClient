using System.Collections.Generic;
using Dmm.Data;
using Dmm.Log;
using UnityEngine;
using Zenject;

namespace Dmm.Sdk
{
    public class AndroidSDK
    {
        #region Inject

        private string _package = "com.tencent.tmgp.ttigd";
        [Inject]
        public void Initialize(ConfigHolder configHolder)
        {
#if UNITY_ANDROID
            _wechatLogic = new AndroidJavaClass(_package + ".weixin.WechatLogic");
//          _alipay = new AndroidJavaClass(_package + ".alipay.AlipayLogic");
            _wxSocial = new AndroidJavaClass(_package + ".weixin.WxShare");
            _apkInstaller = new AndroidJavaClass(_package + ".apk.ApkInstaller");

//            if (_configHolder.XiaoMiMode)
//                // 在非小米版本打包的时候，是没有MiLogic类的。
//                _miLogic = new AndroidJavaClass(_package + "xiaomi.MiLogic");
            _clipboardLogic = new AndroidJavaClass(_package + ".clipboard.ClipboardLogic");
#endif
        }

        #endregion

        #region Context

#if UNITY_ANDROID

        private AndroidJavaClass _alipay;

        private AndroidJavaClass _wxSocial;

        private AndroidJavaClass _apkInstaller;

        private AndroidJavaClass _miLogic;

        private AndroidJavaClass _wechatLogic;

        private AndroidJavaClass _clipboardLogic;

#endif

        #endregion

        #region 小米

        public void MiInit()
        {
#if UNITY_ANDROID //            if (_miLogic != null) _miLogic.CallStatic("init");
#endif
        }

        public void MiLogin()
        {
#if UNITY_ANDROID //            if (_miLogic != null) _miLogic.CallStatic("login");
#endif
        }

        public void MiPay(string outTradeNo, string username, int miBi)
        {
#if UNITY_ANDROID //            if (_miLogic != null) _miLogic.CallStatic("pay", outTradeNo, username, miBi);
#endif
        }

        #endregion

        #region 安装Apk

        public void InstallApk(string filePath)
        {
#if UNITY_ANDROID
            if (_apkInstaller != null) _apkInstaller.CallStatic("installApk", filePath);
#endif
        }

        #endregion

        #region 支付宝

        public void AlipayAndroid(string order)
        {
#if UNITY_ANDROID //            if (_alipay != null) _alipay.CallStatic("pay", order);
#endif
        }

        #endregion

        #region 微信支付

        public void WxPayAndroid(string order)
        {
#if UNITY_ANDROID
            if (_wechatLogic != null) _wechatLogic.CallStatic("pay", order);
#endif
        }

        #endregion

        #region 微信分享 

        public void WxShare(string url, string wxImgUrl, string wxImgPath, string wxTitle, string wxContent,
            string thumbUrl, string content = null)
        {
#if UNITY_ANDROID
            MyLog.InfoWithFrame("Android SDK", "微信分享 url：" + url);
            if (_wxSocial != null)
                _wxSocial.CallStatic("share", url, wxImgUrl, wxImgPath, wxTitle, wxContent, thumbUrl, content);
#endif
        }

        public void WxCircle(string url, string wxImgUrl, string wxImgPath, string wxTitle, string wxContent,
            string thumbUrl, string content = null)
        {
#if UNITY_ANDROID
            MyLog.InfoWithFrame("Android SDK", "微信分享朋友圈 url：" + url);
            if (_wxSocial != null)
                _wxSocial.CallStatic("circle", url, wxImgUrl, wxImgPath, wxTitle, wxContent, thumbUrl, content);
#endif
        }

        /// <summary>
        /// 直接邀请别人玩爱掼蛋。
        /// </summary>
        public void WxIgdInvite()
        {
#if UNITY_ANDROID
            if (_wxSocial != null) _wxSocial.CallStatic("igdInvite");
#endif
        }

        #endregion


#if UNITY_ANDROID
        private AndroidJavaObject ToJavaHashMap(Dictionary<string, string> dic)
        {
            var hashMap = new AndroidJavaObject("java.util.HashMap");
            var putMethod =
                AndroidJNIHelper.GetMethodID(hashMap.GetRawClass(), "put",
                    "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
            var arguments = new object[2];
            foreach (var entry in dic)
            {
                using (var key = new AndroidJavaObject("java.lang.String", entry.Key))
                {
                    using (var val = new AndroidJavaObject("java.lang.String", entry.Value))
                    {
                        arguments[0] = key;
                        arguments[1] = val;
                        AndroidJNI.CallObjectMethod(hashMap.GetRawObject(), putMethod,
                            AndroidJNIHelper.CreateJNIArgArray(arguments));
                    }
                }
            }

            return hashMap;
        }
#endif

        public void WxInit(string appId)
        {
#if UNITY_ANDROID
            if (_wechatLogic != null)
            {
                _wechatLogic.CallStatic("init", appId);
            }
#endif
        }

        public bool IsWechatInstalled()
        {
#if UNITY_ANDROID
            bool isWeixinAvailable = false;
            if (_wechatLogic != null)
            {
                isWeixinAvailable = _wechatLogic.CallStatic<bool>("isWeixinAvailable");
            }
            return isWeixinAvailable;
#endif
            return false;
        }

        public void WxAuth(string deviceId)
        {
#if UNITY_ANDROID //微信授权
            if (_wechatLogic != null)
                _wechatLogic.CallStatic("auth", deviceId);
#endif
        }

        #region Clipboard

        /// <summary>
        /// 复制到粘贴板
        /// </summary>
        /// <param name="input"></param>
        public void CopyToClipboard(string input)
        {
#if UNITY_ANDROID
            if (_clipboardLogic != null)
                _clipboardLogic.CallStatic("CopyTextToClipboard", input);
#endif
        }

        #endregion
    }
}