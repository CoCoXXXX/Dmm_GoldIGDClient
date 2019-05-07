using System.Collections.Generic;

namespace Dmm.DataContainer
{
    public class DataRepository : IDataRepository
    {
        public DataRepository()
        {
            var assembler = new DataAssembler();
            assembler.AssembleData(this);
        }

        private readonly Dictionary<string, IContainer> _containers = new Dictionary<string, IContainer>();

        public IDataContainer<T> GetContainer<T>(string key)
        {
            if (!_containers.ContainsKey(key))
            {
                return null;
            }

            var c = _containers[key];
            if (c.GetType().GetGenericArguments()[0] == typeof(T))
            {
                return (IDataContainer<T>) c;
            }

            return null;
        }

        public bool AddContainer(string key, IContainer container)
        {
            if (_containers.ContainsKey(key))
            {
                return false;
            }

            _containers.Add(key, container);
            return true;
        }
    }
}