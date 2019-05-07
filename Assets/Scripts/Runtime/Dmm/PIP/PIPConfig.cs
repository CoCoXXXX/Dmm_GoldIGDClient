using System.Collections.Generic;
using UnityEngine;

namespace Dmm.PIP
{
    [CreateAssetMenu(menuName = "Dmm/PIPConfig")]
    public class PIPConfig : ScriptableObject
    {
        #region 默认PServer地址

        /// <summary>
        /// 默认的Host。
        /// </summary>
        public string DefaultHost;

        /// <summary>
        /// 默认的端口。
        /// </summary>
        public int DefaultPort;

        #endregion

        #region PIP下载配置

        /// <summary>
        /// 下载特殊渠道的PIP的时候的超时时间。
        /// </summary>
        public float Timeout1 = 5;

        /// <summary>
        /// 下载PIP.All.txt的时候的超时时间。
        /// </summary>
        public float Timeout2 = 10;

        /// <summary>
        /// 针对每个PIP的地址重试的次数。
        /// </summary>
        public int PIPRetryCount = 3;

        /// <summary>
        /// PIP列表。
        /// </summary>
        public List<string> PIPList;

        #endregion

        #region 测试

        public bool Test;

        public string TestHost;

        public int TestPort;

        #endregion
    }
}