using System.Collections.Generic;
using com.morln.game.gd.command;
using Dmm.Hall;
using Dmm.Util;
using UnityEngine;

namespace Dmm.Data
{
    public class TableUserData
    {
        public int MySeat = -1;

        public User User0;

        public User User1;

        public User User2;

        public User User3;

        public bool IsMySelf(User user)
        {
            if (user == null)
            {
                return false;
            }

            var myUser = GetUserAtSeat(MySeat);
            if (myUser == null)
            {
                return false;
            }

            return StringUtil.AreEqual(user.username, myUser.username);
        }

        public bool SelfReady()
        {
            var myUser = GetUserAtSeat(MySeat);
            if (myUser == null)
                return false;

            return myUser.ready == 1;
        }

        public int GetMyTeam()
        {
            if (MySeat == 0 || MySeat == 2)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public User GetMyUser()
        {
            return GetUserAtSeat(MySeat);
        }

        public void SetUserAtSeat(User user, int seat)
        {
            switch (seat)
            {
                case 0:
                    User0 = user;
                    break;

                case 1:
                    User1 = user;
                    break;

                case 2:
                    User2 = user;
                    break;

                case 3:
                    User3 = user;
                    break;
            }
        }

        public SeatPosition PositionOfSeat(int seat)
        {
            if (MySeat == -1)
            {
                return SeatPosition.Null;
            }

            if (seat < 0 || seat > 3)
            {
                return SeatPosition.Null;
            }

            var delta = (seat - MySeat + 4) % 4;

            switch (delta)
            {
                case 0:
                    return SeatPosition.Bottom;

                case 1:
                    return SeatPosition.Right;

                case 2:
                    return SeatPosition.Top;

                case 3:
                    return SeatPosition.Left;

                default:
                    return SeatPosition.Null;
            }
        }

        public int SeatOfPosition(SeatPosition pos)
        {
            if (MySeat == -1)
            {
                return -1;
            }

            switch (pos)
            {
                case SeatPosition.Bottom:
                    return MySeat;

                case SeatPosition.Right:
                    return (MySeat + 1) % 4;

                case SeatPosition.Top:
                    return (MySeat + 2) % 4;

                case SeatPosition.Left:
                    return (MySeat + 3) % 4;

                default:
                    return -1;
            }
        }

        public User GetUserAtPos(SeatPosition pos)
        {
            if (MySeat == -1)
            {
                return null;
            }

            switch (pos)
            {
                case SeatPosition.Bottom:
                    return GetUserAtSeat(MySeat);
                case SeatPosition.Left:
                {
                    var seat = (MySeat + 3) % 4;
                    return GetUserAtSeat(seat);
                }

                case SeatPosition.Right:
                {
                    var seat = (MySeat + 1) % 4;
                    return GetUserAtSeat(seat);
                }

                case SeatPosition.Top:
                {
                    var seat = (MySeat + 2) % 4;
                    return GetUserAtSeat(seat);
                }

                default:
                    return null;
            }
        }

        public User GetUserAtSeat(int seat)
        {
            switch (seat)
            {
                case 0:
                    return User0;

                case 1:
                    return User1;

                case 2:
                    return User2;

                case 3:
                    return User3;

                default:
                    return null;
            }
        }

        public User GetUserFormUserName(string username)
        {
            if (User0 != null &&
                StringUtil.AreEqual(User0.username, username))
                return User0;

            if (User1 != null &&
                StringUtil.AreEqual(User1.username, username))
                return User1;

            if (User2 != null &&
                StringUtil.AreEqual(User2.username, username))
                return User2;

            if (User3 != null &&
                StringUtil.AreEqual(User3.username, username))
                return User3;

            return null;
        }

        public int GetSeatOfUser(User user)
        {
            if (user == null)
            {
                return -1;
            }

            if (User0 != null && User0.username == user.username)
            {
                return 0;
            }

            if (User1 != null && User1.username == user.username)
            {
                return 1;
            }

            if (User2 != null && User2.username == user.username)
            {
                return 2;
            }

            if (User3 != null && User3.username == user.username)
            {
                return 3;
            }

            return -1;
        }

        public int GetSeatOfUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return -1;
            }

            if (User0 != null && User0.username == username)
            {
                return 0;
            }

            if (User1 != null && User1.username == username)
            {
                return 1;
            }

            if (User2 != null && User2.username == username)
            {
                return 2;
            }

            if (User3 != null && User3.username == username)
            {
                return 3;
            }

            return -1;
        }

        public User UserTop()
        {
            return GetUserAtPos(SeatPosition.Top);
        }

        public User UserBottom()
        {
            return GetUserAtPos(SeatPosition.Bottom);
        }

        public User UserRight()
        {
            return GetUserAtPos(SeatPosition.Right);
        }

        public User UserLeft()
        {
            return GetUserAtPos(SeatPosition.Left);
        }

        public bool IsSameTeam(string username1, string username2)
        {
            if (string.IsNullOrEmpty(username1) ||
                string.IsNullOrEmpty(username2))
                return false;

            var seat1 = GetSeatOfUser(username1);
            var seat2 = GetSeatOfUser(username2);

            return (seat1 + seat2) % 2 == 0;
        }

        /// <summary>
        /// 获取桌子上除了我之外的其他用户名。
        /// </summary>
        /// <returns></returns>
        public List<string> GetTableOtherUsernameList()
        {
            var list = new List<string>();

            if (MySeat != 0 && User0 != null)
            {
                list.Add(User0.username);
            }

            if (MySeat != 1 && User1 != null)
            {
                {
                    list.Add(User1.username);
                }
            }

            if (MySeat != 2 && User2 != null)
            {
                list.Add(User2.username);
            }

            if (MySeat != 3 && User3 != null)
            {
                list.Add(User3.username);
            }

            return list;
        }

        public bool InTable(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            var seat = GetSeatOfUser(username);
            return seat != -1;
        }
    }
}