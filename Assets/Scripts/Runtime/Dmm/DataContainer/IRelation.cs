namespace Dmm.DataContainer
{
    public interface IRelation<T>
    {
        T Data { get; set; }

        float Timestamp { get; }

        void Invalidate(float time);
    }
}