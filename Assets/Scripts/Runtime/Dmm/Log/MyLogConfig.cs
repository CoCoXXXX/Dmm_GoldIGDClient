using UnityEngine;

namespace Dmm.Log
{
    public class MyLogConfig : ScriptableObject
    {
        public bool DisableAll;

        public bool EnableInfo;

        public bool EnableDebug;

        public bool EnableWarning;

        public bool EnableError;

        public bool EnableException;
    }
}