using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public class WeChatAdsToHallDataRelation : ChildRelationAdapter<List<WeChatAds>>
    {
        private readonly IDataContainer<HallData> _parent;

        public WeChatAdsToHallDataRelation(IDataContainer<HallData> parent)
        {
            _parent = parent;
        }

        public override List<WeChatAds> Data
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

                var childData = data.wechat_ads;
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