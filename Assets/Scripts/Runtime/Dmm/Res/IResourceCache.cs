using UnityEngine;

namespace Dmm.Res
{
    public interface IResourceCache
    {
        /// <summary>
        /// 载入Sprite从本地文件.
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        Sprite LoadSpriteFromLocalFile(string spriteName);

        /// <summary>
        /// 从Cache载入文件
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        Sprite LoadSpriteFromCache(string spriteName);

        void AddSpriteToCache(string spriteName, Sprite sprite);

        /// <summary>
        /// 返回指定的图片是否已经下载成功了。
        /// </summary>
        /// <param name="fileName"></param> 
        /// <returns></returns>
        bool IsSpriteDownloaded(string fileName);

        /// <summary>
        /// 是否包含目标文件的下载任务。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool ContainsDownloadTask(string fileName);

        /// <summary>
        /// 开始下载文件。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="finishHandler"></param>
        /// <param name="timeoutHandler"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        DownloadTask StartDownload(
            string fileName,
            string url,
            ContentType contentType,
            FinishHandler finishHandler = null,
            TimeoutHandler timeoutHandler = null,
            float timeout = 60);

        /// <summary>
        /// 返回当前有多少个下载任务。
        /// </summary>
        /// <returns></returns>
        int DownloadTaskCount();

        /// <summary>
        /// 停止当前所有的下载任务。
        /// </summary>
        void StopAllDownloadTask();
    }

    public delegate void FinishHandler(DownloadTask task);

    public delegate void TimeoutHandler(DownloadTask task);

    /// <summary>
    /// 下载的内容类型。
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// 图片。
        /// 目前只支持PNG。
        /// </summary>
        Image,

        /// <summary>
        /// 文本。
        /// </summary>
        Text,

        /// <summary>
        /// 二进制流。
        /// </summary>
        Bytes
    }

    /// <summary>
    /// 下载任务。
    /// </summary>
    public class DownloadTask
    {
        /// <summary>
        /// 下载的文件名称。
        /// 每个下载任务都以文件名称为索引。
        /// </summary>
        public readonly string FileName;

        public readonly string Url;

        public readonly ContentType Type;

        public readonly FinishHandler FinishHandler;

        public readonly float Timeout;

        public readonly TimeoutHandler TimeoutHandler;

        public WWW Downloader { get; private set; }

        public float StartTime { get; private set; }

        public bool Started { get; private set; }

        public bool Finished { get; private set; }

        public float Progress
        {
            get
            {
                if (Downloader == null)
                    return 0;

                if (Finished)
                    return 1;

                return Downloader.progress;
            }
        }

        public bool IsDone
        {
            get
            {
                if (Downloader == null)
                    return false;

                return Downloader.isDone;
            }
        }

        public DownloadTask(
            string fileName,
            string url,
            ContentType type,
            FinishHandler finishHandler,
            TimeoutHandler timeoutHandler,
            float timeout)
        {
            FileName = fileName;
            Url = url;
            Type = type;
            Timeout = timeout;
            FinishHandler = finishHandler;
            TimeoutHandler = timeoutHandler;

            Started = false;
            Finished = false;
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(Url)) return;

            Downloader = new WWW(Url);
            StartTime = Time.time;

            Started = true;
            Finished = false;
        }

        public void Finish()
        {
            if (FinishHandler != null)
                FinishHandler(this);

            if (Downloader != null)
            {
                Downloader.Dispose();
                Downloader = null;
            }

            Finished = true;
        }

        public void Cancel()
        {
            if (Downloader != null)
            {
                Downloader.Dispose();
                Downloader = null;
            }

            Started = false;
        }

        protected bool Equals(DownloadTask other)
        {
            return string.Equals(FileName, other.FileName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DownloadTask) obj);
        }

        public override int GetHashCode()
        {
            return (FileName != null ? FileName.GetHashCode() : 0);
        }
    }
}