using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class PrepaymentItem : Item<Prepayment>
    {
        #region Inject

        private SpriteHolder _spriteHolder;

        [Inject]
        public void Initialize(SpriteHolder spriteHolder)
        {
            _spriteHolder = spriteHolder;
        }

        public class Factory : Factory<PrepaymentItem>
        {
        }

        #endregion

        public GameObject NormalGroup;

        public Image Icon;

        public Image HotTag;

        public Image RecommendTag;

        public GameObject NameGroup;

        public Text Name;

        public Text Description;

        public Text Price;

        public RectTransform TagGroup;

        public AsyncImage PackageImg;

        public Button Button;

        private Prepayment _data;

        private string _hotTag = "热销";

        private string _recommendTag = "推荐";

        public override Prepayment GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, Prepayment data)
        {
            _data = data;

            if (data == null)
            {
                if (NormalGroup.activeSelf)
                    NormalGroup.SetActive(false);

                PackageImg.Reset();
                return;
            }

            if (data.currency_type == CurrencyType.COMPOUND)
            {
                // 礼包类型的支付包。
                if (NormalGroup && NormalGroup.activeSelf)
                    NormalGroup.SetActive(false);

                if (string.IsNullOrEmpty(data.pic))
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    PackageImg.SetTargetPic(data.pic, null, data.pic_url);
                }

                return;
            }

            // 普通类型的支付包。

            PackageImg.Reset();

            if (!NormalGroup.activeSelf)
                NormalGroup.SetActive(true);

            if (!Icon.gameObject.activeSelf)
                Icon.gameObject.SetActive(true);

            Icon.sprite = _spriteHolder.GetPrepaymentIcon(data.currency_type);
            Icon.SetNativeSize();

            if (!Price.gameObject.activeSelf)
                Price.gameObject.SetActive(true);

            Price.text = "¥ " + data.price;

            HotTag.gameObject.SetActive(false);
            RecommendTag.gameObject.SetActive(false);

            if (!string.IsNullOrEmpty(data.tag))
            {
                if (!TagGroup.gameObject.activeSelf)
                    TagGroup.gameObject.SetActive(true);

                if (data.tag == _hotTag)
                {
                    HotTag.gameObject.SetActive(true);
                }
                else if (data.tag == _recommendTag)
                {
                    RecommendTag.gameObject.SetActive(true);
                }
            }
            else
            {
                if (TagGroup.gameObject.activeSelf)
                    TagGroup.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(data.display_name))
            {
                if (!NameGroup.activeSelf)
                    NameGroup.SetActive(true);

                Name.text = data.display_name;
                Description.text = data.description;
            }
            else
            {
                if (NameGroup.activeSelf)
                    NameGroup.SetActive(false);
            }
        }

        public override void Reset(int currentIndex)
        {
            if (Icon.gameObject.activeSelf)
                Icon.gameObject.SetActive(false);

            if (Price.gameObject.activeSelf)
                Price.gameObject.SetActive(false);

            if (TagGroup.gameObject.activeSelf)
                TagGroup.gameObject.SetActive(false);
        }

        public override void Select(bool selected)
        {
            // 选中之后没有什么变化。
        }

        public override Button GetClickButton()
        {
            return Button;
        }
    }
}