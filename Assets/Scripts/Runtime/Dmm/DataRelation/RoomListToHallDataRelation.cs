using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class RoomListToHallDataRelation : ChildRelationAdapter<List<Room>>
    {
        private readonly IDataContainer<HallData> _parent;

        public RoomListToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override List<Room> Data
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

                var childData = data.room;
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