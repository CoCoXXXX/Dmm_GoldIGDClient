using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Race
{
    public class DisplayRankItem : Item<RankingList>
    {
        #region Inject

        public class Factory : Factory<DisplayRankItem>
        {
        }

        #endregion

        #region 组件

        public GameObject Bg;

        public Text RankTxt;

        public Text NicknameTxt;

        public Text ScoreTxt;

        #endregion

        private RankingList _data;

        public override RankingList GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, RankingList data)
        {
            _data = data;
            Bg.SetActive(currentIndex % 2 == 0);
            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            RankTxt.text = data.rank.ToString();
            NicknameTxt.text = data.nickname;
            ScoreTxt.text = data.score.ToString();
        }

        public override void Reset(int currentIndex)
        {
            RankTxt.text = "";
            NicknameTxt.text = "";
            ScoreTxt.text = "";
        }

        public override void Select(bool selected)
        {
        }

        public override Button GetClickButton()
        {
            return null;
        }
    }
}