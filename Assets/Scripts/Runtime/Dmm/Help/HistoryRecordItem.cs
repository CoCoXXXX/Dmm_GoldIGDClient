using Dmm.Widget;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Help
{
    public class HistoryRecordItem : Item<HistoryRecord>
    {
        #region inject

        public class Factory : Factory<HistoryRecordItem>
        {
        }

        #endregion

        public Text IssueTxt;

        public Text FeedbackTxt;

        public HistoryRecord Data;

        public override HistoryRecord GetData()
        {
            return null;
        }

        public override void BindData(int currentIndex, HistoryRecord data)
        {
            Data = data;
            if (data == null)
            {
                Reset(currentIndex);
                return;
            }

            if (string.IsNullOrEmpty(Data.content))
            {
                Reset(currentIndex);
                return;
            }

            IssueTxt.text = Data.content;

            if (string.IsNullOrEmpty(Data.reply))
            {
                FeedbackTxt.gameObject.SetActive(false);
                return;
            }

            FeedbackTxt.text = Data.reply;
        }

        public override void Reset(int currentIndex)
        {
            Data = null;

            IssueTxt.text = "";
            FeedbackTxt.text = "";

            IssueTxt.gameObject.SetActive(false);
            FeedbackTxt.gameObject.SetActive(false);
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