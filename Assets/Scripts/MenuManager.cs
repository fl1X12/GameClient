using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Start()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.IsConnected())
        {
            NetworkManager.Instance.Disconnect();
        }
    }
    public void onPnPClick()
    {
        GameSetting.currentGameMode=GameMode.PnP;
        SceneManager.LoadScene("GameScene");
    }

    public void onPvBClick()
    {
        GameSetting.currentGameMode=GameMode.PvB;
        SceneManager.LoadScene("GameScene");
    }

    public async void onPvPClick()
    {
        GameSetting.currentGameMode=GameMode.PvP;
        await NetworkManager.Instance.ConnectAsync();
        SceneManager.LoadScene("GameScene");
    }
}
