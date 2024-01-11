using Godot;

public partial class Player : Node
{
    [Export]
    private PlayerInput _input;

    public PlayerInput Input => _input;

    private long _clientId;

    [Export]
    public long ClientId
    {
        get => _clientId; set
        {
            GD.Print($"Setting player authority: {value}");

            _clientId = value;
            _input.SetMultiplayerAuthority((int)_clientId);
        }
    }

    #region Godot Lifecycle

    public override void _Ready()
    {
        GD.Print($"Player {_clientId} ready!");
    }

    #endregion
}
