using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class CurrentRoomToGLoginResultRelation : ChildRelationAdapter<Room>
    {
        private readonly IDataContainer<GLoginResult> _gLoginResult;

        public CurrentRoomToGLoginResultRelation(IDataContainer<GLoginResult> parent)
        {
            _gLoginResult = parent;
        }

        public override Room Data
        {
            get
            {
                var loginRes = _gLoginResult.Read();
                if (loginRes == null)
                {
                    return null;
                }

                if (loginRes.result != ResultCode.OK)
                {
                    return null;
                }

                return loginRes.room;
            }
            set
            {
                var loginRes = _gLoginResult.Read();
                if (loginRes == null)
                {
                    return;
                }

                if (loginRes.result != ResultCode.OK)
                {
                    return;
                }

                loginRes.room = value;
            }
        }

        protected override float ParentTimestamp
        {
            get { return _gLoginResult.Timestamp; }
        }
    }
}