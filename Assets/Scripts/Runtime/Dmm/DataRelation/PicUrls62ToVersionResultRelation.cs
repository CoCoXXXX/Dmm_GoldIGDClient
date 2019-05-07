using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class PicUrls62ToVersionResultRelation : ChildRelationAdapter<List<string>>
    {
        private readonly IDataContainer<VersionResult> _parent;

        public PicUrls62ToVersionResultRelation(IDataContainer<VersionResult> parent)
        {
            _parent = parent;
        }

        public override List<string> Data
        {
            get
            {
                if (_parent == null)
                {
                    return null;
                }

                var data = _parent.Read();
                if (data == null ||
                    data.result != ResultCode.OK)
                    return null;

                var picUrls = data.pic_urls_6_2;
                if (picUrls == null)
                    return null;

                return picUrls.url;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}