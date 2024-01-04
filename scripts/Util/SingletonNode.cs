using Godot;

public partial class SingletonNode<T> : Node where T : SingletonNode<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(ExitedTree) {
                GD.PushError($"[Singleton] Instance '{typeof(T)}' already exited SceneTree!");
                return null;
            }
            return instance;
        }
    }

    public static bool HasInstance => null != instance && !ExitedTree;

    private static bool ExitedTree;

    #region Godot Lifecycle

    public override void _EnterTree()
    {
        if(HasInstance) {
            GD.PushError($"[Singleton] Instance '{typeof(T)}' already exists!");
            return;
        }

        instance = (T)this;
    }

    public override void _ExitTree()
    {
        ExitedTree = true;
    }

    #endregion
}
