using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Dmm.Util;

namespace Dmm.Data
{
    /// <summary>
    /// 玩家的登陆记录。
    /// </summary>
    public class LoginRecord
    {
        #region 将登陆属性保存到PlayerPrefs

        #region 昵称

        // 目前用于保存第三方登陆账号的昵称。
        private const string NicknameKey = "LoginNickname";

        public static string LastNickname
        {
            get { return PrefsUtil.GetString(NicknameKey, ""); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    PrefsUtil.SetString(NicknameKey, value);
            }
        }

        #endregion

        #region 用户名

        private const string UsernameKey = "LoginUsername";

        /// <summary>
        /// 最后一次登陆使用的用户名。
        /// </summary>
        public static string LastUsername
        {
            get
            {
                string username = null;
                username = PrefsUtil.GetString(UsernameKey, "");
                if (string.IsNullOrEmpty(username))
                {
                    // 兼容旧版本的记录。
                    username = PrefsUtil.GetString("CurrentUsername", "");
                }

                return username;
            }

            set { PrefsUtil.SetString(UsernameKey, value); }
        }

        public const string VisitorIdKey = "LoginVisitor";

        public static string LastVisitorId
        {
            get { return PrefsUtil.GetString(VisitorIdKey, null); }
            set { PrefsUtil.SetString(VisitorIdKey, value); }
        }

        public const string VisitorUsernameKey = "LoginVisitorUsername";

        public static string LastVisitorUsername
        {
            get { return PrefsUtil.GetString(VisitorUsernameKey, null); }
            set { PrefsUtil.SetString(VisitorUsernameKey, value); }
        }

        #endregion

        #region 密码

        private const string PasswordKey = "LoginPassword";

        private const string EncriptionKey = "qazsedcft";

        public static string LastPassword
        {
            get
            {
                var encrypt = PrefsUtil.GetString(PasswordKey, "");
                if (string.IsNullOrEmpty(encrypt))
                    return "";

                return DesDecrypt(encrypt, EncriptionKey);
            }

            set
            {
                var encrypt = DesEncrypt(value, EncriptionKey);
                if (!string.IsNullOrEmpty(encrypt))
                {
                    PrefsUtil.SetString(PasswordKey, encrypt);
                }
            }
        }

        #endregion

        #region 登陆Token

        private const string TokenKey = "LoginToken";

        public static string Token
        {
            get { return PrefsUtil.GetString(TokenKey, ""); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    PrefsUtil.SetString(TokenKey, value);
            }
        }

        #endregion

        #region 玩家当前的登陆类型

        public static int CurrentLoginType;

        #endregion

        #region 最后一次登陆的用户类型

        public const int NoLogin = -1;

        public const int NormalUser = 0;

        public const int Visitor = 1;

        public const int XiaoMi = 10;

        public const int Wechat = 100;

        public const string LoginTypeKey = "LoginUserType";

        public static int LastLoginType
        {
            get { return PrefsUtil.GetInt(LoginTypeKey, NoLogin); }
            set { PrefsUtil.SetInt(LoginTypeKey, value); }
        }

        #endregion

        /// <summary>
        /// 保存所有的属性。
        /// </summary>
        public static void SaveAll()
        {
            PrefsUtil.Flush();
        }

        #endregion

        #region 加密算法

        private static readonly byte[] IV = new byte[8]
        {
            0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF
        };

        /// <summary>
        /// Des加密。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DesEncrypt(string str, string key)
        {
            try
            {
                var byKey = GetKeyData(key, 8);
                var des = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);

                var inputByteArray = Encoding.UTF8.GetBytes(str);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                var result = Convert.ToBase64String(ms.ToArray());

                cs.Close();
                ms.Close();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Des解密。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DesDecrypt(string str, string key)
        {
            try
            {
                var byKey = GetKeyData(key, 8);

                var des = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);

                var inputByteArray = Convert.FromBase64String(str);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                var result = Encoding.UTF8.GetString(ms.ToArray());

                cs.Close();
                ms.Close();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static byte[] GetKeyData(string key, int n)
        {
            var result = new byte[n];
            var tmp = Encoding.UTF8.GetBytes(key);

            if (tmp.Length < n)
            {
                Array.Copy(tmp, 0, result, 0, tmp.Length);
                for (int i = tmp.Length; i < n; i++)
                    result[i] = 0;
            }
            else
            {
                Array.Copy(tmp, 0, result, 0, n);
            }

            return result;
        }

        #endregion

        #region WechatLogin

        /// <summary>
        /// 玩家上一次使用微信登陆时候获得的openId。
        /// </summary>
        public const string WechatOpenId = "wechatOpenId";

        /// <summary>
        /// 玩家上一次登陆时候获得的authCode。
        /// </summary>
        public const string WechatAuthCode = "wechatAuthCode";

        public static string GetOpenId()
        {
            return PrefsUtil.GetString(WechatOpenId, null);
        }

        public static void SaveOpenId(string openId)
        {
            if (string.IsNullOrEmpty(openId))
            {
                PrefsUtil.DeleteKey(WechatOpenId);
            }
            else
            {
                PrefsUtil.SetString(WechatOpenId, openId);
            }
            PrefsUtil.Flush();
        }

        public static void RemoveOpenId()
        {
            PrefsUtil.DeleteKey(WechatOpenId);
            PrefsUtil.Flush();
        }

        public static string GetAuthCode()
        {
            return PrefsUtil.GetString(WechatAuthCode, null);
        }

        public static void SaveAuthCode(string authCode)
        {
            if (string.IsNullOrEmpty(authCode))
            {
                PrefsUtil.DeleteKey(WechatAuthCode);
            }
            else
            {
                PrefsUtil.SetString(WechatAuthCode, authCode);
            }
            PrefsUtil.Flush();
        }

        public static void RemoveAuthCode()
        {
            PrefsUtil.DeleteKey(WechatAuthCode);
            PrefsUtil.Flush();
        }

        #endregion

        #region 玩家暂不更新日期

        public const string DontUpdateDateKey = "dontUpdateDate";

        public static string DontUpdateDate
        {
            get { return PrefsUtil.GetString(DontUpdateDateKey, ""); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    PrefsUtil.SetString(DontUpdateDateKey, value);
                }

                PrefsUtil.Flush();
            }
        }

        #endregion
    }
}