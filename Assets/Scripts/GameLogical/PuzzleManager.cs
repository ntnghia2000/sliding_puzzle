using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlidingPuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        public event DisplayFinishMessageCallback DisplayFinishMessage;

        private readonly float DELAY_TIME_BEFORE_SHUFFLING = 0.5f;
        private readonly float UNITY_REC_SIZE = 1.0f;
        private readonly float ADDITION_POSITION = 0.5f;
        private readonly float DISPLAY_GATEWAY_DELAY_TIME = 0.5f;
        private readonly int CENTER_PUZZLE = 2;

        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteRenderer _board;
        [SerializeField] private Piece _piecePrefab;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private GameObject _emptySpace;
        [SerializeField] private TimeManager _timeManager;

        private Piece[,] _pieces;
        private List<Piece> _piecesList;
        private GameObject _emptyPiece;
        private float _tileSize;
        private bool _isWinning = false;
        private int _targetX;
        private int _targetY;
        private bool _isShuffled = false;
        private bool _canPlay = false;

        private void Start()
        {
            GatherGameObjects();
            InstantiatePieces();
        }

        private void GatherGameObjects()
        {
            _pieces = new Piece[_width, _height];
            _piecesList = new List<Piece>();
            _tileSize = _piecePrefab.GetComponent<SpriteRenderer>().size.x;
            Vector3 center = new Vector3(_width / CENTER_PUZZLE, (float)_height / CENTER_PUZZLE, -10);
            _camera.transform.position = center;
            SpriteRenderer board = Instantiate(_board, new Vector2(center.x, center.y), Quaternion.identity);
            board.size = new Vector2(_width, _height);
        }

        private void InstantiatePieces()
        {
            Texture2D sourceTexture = _sprite.texture;
            int pieceWidth = sourceTexture.width / _width;
            int pieceHeight = sourceTexture.height / _height;

            for (int x = 0; x < _pieces.GetLength(0); x++)
            {
                for (int y = 0; y < _pieces.GetLength(1); y++)
                {
                    Sprite pieceSprite = InstantiatePiece(x, y, pieceWidth, pieceHeight, sourceTexture);

                    if (x == _pieces.GetLength(0) - 1 && y == _pieces.GetLength(1) - 1)
                    {
                        _pieces[x, y].InitPieceData(x, y, pieceSprite, true, _height);
                        _emptyPiece = Instantiate(_emptySpace, GetTilePosition(x, y), Quaternion.identity);
                    }
                    else
                    {
                        _pieces[x, y].InitPieceData(x, y, pieceSprite, false, _height);
                        _piecesList.Add(_pieces[x, y]);
                    }
                }
            }
        }

        private Sprite InstantiatePiece(int x, int y, int pieceWidth, int pieceHeight, Texture2D texture)
        {
            _pieces[x, y] = Instantiate(_piecePrefab, GetTilePosition(x, y), Quaternion.identity);
            _pieces[x, y].name = $"Piece {x * _height + y}";

            Rect rect = new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }

        public void StartShuffling()
        {
            if(!_isShuffled)
            {
                _isShuffled = true;
                StartCoroutine(Shuffle(DELAY_TIME_BEFORE_SHUFFLING));
                _canPlay = true;
            }
        }    

        private IEnumerator Shuffle(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            for (int col = 0; col < _pieces.GetLength(0); col++)
            {
                for (int row = 0; row < _pieces.GetLength(1); row++)
                {
                    int indexX = 0;
                    int indexY = 0;

                    RandomIndexes(col, row, ref indexX, ref indexY);

                    if (IsLastIndex(col, row, indexX, indexY))
                    {
                        RandomIndexes(col, row, ref indexX, ref indexY);
                    }
                    else
                    {
                        ShufflePieces(col, row, indexX, indexY);
                    }
                }
            }
        }

        private bool IsLastIndex(int col, int row, int randomIndexX, int randomIndexY)
        {
            if(col == _pieces.GetLength(0) - 1 && row == _pieces.GetLength(1) - 1 ||
            randomIndexX == _pieces.GetLength(0) - 1 && randomIndexY == _pieces.GetLength(1) - 1)
            {
                return true;
            }

            return false;
        }    

        private void RandomIndexes(int col, int row, ref int randomIndexX, ref int randomIndexY)
        {
            if (col + 1 >= _pieces.GetLength(0))
            {
                randomIndexX = 0;
            }
            else
            {
                randomIndexX = col + 1;
            }
            if (row - 1 < 0)
            {
                randomIndexY = _pieces.GetLength(1) - 1;
            }
            else
            {
                randomIndexY = row - 1;
            }
        }

        private void ShufflePieces(int col, int row, int randomIndexX, int randomIndexY)
        {
            Vector3 tempPosition = _pieces[col, row].transform.position;
            Vector3 tempPosition1 = _pieces[randomIndexX, randomIndexY].transform.position;
            _pieces[col, row].UpdatePosition(tempPosition1);
            _pieces[randomIndexX, randomIndexY].UpdatePosition(tempPosition);
            SwapPieces(ref _pieces[col, row], ref _pieces[randomIndexX, randomIndexY]);
        }    

        private void Update()
        {
            if (_isWinning)
            {
                _canPlay = false;
                StartCoroutine(DisplayMessage(DISPLAY_GATEWAY_DELAY_TIME));
                _timeManager.StopCountDown();
            }

            if(_timeManager.IsTimeOut)
            {
                _canPlay = false;
                StartCoroutine(DisplayMessage(DISPLAY_GATEWAY_DELAY_TIME));
                _timeManager.StopCountDown();
            }

            if(_canPlay)
            {
                InputHandler();
            }
        }

        private IEnumerator DisplayMessage(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            DisplayFinishMessage(_isWinning);
        }

        private void InputHandler()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(GetMousePosition(), Vector3.zero);

                if (CanSwap(hit.transform.position, _emptyPiece.transform.position))
                {
                    Vector3 tempPosition = _emptyPiece.transform.position;
                    _emptyPiece.transform.position = hit.transform.position;
                    hit.transform.position = tempPosition;

                    int tileX = Mathf.FloorToInt((hit.point.x - transform.position.x) / UNITY_REC_SIZE);
                    int tileY = Mathf.FloorToInt((hit.point.y - transform.position.y) / UNITY_REC_SIZE);

                    if (tileX >= 0 && tileX < _pieces.GetLength(0) && tileY >= 0 && tileY < _pieces.GetLength(1))
                    {
                        Debug.Log("hit piece: " + _pieces[tileX, tileY].GetPieceData().Row + " - " + _pieces[tileX, tileY].GetPieceData().Col);
                        Debug.Log("target piece: " + _pieces[_targetX, _targetY].GetPieceData().Row + " - " + _pieces[_targetX, _targetY].GetPieceData().Col);
                        _pieces[tileX, tileY].UpdatePosition(tempPosition);
                        Debug.Log("hit piece after update: " + _pieces[tileX, tileY].GetPieceData().Row + " - " + _pieces[tileX, tileY].GetPieceData().Col);
                        Debug.Log("target piece after update: " + _pieces[_targetX, _targetY].GetPieceData().Row + " - " + _pieces[_targetX, _targetY].GetPieceData().Col);
                        SwapPieces(ref _pieces[tileX, tileY], ref _pieces[_targetX, _targetY]);
                        Debug.Log("hit piece after swap: " + _pieces[tileX, tileY].GetPieceData().Row + " - " + _pieces[tileX, tileY].GetPieceData().Col);
                        Debug.Log("target piece after swap: " + _pieces[_targetX, _targetY].GetPieceData().Row + " - " + _pieces[_targetX, _targetY].GetPieceData().Col);
                    }
                }

                _isWinning = CheckWinCondition();
            }
        }    

        private Vector3 GetTilePosition(int width, int heigth)
        {
            return new Vector3((width * _tileSize) + ADDITION_POSITION, (heigth * _tileSize) + ADDITION_POSITION, 0);
        }

        private bool CheckWinCondition()
        {
            for(int i = 0; i < _piecesList.Count; i++)
            {
                if (_piecesList[i].PieceIndex != _piecesList[i].GetCurrentIndex())
                {
                    return false;
                }
            }

            return true;
        }

        private void SwapPieces(ref Piece movingPiece, ref Piece target)
        {
            Piece temp = target;
            target = movingPiece;
            movingPiece = temp;
        }    

        private bool CanSwap(Vector3 piece, Vector3 target)
        {
            if(Vector2.Distance(target, piece) < 2)
            {
                if(CheckVertical(piece, target) || CheckHorizontal(piece, target))
                {
                    _targetX = (int)target.x;
                    _targetY = (int)target.y;

                    return true;
                }
            }
            return false;
        }
        
        private bool CheckVertical(Vector3 piece, Vector3 target)
        {
            if(target.x == piece.x && target.y + 1 == piece.y ||
               target.x == piece.x && target.y - 1 == piece.y)
            {
                return true;
            } 
            
            return false;
        }

        private bool CheckHorizontal(Vector3 piece, Vector3 target)
        {
            if (target.x + 1 == piece.x && target.y == piece.y ||
                target.x - 1 == piece.x && target.y == piece.y)
            {
                return true;
            }

            return false;
        }

        private Vector3 GetMousePosition()
        {
            return _camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
