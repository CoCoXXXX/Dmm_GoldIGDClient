using System.Net.Sockets;
using Dmm.Msg;

namespace Dmm.Network
{
    public interface INetworkManager
    {
        void Startup();
        void InitLogin();
        void Logout();
        void AbortLogin();
        void StartConnectGServer();
        void StartConnectHServer();

        Server GetServer();
        AddressFamily GetNetworkAddressFamily();

        int GetPort();
        string GetHost();

        NetworkStatus GetStatus();
        bool Connect(Server server, AddressFamily addressFamily = AddressFamily.InterNetwork);
        void Close();

        bool IsConnected();
    }
}