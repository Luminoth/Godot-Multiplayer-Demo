using System.Linq;

using Godot;

public partial class Dedicated : Node
{
    [Export]
    private PackedScene _levelScene;

    public override void _Ready()
    {
        if(!OS.HasFeature("dedicated_server")) {
            GD.PushWarning("Dedicated servers should be run with the dedicated server feature");
        }

        bool useGameLift = OS.GetCmdlineArgs().Contains("--gamelift");
        if(!ServerManager.Instance.StartDedicatedServer(GameManager.Instance.ListenPort, GameManager.Instance.MaxPlayers, useGameLift)) {
            GD.PrintErr("Failed to start dedicated game server");
            return;
        }
    }

    public override void _Process(double delta)
    {
        // TODO: may actually want this to stick around
        // and listen for input to navigate in non-headless mode

        if(ServerManager.Instance.IsServer) {
            var scene = _levelScene.Instantiate();
            GetTree().Root.AddChild(scene);

            QueueFree();
        }
    }
}
