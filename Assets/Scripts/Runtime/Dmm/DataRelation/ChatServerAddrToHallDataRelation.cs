using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class ChatServerAddrToHallDataRelation : ChildRelationAdapter<string>
    {
        private readonly IDataContainer<HallData> _parentData;

        public ChatServerAddrToHallDataRelation(IDataContainer<HallData> parentData)
        {
            _parentData = parentData;
        }

        public override string Data
        {
            get
            {
                if (_parentData == null)
                {
                    return null;
                }

                var data = _parentData.Read();
                if (data == null)
                {
                    return null;
                }

                var childData = data.chat_server_addr;
                return childData;
            }
            set
            {
                if (_parentData == null)
                {
                    return;
                }

                var data = _parentData.Read();
                if (data == null)
                {
                    return;
                }

                data.chat_server_addr = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _parentData.Timestamp; }
        }
    }
}