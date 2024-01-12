using Godot;

public partial class EngineManager : SingletonNode<EngineManager>
{
    public string LogPath => ProjectSettings.GlobalizePath(ProjectSettings.GetSetting("debug/file_logging/log_path").AsString());

    public override void _Ready()
    {
        GD.Print("Engine ready!");
        GD.Print($"Log path: {LogPath}");
    }

    public void Quit()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);

        GetTree().Quit();
    }
}
