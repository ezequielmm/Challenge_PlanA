using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengePlanA.Core;
using ChallengePlanA.Data;

namespace ChallengePlanA.View
{
    /// <summary>
    /// Builds and manages the visual grid from GridModel data.
    /// Handles the gameplay loop: click -> collect -> wait -> gravity -> refill.
    /// </summary>
    public class GridView : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private Sprite blockSprite;

        private GridModel _model;
        private BlockView[,] _blockViews;
        private bool _isProcessing;

        private void Start()
        {
            if (config == null && GameManager.Instance != null)
                config = GameManager.Instance.Config;

            // fallback: create simple white sprite if none assigned
            if (blockSprite == null)
            {
                Texture2D tex = new Texture2D(4, 4);
                Color[] pixels = new Color[16];
                for (int i = 0; i < 16; i++) pixels[i] = Color.white;
                tex.SetPixels(pixels);
                tex.Apply();
                blockSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), Vector2.one * 0.5f, 4);
            }

            InitGrid();

            if (GameManager.Instance != null)
                GameManager.Instance.OnGameReset += HandleGameReset;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameReset -= HandleGameReset;
        }

        private void InitGrid()
        {
            ClearGrid();

            _model = new GridModel(config.Columns, config.Rows, config.BlockTypeCount);
            _model.FillRandom();
            _blockViews = new BlockView[config.Columns, config.Rows];

            float totalWidth = config.Columns * (config.BlockSize + config.BlockSpacing) - config.BlockSpacing;
            float totalHeight = config.Rows * (config.BlockSize + config.BlockSpacing) - config.BlockSpacing;

            float startX = -totalWidth / 2f + config.BlockSize / 2f;
            float startY = -totalHeight / 2f + config.BlockSize / 2f;

            for (int x = 0; x < config.Columns; x++)
            {
                for (int y = 0; y < config.Rows; y++)
                {
                    CreateBlockView(x, y, startX, startY);
                }
            }
        }

        private void CreateBlockView(int x, int y, float startX, float startY)
        {
            GameObject blockObj = new GameObject($"Block_{x}_{y}");
            blockObj.transform.SetParent(transform);

            float posX = startX + x * (config.BlockSize + config.BlockSpacing);
            float posY = startY + y * (config.BlockSize + config.BlockSpacing);
            blockObj.transform.localPosition = new Vector3(posX, posY, 0);
            blockObj.transform.localScale = Vector3.one * config.BlockSize;

            SpriteRenderer sr = blockObj.AddComponent<SpriteRenderer>();
            sr.sprite = blockSprite;

            blockObj.AddComponent<BoxCollider2D>();

            BlockView view = blockObj.AddComponent<BlockView>();
            int blockType = _model.GetBlock(x, y);
            Color color = config.BlockColors[blockType];
            view.Init(x, y, color, this);

            _blockViews[x, y] = view;
        }

        public void OnBlockClicked(int x, int y)
        {
            if (_isProcessing) return;

            var gm = GameManager.Instance;
            if (gm == null || gm.IsGameOver) return;

            List<Vector2Int> connected = BlockCollector.GetConnectedBlocks(_model, x, y);

            if (connected.Count > 0)
            {
                Debug.Log($"Tapped ({x},{y}), found {connected.Count} connected blocks");
                StartCoroutine(ProcessMove(connected));
            }
        }

        private IEnumerator ProcessMove(List<Vector2Int> blocksToRemove)
        {
            _isProcessing = true;

            int points = blocksToRemove.Count;

            foreach (var pos in blocksToRemove)
            {
                _model.ClearBlock(pos.x, pos.y);

                if (_blockViews[pos.x, pos.y] != null)
                {
                    Destroy(_blockViews[pos.x, pos.y].gameObject);
                    _blockViews[pos.x, pos.y] = null;
                }
            }

            GameManager.Instance.MakeMove(points);

            yield return new WaitForSeconds(config.RefillDelay);
            // apply gravity and refill
            GravityHandler.ApplyGravity(_model);
            RebuildVisuals();
            // Debug.Log("grid rebuilt after gravity");

            _isProcessing = false;
        }

        private void RebuildVisuals()
        {
            float totalWidth = config.Columns * (config.BlockSize + config.BlockSpacing) - config.BlockSpacing;
            float totalHeight = config.Rows * (config.BlockSize + config.BlockSpacing) - config.BlockSpacing;
            float startX = -totalWidth / 2f + config.BlockSize / 2f;
            float startY = -totalHeight / 2f + config.BlockSize / 2f;

            for (int x = 0; x < config.Columns; x++)
            {
                for (int y = 0; y < config.Rows; y++)
                {
                    int blockType = _model.GetBlock(x, y);

                    if (_blockViews[x, y] != null)
                    {
                        Color color = config.BlockColors[blockType];
                        _blockViews[x, y].SetColor(color);

                        float posX = startX + x * (config.BlockSize + config.BlockSpacing);
                        float posY = startY + y * (config.BlockSize + config.BlockSpacing);
                        _blockViews[x, y].transform.localPosition = new Vector3(posX, posY, 0);
                    }
                    else
                    {
                        CreateBlockView(x, y, startX, startY);
                    }
                }
            }
        }

        private void ClearGrid()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            _blockViews = null;
        }

        private void HandleGameReset()
        {
            _isProcessing = false;
            InitGrid();
        }
    }
}
