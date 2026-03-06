using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ChallengePlanA.UI
{
    /// <summary>
    /// Subscribes to GameManager events and updates the UI texts.
    /// This class dont hold any game state, it just reflects it.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private TextMeshProUGUI _scoreText;
        private TextMeshProUGUI _movesText;
        private GameObject _gameOverPanel;
        private TextMeshProUGUI _finalScoreText;
        private Button _replayButton;
        private Button _makeMoveButton;

        private void Awake()
        {
            FindUIReferences();
        }

        private void OnEnable()
        {
            if (Core.GameManager.Instance != null)
                SubscribeEvents();
        }

        private void Start()
        {
            SubscribeEvents();
            SetupButtons();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        /// <summary>
        /// I grab references by name at runtime. Its less safe than SerializeField
        /// but avoids the missing reference issue if scene gets rebuilt.
        /// </summary>
        private void FindUIReferences()
        {
            GameObject header = GameObject.Find("Header");
            if (header != null)
            {
                Transform scoreT = header.transform.Find("ScoreText");
                if (scoreT != null) _scoreText = scoreT.GetComponent<TextMeshProUGUI>();

                Transform movesT = header.transform.Find("MovesText");
                if (movesT != null) _movesText = movesT.GetComponent<TextMeshProUGUI>();
            }

            // Find wont catch inactive objects so we search manually
            _gameOverPanel = GameObject.Find("GameOverPanel");
            if (_gameOverPanel == null)
            {
                GameObject canvas = GameObject.Find("MainCanvas");
                if (canvas != null)
                {
                    foreach (Transform child in canvas.transform)
                    {
                        if (child.name == "GameOverPanel")
                        {
                            _gameOverPanel = child.gameObject;
                            break;
                        }
                    }
                }
            }

            if (_gameOverPanel != null)
            {
                Transform finalScoreT = _gameOverPanel.transform.Find("FinalScoreText");
                if (finalScoreT != null) _finalScoreText = finalScoreT.GetComponent<TextMeshProUGUI>();

                Transform replayT = _gameOverPanel.transform.Find("ReplayButton");
                if (replayT != null) _replayButton = replayT.GetComponent<Button>();
            }

            GameObject makeMoveObj = GameObject.Find("MakeMoveButton");
            if (makeMoveObj != null) _makeMoveButton = makeMoveObj.GetComponent<Button>();
        }

        private void SubscribeEvents()
        {
            var gm = Core.GameManager.Instance;
            if (gm == null) return;

            gm.OnScoreChanged += HandleScoreChanged;
            gm.OnMovesChanged += HandleMovesChanged;
            gm.OnGameOver += HandleGameOver;
            gm.OnGameReset += HandleGameReset;
        }

        private void UnsubscribeEvents()
        {
            var gm = Core.GameManager.Instance;
            if (gm == null) return;

            gm.OnScoreChanged -= HandleScoreChanged;
            gm.OnMovesChanged -= HandleMovesChanged;
            gm.OnGameOver -= HandleGameOver;
            gm.OnGameReset -= HandleGameReset;
        }

        private void SetupButtons()
        {
            if (_replayButton != null)
            {
                _replayButton.onClick.RemoveAllListeners();
                _replayButton.onClick.AddListener(OnReplayPressed);
            }

            if (_makeMoveButton != null)
            {
                _makeMoveButton.onClick.RemoveAllListeners();
                _makeMoveButton.onClick.AddListener(OnMakeMovePressed);
            }
        }

        private void HandleScoreChanged(int newScore)
        {
            if (_scoreText != null)
                _scoreText.text = newScore.ToString();
        }

        private void HandleMovesChanged(int newMoves)
        {
            if (_movesText != null)
                _movesText.text = newMoves.ToString();
        }

        private void HandleGameOver(int finalScore)
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(true);

            if (_finalScoreText != null)
                _finalScoreText.text = "Score: " + finalScore;
        }

        private void HandleGameReset()
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);
        }

        private void OnReplayPressed()
        {
            var gm = Core.GameManager.Instance;
            if (gm != null) gm.ResetGame();
        }

        // test button for Task 2, will be removed in Task 3
        private void OnMakeMovePressed()
        {
            var gm = Core.GameManager.Instance;
            if (gm != null) gm.MakeMove(10);
        }
    }
}
