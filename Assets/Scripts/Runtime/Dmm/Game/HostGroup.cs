using com.morln.game.gd.command;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using PokerNumType = Dmm.PokerLogic.PokerNumType;

namespace Dmm.Game
{
    /// <summary>
    /// 左上角的Host面板。
    /// </summary>
    public class HostGroup : MonoBehaviour
    {
        #region Inject

        private SpriteHolder _spriteHolder;

        private IDataContainer<HostInfoResult> _hostInfo;

        private IDataContainer<BFanbei> _fanbei;

        private IDataContainer<TableUserData> _tableUser;

        private IDataContainer<RaceConfig> _raceConfig;

        private IDataContainer<Room> _currentRoom;

        private IDataContainer<Table> _currentTable;

        private IDataContainer<PlayingData> _playingData;

        [Inject]
        public void Initialize(IDataRepository dataRepository, SpriteHolder spriteHolder)
        {
            _spriteHolder = spriteHolder;

            _hostInfo = dataRepository.GetContainer<HostInfoResult>(DataKey.HostInfo);
            _fanbei = dataRepository.GetContainer<BFanbei>(DataKey.BFanbei);
            _tableUser = dataRepository.GetContainer<TableUserData>(DataKey.TableUserData);
            _raceConfig = dataRepository.GetContainer<RaceConfig>(DataKey.RaceConfig);
            _currentRoom = dataRepository.GetContainer<Room>(DataKey.CurrentRoom);
            _currentTable = dataRepository.GetContainer<Table>(DataKey.CurrentTable);
            _playingData = dataRepository.GetContainer<PlayingData>(DataKey.PlayingData);
        }

        #endregion

        /// <summary>
        /// 我方的Sprite。
        /// </summary>
        public Sprite MyTeam;

        /// <summary>
        /// 对方的Sprite。
        /// </summary>
        public Sprite OppositeTeam;

        // 1队=黄队，2队=蓝队

        public Image BlueTeam;

        public Text BlueHost;

        public Toggle BlueHostMark;

        public Image YellowTeam;

        public Text YellowHost;

        public Toggle YellowHostMark;

        public Text BaseMoney;

        public Image CurrencyIcon;

        private float _refreshTime;

        public void Update()
        {
            RefreshContent();
        }

        private void RefreshContent()
        {
            var max = Mathf.Max(
                _hostInfo.Timestamp,
                _fanbei.Timestamp,
                _tableUser.Timestamp,
                _raceConfig.Timestamp,
                _playingData.Timestamp);

            if (_refreshTime >= max)
            {
                return;
            }

            _refreshTime = max;

            SetHost();
            SetBaseMoney();
        }

        private void SetHost()
        {
            var table = _currentTable.Read();
            var gameType = table != null ? table.game_type : TableGameType.NORMAL;

            if (gameType == TableGameType.NULL)
            {
                return;
            }

            var hostInfo = _hostInfo.Read();

            if (gameType == TableGameType.TTZ)
            {
                if (YellowHost == null)
                {
                    return;
                }

                YellowHost.text = PokerNumType.LabelOf(
                    hostInfo != null ? PokerLogicUtil.PokerNumTypeOf(hostInfo.team1_host) : PokerNumType.P2);
            }
            else if (gameType == TableGameType.RACE_TTZ)
            {
                if (YellowHost == null)
                {
                    return;
                }

                YellowHost.text = PokerNumType.LabelOf(
                    hostInfo != null ? PokerLogicUtil.PokerNumTypeOf(hostInfo.team1_host) : PokerNumType.P2);
            }
            else if (gameType == TableGameType.NORMAL)
            {
                if (YellowHost != null)
                {
                    YellowHost.text = PokerNumType.LabelOf(
                        hostInfo != null ? PokerLogicUtil.PokerNumTypeOf(hostInfo.team1_host) : PokerNumType.P2);
                }

                if (YellowHostMark != null)
                {
                    YellowHostMark.isOn = hostInfo != null && hostInfo.host_team == 1;
                }

                var tableUser = _tableUser.Read();

                if (YellowTeam != null)
                {
                    YellowTeam.sprite = tableUser.GetMyTeam() == 1 ? MyTeam : OppositeTeam;
                }

                if (BlueHost != null)
                {
                    BlueHost.text = PokerNumType.LabelOf(
                        hostInfo != null ? PokerLogicUtil.PokerNumTypeOf(hostInfo.team2_host) : PokerNumType.P2);
                }

                if (BlueHostMark != null)
                {
                    BlueHostMark.isOn = hostInfo != null && hostInfo.host_team == 2;
                }

                if (BlueTeam != null)
                {
                    BlueTeam.sprite = tableUser.GetMyTeam() == 2 ? MyTeam : OppositeTeam;
                }
            }
        }

        private void SetBaseMoney()
        {
            var table = _currentTable.Read();
            var gameType = table != null ? table.game_type : TableGameType.NORMAL;

            if (gameType == TableGameType.NULL)
            {
                return;
            }

            if (gameType == TableGameType.RACE_TTZ)
            {
                BaseMoney.gameObject.SetActive(false);
                CurrencyIcon.gameObject.SetActive(false);
                var raceRoomConfig = _raceConfig.Read();
                if (raceRoomConfig == null)
                {
                    return;
                }

                var score = raceRoomConfig.score;
                if (score == null)
                {
                    return;
                }

                BaseMoney.text = "" + score.count;

                var icon = _spriteHolder.GetCurrency(score.type);
                if (icon != null)
                {
                    if (!CurrencyIcon.gameObject.activeSelf)
                    {
                        CurrencyIcon.gameObject.SetActive(true);
                    }

                    CurrencyIcon.sprite = icon;
                }
                else
                {
                    if (CurrencyIcon.gameObject.activeSelf)
                    {
                        CurrencyIcon.gameObject.SetActive(false);
                    }
                }

                BaseMoney.gameObject.SetActive(true);
                CurrencyIcon.gameObject.SetActive(true);
            }
            else
            {
                var room = _currentRoom.Read();
                if (room == null)
                {
                    return;
                }

                BaseMoney.text = "" + room.base_money;

                var playingData = _playingData.Read();
                if (playingData == null)
                {
                    return;
                }

                var fanbei = playingData.fanbei;
                if (fanbei > 1)
                {
                    BaseMoney.text += "x" + fanbei;
                }

                var sprite = _spriteHolder.GetCurrency(room.currency_type);
                if (sprite != null)
                {
                    if (!CurrencyIcon.gameObject.activeSelf)
                    {
                        CurrencyIcon.gameObject.SetActive(true);
                    }

                    CurrencyIcon.sprite = sprite;
                }
                else
                {
                    if (CurrencyIcon.gameObject.activeSelf)
                    {
                        CurrencyIcon.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}