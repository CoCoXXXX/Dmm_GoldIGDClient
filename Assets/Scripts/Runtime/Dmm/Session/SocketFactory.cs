using Dmm.Msg;
using Zenject;

namespace Dmm.Session
{
    public class SocketFactory : ISocketFactory
    {
        #region Inject

        private IMsgRepo _msgRepo;

        [Inject]
        public void Initialize(IMsgRepo msgRepo)
        {
            _msgRepo = msgRepo;
        }

        #endregion

        public ISocketClient CreateSocket()
        {
            return new SocketClient(_msgRepo.GetMessageQueue());
        }
    }
}