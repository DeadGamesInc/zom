using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {
    [SerializeField] public Image Image;
    
    public void Start() {
        StartCoroutine(FadeImage());
    }

    private IEnumerator FadeImage() {
        for (float i = 2; i >= 0; i -= Time.deltaTime) {
            Image.color = new Color(1, 1, 1, i);
            yield return null;
        }
        
        SceneManager.LoadScene((int) SceneId.INIT);
    }
}
