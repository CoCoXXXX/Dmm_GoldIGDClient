using Dmm.Widget;
using UnityEngine.UI;

namespace Dmm.Chat
{
    public class TextItem : Item<string>
    {
        public Text Content;

        public Button Button;

        private string _data;

        public override string GetData()
        {
            return _data;
        }

        public override void BindData(int currentIndex, string data)
        {
            _data = data;

            if (!Content || string.IsNullOrEmpty(data))
            {
                return;
            }

            if (!Content.gameObject.activeSelf)
            {
                Content.gameObject.SetActive(true);
            }

            Content.text = data;
        }

        public override void Reset(int currentIndex)
        {
            if (Content)
            {
                Content.text = "";
            }
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