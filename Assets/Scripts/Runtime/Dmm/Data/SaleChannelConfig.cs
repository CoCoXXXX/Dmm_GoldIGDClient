using System.Collections.Generic;
using UnityEngine;

namespace Dmm.Data
{
    public class SaleChannelConfig : ScriptableObject
    {
        /// <summary>
        /// 渠道列表。
        /// </summary>
        public List<string> SaleChannelList;

        public string[] Values()
        {
            if (SaleChannelList == null || SaleChannelList.Count == 0)
                return null;

            return SaleChannelList.ToArray();
        }

        public int IndexOf(string saleChannel)
        {
            if (SaleChannelList == null || SaleChannelList.Count <= 0)
            {
                return -1;
            }

            return SaleChannelList.FindIndex(value => value == saleChannel);
        }

        public string SaleChannelOf(int index)
        {
            if (SaleChannelList == null) return null;

            if (index < 0 || index >= SaleChannelList.Count)
                return null;

            return SaleChannelList[index];
        }
    }
}