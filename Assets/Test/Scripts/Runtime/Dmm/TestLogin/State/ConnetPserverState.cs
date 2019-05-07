using System.Net.Sockets;
using Dmm.App;
using Dmm.Msg;
using Dmm.StateLogic;

namespace Test.Scripts.Runtime.Dmm.TestLogin.State
{
    public class ConnetPserverState : StateAdapter<IAppContext>
    {
        private const string Tag = "ConnetPserverState";

        public override int GetStateCode()
        {
            return TestLoginStateCode.ConnetPserverState;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            var addressFamily = AddressFamily.InterNetwork;
            var network = context.GetNetworkManager();
            network.Connect(Server.PServer, addressFamily);
        }

        public override bool Process(IAppContext context, float time)
        {
            var network = context.GetNetworkManager();
            var isConnected = network.IsConnected();

            return isConnected;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var dialog = context.GetDialogManager();
            dialog.ShowToast("连接PServer成功",2);
            var stateResult = new StateResult();
            stateResult.NextStateCode = TestLoginStateCode.SetClientVersionState;
            stateResult.Result = StateResult.Ok;
            return stateResult;
        }
    }
}