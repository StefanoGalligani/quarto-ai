using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public struct State
{
    public int[][] BoardState;
    public List<int> FreePositions;
    public List<int> FreePieces;
    public int SelectedPiece;

    public State Copy()
    {
        return new State
        {
            BoardState = CopyBoardState(this.BoardState),
            FreePositions = new List<int>(this.FreePositions),
            FreePieces = new List<int>(this.FreePieces),
            SelectedPiece = this.SelectedPiece
        };
    }

    public State NextState(Move move)
    {
        State s = new State
        {
            BoardState = CopyBoardState(this.BoardState),
            FreePositions = new List<int>(this.FreePositions),
            FreePieces = new List<int>(this.FreePieces)
        };
        s.BoardState[QuartoUtils.BoardX(move.BoardPosition)][QuartoUtils.BoardY(move.BoardPosition)] = this.SelectedPiece;
        s.FreePositions.Remove(move.BoardPosition);

        s.FreePieces.Remove(move.PieceSelection);
        s.SelectedPiece = move.PieceSelection;

        return s;
    }

    private int[][] CopyBoardState(int[][] board)
    {
        int[][] newBoard = new int[4][];
        for (int i = 0; i < 4; i++)
        {
            newBoard[i] = new int[4];
            for (int j = 0; j < 4; j++)
            {
                newBoard[i][j] = board[i][j];
            }
        }
        return newBoard;
    }
}
