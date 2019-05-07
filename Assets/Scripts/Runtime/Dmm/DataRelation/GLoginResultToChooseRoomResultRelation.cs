using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class GLoginResultToChooseRoomResultRelation : AutoCleanRelationAdapter<GLoginResult>
    {
        private readonly IDataContainer<ChooseRoomResult> _chooseRoomResult;

        private GLoginResult _gLoginResult;

        public GLoginResultToChooseRoomResultRelation(IDataContainer<ChooseRoomResult> chooseRoomResult)
        {
            _chooseRoomResult = chooseRoomResult;
        }

        protected override GLoginResult DataContent
        {
            get
            {
                var chooseRoomResult = _chooseRoomResult.Read();
                if (chooseRoomResult == null || chooseRoomResult.result != ResultCode.OK)
                {
                    _gLoginResult = null;
                }

                return _gLoginResult;
            }
            set { _gLoginResult = value; }
        }

        protected override float ParentTimestamp
        {
            get { return _chooseRoomResult.Timestamp; }
        }
    }
}