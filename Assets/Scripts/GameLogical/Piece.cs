using System;
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

        [SerializeField] private PieceData _pieceData;
        [SerializeField] private int _pieceIndex;
        private int _height;

        public void InitPieceData(int col, int row, Sprite sprite, bool isEmptySpace, int height)
        {
            _pieceData = new PieceData(col, row, isEmptySpace);
            _pieceRenderer.sprite = sprite;
            _height = height;
            _pieceIndex = (col * height + row);
            gameObject.SetActive(!isEmptySpace);
        }
        
        public PieceData GetPieceData() 
        { 
            return _pieceData;
        }

        public bool IsEmptySpace()
        {
            return _pieceData.IsEmptySpace;
        }

        public void SwapPieceByPosition(Vector3 position, int col, int row)
        {
            _piece.transform.position = position;
            _pieceData.Col = col;
            _pieceData.Row = row;
        }

        public int GetCurrentIndex()
        {
            return _pieceData.Col * _height + _pieceData.Row;
        }

        public int PieceIndex
        { get { return _pieceIndex; } }
    }

    [Serializable]
    public class PieceData: IPiece
    {
        [SerializeField] private int _row;
        [SerializeField] private int _col;
        private bool _isEmptySpace;

        public PieceData(int col, int row, bool isEmptySpace)
        {
            Col = col;
            Row = row;
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
