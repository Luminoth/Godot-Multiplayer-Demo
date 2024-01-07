using Godot;

using System;
using System.Collections.Generic;

using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;

public partial class ServerManager : SingletonNode<ServerManager>
{
    #region Events

    public event EventHandler PeerConnected;
    public event EventHandler PeerDisconnected;

    #endregion

    [Export]
    private int _listeningPort = 7777;

    public int ListenPort => _listeningPort;

    [Export]
    private int _maxPlayers = 4;

    private bool _isGameLiftServer;

    public bool IsServer => Multiplayer.IsServer();

    public bool IsDedicatedServer => ClientManager.Instance.UniqueId == 1;

    #region Godot Lifecycle

    public override void _Ready()
    {
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
    }

    public override void _ExitTree()
    {
        Multiplayer.PeerConnected -= OnPeerConnected;
        Multiplayer.PeerDisconnected -= OnPeerDisconnected;

        StopServer();
    }

    #endregion

    public bool StartDedicatedServer(bool useGameLift)
    {
        GD.Print("Starting dedicated server ...");

        if(useGameLift) {
            return StartGameLiftServer();
        } else {
            return StartLocalServer();
        }
    }

    private bool StartGameLiftServer()
    {
        GD.Print("Starting GameLift server ...");

        var initSDKOutcome = GameLiftServerAPI.InitSDK();
        if(initSDKOutcome.Success) {
            _isGameLiftServer = true;

            var processParameters = new ProcessParameters(
                // OnStartGameSession
                (GameSession gameSession) => {
                    GD.Print("OnStartGameSession");

                    // TODO: what if this fails?
                    StartLocalServer();

                    GameLiftServerAPI.ActivateGameSession();
                },
                // OnUpdateGameSession
                (UpdateGameSession updateGameSession) => {
                    GD.Print("OnUpdateGameSession");
                },
                // OnProcessTerminate
                () => {
                    GD.Print("OnProcessTerminate");

                    GameLiftServerAPI.ProcessEnding();

                    EngineManager.Instance.Quit();
                },
                // OnHealthCheck
                () => {
                    GD.Print("OnHealthCheck");
                    return true;
                },
                _listeningPort,
                new LogParameters(new List<string>()
                {
                    // TODO:
                    "/local/game/logs/myserver.log"
                }));

            var processReadyOutcome = GameLiftServerAPI.ProcessReady(processParameters);
            if(processReadyOutcome.Success) {
                GD.Print("ProcessReady success.");
                return true;
            } else {
                GD.Print("ProcessReady failure : " + processReadyOutcome.Error.ToString());
                return false;
            }
        } else {
            GD.PrintErr("InitSDK failure : " + initSDKOutcome.Error.ToString());
            return false;
        }
    }

    public bool StartLocalServer()
    {
        GD.Print($"Starting local server on {ListenPort} ...");

        // give the server it's own API so we can run a parallel client
        GetTree().SetMultiplayer(new SceneMultiplayer(), GetPath());
        var peer = new ENetMultiplayerPeer();

        var result = peer.CreateServer(ListenPort, _maxPlayers);
        if(result != Error.Ok) {
            GD.PrintErr($"Failed to create server: {result}");
            return false;
        }

        Multiplayer.MultiplayerPeer = peer;

        return true;
    }

    public void StopServer()
    {
        GD.Print("Stopping game server ...");

        Multiplayer.MultiplayerPeer = null;

        if(_isGameLiftServer) {
            GameLiftServerAPI.Destroy();
            _isGameLiftServer = false;
        }
    }

    #region Event Handlers

    private void OnPeerConnected(long id)
    {
        // TODO: send along the id
        PeerConnected?.Invoke(this, EventArgs.Empty);
    }

    private void OnPeerDisconnected(long id)
    {
        // TODO: send along the id
        PeerDisconnected?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
