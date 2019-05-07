using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class HServerAddressToPLoginResultRelation : ChildRelationAdapter<string>
    {
        private readonly IDataContainer<PLoginResult> _parent;

        public HServerAddressToPLoginResultRelation(IDataContainer<PLoginResult> parent)
        {
            _parent = parent;
        }

        public override string Data
        {
            get
            {
                if (_parent == null)
                {
                    return null;
                }

                var data = _parent.Read();
                if (data == null)
                {
                    return null;
                }

                if (data.result != ResultCode.OK)
                {
                    return null;
                }

                var childData = data.hall_server_addr;
                return childData;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}