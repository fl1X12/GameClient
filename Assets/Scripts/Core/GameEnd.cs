using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI endGameText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    public void onMainMenuClick()
    {
        if(GameSetting.currentGameMode==GameMode.PvP && NetworkManager.Instance.IsConnected())
        {
            NetworkManager.Instance.Disconnect();
        }
        SceneManager.LoadScene("MenuScene");
    }

    public void endScreen(int winner)
    {
        switch (winner)
        {
            case Gametokens.Draw:
                endGameText.text="Game over \n Match is a draw";
                break;

            case Gametokens.X:
                endGameText.text="Game Over \n X is the winner";
                break;

            case Gametokens.O:
                endGameText.text="Game over \n O is the winner";
                break;
        }
        
    }
}
