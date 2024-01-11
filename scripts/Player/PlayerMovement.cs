using Godot;

public partial class PlayerMovement : CharacterBody3D
{
    [Export]
    private float _speed = 5.0f;

    private Vector2 _direction;

    #region Godot Lifecycle

    public override void _Ready()
    {
        SetProcess(GetMultiplayerAuthority() == ClientManager.Instance.UniqueId);
    }

    public override void _Process(double delta)
    {
        _direction = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector3(_direction.X * _speed, Velocity.Y, _direction.Y * 0.0f);
        MoveAndSlide();
    }

    #endregion

    public void SetClientId(long id)
    {
        SetMultiplayerAuthority((int)id);
        SetProcess(GetMultiplayerAuthority() == ClientManager.Instance.UniqueId);
    }
}
