using Dmm.Data;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Checkin
{
    public class DayItem : Item<CheckinDayData>
    {
        public GameObject TodayTag;

        public GameObject Checked;

        public Text Text;

        public Color NormalFontColor;

        public Color PassedFontColor;

        private CheckinDayData _data;

        public override CheckinDayData GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, CheckinDayData data)
        {
            _data = data;
            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            var check = data.Checked && data.Enabled;
            if (Checked && Checked.activeSelf != check)
            {
                Checked.SetActive(check);
            }

            if (Text)
            {
                if (Text.gameObject.activeSelf != data.Enabled)
                {
                    Text.gameObject.SetActive(data.Enabled);
                }

                Text.text = "" + data.Day;
                Text.color = data.Passed ? PassedFontColor : NormalFontColor;
            }

            var today = data.IsToday && data.Enabled;
            if (TodayTag && TodayTag.activeSelf != today)
            {
                TodayTag.SetActive(today);
            }
        }

        public override void Reset(int currentIndex)
        {
            _data = null;

            if (Checked && Checked.activeSelf)
            {
                Checked.SetActive(false);
            }

            if (TodayTag && TodayTag.activeSelf)
            {
                TodayTag.SetActive(false);
            }

            if (Text)
            {
                Text.text = "";
                if (Text.gameObject.activeSelf)
                {
                    Text.gameObject.SetActive(false);
                }
            }
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return null;
        }

        public override string ToString()
        {
            if (!Text)
            {
                return "NULL";
            }

            return Text.text;
        }
    }
}