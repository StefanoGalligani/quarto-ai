using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void StartGame(int players)
    {
        int p1 = players / 10;
        int p2 = players % 10;
        PlayerPrefs.SetString("p1", p1 == 0 ? "h" : "a");
        PlayerPrefs.SetString("p2", p2 == 0 ? "h" : "a");
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
