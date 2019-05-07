namespace Dmm.Report
{
    public class ReportResult
    {
        public const int Ok = 0;

        public const int Error = -100;

        public int result;

        public string error;

        public ReportResult()
        {
        }

        public ReportResult(int result, string errMsg)
        {
            this.result = result;
            this.error = errMsg;
        }
    }
}