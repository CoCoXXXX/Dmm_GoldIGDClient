using System;
using System.Collections.Generic;
using System.IO;
using Dmm.Constant;
using Dmm.Log;
using UnityEngine;
using Zenject;

namespace Dmm.Res
{
    public class ResourceCache : MonoBehaviour, IResourceCache
    {
        #region Inject

        private IFilePicManager _filePicManager;

        [Inject]
        public void Initialize(IFilePicManager filePicManager)
        {
            _filePicManager = filePicManager;
        }

        #endregion

        #region loadSprite

        private readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        public void AddSpriteToCache(string spriteName, Sprite sprite)
        {
            if (_cache.ContainsKey(spriteName))
            {
                return;
            }

            if (sprite)
            {
                _cache.Add(spriteName, sprite);
            }
        }

        public Sprite LoadSpriteFromCache(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            if (_cache.ContainsKey(spriteName))
            {
                return _cache[spriteName];
            }

            return null;
        }

        /// <summary>
        /// 从本地文件加载Sprite
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite LoadSpriteFromLocalFile(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return null;
            }

            if (_cache.ContainsKey(spriteName))
            {
                return _cache[spriteName];
            }

            var sprite = _filePicManager.GetSprite(spriteName);
            if (sprite)
            {
                _cache.Add(spriteName, sprite);
            }

            return sprite;
        }

        public bool IsSpriteDownloaded(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
                return false;

            return _filePicManager.ContainsSprite(spriteName);
        }

        #endregion

        #region download

        private readonly Queue<DownloadTask> _taskQueue = new Queue<DownloadTask>();

        /// <summary>
        /// 当前执行的下载任务。
        /// </summary>
        private DownloadTask _currentTask;

        public bool ContainsDownloadTask(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            if (_currentTask != null &&
                _currentTask.FileName == fileName)
                return true;

            lock (_taskQueue)
            {
                foreach (var t in _taskQueue)
                {
                    if (t.FileName == fileName)
                        return true;
                }

                return false;
            }
        }

        public DownloadTask StartDownload(
            string fileName,
            string url,
            ContentType contentType,
            FinishHandler finishHandler,
            TimeoutHandler timeoutHandler = null,
            float timeout = 60)
        {
            if (string.IsNullOrEmpty(fileName) ||
                string.IsNullOrEmpty(url))
                return null;

            if (_currentTask != null &&
                _currentTask.FileName == fileName)
                return _currentTask;

            lock (_taskQueue)
            {
                foreach (var t in _taskQueue)
                {
                    if (t.FileName == fileName)
                        return t;
                }

                var task = new DownloadTask(
                    fileName,
                    url,
                    contentType,
                    finishHandler,
                    timeoutHandler,
                    timeout);

                _taskQueue.Enqueue(task);
                MyLog.InfoWithFrame(name, string.Format("Start download: {0}", fileName));
                return task;
            }
        }

        public int DownloadTaskCount()
        {
            var count = _currentTask != null ? 1 : 0;
            lock (_taskQueue)
            {
                return _taskQueue.Count + count;
            }
        }

        public void StopAllDownloadTask()
        {
            if (_currentTask != null)
            {
                _currentTask.Cancel();
                _currentTask = null;
            }

            lock (_taskQueue)
            {
                while (_taskQueue.Count > 0)
                {
                    var t = _taskQueue.Dequeue();
                    t.Cancel();
                }

                _taskQueue.Clear();
            }
        }

        public void Update()
        {
            if (_currentTask != null)
            {
                if (_currentTask.IsDone)
                {
                    // 任务完成了。
                    DownloadFinish(_currentTask);
                    _currentTask.Finish();

                    _currentTask = null;
                }
                else if (_currentTask.StartTime + _currentTask.Timeout <= Time.time)
                {
                    //  任务超时了。
                    if (_currentTask.TimeoutHandler != null)
                    {
                        _currentTask.Finish();
                        _currentTask.TimeoutHandler(_currentTask);
                    }

                    _currentTask = null;
                }
            }

            if (_currentTask == null && _taskQueue.Count > 0)
            {
                lock (_taskQueue)
                {
                    _currentTask = _taskQueue.Dequeue();
                }

                _currentTask.Start();
            }
        }

        private void DownloadFinish(DownloadTask task)
        {
            if (task == null)
                return;

            var www = task.Downloader;
            if (www == null)
                return;

            if (!string.IsNullOrEmpty(www.error))
            {
                MyLog.ErrorWithFrame(name, string.Format("Download failed! url: {0}, error: {1}", task.Url, www.error));
                return;
            }

            try
            {
                var type = task.Type;
                if (type == ContentType.Image)
                {
                    // 将下载的图片保存到路径中。
                    if (!www.texture)
                        return;

                    if (string.IsNullOrEmpty(task.FileName))
                        return;

                    var bytes = www.texture.EncodeToPNG();
                    _filePicManager.SavePic(bytes, task.FileName);
                }
                else if (type == ContentType.Bytes)
                {
                    if (!Directory.Exists(FilePath.BinaryFilePath()))
                        Directory.CreateDirectory(FilePath.BinaryFilePath());

                    var path = FilePath.BinaryFilePath() + task.FileName;
                    File.WriteAllBytes(path, www.bytes);
                }
                else if (type == ContentType.Text)
                {
                    if (!Directory.Exists(FilePath.TextPath()))
                        Directory.CreateDirectory(FilePath.TextPath());

                    var path = FilePath.TextPath() + task.FileName;
                    File.WriteAllBytes(path, www.bytes);
                }

                MyLog.InfoWithFrame(name, string.Format("Finish download: {0}", task.FileName));
            }
            catch (Exception e)
            {
                MyLog.ErrorWithFrame(name, e.Message);
            }
        }

        #endregion
    }
}