namespace Dmm.Task
{
    public interface ITaskChecker
    {
        /// <summary>
        /// 检查任务是否已经完成。
        /// </summary>
        /// <returns>
        /// 任务尚未完成则return null。
        /// 任务已经完成则return TaskResult实例。
        /// </returns>
        TaskResult Check();
    }
}