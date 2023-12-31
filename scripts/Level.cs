using Godot;

using System.Collections.Generic;

public partial class Level : Node
{
    [Export]
    private PackedScene _playerScene;

    [Export]
    private NodePath _spawnRoot;

    private Dictionary<long, Player> _players = new Dictionary<long, Player>();

    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print("Level ready ...");
        GD.Print($"Peer ID: {ClientManager.Instance.UniqueId}");
        GD.Print($"Is Server: {ServerManager.Instance.IsServer}");
        GD.Print($"Is Dedicated Server: {ServerManager.Instance.IsDedicatedServer}");
        GD.Print($"Is Multiplayer Authority: {IsMultiplayerAuthority()}");

        if(ServerManager.Instance.IsServer) {
            ServerManager.Instance.PeerConnectedEvent += OnPeerConnected;
            ServerManager.Instance.PeerDisconnectedEvent += OnPeerDisconnected;
        } else {
            GD.Print("signaling level loaded");
            Rpc(nameof(ClientManager.Instance.LevelLoaded), ClientManager.Instance.UniqueId);
        }
    }

    public override void _ExitTree()
    {
        ServerManager.Instance.PeerConnectedEvent -= OnPeerConnected;
        ServerManager.Instance.PeerDisconnectedEvent -= OnPeerDisconnected;
    }

    #endregion

    #region Event Handlers

    private void OnPeerConnected(object sender, ServerManager.PeerEventArgs e)
    {
        GD.Print($"Player {e.Id} connected, spawning ...");

        var player = _playerScene.Instantiate<Player>();
        GetNode(_spawnRoot).AddChild(player);
        _players.Add(e.Id, player);
    }

    private void OnPeerDisconnected(object sender, ServerManager.PeerEventArgs e)
    {
        GD.Print($"Player {e.Id} disconnected, despawning ...");

        _players.Remove(e.Id);
    }

    #endregion
}
