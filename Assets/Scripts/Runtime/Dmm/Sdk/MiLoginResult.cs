namespace Dmm.Sdk
{
    public class MiLoginResult
    {
        public const int SUCCESS = 0;

        public const int FAIL = -102;

        public const int CANCEL = -12;

        public const int EXECUTING = -18006;

        public const int COMMON = FAIL;

        public int result;

        public string uid;

        public string sessionid;

        public string nickname;

        public MiLoginResult()
        {
        }

        public MiLoginResult(int result, string uid, string sessionid, string nickname)
        {
            this.result = result;
            this.uid = uid;
            this.sessionid = sessionid;
            this.nickname = nickname;
        }
    }
}