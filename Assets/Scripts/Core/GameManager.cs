//GameManager.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField]UIController UIController;

    [SerializeField]GameEngine gameEngine;
    
    [SerializeField]GameEnd gameEnd;


    public int side;
    int gridLen;
    
    [Header("Game Mode Settings")]
    GameMode gameMode;
    private int playerIndex = -1;  
    int turn;
    
    void Start()
    {
        UIController.gameObject.SetActive(true);
        gameEnd.gameObject.SetActive(false);
        
        side=GameSetting.boardSide;
        gameEngine.InitializeEngine(side);
        gridLen=side*side;

        
        gameMode=GameSetting.currentGameMode;

        switch (gameMode)
        {
            case GameMode.PnP:
                PnP();
                break;
            
            case GameMode.PvB:
                PvB();
                break;

            case GameMode.PvP:
                PvP();
                break;

            default:
                Debug.Log("Game Mode error");
                break;
        }

        UIController.GenerateBoard(side,OnCellClicked);
    }

    private void PvB()
    {
        Debug.Log("Playing with bot");
        playerIndex=gameEngine.GetPlayerIndex();
    }

    private void PnP()
    {
        Debug.Log("Pass and play");
        playerIndex=gameEngine.GetPlayerIndex();
    }
    
    private void PvP()
    {
        Debug.Log("Online mode");
        UIController.SetPlayerTokenIndicator(true);
            
        NetworkManager.Instance.OnGameStart += OnOnlineGameStart;
        NetworkManager.Instance.OnOppMove += OnOnlineOpponentMove;  
        NetworkManager.Instance.OnGameEnd += OnOnlineGameOver;

        Debug.Log("Searching for online match...");
        NetworkManager.Instance.FindMatch();
    }
    void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnGameStart -= OnOnlineGameStart;
            NetworkManager.Instance.OnOppMove -= OnOnlineOpponentMove;  
            NetworkManager.Instance.OnGameEnd -= OnOnlineGameOver;
        }
    }

    void GameOver(int winner)
    {
        UIController.gameObject.SetActive(false);
        gameEnd.gameObject.SetActive(true);
        gameEnd.endScreen(winner);

        StopAllCoroutines();
    }

    
    void OnCellClicked(int index)
    {
        Debug.Log($"recieved click from index {index}");
        int token=gameEngine.ValidateMove(index);
        if(token > -1)
        {
            PerformMove(index,token);
            return;
        }

        Debug.Log("Invalid");
    }

    void PerformMove(int index,int token)
    {
        
        if (gameMode == GameMode.PvP && token == playerIndex)
        {
            // You need this call to update the Go server state!
            NetworkManager.Instance.SendMove(index);
        }
        Debug.Log($" token: {token}.  playerToken{playerIndex}");
        int result=gameEngine.PerformMove(index,token);
        UIController.PerformMove(index,token);

        if (result != -1)
        {
            GameOver(result);   
            return;
        }
        
        if (gameMode==GameMode.PvB && !gameEngine.IsPlayerTurn())
        {
            MakeBotMove(gameEngine.currTurn);
        }
    }

    void MakeBotMove(int token)
    {
        StartCoroutine(RunAfterDelay(1.0f,()=>{
            int move= BotMove(token);
            PerformMove(move,token);
            }
        ));
    }

    
    IEnumerator RunAfterDelay(float delay, Action task)
    {
        yield return new WaitForSeconds(delay);
        task?.Invoke();
    }

    void OnOnlineGameStart(GameStartPayload data)
    {
        playerIndex = data.playerIndex;
        turn = data.turn;
        gameEngine.SetPlayerIndex(playerIndex);
        
        UIController.SetPlayerToken(data.playerIndex);
        
        if (turn==playerIndex)
        {
            Debug.Log("My turn to start!");
        }
        else
        {
            Debug.Log("Waiting for opponent...");
        }
    }

    void OnOnlineOpponentMove(BroadcastMovePayload data)
    {
        
        Debug.Log($"Opponent played at index {data.index}");
        PerformMove(data.index, data.playerIndex);
    }

    void OnOnlineGameOver(GameOverPayload data)
    {
        Debug.Log($"Game Over! Winner: {data.winner}");
        
        //ticTacToe.IsGameEnded = true;
        //ticTacToe.Winner = data.winner;
        
        if (data.winner == playerIndex)
        {
            Debug.Log("You won!");
        }
        else if (data.winner == -1)
        {
            Debug.Log("Draw!");
        }
        else
        {
            Debug.Log("You lost!");
        }
        GameOver(data.winner);
    }

    public int BotMove(int token)
    {
        int botID = token;
        int opponentID = (botID == 0) ? 1 : 0;

        int winningMove = FindStrategicMove(botID);
        if (winningMove != -1) return winningMove;

        int blockingMove = FindStrategicMove(opponentID);
        if (blockingMove != -1) return blockingMove;

        if (side % 2 != 0)
        {
            int centerIndex = (side * side) / 2;
            if (gameEngine.GetElementFromMatrix(centerIndex) == -1) return centerIndex;
        }

        return GetRandomAvailableMove();
    }

    int FindStrategicMove(int playerToCheck)
    {
        for (int i = 0; i < gridLen; i++)
        {
            if (gameEngine.GetElementFromMatrix(i) == -1)
            {
                gameEngine.SetElementInMatrix(i,playerToCheck);
                bool isWinning = (gameEngine.CheckGame(i, playerToCheck) == playerToCheck);
                gameEngine.SetElementInMatrix(i,-1);    
                if (isWinning) return i;
            }
        }
        return -1;
    }

    int GetRandomAvailableMove()
    {
        List<int> availableMoves = new List<int>();
        for (int i = 0; i < gridLen; i++)
        {
            if (gameEngine.GetElementFromMatrix(i) == -1) {
                availableMoves.Add(i);
            }
        }
        if (availableMoves.Count > 0) 
            return availableMoves[Random.Range(0, availableMoves.Count)];
        return -1;
    }

    public void OnForfeit()
    {
        if (gameMode!=GameMode.PvP)
        {
            gameEngine.UpdateTurn();
            int token=gameEngine.GetCurrTurn();
            GameOver(token);
            return;
        }

        NetworkManager.Instance.SendForfeit();

    }
}