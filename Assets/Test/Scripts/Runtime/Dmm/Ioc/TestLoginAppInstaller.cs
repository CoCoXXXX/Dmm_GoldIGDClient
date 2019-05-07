using Dmm.Analytic;
using Dmm.AnySdk;
using Dmm.App;
using Dmm.Clipboard;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Hall;
using Dmm.Left;
using Dmm.Local;
using Dmm.Login;
using Dmm.Msg;
using Dmm.MsgLogic;
using Dmm.Network;
using Dmm.Pay;
using Dmm.PIP;
using Dmm.Res;
using Dmm.RoundEnd;
using Dmm.Sdk;
using Dmm.Session;
using Dmm.Sound;
using Dmm.Task;
using Dmm.UI;
using Dmm.WeChat;
using Test.Scripts.Runtime.Dmm.TestLogin;
using Zenject;

namespace Dmm.Ioc
{
    public class TestLoginAppInstaller : MonoInstaller
    {
        #region Prefab

        public AppController AppControllerPrefab;

        public MessageRouter MessageRouterPrefab;

        public PIPLogic PIPLogicPrefab;

        public SoundController SoundControllerPrefab;

        public SpriteHolder SpriteHolderPrefab;

        public GameCanvas GameCanvasPrefab;

        public SystemMsgController SystemMsgPanelPrefab;

        public XiaoMiManager XiaoMiManagerPrefab;

        public UIController UIControllerPrefab;

        public RoomBtn RoomBtnPrefab;

        public FriendItem FriendItemPrefab;

        public RoundEndRankMySelf RoundEndRankMySelfPrefab;

        public RoundEndRankOther RoundEndRankOtherPrefab;

        public ConfigHolder ConfigHolderPrefab;

        #endregion

        public override void InstallBindings()
        {
            // 整个游戏优先启动的控制器。
            Container.Bind<IAppController>()
                .FromComponentInNewPrefab(AppControllerPrefab)
                .AsSingle()
                .NonLazy();

            Container.Bind<IAppContext>().To<AppContext>().AsSingle();

            Container.Bind<IMsgRepo>().To<MsgRepo>().AsSingle();
            Container.Bind<RemoteAPI>().To<RemoteAPI>().AsSingle();
            Container.Bind<ISocketFactory>().To<SocketFactory>().AsSingle();
            Container.Bind<IFilePicManager>().To<FilePicManager>().AsSingle();

            Container.Bind<IDataRepository>().To<DataRepository>().AsSingle();

            Container.Bind<IPayManager>()
                .To<PayManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("PayManager")
                .AsSingle();

            Container.Bind<IResourceCache>()
                .To<ResourceCache>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("ResourceCache")
                .AsSingle();

            Container.Bind<IResourceManager>()
                .To<AssetBundleResourceManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("AssetBundleResourceManager")
                .AsSingle();

            Container.Bind<LocalGameServer>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("LocalGameServer")
                .AsSingle();

            Container.Bind<IWeChatManager>()
                .To<WeChatManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("WeChatManager")
                .AsSingle();

            Container.Bind<IAnySDKManager>()
                .To<AnySDKManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("AnySDKManager")
                .AsSingle();

            Container.Bind<XiaoMiManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("XiaoMiManager")
                .AsSingle();

            Container.Bind<IClipboardManager>()
                .To<ClipboardManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("ClipboardManager")
                .AsSingle();

            Container.Bind<ITaskManager>()
                .To<TaskManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("TaskManager")
                .AsSingle();

            Container.Bind<IGameCanvas>()
                .FromComponentInNewPrefab(GameCanvasPrefab)
                .AsSingle();

            Container.Bind<IPIPLogic>()
                .FromComponentInNewPrefab(PIPLogicPrefab)
                .AsSingle();

            Container.Bind<ConfigHolder>()
                .FromComponentInNewPrefab(ConfigHolderPrefab)
                .AsSingle();

            // UIController
            Container.Bind<IUIController>()
                .FromComponentInNewPrefab(UIControllerPrefab)
                .AsSingle();

            Container.Bind<LoginModeWindow.Factory>().AsSingle();
            Container.Bind<PortalWindow.Factory>().AsSingle();
            Container.Bind<RoomWindow.Factory>().AsSingle();
            Container.Bind<SeatWindow.Factory>().AsSingle();

            Container.Bind<GameWindow.Factory>().AsSingle();

            Container.BindFactory<RoomBtn, RoomBtn.Factory>()
                .FromComponentInNewPrefab(RoomBtnPrefab);

            Container.BindFactory<FriendItem, FriendItem.Factory>()
                .FromComponentInNewPrefab(FriendItemPrefab);

            Container.Bind<FlyItem.Factory>().AsSingle();

            // GameWindow
            Container.BindFactory<RoundEndRankMySelf, RoundEndRankMySelf.Factory>()
                .FromComponentInNewPrefab(RoundEndRankMySelfPrefab);

            Container.BindFactory<RoundEndRankOther, RoundEndRankOther.Factory>()
                .FromComponentInNewPrefab(RoundEndRankOtherPrefab);

            // MessageRouter
            Container.Bind<IMessageLogicFactory>().To<MessageLogicFactory>().AsSingle();
            Container.Bind<IMessageRouter>()
                .FromComponentInNewPrefab(MessageRouterPrefab)
                .AsSingle();

            Container.Bind<IAnalyticManager>().To<AnalyticManager>().AsSingle();

            // SDK
            Container.Bind<AndroidSDK>().AsSingle().NonLazy();
            Container.Bind<IosSDK>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("IosSDK").AsSingle().NonLazy();

            Container.Bind<INetworkManager>().To<TestLoginNetworkManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("TestLoginNetworkManager")
                .AsSingle().NonLazy();

            Container.Bind<ISoundController>()
                .FromComponentInNewPrefab(SoundControllerPrefab)
                .AsSingle();

            Container.Bind<SpriteHolder>()
                .FromComponentInNewPrefab(SpriteHolderPrefab)
                .AsSingle()
                .NonLazy();

            Container.Bind<ISystemMsgController>()
                .FromComponentInNewPrefab(SystemMsgPanelPrefab)
                .AsSingle();
        }
    }
}