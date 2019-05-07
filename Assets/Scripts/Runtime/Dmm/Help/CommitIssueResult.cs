namespace Dmm.Help
{
    public class CommitIssueResult
    {
        public const int Ok = 0;

        public const int Error = -100;

        public int result;

        public string error;

        public CommitIssueResult()
        {
        }

        public CommitIssueResult(int result, string errMsg)
        {
            this.result = result;
            this.error = errMsg;
        }
    }
}