using System.Collections.Generic;
using UnityEngine;

public struct PossibleQuartos
{
    public List<Move> moves;
    public void AddMove(int piece, int position)
    {
        if (moves == null) moves = new List<Move>();
        moves.Add(new Move { BoardPosition = position, PieceSelection = piece });
    }
    public int QuartoPosForPiece(int piece)
    {
        if (moves == null) moves = new List<Move>();
        foreach (Move move in moves)
        {
            if (move.PieceSelection == piece) return move.BoardPosition;
        }
        return -1;
    }

    public List<int> PiecesThatDontCauseQuarto(List<int> freePieces, int occupiedPosition)
    {
        if (moves == null) moves = new List<Move>();
        List<int> pieces = new List<int>(freePieces);
        foreach (Move move in moves)
        {
            if (move.BoardPosition != occupiedPosition) pieces.Remove(move.PieceSelection);
        }
        return pieces;
    }
}

public struct Move
{
    public int BoardPosition;
    public int PieceSelection;
}
