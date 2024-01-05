using System.Linq;

using Godot;

public partial class Boot : Node
{
    [Export]
    private PackedScene _mainMenuScene;

    [Export]
    private PackedScene _dedicatedScene;

    public override void _Process(double delta)
    {
        var args = OS.GetCmdlineArgs();
        if(args.Contains("--dedicated")) {
            GD.Print("Starting dedicated server ...");

            if(!OS.HasFeature("dedicated_server")) {
                GD.PushWarning("Dedicated servers should be run with the dedicated server feature");
            }

            var scene = _dedicatedScene.Instantiate();
            GetTree().Root.AddChild(scene);
        } else {
            GD.Print("Starting client ...");

            var scene = _mainMenuScene.Instantiate();
            GetTree().Root.AddChild(scene);
        }

        QueueFree();
    }
}
