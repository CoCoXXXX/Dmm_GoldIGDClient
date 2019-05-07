namespace Dmm.DataContainer
{
    public class DataContainerFactory : IDataContainerFactory
    {
        public IDataContainer<T> CreateChildDataContainer<T>(IRelation<T> relation)
        {
            return new ChildDataContainer<T>(relation);
        }

        public IDataContainer<T> CreateRootDataContainer<T>()
        {
            return new RootDataContainer<T>();
        }
    }
}