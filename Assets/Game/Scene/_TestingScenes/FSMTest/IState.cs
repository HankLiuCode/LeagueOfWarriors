public interface IState
{
    void Execute(float dt);
    void HandleInput();
    void Enter(params object[] args);
    void Exit();
}
