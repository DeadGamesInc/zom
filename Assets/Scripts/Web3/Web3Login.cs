using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Web3Login : MonoBehaviour {
    public Toggle RememberMe;
    
    public void Start() {
        if (!PlayerPrefs.HasKey("RememberMe") || !PlayerPrefs.HasKey("Account")) return;
        if (PlayerPrefs.GetInt("RememberMe") == 1 && !string.IsNullOrEmpty(PlayerPrefs.GetString("Account"))) 
            SceneManager.LoadScene((int) SceneId.SPLASH);
    }

    public async void HandleLogin() {
        var timestamp = (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        var expirationTime = timestamp + 60;
        var message = $"ZomGameLogin-This Is Not A Transaction-{expirationTime.ToString()}";
        var signature = await Web3Wallet.Sign(message);
        var account = await EVM.Verify(message, signature);
        var now = (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;

        if (account.Length == 42 && expirationTime >= now) {
            var remember = RememberMe.isOn ? 1 : 0;
            PlayerPrefs.SetString("Account", account);
            PlayerPrefs.SetInt("RememberMe", remember);
            SceneManager.LoadScene((int) SceneId.SPLASH);
        }
    }
}
