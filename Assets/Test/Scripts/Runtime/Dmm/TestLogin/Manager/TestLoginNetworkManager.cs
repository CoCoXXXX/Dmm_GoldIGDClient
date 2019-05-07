using System.Net.Sockets;
using com.morln.game.gd.command;
using Dmm.App;
using Dmm.Common;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Log;
using Dmm.Msg;
using Dmm.Network;
using Dmm.Session;
using Dmm.StateLogic;
using Dmm.Util;
using Test.Scripts.Runtime.Dmm.TestLogin.Record;
using Test.Scripts.Runtime.Dmm.TestLogin.State;
using UnityEngine;
using Zenject;

namespace Test.Scripts.Runtime.Dmm.TestLogin
{
    public class TestLoginNetworkManager : MonoBehaviour,INetworkManager
    {
        private IAppContext _context;
        private IStateLogic<IAppContext> _stateLogic;

        private IDataContainer<VersionResult> _versionResult;
        private IDataContainer<PLoginResult> _pLoginResult;
        private IDataContainer<bool> _billboardRead;
        private IDataContainer<string> _hServerAddressUser;

        [Inject]
        public void Initialize(IAppContext context, IDataRepository dataRepository)
        {
            _context = context;
            _versionResult = dataRepository.GetContainer<VersionResult>(DataKey.VersionResult);
            _pLoginResult = dataRepository.GetContainer<PLoginResult>(DataKey.PLoginResult);
            _billboardRead = dataRepository.GetContainer<bool>(DataKey.BillboardRead);
            _hServerAddressUser = dataRepository.GetContainer<string>(DataKey.HServerAddress);

            _stateLogic = new StateLogic<IAppContext>("TestLoginStateMachine", context);
            _stateLogic.AddState(new TestBuildFirstAssetBundleState());
            _stateLogic.AddState(new SetPServerState());
            _stateLogic.AddState(new ConnetPserverState());
            _stateLogic.AddState(new SetClientVersionState());
            _stateLogic.AddState(new GetClientVersionState());
            _stateLogic.AddState(new SelectLoginTypeSate());
            _stateLogic.AddState(new LoginPServerState());
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

        #region 连接注销获取server类型

        public void StartConnectGServer()
        {
        }

        public void StartConnectHServer()
        {
        }

        public Server GetServer()
        {
            if (_socket == null)
            {
                return Server.Null;
            }

            return _socket.GetServer();
        }

        public AddressFamily GetNetworkAddressFamily()
        {
            return AddressFamily.Unknown;
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

        public NetworkStatus GetStatus()
        {
            throw new System.NotImplementedException();
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
            _stateLogic.SwitchTo(TestLoginStateCode.TestBuildFirstAssetBundleState);
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
            if (server == Server.PServer)
            {
                var ip = PrefsUtil.GetString(TestLoginRecord.PServerTestLoginIp, null);
                var port = PrefsUtil.GetInt(TestLoginRecord.PServerTestLoginPort, 0);

                if (string.IsNullOrEmpty(ip) || port == 0)
                {
                    return false;
                }
                var socketFactory = _context.GetSocketFactory();

                _socket = socketFactory.CreateSocket();
                return _socket.Connect(ip, port, server, addressFamily);   
            }

            return false;
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