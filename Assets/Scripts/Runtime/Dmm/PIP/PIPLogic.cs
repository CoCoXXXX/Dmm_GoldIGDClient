using System;
using System.Collections;
using System.Text;
using Dmm.Data;
using Dmm.Log;
using Dmm.Util;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Dmm.PIP
{
    /// <summary>
    /// 下载PIP的逻辑。
    /// </summary>
    public class PIPLogic : MonoBehaviour, IPIPLogic
    {
        #region Inject

        private ConfigHolder _configHolder;

        [Inject]
        public void Initialize(ConfigHolder configHolder)
        {
            _configHolder = configHolder;
        }

        #endregion

        #region Config

        /// <summary>
        /// PIP配置。
        /// </summary>
        public PIPConfig PIPConfig;

        private bool Test
        {
            get
            {
                if (PIPConfig == null)
                {
                    return false;
                }

                return PIPConfig.Test;
            }
        }

        private string TestHost
        {
            get
            {
                if (PIPConfig == null)
                {
                    return null;
                }

                return PIPConfig.TestHost;
            }
        }

        private int TestPort
        {
            get
            {
                if (PIPConfig == null)
                {
                    return 0;
                }

                return PIPConfig.TestPort;
            }
        }

        public bool IsTest()
        {
            return Test;
        }

        #endregion

        #region PIP结果

        private PIPData _pipData;

        public PIPData GetPIPData()
        {
            return _pipData;
        }

        public string GetHost()
        {
            if (Test)
            {
                return TestHost;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.Host;
        }

        public int GetPort()
        {
            if (Test)
            {
                return TestPort;
            }

            if (_pipData == null)
            {
                return 0;
            }

            return _pipData.Port;
        }

        public bool EnableIpV6()
        {
            if (Test)
            {
                return false;
            }

            if (_pipData == null)
            {
                return false;
            }

            return _pipData.IpV6;
        }

        public int GetNewVersion()
        {
            if (Test)
            {
                return _configHolder.ClientVersion;
            }

            if (_pipData == null)
            {
                return 0;
            }

            return _pipData.NewVersion;
        }

        public string GetIosUrl()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.IosUrl;
        }

        public string GetAndroidUrl()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.AndroidUrl;
        }

        public bool GetForceUpdate()
        {
            if (Test)
            {
                return false;
            }

            if (_pipData == null)
            {
                return false;
            }

            return _pipData.ForceUpdate;
        }

        public string GetDescription()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.Description;
        }

        public bool ReplaceWS()
        {
            if (Test)
            {
                return false;
            }

            if (_pipData == null)
            {
                return false;
            }

            return _pipData.ReplaceWS;
        }

        public string GetRealNameUrl()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.RealNameUrl;
        }

        public string GetIssueSubmitUrl()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.IssueSubmitUrl;
        }

        public string GetIssueHistoryUrl()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.IssueHistoryUrl;
        }

        public string GetReportUrl()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.ReportUrl;
        }

        public string GetRaceDescriptionUrl()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.RaceDescriptionUrl;
        }

        public string GetRaceHistoryRankUrl()
        {
            if (Test)
            {
                return null;
            }

            if (_pipData == null)
            {
                return null;
            }

            return _pipData.RaceHistoryRankUrl;
        }

        #endregion

        #region 下载PIP

        private int _currentStatus;

        public int GetCurrentStatus()
        {
            if (Test)
            {
                return PIPStatus.Success;
            }

            return _currentStatus;
        }

        /// <summary>
        /// 开始下载PIP。
        /// </summary>
        public void StartDownloadPIP()
        {
            if (Test)
            {
                MyLog.WarnWithFrame(name, string.Format("Using test pip address:{0}:{1}", TestHost, TestPort));
                return;
            }

            StartCoroutine(FetchingPIPCoroutine());
        }

        /// <summary>
        /// 开始下载PIP。
        /// </summary>
        private IEnumerator FetchingPIPCoroutine()
        {
            if (PIPConfig == null || PIPConfig.PIPList == null || PIPConfig.PIPList.Count <= 0)
            {
                _currentStatus = PIPStatus.NoPIP;
                MyLog.ErrorWithFrame(name, "PIP address empty!");
                yield break;
            }

            _currentStatus = PIPStatus.Downloading;

            var product = _configHolder.Product;
            var saleChannel = _configHolder.SaleChannel;
            var clientVersion = _configHolder.ClientVersion;

            // 如果渠道不存在则使用All。
            if (string.IsNullOrEmpty(saleChannel))
            {
                saleChannel = "All";
            }

            foreach (var pip in PIPConfig.PIPList)
            {
                MyLog.InfoWithFrame(name, string.Format("Start fetching pip from: {0}", pip));

                for (int i = 0; i < PIPConfig.PIPRetryCount; i++)
                {
                    MyLog.InfoWithFrame(name, string.Format("Try {0}:", i + 1));

                    // 1、第一次尝试下载产品+渠道+版本的特殊的PIP地址。
                    yield return StartCoroutine(
                        DownloadPIP(
                            string.Format("{0}/PIP.{1}.{2}.{3}.txt?mcachenum={4}",
                                pip,
                                product,
                                saleChannel,
                                clientVersion,
                                DateTime.Now.ToFileTime()),
                            PIPConfig.Timeout1)
                    );

                    // 如果下载成功，则不需要再继续下载了。
                    if (_currentStatus == PIPStatus.Success)
                    {
                        yield break;
                    }

                    // 2、第二次尝试下载产品+渠道的特殊的PIP地址。
                    yield return
                        StartCoroutine(
                            DownloadPIP(
                                string.Format("{0}/PIP.{1}.{2}.txt?mcachenum={3}",
                                    pip,
                                    product,
                                    saleChannel,
                                    DateTime.Now.ToFileTime()),
                                PIPConfig.Timeout1)
                        );

                    // 如果下载成功，则不需要再继续下载了。
                    if (_currentStatus == PIPStatus.Success)
                    {
                        yield break;
                    }

                    // 3、第三次尝试下载产品通用的PIP地址。
                    yield return
                        StartCoroutine(
                            DownloadPIP(
                                string.Format("{0}/PIP.{1}.All.txt?mcachenum={2}",
                                    pip,
                                    product,
                                    DateTime.Now.ToFileTime()),
                                PIPConfig.Timeout2)
                        );

                    if (_currentStatus == PIPStatus.Success)
                    {
                        yield break;
                    }

                    // 4、第三次尝试下载产品通用的PIP地址。
                    yield return
                        StartCoroutine(
                            DownloadPIP(
                                string.Format("{0}/PIP.All.txt?mcachenum={1}",
                                    pip,
                                    DateTime.Now.ToFileTime()),
                                PIPConfig.Timeout2)
                        );

                    if (_currentStatus == PIPStatus.Success)
                    {
                        yield break;
                    }
                }
            }

            // 整个PIPList都没有下载成功，此时应该获取上一次系统中保存的PIP列表。
            var host = GetLastHost();
            var port = GetLastPort();

            if (ValidatePIP(host, port))
            {
                _pipData = new PIPData
                {
                    Host = host,
                    Port = port
                };
                _currentStatus = PIPStatus.Success;
                MyLog.InfoWithFrame(name, string.Format("WWW completely failed, use last pip: {0}:{1}.", host, port));
                yield break;
            }

            // 上一次登陆的地址也获取失败，此时只能使用默认的地址。
            host = PIPConfig.DefaultHost;
            port = PIPConfig.DefaultPort;
            if (ValidatePIP(host, port))
            {
                _pipData = new PIPData
                {
                    Host = host,
                    Port = port
                };
                _currentStatus = PIPStatus.Success;
                MyLog.InfoWithFrame(name,
                    string.Format("Last pip dose not exists, use default pip: {0}:{1}.", host, port));
            }
            else
            {
                // 默认地址也无效，则只能失败了。
                _currentStatus = PIPStatus.Fail;
                MyLog.InfoWithFrame(name, "No default pip, PIP failed!!!!!!!!!!");
            }
        }

        /// <summary>
        /// 下载单个PIP文件。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private IEnumerator DownloadPIP(string url, float timeout)
        {
            MyLog.InfoWithFrame(name,
                string.Format("Start download pip. url: {0}, timeout: {1} seconds.", url, timeout));

            var startTime = Time.time;
            var www = UnityWebRequest.Get(url);
            www.SendWebRequest();

            var isTimeout = false;
            while (!www.isDone)
            {
                if (Time.time - startTime >= timeout)
                {
                    isTimeout = true;
                    MyLog.InfoWithFrame(name, "Download pip failed. Timeout!");
                    break;
                }

                // 一帧检查一次下载是否超时了。
                yield return null;
            }

            if (!isTimeout)
            {
                if (!www.isNetworkError && !www.isHttpError)
                {
                    // 正常下载完成，且未发生错误，解析文本。
                    try
                    {
                        var bytes = www.downloadHandler.data;
                        var content = Encoding.UTF8.GetString(bytes);

                        var pipData = ParsePIP(content);
                        if (pipData != null &&
                            ValidatePIP(pipData.Host, pipData.Port))
                        {
                            // 主机地址和端口正确的情况下，认为下载PIP成功。
                            _pipData = pipData;

                            SavePIP(_pipData.Host, _pipData.Port);
                            _currentStatus = PIPStatus.Success;

                            MyLog.InfoWithFrame(
                                name,
                                string.Format(
                                    "PIP success! Host: {0}, Port: {1}",
                                    _pipData.Host, _pipData.Port
                                )
                            );
                        }
                    }
                    catch (Exception e)
                    {
                        MyLog.ErrorWithFrame(name, "Decoding pip failed: " + e.Message);
                    }
                }
                else
                {
                    MyLog.ErrorWithFrame(name, string.Format("Download pip fail: {0}", www.error));
                }
            }

            www.Dispose();
        }

        #endregion

        #region 保存PIP结果

        /// <summary>
        /// 记录在PlayerPrefs中的最后一次登陆的地址。
        /// </summary>
        public const string LastHostKey = "LastHost";

        /// <summary>
        /// 记录在PlayerPrefs中的最后一次登陆的端口。
        /// </summary>
        public const string LastPortKey = "LastPort";

        /// <summary>
        /// 获取上一次登陆时候使用的地址。
        /// </summary>
        /// <returns></returns>
        private string GetLastHost()
        {
            return PrefsUtil.GetString(LastHostKey, PIPConfig.DefaultHost);
        }

        /// <summary>
        /// 获取上一次登陆时候使用的端口。
        /// </summary>
        /// <returns></returns>
        private int GetLastPort()
        {
            return PrefsUtil.GetInt(LastPortKey, PIPConfig.DefaultPort);
        }

        /// <summary>
        /// 将host和port保存到PlayerPrefs中。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        private void SavePIP(string host, int port)
        {
            PrefsUtil.SetString(LastHostKey, host);
            PrefsUtil.SetInt(LastPortKey, port);
            PrefsUtil.Flush();
        }

        /// <summary>
        /// 检查获取的PIP数据是否有效。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private bool ValidatePIP(string host, int port)
        {
            if (string.IsNullOrEmpty(host))
            {
                return false;
            }

            if (port <= 0)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 解析PIP

        /// <summary>
        /// 解析PIP的json文本。
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private PIPData ParsePIP(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            try
            {
                return JsonUtility.FromJson<PIPData>(content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}