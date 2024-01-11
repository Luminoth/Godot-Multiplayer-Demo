using Godot;

public partial class Player : Node
{
    [Export]
    private PlayerInput _input;

    public PlayerInput Input => _input;

    [Export]
    private long _clientId;

    public long ClientId => _clientId;

    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print($"Player {_clientId} ready!");
    }

    #endregion

    public void SetClientId(long id)
    {
        GD.Print($"Setting player authority: {id}");

        _clientId = id;
        _input.SetMultiplayerAuthority((int)_clientId);
    }
}
