using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlidingPuzzle
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _board;
        [SerializeField] private Piece _piecePrefab;
        [SerializeField] private int _boardSize;

        private Piece[,] _pieces;
        private float _tileSize;

        private void Start()
        {
            _pieces = new Piece[_boardSize, _boardSize];
            _tileSize = _piecePrefab.GetComponent<SpriteRenderer>().size.x;

            InstantiateGameObjects();
        }

        private void InstantiateGameObjects()
        {
            for (int row = 0; row < _pieces.GetLength(0); row++)
            {
                for (int col = 0; col < _pieces.GetLength(1); col++)
                {
                    _pieces[row, col] = Instantiate(_piecePrefab, GetTilePosition(row, col), Quaternion.identity);
                }
            }

            Vector3 centerBoard = new Vector3((float)_tileSize / 2 - 0.5f, (float)_tileSize / 2 - 0.5f, 0);
            SpriteRenderer board = Instantiate(_board, centerBoard, Quaternion.identity);
            board.size = new Vector3(_tileSize, _tileSize, 0);
        }

        private Vector3 GetTilePosition(int width, int heigth)
        {
            return new Vector3(width, heigth, 0) * _tileSize;
        }

        public void GetXY(Vector3 worldPos, ref int x, ref int y)
        {
            x = Mathf.FloorToInt(worldPos.x / _tileSize);
            y = Mathf.FloorToInt(worldPos.y / _tileSize);
        }
    }
}
