using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.UserTask
{
    public class UserTaskItem : Item<UserTaskData>
    {
        #region Inject

        public class Factory : Factory<UserTaskItem>
        {
        }

        #endregion

        public Button Button;

        public Image BtnImage;

        public Image BtnSelectedImage;

        public GameObject Tip;

        private UserTaskData _data;

        public override UserTaskData GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, UserTaskData data)
        {
            _data = data;

            if (data == null)
            {
                Reset(currentIndex);
                return;
            }
            BtnImage.sprite = data.BtnSprite;
            BtnSelectedImage.sprite = data.BtnSelectedSprite;
            Tip.gameObject.SetActive(data.CanGetAward);
        }

        public override void Reset(int currentIndex)
        {
            Tip.SetActive(false);
            BtnImage.gameObject.SetActive(false);
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return Button;
        }

        public void Selected(bool isSelected)
        {
            BtnSelectedImage.gameObject.SetActive(isSelected);
        }
    }
}