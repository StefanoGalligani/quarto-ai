using UnityEngine;
using System.Collections.Generic;
using System.Linq;

struct WeightedMove
{
    public Move Move;
    public int Score;
}

public class AIPlayer : AbstractPlayer
{
    [SerializeField] private uint[] _maxDepths;
    private uint _maxDepth;
    private int _round;
    public override void StartTurn(int round, State state)
    {
        Move move = new Move();
        _round = round;
        _maxDepth = _maxDepths[Mathf.Min(_maxDepths.Length - 1, round)];
        switch (round)
        {
            case 0:
                move.PieceSelection = state.FreePieces[Random.Range(0, state.FreePieces.Count - 1)];
                break;
            case 1:
                state.FreePositions = new List<int> { 0, 1, 5 };
                move = BestMove(state, 0).Move;
                break;
            case 16:
                move.BoardPosition = state.FreePositions[0];
                break;
            default:
                move = BestMove(state, 0).Move;
                break;
        }
        gameManager.SubmitMove(move);
    }

    private WeightedMove BestMove(State state, uint depth)
    {
        Move bestMove = new Move();
        int bestScore = int.MinValue;
        int fallbackPiece = state.FreePieces.Count > 0 ? state.FreePieces[0] : 0;

        PossibleQuartos q = QuartoUtils.FindPossibleQuartos(state.Copy(), true);
        int pos = q.QuartoPosForPiece(state.SelectedPiece);
        if (pos > -1)
        {
            return new WeightedMove()
            {
                Move = new Move() { BoardPosition = pos, PieceSelection = fallbackPiece },
                Score = 1000
            };
        }
        
        foreach (Move m in GeneratePossiblePlacements(state.FreePositions))
        {
            State altState = state.Copy();
            altState.BoardState[QuartoUtils.BoardX(m.BoardPosition)][QuartoUtils.BoardY(m.BoardPosition)] = state.SelectedPiece;
            altState.FreePositions.Remove(m.BoardPosition);
            q = QuartoUtils.FindPossibleQuartos(altState);
            List<int> validPieces = q.PiecesThatDontCauseQuarto(altState.FreePieces, m.BoardPosition);
            if (validPieces.Count > 0) fallbackPiece = validPieces[0];

            if (depth == _maxDepth || _round + depth >= 15 || validPieces.Count == 0)
            //eventualmente si può spostare dentro il loop interno e considerare anche il pezzo selezionato
            //andrebbe modificata la funzione di valutazione per tenerne conto
            {
                Move move = new Move { BoardPosition = m.BoardPosition, PieceSelection = fallbackPiece };
                State newState = state.NextState(move);
                int newScore = validPieces.Count == 0 ? -1000 : PositionValue(newState);
                if (newScore > bestScore)
                {
                    bestMove = move;
                    bestScore = newScore;
                }
                continue;
            }
            foreach (Move move in GeneratePossibleSelections(m, validPieces))
            {
                State newState = state.NextState(move);
                int newScore = -BestMove(newState, depth + 1).Score;
                if (newScore > bestScore)
                {
                    bestMove = move;
                    bestScore = newScore;
                }
            }
        }
        if (bestScore > 0) bestScore--; else bestScore++;
        return new WeightedMove() { Move = bestMove, Score = bestScore };
    }

    private int PositionValue(State state)
    {
        return QuartoUtils.EvalScore(state);
    }

    private Move[] GeneratePossiblePlacements(List<int> freePositions)
    {
        var moves = new List<Move>();
        foreach (int pos in freePositions) moves.Add(new Move { BoardPosition = pos });
        return moves.ToArray();
    }

    private Move[] GeneratePossibleSelections(Move m, List<int> freePieces)
    {
        var moves = new List<Move>();
        foreach (int piece in freePieces) moves.Add(new Move { BoardPosition = m.BoardPosition, PieceSelection = piece });
        return moves.ToArray();
    }

    public override void EndTurn()
    {

    }

    public override string GetName()
    {
        return "AI";
    }
}
