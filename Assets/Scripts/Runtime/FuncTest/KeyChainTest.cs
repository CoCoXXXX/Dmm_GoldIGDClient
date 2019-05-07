using System;
using System.Runtime.InteropServices;
using Dmm.Data;
using UnityEngine;
using UnityEngine.UI;

namespace FuncTest
{
    public class KeyChainTest : MonoBehaviour
    {
        public ConfigHolder ConfigHolder;

        public Text Content;

        public void DoRead()
        {
            var t = GetDeviceId();
            if (string.IsNullOrEmpty(t))
            {
                Content.text = "无数据";
            }
            else
            {
                Content.text = "读出：" + t;
            }
        }

        public void DoNew()
        {
            var uuid = Guid.NewGuid().ToString();
            SaveDeviceId(uuid);
            Content.text = "保存新的uuid：" + uuid;
        }

        public void DoReset()
        {
            ResetDeviceId();
        }

        public Text UsernameContent;

        public void DoReadUsername()
        {
            var t = GetUsername();
            if (string.IsNullOrEmpty(t))
            {
                UsernameContent.text = "无数据";
            }
            else
            {
                UsernameContent.text = "读出：" + t;
            }
        }

        public void DoNewUsername()
        {
            var uuid = Guid.NewGuid().ToString();
            SaveUsername(uuid);
            UsernameContent.text = "保存新的uuid：" + uuid;
        }

        public void DoResetUsername()
        {
            ResetUsername();
        }

        #region SDK

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern string getFromKeyChain(string key, string accessGroup);

        [DllImport("__Internal")]
        private static extern bool saveToKeyChain(string key, string content, string accessGroup);

        [DllImport("__Internal")]
        private static extern bool resetKeyChainContent(string key, string accessGroup);
#endif

        public const string DeviceIdKey = "visitor";

        private string GetDeviceId()
        {
#if UNITY_IOS
            return getFromKeyChain(DeviceIdKey, ConfigHolder.KeyChainGroup);
#endif
            return null;
        }

        private bool SaveDeviceId(string deviceId)
        {
#if UNITY_IOS
            return saveToKeyChain(DeviceIdKey, deviceId, ConfigHolder.KeyChainGroup);
#endif
            return false;
        }

        private bool ResetDeviceId()
        {
#if UNITY_IOS
            return resetKeyChainContent(DeviceIdKey, ConfigHolder.KeyChainGroup);
#endif
            return false;
        }

        private const string UsernameKey = "username";

        private string GetUsername()
        {
#if UNITY_IOS
            return getFromKeyChain(UsernameKey, ConfigHolder.KeyChainGroup);
#endif
            return null;
        }

        private bool SaveUsername(string username)
        {
#if UNITY_IOS
            return saveToKeyChain(UsernameKey, username, ConfigHolder.KeyChainGroup);
#endif
            return false;
        }

        private bool ResetUsername()
        {
#if UNITY_IOS
            return resetKeyChainContent(UsernameKey, ConfigHolder.KeyChainGroup);
#endif
            return false;
        }

        #endregion
    }
}