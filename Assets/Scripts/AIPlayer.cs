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
    [SerializeField] uint _maxDepth;
    public override void StartTurn(int round, State state)
    {
        Move move = new Move();
        switch (round)
        {
            case 0:
                move.PieceSelection = state.FreePieces[Random.Range(0, state.FreePieces.Count - 1)];
                break;
            case 16:
                move.BoardPosition = state.FreePositions[Random.Range(0, state.FreePositions.Count - 1)];
                break;
            default:
                move = BestMoveForPlayer(state, 0).Move;
                break;
        }
        gameManager.SubmitMove(move);
    }

    private WeightedMove BestMoveForPlayer(State state, uint depth)
    {
        Move bestMove = new Move();
        int bestScore = int.MinValue;
        //TODO cercare pezzi che fanno quarto e dove, se tra essi c'è il pezzo selezionato piazzarlo e selezionare arbitrariamente
        foreach (Move m in GeneratePossiblePlacements(state.FreePositions))
        {
            if (depth == _maxDepth)
            //eventualmente si può spostare dentro il loop interno e considerare anche il pezzo selezionato
            //andrebbe modificata la funzione di valutazione per tenerne conto
            {
                Move move = new Move { BoardPosition = m.BoardPosition, PieceSelection = state.FreePieces[0] };
                State newState = state.NextState(move);
                int newScore = PositionValue(newState);
                if (newScore > bestScore)
                {
                    bestMove = move;
                    bestScore = newScore;
                }
                continue;
            }
            //TODO rimuovere dalla selezione i pezzi che causerebbero quarto
            foreach (Move move in GeneratePossibleSelections(m, state.FreePieces))
            {
                State newState = state.NextState(move);
                int newScore = BestMoveForOpponent(newState, depth + 1).Score;
                if (newScore > bestScore)
                {
                    bestMove = move;
                    bestScore = newScore;
                }
            }
        }
        return new WeightedMove(){Move = bestMove, Score = bestScore};
    }

    private WeightedMove BestMoveForOpponent(State state, uint depth)
    {
        Move bestMove = new Move();
        int bestScore = int.MaxValue;
        //TODO cercare pezzi che fanno quarto e dove, se tra essi c'è il pezzo selezionato piazzarlo e selezionare arbitrariamente
        foreach (Move m in GeneratePossiblePlacements(state.FreePositions))
        {
            if (depth == _maxDepth)
            //eventualmente si può spostare dentro il loop interno e considerare anche il pezzo selezionato
            //andrebbe modificata la funzione di valutazione per tenerne conto
            {
                Move move = new Move { BoardPosition = m.BoardPosition, PieceSelection = state.FreePieces[0] };
                State newState = state.NextState(move);
                int newScore = -PositionValue(newState);
                if (newScore < bestScore)
                {
                    bestMove = move;
                    bestScore = newScore;
                }
                continue;
            }
            //TODO rimuovere dalla selezione i pezzi che causerebbero quarto
            foreach (Move move in GeneratePossibleSelections(m, state.FreePieces))
            {
                State newState = state.NextState(move);
                int newScore = BestMoveForPlayer(newState, depth + 1).Score;
                if (newScore < bestScore)
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
