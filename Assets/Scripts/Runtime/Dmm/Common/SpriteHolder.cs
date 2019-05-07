using System.Collections.Generic;
using Dmm.Constant;
using UnityEngine;

namespace Dmm.Common
{
    public class SpriteHolder : MonoBehaviour
    {
        #region Vip图标

        public List<Sprite> VipIconList;

        public Sprite VipIcon(int level)
        {
            if (VipIconList == null || VipIconList.Count <= 0)
                return null;

            if (level < 1 || level > VipIconList.Count)
                return null;

            return VipIconList[level - 1];
        }

        #endregion

        #region 货币图标

        public Sprite GoldEgg;

        public Sprite YinPiao;

        public Sprite YuanBao;

        public Sprite Exp;

        public Sprite Vip;

        public Sprite Score;

        public Sprite CardRecorder;

        public Sprite CheckinCard;

        public Sprite GetCurrency(int currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.GOLDEN_EGG:
                    return GoldEgg;

                case CurrencyType.YUAN_BAO:
                    return YuanBao;

                case CurrencyType.YIN_PIAO:
                    return YinPiao;

                case CurrencyType.EXP:
                    return Exp;

                case CurrencyType.VIP:
                    return Vip;

                case CurrencyType.SCORE:
                    return Score;

                case CurrencyType.CARD_RECORDER:
                    return CardRecorder;

                case CurrencyType.RECHECKIN_CARD:
                    return CheckinCard;

                default:
                    return null;
            }
        }

        #endregion

        #region 支付包图标

        public Sprite PrepaymentIconYinPiao;

        public Sprite PrepaymentIconGoldEgg;

        public Sprite GetPrepaymentIcon(int currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.GOLDEN_EGG:
                    return PrepaymentIconGoldEgg;

                case CurrencyType.YIN_PIAO:
                    return PrepaymentIconYinPiao;

                default:
                    return null;
            }
        }

        #endregion

        #region 玩家的头像

        public Sprite HeadIconMale;

        public Sprite HeadIconFemale;

        #endregion

        #region 胜利和失败的icon

        public Sprite WinIcon;

        public Sprite LoseIcon;

        #endregion
    }
}