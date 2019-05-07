using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class HintItemListToHallDataRelation : ChildRelationAdapter<List<HintItem>>
    {
        private readonly IDataContainer<HallData> _parent;

        public HintItemListToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override List<HintItem> Data
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

                var childData = data.hint_item;
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