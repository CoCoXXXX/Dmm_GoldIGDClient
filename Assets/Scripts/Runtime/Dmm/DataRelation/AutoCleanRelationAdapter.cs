using Dmm.DataContainer;

namespace Dmm.DataRelation
{
    public abstract class AutoCleanRelationAdapter<T> : IRelation<T>
    {
        public float Timestamp { get; private set; }

        public void Invalidate(float time)
        {
            Timestamp = time;
        }

        public T Data
        {
            get
            {
                if (Timestamp < ParentTimestamp)
                {
                    DataContent = default(T);
                    return default(T);
                }

                return DataContent;
            }
            set { DataContent = value; }
        }

        protected abstract T DataContent { get; set; }
        protected abstract float ParentTimestamp { get; }
    }
}