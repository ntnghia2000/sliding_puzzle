using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        private Piece _emptyPiece;
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

            for (int col = 0; col < _pieces.GetLength(0); col++)
            {
                for (int row = 0; row < _pieces.GetLength(1); row++)
                {
                    Sprite pieceSprite = InstantiatePiece(col, row, pieceWidth, pieceHeight, sourceTexture);

                    if (col == _pieces.GetLength(0) - 1 && row == _pieces.GetLength(1) - 1)
                    {
                        _pieces[col, row].InitPieceData(col, row, pieceSprite, true, _height);
                        _emptyPiece = Instantiate(_pieces[col, row]);
                        _emptyPiece.InitPieceData(col, row, pieceSprite, true, _height);
                        _emptyPiece.name = "Empty Piece";
                    }
                    else
                    {
                        _pieces[col, row].InitPieceData(col, row, pieceSprite, false, _height);
                        _piecesList.Add(_pieces[col, row]);
                    }
                }
            }
        }

        private Sprite InstantiatePiece(int col, int row, int pieceWidth, int pieceHeight, Texture2D texture)
        {
            _pieces[col, row] = Instantiate(_piecePrefab, GetTilePositionOnGrid(col, row), Quaternion.identity);
            _pieces[col, row].name = $"Piece {col * _height + row}";

            Rect rect = new Rect(col * pieceWidth, row * pieceHeight, pieceWidth, pieceHeight);
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
                    int randomCol = 0;
                    int randomRow = 0;

                    if (!IsLastIndex(col, row))
                    {
                        RandomIndexes(col, row, ref randomCol, ref randomRow);
                        ShufflePieces(col, row, randomCol, randomRow);
                    }
                }
            }
        }

        private bool IsLastIndex(int col, int row)
        {
            if(col == _pieces.GetLength(0) - 1 && row == _pieces.GetLength(1) - 1)
            {
                return true;
            }

            return false;
        }    

        private Vector2 RandomIndexes(int col, int row, ref int randomCol, ref int randomRow)
        {
            randomCol = col + 1 >= _pieces.GetLength(0) ? 0 : col + 1;
            randomRow = row - 1 < 0 ? _pieces.GetLength(1) - 1 : row - 1;

            if(!IsLastIndex(randomCol, randomRow))
            {
                return new Vector2(randomCol, randomRow);
            }

            return RandomIndexes(col - 1, row + 1, ref randomCol, ref randomRow);
        }

        private void ShufflePieces(int col, int row, int randomCol, int randomRow)
        {
            Vector3 currentPiecePos = _pieces[col, row].transform.position;
            Vector3 randomPiecePos = _pieces[randomCol, randomRow].transform.position;

            _pieces[col, row].SwapPieceByPosition(randomPiecePos, randomCol, randomRow);
            _pieces[randomCol, randomRow].SwapPieceByPosition(currentPiecePos, col, row);
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
                    Vector3 emptyPiecePosition = _emptyPiece.transform.position;
                    Vector3 hitPiecePosition = hit.transform.position;

                    Piece hitPiece = hit.transform.GetComponent<Piece>();
                    int hitCol = hitPiece.GetPieceData().Col;
                    int hitRow = hitPiece.GetPieceData().Row;

                    if (hitCol >= 0 && hitCol < _pieces.GetLength(0) && hitRow >= 0 && hitRow < _pieces.GetLength(1))
                    {
                        hitPiece.SwapPieceByPosition(emptyPiecePosition, _emptyPiece.GetPieceData().Col, _emptyPiece.GetPieceData().Row);
                        _emptyPiece.SwapPieceByPosition(hitPiecePosition, hitCol, hitRow);
                    }
                }

                _isWinning = CheckWinCondition();
            }
        }    

        private Vector3 GetTilePositionOnGrid(int col, int row)
        {
            return new Vector3((col * _tileSize) + ADDITION_POSITION, (row * _tileSize) + ADDITION_POSITION, 0);
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
