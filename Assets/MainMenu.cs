using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour {
    public void PlayGame() {
        SceneManager.LoadScene((int)SceneId.BARRY_DEV_BOX);
    }
    
    public void QuitGame() {
        Debug.Log("Quit");
        Application.Quit();
    }
}
