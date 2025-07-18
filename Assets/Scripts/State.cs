using UnityEngine;
using System.Collections.Generic;

public struct State
{
    public int[][] BoardState;
    public List<int> FreePositions;
    public List<int> FreePieces;
    public int SelectedPiece;
}
