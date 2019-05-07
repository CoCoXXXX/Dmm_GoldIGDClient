using System;
using UnityEngine;

namespace Dmm.Util
{
    /// <summary>
    /// 保存玩家的属性。
    /// </summary>
    public class PrefsUtil
    {
        public const string Tag = "PrefsUtil";

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static string GetString(string key, string defValue)
        {
            return PlayerPrefs.GetString(key, defValue);
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static int GetInt(string key, int defValue)
        {
            return PlayerPrefs.GetInt(key, defValue);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static float GetFloat(string key, float defValue)
        {
            return PlayerPrefs.GetFloat(key, defValue);
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static bool GetBool(string key, bool defValue)
        {
            return PlayerPrefs.GetInt(key, defValue ? 1 : 0) == 1;
        }

        public static void SetLong(string key, long value)
        {
            var str = Convert.ToString(value);
            PlayerPrefs.SetString(key, str);
        }

        public static long GetLong(string key, long defValue)
        {
            var strValue = PlayerPrefs.GetString(key, null);
            if (string.IsNullOrEmpty(strValue))
                return defValue;

            try
            {
                return Convert.ToInt64(strValue);
            }
            catch (Exception e)
            {
                return defValue;
            }
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public static void Flush()
        {
            PlayerPrefs.Save();
        }
    }
}