using com.morln.game.gd.command;
using Dmm.Data;
using Dmm.DataContainer;
using Dmm.Game;
using Dmm.Util;
using UnityEngine;

namespace Dmm.DataRelation
{
    public class TableUserDataRelation : ChildRelationAdapter<TableUserData>
    {
        private readonly IDataContainer<Table> _currentTable;

        private readonly IDataContainer<User> _myUser;

        public TableUserDataRelation(IDataContainer<Table> currentTable, IDataContainer<User> myUser)
        {
            _currentTable = currentTable;
            _myUser = myUser;
        }

        public override TableUserData Data
        {
            get
            {
                var table = _currentTable.Read();
                var tableUserData = new TableUserData();

                if (table == null)
                {
                    return tableUserData;
                }

                var user = _myUser.Read();

                tableUserData.MySeat = MySeat();
                tableUserData.User0 = GetTableUser(table, user, 0);
                tableUserData.User1 = GetTableUser(table, user, 1);
                tableUserData.User2 = GetTableUser(table, user, 2);
                tableUserData.User3 = GetTableUser(table, user, 3);

                return tableUserData;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                var table = _currentTable.Read();
                if (table == null)
                {
                    return;
                }

                GameUtil.SetTableUser(table, 0, value.User0);
                GameUtil.SetTableUser(table, 1, value.User1);
                GameUtil.SetTableUser(table, 2, value.User2);
                GameUtil.SetTableUser(table, 3, value.User3);
            }
        }

        private User GetTableUser(Table table, User user, int seat)
        {
            // 如果获取的是自己的数据，直接返回自己的数据。
            if (seat == MySeat())
            {
                return user;
            }

            if (table == null) return null;

            switch (seat)
            {
                case 0:
                    return table.user1;

                case 1:
                    return table.user2;

                case 2:
                    return table.user3;

                case 3:
                    return table.user4;

                default:
                    return null;
            }
        }

        private int MySeat()
        {
            var user = _myUser.Read();
            if (user == null)
            {
                return -1;
            }

            var userName = user.username;
            var table = _currentTable.Read();

            if (table == null)
            {
                return -1;
            }

            if (table.user1 != null && StringUtil.AreEqual(table.user1.username, userName))
            {
                return 0;
            }

            if (table.user2 != null && StringUtil.AreEqual(table.user2.username, userName))
            {
                return 1;
            }

            if (table.user3 != null && StringUtil.AreEqual(table.user3.username, userName))
            {
                return 2;
            }

            if (table.user4 != null && StringUtil.AreEqual(table.user4.username, userName))
            {
                return 3;
            }

            return -1;
        }

        protected override float ParentTimestamp
        {
            get { return Mathf.Max(_currentTable.Timestamp, _myUser.Timestamp); }
        }
    }
}