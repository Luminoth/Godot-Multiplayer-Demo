public partial class EngineManager : SingletonNode<EngineManager>
{
    public void Quit()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);

        GetTree().Quit();
    }
}
