using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.DataContainer;
using Dmm.Msg;
using Dmm.Widget;
using UnityEngine;
using Zenject;

namespace Dmm.Left
{
    public class FriendList : ItemList<FriendInfo>
    {
        #region Inject

        private FriendItem.Factory _itemFactory;

        private IDataContainer<SFriendListResult> _sFriendListResult;

        [Inject]
        public void Initialize(
            IDataRepository dataRepository,
            FriendItem.Factory itemFactory)
        {
            _sFriendListResult = dataRepository.GetContainer<SFriendListResult>(DataKey.SFriendListResult);
            _itemFactory = itemFactory;
        }

        #endregion

        private List<FriendInfo> GetFriendList()
        {
            var friendListResult = _sFriendListResult.Read();
            if (friendListResult == null || friendListResult.result.code != ResultCode.OK)
            {
                return null;
            }
            return friendListResult.info;
        }

        public override int SlotCount()
        {
            var list = GetFriendList();
            if (list == null)
            {
                return 0;
            }
            return list.Count;
        }

        public override float DataUpdateTime()
        {
            return _sFriendListResult.Timestamp;
        }

        public override int DataCount()
        {
            var list = GetFriendList();
            if (list == null)
            {
                return 0;
            }
            return list.Count;
        }

        public override FriendInfo GetData(int index)
        {
            var list = GetFriendList();
            if (list == null)
            {
                return null;
            }
            if (index < 0 || index >= list.Count)
            {
                return null;
            }

            return list[index];
        }

        public override Item<FriendInfo> CreateItem()
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

        public override void BeforeRefresh()
        {
            SelectEmpty();
        }

        public override void OnItemSelected(Item<FriendInfo> item)
        {
        }
    }
}