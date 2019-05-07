namespace Dmm.DataContainer
{
    public class RootDataContainer<T> : IDataContainer<T>
    {
        private T _data;

        private readonly IRelation<T> _relation;

        public void Write(T data, float timestamp)
        {
            if (timestamp < Timestamp)
            {
                return;
            }
            _data = data;
            Timestamp = timestamp;
        }

        public T Read(bool isDelete)
        {
            var data = _data;
            if (isDelete)
            {
                _data = default(T);
                Timestamp = 0;
            }
            return data;
        }

        public float Timestamp { get; private set; }

        public void Invalidate(float time)
        {
            Timestamp = time;
        }

        public void ClearAndInvalidate(float time)
        {
            _data = default(T);
            Timestamp = time;
        }

        public void ClearNotInvalidate()
        {
            _data = default(T);
        }
    }
}