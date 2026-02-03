using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject[] grid;
    [SerializeField]Image TurnImage;
    [SerializeField] GameObject IndicateMoveToken;
    [SerializeField] Board board;

    [SerializeField] GameObject Forfeit;


    public AudioSource source;
    public AudioClip clickSound;

    //initial vars
    public int side;


    // Game State vars
    int moveCount;
    int currTurn;

    [SerializeField] Sprite[] tokens;
    
    
    
    [Header("Timer")]
    [SerializeField] Image timerImage;
    [SerializeField]Timer timer;
    
    Image playerTokenImage;
    public bool TimeState
    {
        get
        {
            return timer.outOfTime;
        }
    }
    
    
    void Awake()
    {
        side=(int)Math.Round(Math.Sqrt(grid.Length));
    }

    void Start()
    {
        displayCurrPlayerImage();
        currTurn=0;
        moveCount=0;
        if (GameSetting.currentGameMode == GameMode.PvP)
        {
            timerImage.enabled=false;    
        }
    }

    public void GenerateBoard(int side,Action<int> OnCellClick)
    {
        board.GenerateBoard(side,OnCellClick);
    }

    public void SetPlayerToken(int playerToken)
    {
        playerTokenImage = IndicateMoveToken.GetComponentInChildren<Image>();
        playerTokenImage.sprite=tokens[playerToken];
    }
    // Update is called once per frame
    void Update()
    {
        timerImage.fillAmount=timer.fillFraction;
        
    }
    
    public bool getTimeState()
    {
        return timer.outOfTime;
    }

    public void PerformMove(int index,int token)
    {
        board.PerformMove(index,token);
        turnChange();
        displayCurrPlayerImage();
    }

    public int getMoveCount()
    {
        return moveCount;
    }

    public void incrementMoveCount()
    {
        moveCount++;
    } 

    public void changeGameImages(int index) // controller 
    {
        changeTileImage(index,currTurn);
        //PrintMatrix(side);
        Debug.Log(currTurn);
        SetButtonState(index,false);

        timer.ResetTimer();
    }

    public void changeTileImage(int index,int currTurn)
    {
        Image box;
        box=grid[index].GetComponent<Image>();
        box.sprite=tokens[currTurn];
    }

    void SetButtonState(int index,bool state)
    {
        Button buttonClicked;
        buttonClicked=grid[index].GetComponent<Button>();
        buttonClicked.interactable=state;
    }

    public void turnChange()
    {
        currTurn=1-currTurn;
    }

    public int getCurrTurn()
    {
        return currTurn;
    }

    public void SetPlayerTokenIndicator(bool state)
    {
        IndicateMoveToken.SetActive(state);
    }

    public void displayCurrPlayerImage()
    {
        TurnImage.sprite=tokens[currTurn];
    }

    public void PlayAudio()
    {
        source.PlayOneShot(clickSound);
    }

    //Helper Functions
    
}
