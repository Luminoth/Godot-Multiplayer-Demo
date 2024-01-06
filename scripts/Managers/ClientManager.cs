using Godot;

public partial class ClientManager : SingletonNode<ClientManager>
{
    #region Godot Lifecycle

    public override void _ExitTree()
    {
        Disconnect();
    }

    #endregion

    public bool JoinLocalGameSession()
    {
        return JoinGameSession(EngineManager.Instance.DefaultAddress);
    }

    public bool JoinGameSession(string address)
    {
        var parts = address.Split(':');
        return JoinGameSession(parts[0], int.Parse(parts[1]));
    }

    public bool JoinGameSession(string address, int port)
    {
        GD.Print($"Joining game session at {address}:{port} ...");

        var peer = new ENetMultiplayerPeer();

        var result = peer.CreateClient(address, port);
        if(result != Error.Ok) {
            GD.PrintErr($"Failed to create client: {result}");
            return false;
        }

        Multiplayer.MultiplayerPeer = peer;

        return true;
    }

    public void Disconnect()
    {
        Multiplayer.MultiplayerPeer = null;
    }
}
