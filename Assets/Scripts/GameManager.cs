using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PiecesManager _piecesManager;
    [SerializeField] private int _currentSelection = -1;
    [SerializeField] private AbstractPlayer[] _aiPlayers;
    [SerializeField] private AbstractPlayer[] _humanPlayers;
    [SerializeField] private TextMeshProUGUI[] _scoreTexts;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private GameObject _moveBtn;
    [SerializeField] private GameObject _restartBtn;
    private AbstractPlayer[] _players;
    private int[] _scores = new int[2];
    private State _gameState;
    private int _turn = 0;
    private int _round = 0;

    private void Awake()
    {
        _players = new AbstractPlayer[2];
        _players[0] = PlayerPrefs.GetString("p1") == "h" ? _humanPlayers[0] : _aiPlayers[0];
        _players[1] = PlayerPrefs.GetString("p2") == "h" ? _humanPlayers[1] : _aiPlayers[1];
        SetupGame();
    }

    public void SetupGame()
    {
        _piecesManager.InitialPosition();
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
        _gameState.FreePositions = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        _gameState.FreePieces = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        ShowScores();
        _roundText.text = "Round: 0";
        _roundText.color = new Color(0.3f, 0.65f, 0.4f);
        _restartBtn.SetActive(false);
        _moveBtn.SetActive(true);
        _round = 0;
        _currentSelection = -1;
        _players[_turn].StartTurn(_round, _gameState.Copy());
    }

    private void ShowScores()
    {
        _scoreTexts[0].text = _players[0].GetName() + ": " + _scores[0];
        _scoreTexts[1].text = _players[1].GetName() + ": " + _scores[1];
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
        if (!IsValidMove(move))
        {
            Debug.Log("Invalid move");
            return;
        }
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
        _roundText.text = "Round: " + _round;

        var piecesInQuarto = new List<int>();
        if (QuartoUtils.CheckQuarto(_gameState, piecesInQuarto))
        {
            _scores[_turn]++;
            _players[_turn].EndTurn();
            _roundText.text = _players[_turn].GetName() + " Won";
            _roundText.color = _scoreTexts[_turn].color;
            _turn = 1 - _turn;
            ShowScores();
            _piecesManager.GameEnded(piecesInQuarto);
            _restartBtn.SetActive(true);
            _moveBtn.SetActive(false);
            return;
        }
        if (_round > 16)
        {
            _roundText.text = "Draw";
            _restartBtn.SetActive(true);
            _moveBtn.SetActive(false);
            return;
        }

        _piecesManager.PlaceSelection(_currentSelection);

        _players[_turn].EndTurn();
        _turn = 1 - _turn;
        _players[_turn].StartTurn(_round, _gameState.Copy());
    }
}
