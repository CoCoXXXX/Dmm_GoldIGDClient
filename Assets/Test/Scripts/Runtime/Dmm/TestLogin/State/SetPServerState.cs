using Dmm.App;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.StateLogic;
using Dmm.Widget;
using UnityEngine;

namespace Test.Scripts.Runtime.Dmm.TestLogin.State
{
    public class SetPServerState : StateAdapter<IAppContext> 
    {
        private const string Tag = "SetPServerState";

        private IDataContainer<bool> _isSetTestLogin;
        
        public override int GetStateCode()
        {
            return TestLoginStateCode.SetPServerState;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            var dialog = context.GetDialogManager();
            var dataReposity = context.GetDataRepository();
            _isSetTestLogin = dataReposity.GetContainer<bool>(DataKey.IsSetTestPServer);
            
            _isSetTestLogin.Write(false,Time.time);
            
            dialog.ShowDialog<UIWindow>(DialogName.SetPServerDialog);
        }

        public override bool Process(IAppContext context, float time)
        {
            var isSetTestLogin = _isSetTestLogin.Read();
            return isSetTestLogin;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var stateResult = new StateResult();
            stateResult.NextStateCode = TestLoginStateCode.ConnetPserverState;
            stateResult.Result = StateResult.Ok;
            return stateResult;
        }
    }
}
