using Dmm.Analytic;
using Dmm.Clipboard;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Pay;
using Dmm.PIP;
using Dmm.Res;
using Dmm.Sdk;
using Dmm.Session;
using Dmm.Sound;
using Dmm.Task;
using Dmm.WeChat;
using Zenject;

namespace Dmm.App
{
    public class AppContext : IAppContext
    {
        private IAppController _appController;

        private IGameCanvas _gameCanvas;

        private IDialogManager _dialogManager;

        private INetworkManager _network;

        private IPayManager _payManager;

        private IAnalyticManager _analyticManager;

        private ITaskManager _taskManager;

        private ISystemMsgController _systemMsgController;

        private ISoundController _soundController;

        private IWeChatManager _weChatManager;

        private RemoteAPI _remoteAPI;

        private ConfigHolder _configHolder;

        private IosSDK _iosSDK;

        private IPIPLogic _pipLogic;

        private ISocketFactory _socketFactory;

        private IMessageRouter _messageRouter;

        private IMsgRepo _msgRepo;

        private XiaoMiManager _xiaoMiManager;

        private IDataRepository _dataRepository;

        private IClipboardManager _clipboardManager;

        private IResourceManager _resourceManager;

        [Inject]
        public void Inject(
            IAppController appController,
            INetworkManager network,
            IGameCanvas gameCanvas,
            ITaskManager taskManager,
            IDialogManager dialogManager,
            IPayManager payManager,
            IAnalyticManager analyticManager,
            ISystemMsgController systemMsgController,
            ISoundController soundController,
            IWeChatManager weChatManager,
            RemoteAPI remoteAPI,
            ConfigHolder configHolder,
            IosSDK iosSDK,
            IPIPLogic pipLogic,
            IClipboardManager clipboardManager,
            ISocketFactory socketFactory,
            IMessageRouter messageRouter,
            IMsgRepo msgRepo,
            XiaoMiManager xiaoMiManager,
            IResourceManager resourceManager,
            IDataRepository dataRepository)
        {
            _network = network;
            _taskManager = taskManager;
            _appController = appController;
            _gameCanvas = gameCanvas;
            _dialogManager = dialogManager;
            _payManager = payManager;
            _analyticManager = analyticManager;
            _systemMsgController = systemMsgController;
            _soundController = soundController;
            _weChatManager = weChatManager;
            _remoteAPI = remoteAPI;
            _configHolder = configHolder;
            _iosSDK = iosSDK;
            _socketFactory = socketFactory;
            _pipLogic = pipLogic;
            _messageRouter = messageRouter;
            _msgRepo = msgRepo;
            _xiaoMiManager = xiaoMiManager;
            _dataRepository = dataRepository;
            _clipboardManager = clipboardManager;
            _resourceManager = resourceManager;
        }

        public IAppController GetAppController()
        {
            return _appController;
        }

        public IGameCanvas GetGameCanvas()
        {
            return _gameCanvas;
        }

        public IDialogManager GetDialogManager()
        {
            return _dialogManager;
        }

        public ITaskManager GetTaskManager()
        {
            return _taskManager;
        }

        public IAnalyticManager GetAnalyticManager()
        {
            return _analyticManager;
        }

        public ISystemMsgController GetSystemMsgController()
        {
            return _systemMsgController;
        }

        public IPayManager GetPayManager()
        {
            return _payManager;
        }

        public INetworkManager GetNetworkManager()
        {
            return _network;
        }

        public ISoundController GetSoundController()
        {
            return _soundController;
        }

        public IWeChatManager GetWeChatManager()
        {
            return _weChatManager;
        }

        public RemoteAPI GetRemoteAPI()
        {
            return _remoteAPI;
        }

        public IosSDK GetIosSDK()
        {
            return _iosSDK;
        }

        public ConfigHolder GetConfigHolder()
        {
            return _configHolder;
        }

        public IPIPLogic GetPIPLogic()
        {
            return _pipLogic;
        }

        public ISocketFactory GetSocketFactory()
        {
            return _socketFactory;
        }

        public IMessageRouter GetMessageRouter()
        {
            return _messageRouter;
        }

        public IMsgRepo GetMsgRepo()
        {
            return _msgRepo;
        }

        public XiaoMiManager GetXiaoMiManager()
        {
            return _xiaoMiManager;
        }

        public IDataRepository GetDataRepository()
        {
            return _dataRepository;
        }

        public IClipboardManager GetClipboardManager()
        {
            return _clipboardManager;
        }

        public IResourceManager GetResourceManager()
        {
            return _resourceManager;
        }
    }
}