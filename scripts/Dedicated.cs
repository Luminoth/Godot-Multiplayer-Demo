using System.Linq;

using Godot;

public partial class Dedicated : Node
{
    public override void _Ready()
    {
        if(!OS.HasFeature("dedicated_server")) {
            GD.PushWarning("Dedicated servers should be run with the dedicated server feature");
        }

        bool useGameLift = OS.GetCmdlineArgs().Contains("--gamelift");
        ServerManager.Instance.StartDedicatedServer(useGameLift);
    }
}
