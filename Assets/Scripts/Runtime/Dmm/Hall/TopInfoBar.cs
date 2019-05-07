using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Res;
using Dmm.Shop;
using Dmm.Util;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Hall
{
    public class TopInfoBar : MonoBehaviour
    {
        private IDialogManager _dialogManager;

        private IDataContainer<User> _user;

        private IDataContainer<TableUserData> _tableUser;

        [Inject]
        public void Inject(
            IDataRepository dataRepository,
            IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
            _user = dataRepository.GetContainer<User>(DataKey.MyUser);
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        #region DataContainer

        #endregion

        public Text Nickname;

        public GameObject WechatTag;

        public Text Level;

        public Text WinRate;

        public Text SecondMoney;

        public Text Money;

        public AsyncImage HeadIcon;

        private void OnEnable()
        {
            RefreshContent();
        }

        private void Update()
        {
            RefreshContent();
        }

        private float _refreshTime;

        private void RefreshContent()
        {
            if (_refreshTime >= _user.Timestamp)
            {
                return;
            }

            _refreshTime = _user.Timestamp;

            var user = _user.Read();
            if (user == null)
            {
                ResetContent();
                return;
            }

            Nickname.text = user.nickname;
            Level.text = string.Format("{0}级 {1}", user.level, user.title);
            float rate = user.round_count != 0 ? ((float) user.win_count / user.round_count) : 0;
            WinRate.text = string.Format("胜率 {0}", DataUtil.FormatWinRate(rate));
            SecondMoney.text = "" + user.second_money;
            Money.text = "" + user.money;
            if (string.IsNullOrEmpty(user.headicon_url))
            {
                HeadIcon.gameObject.SetActive(false);
            }
            else
            {
                var myUser = _user.Read();
                HeadIcon.SetTargetPic(myUser.HeadIconPic(), null, user.headicon_url);
            }
            WechatTag.SetActive(user.type == UserType.Wechat);
        }

        private void ResetContent()
        {
            HeadIcon.Reset();
            Nickname.text = "";
            WechatTag.SetActive(false);
            Level.text = "";
            WinRate.text = "";
            SecondMoney.text = "";
            Money.text = "";
        }

        public void Charge()
        {
            _dialogManager.ShowDialog<ShopPanel>(DialogName.ShopPanel, false, false,
                (shop) => { shop.Show(ShopPanel.ShopType.Charge); });
        }

        public void ShowLevelInfo()
        {
            var content = "";
            var levels = Constant.LevelTitle.Titles;
            if (levels != null && levels.Length > 0)
            {
                var myUser = _user.Read();
                var myLevel = myUser == null ? 1 : myUser.level;
                for (int i = 0; i < levels.Length; i++)
                {
                    if (myLevel == i + 1)
                    {
                        content += string.Format("<color=red>{0}:{1}</color>", i + 1, levels[i]);
                    }
                    else
                    {
                        content += string.Format("{0}:{1}", i + 1, levels[i]);
                    }

                    if (i < levels.Length - 1)
                    {
                        content += "\n";
                    }
                }
            }

            if (!string.IsNullOrEmpty(content))
            {
                _dialogManager.ShowMessageBox(content);
            }
        }

        public void ShowWinRateInfo()
        {
            var user = _user.Read();
            if (user != null)
            {
                var content = string.Format("胜利：{0}场\n失败：{1}场\n逃跑：{2}场",
                    user.win_count, user.round_count - user.win_count, user.escape_count);
                _dialogManager.ShowMessageBox(content);
            }
        }

        public void ShowPlayerInfo()
        {
            _dialogManager.ShowDialog<PlayerInfoPanel>(DialogName.PlayerInfoPanel, true, true);
        }
    }
}