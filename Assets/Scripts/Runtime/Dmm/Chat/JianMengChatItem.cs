using com.morln.game.gd.command;
using Dmm.Res;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Chat
{
    public class JianMengChatItem : Item<JianMengItem>
    {
        #region Inject

        public class Factory : Factory<JianMengChatItem>
        {
        }

        #endregion

        public AsyncImage Pic;

        public GameObject VipTag;

        public Button Button;

        private JianMengItem _data;

        public override JianMengItem GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, JianMengItem data)
        {
            _data = data;
            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            var vip = data.vip > 0;
            if (VipTag.activeSelf != vip)
                VipTag.SetActive(vip);

            Pic.SetTargetPic(data.pic, ResourcePath.JianMeng, data.pic_url);
        }

        public override void Reset(int currentIndex)
        {
            Pic.Reset();
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