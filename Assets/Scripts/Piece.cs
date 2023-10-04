using System.Collections;
using System.Collections.Generic;
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
        private PieceData _pieceData;

        private void Start()
        {
        }
    }

    public class PieceData: IPiece
    {
        private int _row;
        private int _col;

        public PieceData(int row, int col)
        {
            Row = row; 
            Col = col;
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
    }
}
