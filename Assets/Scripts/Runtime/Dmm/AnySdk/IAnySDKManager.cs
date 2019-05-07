using System.Collections.Generic;

namespace Dmm.AnySdk
{
    public interface IAnySDKManager
    {
        void Init();

        void Login();

        void Login(Dictionary<string, string> data);
    }
}