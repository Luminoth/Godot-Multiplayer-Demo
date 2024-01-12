using Godot;

public partial class Dedicated : Node
{
    public override void _Ready()
    {
        if(!OS.HasFeature("dedicated_server")) {
            GD.PushWarning("Dedicated servers should be run with the dedicated server feature");
        }

        bool useGameLift = EngineManager.Instance.CommandLineArgs.ContainsKey("--gamelift");
        if(!GameManager.Instance.StartDedicatedServer(useGameLift)) {
            GD.PrintErr("Failed to start dedicated game server");
            return;
        }

        QueueFree();
    }
}
