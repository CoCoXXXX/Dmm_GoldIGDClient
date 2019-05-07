using System;
using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public abstract class ChildRelationAdapter<T> : IRelation<T>
    {
        public abstract T Data { get; set; }

        private float _timestamp;

        public float Timestamp
        {
            get { return Math.Max(_timestamp, ParentTimestamp); }
            private set { _timestamp = value; }
        }

        public void Invalidate(float time)
        {
            Timestamp = time;
        }

        protected abstract float ParentTimestamp { get; }
    }
}