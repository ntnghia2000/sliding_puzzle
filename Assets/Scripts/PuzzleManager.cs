using SlidingPuzzle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlidingPuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _board;
        [SerializeField] private Piece _piecePrefab;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private GameObject _emptySpace;

        private Piece[,] _pieces;
        private List<Piece> _piecesList;
        private GameObject _emptyPiece;
        private float _tileSize;
        private bool _isWinning;
        private int _targetX;
        private int _targetY;

        private void Start()
        {
            _pieces = new Piece[width, height];
            _piecesList = new List<Piece>();
            _tileSize = _piecePrefab.GetComponent<SpriteRenderer>().size.x;

            InstantiatePieces();

            StartCoroutine(Shuffle(1.0f));
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(GetMousePosition(), Vector2.zero);

                if (CanSwap(hit.transform.position, _emptyPiece.transform.position))
                {
                    Vector3 tempPosition = _emptyPiece.transform.position;
                    _emptyPiece.transform.position = hit.transform.position;
                    hit.transform.position = tempPosition;

                    int tileX = Mathf.FloorToInt((hit.point.x - transform.position.x) / 1.0f);
                    int tileY = Mathf.FloorToInt((hit.point.y - transform.position.y) / 1.0f);

                    if (tileX >= 0 && tileX < _pieces.GetLength(0) && tileY >= 0 && tileY < _pieces.GetLength(1))
                    {
                        _pieces[tileX, tileY].UpdatePosition(tempPosition);
                        SwapPiece(ref _pieces[tileX, tileY], ref _pieces[_targetX, _targetY]);
                    }
                }

                CheckWinCondition();
            }

            if(_isWinning)
            {
                Debug.Log("Winning");
            }
        }

        private void InstantiatePieces()
        {
            Texture2D sourceTexture = _sprite.texture;
            int pieceWidth = sourceTexture.width / width;
            int pieceHeight = sourceTexture.height / height;

            for (int x = 0; x < _pieces.GetLength(0); x++)
            {
                for (int y = 0; y < _pieces.GetLength(1); y++)
                {
                    _pieces[x, y] = Instantiate(_piecePrefab, GetTilePosition(x, y), Quaternion.identity);
                    _pieces[x, y].name = $"Piece {x * height + y}";
                    Rect rect = new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight);
                    Sprite pieceSprite = Sprite.Create(sourceTexture, rect, new Vector2(0.5f, 0.5f));
                    if(x == _pieces.GetLength(0) - 1 && y == _pieces.GetLength(1) - 1)
                    {
                        _pieces[x, y].InitPieceData(x, y, pieceSprite, true, height);
                        _pieces[x, y].gameObject.SetActive(false);
                        _emptyPiece = Instantiate(_emptySpace, GetTilePosition(x, y), Quaternion.identity);
                    }
                    else
                    {
                        _pieces[x, y].InitPieceData(x, y, pieceSprite, false, height);
                        _piecesList.Add(_pieces[x, y]);
                    }
                }
            }
        }

        private Vector3 GetTilePosition(int width, int heigth)
        {
            return new Vector3(width, heigth, 0) * _tileSize + new Vector3(0.5f, 0.5f, 0);
        }

        public void GetXY(Vector3 worldPos, ref int x, ref int y)
        {
            x = Mathf.FloorToInt(worldPos.x / _tileSize);
            y = Mathf.FloorToInt(worldPos.y / _tileSize);

        }

        private IEnumerator Shuffle(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            for (int x = 0; x < _pieces.GetLength(0); x++)
            {
                for (int y = 0; y < _pieces.GetLength(1); y++)
                {
                    int indexX = 0;
                    int indexY = 0;

                    GetRandomIndexes(x, y, ref indexX, ref indexY);

                    if (x == _pieces.GetLength(0) - 1 && y == _pieces.GetLength(1) - 1 ||
                        indexX == _pieces.GetLength(0) - 1 && indexY == _pieces.GetLength(1) - 1)
                    {
                        GetRandomIndexes(x, y, ref indexX, ref indexY);
                    }
                    else
                    {
                        Vector3 tempPosition = _pieces[x, y].transform.position;
                        Vector3 tempPosition1 = _pieces[indexX, indexY].transform.position;
                        _pieces[x, y].UpdatePosition(tempPosition1);
                        _pieces[indexX, indexY].UpdatePosition(tempPosition);
                        SwapPiece(ref _pieces[x, y], ref _pieces[indexX, indexY]);
                    }
                }
            }
            
        }

        private void GetRandomIndexes(int x, int y, ref int indexX, ref int indexY)
        {
            if (x + 1 >= _pieces.GetLength(0))
            {
                indexX = 0;
            }
            else
            {
                indexX = x + 1;
            }
            if (y - 1 < 0)
            {
                indexY = _pieces.GetLength(1) - 1;
            }
            else
            {
                indexY = y - 1;
            }
        }

        private bool CheckWinCondition()
        {
            for(int i = 0; i < _piecesList.Count; i++)
            {
                if (_piecesList[i].PieceIndex != _piecesList[i].GetCurrentIndex())
                {
                    _isWinning = false;
                    return _isWinning;
                }
            }

            _isWinning = true;
            return _isWinning;
        }

        private void SwapPiece(ref Piece movingPiece, ref Piece target)
        {
            Piece temp = target;
            target = movingPiece;
            movingPiece = temp;
        }    

        private bool CanSwap(Vector2 piece, Vector2 target)
        {
            if(Vector2.Distance(target, piece) < 2)
            {
                if(target.x + 1 == piece.x && target.y == piece.y ||
                    target.x - 1 == piece.x && target.y == piece.y ||
                    target.x == piece.x && target.y + 1 == piece.y ||
                    target.x == piece.x && target.y - 1 == piece.y)
                {
                    _targetX = (int)target.x;
                    _targetY = (int)target.y;

                    return true;
                }
            }
            return false;
        }   

        private Vector3 GetMousePosition()
        {
            return _camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
