using com.morln.game.gd.command;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.PokerLogic;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Game
{
    /// <summary>
    /// 进贡信息面板。
    /// 在进还贡阶段实时显示进还贡的信息。
    /// </summary>
    public class JinGongInfoPanel : MonoBehaviour
    {
        #region Inject

        private IDataContainer<PlayingData> _playingData;

        private IDataContainer<TableUserData> _tableUserData;

        [Inject]
        public void Initialize(IDataRepository dataRepository)
        {
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
            _tableUserData = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
        }

        #endregion

        private void Update()
        {
            RefreshTotal();
        }

        public CardHelper CardHelper;

        public Color JinGongColor;

        public Color HuanGongColor;

        public GameObject Content;

        public GameObject Group1;

        public GameObject JinGong1;

        public Text JinGongUser1;

        public MiniCardSlot JinGongPoker1;

        public Text JinGongDest1;

        public GameObject HuanGong1;

        public Text HuanGongUser1;

        public MiniCardSlot HuanGongPoker1;

        public Text HuanGongDest1;

        public GameObject Group2;

        public GameObject JinGong2;

        public Text JinGongUser2;

        public MiniCardSlot JinGongPoker2;

        public Text JinGongDest2;

        public GameObject HuanGong2;

        public Text HuanGongUser2;

        public MiniCardSlot HuanGongPoker2;

        public Text HuanGongDest2;

        private float TotalRefreshTime { get; set; }

        private void RefreshTotal()
        {
            if (TotalRefreshTime >= _playingData.Timestamp)
            {
                return;
            }

            TotalRefreshTime = _playingData.Timestamp;

            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return;
            }

            var peroid = playingData.period;
            if (peroid == TablePeriod.JinGong || peroid == TablePeriod.HuanGong)
            {
                SetContent();
            }
        }

        private void SetContent()
        {
            var playingData = _playingData.Read();
            if (playingData == null)
            {
                return;
            }

            // 刷新进贡的状态。
            if (playingData.jg_seat1 != -1)
            {
                if (Group1 && !Group1.activeSelf)
                {
                    Group1.SetActive(true);
                }

                if (!JinGong1.activeSelf)
                {
                    JinGong1.SetActive(true);
                }

                SetJinGongState(
                    JinGongUser1,
                    JinGongPoker1,
                    JinGongDest1,
                    GetTableUser(playingData.jg_seat1),
                    GameUtil.GetPokerFromPokerId(playingData.jg_poker1),
                    GetTableUser(playingData.jg_dest1));
            }
            else
            {
                if (Group1 && Group1.activeSelf)
                {
                    Group1.SetActive(false);
                }
            }

            if (playingData.jg_seat2 != -1)
            {
                if (Group2 && !Group2.activeSelf)
                {
                    Group2.SetActive(true);
                }

                if (!JinGong2.activeSelf)
                {
                    JinGong2.SetActive(true);
                }

                SetJinGongState(
                    JinGongUser2,
                    JinGongPoker2,
                    JinGongDest2,
                    GetTableUser(playingData.jg_seat2),
                    GameUtil.GetPokerFromPokerId(playingData.jg_poker2),
                    GetTableUser(playingData.jg_dest2));
            }
            else
            {
                if (Group2 && Group2.activeSelf)
                {
                    Group2.SetActive(false);
                }
            }

            // 刷新还贡的状态。
            if (playingData.hg_seat1 != -1)
            {
                if (!HuanGong1.activeSelf)
                {
                    HuanGong1.SetActive(true);
                }

                SetHuanGongState(
                    HuanGongUser1,
                    HuanGongPoker1,
                    HuanGongDest1,
                    GetTableUser(playingData.hg_seat1),
                    GameUtil.GetPokerFromPokerId(playingData.hg_poker1),
                    GetTableUser(playingData.hg_dest1));
            }
            else
            {
                if (HuanGong1 && HuanGong1.activeSelf)
                {
                    HuanGong1.SetActive(false);
                }
            }

            if (playingData.hg_seat2 != -1)
            {
                if (!HuanGong2.activeSelf)
                {
                    HuanGong2.SetActive(true);
                }

                SetHuanGongState(
                    HuanGongUser2,
                    HuanGongPoker2,
                    HuanGongDest2,
                    GetTableUser(playingData.hg_seat2),
                    GameUtil.GetPokerFromPokerId(playingData.hg_poker2),
                    GetTableUser(playingData.hg_dest2));
            }
            else
            {
                if (HuanGong2 && HuanGong2.activeSelf)
                {
                    HuanGong2.SetActive(false);
                }
            }
        }

        private void SetJinGongState(
            Text jinGongUserTxt,
            MiniCardSlot jinGongPokerSlot,
            Text jinGongDestTxt,
            User jinGongUser,
            Poker jinGongPoker,
            User jinGongDestUser)
        {
            if (!jinGongUserTxt.gameObject.activeSelf)
            {
                jinGongUserTxt.gameObject.SetActive(true);
            }

            var nickname = FormatNickname(jinGongUser, "爱掼蛋玩家");

            if (jinGongPoker == null)
            {
                jinGongUserTxt.text = "等待 " + nickname + " 进贡";

                if (jinGongPokerSlot.gameObject.activeSelf)
                {
                    jinGongPokerSlot.gameObject.SetActive(false);
                }

                if (jinGongDestTxt.gameObject.activeSelf)
                {
                    jinGongDestTxt.gameObject.SetActive(false);
                }
            }
            else
            {
                jinGongUserTxt.text = nickname + " 进贡";

                if (!jinGongPokerSlot.gameObject.activeSelf)
                {
                    jinGongPokerSlot.gameObject.SetActive(true);
                }

                jinGongPokerSlot.CardHelper = CardHelper;
                jinGongPokerSlot.Poker = jinGongPoker;

                var destNickname = FormatNickname(jinGongDestUser, null);
                if (destNickname != null)
                {
                    if (!jinGongDestTxt.gameObject.activeSelf)
                    {
                        jinGongDestTxt.gameObject.SetActive(true);
                    }

                    jinGongDestTxt.text = "给 " + destNickname;
                }
                else
                {
                    if (jinGongDestTxt.gameObject.activeSelf)
                    {
                        jinGongDestTxt.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void SetHuanGongState(
            Text huanGongUserTxt,
            MiniCardSlot huanGongPokerSlot,
            Text huanGongDestTxt,
            User huanGongUser,
            Poker huanGongPoker,
            User huanGongDestUser)
        {
            if (!huanGongUserTxt.gameObject.activeSelf)
            {
                huanGongUserTxt.gameObject.SetActive(true);
            }

            var nickname = FormatNickname(huanGongUser, "爱掼蛋玩家");
            if (huanGongPoker == null)
            {
                huanGongUserTxt.text = "等待 " + nickname + " 还贡";

                if (huanGongPokerSlot.gameObject.activeSelf)
                {
                    huanGongPokerSlot.gameObject.SetActive(false);
                }

                if (huanGongDestTxt.gameObject.activeSelf)
                {
                    huanGongDestTxt.gameObject.SetActive(false);
                }
            }
            else
            {
                huanGongUserTxt.text = nickname + " 还贡";

                if (!huanGongPokerSlot.gameObject.activeSelf)
                {
                    huanGongPokerSlot.gameObject.SetActive(true);
                }

                huanGongPokerSlot.CardHelper = CardHelper;
                huanGongPokerSlot.Poker = huanGongPoker;

                var destNickname = FormatNickname(huanGongDestUser, null);
                if (destNickname != null)
                {
                    if (!huanGongDestTxt.gameObject.activeSelf)
                    {
                        huanGongDestTxt.gameObject.SetActive(true);
                    }

                    huanGongDestTxt.text = "给 " + destNickname;
                }
                else
                {
                    if (huanGongDestTxt.gameObject.activeSelf)
                    {
                        huanGongDestTxt.gameObject.SetActive(false);
                    }
                }
            }
        }

        // TODO 决定是否需要判定昵称为null。
        private string FormatNickname(User user, string replaceEmpty)
        {
            string nickname = null;
            if (user != null)
            {
                nickname = user.nickname;
            }

            if (!string.IsNullOrEmpty(nickname))
            {
                nickname = DataUtil.FormatNickname(nickname);
            }
            else
            {
                nickname = replaceEmpty;
            }

            return nickname;
        }

        private User GetTableUser(int seat)
        {
            var tableUser = _tableUserData.Read();
            return tableUser.GetUserAtSeat(seat);
        }
    }
}