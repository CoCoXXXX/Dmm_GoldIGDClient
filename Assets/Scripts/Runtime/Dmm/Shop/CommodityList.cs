using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Shop
{
    public class CommodityList : ItemList<Commodity>
    {
        #region Inject

        private CommodityItem.Factory _itemFactory;

        private IDataContainer<Bag> _bag;

        private IDataContainer<List<Commodity>> _commodityListContainer;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            CommodityItem.Factory itemFactory)
        {
            _itemFactory = itemFactory;
            _bag = dataRepository.GetContainer<Bag>(DataKey.MyBag);
            _commodityListContainer = dataRepository.GetContainer<List<Commodity>>(DataKey.CommodityList);
        }

        #endregion

        public CommodityTab CommodityTab;

        public int CommodityType
        {
            get { return _commodityType; }
            set
            {
                _commodityType = value;
                ExtractCommodity();
            }
        }

        private int _commodityType = Constant.CommodityType.Hair;

        private readonly List<Commodity> _commodityList = new List<Commodity>();

        public void OnEnable()
        {
            RefreshContent();
        }

        public override void OnUpdate()
        {
        }

        public override void BeforeRefresh()
        {
            if (CommodityTab)
                CommodityTab.SelectCommodity(null);

            ExtractCommodity();
        }

        private void ExtractCommodity()
        {
            _commodityList.Clear();
            var commodityList = _commodityListContainer.Read();
            if (commodityList == null)
            {
                return;
            }
            for (int i = 0; i < commodityList.Count; i++)
            {
                var c = commodityList[i];
                if (c == null || c.type != CommodityType)
                {
                    continue;
                }

                if (c.hide_sale)
                {
                    var bag = _bag.Read();
                    if (!GameUtil.HasCommodity(bag, c.name))
                    {
                        // 被删除的，我没有的商品，不显示。
                        continue;
                    }
                }

                _commodityList.Add(c);
            }
        }

        public override int SlotCount()
        {
            return _commodityList.Count;
        }

        public override Item<Commodity> CreateItem()
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
            return _bag.Timestamp;
        }

        public override int DataCount()
        {
            return _commodityList.Count;
        }

        public override Commodity GetData(int index)
        {
            return _commodityList[index];
        }

        public override void OnItemSelected(Item<Commodity> item)
        {
            if (!item) return;

            var data = item.GetData();
            if (CommodityTab)
                CommodityTab.SelectCommodity(data);
        }
    }
}