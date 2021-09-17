using Mirror;

public abstract class State : NetworkBehaviour
{
    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}
