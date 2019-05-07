using System;

namespace Dmm.Res
{
    [Serializable]
    public class AssetVersion
    {
        /// <summary>
        /// 资源的名称。
        /// </summary>
        public string Asset;

        /// <summary>
        /// 当前的版本。
        /// </summary>
        public int Version;

        /// <summary>
        /// 资源的下载地址。
        /// 如果不存在，则使用默认下载地址。
        /// </summary>
        public string Url;
    }
}