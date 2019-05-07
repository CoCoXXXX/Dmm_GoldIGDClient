namespace Dmm.Msg
{
    public interface IMsgRepo
    {
        void SendMsg(ProtoMessage msg);
        void ReceiveMsg(ProtoMessage msg);

        MessageQueue<ProtoMessage> GetMessageQueue();

        int WriteMessageCount();
        ProtoMessage DequeueWriteMessage();
        void ClearWriteMessage();

        int ReadMessageCount();
        ProtoMessage DequeueReadMessage();
        void ClearReadMessage();
    }
}