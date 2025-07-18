using UnityEngine;

public abstract class AbstractPlayer : MonoBehaviour
{
    public GameManager gameManager;
    public abstract void StartTurn(int round, State state);
    public abstract void EndTurn();
}
