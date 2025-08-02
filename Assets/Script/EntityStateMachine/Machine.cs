using UnityEngine;

namespace Script.StateMachine
{
    /// <summary>
    /// 状态机
    /// </summary>
    public class Machine
    {
        public EntityState currentState { get; private set; }

        public void Initialize(EntityState _startState)
        {
            currentState = _startState;
            currentState.OnEnter();
        }

        public void ChangeState(EntityState _newState)
        {
            currentState.OnExit();
            currentState = _newState;
            currentState.OnEnter();
        }
    }
}