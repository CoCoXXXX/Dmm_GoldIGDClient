using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Res;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class CommodityItem : Item<Commodity>
    {
        #region Inject

        private IDataContainer<Bag> _bag;

        private IDataContainer<User> _user;

        [Inject]
        public void Initialize(IDataRepository dataRepository)
        {
            _bag = dataRepository.GetContainer<Bag>(DataKey.MyBag);
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
        }

        public class Factory : Factory<CommodityItem>
        {
        }

        #endregion

        public Button Button;

        public Image SelectCover;

        public AsyncImage Image;

        public AsyncImage BgImage;

        public GameObject StateGroup;

        public Text StateText;

        public GameObject UnlockTag;

        public GameObject VIPTag;

        private Commodity _data;

        public override Commodity GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, Commodity data)
        {
            if (data == null)
            {
                return;
            }

            _data = data;

            if (SelectCover && !SelectCover.gameObject.activeSelf)
            {
                SelectCover.gameObject.SetActive(false);
            }

            var pic = data.pic;
            if (Image)
            {
                if (!string.IsNullOrEmpty(pic))
                {
                    Image.SetTargetPic(pic, ResourcePath.CommodityPath, data.pic_url, true);
                }
                else
                {
                    Image.Reset();
                }
            }

            if (BgImage)
            {
                var picBg = data.pic_bg;
                if (!string.IsNullOrEmpty(picBg))
                {
                    BgImage.SetTargetPic(picBg, ResourcePath.CommodityPath, data.pic_bg_url, true);
                }
                else
                {
                    BgImage.Reset();
                }
            }

            if (data.vip_level > 0)
            {
                if (!VIPTag.activeSelf)
                    VIPTag.SetActive(true);
            }
            else
            {
                if (VIPTag.activeSelf)
                    VIPTag.SetActive(false);
            }

            var bag = _bag.Read();
            var user = _user.Read();
            if (!GameUtil.HasCommodity(bag, data))
            {
                if (UnlockTag && !UnlockTag.activeSelf)
                    UnlockTag.SetActive(true);

                if (StateGroup && !StateGroup.activeSelf)
                    StateGroup.SetActive(true);

                if (StateText)
                    StateText.text = "" + CommodityHelper.GetPrice(data) +
                                     CurrencyType.LabelOf(CommodityHelper.GetCurrencyType(data));
            }
            else
            {
                if (UnlockTag && UnlockTag.activeSelf)
                    UnlockTag.SetActive(false);

                if (StateGroup && !StateGroup.activeSelf)
                    StateGroup.SetActive(true);

                if (StateText)
                    StateText.text = GameUtil.IsCommodityEquiped(bag, user, data)
                        ? "<color=#4ae91a>当前装饰</color>"
                        : "<color=#f0aa4b>已购买</color>";
            }
        }

        public override void Reset(int currentIndex)
        {
            if (SelectCover && SelectCover.gameObject.activeSelf)
                SelectCover.gameObject.SetActive(false);

            /*
            if (Image)
                Image.Reset();

            if (BgImage)
                BgImage.Reset();
                */

            if (StateGroup && StateGroup.activeSelf)
                StateGroup.SetActive(false);
        }

        public override void Select(bool selected)
        {
            if (SelectCover && SelectCover.gameObject.activeSelf != selected)
                SelectCover.gameObject.SetActive(selected);
        }

        public override Button GetClickButton()
        {
            return Button;
        }
    }
}