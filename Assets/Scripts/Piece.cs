using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace SlidingPuzzle
{
    public interface IPiece
    {
        int Row { get; set; }
        int Col { get; set; }
    }

    public class Piece : MonoBehaviour
    {
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

        public void UpdatePosition(Vector2 position)
        {
            _piecePosition = position;
            _pieceData.Row = (int)(position.x - 0.5f);
            _pieceData.Col = (int)(position.y - 0.5f);
        }

        public int GetCurrentIndex()
        {
            Debug.Log(this.name + ": " + _pieceData.Row + " - " + _pieceData.Col);
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
