using System.Net.Sockets;
using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.DataRelation;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Session;
using Dmm.StateLogic;
using UnityEngine;
using Zenject;

namespace Dmm.Network
{
    public class NetworkManager : MonoBehaviour, INetworkManager
    {
        private IAppContext _context;
        private IStateLogic<IAppContext> _stateLogic;
        private IDataContainer<VersionResult> _versionResult;
        private IDataContainer<PLoginResult> _pLoginResult;
        private IDataContainer<bool> _billboardRead;
        private IDataContainer<string> _hServerAddress;
        private IDataContainer<ServerAddress> _gServerAddress;

        [Inject]
        public void Initialize(IAppContext context, IDataRepository dataRepository)
        {
            _context = context;
            _versionResult = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
            _pLoginResult = dataRepository.GetContainer<PLoginResult>(DataKey.PLoginResult);
            _billboardRead = dataRepository.GetContainer<bool>(DataKey.BillboardRead);
            _hServerAddress = dataRepository.GetContainer<string>(DataKey.HServerAddress);
            _gServerAddress = dataRepository.GetContainer<ServerAddress>(DataKey.GameServerAddress);

            _stateLogic = new StateLogic<IAppContext>("LoginStateMachine", context);
            _stateLogic.AddState(new NetworkPIPState());
            _stateLogic.AddState(new NetworkBuildFirstCacheState());
            _stateLogic.AddState(new NetworkDownloadResourcesState());
            _stateLogic.AddState(new NetworkConnectGateServerState());
            _stateLogic.AddState(new NetworkClientVersionState());
            _stateLogic.AddState(new NetworkLoginTypeState());
            _stateLogic.AddState(new NetworkLoginGateServerState());
            _stateLogic.AddState(new NetworkConnectHallServerState());
            _stateLogic.AddState(new NetworkLoginHallServerState());
            _stateLogic.AddState(new NetworkLoginHallServerOkState());
            _stateLogic.AddState(new NetworkConnectGameServerState());
            _stateLogic.AddState(new NetworkLoginGameServerState());
            _stateLogic.AddState(new NetworkLoginGameServerOkState());
        }

        private bool _stateError = false;

