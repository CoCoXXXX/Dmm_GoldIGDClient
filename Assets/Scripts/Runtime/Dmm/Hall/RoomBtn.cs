using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Res;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Hall
{
    /// <summary>
    /// 房间按钮。
    /// </summary>
    public class RoomBtn : Item<Room>
    {
        #region Inject

        private IResourceCache _resourceCache;

        [Inject]
        public void Initialize(IResourceCache resourceCache)
        {
            _resourceCache = resourceCache;
        }

        public class Factory : Factory<RoomBtn>
        {
        }

        #endregion

        /// <summary>
        /// 房间标签
        /// </summary>
        public GameObject TagContainer;

        /// <summary>
        /// 房间背景和文字显示
        /// </summary>
        public GameObject RoomContainer;

        /// <summary>
        /// 房间图片。
        /// </summary>
        public AsyncImage RoomPic;

        /// <summary>
        /// 在线人数。
        /// </summary>
        public Text PlayerCount;

        /// <summary>
        /// 底注。
        /// </summary>
        public CurrencyValue BaseMoney;

        /// <summary>
        /// 标签1组。
        /// </summary>
        public GameObject Tag1Group;

        /// <summary>
        /// 标签1。
        /// </summary>
        public Text Tag1;

        /// <summary>
        /// 标签2组。
        /// </summary>
        public GameObject Tag2Group;

        /// <summary>
        /// 标签2。
        /// </summary>
        public Text Tag2;

        /// <summary>
        /// 按钮。
        /// </summary>
        public Button Button;

        /// <summary>
        /// 房间ID。
        /// </summary>
        public long RoomId
        {
            get
            {
                if (_data == null)
                {
                    return -1;
                }

                return _data.room_id;
            }
        }

        /// <summary>
        /// 房间数据。
        /// </summary>
        private Room _data;

        public void UpdatePlayerCount(int count)
        {
            PlayerCount.text = "" + count;
        }

        public override Room GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, Room data)
        {
            _data = data;
            if (data == null)
            {
                // 如果没有数据的话，应该怎么显示房间按钮呢？
                // 房间可以不显示，但是是不是要显示在线人数。
                return;
            }

            // 载入人数和底注。
            BaseMoney.SetCurrency(_data.base_money, CurrencyType.GOLDEN_EGG);
            PlayerCount.text = "" + data.current_player_num;

            // 载入房间图片。
            if (string.IsNullOrEmpty(data.room_pic))
            {
                RoomPic.Reset();
            }
            else
            {
                var resourcePath = GetAssetBundleByPicNameMap.GetAssetBundleName(data.room_pic);
                RoomPic.SetTargetPic(data.room_pic, resourcePath, data.room_pic_url);
            }

            if (data.type == RoomType.Ad)
            {
                RoomContainer.SetActive(false);
                TagContainer.SetActive(false);
                var hintItem = data.hint_item;
                if (hintItem != null && !string.IsNullOrEmpty(hintItem.content_pic))
                {
                    var contentPic = _resourceCache.LoadSpriteFromLocalFile(hintItem.content_pic);
                    if (!contentPic)
                    {
                        if (!string.IsNullOrEmpty(hintItem.content_pic_url) &&
                            !_resourceCache.ContainsDownloadTask(hintItem.content_pic))
                        {
                            _resourceCache.StartDownload(
                                hintItem.content_pic,
                                hintItem.content_pic_url,
                                ContentType.Image);
                        }
                    }
                }
            }
            else
            {
                RoomContainer.SetActive(true);
                TagContainer.SetActive(true);
            }

            // 设置标签。
            if (!string.IsNullOrEmpty(data.tag))
            {
                if (!Tag1Group.activeSelf)
                {
                    Tag1Group.SetActive(true);
                }

                Tag1.text = data.tag;
            }
            else
            {
                if (Tag1Group.activeSelf)
                {
                    Tag1Group.SetActive(false);
                }
            }

            if (!string.IsNullOrEmpty(data.tag1))
            {
                if (!Tag2Group.activeSelf)
                {
                    Tag2Group.SetActive(true);
                }

                Tag2.text = data.tag1;
            }
            else
            {
                if (Tag2Group.activeSelf)
                {
                    Tag2Group.SetActive(false);
                }
            }

            if (!Button.gameObject.activeSelf)
            {
                Button.gameObject.SetActive(true);
            }
        }

        public override void Reset(int currentIndex)
        {
            if (!RoomPic.gameObject.activeSelf)
            {
                RoomPic.gameObject.SetActive(true);
            }

//            RoomPic.Reset();

            if (Tag1Group.activeSelf)
            {
                Tag1Group.SetActive(false);
            }

            if (Tag2Group.activeSelf)
            {
                Tag2Group.SetActive(false);
            }

            if (Button.gameObject.activeSelf)
            {
                Button.gameObject.SetActive(false);
            }

            _data = null;
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return Button;
        }
    }
}