using Godot;

public partial class PlayerInput : MultiplayerSynchronizer
{
    [Export]
    private Vector2 _direction;

    public Vector2 Direction => _direction;

    #region Godot Lifecycle

    public override void _EnterTree()
    {
        bool isAuthority = GetMultiplayerAuthority() == ClientManager.Instance.UniqueId;
        GD.Print($"Player {GetParent<Player>().ClientId} input authority: {isAuthority}");
        SetProcess(isAuthority);
    }

    public override void _Process(double delta)
    {
        _direction = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
    }

    #endregion
}