        private void Update()
        {
            if (_stateLogic == null || _stateError)
            {
                return;
            }

            var result = _stateLogic.Process(Time.time);
            if (result == null)
            {
                return;
            }

            if (result.Result == StateResult.Error)
            {
                _stateError = true;
                _stateLogic.Stop();
                var errMsg = "";
                if (string.IsNullOrEmpty(result.ErrMsg))
                {
                    errMsg = "登录发生错误，请重新登录";
                }
                else
                {
                    errMsg = result.ErrMsg;
                }

                ShowBigReconnectDialog(errMsg);
            }
            else if (result.Result == StateResult.StateNotFound)
            {
                _stateError = true;
                _stateLogic.Stop();
                var errMsg = "";
                if (string.IsNullOrEmpty(result.ErrMsg))
                {
                    errMsg = "登录异常，请重新登录";
                }
                else
                {
                    errMsg = result.ErrMsg;
                }

                ShowBigReconnectDialog(errMsg);
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (_stateLogic != null)
            {
                _stateLogic.OnPause(pause, Time.time);
            }
        }

        public NetworkStatus GetStatus()
        {
            var currentState = _stateLogic.GetCurrentStateCode();

            switch (currentState)
            {
                case NetworkState.PIP:
                    return NetworkStatus.PIP;

                case NetworkState.BuildFirstCache:
                    return NetworkStatus.BuildFirstCache;

                case NetworkState.DownloadResources:
                    return NetworkStatus.DownloadResources;

                case NetworkState.ConnectGateServer:
                    return NetworkStatus.ConnectGateServer;

                case NetworkState.ClientVersion:
                    return NetworkStatus.ClientVersion;

                case NetworkState.ChooseLoginType:
                    return NetworkStatus.ChooseLoginType;

                case NetworkState.LoginGateServer:
                    return NetworkStatus.LoginGateServer;

                case NetworkState.ConnectHallServer:
                    return NetworkStatus.ConnectHallServer;

                case NetworkState.LoginHallServer:
                    return NetworkStatus.LoginHallServer;

                case NetworkState.LoginHallServerOk:
                    return NetworkStatus.LoginHallServerOk;

                case NetworkState.ConnectGameServer:
                    return NetworkStatus.ConnectGameServer;

                case NetworkState.LoginGameServer:
                    return NetworkStatus.LoginGameServer;
                    
                case NetworkState.LoginGameServerOk:
                    return NetworkStatus.LoginGameServerOk;

                default:
                    return NetworkStatus.Null;
            }
        }

        #region Network Info

        private AddressFamily _networkAddressFamily = AddressFamily.InterNetwork;

        public AddressFamily GetNetworkAddressFamily()
        {
            return _networkAddressFamily;
        }

        #endregion

        #region 连接注销获取server类型

        public Server GetServer()
        {
            if (_socket == null)
            {
                return Server.Null;
            }

            return _socket.GetServer();
        }

        #region Host Port

        public string GetHost()
        {
            var pip = _context.GetPIPLogic();
            if (pip == null)
            {
                return null;
            }

            return pip.GetHost();
        }

        public int GetPort()
        {
            var pip = _context.GetPIPLogic();
            if (pip == null)
            {
                return 11112;
            }

            return pip.GetPort();
        }

        #endregion

        /// <summary>
        /// 开始连接网络。
        /// </summary>
        public void Startup()
        {
            _stateError = false;
            _stateLogic.SwitchTo(NetworkState.PIP);
            _stateLogic.Start();
        }

        public void InitLogin()
        {
            Close();

            ClearPUData();

            var app = _context.GetAppController();
            app.ClearAppStateData();

            Startup();
        }

        /// <summary>
        /// 注销。
        /// </summary>
        public void Logout()
        {
            ClearPUData();

            var app = _context.GetAppController();
            app.ClearAppStateData();

            LoginRecord.LastLoginType = LoginRecord.NoLogin;
            LoginRecord.SaveAll();

            Startup();
        }

        /// <summary>
        /// 如果正在登陆状态内，则暂停状态机的运行，如果已经登陆成功了，处于loginok状态，则不会刷新心跳
        /// </summary>
        public void AbortLogin()
        {
            _stateLogic.Stop();
        }

        public void StartConnectGServer()
        {
            _stateError = false;
            _stateLogic.SwitchTo(NetworkState.ConnectGameServer);
            _stateLogic.Start();
        }

        public void StartConnectHServer()
        {
            _stateError = false;
            _stateLogic.SwitchTo(NetworkState.ConnectHallServer);
            _stateLogic.Start();
        }

        #endregion

        #region Socket连接

        /// <summary>
        /// 连接用的客户端。
        /// </summary>
        private ISocketClient _socket;

        /// <summary>
        /// 连接指定的服务器。
        /// </summary>
        public bool Connect(Server server, AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            _networkAddressFamily = addressFamily;

            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }

            if (server == Server.Null ||
                server == Server.CServer)
            {
                // 其他的服务器就不用连接了。
                return false;
            }

            var hServerAddress = _hServerAddress.Read();
            var gServerAddress = _gServerAddress.Read();

            string host = "";
            int port = 0;

            switch (server)
            {
                case Server.PServer:
                    host = GetHost();
                    port = GetPort();
                    break;

                case Server.HServer:
                    host = hServerAddress;
                    port = 11122;
                    break;

                case Server.GServer:
                    
                    if (gServerAddress == null)
                    {
                        MyLog.ErrorWithFrame(
                            name,
                            string.Format("GameServer Address Error ")
                        );

                        return false;
                    }

                    host = gServerAddress.Ip;
                    port = gServerAddress.Port;
                    break;

                default:
                    break;
            }

            MyLog.InfoWithFrame(
                name,
                string.Format("Connect gate server: {0}:{1}",
                    host,
                    port)
            );

#if UNITY_IOS
            var ios = _context.GetIosSDK();

            if (addressFamily == AddressFamily.InterNetworkV6)
            {
                host = ios.GetIpV6(host);
                MyLog.InfoWithFrame(name, string.Format("convert to ipv6: {0}", host));
            }
#endif

            if (string.IsNullOrEmpty(host))
            {
                return false;
            }

            var socketFactory = _context.GetSocketFactory();

            _socket = socketFactory.CreateSocket();
            return _socket.Connect(host, port, server, addressFamily);
        }

        public bool IsConnected()
        {
            if (_socket == null)
            {
                return false;
            }

            return _socket.GetStatus() == SocketStatus.Connected;
        }

        public void Close()
        {
            if (_socket == null)
            {
                return;
            }

            _socket.Close();
            _socket = null;
        }

        #endregion

        private void ShowBigReconnectDialog(string errMsg = "")
        {
            var dialogManager = _context.GetDialogManager();

            dialogManager.ShowDialog<DisconnectedDialog>(DialogName.DisconnectedDialog, false, true,
                (dialog) =>
                {
                    dialog.ApplyData(errMsg);
                    dialog.Show();
                });
        }

        private void ClearPUData()
        {
            _versionResult.ClearNotInvalidate();
            _billboardRead.ClearNotInvalidate();

            _pLoginResult.ClearAndInvalidate(Time.time);
        }
    }
}