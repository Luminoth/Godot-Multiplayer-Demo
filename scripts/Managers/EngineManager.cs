public partial class EngineManager : SingletonNode<EngineManager>
{
    public string DefaultAddress => $"127.0.0.1:{ServerManager.Instance.ListenPort}";

    public void Quit()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);

        GetTree().Quit();
    }
}
