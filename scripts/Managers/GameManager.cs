using Godot;

using System;

public partial class GameManager : SingletonNode<GameManager>
{
    [Export]
    private int _listeningPort = 7777;

    public int ListenPort => _listeningPort;

    public string DefaultAddress => $"127.0.0.1:{ListenPort}";

    [Export]
    private int _maxPlayers = 4;

    public int MaxPlayers => _maxPlayers;

    [Export]
    private PackedScene _levelScene;

    public bool IsActualMultiplayerAuthority => ServerManager.Instance.IsActualServer;

    #region Godot Lifecycle

    public override void _ExitTree()
    {
        ServerManager.Instance.PeerConnectedEvent -= OnPeerConnected;

        base._ExitTree();
    }

    #endregion

    public bool StartLocalServer()
    {
        if(!ServerManager.Instance.StartLocalServer(ListenPort, MaxPlayers)) {
            GD.PrintErr("Failed to start local game server");
            return false;
        }

        FinishStartServer();

        return true;
    }

    public bool StartDedicatedServer(bool useGameLift)
    {
        if(!ServerManager.Instance.StartDedicatedServer(ListenPort, MaxPlayers, useGameLift)) {
            GD.PrintErr("Failed to start dedicated game server");
            return false;
        }

        FinishStartServer();

        return true;
    }

    private void FinishStartServer()
    {
        ServerManager.Instance.PeerConnectedEvent += OnPeerConnected;

        GD.Print("Server loading level ...");

        var scene = _levelScene.Instantiate();
        ServerManager.Instance.AddChild(scene);
    }

    #region Event Handlers

    private void OnPeerConnected(object sender, ServerManager.PeerEventArgs e)
    {
        GD.Print($"Player {e.Id} connected, signaling to load level");

        ServerManager.Instance.RpcId(e.Id, nameof(ServerManager.Instance.LoadLevel));
    }

    #endregion
}
