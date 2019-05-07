namespace Dmm.DataContainer
{
    /// <summary>
    /// This interface is used to bind with Zenject
    /// </summary>
    public interface IDataContainerFactory
    {
        /// <summary>
        /// Create an instance of a child DataContainer with relation
        /// Child DataContainer only holds reference to the root DataContainer by relation
        /// </summary>
        /// <returns>The new instance</returns>
        IDataContainer<T> CreateChildDataContainer<T>(IRelation<T> relation);

        /// <summary>
        /// Create an instance of a root DataContainer
        /// Root DataContainer must be created before any of its children.
        /// Only root DataContainer holds real data.
        /// </summary>
        /// <returns>The new instance</returns>
        IDataContainer<T> CreateRootDataContainer<T>();
    }
}