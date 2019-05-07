using System.Collections.Generic;

namespace Dmm.Base
{
    public class ListMap<KEY, VALUE>
    {
        private readonly List<VALUE> _list = new List<VALUE>();

        private readonly Dictionary<KEY, VALUE> _map = new Dictionary<KEY, VALUE>();

        private static readonly object Loker = new object();

        public int Count
        {
            get
            {
                lock (Loker)
                {
                    return _list.Count;
                }
            }
        }

        public void Add(KEY key, VALUE value)
        {
            if (!_map.ContainsKey(key))
                _map[key] = value;

            if (!_list.Contains(value))
                _list.Add(value);
        }

        public VALUE GetByKey(KEY key)
        {
            if (_map.ContainsKey(key))
                return _map[key];

            return default(VALUE);
        }

        public VALUE GetByIndex(int index)
        {
            if (index < 0 || index >= _list.Count)
                return default(VALUE);

            return _list[index];
        }
    }
}