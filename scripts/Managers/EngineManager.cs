using System.Collections.Generic;

using Godot;

public partial class EngineManager : SingletonNode<EngineManager>
{
    public string LogPath => ProjectSettings.GlobalizePath(ProjectSettings.GetSetting("debug/file_logging/log_path").AsString());

    private readonly Dictionary<string, string> _commandLineArgs = new Dictionary<string, string>();

    public Dictionary<string, string> CommandLineArgs => _commandLineArgs;

    public override void _Ready()
    {
        GD.Print("Engine ready!");
        GD.Print($"Log path: {LogPath}");

        var commandLineArgs = OS.GetCmdlineArgs();
        foreach(var arg in commandLineArgs) {
            var parts = arg.Split('=');
            if(parts.Length == 1) {
                _commandLineArgs[parts[0].TrimStart('-')] = string.Empty;
            } else if(parts.Length == 2) {
                _commandLineArgs[parts[0].TrimStart('-')] = parts[1];
            } else {
                GD.PrintErr($"Invalid command line argument: {arg}");
            }
        }
    }

    public void Quit()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);

        GetTree().Quit();
    }
}
