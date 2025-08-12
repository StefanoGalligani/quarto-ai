using Unity.Jobs;
using UnityEngine;

public static class QuartoUtils
{
    public static int[] binaryMapping = new int[] {0, 1, 10, 11, 100, 101, 110, 111, 1000, 1001, 1010, 1011, 1100, 1101, 1110, 1111};

    private static bool IsQuarto(int binSum)
    {
        int d = binSum % 10;
        if (d == 4 || d == 0) return true;
        d = binSum / 10 % 10;
        if (d == 4 || d == 0) return true;
        d = binSum / 100 % 10;
        if (d == 4 || d == 0) return true;
        d = binSum / 1000 % 10;
        if (d == 4 || d == 0) return true;
        return false;
    }
    
    private static int QuartoScore(int binSum, int nPieces)
    {
        int[] scores = { 0, 0, 5, 20, 1000 };
        int score = 0;
        int d = binSum % 10;
        if (d == nPieces || d == 0) score += scores[nPieces];
        d = binSum / 10 % 10;
        if (d == nPieces || d == 0) score += scores[nPieces];
        d = binSum / 100 % 10;
        if (d == nPieces || d == 0) score += scores[nPieces];
        d = binSum / 1000 % 10;
        if (d == nPieces || d == 0) score += scores[nPieces];
        return score;
    }

    public static bool CheckQuarto(State state)
    {
        int[][] board = state.BoardState;
        //ROWS
        for (int i = 0; i < 4; i++)
        {
            if (board[i][0] == -1 || board[i][1] == -1 || board[i][2] == -1 || board[i][3] == -1) continue;
            int tot = 0;
            for (int j = 0; j < 4; j++)
            {
                tot += binaryMapping[board[i][j]];
            }
            if (IsQuarto(tot)) return true;
        }
        //COLUMNS
        for (int i = 0; i < 4; i++)
        {
            if (board[0][i] == -1 || board[1][i] == -1 || board[2][i] == -1 || board[3][i] == -1) continue;
            int tot = 0;
            for (int j = 0; j < 4; j++)
            {
                tot += binaryMapping[board[j][i]];
            }
            if (IsQuarto(tot)) return true;
        }
        //DIAGONALS
        if (!(board[0][0] == -1 || board[1][1] == -1 || board[2][2] == -1 || board[3][3] == -1))
        {
            int tot = 0;
            for (int i = 0; i < 4; i++)
            {
                tot += binaryMapping[board[i][i]];
            }
            if (IsQuarto(tot)) return true;
        }
        if (!(board[0][3] == -1 || board[1][2] == -1 || board[2][1] == -1 || board[3][0] == -1))
        {
            int tot = 0;
            for (int i = 0; i < 4; i++)
            {
                tot += binaryMapping[board[i][3 - i]];
            }
            if (IsQuarto(tot)) return true;
        }
        //SQUARES
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i][j] == -1 || board[i][j + 1] == -1 || board[i + 1][j] == -1 || board[i + 1][j + 1] == -1) continue;
                int tot = 0;
                tot += binaryMapping[board[i][j]] + binaryMapping[board[i][j + 1]] +
                    binaryMapping[board[i + 1][j]] + binaryMapping[board[i + 1][j + 1]];
                if (IsQuarto(tot)) return true;
            }
        }
        return false;
    }

    public static int EvalScore(State state)
    {
        int[][] board = state.BoardState;
        int score = 0;
        //ROWS
        for (int i = 0; i < 4; i++)
        {
            int nPieces = 4;
            int tot = 0;
            for (int j = 0; j < 4; j++)
            {
                if (board[i][j] == -1) nPieces--;
                else tot += binaryMapping[board[i][j]];
            }
            score += QuartoScore(tot, nPieces);
        }
        //COLUMNS
        for (int i = 0; i < 4; i++)
        {
            int nPieces = 4;
            int tot = 0;
            for (int j = 0; j < 4; j++)
            {
                if (board[j][i] == -1) nPieces--;
                else tot += binaryMapping[board[j][i]];
            }
            score += QuartoScore(tot, nPieces);
        }
        //DIAGONALS
        {
            int nPieces = 4;
            int tot = 0;
            for (int i = 0; i < 4; i++)
            {
                if (board[i][i] == -1) nPieces--;
                else tot += binaryMapping[board[i][i]];
            }
            score += QuartoScore(tot, nPieces);
        }
        {
            int nPieces = 4; 
            int tot = 0;
            for (int i = 0; i < 4; i++)
            {
                if (board[i][3 - i] == -1) nPieces--;
                else tot += binaryMapping[board[i][3 - i]];
            }
            score += QuartoScore(tot, nPieces);
        }
        //SQUARES
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int nPieces = 4;
                int tot = 0;
                if (board[i][j] == -1) nPieces--; else tot += binaryMapping[board[i][j]];
                if (board[i][j + 1] == -1) nPieces--; else tot += binaryMapping[board[i][j + 1]];
                if (board[i + 1][j] == -1) nPieces--; else tot += binaryMapping[board[i + 1][j]];
                if (board[i + 1][j + 1] == -1) nPieces--; else tot += binaryMapping[board[i + 1][j + 1]];
                score += QuartoScore(tot, nPieces);
            }
        }
        return score;
    }

    public static int BoardX(int pos)
    {
        return pos % 4;
    }
    
    public static int BoardY(int pos)
    {
        return pos / 4;
    }
}
