using Godot;

public partial class Player : Node
{
    [Export]
    NodePath _movementPath;

    PlayerMovement _movement;

    [Export]
    private long _clientId;

    public PlayerMovement Movement => _movement;

    #region Godot Lifecycle

    public override void _EnterTree()
    {
        _movement = GetNode<PlayerMovement>(_movementPath);
    }

    public override void _Ready()
    {
        GD.Print($"Player {ClientManager.Instance.UniqueId} ready!");
    }

    #endregion

    public void SetClientId(long id)
    {
        GD.Print($"Setting player authority: {id}");
        _clientId = id;

        _movement.SetClientId(_clientId);
    }
}
