using UnityEngine;
using UnityEngine.UI;

namespace Dmm.RoundEnd
{
    public class MidRoundEnd : MonoBehaviour
    {
        #region 结果图片

        public Sprite WinImg;

        public Sprite LoseImg;

        public Image RoundEndImg;

        #endregion

        public Sprite WinIcon;

        public Sprite LoseIcon;

        public RectTransform ShareGroup;

        public Text Exp;

        public Text NextHost;

        public Text ReadyLeftTime;

        public void ResetContent()
        {
        }
    }
}