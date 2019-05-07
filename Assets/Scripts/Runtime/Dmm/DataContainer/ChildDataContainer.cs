using System;

namespace Dmm.DataContainer
{
    public class ChildDataContainer<T> : IDataContainer<T>
    {
        private readonly IRelation<T> _relation;

        public ChildDataContainer(IRelation<T> relation)
        {
            if (relation == null)
            {
                throw new ArgumentException("Argument relation should not be null!");
            }

            _relation = relation;
        }

        public void Write(T data, float timestamp)
        {
            if (timestamp < Timestamp)
            {
                return;
            }

            _relation.Data = data;
            _relation.Invalidate(timestamp);
        }

        public T Read(bool isDelete = false)
        {
            var data = _relation.Data;

            if (isDelete)
            {
                _relation.Data = default(T);
                _relation.Invalidate(0);
            }

            return data;
        }

        public float Timestamp
        {
            get { return _relation.Timestamp; }
        }

        public void Invalidate(float time)
        {
            _relation.Invalidate(time);
        }

        public void ClearAndInvalidate(float time)
        {
            _relation.Data = default(T);
            _relation.Invalidate(time);
        }

        public void ClearNotInvalidate()
        {
            _relation.Data = default(T);
        }
    }
}