using UnityEngine;

namespace ChallengePlanA.Core
{
    /// <summary>
    /// Central configuration asset for the puzzle game.
    /// I chose ScriptableObject here so designers (or myself) can tweak values
    /// from the Inspector without touching code. It also ensures a single source
    /// of truth for game constants across all systems.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Plan A/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Grid")]
        [SerializeField] private int columns = 6;
        [SerializeField] private int rows = 5;

        [Header("Gameplay")]
        [SerializeField] private int initialMoves = 5;
        [SerializeField] private float refillDelay = 1f;

        [Header("Block Colors")]
        [Tooltip("Each color represents a block type. Index 0 = type 0, etc.")]
        [SerializeField] private Color[] blockColors = new Color[]
        {
            new Color(0.90f, 0.25f, 0.30f), // Red
            new Color(0.20f, 0.65f, 0.85f), // Blue
            new Color(0.30f, 0.80f, 0.40f), // Green
            new Color(0.95f, 0.75f, 0.15f), // Yellow
            new Color(0.70f, 0.35f, 0.85f), // Purple
        };

        [Header("Visual")]
        [SerializeField] private float blockSize = 1.2f;
        [SerializeField] private float blockSpacing = 0.15f;

        public int Columns => columns;
        public int Rows => rows;
        public int InitialMoves => initialMoves;
        public float RefillDelay => refillDelay;
        public Color[] BlockColors => blockColors;
        public float BlockSize => blockSize;
        public float BlockSpacing => blockSpacing;

        /// <summary>
        /// Total number of different block types available.
        /// I derive this from the color array length so adding a new color
        /// automatically creates a new block type - no magic numbers.
        /// </summary>
        public int BlockTypeCount => blockColors.Length;
    }
}
