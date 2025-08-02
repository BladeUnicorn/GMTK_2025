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
        private GameState currentGameState;

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
        
    }
}