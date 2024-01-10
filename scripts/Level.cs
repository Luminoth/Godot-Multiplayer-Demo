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

        GD.Print($"Server Is Server (this is a lie): {ServerManager.Instance.IsServer}");
        GD.Print($"Server Is Actual Server: {ServerManager.Instance.IsActualServer}");
        GD.Print($"Is Dedicated Server: {ServerManager.Instance.IsDedicatedServer}");
        GD.Print($"Is Multiplayer Authority (this is a lie): {IsMultiplayerAuthority()}");
        GD.Print($"Is Actual Multiplayer Authority: {GameManager.Instance.IsActualMultiplayerAuthority}");

        if(ServerManager.Instance.IsActualServer) {
            ServerManager.Instance.PeerConnectedEvent += OnPeerConnected;
            ServerManager.Instance.PeerDisconnectedEvent += OnPeerDisconnected;
        } else {
            ClientManager.Instance.Rpc(nameof(ClientManager.Instance.LevelLoaded), ClientManager.Instance.UniqueId);
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
        player.Name = $"Player {e.Id}";
        GetNode(_spawnRoot).AddChild(player);
        _players.Add(e.Id, player);
    }

    private void OnPeerDisconnected(object sender, ServerManager.PeerEventArgs e)
    {
        GD.Print($"Player {e.Id} disconnected, despawning ...");

        _players.Remove(e.Id);
        // TODO: remove the player's node
    }

    #endregion
}
