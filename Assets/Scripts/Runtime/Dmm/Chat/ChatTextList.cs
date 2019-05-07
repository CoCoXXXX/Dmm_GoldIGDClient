using System.Collections.Generic;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Chat
{
    public class ChatTextList : ItemList<string>
    {
        private IDataContainer<List<string>> _container;

        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            RemoteAPI remoteAPI)
        {
            _container = dataRepository.GetContainer<List<string>>(DataKey.TextChatPresets);
            _dialogManager = dialogManager;
            _remoteAPI = remoteAPI;
        }

        #endregion

        public TextItem TextItemPrefab;

        public ScrollRect ScrollRect;

        private void OnEnable()
        {
            RefreshContent();

            ScrollRect.velocity = new Vector2(0, -1000);
        }

        public override void BeforeRefresh()
        {
            SelectEmpty();
        }

        public override int SlotCount()
        {
            var textChatPresetCount = _container.Read().Count;
            return textChatPresetCount;
        }

        public override Item<string> CreateItem()
        {
            return Instantiate(TextItemPrefab) as TextItem;
        }

        public override bool HasDivider()
        {
            return false;
        }

        public override GameObject CreateDivider()
        {
            return null;
        }

        public override float DataUpdateTime()
        {
            return _container.Timestamp;
        }

        public override int DataCount()
        {
            var textChatPresetCount = _container.Read().Count;
            return textChatPresetCount;
        }

        public override string GetData(int index)
        {
            var list = _container.Read();
            if (index < 0 || index >= list.Count)
            {
                return null;
            }

            return list[index];
        }

        public override void OnItemSelected(Item<string> item)
        {
            if (!item)
            {
                return;
            }

            var data = item.GetData();
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            _remoteAPI.SendTextMsg(data);
            _dialogManager.HideDialog(DialogName.ChatPanel);
        }
    }
}