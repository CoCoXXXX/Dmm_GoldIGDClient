using UnityEngine;

namespace Dmm.Data
{
    /// <summary>
    /// 客户端的版本信息。
    /// </summary>
    public class ClientConfig : ScriptableObject
    {
        /// <summary>
        /// 产品
        /// </summary>
        public ProductConfig Product;

        /// <summary>
        /// 版本号。
        /// </summary>
        public int ClientVersion;

        /// <summary>
        /// 显示的客户端版本号
        /// </summary>
        public string VersionTxt;

        /// <summary>
        /// 渠道。
        /// </summary>
        public string SaleChannel;

        /// <summary>
        /// 是否是小米模式。
        /// </summary>
        public bool XiaoMiMode;

        /// <summary>
        /// 平台。
        /// </summary>
        public int Platform;
    }
}