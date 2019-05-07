namespace Dmm.Network
{
    public enum NetworkStatus
    {
        Null,

        PIP,

        BuildFirstCache,

        DownloadResources,

        ConnectGateServer,

        ClientVersion,

        ChooseLoginType,

        LoginGateServer,

        ConnectHallServer,

        LoginHallServer,

        LoginHallServerOk,

        ConnectGameServer,

        LoginGameServer,
        
        LoginGameServerOk,

        LoginFail
    }
}