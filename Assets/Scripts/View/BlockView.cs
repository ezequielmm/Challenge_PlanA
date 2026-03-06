using UnityEngine;
using UnityEngine.EventSystems;

namespace ChallengePlanA.View
{
    /// <summary>
    /// Sits on each block GameObject. Handles click and stores grid position.
    /// Uses IPointerClickHandler so it works with both mouse and touch input.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BlockView : MonoBehaviour, IPointerClickHandler
    {
        private int _x;
        private int _y;
        private SpriteRenderer _renderer;
        private GridView _gridView;

        public int X => _x;
        public int Y => _y;

        public void Init(int x, int y, Color color, GridView gridView)
        {
            _x = x;
            _y = y;
            _gridView = gridView;

            _renderer = GetComponent<SpriteRenderer>();
            _renderer.color = color;

            // collider needs to match the sprite for clicks to work
            var collider = GetComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        public void SetColor(Color color)
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            _renderer.color = color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_gridView != null)
                _gridView.OnBlockClicked(_x, _y);
        }
    }
}
