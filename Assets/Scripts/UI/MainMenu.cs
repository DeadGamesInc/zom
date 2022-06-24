using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void PlayGame() => SceneManager.LoadScene((int)SceneId.GAME);
    public void QuitGame() => Application.Quit();

    public void Logout() {
        PlayerPrefs.SetString("Account", "");
        PlayerPrefs.SetInt("RememberMe", 0);
        Destroy(GameController.GetGameObject());
        Destroy(Player.GetGameObject());
        SceneManager.LoadScene((int) SceneId.WEB3_LOGIN);
    }
}
