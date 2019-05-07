using System;

namespace Dmm.Res
{
    public interface IResourceLoader
    {
        T Load<T>(string resourcePath, string resourceName, Action<T> completeListener);
    }
}