using Dmm.App;
using Dmm.Data;
using Dmm.StateLogic;

namespace Test.Scripts.Runtime.Dmm.TestLogin.State
{
    public class SelectLoginTypeSate : StateAdapter<IAppContext>
    {
        private const string Tag = "SelectLoginTypeSate";

        public override int GetStateCode()
        {
            return TestLoginStateCode.SelectLoginTypeSate;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext data, float time)
        {
        }

        public override bool Process(IAppContext data, float time)
        {
            return LoginRecord.CurrentLoginType != LoginRecord.NoLogin;
        }

        public override StateResult Finish(IAppContext data, float time)
        {
            var stateResult = new StateResult();
            stateResult.NextStateCode = TestLoginStateCode.LoginPServerState;
            stateResult.Result = StateResult.Ok;
            return stateResult;
        }
    }
}