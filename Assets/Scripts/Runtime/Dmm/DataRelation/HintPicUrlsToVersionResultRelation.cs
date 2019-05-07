using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class HintPicUrlsToVersionResultRelation : ChildRelationAdapter<List<string>>
    {
        private readonly IDataContainer<VersionResult> _parent;

        public HintPicUrlsToVersionResultRelation(IDataContainer<VersionResult> parent)
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
                {
                    return null;
                }

                var hintPicUrls = data.hint_pic_urls;
                if (hintPicUrls == null)
                {
                    return null;
                }

                return hintPicUrls.url;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _parent.Timestamp; }
        }
    }
}