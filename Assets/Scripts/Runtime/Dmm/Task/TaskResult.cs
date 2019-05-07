namespace Dmm.Task
{
    public class TaskResult
    {
        public static TaskResult Success()
        {
            return new TaskResult(true);
        }

        public static TaskResult Fail(int errCode, string errMsg)
        {
            return new TaskResult(false, errCode, errMsg);
        }

        public TaskResult(bool isSuccess, int errCode, string errMsg)
        {
            IsSuccess = isSuccess;
            ErrCode = errCode;
            ErrMsg = errMsg;
        }

        public TaskResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
            ErrCode = -1;
            ErrMsg = null;
        }

        public readonly bool IsSuccess;

        public readonly int ErrCode;

        public readonly string ErrMsg;
    }
}