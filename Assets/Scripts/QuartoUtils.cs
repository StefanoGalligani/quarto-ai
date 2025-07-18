using UnityEngine;

public static class QuartoUtils
{
    public static bool CheckQuarto(State state)
    {
        return false;
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
