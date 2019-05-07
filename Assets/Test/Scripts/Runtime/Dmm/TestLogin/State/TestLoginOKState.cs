using Dmm.App;
using Dmm.StateLogic;

namespace Test.Scripts.Runtime.Dmm.TestLogin.State
{
    public class TestLoginOKState :StateAdapter<IAppContext>
    {
        private const string Tag = "TestLoginOKState";
        
        public override int GetStateCode()
        {
            return TestLoginStateCode.TestLoginOKState;
        }

        public override string GetStateName()
        {
            return Tag;
        }

        public override void Initialize(IAppContext context, float time)
        {

        }

        public override bool Process(IAppContext context, float time)
        {
            return false;
        }

        public override StateResult Finish(IAppContext context, float time)
        {
            return null;
        }
    }
}