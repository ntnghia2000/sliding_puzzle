using UnityEngine;

namespace SlidingPuzzle
{
    public interface IPiece
    {
        int Row { get; set; }
        int Col { get; set; }
    }

    public class Piece : MonoBehaviour
    {
        private readonly float ADDITION_POSITION = 0.5f;

        [SerializeField] private GameObject _piece;
        [SerializeField] private SpriteRenderer _pieceRenderer;

        private PieceData _pieceData;
        private Vector3 _piecePosition;
        private int _pieceIndex;
        private int _height;

        private void Start()
        {
            _piecePosition = _piece.transform.position;
        }

        public void InitPieceData(int row, int col, Sprite sprite, bool isEmptySpace, int height)
        {
            _pieceData = new PieceData(row, col, isEmptySpace);
            _pieceRenderer.sprite = sprite;
            _height = height;
            _pieceIndex = (row * height + col);
        }
        
        public PieceData GetPieceData() 
        { 
            return _pieceData;
        }

        public bool IsEmptySpace()
        {
            return _pieceData.IsEmptySpace;
        }

        public void UpdatePosition(Vector3 position)
        {
            _piece.transform.position = position;
            _pieceData.Row = (int)(position.x - ADDITION_POSITION);
            _pieceData.Col = (int)(position.y - ADDITION_POSITION);
        }

        public int GetCurrentIndex()
        {
            return _pieceData.Row * _height + _pieceData.Col;
        }

        public int PieceIndex
        { get { return _pieceIndex; } }
    }

    public class PieceData: IPiece
    {
        private int _row;
        private int _col;
        private bool _isEmptySpace;

        public PieceData(int row, int col, bool isEmptySpace)
        {
            Row = row; 
            Col = col;
            _isEmptySpace = isEmptySpace;
        }

        public int Row
        {
            get => _row;
            set { _row = value; }
        }

        public int Col
        {
            get => _col;
            set { _col = value; }
        }

        public bool IsEmptySpace
        {
            get { return _isEmptySpace; }
            set { _isEmptySpace = value; }
        }
    }
}
