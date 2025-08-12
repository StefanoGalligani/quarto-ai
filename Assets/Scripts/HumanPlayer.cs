using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : AbstractPlayer
{
    [SerializeField] PiecesManager _piecesManager;
    [SerializeField] Button _moveBtn;
    private bool _canInteract = false;
    private bool _isFirstRound;
    private State _state;
    private GameObject _piece;
    private GameObject _boardPos;

    private void Start()
    {
    }

    public override void StartTurn(int round, State state)
    {
        _canInteract = true;
        _isFirstRound = round == 0;
        _state = state;
        _moveBtn.onClick.AddListener(SubmitMove);
    }
    public override void EndTurn()
    {
        _canInteract = false;
        _moveBtn.onClick.RemoveListener(SubmitMove);
    }

    void Update()
    {
        if (!_canInteract) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                LayerMask layer = hit.collider.gameObject.layer;
                int index = hit.collider.gameObject.transform.GetSiblingIndex();
                if (layer == LayerMask.NameToLayer("Pieces") && _state.FreePieces.Contains(index))
                {
                    _piece = hit.collider.gameObject;
                    _piecesManager.TemporarySelection(index);
                }
                if (layer == LayerMask.NameToLayer("Board") && _state.FreePositions.Contains(index))
                {
                    _boardPos = hit.collider.gameObject;
                    _piecesManager.TemporaryPlacement(index);
                }
            }
        }
    }

    public override string GetName()
    {
        return "Human";
    }

    public void SubmitMove()
    {
        if (!_canInteract) return;
        Move move;
        move.BoardPosition = _boardPos ? _boardPos.transform.GetSiblingIndex() : -1;
        move.PieceSelection = _piece ? _piece.transform.GetSiblingIndex() : -1;
        if (gameManager.IsValidMove(move))
        {
            _boardPos = null;
            _piece = null;
            gameManager.SubmitMove(move);
        }
        else
        {
            Debug.Log("Invalid move");
        }
    }
}
