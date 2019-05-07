using Dmm.Game;
using Dmm.Res;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class YuanBaoItem : Item<com.morln.game.gd.command.YuanBaoItem>
    {
        #region Inject

        public class Factory : Factory<YuanBaoItem>
        {
        }

        #endregion

        #region 组件

        public AsyncImage Icon;

        public Text PriceTxt;

        public Text LeftCountTxt;

        public GameObject NameGroup;

        public Text NameTxt;

        public GameObject TagGroup;

        public Text TagTxt;

        public Button Button;

        #endregion

        #region 接口

        private com.morln.game.gd.command.YuanBaoItem _data;

        public override com.morln.game.gd.command.YuanBaoItem GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, com.morln.game.gd.command.YuanBaoItem data)
        {
            _data = data;

            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            if (Icon)
            {
                Icon.SetTargetPic(data.pic, ResourcePath.YuanBaoPath, data.pic_url);
            }

            if (PriceTxt)
                PriceTxt.text = string.Format("{0}兑奖券", data.price);

            if (LeftCountTxt)
                LeftCountTxt.text = string.Format("剩余{0}个", data.left_count);

            if (NameGroup)
            {
                var hasDisplayName = !string.IsNullOrEmpty(data.displayName);
                if (NameGroup.activeSelf != hasDisplayName)
                    NameGroup.SetActive(hasDisplayName);
            }

            if (NameTxt)
                NameTxt.text = data.displayName;

            if (TagGroup)
            {
                var hasTag = !string.IsNullOrEmpty(data.tag);
                if (TagGroup.activeSelf != hasTag)
                    TagGroup.SetActive(hasTag);
            }

            if (TagTxt)
                TagTxt.text = data.tag;
        }

        public override void Reset(int currentIndex)
        {
            if (Icon)
                Icon.Reset();

            if (PriceTxt)
                PriceTxt.text = "";

            if (LeftCountTxt)
                LeftCountTxt.text = "";

            if (NameGroup && NameGroup.gameObject.activeSelf)
                NameGroup.SetActive(false);

            if (TagGroup && TagGroup.activeSelf)
                TagGroup.SetActive(false);
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return Button;
        }

        #endregion
    }
}