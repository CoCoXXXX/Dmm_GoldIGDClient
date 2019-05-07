using Dmm.App;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.StateLogic;
using Dmm.Widget;
using UnityEngine;

namespace Test.Scripts.Runtime.Dmm.TestLogin.State
{
    public class SetClientVersionState : StateAdapter<IAppContext>
    {
        private const string Tag = "SetClientVersionState";
        
        private IDataContainer<bool> _isSetTestClientVersion;

        public override int GetStateCode()
        {
            return TestLoginStateCode.SetClientVersionState;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {
            var dialog = context.GetDialogManager();
            var dataReposity = context.GetDataRepository();
            _isSetTestClientVersion = dataReposity.GetContainer<bool>(DataKey.IsSetTestClientVersion);
            
            _isSetTestClientVersion.Write(false,Time.time);
            
            dialog.ShowDialog<UIWindow>(DialogName.SetClientVersionDialog);
        }

        public override bool Process(IAppContext context, float time)
        {
            var isSetTestClientVersion = _isSetTestClientVersion.Read();
            return isSetTestClientVersion;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            var stateResult = new StateResult();
            stateResult.NextStateCode = TestLoginStateCode.GetClientVersionState;
            stateResult.Result = StateResult.Ok;
            return stateResult;
        }
    }
}