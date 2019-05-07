using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class UserToHLoginResultRelation : ChildRelationAdapter<User>
    {
        private readonly IDataContainer<HLoginResult> _parent;

        public UserToHLoginResultRelation(IDataContainer<HLoginResult> parent)
        {
            _parent = parent;
        }

        public override User Data
        {
            get
            {
                var loginResult = _parent.Read();
                if (loginResult == null || loginResult.result != ResultCode.OK)
                {
                    return null;
                }

                return loginResult.user;
            }
            set
            {
                if (_parent == null)
                {
                    return;
                }

                var loginResult = _parent.Read();
                if (loginResult == null || loginResult.result != ResultCode.OK)
                {
                    return;
                }

                loginResult.user = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}