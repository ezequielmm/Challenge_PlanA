using System;
using UnityEngine;

namespace ChallengePlanA.Core
{
    /// <summary>
    /// Handles game state. Singleton because we only need one and
    /// other scripts need easy access to it.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameConfig config;

        private int _score;
        private int _movesRemaining;
        private bool _isGameOver;

        // Using C# events instead of UnityEvents, they are faster
        // and I prefer to keep the wiring in code for this kind of project
        public event Action<int> OnScoreChanged;
        public event Action<int> OnMovesChanged;
        public event Action<int> OnGameOver;
        public event Action OnGameReset;

        public int Score => _score;
        public int MovesRemaining => _movesRemaining;
        public bool IsGameOver => _isGameOver;
        public GameConfig Config => config;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            ResetGame();
            Debug.Log($"GameManager ready. Moves: {_movesRemaining}");
        }

        /// <summary>
        /// Points param is flexible so Task 3 can pass the real block count
        /// instead of the hardcoded 10 from the test button.
        /// </summary>
        public void MakeMove(int points)
        {
            if (_isGameOver) return;

            _score += points;
            OnScoreChanged?.Invoke(_score);

            _movesRemaining--;
            OnMovesChanged?.Invoke(_movesRemaining);

            if (_movesRemaining <= 0)
            {
                _isGameOver = true;
                Debug.Log($"Game Over! Final score: {_score}");
                OnGameOver?.Invoke(_score);
            }
        }

        public void ResetGame()
        {
            _score = 0;
            _movesRemaining = config != null ? config.InitialMoves : 5;
            _isGameOver = false;

            OnScoreChanged?.Invoke(_score);
            OnMovesChanged?.Invoke(_movesRemaining);
            OnGameReset?.Invoke();
        }
    }
}
