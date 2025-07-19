using UnityEngine;
using System.Collections.Generic;

public class AIPlayer : AbstractPlayer
{
    public override void StartTurn(int round, State state)
    {
        Move move;
        move.BoardPosition = state.FreePositions[Random.Range(0, state.FreePositions.Count - 1)];
        move.PieceSelection = round < 16 ? state.FreePieces[Random.Range(0, state.FreePieces.Count - 1)] : -1;
        gameManager.SubmitMove(move);
    }
    
    public override void EndTurn()
    {

    }

    public override string GetName()
    {
        return "AI";
    }
}
