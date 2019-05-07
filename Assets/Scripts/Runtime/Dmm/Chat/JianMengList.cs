using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Analytic;
using Dmm.DataContainer;
using Dmm.Dialog;
using Dmm.Network;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Chat
{
    public class JianMengList : ItemList<JianMengItem>
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private IDialogManager _dialogManager;

        private JianMengChatItem.Factory _itemFactory;

        private IAnalyticManager _analyticManager;

        private IDataContainer<List<JianMengItem>> _jianMengList;

        private IDataContainer<FeatureSwitch> _featureSwitch;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            IDialogManager dialogManager,
            IAnalyticManager analyticManager,
            RemoteAPI remoteAPI,
            JianMengChatItem.Factory itemFactory)
        {
            _jianMengList = dataRepository.GetContainer<List<JianMengItem>>(DataKey.JianMengItemList);
            _featureSwitch = dataRepository.GetContainer<FeatureSwitch>(DataKey.FeatureSwitch);

            _remoteAPI = remoteAPI;
            _dialogManager = dialogManager;
            _itemFactory = itemFactory;
            _analyticManager = analyticManager;
        }

        #endregion

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            RefreshContent();
            SetRefreshTime(Time.time);
        }

        private readonly List<JianMengItem> _jianMengItems = new List<JianMengItem>();

        public override void BeforeRefresh()
        {
            SelectEmpty();

            _jianMengItems.Clear();

            var list = _jianMengList.Read();
            if (list == null || list.Count <= 0)
            {
                return;
            }

            var featureSwitch = _featureSwitch.Read();
            foreach (var item in list)
            {
                if (featureSwitch != null && !featureSwitch.vip && item.vip > 0)
                {
                    // 关闭VIP的时候，不显示VIP专用的贱萌表情。
                    continue;
                }

                _jianMengItems.Add(item);
            }
        }

        public override int SlotCount()
        {
            return _jianMengItems.Count;
        }

        public override float DataUpdateTime()
        {
            return _jianMengList.Timestamp;
        }

        public override int DataCount()
        {
            return _jianMengItems.Count;
        }

        public override JianMengItem GetData(int index)
        {
            if (index < 0 || index >= _jianMengItems.Count)
            {
                return null;
            }

            return _jianMengItems[index];
        }

        public override void OnItemSelected(Item<JianMengItem> item)
        {
            if (item == null)
            {
                return;
            }

            var data = item.GetData();
            if (data == null)
            {
                return;
            }

            _remoteAPI.SendJianMeng(data.cmd);
            _dialogManager.HideDialog(DialogName.ChatPanel);

            var attrs = new Dictionary<string, string>();
            attrs.Add("cmd", data.cmd);
            _analyticManager.Event("jianmeng_send", attrs);
        }

        public override Item<JianMengItem> CreateItem()
        {
            return _itemFactory.Create();
        }

        public override bool HasDivider()
        {
            return false;
        }

        public override GameObject CreateDivider()
        {
            return null;
        }
    }
}