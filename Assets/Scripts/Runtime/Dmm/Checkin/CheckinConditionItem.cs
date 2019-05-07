using com.morln.game.gd.command;
using Dmm.Res;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Checkin
{
    public class CheckinConditionItem : Item<CheckinCondition>
    {
        #region Injected

        public class Factory : Factory<CheckinConditionItem>
        {
        }

        #endregion

        public AsyncImage AwardImage;

        public Text DayCount;

        public Text AwardDescription;

        public GameObject FinishTag;

        private CheckinCondition _data;

        public override CheckinCondition GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, CheckinCondition data)
        {
            _data = data;
            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            if (!AwardImage.gameObject.activeSelf)
            {
                AwardImage.gameObject.SetActive(true);
            }

            AwardImage.SetTargetPic(data.pic, ResourcePath.CheckinPath, data.pic_url);

            if (!DayCount.gameObject.activeSelf)
            {
                DayCount.gameObject.SetActive(true);
            }

            DayCount.text = "连续签到" + data.day_count + "天";

            if (!AwardDescription.gameObject.activeSelf)
            {
                AwardDescription.gameObject.SetActive(true);
            }

            AwardDescription.text = data.description;

            if (FinishTag.activeSelf != data.awarded)
            {
                FinishTag.SetActive(data.awarded);
            }
        }

        public override void Reset(int currentIndex)
        {
            AwardImage.Reset();
            DayCount.text = "";
            AwardDescription.text = "";

            if (FinishTag.activeSelf)
            {
                FinishTag.SetActive(false);
            }
        }

        public override void Select(bool selected)
        {
            // 选中也没啥反应。
        }

        public override Button GetClickButton()
        {
            return null;
        }
    }
}