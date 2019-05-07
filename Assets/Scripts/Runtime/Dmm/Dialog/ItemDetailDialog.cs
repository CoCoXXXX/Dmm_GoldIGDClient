using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.UserTask;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Dialog
{
    public class ItemDetailDialog : MyDialog
    {
        private TaskItem _item;

        private UserTaskState _data;

        public AsyncImage AwardIcon;

        public Text AwardName;

        public Text AwardDescription;

        public GameObject GoToBtn;

        public GameObject GetAwardBtn;

        public GameObject HasGet;

        public void ApplyData(UserTaskState data, TaskItem item)
        {
            _data = data;

            _item = item;

            if (data == null)
            {
                return;
            }

            if (item == null)
            {
                return;
            }

            float progress = (float) data.current_progress / data.total_progress;

            GoToBtn.gameObject.SetActive(false);
            GetAwardBtn.gameObject.SetActive(false);
            HasGet.SetActive(false);

            if (progress >= 0 && progress < 1)
            {
                GoToBtn.gameObject.SetActive(true);
            }
            else
            {
                if (data.award_posted)
                {
                    HasGet.SetActive(true);
                }
                else
                {
                    GetAwardBtn.gameObject.SetActive(true);
                }
            }

            AwardIcon.SetTargetPic(data.pic, null, data.pic_url);

            AwardName.text = CurrencyType.LabelOf(data.display_currency_type);

            var currencyList = data.currency;

            if (currencyList == null || currencyList.Count <= 0)
            {
                AwardDescription.text = "";
                return;
            }

            var awards = "";

            for (int i = 0; i < currencyList.Count; i++)
            {
                awards = awards + CurrencyType.LabelOf(currencyList[i].type) + " x " + currencyList[i].count + "  ";
            }

            AwardDescription.text = "领取后立即得到：" + awards;
        }

        public void GoToTask()
        {
            if (_item != null)
            {
                _item.GoToTask();
            }

            Hide();
        }

        public void GetAward()
        {
            if (_item != null)
            {
                _item.GetAward();
            }

            Hide();
        }
    }
}