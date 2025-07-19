using UnityEngine;

public class PiecesManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _pieces;
    [SerializeField] private float _spacing;
    [SerializeField] private Vector2 _initialPos;
    [SerializeField] private float _boardOffset;
    [SerializeField] private Vector2 _firstCell;
    [SerializeField] private float _boardSpacing;
    private int _selectionIndex = -1;
    private int _temporarySelectionIndex = -1;

    public void Awake()
    {
        InitialPosition();
    }

    public void PlacePiece(int piece, int pos)
    {
        int posx = QuartoUtils.BoardX(pos);
        int posy = QuartoUtils.BoardY(pos);
        _pieces[piece].transform.position = new Vector3(_firstCell.x + posx * _boardSpacing, 0, _firstCell.y + posy * _boardSpacing);
    }

    public void PlaceSelection(int piece)
    {
        _pieces[piece].transform.position = new Vector3(0, 0.4f, 0);
        _selectionIndex = piece;
        _temporarySelectionIndex = -1;
    }

    public void TemporarySelection(int piece)
    {
        if (_temporarySelectionIndex >= 0)
        {
            _pieces[_temporarySelectionIndex].transform.position -= Vector3.up * 0.3f;
        }
        _pieces[piece].transform.position += Vector3.up * 0.3f;
        _temporarySelectionIndex = piece;
    }

    public void TemporaryPlacement(int position)
    {
        if (_selectionIndex < 0) return;
        int posx = QuartoUtils.BoardX(position);
        int posy = QuartoUtils.BoardY(position);

        _pieces[_selectionIndex].transform.position = new Vector3(_firstCell.x + posx * _boardSpacing, 0.3f, _firstCell.y + posy * _boardSpacing);
    }

    public void InitialPosition()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float offset = (i > 1) ? _boardOffset : 0;
                _pieces[i * 4 + j].transform.position = new Vector3(_initialPos.x + i * _spacing + offset, 0, _initialPos.y + j * _spacing);
            }
        }
    }
}
