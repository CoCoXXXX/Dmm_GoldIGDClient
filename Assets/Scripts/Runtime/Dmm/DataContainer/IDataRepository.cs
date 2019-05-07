namespace Dmm.DataContainer
{
    public interface IDataRepository
    {
        /// <summary>
        /// Try to Return a DataContainer of a certain generic type by key;
        /// </summary>
        /// <typeparam name="T">The generic argument T of the DataContainer</typeparam>
        /// <param name="key">The key of the Container.</param>
        /// <returns>The Container, null if not found.</returns>
        IDataContainer<T> GetContainer<T>(string key);

        /// <summary>
        /// Add a Container with specific key.
        /// Return true if succeeded, false if already exists.
        /// </summary>
        bool AddContainer(string key, IContainer container);
    }
}