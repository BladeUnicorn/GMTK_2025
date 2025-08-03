using System;
using Script.Base;

namespace Script.Manager
{
    public enum GameState
    {
        GenerateGrid = 0,
        SpawnPlayer = 1,
    }
    
    public class GameManager : SingletonBase<GameManager>
    {
        public GameState currentGameState { get; private set; }
        // //新输入系统
        // public InputSystem_Actions inputSystem { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            // inputSystem = new InputSystem_Actions();
        }

        private void Start()
        {
            ChangeState(GameState.GenerateGrid);
        }

        public void ChangeState(GameState _newState)
        {
            currentGameState = _newState;
            switch (_newState)
            {
                case GameState.GenerateGrid:
                    GridManager.instance.GenerateGrid();
                    break;
                case GameState.SpawnPlayer:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_newState), _newState,
                        "Argument out of range exception");
            }
        }

        // private void OnEnable()
        // {
        //     inputSystem.Enable();
        // }
        //
        // private void OnDisable()
        // {
        //     inputSystem.Disable();
        // }
    }
}