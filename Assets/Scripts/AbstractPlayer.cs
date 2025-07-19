using UnityEngine;

public abstract class AbstractPlayer : MonoBehaviour
{
    [HideInInspector] public GameManager gameManager;
    public abstract void StartTurn(int round, State state);
    public abstract void EndTurn();
    public abstract string GetName();
}
