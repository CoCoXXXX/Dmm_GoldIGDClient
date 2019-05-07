using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Network;
using Dmm.Widget;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dmm.Shop
{
    public class VipTab : ItemList<VipExchange>
    {
        #region Inject

        private RemoteAPI _remoteAPI;

        private VipItem.Factory _itemFactory;

        private IDataContainer<VipExchangeListResult> _vipExchangeListResult;

        [Inject]
        public void Initialize(IDataRepository dataRepository, RemoteAPI remoteAPI, VipItem.Factory itemFactory)
        {
            _remoteAPI = remoteAPI;
            _itemFactory = itemFactory;
            _vipExchangeListResult = dataRepository.GetContainer<VipExchangeListResult>(DataKey.VipExchangeListResult);
        }

        #endregion

        // 如果HUData中不存在VipExchange数据，则直接请求。
        // 如果进行了续费或者升级之类的操作，则直接请求新的VIP数据。

        public Text WaitingText;

        public GameObject TipGroup;

        public Text TipText;

        public void OnEnable()
        {
            var res = _vipExchangeListResult.Read();
            if (res == null)
            {
                // 如果当前不存在VipExchange数据的话，想服务器发请求。
                _remoteAPI.RequestVipExchangeList();

                if (TipGroup && TipGroup.activeSelf)
                    TipGroup.SetActive(false);

                if (WaitingText && !WaitingText.gameObject.activeSelf)
                    WaitingText.gameObject.SetActive(true);
            }
            else
            {
                if (TipGroup && !TipGroup.activeSelf)
                    TipGroup.SetActive(true);

                if (WaitingText && WaitingText.gameObject.activeSelf)
                    WaitingText.gameObject.SetActive(false);
            }
        }

        public override void OnUpdate()
        {
            if (WaitingText && WaitingText.gameObject.activeSelf)
            {
                WaitingText.text = "正在请求数据";
                var count = (int) Time.time % 3 + 1;
                for (int i = 0; i < count; i++)
                    WaitingText.text += ".";
            }
        }

        public override void BeforeRefresh()
        {
            SelectEmpty();

            var res = _vipExchangeListResult.Read();
            if (res == null)
            {
                if (TipGroup && TipGroup.activeSelf)
                    TipGroup.SetActive(false);

                if (WaitingText && !WaitingText.gameObject.activeSelf)
                    WaitingText.gameObject.SetActive(true);
            }
            else
            {
                if (TipGroup && !TipGroup.activeSelf)
                    TipGroup.SetActive(true);

                if (WaitingText && WaitingText.gameObject.activeSelf)
                    WaitingText.gameObject.SetActive(false);

                if (TipText)
                {
                    if (res.current_vip_level > 0)
                    {
                        TipText.text = string.Format(
                            "您是尊敬的 <color=orange>{0}</color> 有效期 <color=orange>{1}</color> 天",
                            GetVipLabel(res.current_vip_level),
                            res.left_vip_days);
                    }
                    else
                    {
                        TipText.text = "您还不是VIP，尚未拥有VIP的各项特权";
                    }
                }
            }
        }

        private string GetVipLabel(int level)
        {
            switch (level)
            {
                case 1:
                    return "普通VIP";

                case 2:
                    return "白银VIP";

                case 3:
                    return "黄金VIP";

                case 4:
                    return "钻石VIP";

                case 5:
                    return "至尊VIP";

                default:
                    return "非VIP";
            }
        }

        public override int SlotCount()
        {
            return VipExchangeCount();
        }

        public override Item<VipExchange> CreateItem()
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

        public override float DataUpdateTime()
        {
            return _vipExchangeListResult.Timestamp;
        }

        public override int DataCount()
        {
            return VipExchangeCount();
        }

        public override VipExchange GetData(int index)
        {
            return GetVipExchange(index);
        }

        public override void OnItemSelected(Item<VipExchange> item)
        {
        }

        public List<VipExchange> VipExchangeList()
        {
            var data = _vipExchangeListResult.Read();
            if (data == null)
                return null;

            return data.exchange;
        }

        public int VipExchangeCount()
        {
            var list = VipExchangeList();
            if (list == null)
                return 0;

            return list.Count;
        }

        public VipExchange GetVipExchange(int index)
        {
            var list = VipExchangeList();
            if (list == null)
                return null;

            if (index < 0 || index >= list.Count)
                return null;

            return list[index];
        }
    }
}