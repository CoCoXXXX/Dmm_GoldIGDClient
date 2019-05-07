using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class ChooseRoomResultToHLoginResultRelation : AutoCleanRelationAdapter<ChooseRoomResult>
    {
        private readonly IDataContainer<HLoginResult> _hLoginResult;

        private ChooseRoomResult _chooseRoomResult;

        public ChooseRoomResultToHLoginResultRelation(IDataContainer<HLoginResult> hLoginResult)
        {
            _hLoginResult = hLoginResult;
        }

        protected override ChooseRoomResult DataContent
        {
            get
            {
                var loginResultData = _hLoginResult.Read();

                if (loginResultData == null || loginResultData.result != ResultCode.OK)
                {
                    _chooseRoomResult = null;
                }

                return _chooseRoomResult;
            }
            set { _chooseRoomResult = value; }
        }

        protected override float ParentTimestamp
        {
            get { return _hLoginResult.Timestamp; }
        }
    }
}