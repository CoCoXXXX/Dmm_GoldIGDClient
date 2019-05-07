using System.Collections.Generic;
using UnityEngine;

namespace Dmm.Pay
{
    [CreateAssetMenu(menuName = "Dmm/IapConfig")]
    public class IapConfig : ScriptableObject
    {
        public List<string> ProductIdList;
    }
}