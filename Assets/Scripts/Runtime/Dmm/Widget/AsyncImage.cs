using Dmm.Common;
using Dmm.Log;
using Dmm.Res;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Widget
{
    public class AsyncImage : MonoBehaviour
    {
        #region Inject

        private IResourceManager _resource;

        private IResourceCache _resourceCache;

        [Inject]
        public void Initialize(IResourceManager resource, IResourceCache resourceCache)
        {
            _resource = resource;
            _resourceCache = resourceCache;
        }

        #endregion

        #region Status

        public const int StatusIdle = 0;

        public const int StatusChecking = 1;

        public const int StatusComplete = 2;

        /// <summary>
        /// 当前的状态。
        /// </summary>
        public int Status { get; private set; }

        /// <summary>
        /// 切换到新的状态。
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="keepContent"></param>
        private void SwitchToStatus(int newStatus, bool keepContent = false)
        {
            // MyLog.InfoWithFrame(name, string.Format("switch to status: {0}", newStatus));
            Status = newStatus;

            switch (newStatus)
            {
                case StatusIdle:
                    // idle状态下ContentImage和WaitingImage都需要关闭。
                    if (ContentImage && !keepContent && ContentImage.gameObject.activeSelf)
                        ContentImage.gameObject.SetActive(false);
                    if (WaitingImage && WaitingImage.gameObject.activeSelf)
                        WaitingImage.gameObject.SetActive(false);
                    break;

                case StatusChecking:
                    // Checking状态下，隐藏ContentImage，显示WaitingImage。
                    if (ContentImage && !keepContent && ContentImage.gameObject.activeSelf)
                        ContentImage.gameObject.SetActive(false);
                    if (WaitingImage && !WaitingImage.gameObject.activeSelf)
                        WaitingImage.gameObject.SetActive(true);
                    break;

                case StatusComplete:
                    // 在Complete状态县，显示ContentImage，隐藏WaitingImage。
                    if (ContentImage && !keepContent && !ContentImage.gameObject.activeSelf)
                        ContentImage.gameObject.SetActive(true);
                    if (WaitingImage && WaitingImage.gameObject.activeSelf)
                        WaitingImage.gameObject.SetActive(false);

                    if (_onCompleteListener != null)
                        _onCompleteListener();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Image组件。
        /// </summary>
        public Image ContentImage;

        /// <summary>
        /// 载入过程中显示的等待菊花。
        /// </summary>
        public Image WaitingImage;

        /// <summary>
        /// 等待菊花旋转一圈所用的时间。
        /// </summary>
        public float WaitingRotateTime = 2f;

        /// <summary>
        /// 两次检查之间的间隔时间。
        /// </summary>
        private float _checkInterval = 0.1f;

        /// <summary>
        /// 内容图片的高度。
        /// </summary>
        public float ContentHeight
        {
            get
            {
                if (!ContentImage)
                {
                    return 0;
                }

                return ContentImage.rectTransform.rect.height;
            }
        }

        public float ContentWidth
        {
            get
            {
                if (!ContentImage)
                {
                    return 0;
                }

                return ContentImage.rectTransform.rect.width;
            }
        }

        /// <summary>
        /// 图片的名称。
        /// </summary>
        public string PicName { get; private set; }

        /// <summary>
        /// 图片在Resources文件夹下的路径。
        /// </summary>
        public string ResourcePath { get; private set; }

        /// <summary>
        /// 图片获取成功之后，是否要设置成NativeSize。
        /// </summary>
        public bool NativeSize { get; private set; }

        private bool _downLoadByUrl;

        public delegate void OnComplete();

        private OnComplete _onCompleteListener;

        /// <summary>
        /// 设置目标图片  从 cache中找 -> 用initSprite 赋值 -> 从Assetbundle中找 —> 从url下载
        /// </summary>
        /// <param name="picName"></param>
        /// <param name="resourcePath"></param>
        /// <param name="url"></param>
        /// <param name="nativeSize"></param>
        /// <param name="initSprite"></param>
        /// <param name="onCompleteListener"></param>
        /// <returns></returns>
        public bool SetTargetPic(
            string picName,
            string resourcePath,
            string url = null,
            bool nativeSize = false,
            Sprite initSprite = null,
            OnComplete onCompleteListener = null)
        {
            PicName = picName;
            ResourcePath = resourcePath;
            NativeSize = nativeSize;
            _onCompleteListener = onCompleteListener;

            LastCheckTime = 0;

            if (!ContentImage)
            {
                // 如果没有内容图片，则直接切换到完成状态。
                SwitchToStatus(StatusComplete);
                return true;
            }

            //先从Cache中取图片
            var sprite = _resourceCache.LoadSpriteFromCache(PicName);
            if (sprite)
            {
                ContentImage.sprite = sprite;
                if (nativeSize)
                {
                    ContentImage.SetNativeSize();
                }

                SwitchToStatus(StatusComplete);
                return true;
            }

            if (initSprite != null)
            {
                if (ContentImage)
                {
                    ContentImage.sprite = initSprite;
                    if (nativeSize)
                    {
                        ContentImage.SetNativeSize();
                    }

                    if (!ContentImage.gameObject.activeSelf)
                    {
                        ContentImage.gameObject.SetActive(true);
                    }
                }

                if (WaitingImage && WaitingImage.gameObject.activeSelf)
                {
                    WaitingImage.gameObject.SetActive(false);
                }

                SwitchToStatus(StatusComplete);
                return true;
            }

            SwitchToStatus(StatusChecking, initSprite != null);

            if (!string.IsNullOrEmpty(ResourcePath))
            {
                _downLoadByUrl = false;
                _resource.StartLoadResource(ResourcePath, PicName);
            }
            else
            {
                if (!string.IsNullOrEmpty(url))
                {
                    _downLoadByUrl = true;
                    if (!_resourceCache.ContainsDownloadTask(picName))
                    {
                        _resourceCache.StartDownload(picName, url, ContentType.Image);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 已经有Sprite的情况下，直接设置sprite。
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="nativeSize"></param>
        public void SetSprite(Sprite sprite, bool nativeSize = false)
        {
            if (!sprite)
            {
                return;
            }

            PicName = sprite.name;
            NativeSize = nativeSize;
            LastCheckTime = 0;

            if (ContentImage)
            {
                ContentImage.sprite = sprite;
                if (nativeSize)
                {
                    ContentImage.SetNativeSize();
                }
            }

            SwitchToStatus(StatusComplete);
        }

        /// <summary>
        /// 重置参数。
        /// </summary>
        public void Reset()
        {
            PicName = null;
            NativeSize = false;

            LastCheckTime = 0;
            SwitchToStatus(StatusIdle);
        }

        /// <summary>
        /// 最后一次检查的时间。
        /// </summary>
        public float LastCheckTime { get; private set; }

        public void Update()
        {
            if (Status != StatusChecking)
                return;

            // 更新WaitingImage：
            if (WaitingImage)
            {
                var tr = WaitingImage.transform;
                var curAngle = tr.rotation.eulerAngles.z;
                var destAngle = curAngle - (360f / WaitingRotateTime) * Time.deltaTime;
                if (destAngle < 0) destAngle += 360f;
                tr.rotation = Quaternion.Euler(0, 0, destAngle);
            }

            // 检查图片是否已经存在。
            if (!ContentImage)
            {
                return;
            }

            if (Time.time - LastCheckTime < _checkInterval)
            {
                return;
            }

            LastCheckTime = Time.time;

            Sprite sprite;
            if (_downLoadByUrl)
            {
                sprite = _resourceCache.LoadSpriteFromLocalFile(PicName);
            }
            else
            {
                if (string.IsNullOrEmpty(ResourcePath))
                {
                    MyLog.ErrorWithFrame(name,"从图片名找不到Assetbundle :"+PicName);
                    return;
                }
                
                sprite = _resource.GetResource<Sprite>(ResourcePath, PicName);
                if (sprite)
                {
                    _resourceCache.AddSpriteToCache(PicName, sprite);
                }
            }

            if (sprite)
            {
                ContentImage.sprite = sprite;
                if (NativeSize)
                    ContentImage.SetNativeSize();

                SwitchToStatus(StatusComplete);
            }
        }
    }
}