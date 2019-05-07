using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Race
{
    public class RaceHistoryRankItem : Item<RaceData>
    {
        #region Inject

        public class Factory : Factory<RaceHistoryRankItem>
        {
        }

        #endregion

        #region 组件

        public Button Date;

        public GameObject MyDateRank;

        public Text DateTxt;

        public Text MyDateTxt;

        public Text MyRankTxt;

        #endregion

        public RaceData Data;

        public override RaceData GetData()
        {
            return null;
        }

        public override void BindData(int currentIndex, RaceData data)
        {
            Data = data;

            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            MyRankTxt.text = data.rank < 0 ? "暂无排行" : "我的当日排名：" + data.rank;

            if (string.IsNullOrEmpty(data.signUpTime))
            {
                Reset(currentIndex);
                return;
            }

            DateTxt.text = data.signUpTime;
            MyDateTxt.text = data.signUpTime;
        }

        public override void Reset(int currentIndex)
        {
            DateTxt.text = "";
            MyDateTxt.text = "";
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return Date;
        }

        public void ShowDate()
        {
            Date.gameObject.SetActive(true);
            MyDateRank.SetActive(false);
        }

        public void ShowMyRank()
        {
            Date.gameObject.SetActive(false);
            MyDateRank.SetActive(true);
        }
    }
}