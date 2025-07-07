using UnityEngine;

public class EnemyStateMachine<T>
{
    public EnemyState<T> currentState { get; private set; }
    T owner;

    public EnemyStateMachine(T _owner)
    {
        owner = _owner;
    }

    public void ChangeState(EnemyState<T> state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter(owner);
    }

    public void Execute()
    {
        currentState.Execute();
    }
}
