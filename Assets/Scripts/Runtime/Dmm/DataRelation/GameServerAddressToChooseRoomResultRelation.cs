using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class GameServerAddressToChooseRoomResultRelation : ChildRelationAdapter<ServerAddress>
    {
        private readonly IDataContainer<ChooseRoomResult> _parent;

        public GameServerAddressToChooseRoomResultRelation(IDataContainer<ChooseRoomResult> parent)
        {
            _parent = parent;
        }

        public override ServerAddress Data
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

                var addr = data.game_server_addr;
                if (string.IsNullOrEmpty(addr))
                {
                    return null;
                }

                var addrArray = addr.Split(':');
                if (addrArray == null || addrArray.Length != 2)
                {
                    return null;
                }

                var childData = new ServerAddress();

                int port = 0;
                if (int.TryParse(addrArray[1], out port))
                {
                    childData.Port = port;
                }
                else
                {
                    return null;
                }

                childData.Ip = addrArray[0];

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