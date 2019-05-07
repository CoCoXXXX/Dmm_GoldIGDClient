using UnityEngine;
using UnityEngine.UI;

namespace Dmm.Game
{
    public class BottomBtnGroup : MonoBehaviour
    {
        public Button BackToOnlineBtn;

        public Button KanZhuoMianBtn;

        public Button ChatBtn;

        public Button TongHuaBtn;

        public Button LiChengYiLieBtn;

        public Button CheXiaoLiPaiBtn;

        public void EnableAllBtn(bool enableAll)
        {
            if (BackToOnlineBtn)
            {
                BackToOnlineBtn.interactable = enableAll;
            }

            if (KanZhuoMianBtn)
            {
                KanZhuoMianBtn.interactable = enableAll;
            }

            if (ChatBtn)
            {
                ChatBtn.interactable = enableAll;
            }

            if (TongHuaBtn)
            {
                TongHuaBtn.interactable = enableAll;
            }

            if (LiChengYiLieBtn)
            {
                LiChengYiLieBtn.interactable = enableAll;
            }

            if (CheXiaoLiPaiBtn)
            {
                CheXiaoLiPaiBtn.interactable = enableAll;
            }
        }

        public void EnableChatBtn(bool isEnable)
        {
            if (ChatBtn)
            {
                ChatBtn.interactable = isEnable;
            }
        }
    }
}