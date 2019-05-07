namespace Dmm.DataContainer
{
    public interface IDataContainer<T> : IContainer
    {
        /// <summary>
        /// Write the data and refresh the time stamp,
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="timestamp">The time of writing this.</param>
        void Write(T data, float timestamp);

        /// <summary>
        /// Read the data
        /// </summary>
        /// <returns>The data</returns>
        T Read(bool isDelete = false);

        /// <summary>
        /// Return a time as float represented by the lastest modification time of itself and all of its children containers
        /// </summary>
        float Timestamp { get; }

        /// <summary>
        /// Invalidate the old data,updtate new data
        /// </summary>
        void Invalidate(float timestamp);

        /// <summary>
        /// Clear data and updtae
        /// </summary>
        void ClearAndInvalidate(float timestamp);

        /// <summary>
        /// Clear data not updtae
        /// </summary>
        void ClearNotInvalidate();
    }
}