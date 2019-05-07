using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Res;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Game
{
    /// <summary>
    /// 玩家对象。
    /// </summary>
    public class PlayerFigure : MonoBehaviour
    {
        #region Inject

        private IDataContainer<List<Commodity>> _commodityList;

        [Inject]
        public void Initialize(IDataRepository dataRepository)
        {
            _commodityList = dataRepository.GetContainer<List<Commodity>>(DataKey.CommodityList);
        }

        #endregion

        #region Default Sprite

        public Sprite DefBoyHead;

        public Sprite DefGirlHead;

        public Sprite DefBoyBody;

        public Sprite DefGirlBody;

        public Sprite DefBoyHair;

        public Sprite DefGirlHair;

        public Sprite DefGirlHairBg;

        #endregion

        #region Head

        public Image HeadImage;

        public void SetHead(int sex)
        {
            if (!HeadImage)
            {
                return;
            }

            // head与其他不同，不需要异步载入。
            if (!HeadImage.gameObject.activeSelf)
            {
                HeadImage.gameObject.SetActive(true);
            }

            var headSprite = sex == 1 ? DefBoyHead : DefGirlHead;

            HeadImage.sprite = headSprite;
            HeadImage.SetNativeSize();
        }

        #endregion

        #region Body

        public AsyncImage BodyImage;

        public void SetBody(int sex, string body)
        {
            if (!BodyImage)
            {
                return;
            }

            var defBody = sex == 1 ? DefBoyBody : DefGirlBody;
            var commodityList = _commodityList.Read();
            var data = GameUtil.GetCommodity(commodityList, body);

            if (data == null || string.IsNullOrEmpty(data.pic))
            {
                BodyImage.SetSprite(defBody, true);
                return;
            }

            BodyImage.SetTargetPic(data.pic, ResourcePath.CommodityPath, data.pic_url, true, defBody);
        }

        #endregion

        #region Hair

        public AsyncImage HairImage;

        public AsyncImage HairBgImage;

        public void SetHair(int sex, string hair)
        {
            // MyLog.InfoWithFrame(name, string.Format("set hair: {0}, {1}", sex, hair));
            if (!HairImage)
            {
                return;
            }

            var defHair = sex == 1 ? DefBoyHair : DefGirlHair;
            var defHairBg = sex == 1 ? null : DefGirlHairBg;

            var commodityList = _commodityList.Read();
            var data = GameUtil.GetCommodity(commodityList, hair);

            if (data == null || string.IsNullOrEmpty(data.pic))
            {
                HairImage.SetSprite(defHair, true);

                // 设置默认的HairBg。
                if (HairBgImage)
                {
                    if (sex == 1)
                    {
                        HairBgImage.Reset();
                    }
                    else
                    {
                        HairBgImage.SetSprite(defHairBg, true);
                    }
                }
                return;
            }

            HairImage.SetTargetPic(data.pic, ResourcePath.CommodityPath, data.pic_url, true, defHair);

            if (HairBgImage)
            {
                if (!string.IsNullOrEmpty(data.pic_bg))
                {
                    HairBgImage.SetTargetPic(data.pic_bg, ResourcePath.CommodityPath, data.pic_bg_url, true,
                        defHairBg);
                }
                else
                {
                    HairBgImage.Reset();
                }
            }
        }

        #endregion

        #region DeskItem

        public AsyncImage DeskItemImage;

        public void SetDeskItem(string deskItem)
        {
            if (!DeskItemImage)
            {
                return;
            }

            var commodityList = _commodityList.Read();
            var data = GameUtil.GetCommodity(commodityList, deskItem);
            if (data == null || string.IsNullOrEmpty(data.pic))
            {
                DeskItemImage.Reset();
                return;
            }

            DeskItemImage.SetTargetPic(data.pic, ResourcePath.CommodityPath, data.pic_url, true);
        }

        #endregion

        /// <summary>
        /// 清空PlayerFigure的内容。
        /// </summary>
        public void Clear()
        {
            if (BodyImage)
            {
                BodyImage.Reset();
            }

            if (HeadImage)
            {
                HeadImage.sprite = null;
                if (HeadImage.gameObject.activeSelf)
                {
                    HeadImage.gameObject.SetActive(false);
                }
            }

            if (HairImage)
            {
                HairImage.Reset();
            }

            if (HairBgImage)
            {
                HairBgImage.Reset();
            }

            if (DeskItemImage)
            {
                DeskItemImage.Reset();
            }
        }

        /// <summary>
        /// 切换PlayerFigure的内容。
        /// </summary>
        /// <param name="data"></param>
        public void ApplyData(User data)
        {
            if (data == null)
            {
                Clear();
                return;
            }

            SetHead(data.sex);
            SetBody(data.sex, data.body);
            SetHair(data.sex, data.hair);

            string deskItem = null;
            if (data.item_show != null &&
                data.item_show.Count > 0)
            {
                deskItem = data.item_show[0];
            }
            SetDeskItem(deskItem);
        }
    }
}