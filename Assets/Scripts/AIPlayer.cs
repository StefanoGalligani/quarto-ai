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
    private uint _round;
    public override void StartTurn(int round, State state)
    {
        Move move = new Move();
        _round = (uint)round;
        _maxDepth = _maxDepths[Mathf.Min(_maxDepths.Length - 1, round)];
        switch (round)
        {
            case 0:
                move.PieceSelection = state.FreePieces[Random.Range(0, state.FreePieces.Count - 1)];
                break;
            case 1:
                move.PieceSelection = (7 - state.SelectedPiece / 2) * 2 + state.SelectedPiece % 2;
                List<int> angles = new List<int>() { 0, 3, 12, 15 };
                move.BoardPosition = angles[Random.Range(0, 3)];
                break;
            case 16:
                move.BoardPosition = state.FreePositions[0];
                break;
            default:
                move = BestMove(state, 0, -10000).Move;
                break;
        }
        gameManager.SubmitMove(move);
    }

    private WeightedMove BestMove(State state, uint depth, int currentBest) {
        Move bestMove = new Move();
        int bestScore = -10000;
        int fallbackPiece = state.FreePieces.Count > 0 ? state.FreePieces[0] : 0;
        State originalState = state.Copy();

        SimplifyState(state, _round + depth);

        PossibleQuartos q;
        if (_round > 3)
        {
            q = QuartoUtils.FindPossibleQuartos(state.Copy(), true);
            int pos = q.QuartoPosForPiece(state.SelectedPiece);
            if (pos > -1)
            {
                return new WeightedMove()
                {
                    Move = new Move() { BoardPosition = pos, PieceSelection = fallbackPiece },
                    Score = 1000
                };
            }
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
                State newState = originalState.NextState(move);
                int newScore = -BestMove(newState, depth + 1, bestScore).Score;
                //first condition is a guaranteed quarto, second is for alpha-beta pruning
                if (newScore > 980 || newScore > -currentBest)
                {
                    return new WeightedMove() { Move = move, Score = newScore };
                }
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

    //In the first two rounds many pieces and positions are symmetrically equivalent
    private void SimplifyState(State state, uint actualRound)
    {
        if (actualRound == 1) //unused
        {
            var altPos = new List<int> { 0, 1, 5 };
            int p0 = 15 - state.SelectedPiece;
            int p1 = (7 - state.SelectedPiece / 2) * 2 + state.SelectedPiece % 2;
            int p2 = (3 - state.SelectedPiece / 4) * 4 + state.SelectedPiece % 4;
            int p3 = (1 - state.SelectedPiece / 8) * 8 + state.SelectedPiece % 8;
            var altPieces = new List<int> { p1, p2, p3, p0 };
            state.FreePositions = altPos;
            state.FreePieces = altPieces;
        }
        if (actualRound == 2)
        {
            int placedPiece = -1;
            int placedPosX = -1;
            int placedPosY = -1;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (state.BoardState[i][j] != -1)
                    {
                        placedPiece = state.BoardState[i][j];
                        placedPosX = i;
                        placedPosY = j;
                    }
                }
            }
            var altPos = state.FreePositions;
            if (placedPosX - placedPosY == 0)
            {
                altPos = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        if (i != placedPosX || j != placedPosY)
                        {
                            altPos.Add(i + j * 4);
                        }
                    }
                }
            }
            else if (placedPosX + placedPosY == 3)
            {
                altPos = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j <= 3 - i; j++)
                    {
                        if (i != placedPosX || j != placedPosY)
                        {
                            altPos.Add(i + j * 4);
                        }
                    }
                }
            }
            state.FreePositions = altPos;
            int p0 = 15 - state.SelectedPiece;
            int p1 = (7 - state.SelectedPiece / 2) * 2 + state.SelectedPiece % 2;
            int p2 = (3 - state.SelectedPiece / 4) * 4 + state.SelectedPiece % 4;
            int p3 = (1 - state.SelectedPiece / 8) * 8 + state.SelectedPiece % 8;
            var altPieces = new List<int> { p0, p1, p2, p3 };
            state.FreePositions = altPos;
            state.FreePieces = altPieces;
        }
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
