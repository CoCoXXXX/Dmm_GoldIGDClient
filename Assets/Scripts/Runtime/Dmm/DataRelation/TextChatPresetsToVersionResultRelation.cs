using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Msg;

namespace Dmm.DataRelation
{
    public class TextChatPresetsToVersionResultRelation : ChildRelationAdapter<List<string>>
    {
        private readonly IDataContainer<VersionResult> _rootData;

        public TextChatPresetsToVersionResultRelation(IDataContainer<VersionResult> rootData)
        {
            _rootData = rootData;
        }

        public override List<string> Data
        {
            get
            {
                if (_rootData == null)
                {
                    return DefaultData.ChatTextPresets;
                }

                var data = _rootData.Read();
                if (data == null ||
                    data.result != ResultCode.OK)
                    return DefaultData.ChatTextPresets;

                var textPresets = data.text_chat_presets;
                if (textPresets == null)
                    return DefaultData.ChatTextPresets;

                var list = textPresets.chat_msg;
                if (list == null || list.Count <= 0)
                    return DefaultData.ChatTextPresets;

                return list;
            }
            set { }
        }

        protected override float ParentTimestamp
        {
            get { return _rootData.Timestamp; }
        }
    }
}