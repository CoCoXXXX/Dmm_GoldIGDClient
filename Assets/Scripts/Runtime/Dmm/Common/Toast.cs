using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Common
{
    public class Toast : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;

        public Color NormalColor;

        public Color ErrorColor;

        public Text ContentTxt;

        public string Content
        {
            get
            {
                if (ContentTxt)
                    return ContentTxt.text;
                else
                    return null;
            }
            set
            {
                if (ContentTxt)
                    ContentTxt.text = value;
            }
        }

        public bool Error
        {
            get { return _error; }
            set
            {
                _error = value;
                if (ContentTxt)
                    ContentTxt.color = _error ? ErrorColor : NormalColor;
            }
        }

        private bool _error;
    }
}