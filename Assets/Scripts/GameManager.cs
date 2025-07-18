using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PiecesManager _piecesManager;
    [SerializeField] private int _currentSelection = -1;
    [SerializeField] private AbstractPlayer[] _players;
    private State _gameState;
    private int _turn = 0;
    private int _round = 0;

    private void Start()
    {
        _gameState.BoardState = new int[4][];
        for (int i = 0; i < 4; i++)
        {
            _gameState.BoardState[i] = new int[4];
            for (int j = 0; j < 4; j++)
            {
                _gameState.BoardState[i][j] = -1;
            }
        }
        _players[0].gameManager = this;
        _players[1].gameManager = this;
        _gameState.FreePositions = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
        _gameState.FreePieces = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

        _players[_turn].StartTurn(_round, _gameState);
    }

    public bool IsValidMove(Move move)
    {
        if (_round > 0)
        {
            if (move.BoardPosition < 0) return false;
            if (!_gameState.FreePositions.Contains(move.BoardPosition)) return false;
        }
        if (_round < 16)
        {
            if (move.PieceSelection < 0) return false;
            if (!_gameState.FreePieces.Contains(move.PieceSelection)) return false;
        }
        return true;
    }

    public void SubmitMove(Move move)
    {
        if (!IsValidMove(move)) return;
        ApplyMove(move);
    }

    private void ApplyMove(Move move)
    {
        if (_currentSelection >= 0)
        {
            _piecesManager.PlacePiece(_currentSelection, move.BoardPosition);
            int posx = QuartoUtils.BoardX(move.BoardPosition);
            int posy = QuartoUtils.BoardY(move.BoardPosition);
            _gameState.BoardState[posx][posy] = _currentSelection;
            _gameState.FreePositions.Remove(move.BoardPosition);
        }
        _currentSelection = move.PieceSelection;
        _gameState.SelectedPiece = move.PieceSelection;
        _gameState.FreePieces.Remove(move.PieceSelection);
        _round++;

        if (QuartoUtils.CheckQuarto(_gameState))
        {
            Debug.Log("Player " + _turn + " won");
            return;
        }
        if (_round > 16)
        {
            Debug.Log("Draw");
            return;
        }

        _piecesManager.PlaceSelection(_currentSelection);

        _players[_turn].EndTurn();
        _turn = 1 - _turn;
        _players[_turn].StartTurn(_round, _gameState);
    }
}
