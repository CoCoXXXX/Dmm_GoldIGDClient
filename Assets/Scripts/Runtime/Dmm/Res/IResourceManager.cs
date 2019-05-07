using System;

namespace Dmm.Res
{
    /// <summary>
    /// 负责所有资源的载入。
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// 获取FirstBuildAssetBundle 的Key
        /// </summary>
        /// <returns></returns>
        string GetAssetBundleInitializedKey();

        /// <summary>
        /// 如果需要的情况下初始化资源。
        /// </summary>
        bool InitiateIfNeeded();

        /// <summary>
        /// 开始从CDN下载Assetbundle 到unity cache
        /// </summary>
        void StartDownloadResource();

        /// <summary>
        /// 开始从cache加载assetbundle资源
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="resourceName"></param>
        void StartLoadResource(string resourcePath, string resourceName);

        /// <summary>
        /// 异步载入资源。
        /// </summary>
        /// <param name="resourcePath">
        /// 资源的路径，相当于命名空间。
        /// 如果资源在AssetBundle内，则是AssetBundle的名称。
        /// </param>
        /// <param name="resourceName">资源的名称</param>
        /// <param name="completeListener">资源载入完成监听器</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T LoadResrouceAsync<T>(string resourcePath, string resourceName, Action<T> completeListener);

        /// <summary>
        /// 从已经加载好的assetbundle字典中取出对应的assetbundle，并生成资源
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="resourceName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetResource<T>(string resourcePath, string resourceName) where T : UnityEngine.Object;

        /// <summary>
        /// AssetBundle是否缓存
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        bool HasCached(string resourcePath);
    }
}