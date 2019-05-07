using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class ChooseTableResultToCurrentRoomRelation : AutoCleanRelationAdapter<ChooseTableResult>
    {
        private readonly IDataContainer<Room> _room;

        private ChooseTableResult _chooseTableResult;

        public ChooseTableResultToCurrentRoomRelation(IDataContainer<Room> room)
        {
            _room = room;
        }

        protected override ChooseTableResult DataContent
        {
            get
            {
                var currenRoom = _room.Read();
                if (currenRoom == null)
                {
                    _chooseTableResult = null;
                }

                return _chooseTableResult;
            }
            set { _chooseTableResult = value; }
        }

        protected override float ParentTimestamp
        {
            get { return _room.Timestamp; }
        }
    }
}