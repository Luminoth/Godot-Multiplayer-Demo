using Godot;

using System.Collections.Generic;

using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;

public partial class ServerManager : SingletonNode<ServerManager>
{
    [Export]
    private int _listeningPort = 7777;

    public int ListenPort => _listeningPort;

    private bool _isGameLiftServer;

    #region Godot Lifecycle

    public override void _ExitTree()
    {
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
            GD.PushError("InitSDK failure : " + initSDKOutcome.Error.ToString());
            return false;
        }
    }

    public bool StartLocalServer()
    {
        GD.Print("Starting local server ...");

        return false;
    }

    public void StopServer()
    {
        if(_isGameLiftServer) {
            GameLiftServerAPI.Destroy();
            _isGameLiftServer = false;
        }
    }
}
