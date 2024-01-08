using Godot;

using System;

public partial class GameManager : SingletonNode<GameManager>
{
    #region Events

    public event EventHandler LoadLevelEvent;

    #endregion

    [Export]
    private int _listeningPort = 7777;

    public int ListenPort => _listeningPort;

    public string DefaultAddress => $"127.0.0.1:{ListenPort}";

    [Export]
    private int _maxPlayers = 4;

    public int MaxPlayers => _maxPlayers;

    #region Client RPC

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LevelLoaded(long id)
    {
        GD.Print($"client {id} says level loaded");
    }

    #endregion

    #region Server RPC

    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LoadLevel()
    {
        GD.Print("server says load level");
        LoadLevelEvent?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
