using Dmm.App;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.RoundEnd
{
    public class RoundEndRankMySelf : MonoBehaviour
    {
        #region Inject

        private IAppController _appController;

        private SpriteHolder _spriteHolder;

        [Inject]
        public void Initialize(IAppController appController, SpriteHolder spriteHolder)
        {
            _appController = appController;
            _spriteHolder = spriteHolder;
        }

        public class Factory : Factory<RoundEndRankMySelf>
        {
        }

        #endregion

        public Image WinIcon;

        public NicknameGroup Nickname;

        public Text LianDaTipTxt;

        public RectTransform FanbeiGroup;

        public Text FanbeiTxt;

        public Text ExpLabel;

        public Text ExpTxt;

        public Text MoneyLabel;

        public Text MoneyTxt;

        public void Reset()
        {
            if (WinIcon && WinIcon.gameObject.activeSelf)
                WinIcon.gameObject.SetActive(false);

            if (Nickname)
                Nickname.Clear();

            if (LianDaTipTxt && LianDaTipTxt.gameObject.activeSelf)
                LianDaTipTxt.gameObject.SetActive(false);

            if (FanbeiGroup && FanbeiGroup.gameObject.activeSelf)
                FanbeiGroup.gameObject.SetActive(false);

            if (ExpLabel && ExpLabel.gameObject.activeSelf)
                ExpLabel.gameObject.SetActive(false);

            if (ExpTxt && ExpTxt.gameObject.activeSelf)
                ExpTxt.gameObject.SetActive(false);

            if (MoneyLabel && MoneyLabel.gameObject.activeSelf)
                MoneyLabel.gameObject.SetActive(false);

            if (MoneyTxt && MoneyTxt.gameObject.activeSelf)
                MoneyTxt.gameObject.SetActive(false);
        }

        public void ApplyData(
            bool lianDa,
            bool win,
            string nickname,
            int vip,
            int exp,
            int currencyType,
            int money,
            int raceId,
            int totalScore,
            int fanbei)
        {
            if (WinIcon)
            {
                if (!WinIcon.gameObject.activeSelf)
                    WinIcon.gameObject.SetActive(true);

                WinIcon.sprite = win ? _spriteHolder.WinIcon : _spriteHolder.LoseIcon;
            }

            if (Nickname)
                Nickname.SetData(nickname, vip);

            if (ExpTxt)
            {
                if (!_appController.IsSingleGameMode())
                    ExpTxt.text = (exp > 0 ? "+" : "") + exp;
                else
                    ExpTxt.text = "单机模式";
            }

            if (MoneyTxt)
            {
                if (!_appController.IsSingleGameMode())
                    MoneyTxt.text = (money > 0 ? "+" : "") + money;
                else
                    MoneyTxt.text = "单机模式";
            }

            if (fanbei > 1)
            {
                if (FanbeiTxt)
                {
                    if (!FanbeiTxt.gameObject.activeSelf)
                        FanbeiTxt.gameObject.SetActive(true);

                    FanbeiTxt.text = "" + fanbei;
                }
            }
            else
            {
                if (FanbeiGroup && FanbeiGroup.gameObject.activeSelf)
                    FanbeiGroup.gameObject.SetActive(false);
            }

            if (raceId > 0)
            {
                // 是比赛房
                if (ExpLabel)
                {
                    if (!ExpLabel.gameObject.activeSelf)
                        ExpLabel.gameObject.SetActive(true);

                    ExpLabel.text = "总分";
                    ExpTxt.text = "" + totalScore;
                }

                if (MoneyLabel)
                {
                    if (!MoneyLabel.gameObject.activeSelf)
                        MoneyLabel.gameObject.SetActive(true);

                    MoneyLabel.text = "得分";
                }
            }
            else
            {
                if (ExpLabel)
                {
                    if (!ExpLabel.gameObject.activeSelf)
                        ExpLabel.gameObject.SetActive(true);

                    ExpLabel.text = "经验";
                }

                if (MoneyLabel)
                {
                    if (!MoneyLabel.gameObject.activeSelf)
                        MoneyLabel.gameObject.SetActive(true);

                    MoneyLabel.text = CurrencyType.LabelOf(currencyType);
                }
            }
        }
    }
}