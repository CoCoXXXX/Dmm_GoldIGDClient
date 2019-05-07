using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.AnySdk;
using Dmm.Common;
using Dmm.Constant;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Local;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Sdk;
using Dmm.UI;
using Dmm.WeChat;
using UnityEngine;
using Zenject;

namespace Dmm.App
{
    /// <summary>
    /// 整个游戏的控制器。
    /// </summary>
    public class AppController : MonoBehaviour, IAppController
    {
        #region Inject

        private IMsgRepo _msgRepo;

        private INetworkManager _network;

        private IUIController _uiController;

        private ConfigHolder _configHolder;

        private IAnySDKManager _anySdkManager;

        private XiaoMiManager _xiaoMiManager;

        private IAnalyticManager _analyticManager;

        private IWeChatManager _weChatManager;

        private LocalGameServer _localGameServer;

        private IosSDK _ios;

        private IDataContainer<VersionResult> _versionResult;

        private IDataContainer<PLoginResult> _pLoginResult;

        private IDataContainer<bool> _billboardRead;

        private IDataContainer<AppState> _appState;

        private IDataContainer<int> _currentGameMode;

        private IDataContainer<VipExchangeListResult> _vipExchangeList;

        private IDataContainer<TreasureChestData> _treasureChestData;

        private IDataContainer<HLoginResult> _hloginResult;

        private IDataContainer<SFriendListResult> _friendListResult;

        [Inject]
        public void Initialize(
            IUIController uiController,
            IMsgRepo msgRepo,
            INetworkManager network,
            XiaoMiManager xiaoMiManager,
            IAnalyticManager analyticManager,
            ConfigHolder configHolder,
            IAnySDKManager anySdkManager,
            IosSDK ios,
            IWeChatManager weChatManager,
            IDataRepository dataRepository,
            LocalGameServer localGameServer)
        {
            _uiController = uiController;
            _ios = ios;
            _network = network;
            _msgRepo = msgRepo;
            _configHolder = configHolder;
            _xiaoMiManager = xiaoMiManager;
            _weChatManager = weChatManager;
            _analyticManager = analyticManager;
            _localGameServer = localGameServer;
            _anySdkManager = anySdkManager;

            _appState = dataRepository.GetContainer<AppState>(DataKey.AppState);
            _versionResult = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
            _pLoginResult = dataRepository.GetContainer<PLoginResult>(DataKey.PLoginResult);
            _billboardRead = dataRepository.GetContainer<bool>(DataKey.BillboardRead);
            _currentGameMode = dataRepository.GetContainer<int>(DataKey.CurrentGameMode);
            _vipExchangeList = dataRepository.GetContainer<VipExchangeListResult>(DataKey.VipExchangeListResult);
            _treasureChestData = dataRepository.GetContainer<TreasureChestData>(DataKey.TreasureChestData);
            _hloginResult = dataRepository.GetContainer<HLoginResult>(DataKey.HLoginResult);
            _friendListResult = dataRepository.GetContainer<SFriendListResult>(DataKey.SFriendListResult);
        }

        #endregion

        #region Unity方法

        /// <summary>
        /// 如果Test为true，就不会执行所有的初始化动作。
        /// </summary>
        public bool Test;

        public int FrameRate = 30;

        public void Awake()
        {
            // 初始化目标的帧率。
            Application.targetFrameRate = FrameRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void Start()
        {
            _uiController.SwitchTo(UIWindowType.LoginMode);

            var productIdList = _configHolder.GetIapProductIdList();
            _ios.GetProductInfo(productIdList);

            if (Test) return;

            // 初始化事件统计。
            _analyticManager.Init();
            _weChatManager.Init();
            _anySdkManager.Init();

#if UNITY_ANDROID // 小米模式下初始化小米。
            if (_configHolder.XiaoMiMode)
            {
                _xiaoMiManager.Init();
            }
#endif
            _network.InitLogin();

            ClearAppStateData();
            // version >= 6.4.0 不再显示SplashScreen了。

            // 因为AppController控制应用程序的状态。
            // 所以也应该由AppController在进入游戏的时候切换到LoginPanel。
            AppStateRefreshTime = -1;
        }

        public void Update()
        {
            UpdateAppState();
        }

        #endregion

        #region 应用状态

        /// <summary>
        /// 应用的刷新时间。
        /// </summary>
        public float AppStateRefreshTime { get; private set; }

        /// <summary>
        /// 清空当前应用的状态。
        /// </summary>
        public void ClearAppStateData()
        {
            _pLoginResult.ClearAndInvalidate(Time.time);
            _versionResult.ClearNotInvalidate();
            _billboardRead.ClearNotInvalidate();

            _hloginResult.ClearAndInvalidate(Time.time);
            _currentGameMode.Write(GameMode.Null, Time.time);
            _vipExchangeList.ClearNotInvalidate();
            _treasureChestData.ClearNotInvalidate();
            _friendListResult.ClearNotInvalidate();
        }

        private AppState _currentAppState = AppState.Null;

        private void UpdateAppState()
        {
            // 测试模式下，不刷新用用的状态。
            if (Test)
            {
                return;
            }

            // 后面更新的是登陆成功之后的状态。
            var time = _appState.Timestamp;
            if (AppStateRefreshTime >= time)
            {
                return;
            }

            AppStateRefreshTime = time;

            var curState = _appState.Read();
            if (curState == _currentAppState)
            {
                return;
            }

            _currentAppState = curState;
            MyLog.InfoWithFrame(name, string.Format("app state change to: {0}", curState));

            // 根据当前游戏的状态，切换界面状态。
            switch (curState)
            {
                case AppState.NoLogin:
                    _uiController.SwitchTo(UIWindowType.LoginMode);
                    break;

                case AppState.LoginOk:
                    _uiController.SwitchTo(UIWindowType.Portal);
                    break;

                case AppState.ChooseRoom:
                    _uiController.SwitchTo(UIWindowType.Room);
                    break;

                case AppState.InRoom:
                    _uiController.SwitchTo(UIWindowType.Seat);
                    break;

                case AppState.InTable:
                    _uiController.SwitchTo(UIWindowType.Seat);
                    break;

                case AppState.BetweenRound:
                case AppState.Playing:
                    _uiController.SwitchTo(UIWindowType.Game);
                    break;

                default:
                    _uiController.SwitchTo(UIWindowType.LoginMode);
                    break;
            }
        }

        #endregion

        #region 单机模式

        public bool IsSingleGameMode()
        {
            return _singleGameMode;
        }

        private bool _singleGameMode;

        /// <summary>
        /// 进入单机模式。
        /// </summary>
        public void GoToSingleGame()
        {
            _singleGameMode = true;

            _network.AbortLogin();
            _network.Close();

            _localGameServer.Init();

            _analyticManager.Event("goto_single_game");
        }

        /// <summary>
        /// 退出单机模式。
        /// </summary>
        public void ExitSingleGame()
        {
            _singleGameMode = false;
            ClearAppStateData();
            _network.InitLogin();
        }

        #endregion
    }
}