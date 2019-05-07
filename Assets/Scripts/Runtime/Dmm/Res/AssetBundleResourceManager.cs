using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dmm.App;
using Dmm.Constant;
using Dmm.DataContainer;
using Dmm.Log;
using Dmm.StateLogic;
using Dmm.Util;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Dmm.Res
{
    public class AssetBundleResourceManager : MonoBehaviour, IResourceManager
    {
        private IAppContext _context;

        private IDataContainer<BuildFirstCacheResult> _buildFirstCacheResult;

        private IDataContainer<DownloadResourceResult> _downloadResourceResult;

        private IDataContainer<DownloadAssetBundleInfo> _downloadAssetBundleInfo;

        [Inject]
        public void Inject(IAppContext context, IDataRepository dataRepository)
        {
            _context = context;
            _buildFirstCacheResult = dataRepository.GetContainer<BuildFirstCacheResult>(DataKey.BuildFirstCacheResult);
            _downloadResourceResult =
                dataRepository.GetContainer<DownloadResourceResult>(DataKey.DownloadResourceResult);
            _downloadAssetBundleInfo =
                dataRepository.GetContainer<DownloadAssetBundleInfo>(DataKey.DownloadAssetBundleInfo);
        }

        public string GetAssetBundleInitializedKey()
        {
            var configHolder = _context.GetConfigHolder();
            var clientVersion = configHolder.ClientVersion;
            var initAssetKey = PrefsKeys.AssetBundleInitializedKey + clientVersion;

            return initAssetKey;
        }

        public bool InitiateIfNeeded()
        {
            var initAssetKey = GetAssetBundleInitializedKey();
            var initialized = PrefsUtil.GetInt(initAssetKey, 0);
            if (initialized != 0)
            {
                SetBuildFirstCacheResult(BuildFirstCacheResult.Ok, "");
                return false;
            }

            StartCoroutine(BuildFirstCache());
            return true;
        }

        public string StreamingAssetsPath
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_IOS
                return "file://" + Application.streamingAssetsPath + "/";
#else
            return  Application.streamingAssetsPath + "/";
#endif
            }
        }

        private IEnumerator BuildFirstCache()
        {
            var config = _context.GetConfigHolder();
            var resVersion = config.ResourceVersion;

            using (var manifestReq = UnityWebRequest.GetAssetBundle(
                Path.Combine(StreamingAssetsPath, "StreamingAssets"),
                (uint) resVersion,
                (uint) 0))
            {
                manifestReq.SendWebRequest();
                while (!manifestReq.isDone)
                {
                    yield return null;
                }

                if (manifestReq.isNetworkError || manifestReq.isHttpError)
                {
                    MyLog.ErrorWithFrame(name,
                        "加manifest失败   error " +
                        manifestReq.error + "  manifestPath = " + Path.Combine(StreamingAssetsPath, "StreamingAssets"));

                    SetBuildFirstCacheResult(BuildFirstCacheResult.Error, string.Format("初始化资源列表失败，请重新登陆\n【错误码{0}】）",
                        NetworkStateErrorCode.BuildFirstCacheFailCode));
                    yield break;
                }

                var manifestBundle = DownloadHandlerAssetBundle.GetContent(manifestReq);
                if (manifestBundle == null)
                {
                    MyLog.ErrorWithFrame(name,
                        "  manifestBundle ==  null manifestPath = " +
                        Path.Combine(StreamingAssetsPath, "StreamingAssets"));
                    SetBuildFirstCacheResult(BuildFirstCacheResult.Error, string.Format("获取初始化资源失败，请重新登陆\n【错误码{0}】）",
                        NetworkStateErrorCode.BuildFirstCacheFailCode));
                    yield break;
                }

                var manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest == null)
                {
                    MyLog.ErrorWithFrame(name,
                        "  manifest =null   " + Path.Combine(StreamingAssetsPath, "StreamingAssets"));

                    SetBuildFirstCacheResult(BuildFirstCacheResult.Error, string.Format("获取初始化资源失败，请重新登陆\n【错误码{0}】）",
                        NetworkStateErrorCode.BuildFirstCacheFailCode));
                    yield break;
                }

                var allAssetBundles = manifest.GetAllAssetBundles();
                if (allAssetBundles == null || allAssetBundles.Length <= 0)
                {
                    MyLog.ErrorWithFrame(name,
                        "allAssetBundles == null   manifest = " + Path.Combine(StreamingAssetsPath, "StreamingAssets"));

                    SetBuildFirstCacheResult(BuildFirstCacheResult.Error, string.Format("获取初始化资源失败，请重新登陆\n【错误码{0}】）",
                        NetworkStateErrorCode.BuildFirstCacheFailCode));
                    yield break;
                }

                var downloadAssetBundleInfo = new DownloadAssetBundleInfo();
                downloadAssetBundleInfo.LoadType = DownloadAssetBundleInfo.DownloadType.FirstBuild;
                downloadAssetBundleInfo.TotalCount = allAssetBundles.Length;
                downloadAssetBundleInfo.CompleteCount = 0;

                foreach (var abName in allAssetBundles)
                {
                    if (string.IsNullOrEmpty(abName))
                    {
                        continue;
                    }

                    _downloadAssetBundleInfo.Write(downloadAssetBundleInfo, Time.time);
                    using (var bundleReq = UnityWebRequest.GetAssetBundle(
                        Path.Combine(StreamingAssetsPath, abName),
                        (uint) resVersion,
                        (uint) 0))
                    {
                        bundleReq.SendWebRequest();
                        while (!bundleReq.isDone)
                        {
                            yield return null;
                        }

                        if (bundleReq.isNetworkError || bundleReq.isHttpError)
                        {
                            MyLog.ErrorWithFrame(name, abName + " upload to cache fail ");
                            SetBuildFirstCacheResult(BuildFirstCacheResult.Error, string.Format(
                                "下载获取初始化资源失败，请重新登陆\n【错误码{0}】）",
                                NetworkStateErrorCode.BuildFirstCacheFailCode));

                            yield break;
                        }
                        downloadAssetBundleInfo.CompleteCount++;
                    }
                }

                yield return null;
                _downloadAssetBundleInfo.ClearAndInvalidate(Time.time);
                MyLog.InfoWithFrame(name, " upload all resource succ >>>>>>>>>>>>> UnloadAllAssetBundles >>>>>>>>>>>");
                SetBuildFirstCacheResult(BuildFirstCacheResult.Ok, "");
                AssetBundle.UnloadAllAssetBundles(false);
            }
        }

        private void SetBuildFirstCacheResult(int result, string error)
        {
            var buildFirstCacheResult = new BuildFirstCacheResult();
            buildFirstCacheResult.result = result;
            buildFirstCacheResult.error = error;

            _buildFirstCacheResult.Write(buildFirstCacheResult, Time.time);
        }

        public void StartDownloadResource()
        {
            StartCoroutine(DownloadResource());
        }

        private IEnumerator DownloadResource()
        {
            var pip = _context.GetPIPLogic();

            if (pip.IsTest())
            {
                SetDownloadResourceResult(DownloadResourceResult.Ok, "");
                yield break;
            }
            var data = pip.GetPIPData();

            if (data == null)
            {
                SetDownloadResourceResult(DownloadResourceResult.Error, string.Format(
                    "获取下载资源失败，请重新登陆\n【错误码{0}】）", NetworkStateErrorCode.DownloadResourcesFailCode));
                yield break;
            }

            if (data.Assets != null && data.Assets.Length > 0)
            {
                var downloadAssetBundleInfo = new DownloadAssetBundleInfo();
                downloadAssetBundleInfo.LoadType = DownloadAssetBundleInfo.DownloadType.Download;
                downloadAssetBundleInfo.TotalCount = data.Assets.Length;
                downloadAssetBundleInfo.CompleteCount = 0;

                foreach (var asset in data.Assets)
                {
                    if (asset == null)
                    {
                        continue;
                    }

                    //已下载过相同版本
                    if (GetLastAssetVersion(asset.Asset) != null &&
                        GetLastAssetVersion(asset.Asset).Version == asset.Version)
                    {
                        continue;
                    }

                    _downloadAssetBundleInfo.Write(downloadAssetBundleInfo, Time.time);

                    var req = UnityWebRequest.GetAssetBundle(asset.Url, (uint) asset.Version, (uint) 0);

                    req.SendWebRequest();

                    while (!req.isDone)
                    {
                        yield return null;
                    }

                    if (req.isNetworkError || req.isHttpError)
                    {
                        MyLog.ErrorWithFrame(name, asset.Url + " download  fail ");
                        SetDownloadResourceResult(DownloadResourceResult.Error, string.Format(
                            "下载资源失败，请重新登陆\n【错误码{0}】）", NetworkStateErrorCode.DownloadResourcesFailCode));

                        yield break;
                    }

                    try
                    {
                        var assetJson = JsonUtility.ToJson(asset);
                        PrefsUtil.SetString(asset.Asset, assetJson);
                        PrefsUtil.Flush();

                        downloadAssetBundleInfo.CompleteCount++;

                        //只卸载没有被cache的临时的assetbundle
                        if (!HasCached(asset.Asset))
                        {
                            var assetBundle = DownloadHandlerAssetBundle.GetContent(req);
                            if (assetBundle)
                            {
                                assetBundle.Unload(false);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MyLog.ErrorWithFrame(name, asset.Url + " download  fail with error :" + e);
                        continue;
                    }
                }
            }

            yield return null;
            _downloadAssetBundleInfo.ClearAndInvalidate(Time.time);
            SetDownloadResourceResult(DownloadResourceResult.Ok, "");
            MyLog.InfoWithFrame(name,
                " download all resource succ  >>>>>>>>>>>>>>>>> UnloadAllAssetBundles >>>>>>>>>>>");
        }

        private void SetDownloadResourceResult(int result, string error)
        {
            var downloadResourceResult = new DownloadResourceResult();
            downloadResourceResult.result = result;
            downloadResourceResult.error = error;

            _downloadResourceResult.Write(downloadResourceResult, Time.time);
        }

        private AssetBundleManifest _manifest;

        /// <summary>
        /// 当key存在value为null时，说明资源已经开始下载，还没有下载完
        /// </summary>
        private readonly Dictionary<string, AssetBundle> _assetBundleCache = new Dictionary<string, AssetBundle>();

        public bool HasCached(string resourcePath)
        {
            return _assetBundleCache.ContainsKey(resourcePath) && _assetBundleCache[resourcePath] != null;
        }

        public void StartLoadResource(string resourcePath, string resourceName)
        {
            StartCoroutine(LoadResource(resourcePath, resourceName));
        }

        private IEnumerator LoadResource(string resourcePath, string resourceName)
        {
            if (string.IsNullOrEmpty(resourcePath) || string.IsNullOrEmpty(resourceName))
            {
                MyLog.ErrorWithFrame(name,
                    "加载资源失败 resourcePath = " + resourcePath + "  resourceName = " + resourceName);
                yield break;
            }

            //避免同一个assetbundle 同一时间多次下载
            if (_assetBundleCache.ContainsKey(resourcePath))
            {
                yield break;
            }

            _assetBundleCache.Add(resourcePath, null);

            MyLog.InfoWithFrame(name, "加载资源 ：" + resourcePath);

            var config = _context.GetConfigHolder();

            var baseVersion = config.ResourceVersion;

            if (_manifest == null)
            {
                var lastManifestAssetVersion = GetLastAssetVersion("StreamingAssets");
                var manifestPath = lastManifestAssetVersion == null
                    ? Path.Combine(StreamingAssetsPath, "StreamingAssets")
                    : lastManifestAssetVersion.Url;
                var manifestVersion = lastManifestAssetVersion == null
                    ? baseVersion
                    : lastManifestAssetVersion.Version;

                using (var manifestReq = UnityWebRequest.GetAssetBundle(
                    manifestPath,
                    (uint) manifestVersion,
                    (uint) 0))
                {
                    manifestReq.SendWebRequest();

                    while (!manifestReq.isDone)
                    {
                        yield return null;
                    }

                    if (manifestReq.isNetworkError || manifestReq.isHttpError)
                    {
                        MyLog.ErrorWithFrame(name,
                            "加manifest失败 resourcePath = " + resourcePath + "  resourceName = " + resourceName +
                            " error " +
                            manifestReq.error + "  manifestPath = " + manifestPath);
                        yield break;
                    }

                    var manifestBundle = DownloadHandlerAssetBundle.GetContent(manifestReq);
                    if (manifestBundle == null)
                    {
                        MyLog.ErrorWithFrame(name,
                            "加载manifest失败 resourcePath = " + resourcePath + "  resourceName = " + resourceName +
                            "  manifestPath = " + manifestPath);
                        yield break;
                    }

                    var manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                    manifestBundle.Unload(false);

                    if (manifest == null)
                    {
                        MyLog.ErrorWithFrame(name,
                            "加载manifest失败 resourcePath = " + resourcePath + "   resourceName = " + resourceName +
                            "  manifest == null" + "  manifestPath = " + manifestPath);
                        yield break;
                    }
                    _manifest = manifest;
                }
            }

            var dependencies = _manifest.GetAllDependencies(resourcePath);
            if (dependencies != null && dependencies.Length > 0)
            {
                foreach (var dep in dependencies)
                {
                    if (string.IsNullOrEmpty(dep))
                    {
                        continue;
                    }

                    if (_assetBundleCache.ContainsKey(dep))
                    {
                        while (_assetBundleCache[dep] == null)
                        {
                            yield return null;
                        }

                        continue;
                    }

                    _assetBundleCache.Add(dep, null);

                    var lastDepAssetVersion = GetLastAssetVersion(dep);
                    var depPath = lastDepAssetVersion == null
                        ? Path.Combine(StreamingAssetsPath, dep)
                        : lastDepAssetVersion.Url;
                    var depVersion = lastDepAssetVersion == null
                        ? baseVersion
                        : lastDepAssetVersion.Version;

                    using (var depReq = UnityWebRequest.GetAssetBundle(
                        depPath,
                        (uint) depVersion,
                        (uint) 0))
                    {
                        depReq.SendWebRequest();

                        while (!depReq.isDone)
                        {
                            yield return null;
                        }

                        if (depReq.isNetworkError || depReq.isHttpError)
                        {
                            MyLog.ErrorWithFrame(name,
                                "加载依赖资源失败 resourcePath = " + resourcePath + "resourceName = " + resourceName +
                                "  " +
                                depReq.error);
                            yield break;
                        }

                        var depBundle = DownloadHandlerAssetBundle.GetContent(depReq);
                        if (depBundle == null)
                        {
                            MyLog.ErrorWithFrame(name,
                                "加载依赖资源失败 resourcePath = " + resourcePath + "resourceName = " + resourceName);
                            yield break;
                        }

                        if (_assetBundleCache.ContainsKey(dep))
                        {
                            _assetBundleCache[dep] = depBundle;
                        }
                        else
                        {
                            _assetBundleCache.Add(dep, depBundle);
                        }
                    }
                }
            }

            var lastAssetVersion = GetLastAssetVersion(resourcePath);
            var path = lastAssetVersion == null
                ? Path.Combine(StreamingAssetsPath, resourcePath)
                : lastAssetVersion.Url;
            var version = lastAssetVersion == null
                ? baseVersion
                : lastAssetVersion.Version;

            using (var abReq = UnityWebRequest.GetAssetBundle(
                path,
                (uint) version,
                (uint) 0))
            {
                abReq.SendWebRequest();
                while (!abReq.isDone)
                {
                    yield return null;
                }

                if (abReq.isNetworkError || abReq.isHttpError)
                {
                    MyLog.ErrorWithFrame(name,
                        "加载资源失败 resourcePath = " + resourcePath + " resourceName = " + resourceName +
                        "  " +
                        abReq.error);
                    yield break;
                }

                var assetBundle = DownloadHandlerAssetBundle.GetContent(abReq);
                if (assetBundle == null)
                {
                    MyLog.ErrorWithFrame(name,
                        "加载资源失败 resourcePath = " + resourcePath + " resourceName = " + resourceName);
                    yield break;
                }

                if (_assetBundleCache.ContainsKey(resourcePath))
                {
                    _assetBundleCache[resourcePath] = assetBundle;
                }
                else
                {
                    _assetBundleCache.Add(resourcePath, assetBundle);
                }
            }
        }

        private AssetVersion GetLastAssetVersion(string resourcePath)
        {
            var lastAssetVersion = PrefsUtil.GetString(resourcePath, "");
            if (string.IsNullOrEmpty(lastAssetVersion))
            {
                return null;
            }

            try
            {
                var assetVersion = JsonUtility.FromJson<AssetVersion>(lastAssetVersion);

                return assetVersion;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public T GetResource<T>(string resourcePath, string resourceName) where T : UnityEngine.Object
        {
            AssetBundle asset;
            if (!_assetBundleCache.ContainsKey(resourcePath))
            {
                return default(T);
            }

            if (_assetBundleCache[resourcePath] == null)
            {
                return default(T);
            }

            asset = _assetBundleCache[resourcePath];
            var ab = asset.LoadAsset<T>(resourceName);

            if (ab == null)
            {
                MyLog.ErrorWithFrame(name, " AssetBundle :" + resourcePath + " 中不存在资源：" + resourceName);
            }
            return ab;
        }

        public T LoadResrouceAsync<T>(string resourcePath, string resourceName, Action<T> completeListener)
        {
            return default(T);
        }
    }
}