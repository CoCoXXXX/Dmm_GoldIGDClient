namespace Dmm.MoreFunction
{
    public class RealNameResult
    {
        public const int Ok = 0;

        public const int Error = -100;

        public int result;

        public string error;

        public RealNameResult()
        {
        }

        public RealNameResult(int result, string errMsg)
        {
            this.result = result;
            this.error = errMsg;
        }
    }
}