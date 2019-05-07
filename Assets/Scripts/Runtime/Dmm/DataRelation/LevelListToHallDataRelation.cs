using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class LevelListToHallDataRelation : ChildRelationAdapter<List<Level>>
    {
        private readonly IDataContainer<HallData> _parent;

        public LevelListToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override List<Level> Data
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

                var childData = data.level;
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