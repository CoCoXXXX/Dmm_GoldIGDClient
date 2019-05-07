using System.Net.Sockets;
using Dmm.Msg;
using Dmm.Network;

namespace Test.Scripts.Runtime.Dmm.TestLogin
{
	public interface ITestLoginNetworkManager {

		void Startup();
		void InitLogin();
		void Logout();
		void AbortLogin();

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
