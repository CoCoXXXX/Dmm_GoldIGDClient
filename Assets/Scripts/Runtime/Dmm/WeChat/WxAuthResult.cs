namespace Dmm.WeChat
{
    [System.Serializable]
    public class WxAuthResult
    {
        public const int Ok = 0;

        public const int Error = -1;

        public const int Cancel = -2;

        public int Result;

        public string ErrMsg;

        public string Code;

        public string State;

        public string Lang;

        public string Country;
    }
}