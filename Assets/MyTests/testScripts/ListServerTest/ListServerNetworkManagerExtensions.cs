using Mirror;
using System.Diagnostics;

public class ListServerNetworkManagerExtensions : NetworkManager
{
    public override void OnClientConnect(NetworkConnection conn)
    {
        ListServerFrontEnd.instance.OnClientConnect(conn);
        base.OnClientConnect(conn);
    }
}
